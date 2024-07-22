using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Model
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
