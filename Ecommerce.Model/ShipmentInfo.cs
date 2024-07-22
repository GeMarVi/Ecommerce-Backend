using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Model
{
    public class ShipmentInfo
    {
        [Key]
        public Guid ShipmentInfo_Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string LastName { get; set; }
        public string street { get; set; }
        public int ExteriorNumber { get; set; }
        public int? InteriorNumber { get; set; }
        public int CodigoPostal { get; set; }
        public string? AditionalInformation { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Municipality { get; set; }
        public string Colony { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string User_Id { get; set; }
        [ForeignKey("User_Id")]
        public ApplicationUser user { get; set; }
        public List<Order> orders { get; set; }
    }
}
