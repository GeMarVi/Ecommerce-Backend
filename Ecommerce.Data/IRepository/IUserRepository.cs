using Ecommerce.Model;

namespace Ecommerce.Data.IRepository
{
    public interface IUserRepository
    {
        Task<bool> CreateRefreshToken(RefreshToken refreshToken);
        Task<bool> UpdateRefreshToken(int Id);
        Task<RefreshToken> TokenIsValid(string refreshToken);
        Task<ApplicationUser> GetUserByEmail(string Email);
        Task<ApplicationUser> GetUserById(string Id);
        Task<bool> DoesUserExistById(string id);
        Task<bool> DoesUserExistByEmail(string email);
        Task<ApplicationUser> UserLogin(string email, string password);
        Task<(ApplicationUser, string)> UserRegister(ApplicationUser user, string password, VerificationCode verificationCode);
        Task<VerificationCode> UserVerificationCode(string id);
        Task<VerificationCode> UpdateVerificationCode(VerificationCode verificationCode);
        Task<bool> DeleteVerificationCode(VerificationCode verificationCode);
        Task<bool> UserConfirm(ApplicationUser user);
        Task<List<string>> GetRole(ApplicationUser user);
        Task<bool> DeleteUser(ApplicationUser user);
        Task<bool> Save();
    }
}
