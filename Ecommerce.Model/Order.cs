using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Model
{
    public class Order
    {
        [Key]
        public Guid OrderId { get; set; }
        public DateTime PurchaseDate { get; set; } = DateTime.Now;
        public bool IsPaid { get; set; }
        public string UserId { get; set; }
        public Guid ShippmentInfo_Id { get; set; }
        public int OrderStatusId { get; set; }

        [ForeignKey("OrderStatusId")]
        public OrderStatus OrderStatus { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        [ForeignKey("ShippmentInfo_Id")]
        public ShipmentInfo ShipmentInfo { get; set; }

        public List<OrderItem> OrderItems { get; set; }
        public PaymentDetails paymentDetails { get; set; }
    }
}
