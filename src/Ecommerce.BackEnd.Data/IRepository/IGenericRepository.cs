using Ecommerce.BackEnd.Data.Models;

namespace Ecommerce.BackEnd.Data.IRepository
{
    public interface IGenericRepository<T>
    {
        Task<(List<Product>, int)> GetProductsByBrand(string brand, int page, int pageSize);
    }
}
