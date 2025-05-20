using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.BackEnd.Data.Models
{
    public class Tax
    {
        [Key]
        public int Tax_Id { get; set; }
        [Required]
        [MaxLength(20)]
        public string? FirstName { get; set; }
        [MaxLength(20)]
        public string? LastName { get; set; }
        [MaxLength(50)]
        public string? StreetAndNumber { get; set; }
        [MaxLength(30)]
        public string? Neighborhood { get; set; }
        [MaxLength(30)]
        public string? City { get; set; }
        [MaxLength(30)]
        public string? State { get; set; }
        [RegularExpression(@"^\d{5}$", ErrorMessage = "El código postal debe contener exactamente 5 dígitos.")]
        public int? PostalCode { get; set; }
        [MaxLength(12)]
        public string? Phone { get; set; }
        [MaxLength(200)]
        public string? AdditionalReferences { get; set; }
        public string Client_Id { get; set; }
        [ForeignKey("Client_Id")]
        public ApplicationUser Client { get; set; }
    }
}
