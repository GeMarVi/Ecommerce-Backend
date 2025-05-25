using Ecommerce.BackEnd.Data.IRepository;
using ROP;

namespace Ecommerce.BackEnd.UseCases.Auth
{
    public class DeleteIdentity
    {
        private readonly IAuthRepository _auth;

        public DeleteIdentity(IAuthRepository auth)
        {
            _auth = auth;
        }

        public async Task<Result<Unit>> Execute(string id)
        {
            var user = await _auth.GetIdentityById(id);
            if (user.Value == null)
                return Result.Failure<Unit>("User not found");

            return await _auth.DeleteIdentity(user.Value);
        }
    }
}
