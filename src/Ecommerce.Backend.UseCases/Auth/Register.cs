using ROP;
using Ecommerce.BackEnd.Data.IRepository;
using Ecommerce.BackEnd.Data.Models;
using Ecommerce.BackEnd.Infrastructure.Email;
using Ecommerce.BackEnd.Shared.AuthDto;

namespace Ecommerce.BackEnd.UseCases.Auth
{
    public class Register
    {
        private readonly IAuthRepository _user;
        private readonly IEmailServices _email;
        private string _role { get; set; } = default!;

        public Register(IAuthRepository user, IEmailServices email)
        {
            _user = user;
            _email = email;
        }
        public async Task<Result<string>> Execute(RegisterDto user, string role)
        {
            _role = role;
            return await ValidateUser(user)
                .Combine(_ => CreateVerificationCode())
                .Bind(SendEmailVerificationCode)
                .Bind(SaveUser);
        }

        private async Task<Result<RegisterDto>> ValidateUser(RegisterDto user)
        {
            var exist = await _user.IdentityExistsByEmail(user.Email);
            return exist.Success && exist.Value == true
                ? Result.Failure<RegisterDto>("User already exists")
                : user;
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

        private async Task<Result<(RegisterDto, VerificationCode)>> SendEmailVerificationCode((RegisterDto user, VerificationCode code) items)
        {
            var subject = "Verify your Email";
            var emailBody = $@"
            Thank you for registering. Please use the following code to verify your email address:
            {items.code.Code}
            If you did not request this, please ignore this email.";

            var sendEmail = await _email.SendEmail(items.user.Email, subject, emailBody);
            return sendEmail.Success
                ? items
                : Result.Failure<(RegisterDto, VerificationCode)>(sendEmail.Errors);
        }

        private async Task<Result<string>> SaveUser((RegisterDto user, VerificationCode code) args)
        {
            var id = Guid.NewGuid().ToString();
            var newUser = Mappers.ToApplicationUser(args.user);
            newUser.Id = id;
            var register = new RegisterData<ApplicationUser, VerificationCode>
            {
                Identity = newUser,
                VerificationCode = args.code,
                Password = args.user.Password,
                Role = _role
            };
            var result = await _user.RegisterIdentity(register);
            if (!result.Success)
                return Result.Failure<string>(result.Errors);

            return id;
        }
    }
}
