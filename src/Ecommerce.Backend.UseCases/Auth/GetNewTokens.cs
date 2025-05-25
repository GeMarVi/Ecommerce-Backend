using Ecommerce.BackEnd.Data.IRepository;
using Ecommerce.BackEnd.Data.Models;
using Ecommerce.BackEnd.Infrastructure.Auth;
using Ecommerce.BackEnd.Shared.AuthDto;
using Ecommerce.BackEnd.Shared.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ROP;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Ecommerce.BackEnd.UseCases.Auth
{
    public class GetNewTokens
    {
        private readonly IAuthRepository _user;
        private readonly JwtConfig _jwtConfig;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public GetNewTokens(IAuthRepository user, IOptions<JwtConfig> jwtConfig, IJwtTokenGenerator jwtTokenGenerator)
        {
            _user = user;
            _jwtConfig = jwtConfig.Value;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<Result<TokensResponseDto>> Execute(TokensRequestDto tokensRequestDto)
        {
            return await ValidateAccessToken(tokensRequestDto.Token)
                .Async()
                .Bind(ValidateTokenClaims)
                .Bind(id => GetRefreshTokenDb(tokensRequestDto.RefreshToken, id))
                .Bind(UpdateStoredRefreshToken)
                .Bind(CreateNewRefreshToken)
                .Bind(SaveNewRefreshToken)
                .Combine(GetUser)
                .Bind(CreateAccessToken);
        }

        private Result<ClaimsPrincipal> ValidateAccessToken(string accessToken)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtConfig.Secret);
            var tokenValidationParams = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ClockSkew = TimeSpan.Zero
            };

            ClaimsPrincipal principal = jwtTokenHandler.ValidateToken(accessToken, tokenValidationParams, out var validatedToken);
            var jwtSecurityToken = validatedToken as JwtSecurityToken;
            return jwtSecurityToken != null && jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase)
                ? Result.Success(principal)
                : Result.Failure<ClaimsPrincipal>("Invalid Token");
        }

        private Result<string> ValidateTokenClaims(ClaimsPrincipal principal)
        {
            var userId = principal.FindFirst("id")?.Value;
            var jti = principal.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
            return !string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(jti)
                ? Result.Success(jti)
                : Result.Failure<string>("Invalid Token");
        }

        private async Task<Result<RefreshToken>> GetRefreshTokenDb(string refreshToken, string jti)
        {
            var result = await _user.ValidateRefreshToken(refreshToken);
            if (!result.Success)
                return Result.Failure<RefreshToken>(result.Errors);

            if(result.Value.ExpiryDate < DateTime.UtcNow)
                return Result.Failure<RefreshToken>("Refresh Token is expired");

            return result.Value.IsRevoked
                || result.Value.IsUsed
                || result.Value.JwtId != jti
               ? Result.Failure<RefreshToken>("Invalid Refresh Token")
               : Result.Success(result.Value);
        }

        private async Task<Result<RefreshToken>> UpdateStoredRefreshToken(RefreshToken storedToken)
        {
            storedToken.IsRevoked = true;
            storedToken.IsUsed = true;
            var updateResult = await _user.UpdateRefreshToken(storedToken);
            return updateResult.Success
                ? Result.Success(storedToken)
                : Result.Failure<RefreshToken>(updateResult.Errors);
        }

        private Result<RefreshTokenDto> CreateNewRefreshToken(RefreshToken refreshToken)
        {
            var newRefreshToken = _jwtTokenGenerator.GenerateRefreshToken(refreshToken.JwtId, refreshToken.ExpiryDate, refreshToken.UserId);
            if (!newRefreshToken.Success)
                return Result.Failure<RefreshTokenDto>(newRefreshToken.Errors);

            return newRefreshToken.Value;
        } 

        private async Task<Result<RefreshToken>> SaveNewRefreshToken(RefreshTokenDto refreshToken)
        {
            var newRefresh = Mappers.ToRefreshToken(refreshToken);
            var result = await _user.CreateRefreshToken(newRefresh);
            if (!result.Success)
                return Result.Failure<RefreshToken>(result.Errors);

            return newRefresh;
        }

        private async Task<Result<ApplicationUser>> GetUser(RefreshToken refreshToken)
        {
            var result = await _user.GetIdentityById(refreshToken.UserId);
            if (!result.Success)
                return Result.Failure<ApplicationUser>(result.Errors);
            
            if(result.Value == null)
                return Result.Failure<ApplicationUser>("Refresh token user not found.");

            if (result.Value.Id != refreshToken.UserId)
                return Result.Failure<ApplicationUser>("Refresh token does not correspond to the assigned user.");

            return result.Value;
        }

        private Result<TokensResponseDto> CreateAccessToken((RefreshToken storedToken, ApplicationUser user) args)
        {
            var newToken = _jwtTokenGenerator.GenerateJwtToken("User", args.user.Id, args.user.Email!, args.storedToken.JwtId);
            if (!newToken.Success)
                return Result.Failure<TokensResponseDto>(newToken.Errors);

            return new TokensResponseDto
            {
                Token = newToken.Value.Token,
                RefreshToken = args.storedToken.Token,
            };
        }
    }
}
