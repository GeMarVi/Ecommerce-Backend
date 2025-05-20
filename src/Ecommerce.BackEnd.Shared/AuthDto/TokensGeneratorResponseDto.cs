
namespace Ecommerce.BackEnd.Shared.AuthDto
{
    public class TokensGeneratorResponseDto
    {
        public string Token { get; set; }
        public RefreshTokenDto RefreshToken { get; set; }
    }
}
