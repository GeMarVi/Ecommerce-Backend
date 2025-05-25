using Ecommerce.BackEnd.Data.IRepository;
using Ecommerce.BackEnd.Data.Models;
using Ecommerce.BackEnd.UseCases.Auth;
using Moq;
using ROP;

namespace Ecommerce.BackEnd.Test.Auth
{
    public class LogoutTest
    {
        private readonly Mock<IAuthRepository> _userRepoMock;

        public LogoutTest()
        {
            _userRepoMock = new Mock<IAuthRepository>();
        }

        [Fact]
        public async Task Execute_WithValidToken_ShouldReturnSuccess()
        {
            // Arrange
            var token = new RefreshToken { Token = "valid_token", IsRevoked = false };

            _userRepoMock.Setup(repo => repo.ValidateRefreshToken("valid_token"))
                         .ReturnsAsync(Result.Success(token));

            _userRepoMock.Setup(repo => repo.UpdateRefreshToken(It.Is<RefreshToken>(t => t.IsRevoked)))
                         .ReturnsAsync(Result.Success());

            // Act
            var useCase = new Logout(_userRepoMock.Object);
            var result = await useCase.Execute("valid_token");

            // Assert
            Assert.True(result.Success);
            _userRepoMock.Verify(repo => repo.UpdateRefreshToken(It.Is<RefreshToken>(t => t.IsRevoked)), Times.Once);
        }

        [Fact]
        public async Task Execute_WithInvalidToken_ShouldReturnFailure()
        {
            // Arrange
            _userRepoMock.Setup(repo => repo.ValidateRefreshToken("invalid_token"))
                         .ReturnsAsync(Result.Failure<RefreshToken>("Token not found"));

            // Act
            var useCase = new Logout(_userRepoMock.Object);
            var result = await useCase.Execute("invalid_token");

            // Assert
            Assert.False(result.Success);
            Assert.Contains("Token not found", result.Errors[0].Message);
            _userRepoMock.Verify(repo => repo.UpdateRefreshToken(It.IsAny<RefreshToken>()), Times.Never);
        }

        [Fact]
        public async Task Execute_WithNullToken_ShouldReturnFailure()
        {
            // Arrange
            _userRepoMock.Setup(repo => repo.ValidateRefreshToken("null_token"))
                         .ReturnsAsync(Result.Success<RefreshToken>(null));

            // Act
            var useCase = new Logout(_userRepoMock.Object);
            var result = await useCase.Execute("null_token");

            // Assert
            Assert.False(result.Success);
            Assert.Contains("Token not found or already invalid.", result.Errors[0].Message);
            _userRepoMock.Verify(repo => repo.UpdateRefreshToken(It.IsAny<RefreshToken>()), Times.Never);
        }
    }
}
