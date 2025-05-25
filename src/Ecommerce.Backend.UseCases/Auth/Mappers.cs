using Ecommerce.BackEnd.Data.Models;
using Ecommerce.BackEnd.Shared.AuthDto;

namespace Ecommerce.BackEnd.UseCases.Auth
{
    static class Mappers
    {
        public static ApplicationUser ToApplicationUser(RegisterDto registerUserDto)
        {
            return new ApplicationUser
            {
                UserName = registerUserDto.Email,
                Email = registerUserDto.Email,
                EmailConfirmed = false,
            };
        }

        public static RefreshToken ToRefreshToken(RefreshTokenDto dto)
        {
            return new RefreshToken
            {
                JwtId = dto.JwtId,
                Token = dto.Token,
                AddedDate = dto.AddedDate,
                ExpiryDate = dto.ExpiryDate,
                IsRevoked = dto.IsRevoked,
                IsUsed = dto.IsUsed,
                UserId = dto.UserId
            };
        }
    }
}
