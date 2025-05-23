using Ecommerce.BackEnd.Data.Models;
using ROP;

namespace Ecommerce.BackEnd.Data.IRepository
{
    public interface IUserRepository
    {
        Task<Result<Unit>> UserRegister(ApplicationUser user, string password, VerificationCode verificationCode);
        Task<Result<VerificationCode>> UserVerificationCode(string id, string code);
        Task<Result<VerificationCode>> UpdateVerificationCode(VerificationCode verificationCode);
        Task<Result<Unit>> UserConfirmAndRevokeVerificationCode(ApplicationUser applicationUser, VerificationCode verificationCode);
        Task<Result<ApplicationUser>> UserLogin(string email, string password);
        Task<Result<bool>> IsEmailConfirm(string email);
        Task<Result<List<string>>> GetRole(ApplicationUser user);
        Task<Result<Unit>> DeleteUser(ApplicationUser user);
        Task<Result<bool>> DoesUserExistById(string id);
        Task<Result<bool>> DoesUserExistByEmail(string email);
        Task<Result<ApplicationUser>> GetUserByEmail(string email);
        Task<Result<ApplicationUser>> GetUserById(string id);
        Task<Result<Unit>> CreateRefreshToken(RefreshToken refreshToken);
        Task<Result<Unit>> UpdateRefreshToken(RefreshToken token);
        Task<Result<RefreshToken>> TokenIsValid(string refreshToken);
        Task<bool> Save();
    }
}
