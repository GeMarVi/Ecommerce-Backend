using Microsoft.AspNetCore.Identity;
using Ecommerce.BackEnd.Data.Models;
using Ecommerce.BackEnd.Data.IRepository;

namespace Ecommerce.BackEnd.Data.Repository
{
    public class AdminRepository : IAdminRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AdminRepository(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<ApplicationUser> AdminRegister(ApplicationUser admin, string password)
        {
            var isCreated = await _userManager.CreateAsync(admin, password);

            if (!isCreated.Succeeded)
            {
                return null;
            }

            await _userManager.AddToRoleAsync(admin, "Admin");
            return admin;
        }
    }
}
