using Ecommerce.BackEnd.Data.IRepository;
using Ecommerce.BackEnd.Data.Models;
using Ecommerce.BackEnd.Infrastructure.Auth;
using Ecommerce.BackEnd.Shared.AuthDto;
using ROP;

namespace Ecommerce.BackEnd.UseCases.Auth
{
    public class Login
    {
        private readonly IAuthRepository _user;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        private string _role {  get; set; } = default!;
        public Login(IAuthRepository user, IJwtTokenGenerator jwtTokenGenerator)
        {
            _user = user;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<Result<TokensResponseDto>> Execute(RegisterDto loginUserDto, string role)
        {
            _role = role;
            return await VerifyCredentials(loginUserDto)
                .Bind(ValidateRole)
                .Combine(GenerateJwtToken)
                .Bind(GenerateRefreshToken);
        }

        private async Task<Result<ApplicationUser>> VerifyCredentials(RegisterDto loginUserDto)
        {
            var user = await _user.LoginIdentity(loginUserDto.Email, loginUserDto.Password);
           
            if (!user.Success)
                return Result.Failure<ApplicationUser>(user.Errors);

            var isConfirm = await _user.IsEmailConfirmed(loginUserDto.Email);

            if(!isConfirm.Success)
                return Result.Failure<ApplicationUser>(isConfirm.Errors);

            if (!isConfirm.Value)
                return Result.Failure<ApplicationUser>("User need to be confirm");
          
            return user.Value;
        }

        private async Task<Result<ApplicationUser>> ValidateRole(ApplicationUser applicationUser)
        {
            var roles = await _user.GetIdentityRoles(applicationUser);
            if(!roles.Success)
                return Result.Failure<ApplicationUser>(roles.Errors);

            var exist = roles.Value.Any(r => r == _role);

            if (!exist)
                return Result.Failure<ApplicationUser>("Assigned role is not valid");

            return applicationUser;
        }

        private Result<JwtResponseDto> GenerateJwtToken(ApplicationUser user)
        {
            var token = _jwtTokenGenerator.GenerateJwtToken(_role, user.Id, user.Email!, null);
            return !token.Success
                ? Result.Failure<JwtResponseDto>(token.Errors)
                : Result.Success<JwtResponseDto>(token.Value);
        }

        private Result<TokensResponseDto> GenerateRefreshToken((ApplicationUser user, JwtResponseDto token) items)
        {
            var refresh = _jwtTokenGenerator.GenerateRefreshToken(items.token.TokenId, DateTime.UtcNow.AddMonths(1), items.user.Id);
            if (!refresh.Success)
                return Result.Failure<TokensResponseDto>(refresh.Errors);

            var mapper = Mappers.ToRefreshToken(refresh.Value);

            var refreshDb = _user.CreateRefreshToken(mapper);

            if (!refresh.Success)
                return Result.Failure<TokensResponseDto>(refresh.Errors);

            return new  TokensResponseDto
            {
                RefreshToken = refresh.Value.Token,
                Token = items.token.Token
            };
        }
    }
}
