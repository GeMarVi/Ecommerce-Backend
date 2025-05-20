using Ecommerce.BackEnd.Infrastructure.Auth;
using Ecommerce.BackEnd.Infrastructure.Email;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.BackEnd.Infrastructure
{
    public static class ExternalServiciesDependenciesInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        => services.Infrastructure();

        private static IServiceCollection Infrastructure(this IServiceCollection services)
            => services.AddScoped<IEmailServices, EmailService>()
              .AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
      
    }
}
 