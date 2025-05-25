using Ecommerce.BackEnd.Data.Models;
using Ecommerce.BackEnd.Shared.AuthDto;
using Microsoft.AspNetCore.Identity;
using ROP;

namespace Ecommerce.BackEnd.Data.IRepository
{
    public interface IAuthRepository
    {
        // ─── Identity CRUD ─────────────────────────────────────
        Task<Result<Unit>> RegisterIdentity(RegisterData<ApplicationUser, VerificationCode> userRegisterData);
        Task<Result<Unit>> DeleteIdentity(ApplicationUser user);
        Task<Result<ApplicationUser>> GetIdentityByEmail(string email);
        Task<Result<ApplicationUser>> GetIdentityById(string id);

        // ─── Verification Code ────────────────────────────────
        Task<Result<VerificationCode>> GetVerificationCode(string id, string code);
        Task<Result<Unit>> ConfirmIdentityAndRevokeCode(ApplicationUser applicationUser, VerificationCode verificationCode);
        Task<Result<VerificationCode>> UpdateVerificationCode(VerificationCode verificationCode);

        // ─── Authentication ───────────────────────────────────
        Task<Result<ApplicationUser>> LoginIdentity(string email, string password);
        Task<Result<bool>> IsEmailConfirmed(string email);

        // ─── Roles ─────────────────────────────────────────────
        Task<Result<List<string>>> GetIdentityRoles(ApplicationUser user);
        Task<Result<Unit>> CreateNewRoleAsync(IdentityRole identity);

        // ─── Existence Checks ─────────────────────────────────
        Task<Result<bool>> IdentityExistsByEmail(string email);
        Task<Result<bool>> IdentityExistsById(string id);

        // ─── Refresh Tokens ───────────────────────────────────
        Task<Result<Unit>> CreateRefreshToken(RefreshToken refreshToken);
        Task<Result<Unit>> UpdateRefreshToken(RefreshToken token);
        Task<Result<RefreshToken>> ValidateRefreshToken(string refreshToken);

        // ─── Persistence ──────────────────────────────────────
        Task<bool> Save();
    }
}
