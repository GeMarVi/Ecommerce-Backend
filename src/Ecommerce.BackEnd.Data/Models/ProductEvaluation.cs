using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.BackEnd.Data.Models
{
    public class ProductEvaluation
    {
        [Key]
        public int ProductEvaluation_Id { get; set; }
        [Required]
        [Range(0, 5)]
        public int Evaluation { get; set; }
        [MaxLength(1000)]
        public string? Comments { get; set; }
        [Required]
        public DateTime EvaluationCreationDate { get; set; }
        public int Product_Id { get; set; }
        [ForeignKey("Product_Id")]
        public Product product { get; set; }
        public string User_Id { get; set; }
        [ForeignKey("User_Id")]
        public ApplicationUser user { get; set; }
        public Guid? Order_Id { get; set; }
        [ForeignKey("Order_Id")]
        public Order? Order { get; set; }
    }
}
