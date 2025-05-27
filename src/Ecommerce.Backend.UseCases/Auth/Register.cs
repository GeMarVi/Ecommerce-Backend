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
  
        public Register(IAuthRepository user, IEmailServices email)
        {
            _user = user;
            _email = email;
        }
        public async Task<Result<string>> Execute(RegisterDto user, string role)
        {
            return await ValidateUser(user)
                .Bind(_ => CreateVerificationCode())
                .Bind(code => SendEmailVerificationCode(code, user))
                .Bind( code => SaveUser(code, user, role));
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

        private async Task<Result<VerificationCode>> SendEmailVerificationCode(VerificationCode verificationCode, RegisterDto registerDto)
        {
            var subject = "Verify your Email";
            var emailBody = $@"
            Thank you for registering. Please use the following code to verify your email address:
            {verificationCode.Code}
            If you did not request this, please ignore this email.";

            var sendEmail = await _email.SendEmail(registerDto.Email, subject, emailBody);
            return sendEmail.Success
                ? verificationCode
                : Result.Failure<VerificationCode>(sendEmail.Errors);
        }

        private async Task<Result<string>> SaveUser(VerificationCode verificationCode, RegisterDto registerDto, string role)
        {
            var id = Guid.NewGuid().ToString();
            var newUser = Mappers.ToApplicationUser(registerDto);
            newUser.Id = id;
            var register = new RegisterData<ApplicationUser, VerificationCode>
            {
                Identity = newUser,
                VerificationCode = verificationCode,
                Password = registerDto.Password,
                Role = role
            };
            var result = await _user.RegisterIdentity(register);
            if (!result.Success)
                return Result.Failure<string>(result.Errors);

            return id;
        }
    }
}
