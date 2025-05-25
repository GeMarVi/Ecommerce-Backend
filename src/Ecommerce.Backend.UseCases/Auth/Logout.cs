using Ecommerce.BackEnd.Data.IRepository;
using Ecommerce.BackEnd.Data.Models;
using ROP;

namespace Ecommerce.BackEnd.UseCases.Auth
{
    public class Logout
    {
        private readonly IAuthRepository _user;
        public Logout(IAuthRepository user)
        {
            _user = user;
        }
        public async Task<Result<Unit>> Execute(string refreshToken)
        {
            return await ValidateToken(refreshToken)
                         .Bind(UserLogout);
        }

        public async Task<Result<RefreshToken>> ValidateToken(string refreshToken)
        {
            var result = await _user.ValidateRefreshToken(refreshToken);
            if (!result.Success)
                return Result.Failure<RefreshToken>(result.Errors);
            if(result.Value == null)
                return Result.Failure<RefreshToken>("Token not found or already invalid.");              

            return result.Value;
        }

        private async Task<Result<Unit>> UserLogout(RefreshToken refreshToken)
        {
            refreshToken.IsRevoked = true;
            return await _user.UpdateRefreshToken(refreshToken);
        }
    }
}
