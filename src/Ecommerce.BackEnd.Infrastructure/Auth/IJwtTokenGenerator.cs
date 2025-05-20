using Ecommerce.BackEnd.Shared.AuthDto;
using ROP;

namespace Ecommerce.BackEnd.Infrastructure.Auth
{
    interface IJwtTokenGenerator
    {
        Result<JwtResponseDto> GenerateJwtToken(string role, string userId, string email, string? existingJti);
        Result<RefreshTokenDto> GenerateRefreshToken(string tokenId, DateTime expiryDate, string userId);
        Result<TokensGeneratorResponseDto> GenerateNewTokens(string accessToken, RefreshTokenDto refreshToken);
    }
}
