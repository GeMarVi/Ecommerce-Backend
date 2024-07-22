using Ecommerce.Model;

namespace Ecommerce.Data.IRepository
{
    public interface IGenericRepository<T>
    {
        Task<(List<Product>, int)> GetProductsByBrand(string brand, int page, int pageSize);
    }
}
