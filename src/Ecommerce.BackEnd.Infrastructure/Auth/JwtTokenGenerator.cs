using Ecommerce.BackEnd.Infrastructure.Auth;
using Ecommerce.BackEnd.Shared.AuthDto;
using Ecommerce.BackEnd.Shared.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ROP;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtConfig _jwtConfig;
    private readonly JwtSecurityTokenHandler _tokenHandler;

    public JwtTokenGenerator(IOptions<JwtConfig> jwtConfig)
    {
        _jwtConfig = jwtConfig.Value;
        _tokenHandler = new JwtSecurityTokenHandler();
    }

    public Result<JwtResponseDto> GenerateJwtToken(string role, string userId, string email, string? existingJti = null)
    {
        var jti = existingJti ?? Guid.NewGuid().ToString();

        var claims = new List<Claim>
        {
            new("id", userId),
            new("role", role),
            new("email", email),
            new(JwtRegisteredClaimNames.Jti, jti),
            new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtConfig.ExpiryTime),
            SigningCredentials = credentials
        };

        var token = _tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = _tokenHandler.WriteToken(token);

        return new JwtResponseDto
        {
            Token = tokenString,
            TokenId = jti
        };
    }

    public Result<RefreshTokenDto> GenerateRefreshToken(string jwtId, DateTime expiry, string userId)
    {
        return new RefreshTokenDto
        {
            JwtId = jwtId,
            Token = Guid.NewGuid().ToString("N"),
            AddedDate = DateTime.UtcNow,
            ExpiryDate = expiry,
            IsRevoked = false,
            IsUsed = false,
            UserId = userId
        };
    }
}