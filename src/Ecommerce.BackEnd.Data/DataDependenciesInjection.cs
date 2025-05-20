using Ecommerce.BackEnd.Data.IRepository;
using Microsoft.Extensions.DependencyInjection;
using Ecommerce.BackEnd.Data.Repository;

namespace Ecommerce.Backend.Data
{
    public static class DataDependenciesInjection
    {
        public static IServiceCollection AddRepository(this IServiceCollection services)
          => services.Repository();

        private static IServiceCollection Repository(this IServiceCollection services)
            => services.AddScoped<IAdminRepository, AdminRepository>()
               .AddScoped<IUserRepository, UserRepository>()
               .AddScoped<ICreateOrderRepository, CreateOrderRepository>()
               .AddScoped<IProductRepository, ProductRepository>()
               .AddScoped<IPurchaseRepository, PurchaseRepository>();
               
    }
}
