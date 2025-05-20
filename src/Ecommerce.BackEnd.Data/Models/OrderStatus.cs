using System.ComponentModel.DataAnnotations;

namespace Ecommerce.BackEnd.Data.Models
{
    public class OrderStatus
    {
        [Key]
        public int OrderStatus_Id { get; set; }
        [MaxLength(50)]
        [Required]
        public string Status { get; set; }
        public List<Order> Orders { get; set; }
    }
}
