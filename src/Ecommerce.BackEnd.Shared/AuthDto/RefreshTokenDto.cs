namespace Ecommerce.BackEnd.Shared.AuthDto
{
    public class RefreshTokenDto
    {
        public string JwtId { get; set; }
        public string Token { get; set; } 
        public DateTime AddedDate { get; set; } 
        public DateTime ExpiryDate { get; set; }
        public bool IsRevoked { get; set; } 
        public bool IsUsed { get; set; }
        public string UserId { get; set; } 
    }
}
