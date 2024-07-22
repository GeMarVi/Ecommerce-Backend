using Ecommerce.Shared.Validations;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Shared.DTOs
{
    public class UpdateProductDto
    {
        [Required(ErrorMessage = "Campo nombre requerido")]
        [MaxLength(100, ErrorMessage = "El nombre debe tener menos de 100 caractéres")]
        [Display(Name = "Product Name")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Campo descripcion requerida")]
        [MaxLength(10000, ErrorMessage = "La descripción debe tener menos de 10000 caractéres")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Campo Modelo Requerido")]
        [MaxLength(100, ErrorMessage = "Campo modelo acepta 100 caracteres máximo")]
        public string Model { get; set; }

        [Required(ErrorMessage = "Campo marca requerido")]
        [ConstantProductValues("Brand")]
        public string Brand { get; set; }

        [Required(ErrorMessage = "Campo Clousure type Requerido")]
        [Display(Name = "Closure Type")]
        [ConstantProductValues("ClosureType")]
        public string ClosureType { get; set; }

        [Required(ErrorMessage = "Campo Outer Material Requerido")]
        [Display(Name = "Outer Material")]
        [ConstantProductValues("OuterMaterial")]
        public string OuterMaterial { get; set; }

        [Required(ErrorMessage = "Campo Sole Material Requerido")]
        [Display(Name = "Sole Material")]
        [ConstantProductValues("SoleMaterial")]
        public string SoleMaterial { get; set; }

        [Required(ErrorMessage = "Campo Type Deport Requerido")]
        [Display(Name = "Type Deport")]
        [ConstantProductValues("TypeDeport")]
        public string TypeDeport { get; set; }

        [Required(ErrorMessage = "Campo Color Requerido")]
        [MaxLength(100, ErrorMessage = "Campo color acepta 100 caracteres máximo")]
        public string Color { get; set; }

        [Required(ErrorMessage = "Campo género requerido")]
        [MaxLength(20)]
        [ConstantProductValues("Gender")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Campo stock requerido")]
        public List<int> Stock { get; set; }

        [Required(ErrorMessage = "Campo Talla Requerido")]
        public List<double> Size { get; set; }

        [ConstantProductValues("ProductStatus")]
        [MaxLength(30)]
        public string ProductStatus { get; set; }

        [Required(ErrorMessage = "Campo de precio requerido")]
        [Range(0, 500000, ErrorMessage = "Rango de 0 a 500,000")]
        public decimal Price { get; set; }

        [Range(0, 100, ErrorMessage = "Rango de descuento 0 a 100 %")]
        [Display(Name = "Discount Rate")]
        public decimal DiscountRate { get; set; } = 0;

        [Display(Name = "Has Discount")]
        [IsBoolValidations]
        public bool HasDiscount { get; set; } = false;
    }
}
