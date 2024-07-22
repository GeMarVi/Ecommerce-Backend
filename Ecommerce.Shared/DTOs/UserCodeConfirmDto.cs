
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Shared.DTOs
{
    public class UserCodeConfirmDto
    {
        [Required]
        [MaxLength(50)]
        public string user_Id {  get; set; }

        [Required]
        [StringLength(6, ErrorMessage = "code must be 6 characters")]
        public string code { get; set; }
    }
}
