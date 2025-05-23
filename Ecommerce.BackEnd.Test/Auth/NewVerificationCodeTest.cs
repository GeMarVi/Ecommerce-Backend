using Ecommerce.BackEnd.Data.IRepository;
using Ecommerce.BackEnd.Data.Models;
using Ecommerce.BackEnd.Infrastructure.Email;
using Ecommerce.BackEnd.UseCases.Auth;
using Moq;
using ROP;

namespace Ecommerce.BackEnd.Test.Auth
{
    public class NewVerificationCodeTest
    {
        [Fact]
        public async Task Execute_Should_Return_Success_When_Code_Updated_And_Email_Sent()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var email = "user@example.com";
            var code = "123456";

            var verificationCode = new VerificationCode
            {
                Code = code,
                ExpirationTime = DateTime.UtcNow.AddMinutes(5),
                User_Id = userId
            };

            var user = new ApplicationUser
            {
                Id = userId,
                Email = email
            };

            var userRepoMock = new Mock<IUserRepository>();
            var emailServiceMock = new Mock<IEmailServices>();

            userRepoMock.Setup(r => r.UpdateVerificationCode(It.IsAny<VerificationCode>()))
                        .ReturnsAsync(Result.Success(verificationCode));

            userRepoMock.Setup(r => r.GetUserById(userId))
                        .ReturnsAsync(Result.Success(user));

            emailServiceMock.Setup(e => e.SendEmail(email, It.IsAny<string>(), It.IsAny<string>()))
                            .ReturnsAsync(Result.Success());

            var useCase = new NewVerificationCode(userRepoMock.Object, emailServiceMock.Object);

            // Act
            var result = await useCase.Execute(userId);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("A new verification code has been sent to your email", result.Value);
        }

        [Fact]
        public async Task Execute_Should_Return_Failure_When_Code_Update_Fails()
        {
            var userId = Guid.NewGuid().ToString();

            var userRepoMock = new Mock<IUserRepository>();
            var emailServiceMock = new Mock<IEmailServices>();

            userRepoMock.Setup(r => r.UpdateVerificationCode(It.IsAny<VerificationCode>()))
                        .ReturnsAsync(Result.Failure<VerificationCode>("Update failed"));

            var useCase = new NewVerificationCode(userRepoMock.Object, emailServiceMock.Object);

            var result = await useCase.Execute(userId);

            Assert.False(result.Success);
            Assert.Contains("Update failed", result.Errors[0].Message);
        }

        [Fact]
        public async Task Execute_Should_Return_Failure_When_User_Not_Found()
        {
            var userId = Guid.NewGuid().ToString();
            var code = "123456";
            var verificationCode = new VerificationCode { Code = code, User_Id = userId };

            var userRepoMock = new Mock<IUserRepository>();
            var emailServiceMock = new Mock<IEmailServices>();

            userRepoMock.Setup(r => r.UpdateVerificationCode(It.IsAny<VerificationCode>()))
                        .ReturnsAsync(Result.Success(verificationCode));

            userRepoMock.Setup(r => r.GetUserById(userId))
                        .ReturnsAsync(Result.Failure<ApplicationUser>("User not found"));

            var useCase = new NewVerificationCode(userRepoMock.Object, emailServiceMock.Object);

            var result = await useCase.Execute(userId);

            Assert.False(result.Success);
            Assert.Contains("User not found", result.Errors[0].Message);
        }

        [Fact]
        public async Task Execute_Should_Return_Failure_When_Email_Sending_Fails()
        {
            var userId = Guid.NewGuid().ToString();
            var code = "123456";
            var verificationCode = new VerificationCode { Code = code, User_Id = userId };
            var user = new ApplicationUser { Id = userId, Email = "user@example.com" };

            var userRepoMock = new Mock<IUserRepository>();
            var emailServiceMock = new Mock<IEmailServices>();

            userRepoMock.Setup(r => r.UpdateVerificationCode(It.IsAny<VerificationCode>()))
                        .ReturnsAsync(Result.Success(verificationCode));

            userRepoMock.Setup(r => r.GetUserById(userId))
                        .ReturnsAsync(Result.Success(user));

            emailServiceMock.Setup(e => e.SendEmail(user.Email!, It.IsAny<string>(), It.IsAny<string>()))
                            .ReturnsAsync(Result.Failure<Unit>("An error occurred while sending the email."));

            var useCase = new NewVerificationCode(userRepoMock.Object, emailServiceMock.Object);

            var result = await useCase.Execute(userId);

            Assert.False(result.Success);
        }
    }
}
