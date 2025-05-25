using Ecommerce.BackEnd.Data.IRepository;
using Ecommerce.BackEnd.Data.Models;
using Ecommerce.BackEnd.Shared.AuthDto;
using Ecommerce.BackEnd.UseCases.Auth;
using Moq;
using ROP;

namespace Ecommerce.BackEnd.Test.Auth
{
    public class EmailConfirmTest
    {
        [Fact]
        public async Task Execute_Should_Return_Success_When_Code_Is_Valid_And_User_Is_Confirmed()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var codeValue = "123456";
            var codeConfirmDto = new CodeConfirmDto { id = userId, code = codeValue };

            var verificationCode = new VerificationCode
            {
                Code = codeValue,
                User_Id = userId,
                ExpirationTime = DateTime.UtcNow.AddMinutes(10)
            };

            var user = new ApplicationUser
            {
                Id = userId,
                Email = "test@example.com",
                EmailConfirmed = false
            };

            var userRepoMock = new Mock<IAuthRepository>();

            userRepoMock.Setup(r => r.GetVerificationCode(userId, codeValue))
                        .ReturnsAsync(Result.Success(verificationCode));

            userRepoMock.Setup(r => r.GetIdentityById(userId))
                        .ReturnsAsync(Result.Success(user));

            userRepoMock.Setup(r => r.ConfirmIdentityAndRevokeCode(user, verificationCode))
                        .ReturnsAsync(Result.Success());

            var useCase = new EmailConfirm(userRepoMock.Object);

            // Act
            var result = await useCase.Execute(codeConfirmDto);

            // Assert
            Assert.True(result.Success);
            Assert.True(user.EmailConfirmed);
        }

        [Fact]
        public async Task Execute_Should_Return_Failure_When_Code_Is_Invalid()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var codeValue = "invalid";
            var codeConfirmDto = new CodeConfirmDto { id = userId, code = codeValue };

            var userRepoMock = new Mock<IAuthRepository>();
            userRepoMock.Setup(r => r.GetVerificationCode(userId, codeValue))
                        .ReturnsAsync(Result.Failure<VerificationCode>("Invalid verification code."));

            var useCase = new EmailConfirm(userRepoMock.Object);

            // Act
            var result = await useCase.Execute(codeConfirmDto);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("Invalid verification code.", result.Errors[0].Message);
        }

        [Fact]
        public async Task Execute_Should_Return_Failure_When_Code_Is_Expired()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var codeValue = "123456";
            var codeConfirmDto = new CodeConfirmDto { id = userId, code = codeValue };

            var expiredCode = new VerificationCode
            {
                Code = codeValue,
                User_Id = userId,
                ExpirationTime = DateTime.UtcNow.AddMinutes(-5)
            };

            var userRepoMock = new Mock<IAuthRepository>();
            userRepoMock.Setup(r => r.GetVerificationCode(userId, codeValue))
                        .ReturnsAsync(Result.Success(expiredCode));

            var useCase = new EmailConfirm(userRepoMock.Object);

            // Act
            var result = await useCase.Execute(codeConfirmDto);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("Expired verification code.", result.Errors[0].Message);
        }
    }
}
