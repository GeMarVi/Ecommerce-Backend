using ROP;
using Ecommerce.BackEnd.Data.IRepository;
using Ecommerce.BackEnd.Data.Models;
using Ecommerce.BackEnd.Infrastructure.Email;
using Ecommerce.BackEnd.Shared.AuthDto;

namespace Ecommerce.BackEnd.UseCases.Auth
{
    public class UserRegister
    {
        private readonly IUserRepository _user;
        private readonly IEmailServices _email;
        public UserRegister(IUserRepository user, IEmailServices email)
        {
            _user = user;
            _email = email;
        }
        public async Task<Result<string>> Execute(RegisterUserDto user)
        {
            return await ValidateUser(user)
                .Combine(_ => CreateVerificationCode())
                .Bind(SendEmailVerificationCode)
                .Bind(SaveUser);
        }

        private async Task<Result<RegisterUserDto>> ValidateUser(RegisterUserDto user)
        {
            var exist = await _user.DoesUserExistByEmail(user.Email);
            return exist.Success && exist.Value == true
                ? Result.Failure<RegisterUserDto>("User already exists")
                : user.Success();
        }

        private Result<VerificationCode> CreateVerificationCode()
        {
            var verificationCode = new VerificationCode()
            {
                Code = Helpers.Helpers.GenerateVerificationCode(),
                ExpirationTime = DateTime.UtcNow.AddMinutes(5),
            };
            return verificationCode;
        }

        private async Task<Result<(RegisterUserDto, VerificationCode)>> SendEmailVerificationCode((RegisterUserDto user, VerificationCode code) items)
        {
            var subject = "Verify your Email";
            var emailBody = $@"
            Thank you for registering. Please use the following code to verify your email address:
            {items.code.Code}
            If you did not request this, please ignore this email.";

            var sendEmail = await _email.SendEmail(items.user.Email, subject, emailBody);
            return sendEmail.Success
                ? items
                : Result.Failure<(RegisterUserDto, VerificationCode)>(sendEmail.Errors);
        }

        private async Task<Result<string>> SaveUser((RegisterUserDto user, VerificationCode code) items)
        {
            var id = Guid.NewGuid().ToString();
            var newUser = Mappers.ToApplicationUser(items.user);
            newUser.Id = id;
            var result = await _user.UserRegister(newUser, items.user.Password, items.code);
            if (!result.Success)
                return Result.Failure<string>(result.Errors);

            return id;
        }
    }
}
