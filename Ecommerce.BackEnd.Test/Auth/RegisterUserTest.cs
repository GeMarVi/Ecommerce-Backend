using Moq;
using ROP;
using Ecommerce.BackEnd.UseCases.Auth;
using Ecommerce.BackEnd.Data.IRepository;
using Ecommerce.BackEnd.Infrastructure.Email;
using Ecommerce.BackEnd.Shared.AuthDto;
using Ecommerce.BackEnd.Data.Models;

namespace Ecommerce.BackEnd.Test.Auth
{
    public class RegisterUserTest
    {

           [Fact]
            public async Task Execute_ShouldRegisterUserSuccessfully_WhenEmailDoesNotExist()
            {
                // Arrange
                var mockUserRepo = new Mock<IUserRepository>();
                var mockEmailService = new Mock<IEmailServices>();

                var registerUserDto = new RegisterUserDto
                {
                    Email = "test@example.com",
                    Password = "SecurePassword123"
                };

                mockUserRepo
                    .Setup(x => x.DoesUserExistByEmail(registerUserDto.Email))
                    .ReturnsAsync(Result.Success(false));

                mockEmailService
                    .Setup(x => x.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                    .ReturnsAsync(Result.Success());

                mockUserRepo
                    .Setup(x => x.UserRegister(It.IsAny<ApplicationUser>(), registerUserDto.Password, It.IsAny<VerificationCode>()))
                    .ReturnsAsync(Result.Success());

                var useCase = new UserRegister(mockUserRepo.Object, mockEmailService.Object);

                // Act
                var result = await useCase.Execute(registerUserDto);

                // Assert
                Assert.True(result.Success);
            }

            [Fact]
            public async Task Execute_ShouldFail_WhenEmailAlreadyExists()
            {
                // Arrange
                var mockUserRepo = new Mock<IUserRepository>();
                var mockEmailService = new Mock<IEmailServices>();

                var registerUserDto = new RegisterUserDto
                {
                    Email = "existing@example.com",
                    Password = "password"
                };

                mockUserRepo
                    .Setup(x => x.DoesUserExistByEmail(registerUserDto.Email))
                    .ReturnsAsync(Result.Success(true)); // Email ya registrado

                var useCase = new UserRegister(mockUserRepo.Object, mockEmailService.Object);

                // Act
                var result = await useCase.Execute(registerUserDto);

                // Assert
                Assert.False(result.Success);
                Assert.Contains("User already exists", result.Errors[0].Message);
            }
        }
    }



