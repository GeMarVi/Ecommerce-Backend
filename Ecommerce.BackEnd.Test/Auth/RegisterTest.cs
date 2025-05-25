using Moq;
using ROP;
using Ecommerce.BackEnd.UseCases.Auth;
using Ecommerce.BackEnd.Data.IRepository;
using Ecommerce.BackEnd.Infrastructure.Email;
using Ecommerce.BackEnd.Shared.AuthDto;
using Ecommerce.BackEnd.Data.Models;

namespace Ecommerce.BackEnd.Test.Auth
{
    public class RegisterTest
    {
        private string _role {  get; }
        public RegisterTest()
        {
            _role = "fake-role";
        }

        [Fact]
            public async Task Execute_ShouldRegisterUserSuccessfully_WhenEmailDoesNotExist()
            {
                // Arrange
                var mockUserRepo = new Mock<IAuthRepository>();
                var mockEmailService = new Mock<IEmailServices>();
                         
                var registerUserDto = new RegisterDto
                {
                    Email = "test@example.com",
                    Password = "SecurePassword123"
                };

                mockUserRepo
                    .Setup(x => x.IdentityExistsByEmail(registerUserDto.Email))
                    .ReturnsAsync(Result.Success(false));

                mockEmailService
                    .Setup(x => x.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                    .ReturnsAsync(Result.Success());

                mockUserRepo
                    .Setup(x => x.RegisterIdentity(It.IsAny <RegisterData<ApplicationUser, VerificationCode>> ()))
                    .ReturnsAsync(Result.Success());


            var useCase = new Register(mockUserRepo.Object, mockEmailService.Object);

                // Act
                var result = await useCase.Execute(registerUserDto, _role);

                // Assert
                Assert.True(result.Success);
            }

            [Fact]
            public async Task Execute_ShouldFail_WhenEmailAlreadyExists()
            {
                // Arrange
                var mockUserRepo = new Mock<IAuthRepository>();
                var mockEmailService = new Mock<IEmailServices>();

                var registerUserDto = new RegisterDto
                {
                    Email = "existing@example.com",
                    Password = "password"
                };

                mockUserRepo
                    .Setup(x => x.IdentityExistsByEmail(registerUserDto.Email))
                    .ReturnsAsync(Result.Success(true)); // Email ya registrado

                var useCase = new Register(mockUserRepo.Object, mockEmailService.Object);

                // Act
                var result = await useCase.Execute(registerUserDto, _role);

                // Assert
                Assert.False(result.Success);
                Assert.Contains("User already exists", result.Errors[0].Message);
            }
        }
    }



