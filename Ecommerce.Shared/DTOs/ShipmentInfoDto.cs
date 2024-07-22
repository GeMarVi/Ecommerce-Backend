
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Shared.DTOs
{
    public class ShipmentInfoDto
    {
        [Required(ErrorMessage ="Name Required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Last Name Required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Street Name Required")]
        public string street { get; set; }

        [Required(ErrorMessage = "Exterior Number Required")]
        public int ExteriorNumber { get; set; }

        public int? InteriorNumber { get; set; }

        [Required(ErrorMessage = "Codigo postale required")]
        public int CodigoPostal { get; set; }

        public string? AditionalInformation { get; set; }

     
        [Required(ErrorMessage = "City Required")]
        public string City { get; set; }

        [Required(ErrorMessage = "Colony Required")]
        public string Colony { get; set; }

        [Required(ErrorMessage = "State Name Required")]
        public string State { get; set; }
        
        [Required(ErrorMessage = "Country Required")]
        public string Country { get; set; }

        [Required(ErrorMessage = "Municipality Name Required")]
        public string Municipality { get; set; }

        [Required(ErrorMessage = "Phone Required")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Email Required")]
        [EmailAddress(ErrorMessage = "Error Email Format")]
        public string Email { get; set; }
    }
}
