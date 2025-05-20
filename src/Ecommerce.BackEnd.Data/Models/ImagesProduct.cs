using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.BackEnd.Data.Models
{
    public class ImagesProduct
    {
        [Key]
        public int Images_Id { get; set; }

        [MaxLength(200)]
        public string Url { get; set; }
        public int Product_Id { get; set; }

        //Relationship
        [ForeignKey("Product_Id")]
        public Product Product { get; set; }
    }
}

