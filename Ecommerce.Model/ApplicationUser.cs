using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Model
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime creationDate { get; set; } = DateTime.Now;
        public UserPaymentInformation? UserPaymentInformation { get; set; }
        public Tax? Tax { get; set; }
        public List<Product>? Products { get; set; }
        public List<ShipmentInfo>? Shipments { get; set; }
    }
}
