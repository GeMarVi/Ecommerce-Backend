using Ecommerce.Model;
using System.Linq.Expressions;

namespace Ecommerce.Data.IRepository
{
    public interface IProductRepository
    {
        Task<(List<Product>, int)> GetProducts(int page, int pageZize);
        Task<List<Product>> GetProductByNameOrModel(string name);
        Task<(List<Product>, int)> GetProductsByFilter(Expression<Func<Product, bool>> filter, int page, int pageSize);
        Task<(List<Product>, int)> GetProductsByDiscount(int page, int pageZize);
        Task<(List<Product>, int)> GetProductsByNewProducts(int page, int pageZize);
        Task<List<Product>> GetProductsShoppingCart(List<int> ids);
        Task<Product> GetProductById(int id);
        Task<List<string>> GetImagesUrl(int Id);
        Task<Product> GetProductByIdWithoutImages(int id);
        Task<SizeStock> GetProductStock(int id, double size);
        Task UpdateProductStock(SizeStock sizeStock);
        Task<bool> CreateProduct( Product product, List<SizeStock> sizeStock, List<ImagesProduct> images);
        bool DoesProductExist(int id);
        bool DoesProductExist(string name);
        string DoesImageExist(int id);
        Task<bool> DeleteProduct(Product product);
        Task<bool> UpdateProduct(Product product);
        Task<bool> UpdateImageProductVariant(int Id, string newUrl);
        Task<bool> Save();
    }
}
