using Ecommerce.Services.IServices;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Ecommerce.Services
{
    public class SendEmail : ISendEmail
    {
        private readonly IEmailSender _email;
        public SendEmail(IEmailSender email)
        {
            _email = email;
        }

        public async Task SendVerificationEmail(string email, string verificationCode)
        {
            var emailBody = $@"
                <p>Thank you for registering. Please use the following code to verify your email address:</p>
                <h2>{verificationCode}</h2>
                <p>If you did not request this, please ignore this email.</p>";

            await _email.SendEmailAsync(email, "Verify your Email", emailBody);
        }
    }
}
