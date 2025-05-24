using Ecommerce.BackEnd.Data.Data;
using Ecommerce.BackEnd.Data.IRepository;
using Ecommerce.BackEnd.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ROP;

namespace Ecommerce.BackEnd.Data.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserRepository(ApplicationDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<Result<Unit>> UserRegister(ApplicationUser user, string password, VerificationCode verificationCode)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var result = await _userManager.CreateAsync(user, password);
                if (!result.Succeeded)
                {
                    await transaction.RollbackAsync();
                    return Result.Failure<Unit>("Error creating user: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                }

                await _userManager.AddToRoleAsync(user, "User");

                verificationCode.User_Id = user.Id;
                await _db.VerificationCodes.AddAsync(verificationCode);
                await _db.SaveChangesAsync();

                await transaction.CommitAsync();
                return Result.Success();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Result.Failure<Unit>("An error occurred: " + ex.Message);
            }
        }

        public async Task<Result<VerificationCode>> UserVerificationCode(string id, string code)
        {
            try
            {
                var result = await _db.VerificationCodes.AsNoTracking()
                    .Where(p => p.User_Id == id && p.Code == code)
                    .FirstOrDefaultAsync();
                return result != null ? Result.Success(result) 
                    : Result.Failure<VerificationCode>("Verification code not found");
            }
            catch (Exception ex)
            {
                return Result.Failure<VerificationCode>(ex.Message);
            }
        }

        public async Task<Result<VerificationCode>> UpdateVerificationCode(VerificationCode verificationCode)
        {
            try
            {
                var result = await _db.VerificationCodes.FirstOrDefaultAsync(v => v.User_Id == verificationCode.User_Id);
                if (result == null)
                    return Result.Failure<VerificationCode>("Verification code not found");

                result.ExpirationTime = verificationCode.ExpirationTime;
                result.Code = verificationCode.Code;
                _db.VerificationCodes.Update(result);
                await _db.SaveChangesAsync();
                return Result.Success(result);
            }
            catch (Exception ex)
            {
                return Result.Failure<VerificationCode>(ex.Message);
            }
        }

        public async Task<Result<Unit>> UserConfirmAndRevokeVerificationCode(ApplicationUser user, VerificationCode verificationCode)
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
            catch
            {
                return Result.Failure<Unit>("An unexpected error occurred.");
            }
        }

        public async Task<Result<ApplicationUser>> UserLogin(string email, string password)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                    return Result.Failure<ApplicationUser>("User doest not exist");

                if (!await _userManager.CheckPasswordAsync(user, password))
                    return Result.Failure<ApplicationUser>("Invalid credentials");
                return Result.Success(user);
            }
            catch (Exception ex)
            {
                return Result.Failure<ApplicationUser>(ex.Message);
            }
        }

        public async Task<Result<bool>> IsEmailConfirm(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                    return Result.Failure<bool>("User doest not exist");

                var confirm = await _userManager.IsEmailConfirmedAsync(user);
               
                return confirm;
            }
            catch (Exception ex) { 
                return Result.Failure<bool>("Something went wrong. Please try again later.");
            }
        }

        public async Task<Result<List<string>>> GetRole(ApplicationUser user)
        {
            try
            {
                var roles = await _userManager.GetRolesAsync(user);
                return Result.Success(roles.ToList());
            }
            catch (Exception ex)
            {
                return Result.Failure<List<string>>(ex.Message);
            }
        }

        public async Task<Result<bool>> DoesUserExistById(string id)
        {
            try
            {
                var exists = await _userManager.FindByIdAsync(id) != null;
                if(exists)
                return Result.Success<bool>(true);

                return Result.Success<bool>(false);
            }
            catch (Exception ex)
            {
                return Result.Failure<bool>("Error en el servidor al buscar un usuario");
            }
        }

        public async Task<Result<bool>> DoesUserExistByEmail(string email)
        {
            try
            {
                var exists = await _userManager.FindByEmailAsync(email) != null;
                if (exists)
                    return true;

                return false;
            }
            catch (Exception ex)
            {
                return Result.Failure<bool>("Error en el servidor al buscar un usuario");
            }
        }

        public async Task<Result<ApplicationUser>> GetUserById(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                return user != null ? Result.Success(user) : Result.Failure<ApplicationUser>("User not found");
            }
            catch (Exception ex)
            {
                return Result.Failure<ApplicationUser>(ex.Message);
            }
        }

        public async Task<Result<Unit>> DeleteUser(ApplicationUser user)
        {
            try
            {
                _db.Users.Remove(user);
                var saved = await Save();
                return saved ? Result.Success() : Result.Failure("Failed to delete user");
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.Message);
            }
        }

        public async Task<Result<ApplicationUser>> GetUserByEmail(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                return user != null ? Result.Success(user) : Result.Failure<ApplicationUser>("User not found");
            }
            catch (Exception ex)
            {
                return Result.Failure<ApplicationUser>(ex.Message);
            }
        }

        public async Task<Result<Unit>> CreateRefreshToken(RefreshToken refreshToken)
        {
            try
            {
                await _db.RefreshTokens.AddAsync(refreshToken);
                var saved = await Save();
                return saved ? Result.Success() : Result.Failure("Failed to create refresh token");
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.Message);
            }
        }

        public async Task<Result<Unit>> UpdateRefreshToken(RefreshToken token)
        {
            try
            {
                var tokenDb = await _db.RefreshTokens.FirstOrDefaultAsync(t => t.Id == token.Id);
                if (tokenDb == null)
                    return Result.Failure("Refresh token not found");

                _db.RefreshTokens.Update(tokenDb);
                var saved = await Save();
                return saved ? Result.Success() : Result.Failure("Failed to update refresh token");
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.Message);
            }
        }

        public async Task<Result<RefreshToken>> TokenIsValid(string refreshToken)
        {
            try
            {
                var result = await _db.RefreshTokens.FirstOrDefaultAsync(t => t.Token == refreshToken);
                return result != null ? Result.Success(result) : Result.Failure<RefreshToken>("Invalid refresh token");
            }
            catch (Exception ex)
            {
                return Result.Failure<RefreshToken>(ex.Message);
            }
        }

        public async Task<bool> Save()
        {
            return await _db.SaveChangesAsync() > 0 ? true : false;
        }
    }
}
