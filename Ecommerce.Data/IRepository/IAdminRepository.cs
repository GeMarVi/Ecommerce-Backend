using Ecommerce.Model;

namespace Ecommerce.Data.IRepository
{
    public interface IAdminRepository
    {
        Task<ApplicationUser> AdminRegister(ApplicationUser user, string password);
        
    }
}
