namespace Ecommerce.Shared.DTOs
{
    public class ProductResponseDto
    {
        public int Product_Id { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string ClosureType { get; set; }
        public string OuterMaterial { get; set; }
        public string SoleMaterial { get; set; }
        public string TypeDeport { get; set; }
        public string Gender { get; set; }
        public string Color { get; set; }
        public decimal Price { get; set; }
        public bool hasDiscount { get; set; }
        public string ProductStatus { get; set; }
        public decimal DiscountRate { get; set; }
        public List<SizeStockResponseDto> SizeStocks { get; set; }
        public List<ImagesProductDto> ImagesProduct { get; set; }
    }
}
