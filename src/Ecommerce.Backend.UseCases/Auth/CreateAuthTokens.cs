using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ROP;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Ecommerce.BackEnd.UseCases.Auth
{
    public class CreateAuthTokens : IGenerateJWTServices
    {
        private readonly IGenerateTokenDependencies _dependencies;
        private readonly JwtConfig _jwtConfig;
        private readonly TokenValidationParameters _tokenValidationParameters;
        public CreateAuthTokens(IGenerateTokenDependencies dependencies, IOptions<JwtConfig> jwtConfig, TokenValidationParameters tokenValidationParameters)
        {
            _dependencies = dependencies;
            _jwtConfig = jwtConfig.Value;
            _tokenValidationParameters = tokenValidationParameters;

        }

        public async Task<Result<JwtGeneratorResponseDto>> GenerateToken(ApplicationUser applicationUser, string? existingJti = null)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtConfig.Secret);

            var roles = await _dependencies.GetRoles(applicationUser);
            if (!roles.Success)
                return Result.Failure<JwtGeneratorResponseDto>(roles.Errors);

            var claims = new List<Claim>
{
                new Claim(ClaimTypes.NameIdentifier, applicationUser.Id),
                new Claim(JwtRegisteredClaimNames.Email, applicationUser.Email),
                new Claim(JwtRegisteredClaimNames.Jti, existingJti ?? Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            claims.AddRange(roles.Value.Select(role => new Claim(ClaimTypes.Role, role)));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(1), 
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);

            // Crear Refresh Token solo si no es una renovación
            var refreshToken = existingJti == null
                ? await CreateRefreshToken(token.Id, DateTime.UtcNow.AddDays(30), applicationUser.Id)
                : null;

            if (!refreshToken.Success && existingJti == null)
                return Result.Failure<JwtGeneratorResponseDto>(refreshToken.Errors);

            return new JwtGeneratorResponseDto
            {
                Token = jwtTokenHandler.WriteToken(token),
                RefreshToken = refreshToken.Value ?? string.Empty 
            };
        }

        private async Task<Result<string>> CreateRefreshToken(string tokenId, DateTime expiryDate, string userId)
        {
            var refreshToken = new RefreshToken
            {
                JwtId = tokenId,
                Token = Guid.NewGuid().ToString("N"),
                AddedDate = DateTime.UtcNow,
                ExpiryDate = expiryDate,
                IsRevoked = false,
                IsUsed = false,
                UserId = userId
            };

            var result = await _dependencies.CreateRefreshTokenDb(refreshToken);
            if (!result.Success)
                return Result.Failure<string>(result.Errors);

            return refreshToken.Token;
        }

        public async Task<Result<JwtGeneratorResponseDto>> RefreshTokenAsync(string accessToken, string refreshToken)
        {
            var storedRefreshToken = await _dependencies.TokenIsValid(refreshToken);
            if (!storedRefreshToken.Success)
                return Result.Failure<JwtGeneratorResponseDto>(storedRefreshToken.Errors);

            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtConfig.Secret);

            SecurityToken validatedToken;
            var tokenValidationParams = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ClockSkew = TimeSpan.Zero
            };

            ClaimsPrincipal principal = jwtTokenHandler.ValidateToken(accessToken, tokenValidationParams, out validatedToken);
            var jwtSecurityToken = validatedToken as JwtSecurityToken;

            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                return Result.Failure<JwtGeneratorResponseDto>("Invalid Token");

            var userId = principal.FindFirst("Id")?.Value;
            var jti = principal.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(jti))
                return Result.Failure<JwtGeneratorResponseDto>("Invalid Token");

            if (storedRefreshToken.Value.IsRevoked
                || storedRefreshToken.Value.IsUsed
                || storedRefreshToken.Value.ExpiryDate < DateTime.UtcNow
                || storedRefreshToken.Value.JwtId != jti)
                return Result.Failure<JwtGeneratorResponseDto>("Invalid Refresh Token");

            storedRefreshToken.Value.IsRevoked = true;
            storedRefreshToken.Value.IsUsed = true;

            var updateToken = await _dependencies.UpdateRefreshToken(storedRefreshToken.Value);
            if (!updateToken.Success)
                return Result.Failure<JwtGeneratorResponseDto>(updateToken.Errors);

            var user = await _dependencies.GetUser(userId);
            if (!user.Success)
                return Result.Failure<JwtGeneratorResponseDto>(storedRefreshToken.Errors);

            var newToken = await GenerateToken(user.Value, jti);
            if (!newToken.Success)
                return Result.Failure<JwtGeneratorResponseDto>(storedRefreshToken.Errors);

            var newRefreshToken = await CreateRefreshToken(storedRefreshToken.Value.JwtId, storedRefreshToken.Value.ExpiryDate, user.Value.Id);
            if (!newRefreshToken.Success)
                return Result.Failure<JwtGeneratorResponseDto>(newRefreshToken.Errors);

            var result = new JwtGeneratorResponseDto
            {
                Token = newToken.Value.Token,
                RefreshToken = newRefreshToken.Value
            };

            return result;
        }
    }
}
