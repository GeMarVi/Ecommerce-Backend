using Ecommerce.BackEnd.Data.Models;
using Ecommerce.BackEnd.Shared.AuthDto;

namespace Ecommerce.BackEnd.UseCases.Auth
{
    static class Mappers
    {
        public static ApplicationUser ToApplicationUser(RegisterUserDto registerUserDto)
        {
            return new ApplicationUser
            {
                UserName = registerUserDto.Email,
                Email = registerUserDto.Email,
                EmailConfirmed = false,
            };
        }
    }
}
