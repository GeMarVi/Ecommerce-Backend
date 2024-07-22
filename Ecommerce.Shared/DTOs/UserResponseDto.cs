namespace Ecommerce.Shared.DTOs
{
    public class UserResponseDto
    {
        public string User_Id { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
