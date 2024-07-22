using Ecommerce.Data.IRepository;
using Ecommerce.Model;
using Ecommerce.Services.IServices;
using Ecommerce.Shared.Configuration;
using Ecommerce.Shared.DTOs;
using Ecommerce.Shared.Helpers;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace Ecommerce.Services
{
    public class GenerateJWTServices : IGenerateJWTServices
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtConfig _jwtConfig;
        private readonly TokenValidationParameters _tokenValidationParameters;
        public GenerateJWTServices(IUserRepository userRepository, IOptions<JwtConfig> jwtConfig, TokenValidationParameters tokenValidationParameters)
        {
            _userRepository = userRepository;
            _jwtConfig = jwtConfig.Value;
            _tokenValidationParameters = tokenValidationParameters;

        }
        public async Task<JwtGeneratorResponseDto> GenerateTokenAsync(ApplicationUser applicationUser)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtConfig.Secret);
            var roles = _userRepository.GetRole(applicationUser).Result;

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new ClaimsIdentity(new[]
                {
                    new Claim("Id", applicationUser.Id),
                    new Claim(JwtRegisteredClaimNames.Sub, applicationUser.Email),
                    new Claim(JwtRegisteredClaimNames.Email, applicationUser.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString())
                })),
                Expires = DateTime.UtcNow.Add(_jwtConfig.ExpiryTime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)

            };

            foreach (var role in roles)
            {
                tokenDescriptor.Subject.AddClaim(new Claim(ClaimTypes.Role, role));
            }

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);

            var jwtToken = jwtTokenHandler.WriteToken(token);

            var refreshToken = new RefreshToken()
            {
                JwtId = token.Id,
                Token = RandomGenerator.GenetateRandomString(23),
                AddedDate = DateTime.Now,
                ExpiryDate = DateTime.UtcNow.AddMonths(1),
                IsRevoked = false,
                IsUsed = false,
                UserId = applicationUser.Id,
            };

            bool result = await _userRepository.CreateRefreshToken(refreshToken);

            if (!result)
            {
                return null;
            }

            return new JwtGeneratorResponseDto
            {
                Token = jwtToken,
                RefreshToken = refreshToken.Token
            };
        }

        public async Task<ApiResponse> VerifyAndGenerateTokenAsync(TokenRequestDto tokenRequest)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            try
            {
                _tokenValidationParameters.ValidateLifetime = false;

                var tokenBeingVerified = jwtTokenHandler.ValidateToken(tokenRequest.Token, _tokenValidationParameters, out var validatedToken);

                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);

                    if (!result || tokenBeingVerified == null)
                    {
                        throw new Exception("Invalid Token");
                    }
                }

                var utcExpiryDate = long.Parse(tokenBeingVerified.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp).Value);

                var expiryDate = DateTimeOffset.FromUnixTimeSeconds(utcExpiryDate).UtcDateTime;
                if (expiryDate < DateTime.UtcNow)
                {
                    throw new Exception("Token Expired");
                }

                var storedToken = await _userRepository.TokenIsValid(tokenRequest.RefreshToken);
                if (storedToken == null)
                    throw new Exception("Invalid Token");

                if (storedToken.IsRevoked || storedToken.IsUsed)
                    throw new Exception("Invalid Token");

                var jti = tokenBeingVerified.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti).Value;

                if (jti != storedToken.JwtId)
                {
                    throw new Exception("Invalid Token");
                }

                if (storedToken.ExpiryDate < DateTime.UtcNow)
                {
                    throw new Exception("Token Expired");
                }

                var updateRefreshTokenDb = await _userRepository.UpdateRefreshToken(storedToken.Id);

                var dbUser = await _userRepository.GetUserById(storedToken.UserId);

                var tokens = await GenerateTokenAsync(dbUser);

                return ApiResponse.Success( "", tokens);
            }
            catch (Exception e)
            {
                return ApiResponse.Error(e.Message, HttpStatusCode.BadRequest);
            }
        }
    }
}
