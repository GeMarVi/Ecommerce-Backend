
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Model
{
    public class OrderItem
    {
        [Key]
        public int OrderItemId { get; set; }
        public Guid OrderId { get; set; }
        public int ProductId { get; set; }
        public double Size { get; set; }
        public int Quantity { get; set; }

        [Column(TypeName = "money")]
        public decimal UnitPrice { get; set; }

        [Column(TypeName = "money")]
        public decimal TotalPrice { get; set; }

        [ForeignKey("OrderId")]
        public Order Order { get; set; }

        [ForeignKey("ProductId")]
        public Product product { get; set; }
    }
}
