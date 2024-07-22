using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Shared.DTOs
{
    public class TokenRequestDto
    {
        [Required]
        [MaxLength(1000)]
        public string Token { get; set; }
        [Required]
        [MaxLength(1000)]
        public string RefreshToken { get; set; }
    }
}
