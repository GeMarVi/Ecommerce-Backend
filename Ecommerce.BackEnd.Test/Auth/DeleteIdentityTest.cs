using Ecommerce.BackEnd.Data.IRepository;
using Ecommerce.BackEnd.Data.Models;
using Ecommerce.BackEnd.UseCases.Auth;
using Moq;
using ROP;

namespace Ecommerce.BackEnd.Test.Auth
{
    public class DeleteIdentityTest
    {
        [Fact]
        public async Task Execute_ShouldReturnSuccess_WhenUserIsDeleted()
        {
            // Arrange
            var userId = "user-123";
            var user = new ApplicationUser { Id = userId, Email = "test@example.com" };

            var mockRepo = new Mock<IAuthRepository>();
            mockRepo.Setup(r => r.GetIdentityById(userId))
                    .ReturnsAsync(Result.Success(user));

            mockRepo.Setup(r => r.DeleteIdentity(user))
                    .ReturnsAsync(Result.Success());

            var useCase = new DeleteIdentity(mockRepo.Object);

            // Act
            var result = await useCase.Execute(userId);

            // Assert
            Assert.True(result.Success);
        }

        [Fact]
        public async Task Execute_ShouldReturnFailure_WhenUserNotFound()
        {
            // Arrange
            var userId = "nonexistent-user";

            var mockRepo = new Mock<IAuthRepository>();
            mockRepo.Setup(r => r.GetIdentityById(userId))
                    .ReturnsAsync(Result.Success<ApplicationUser>(null));

            var useCase = new DeleteIdentity(mockRepo.Object);

            // Act
            var result = await useCase.Execute(userId);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("User not found", result.Errors[0].Message);
        }

        [Fact]
        public async Task Execute_ShouldReturnFailure_WhenDeletionFails()
        {
            // Arrange
            var userId = "user-456";
            var user = new ApplicationUser { Id = userId, Email = "test@example.com" };

            var mockRepo = new Mock<IAuthRepository>();
            mockRepo.Setup(r => r.GetIdentityById(userId))
                    .ReturnsAsync(Result.Success(user));

            mockRepo.Setup(r => r.DeleteIdentity(user))
                    .ReturnsAsync(Result.Failure("Failed to delete"));

            var useCase = new DeleteIdentity(mockRepo.Object);

            // Act
            var result = await useCase.Execute(userId);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("Failed to delete", result.Errors[0].Message);
        }
    }
}
