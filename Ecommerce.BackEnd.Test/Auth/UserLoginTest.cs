using Ecommerce.BackEnd.Data.IRepository;
using Ecommerce.BackEnd.Data.Models;
using Ecommerce.BackEnd.Infrastructure.Auth;
using Ecommerce.BackEnd.Shared.AuthDto;
using Ecommerce.BackEnd.UseCases.Auth;
using Moq;
using ROP;

namespace Ecommerce.BackEnd.Test.Auth
{
    public class UserLoginTest
    {
        [Fact]
        public async Task Execute_Should_Return_Tokens_When_Credentials_Are_Valid_And_Email_Confirmed()
        {
            // Arrange
            var email = "test@example.com";
            var password = "securePass";
            var userId = Guid.NewGuid().ToString();
            var loginDto = new RegisterUserDto { Email = email, Password = password };
            var user = new ApplicationUser { Id = userId, Email = email };

            var userRepoMock = new Mock<IUserRepository>();
            var jwtMock = new Mock<IJwtTokenGenerator>();

            userRepoMock.Setup(r => r.UserLogin(email, password))
                        .ReturnsAsync(Result.Success(user));

            userRepoMock.Setup(r => r.IsEmailConfirm(email))
                        .ReturnsAsync(Result.Success(true));

            var tokenId = Guid.NewGuid().ToString();
            var jwt = new JwtResponseDto { Token = "jwt.token", TokenId = tokenId };

            jwtMock.Setup(j => j.GenerateJwtToken("User", userId, email, null))
                   .Returns(Result.Success(jwt));

            var refreshToken = new RefreshToken
            {
                Token = "refresh.token",
                UserId = userId,
                JwtId = jwt.TokenId,
                IsUsed = false,
                IsRevoked = false, 
                AddedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(30)
            };

            var refreshTokenDto = new RefreshTokenDto
            {
                Token = "refresh.token",
                UserId = userId,
                JwtId = jwt.TokenId,
                IsUsed = false,
                IsRevoked = false,
                AddedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(30)
            };

            jwtMock.Setup(j => j.GenerateRefreshToken(tokenId, It.IsAny<DateTime>(), userId))
                   .Returns(Result.Success(refreshTokenDto));

            userRepoMock.Setup(r => r.CreateRefreshToken(It.IsAny<Data.Models.RefreshToken>()))
                        .ReturnsAsync(Result.Success());

            var useCase = new UserLogin(userRepoMock.Object, jwtMock.Object);

            // Act
            var result = await useCase.Execute(loginDto);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("jwt.token", result.Value.Token);
            Assert.Equal("refresh.token", result.Value.RefreshToken);
        }

        [Fact]
        public async Task Execute_Should_Return_Failure_When_Credentials_Are_Invalid()
        {
            // Arrange
            var loginDto = new RegisterUserDto { Email = "fail@example.com", Password = "wrong" };

            var userRepoMock = new Mock<IUserRepository>();
            var jwtMock = new Mock<IJwtTokenGenerator>();

            userRepoMock.Setup(r => r.UserLogin(loginDto.Email, loginDto.Password))
                        .ReturnsAsync(Result.Failure<ApplicationUser>("Invalid credentials"));

            var useCase = new UserLogin(userRepoMock.Object, jwtMock.Object);

            // Act
            var result = await useCase.Execute(loginDto);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("Invalid credentials", result.Errors[0].Message);
        }

        [Fact]
        public async Task Execute_Should_Return_Failure_When_Email_Is_Not_Confirmed()
        {
            // Arrange
            var email = "notconfirmed@example.com";
            var password = "validpass";
            var user = new ApplicationUser { Id = Guid.NewGuid().ToString(), Email = email };
            var loginDto = new RegisterUserDto { Email = email, Password = password };

            var userRepoMock = new Mock<IUserRepository>();
            var jwtMock = new Mock<IJwtTokenGenerator>();

            userRepoMock.Setup(r => r.UserLogin(email, password))
                        .ReturnsAsync(Result.Success(user));

            userRepoMock.Setup(r => r.IsEmailConfirm(email))
                        .ReturnsAsync(Result.Success(false));

            var useCase = new UserLogin(userRepoMock.Object, jwtMock.Object);

            // Act
            var result = await useCase.Execute(loginDto);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("User need to be confirm", result.Errors[0].Message);
        }

    }
}
 