using System.ComponentModel.DataAnnotations;

namespace Ecommerce.BackEnd.Shared.AuthDto
{
    public class TokensRequestDto
    {
       
        [Required]
        [MaxLength(1000)]
        public string Token { get; set; }
       
        [MaxLength(40)]
        [Required]
        public string RefreshToken { get; set; }
       
    }
}
