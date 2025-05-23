using Ecommerce.BackEnd.Data.Models;
using System.Linq.Expressions;
using ROP;
using Ecommerce.BackEnd.Shared.ProductDto;

namespace Ecommerce.BackEnd.Data.IRepository
{
    public interface IProductRepository
    {
        Task<(List<Product>, int)> GetProducts(int page, int pageZize);
        Task<List<Product>> GetProductByNameOrModel(string name);
        Task<(List<Product>, int)> GetProductsByFilter(Expression<Func<Product, bool>> filter, int page, int pageSize);
        Task<(List<Product>, int)> GetProductsByDiscount(int page, int pageZize);
        Task<(List<Product>, int)> GetProductsByNewProducts(int page, int pageZize);
        //Task<Result<List<ProductsShoppingCartDto>>> GetProductsShoppingCart(List<int> ids);
        Task<Result<Product>> GetProductById(int id);
        Task<Result<List<Product>>> GetProductsToPaymentOrder(List<ProductsToBuyDto> listProducts);
        Task<List<string>> GetImagesUrl(int Id);
        Task<Result<Product>> GetProductByIdWithoutImages(int id);
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
