using Ecommerce.BackEnd.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.BackEnd.Data.Data
{
 
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<UserPaymentInformation> ClientPaymentInformation { get; set; }
        public DbSet<ImagesProduct> ImagesProducts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderStatus> OrderStatus { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductEvaluation> ProductEvaluations { get; set; }
        public DbSet<Tax> Taxs { get; set; }
        public DbSet<SizeStock> SizeStocks { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<VerificationCode> VerificationCodes { get; set; }
        public DbSet<ShipmentInfo> ShipmentInfo { get; set; }
        public DbSet<OrderItem> OrderItem { get; set; }
        public DbSet<PaymentDetails> PaymentDetails { get; set; }
        public DbSet<DiscountCoupon> DiscountCoupons { get; set; }
    }
}
