using System.ComponentModel.DataAnnotations;

namespace Ecommerce.BackEnd.Shared.ProductDto
{
    public class ProductsToBuyDto
    {
        [Required(ErrorMessage = "Nombre requerido")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Price requerido")]
        [Range(0,50000, ErrorMessage = "Rango de precio no valido")]
        public decimal Price { get; set; } 
        
        [Required(ErrorMessage = "Discount requerido")]
        [Range(0,100, ErrorMessage = "Rango de precio no valido")]
        public byte ProductDiscount { get; set; }

        [Required(ErrorMessage = "Price requerido")]
        [Range(0, 50000, ErrorMessage = "Rango de precio no valido")]
        public decimal PriceWithDiscount { get; set; }

        [Required(ErrorMessage = "Cantidad requerido")]
        [Range(0, 10, ErrorMessage = "Cantidad no valida")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Product id requerido")]
        public int Product_Id { get; set; }

        [Required(ErrorMessage = "Talla es requerida")]
        public double Size { get; set; }
    }
}
