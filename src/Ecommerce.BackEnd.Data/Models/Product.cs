using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.BackEnd.Data.Models
{
    public class Product
    {
        [Key]
        public int Product_Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string ProductName { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }

        [Required]
        [MaxLength(10000)]
        public string Description { get; set; }

        [Required]
        [MaxLength(50)]
        public string Brand { get; set; }

        [Required]
        [MaxLength(100)]
        public string Model { get; set; }

        [Required]
        [MaxLength(50)]
        public string ClosureType { get; set; }

        [Required]
        [MaxLength(50)]
        public string OuterMaterial { get; set; }

        [Required]
        [MaxLength(50)]
        public string SoleMaterial { get; set; }

        [Required]
        [MaxLength(50)]
        public string TypeDeport { get; set; }


        [MaxLength(50)]
        public string Color { get; set; }

        [Required]
        [MaxLength(20)]
        public string Gender { get; set; }

        [Required]
        [Column(TypeName = "money")]
        [Range(0, 10000)]
        public decimal Price { get; set; }

        public bool hasDiscount { get; set; }

        [Required]
        [MaxLength(30)]
        public string ProductStatus { get; set; }

        [Range(0, 100)]
        public byte DiscountRate { get; set; }

        public List<ApplicationUser> Users { get; set; }
        public List<ImagesProduct> ImagesProduct { get; set; }
        public List<SizeStock> sizeStocks { get; set; }
        public List<OrderItem> orderItems { get; set; }
    }
}
