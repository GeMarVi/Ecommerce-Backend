using Ecommerce.BackEnd.Data.IRepository;
using Ecommerce.BackEnd.Shared.AuthDto;
using Microsoft.AspNetCore.Identity;
using ROP;

namespace Ecommerce.BackEnd.UseCases.Auth
{
    public class CreateNewRole
    {
        private readonly IAuthRepository _user;
       
        public CreateNewRole(IAuthRepository user)
        {
            _user = user;
        }
        public async Task<Result<Unit>> Execute(CreateRoleDto createRole)
        {
            var identityRole = new IdentityRole
            {
                Name = createRole.Role,
                NormalizedName = createRole.Role.ToUpper(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
            };

            var save = await _user.CreateNewRoleAsync(identityRole);
            if (!save.Success)
                return Result.Failure<Unit>(save.Errors);

            return Result.Success();
        }
    }
}
