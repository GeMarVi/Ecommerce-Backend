
namespace Ecommerce.Shared.DTOs
{
    public class PurchasesInfoDto
    {
        public Guid OrderId { get; set; }
        public DateTime PurchaseDate { get; set; }
        public bool IsPaid { get; set; }
        public string UserId { get; set; }
        public string OrderStatus { get; set; }
        public List<ItemDto> OrderItems { get; set; }
        public DetailsDto paymentDetails { get; set; }
    }

    public class ItemDto
    {
        public Guid OrderId { get; set; }
        public double Size { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public OrderItemProductDto product { get; set; }
    }

    public class DetailsDto
    {
        public long? TransactionId { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string? PaymentStatus { get; set; }
        public decimal? TransactionAmount { get; set; }
        public decimal? NetReceivedAmount { get; set; }
        public string? OrderId { get; set; }
    }

    public class OrderItemProductDto
    {
        public string ProductName { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string Color { get; set; }
        public decimal Price { get; set; }
        public string ProductStatus { get; set; }
        public decimal DiscountRate { get; set; }
        public string image { get; set; }
    }
}
