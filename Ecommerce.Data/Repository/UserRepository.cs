using Ecommerce.Data.Context;
using Ecommerce.Data.IRepository;
using Ecommerce.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Data.Repository
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

        public async Task<(ApplicationUser, string)> UserRegister(ApplicationUser user, string password, VerificationCode verificationCode)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var result = await _userManager.CreateAsync(user, password);
                if (!result.Succeeded)
                {
                    await transaction.RollbackAsync();
                    return (null, "");
                }

                await _userManager.AddToRoleAsync(user, "User");

                verificationCode.User_Id = user.Id;

                await _db.VerificationCodes.AddAsync(verificationCode);
                await _db.SaveChangesAsync();

                await transaction.CommitAsync();

                return (user, verificationCode.Code);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (null, "");
            }
        }

        public async Task<VerificationCode> UserVerificationCode(string id)
        {
            var result = await _db.VerificationCodes.Where(p => p.User_Id == id).FirstOrDefaultAsync();
            return result;
        }

        public async Task<VerificationCode> UpdateVerificationCode(VerificationCode verificationCode)
        {
            var result = await _db.VerificationCodes.FirstOrDefaultAsync(v => v.User_Id == verificationCode.User_Id);
            result.ExpirationTime = verificationCode.ExpirationTime;
            result.Code = verificationCode.Code;
            _db.VerificationCodes.Update(result);
            var save = await Save();
            if (!save)
            {
                return null;
            }
            return result;
        }

        public async Task<bool> UserConfirm(ApplicationUser user)
        {
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteVerificationCode(VerificationCode verificationCode)
        {
            _db.VerificationCodes.Remove(verificationCode);
            return await Save();
        }

        public async Task<ApplicationUser> UserLogin(string email, string password)
        {
            var user = await GetUserByEmail(email);
            var checkUserAndPass = await _userManager.CheckPasswordAsync(user, password);

            if (!checkUserAndPass)
            {
                return null;
            }
            return user;
        }

    
        public async Task<List<string>> GetRole(ApplicationUser user)
        {
            var role = await _userManager.GetRolesAsync(user);

            var rolesList = new List<string>(role);

            return rolesList;
        }

        public async Task<bool> DeleteUser(ApplicationUser user)
        {
            _db.Users.Remove(user);
            return await Save();
        }

        public async Task<bool> DoesUserExistById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> DoesUserExistByEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                return true;
            }
            return false;
        }

        public async Task<ApplicationUser> GetUserByEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return null;
            }

            return user;
        }

        public async Task<ApplicationUser> GetUserById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return null;
            }
            return user;
        }

        public async Task<bool> CreateRefreshToken(RefreshToken refreshToken)
        {
            await _db.RefreshTokens.AddAsync(refreshToken);
            return await Save();
        }

        public async Task<bool> UpdateRefreshToken(int Id)
        {
            var tokenDb = await _db.RefreshTokens.FirstOrDefaultAsync(t => t.Id == Id);
            tokenDb.IsUsed = true;
            _db.RefreshTokens.Update(tokenDb);
            return await Save();
        }

        public async Task<RefreshToken> TokenIsValid(string refreshToken)
        {
            var result = await _db.RefreshTokens.FirstOrDefaultAsync(t => t.Token == refreshToken);

            if (result == null)
            {
                return null;
            }
            return result;
        }

        public async Task<bool> Save()
        {
            return await _db.SaveChangesAsync() > 0 ? true : false;
        }
    }
}
