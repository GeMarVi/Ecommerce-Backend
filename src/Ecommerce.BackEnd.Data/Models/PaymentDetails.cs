using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.BackEnd.Data.Models
{
    public class PaymentDetails
    {
        [Key]
        public int PaymentDetails_Id { get; set; }
        public long? TransactionId { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string? PaymentStatus { get; set; }
        [Column(TypeName = "money")]
        public decimal? TransactionAmount { get; set; }
        [Column(TypeName = "money")]
        public decimal? NetReceivedAmount { get; set; }
        public Guid OrderId { get; set; }
        
        [ForeignKey("OrderId")]
        public Order Order { get; set; }
    }
}
