using Microsoft.EntityFrameworkCore;
using ROP;
using Ecommerce.BackEnd.Data.Data;
using Ecommerce.BackEnd.Data.IRepository;
using Ecommerce.BackEnd.Data.Models;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Ecommerce.BackEnd.Data.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<ProductRepository> _logger;

        public ProductRepository(ApplicationDbContext db, ILogger<ProductRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<Result<(List<Product>, int)>> GetPaginatedProductsAsync(int page, int pageSize)
        {
            try
            {
                var productsQuery = _db.Products
                    .AsNoTracking()
                    .Include(p => p.sizeStocks)
                    .Include(p => p.ImagesProduct.OrderBy(i => i.Images_Id))
                    .OrderByDescending(p => p.Priority);

                var totalCount = await productsQuery.CountAsync();
                var products = await productsQuery
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return Result.Success((products, totalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error accessing the database when retrieving paginated products.");
                return Result.Failure<(List<Product>, int)>("An error occurred while accessing the product data.");
            }
        }

        public async Task<Result<(List<Product>, int)>> GetFilteredProductsAsync(
            Expression<Func<Product, bool>> filter,
            int page,
            int pageSize)
        {
            try
            {
                var query = _db.Products
                    .AsNoTracking()
                    .Where(filter)
                    .Include(p => p.sizeStocks)
                    .Include(p => p.ImagesProduct.OrderBy(i => i.Images_Id));

                var totalCount = await query.CountAsync();

                var products = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return (products, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving filtered products from the database.");
                throw new Exception("Ocurrió un error al obtener los productos filtrados. Intente más tarde o contacte al soporte.", ex);
            }
        }
    }
}
