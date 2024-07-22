using Ecommerce.Shared.DTOs;

namespace Ecommerce.Services.IServices
{
    public interface IUserServices
    {
        Task<ApiResponse> CreateUser(RegisterUserDto createUserDto);
        Task<ApiResponse> LoginUser(RegisterUserDto createUserDto);
        Task<ApiResponse> VerifyAndGenerateToken(TokenRequestDto token);
        Task<ApiResponse> ConfirmEmail(UserCodeConfirmDto userCode);
        Task<ApiResponse> NewVerificationCode(string id);
    }
}
