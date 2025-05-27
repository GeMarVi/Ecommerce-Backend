namespace Ecommerce.BackEnd.Shared.ProductDto
{
    public class ListProductResponseDto
    {
        public int numberPages { get; set; }
        public List<CompleteProductResponseDto> Products { get; set; }
    }
}
