using Ecommerce.Data.Context;
using Ecommerce.Data.IRepository;
using Microsoft.EntityFrameworkCore.Storage;

namespace Ecommerce.Data.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public IPurchaseRepository Orders { get; }
        public IProductRepository Products { get; }

        public UnitOfWork(ApplicationDbContext db, IPurchaseRepository orderRepository, IProductRepository productRepository)
        {
            _db = db;
            Orders = orderRepository;
            Products = productRepository;
        }

        public async Task<bool> SaveAsync()
        {
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _db.Database.BeginTransactionAsync();
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
