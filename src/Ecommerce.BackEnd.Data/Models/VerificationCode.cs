using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.BackEnd.Data.Models
{
    public class VerificationCode
    {
        public int Id { get; set; }
        public string User_Id { get; set; }
        public string Code { get; set; }
        public DateTime ExpirationTime { get; set; }
        [ForeignKey("User_Id")]
        public ApplicationUser user { get; set; }
    }
}
