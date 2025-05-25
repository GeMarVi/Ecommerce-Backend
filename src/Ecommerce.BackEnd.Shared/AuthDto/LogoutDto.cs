using System.ComponentModel.DataAnnotations;

namespace Ecommerce.BackEnd.Shared.AuthDto
{
    public class LogoutDto
    {
        [Required]
        [MaxLength(32)]
        public string RefreshToken { get; set; }
    }
}
