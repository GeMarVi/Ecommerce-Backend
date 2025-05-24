using Ecommerce.BackEnd.Data.IRepository;
using Ecommerce.BackEnd.Data.Models;
using Ecommerce.BackEnd.Infrastructure.Email;
using ROP;

namespace Ecommerce.BackEnd.UseCases.Auth
{
    public class NewVerificationCode
    {
        private readonly IUserRepository _user;
        private readonly IEmailServices _email;

        public NewVerificationCode(IUserRepository user, IEmailServices email)
        {
            _user = user;
            _email = email;
        }
        public async Task<Result<string>> Execute(string id)
        {
            return await CreateVerificationCodeToUpdate(id)
                        .Async()
                        .Bind(UpdateVerificationCode)
                        .Combine(_ => VerifyUser(id))
                        .Bind(userAndCode => SendNewEmail(userAndCode.Item1.Code, userAndCode.Item2.Email!))
                        .Map(_ => "A new verification code has been sent to your email");
        }

        private Result<VerificationCode> CreateVerificationCodeToUpdate(string userId)
        {
            var verificationCode = new VerificationCode()
            {
                Code = Helpers.Helpers.GenerateVerificationCode(),
                ExpirationTime = DateTime.UtcNow.AddMinutes(5),
                User_Id = userId
            };
            return verificationCode;
        }

        private async Task<Result<VerificationCode>> UpdateVerificationCode(VerificationCode verificationCode)
        {
            return await _user.UpdateVerificationCode(verificationCode);
        }

        private async Task<Result<ApplicationUser>> VerifyUser(string userId)
        {
            return await _user.GetUserById(userId);
        }

        private async Task<Result<Unit>> SendNewEmail(string code, string email)
        {
            var subject = "Verify your Email";
            var emailBody = $@"
            <p>Thank you for registering. Please use the following code to verify your email address:</p>
            <h2>{code}</h2>
            <p>If you did not request this, please ignore this email.</p>";

            return await _email.SendEmail(email, subject, emailBody);
        }
    }
}
