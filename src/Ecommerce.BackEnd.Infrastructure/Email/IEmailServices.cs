using ROP;

namespace Ecommerce.BackEnd.Infrastructure.Email
{
    public interface IEmailServices
    {
        Task<Result<Unit>> SendEmail(string email, string subject, string body);
    }
}
