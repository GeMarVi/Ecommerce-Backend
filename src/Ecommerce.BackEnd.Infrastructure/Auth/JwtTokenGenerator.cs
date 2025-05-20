using Ecommerce.BackEnd.Infrastructure.Auth;
using Ecommerce.BackEnd.Shared.AuthDto;
using Ecommerce.BackEnd.Shared.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ROP;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtConfig _jwtConfig;
    private readonly JwtSecurityTokenHandler _tokenHandler;

    public JwtTokenGenerator(IOptions<JwtConfig> jwtConfig)
    {
        _jwtConfig = jwtConfig.Value;
        _tokenHandler = new JwtSecurityTokenHandler();
    }

    public Result<JwtResponseDto> GenerateJwtToken(string role, string userId, string email, string? existingJti = null)
    {
        var jti = existingJti ?? Guid.NewGuid().ToString();

        var claims = new List<Claim>
        {
            new("id", userId),
            new("role", role),
            new("email", email),
            new(JwtRegisteredClaimNames.Jti, jti),
            new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(1),
            SigningCredentials = credentials
        };

        var token = _tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = _tokenHandler.WriteToken(token);

        return new JwtResponseDto
        {
            Token = tokenString,
            TokenId = jti
        };
    }

    public Result<RefreshTokenDto> GenerateRefreshToken(string jwtId, DateTime expiry, string userId)
    {
        return new RefreshTokenDto
        {
            JwtId = jwtId,
            Token = Guid.NewGuid().ToString("N"),
            AddedDate = DateTime.UtcNow,
            ExpiryDate = expiry,
            IsRevoked = false,
            IsUsed = false,
            UserId = userId
        };
    }

    public Result<TokensGeneratorResponseDto> GenerateNewTokens(string expiredAccessToken, RefreshTokenDto usedRefreshToken)
    {
        var principal = GetPrincipalFromToken(expiredAccessToken, out var jwtSecurityToken);
        if (principal == null || jwtSecurityToken == null)
            return Result.Failure<TokensGeneratorResponseDto>("Invalid or malformed token.");

        // Extract claims from expired token
        var userId = principal.FindFirst("id")?.Value;
        var role = principal.FindFirst("role")?.Value;
        var email = principal.FindFirst("email")?.Value;

        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(role) || string.IsNullOrWhiteSpace(email))
            return Result.Failure<TokensGeneratorResponseDto>("Missing claims in token.");

        // Generate new token with new jti
        var newJti = Guid.NewGuid().ToString();
        var accessTokenResult = GenerateJwtToken(role, userId, email, newJti);
        if (!accessTokenResult.Success)
            return Result.Failure<TokensGeneratorResponseDto>(accessTokenResult.Errors);

        var refreshTokenResult = GenerateRefreshToken(newJti, usedRefreshToken.ExpiryDate, userId);
        if (!refreshTokenResult.Success)
            return Result.Failure<TokensGeneratorResponseDto>(refreshTokenResult.Errors);

        return new TokensGeneratorResponseDto
        {
            Token = accessTokenResult.Value.Token,
            RefreshToken = refreshTokenResult.Value
        };
    }

    private ClaimsPrincipal? GetPrincipalFromToken(string token, out JwtSecurityToken? validatedJwt)
    {
        validatedJwt = null;

        var tokenValidationParams = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Secret)),
            ValidateLifetime = false, // Allow expired tokens
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            var principal = _tokenHandler.ValidateToken(token, tokenValidationParams, out var validatedToken);
            validatedJwt = validatedToken as JwtSecurityToken;

            if (validatedJwt == null || !validatedJwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                return null;

            return principal;
        }
        catch
        {
            return null;
        }
    }
}
