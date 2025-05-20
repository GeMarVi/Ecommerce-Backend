using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.BackEnd.Data.Models
{
    public class Order
    {
        [Key]
        public Guid OrderId { get; set; }
        public DateTime PurchaseDate { get; set; }
        public bool IsPaid { get; set; }
        public string? UserId { get; set; }
        public bool IsGuest { get; set; }
        public string? GuestId { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TotalPriceWithDiscount { get; set; }
        public Guid ShippmentInfo_Id { get; set; }
        public int OrderStatusId { get; set; }
        public int? DiscountCouponId { get; set; }

        [ForeignKey("OrderStatusId")]
        public OrderStatus OrderStatus { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }

        [ForeignKey("ShippmentInfo_Id")]
        public ShipmentInfo ShipmentInfo { get; set; }

        [ForeignKey("DiscountCouponId")]
        public DiscountCoupon? DiscountCoupon { get; set; }

        public List<OrderItem> OrderItems { get; set; }
        public PaymentDetails PaymentDetails { get; set; }
    }
}
