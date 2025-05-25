using Ecommerce.BackEnd.Data.IRepository;
using Ecommerce.BackEnd.Data.Models;
using Ecommerce.BackEnd.Infrastructure.Auth;
using Ecommerce.BackEnd.Shared.AuthDto;
using Ecommerce.BackEnd.UseCases.Auth;
using Moq;
using ROP;

namespace Ecommerce.BackEnd.Test.Auth
{
    public class LoginTest
    {
        private readonly string _role = "fake-role";

        private readonly Mock<IAuthRepository> _userRepoMock;
        private readonly Mock<IJwtTokenGenerator> _jwtMock;
        private readonly Login _useCase;

        private readonly string _email = "test@example.com";
        private readonly string _password = "securePass";
        private readonly string _userId = Guid.NewGuid().ToString();
        private readonly ApplicationUser _user;
        private readonly RegisterDto _loginDto;

        public LoginTest()
        {
            _userRepoMock = new Mock<IAuthRepository>();
            _jwtMock = new Mock<IJwtTokenGenerator>();

            _user = new ApplicationUser { Id = _userId, Email = _email };
            _loginDto = new RegisterDto { Email = _email, Password = _password };

            _useCase = new Login(_userRepoMock.Object, _jwtMock.Object);
        }

        private void SetupDefaultLoginFlow()
        {
            _userRepoMock.Setup(r => r.LoginIdentity(_email, _password))
                .ReturnsAsync(Result.Success(_user));

            _userRepoMock.Setup(r => r.IsEmailConfirmed(_email))
                .ReturnsAsync(Result.Success(true));

            _userRepoMock.Setup(r => r.GetIdentityRoles(_user))
                .ReturnsAsync(Result.Success(new List<string> { _role }));

            var tokenId = Guid.NewGuid().ToString();
            var jwt = new JwtResponseDto { Token = "jwt.token", TokenId = tokenId };

            _jwtMock.Setup(j => j.GenerateJwtToken(_role, _userId, _email, null))
                .Returns(Result.Success(jwt));

            var refreshTokenDto = new RefreshTokenDto
            {
                Token = "refresh.token",
                UserId = _userId,
                JwtId = tokenId,
                IsUsed = false,
                IsRevoked = false,
                AddedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(1)
            };

            _jwtMock.Setup(j => j.GenerateRefreshToken(tokenId, It.IsAny<DateTime>(), _userId))
                .Returns(Result.Success(refreshTokenDto));

            _userRepoMock.Setup(r => r.CreateRefreshToken(It.IsAny<RefreshToken>()))
                .ReturnsAsync(Result.Success());
        }

        [Fact]
        public async Task Execute_Should_Return_Tokens_When_Credentials_Are_Valid_And_Email_Confirmed()
        {
            // Arrange
            SetupDefaultLoginFlow();

            // Act
            var result = await _useCase.Execute(_loginDto, _role);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("jwt.token", result.Value.Token);
            Assert.Equal("refresh.token", result.Value.RefreshToken);
        }

        [Fact]
        public async Task Execute_Should_Return_Failure_When_Credentials_Are_Invalid()
        {
            // Arrange
            _userRepoMock.Setup(r => r.LoginIdentity(_loginDto.Email, _loginDto.Password))
                         .ReturnsAsync(Result.Failure<ApplicationUser>("Invalid credentials"));

            // Act
            var result = await _useCase.Execute(_loginDto, _role);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("Invalid credentials", result.Errors[0].Message);
        }

        [Fact]
        public async Task Execute_Should_Return_Failure_When_Email_Is_Not_Confirmed()
        {
            // Arrange
            _userRepoMock.Setup(r => r.LoginIdentity(_loginDto.Email, _loginDto.Password))
                         .ReturnsAsync(Result.Success(_user));

            _userRepoMock.Setup(r => r.IsEmailConfirmed(_loginDto.Email))
                         .ReturnsAsync(Result.Success(false));

            // Act
            var result = await _useCase.Execute(_loginDto, _role);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("User need to be confirm", result.Errors[0].Message);
        }

        [Fact]
        public async Task Execute_ShouldFail_WhenUserDoesNotHaveAssignedRole()
        {
            // Arrange
            _userRepoMock.Setup(x => x.LoginIdentity(_loginDto.Email, _loginDto.Password))
                         .ReturnsAsync(Result.Success(_user));

            _userRepoMock.Setup(x => x.IsEmailConfirmed(_loginDto.Email))
                         .ReturnsAsync(Result.Success(true));

            _userRepoMock.Setup(x => x.GetIdentityRoles(_user))
                         .ReturnsAsync(Result.Success(new List<string> { "Admin", "Manager" })); // no incluye _role

            // Act
            var result = await _useCase.Execute(_loginDto, _role);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("Assigned role is not valid", result.Errors[0].Message);
        }
    }

}
