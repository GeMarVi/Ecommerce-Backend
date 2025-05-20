using System.ComponentModel.DataAnnotations;

namespace Ecommerce.BackEnd.Shared.ProductDto
{
    public class ShoppingCartDto
    {
        [Required(ErrorMessage ="Se requiere una lista de id para el carrito de compras")]
        public List<int> ids { get; set; }
    }

    public class ProductsShoppingCartDto
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public bool HasDiscount { get; set; }
        public byte DiscountRate { get; set; }
        public decimal? PriceWithDiscount { get; set; }
        public string FirstImageUrl { get; set; }
    }
}
