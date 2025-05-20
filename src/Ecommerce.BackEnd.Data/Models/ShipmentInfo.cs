using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.BackEnd.Data.Models
{
    public class ShipmentInfo
    {
        [Key]
        public Guid ShipmentInfo_Id { get; set; }
        public DateTime CreationDate { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        [MaxLength(500)]
        public string Address { get; set; }
        public int CodigoPostal { get; set; }
        public string? AditionalInformation { get; set; }
        public string Locality { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Colony { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string? GuestId { get; set; }
        public bool isGuest { get; set; }
        public string? User_Id { get; set; }
        [ForeignKey("User_Id")]
        public ApplicationUser user { get; set; }
        public List<Order> orders { get; set; }
    }
}
