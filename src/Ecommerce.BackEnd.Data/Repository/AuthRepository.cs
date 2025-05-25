using Ecommerce.BackEnd.Data.Data;
using Ecommerce.BackEnd.Data.IRepository;
using Ecommerce.BackEnd.Data.Models;
using Ecommerce.BackEnd.Shared.AuthDto;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ROP;

namespace Ecommerce.BackEnd.Data.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AuthRepository> _logger;

        public AuthRepository(
            ApplicationDbContext db,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<AuthRepository> logger)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task<Result<Unit>> RegisterIdentity(RegisterData<ApplicationUser, VerificationCode> userRegisterData)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var result = await _userManager.CreateAsync(userRegisterData.Identity, userRegisterData.Password);
                if (!result.Succeeded)
                {
                    await transaction.RollbackAsync();
                    return Result.Failure<Unit>("An unexpected error occurred.");
                }

                await _userManager.AddToRoleAsync(userRegisterData.Identity, userRegisterData.Role);
                userRegisterData.VerificationCode.User_Id = userRegisterData.Identity.Id;
                await _db.VerificationCodes.AddAsync(userRegisterData.VerificationCode);
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
                return Result.Success();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error in UserRegister");
                return Result.Failure<Unit>("An unexpected error occurred.");
            }
        }

        public async Task<Result<Unit>> DeleteIdentity(ApplicationUser user)
        {
            try
            {
                _db.Users.Remove(user);
                var saved = await Save();
                return saved ? Result.Success() : Result.Failure("An unexpected error occurred.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteUser");
                return Result.Failure<Unit>("An unexpected error occurred.");
            }
        }

        public async Task<Result<ApplicationUser>> GetIdentityByEmail(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                return Result.Success(user!);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetUserByEmail");
                return Result.Failure<ApplicationUser>("An unexpected error occurred.");
            }
        }

        public async Task<Result<ApplicationUser>> GetIdentityById(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                return Result.Success(user!);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetUserById");
                return Result.Failure<ApplicationUser>("An unexpected error occurred.");
            }
        }

        //// ─── Verification Code ────────────────────────────────

        public async Task<Result<VerificationCode>> GetVerificationCode(string id, string code)
        {
            try
            {
                var result = await _db.VerificationCodes
                    .AsNoTracking()
                    .Where(p => p.User_Id == id && p.Code == code)
                    .FirstOrDefaultAsync();

                return Result.Success(result!);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetVerificationCode");
                return Result.Failure<VerificationCode>("An unexpected error occurred.");
            }
        }

        public async Task<Result<Unit>> ConfirmIdentityAndRevokeCode(ApplicationUser user, VerificationCode verificationCode)
        {
            try
            {
                using var transaction = await _db.Database.BeginTransactionAsync();

                await _userManager.UpdateAsync(user);
                _db.VerificationCodes.Remove(verificationCode);
                await _db.SaveChangesAsync();

                await transaction.CommitAsync();
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ConfirmIdentityAndRevokeCode");
                return Result.Failure<Unit>("An unexpected error occurred.");
            }
        }
        public async Task<Result<VerificationCode>> UpdateVerificationCode(VerificationCode verificationCode)
        {
            try
            {
                var result = await _db.VerificationCodes.FirstOrDefaultAsync(v => v.User_Id == verificationCode.User_Id);

                result!.ExpirationTime = verificationCode.ExpirationTime;
                result.Code = verificationCode.Code;

                _db.VerificationCodes.Update(result);
                await _db.SaveChangesAsync();

                return Result.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateVerificationCode");
                return Result.Failure<VerificationCode>("An unexpected error occurred.");
            }
        }

        //// ─── Authentication ───────────────────────────────────

        public async Task<Result<ApplicationUser>> LoginIdentity(string email, string password)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                var isValid = await _userManager.CheckPasswordAsync(user!, password);
                return isValid ? Result.Success(user!) : Result.Failure<ApplicationUser>("An unexpected error occurred.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LoginIdentity");
                return Result.Failure<ApplicationUser>("An unexpected error occurred.");
            }
        }

        public async Task<Result<bool>> IsEmailConfirmed(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                var confirm = await _userManager.IsEmailConfirmedAsync(user!);
                return Result.Success(confirm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in IsEmailConfirmed");
                return Result.Failure<bool>("An unexpected error occurred.");
            }
        }

        //// ─── Roles ─────────────────────────────────────────────
        public async Task<Result<List<string>>> GetIdentityRoles(ApplicationUser user)
        {
            try
            {
                var roles = await _userManager.GetRolesAsync(user);
                return roles.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetIdentityRoles");
                return Result.Failure<List<string>>("An unexpected error occurred.");
            }
        }

        public async Task<Result<Unit>> CreateNewRoleAsync(IdentityRole role)
        {
            try
            {
                var result = await _roleManager.CreateAsync(role);
                return Result.Success();
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Error in CreateNewRoleAsync");
                return Result.Failure<Unit>("An unexpected error occurred.");
            }
        }

        //// ─── Existence Checks ─────────────────────────────────

        public async Task<Result<bool>> IdentityExistsById(string id)
        {
            try
            {
                var exists = await _userManager.FindByIdAsync(id) != null;
                return Result.Success(exists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in IdentityUserExistById");
                return Result.Failure<bool>("An unexpected error occurred.");
            }
        }

        public async Task<Result<bool>> IdentityExistsByEmail(string email)
        {
            try
            {
                var exists = await _userManager.FindByEmailAsync(email) != null;
                return Result.Success(exists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in IdentityExistsByEmail");
                return Result.Failure<bool>("An unexpected error occurred.");
            }
        }

        //// ─── Refresh Tokens ───────────────────────────────────

        public async Task<Result<Unit>> CreateRefreshToken(RefreshToken refreshToken)
        {
            try
            {
                await _db.RefreshTokens.AddAsync(refreshToken);
                var saved = await Save();
                return saved ? Result.Success() : Result.Failure("An unexpected error occurred.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateRefreshToken");
                return Result.Failure<Unit>("An unexpected error occurred.");
            }
        }

        public async Task<Result<Unit>> UpdateRefreshToken(RefreshToken token)
        {
            try
            {
                var tokenDb = await _db.RefreshTokens.FirstOrDefaultAsync(t => t.Id == token.Id);
                _db.RefreshTokens.Update(tokenDb!);
                var saved = await Save();
                return saved ? Result.Success() : Result.Failure("An unexpected error occurred.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateRefreshToken");
                return Result.Failure<Unit>("An unexpected error occurred.");
            }
        }

        public async Task<Result<RefreshToken>> ValidateRefreshToken(string refreshToken)
        {
            try
            {
                var result = await _db.RefreshTokens.FirstOrDefaultAsync(t => t.Token == refreshToken);
                return Result.Success(result!);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ValidateRefreshToken");
                return Result.Failure<RefreshToken>("An unexpected error occurred.");
            }
        }

        public async Task<bool> Save()
        {
            return await _db.SaveChangesAsync() > 0;
        }
    }
}
