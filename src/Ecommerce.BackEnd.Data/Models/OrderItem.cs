
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.BackEnd.Data.Models
{
    public class OrderItem
    {
        [Key]
        public long OrderItemId { get; set; }
        public Guid OrderId { get; set; }
        public string Name { get; set; }
        public int ProductId { get; set; }
        public double Size { get; set; }
        public int Quantity { get; set; }
        public bool Coupon { get; set; }
        public byte DiscountPercentaje { get; set; }

        [Column(TypeName = "money")]
        public decimal Price { get; set; }
        public decimal PriceDiscount { get; set; }
        public decimal PriceDiscountAndCoupon { get; set; }
        [Column(TypeName = "money")]
        public decimal TotalPrice { get; set; }
        public decimal TotalPriceDiscount { get; set; }
        public decimal TotalPriceDiscountAndCoupon { get; set; }

        [ForeignKey("OrderId")]
        public Order Order { get; set; }

        [ForeignKey("ProductId")]
        public Product product { get; set; }
    }
}
