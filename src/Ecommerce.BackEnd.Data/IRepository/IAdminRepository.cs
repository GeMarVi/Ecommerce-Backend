using Ecommerce.BackEnd.Data.Models;

namespace Ecommerce.BackEnd.Data.IRepository
{
    public interface IAdminRepository
    {
        Task<ApplicationUser> AdminRegister(ApplicationUser user, string password);
        
    }
}
