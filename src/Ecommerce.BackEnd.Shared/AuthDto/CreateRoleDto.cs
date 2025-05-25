using System.ComponentModel.DataAnnotations;

namespace Ecommerce.BackEnd.Shared.AuthDto
{
    public class CreateRoleDto
    {
        [Required]
        [MaxLength(50)]
        public string Role { get; set; } = default!;
    }
}
