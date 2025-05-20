using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.BackEnd.Data.Models
{
    public class UserPaymentInformation
    {
        [Key]
        public int UserPaymentInformation_Id { get; set; }
        [Required]
        [RegularExpression("^(?:\\d{16}|\\d{4}-\\d{4}-\\d{4}-\\d{4})$\r\n", ErrorMessage = "El numero de tarjeta debe contener 16 digitos")]
        public int NumberCard { get; set; }
        [Required]
        [MaxLength(5)]
        [RegularExpression("@\"^(0[1-9]|1[0-2])/(20\\d{2}|21[0-9])$\"", ErrorMessage = "El formato de fecha debe ser MM/AAAA")]
        public string ExpirationDate { get; set; }
        [Required]
        [MaxLength(3)]
        [RegularExpression("^\\d{3}$\r\n", ErrorMessage = "El formato debe tener 3 digitos enteros")]
        public int Cvv { get; set; }
        public string? User_Id { get; set; }
        [ForeignKey("User_Id")]
        public ApplicationUser? User { get; set; }
    }
}
