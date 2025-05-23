using System.ComponentModel.DataAnnotations;

namespace Ecommerce.BackEnd.Shared.AuthDto
{
    public class CodeConfirmDto
    {
       
        [Required]
        [MaxLength(40)]
        [RegularExpression(@"^[0-9a-fA-F]{8}(-[0-9a-fA-F]{4}){3}-[0-9a-fA-F]{12}$",
           ErrorMessage = "The identifier is not in a valid GUID format.")]
        public string id { get; set; }

        [Required]
        [StringLength(6, ErrorMessage = "code must be 6 characters")]
        public string code { get; set; }
        
    }
}
