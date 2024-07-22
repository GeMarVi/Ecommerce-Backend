using Ecommerce.Model;
using Ecommerce.Shared.DTOs;

namespace Ecommerce.Services.IServices
{
    public interface IGenerateJWTServices
    {
        Task<JwtGeneratorResponseDto> GenerateTokenAsync(ApplicationUser applicationUser);
        Task<ApiResponse> VerifyAndGenerateTokenAsync(TokenRequestDto tokenRequest);
    }

}
