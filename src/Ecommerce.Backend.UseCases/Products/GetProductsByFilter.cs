using Ecommerce.BackEnd.Data.IRepository;
using Ecommerce.BackEnd.Data.Models;
using Ecommerce.BackEnd.Shared.ProductDto;
using ROP;
using System.Linq.Expressions;

namespace Ecommerce.BackEnd.UseCases.Products
{
    public class GetProductsByFilter
    {
        private readonly IProductRepository _product;

        public enum ProductFilterType
        {
            Gender,
            Brand,
            TypeDeport
        }

        public GetProductsByFilter(IProductRepository product)
        {
            _product = product;
        }

        public async Task<Result<ListProductResponseDto>> Execute(string keyword, int page, int pageSize, ProductFilterType filterType)
        {
            return await Validate(page, pageSize)
                        .Async()
                        .Bind(_ => Get(keyword, page, pageSize, filterType))
                        .Bind(p => MapperToCompleteResponseDto(p.Item1, p.Item2, pageSize));
        }

        private Result<Unit> Validate(int page, int pageSize)
        {
            if (page < 1 || pageSize < 1)
            {
                return Result.Failure<Unit>("Page and page size must be greater than zero.");
            }
            return Result.Success();
        }

        public async Task<Result<(List<Product>, int)>> Get(string keyword, int page, int pageSize, ProductFilterType filterType)
        {
            Expression<Func<Product, bool>> filter = filterType switch
            {
                ProductFilterType.Gender => p => p.Gender == keyword,
                ProductFilterType.Brand => p => p.Brand == keyword,
                ProductFilterType.TypeDeport => p => p.TypeDeport == keyword,
                _ => p => false 
            };

            var productsResult = await _product.GetFilteredProductsAsync(filter, page, pageSize);

            if (!productsResult.Success)
                return Result.Failure<(List<Product>, int)>(productsResult.Errors);

            var (products, _) = productsResult.Value;

            if (products == null || products.Count == 0)
                return Result.NotFound<(List<Product>, int)>("No products found.");

            return productsResult.Value;
        }

        private Result<ListProductResponseDto> MapperToCompleteResponseDto(List<Product> products, int totalCount, int pageSize)
        {
            var totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
            var result = Mappers.ToListCompleteResponseDto(products);

            return new ListProductResponseDto { numberPages = totalPages, Products = result };
        }
    }
}
