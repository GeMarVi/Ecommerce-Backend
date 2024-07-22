
namespace Ecommerce.Services.IServices
{
    public interface ISendEmail
    {
        Task SendVerificationEmail(string email, string code);
    }
}
