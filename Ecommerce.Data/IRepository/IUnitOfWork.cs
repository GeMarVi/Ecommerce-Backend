
using Microsoft.EntityFrameworkCore.Storage;

namespace Ecommerce.Data.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        IPurchaseRepository Orders { get; }
        IProductRepository Products { get; }
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task<bool> SaveAsync();
    }

}
