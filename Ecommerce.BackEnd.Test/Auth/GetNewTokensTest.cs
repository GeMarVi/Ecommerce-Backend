using Ecommerce.BackEnd.Data.IRepository;
using Ecommerce.BackEnd.Data.Models;
using Ecommerce.BackEnd.Infrastructure.Auth;
using Ecommerce.BackEnd.Shared.AuthDto;
using Ecommerce.BackEnd.Shared.Configuration;
using Ecommerce.BackEnd.UseCases.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Newtonsoft.Json.Linq;
using ROP;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Ecommerce.BackEnd.Test.Auth
{
    public class GetNewTokensTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock = new();
        private readonly Mock<IJwtTokenGenerator> _jwtGeneratorMock = new();
        private readonly JwtConfig _jwtConfig = new() { Secret = "0123456789ABCDEF0123456789ABCDEF" };

        private GetNewTokens CreateService() =>
            new(_userRepositoryMock.Object, Options.Create(_jwtConfig), _jwtGeneratorMock.Object);

        private string GenerateFakeAccessToken(string jwtId, string userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtConfig.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim("id", userId),
                new Claim(JwtRegisteredClaimNames.Jti, jwtId),
            }),
                Expires = DateTime.UtcNow.AddMinutes(5), // Expired
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        [Fact]
        public async Task Execute_ShouldReturnNewTokens_WhenRequestIsValid()
        {
            // Arrange
            var jwtId = Guid.NewGuid().ToString();
            var userId = Guid.NewGuid().ToString();
            var refreshTokenStr = "valid-refresh-token";
            var accessToken = GenerateFakeAccessToken(jwtId, userId);
            var service = CreateService();

            var storedRefreshToken = new RefreshToken
            {
                Token = refreshTokenStr,
                JwtId = jwtId,
                IsUsed = false,
                IsRevoked = false,
                ExpiryDate = DateTime.UtcNow.AddDays(1),
                UserId = userId
            };

            var newRefreshTokenDto = new RefreshTokenDto
            {
                Token = "new-refresh-token",
                UserId = userId,
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                JwtId = jwtId
            };

            var user = new ApplicationUser { Id = userId, Email = "test@example.com" };

            _userRepositoryMock.Setup(x => x.TokenIsValid(refreshTokenStr))
                .ReturnsAsync(Result.Success(storedRefreshToken));

            _userRepositoryMock.Setup(x => x.UpdateRefreshToken(It.IsAny<RefreshToken>()))
                .ReturnsAsync(Result.Success());

            _jwtGeneratorMock.Setup(x => x.GenerateRefreshToken(jwtId, It.IsAny<DateTime>(), userId))
                .Returns(Result.Success(newRefreshTokenDto));

            _userRepositoryMock.Setup(x => x.CreateRefreshToken(It.IsAny<RefreshToken>()))
                .ReturnsAsync(Result.Success());

            _userRepositoryMock.Setup(x => x.GetUserById(userId))
                .ReturnsAsync(Result.Success(user));

            _jwtGeneratorMock.Setup(x => x.GenerateJwtToken("User", userId, user.Email, jwtId))
                .Returns(Result.Success(new JwtResponseDto { Token = accessToken }));

            var input = new TokensRequestDto { Token = accessToken, RefreshToken = refreshTokenStr };

            // Act
            var result = await service.Execute(input);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(accessToken, result.Value.Token);
            Assert.Equal("new-refresh-token", result.Value.RefreshToken); 
        }

        [Fact]
        public async Task Execute_ShouldFail_WhenRefreshTokenIsInvalid()
        {
            // Arrange
            var jwtId = Guid.NewGuid().ToString();
            var userId = Guid.NewGuid().ToString();
            var accessToken = GenerateFakeAccessToken(jwtId, userId);
            var input = new TokensRequestDto { Token = accessToken, RefreshToken = "invalid" };

            _userRepositoryMock.Setup(x => x.TokenIsValid("invalid"))
                .ReturnsAsync(Result.Failure<RefreshToken>("Invalid token"));

            var service = CreateService();

            // Act
            var result = await service.Execute(input);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("Invalid token", result.Errors[0].Message);
        }

        [Fact]
        public async Task Execute_ShouldFail_WhenRefreshTokenIsExpired()
        {
            // Arrange
            var jwtId = Guid.NewGuid().ToString();
            var userId = Guid.NewGuid().ToString();
            var accessToken = GenerateFakeAccessToken(jwtId, userId);

            var expiredToken = new RefreshToken
            {
                Token = "expired",
                JwtId = jwtId,
                IsUsed = false,
                IsRevoked = false,
                ExpiryDate = DateTime.UtcNow.AddMinutes(-1),
                UserId = userId
            };

            _userRepositoryMock.Setup(x => x.TokenIsValid("expired"))
                .ReturnsAsync(Result.Success(expiredToken));

            var input = new TokensRequestDto { Token = accessToken, RefreshToken = "expired" };
            var service = CreateService();

            // Act
            var result = await service.Execute(input);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("expired", result.Errors[0].Message);
        }

        [Fact]
        public async Task Execute_ShouldFail_WhenTokenHasMissingClaims()
        {
            // Arrange
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtConfig.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] { }), // no claims
                Expires = DateTime.UtcNow.AddMinutes(5),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(token);
            var service = CreateService();

            var input = new TokensRequestDto { Token = accessToken, RefreshToken = "doesntmatter" };

            // Act
            var result = await service.Execute(input);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("Invalid Token", result.Errors[0].Message);
        }
    }
}