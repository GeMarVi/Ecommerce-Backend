using Ecommerce.BackEnd.UseCases.Auth;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.BackEnd.UseCases
{
    public static class UseCasesDependencyInjection
    {
        public static IServiceCollection AddUseCases(this IServiceCollection services)
            => services.User();

        private static IServiceCollection User(this IServiceCollection services)
            => services.AddScoped<Register>()
            .AddScoped<Login>()
            .AddScoped<EmailConfirm>()
            .AddScoped<GetNewTokens>()
            .AddScoped<NewVerificationCode>()
            .AddScoped<Logout>()
            .AddScoped<CreateNewRole>()
            .AddScoped<DeleteUser>();
    }    
}
