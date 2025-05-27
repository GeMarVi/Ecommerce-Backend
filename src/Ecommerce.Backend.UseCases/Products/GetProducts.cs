using Ecommerce.BackEnd.Data.IRepository;
using Ecommerce.BackEnd.Data.Models;
using Ecommerce.BackEnd.Shared.ProductDto;
using ROP;

namespace Ecommerce.BackEnd.UseCases.Products
{
    public class GetProducts
    {
        private readonly IProductRepository _product;
        public GetProducts(IProductRepository product)
        {
            _product = product;
        }

        public async Task<Result<ListProductResponseDto>> Execute(int page, int pageSize)
        {
            return await Validate(page, pageSize)
                        .Async()
                        .Bind(Get)
                        .Bind(p => CreateResponse(p.Item1, p.Item2, pageSize));
        }

        public Result<(int, int)> Validate(int page, int pageSize)
        {
            if (page < 1 || pageSize < 1)
            {
                return Result.Failure<(int, int)>("Page and page size must be greater than zero.");
            }
            return (page, pageSize);
        }

        public async Task<Result<(List<Product>, int)>> Get((int page, int pageSize) args)
        {
            var products = await _product.GetPaginatedProductsAsync(args.page, args.pageSize);

            if (!products.Success)
                return Result.Failure<(List<Product>, int)>(products.Errors);

            if (products.Value.Item1 == null || products.Value.Item1.Count == 0)
            {
                return Result.NotFound<(List<Product>, int)>("No products found.");
            }

            return products.Value;
        }

        public Result<ListProductResponseDto> CreateResponse(List<Product> products, int totalCount, int pageSize)
        {
            var totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
            var result = Mappers.ToListCompleteResponseDto(products);

            return new ListProductResponseDto { numberPages = totalPages, Products = result };
        }
    }
}