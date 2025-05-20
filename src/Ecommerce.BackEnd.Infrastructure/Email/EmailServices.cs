using Ecommerce.BackEnd.Shared.Configuration;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using ROP;
using System.Net;

namespace Ecommerce.BackEnd.Infrastructure.Email
{
    public class EmailService : IEmailServices
    {
        private readonly SmtpSettings _smtpSettings;
        public EmailService(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
        }

        public async Task<Result<Unit>> SendEmail(string email, string subject, string body)
        {
            try
            {
                var smtpClient = new SmtpClient(_smtpSettings.Server, _smtpSettings.Port);
                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = false;

                smtpClient.Credentials = new NetworkCredential(_smtpSettings.SenderEmail, _smtpSettings.Password);
                var message = new MailMessage(_smtpSettings.SenderEmail, email, subject, body);
                await smtpClient.SendMailAsync(message);
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure<Unit>("Ha ocurrido un error en el servicio de envio de correo");
            }
        }
    }
}
 