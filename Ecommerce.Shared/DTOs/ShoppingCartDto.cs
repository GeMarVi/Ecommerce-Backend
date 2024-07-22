
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Shared.DTOs
{
    public class ShoppingCartDto
    {
        [Required(ErrorMessage ="Se requiere una lista de id para el carrito de compras")]
        public List<int> ids { get; set; }
    }
}
