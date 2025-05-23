using Ecommerce.BackEnd.Data.IRepository;
using Ecommerce.BackEnd.Data.Models;
using Ecommerce.BackEnd.Infrastructure.Auth;
using Ecommerce.BackEnd.Shared.AuthDto;
using ROP;

namespace Ecommerce.BackEnd.UseCases.Auth
{
    public class UserLogin
    {
        private readonly IUserRepository _user;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        public UserLogin(IUserRepository user, IJwtTokenGenerator jwtTokenGenerator)
        {
            _user = user;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<Result<UserTokensResponseDto>> Execute(RegisterUserDto loginUserDto)
        {
            return await VerifyCredentials(loginUserDto)
                .Combine(GenerateJwtToken)
                .Bind(GenerateRefreshToken);
        }

        private async Task<Result<ApplicationUser>> VerifyCredentials(RegisterUserDto loginUserDto)
        {
            var user = await _user.UserLogin(loginUserDto.Email, loginUserDto.Password);

            if (!user.Success)
                return Result.Failure<ApplicationUser>(user.Errors);

            var isConfirm = await _user.IsEmailConfirm(loginUserDto.Email);

            if(!isConfirm.Success)
                return Result.Failure<ApplicationUser>(isConfirm.Errors);

            if (!isConfirm.Value)
                return Result.Failure<ApplicationUser>("User need to be confirm");
          
            return user.Value;
        }

        private Result<JwtResponseDto> GenerateJwtToken(ApplicationUser user)
        {
            var token = _jwtTokenGenerator.GenerateJwtToken("User", user.Id, user.Email!, null);
            return !token.Success
                ? Result.Failure<JwtResponseDto>(token.Errors)
                : Result.Success<JwtResponseDto>(token.Value);
        }

        private Result<UserTokensResponseDto> GenerateRefreshToken((ApplicationUser user, JwtResponseDto token) items)
        {
            var refresh = _jwtTokenGenerator.GenerateRefreshToken(items.token.TokenId, DateTime.UtcNow.AddMonths(1), items.user.Id);
            if (!refresh.Success)
                return Result.Failure<UserTokensResponseDto>(refresh.Errors);

            var mapper = Mappers.ToRefreshToken(refresh.Value);

            var refreshDb = _user.CreateRefreshToken(mapper);

            if (!refresh.Success)
                return Result.Failure<UserTokensResponseDto>(refresh.Errors);

            return new  UserTokensResponseDto
            {
                RefreshToken = refresh.Value.Token,
                Token = items.token.Token
            };
        }
    }
}
