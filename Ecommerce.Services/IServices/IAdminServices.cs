using Ecommerce.Shared.DTOs;

namespace Ecommerce.Services.IServices
{
    public interface IAdminServices
    {
        Task<ApiResponse> CreateAdmin(LoginAdminDto admin);
        Task<ApiResponse> LoginAdmin(LoginAdminDto admin);
    }
}
