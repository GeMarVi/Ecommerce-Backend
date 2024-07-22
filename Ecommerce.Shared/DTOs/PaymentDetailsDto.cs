
namespace Ecommerce.Shared.DTOs
{
    public class PaymentDetailsDto
    {
        public long? TransactionId {  get; set; }
        public DateTime? ApprovedDate { get; set;}
        public string? PaymentStatus { get; set; }
        public decimal? TransactionAmount { get; set; }
        public decimal? NetReceivedAmount { get; set; }
        public string?  OrderId { get; set; }
    }
}
