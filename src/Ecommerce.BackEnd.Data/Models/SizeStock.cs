using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.BackEnd.Data.Models
{
    public class SizeStock
    {
        [Key]
        public int SizeStock_Id { get; set; }

        [Range(2, 13)]
        public double Size { get; set; }

        [Range(0, 10000)]
        public int Stock { get; set; }

        public int Product_Id { get; set; }

        [ForeignKey("Product_Id")]
        public Product Product { get; set; }
    }
}
