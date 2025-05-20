using System.ComponentModel.DataAnnotations;

namespace Ecommerce.BackEnd.Data.Models
{
    public class DiscountCoupon
    {
        [Key]
        public int DiscountCoupon_Id { get; set; }

        [MaxLength(12)]
        public string Coupon { get; set; }

        [Range(1, 100)]
        public byte DiscountPercentage { get; set; }

        public DateTime CreationDate { get; set; } = DateTime.Now;

        public List<Order> Orders { get; set; }
    }
}
