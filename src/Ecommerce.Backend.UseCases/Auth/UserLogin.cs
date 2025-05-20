using Ecommerce.BackEnd.Data.IRepository;
using Ecommerce.BackEnd.Data.Models;
using Ecommerce.BackEnd.Shared.AuthDto;
using ROP;

namespace Ecommerce.BackEnd.UseCases.Auth
{
    class UserLogin
    {
        private readonly IUserRepository _user;
        public UserLogin(IUserRepository user)
        {
            _user = user;
        }

        public async Task<Result<UserResponseDto>> Execute(RegisterUserDto loginUserDto)
        {
            return await DoesUserExist(loginUserDto.Email)
                .Bind(_ => VerifyCredentials(loginUserDto))
                .Combine(GenerateToken)
                .Bind(CreateResponse);
        }

        private async Task<Result<Unit>> DoesUserExist(string email)
        {
            var exist = await _user.DoesUserExistByEmail(email);
            return exist.Success ? Result.Success() : Result.Failure<Unit>("Invalid Payload");
        }

        private async Task<Result<ApplicationUser>> VerifyCredentials(RegisterUserDto loginUserDto)
        {
            return await _user.UserLogin(loginUserDto.Email, loginUserDto.Password);
        }

        private async Task<Result<JwtGeneratorResponseDto>> GenerateToken(ApplicationUser user)
        {
            var token = await _jwtGenerator.GenerateToken(user);
            return token.Success
                ? Result.Success<JwtGeneratorResponseDto>(token.Value)
                : Result.Failure<JwtGeneratorResponseDto>(token.Errors);
        }

        private Result<UserResponseDto> CreateResponse((ApplicationUser user, JwtGeneratorResponseDto tokens) items)
        {
            return new UserResponseDto()
            {
                User_Id = items.user.Id,
                Token = items.tokens.Token,
                RefreshToken = items.tokens.RefreshToken
            };
        }
    }
}
