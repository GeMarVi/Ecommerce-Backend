using Ecommerce.BackEnd.UseCases.Auth;
using Ecommerce.BackEnd.UseCases.Products;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.BackEnd.UseCases
{
    public static class UseCasesDependencyInjection
    {
        public static IServiceCollection AddUseCases(this IServiceCollection services)
            => services.User().Product();

        private static IServiceCollection User(this IServiceCollection services)
            => services.AddScoped<Register>()
            .AddScoped<Login>()
            .AddScoped<EmailConfirm>()
            .AddScoped<GetNewTokens>()
            .AddScoped<NewVerificationCode>()
            .AddScoped<Logout>()
            .AddScoped<CreateNewRole>()
            .AddScoped<DeleteIdentity>();

        private static IServiceCollection Product(this IServiceCollection services)
            => services.AddScoped<GetProducts>()
              .AddScoped<GetProductsByFilter>();
    }    

}
