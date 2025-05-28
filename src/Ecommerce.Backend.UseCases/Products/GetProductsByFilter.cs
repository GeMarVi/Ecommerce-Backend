using Ecommerce.BackEnd.Data.IRepository;
using Ecommerce.BackEnd.Data.Models;
using Ecommerce.BackEnd.Shared.ProductDto;
using Ecommerce.BackEnd.Shared.Validations.Ecommerce.BackEnd.Shared.Constants;
using Ecommerce.BackEnd.UseCases.Helpers;
using ROP;

namespace Ecommerce.BackEnd.UseCases.Products
{
    public class GetProductsByFilter
    {
        private readonly IProductRepository _product;

        public GetProductsByFilter(IProductRepository product)
        {
            _product = product;
        }

        public async Task<Result<ListProductResponseDto>> Execute(Dictionary<string, string> filters, int page, int pageSize)
        {
            return await Validate(filters, page, pageSize)
                    .Async()
                    .Bind(_ => Get(filters, page, pageSize))
                    .Bind(p => MapperToCompleteResponseDto(p.Item1, p.Item2, pageSize));
        }

        private Result<Unit> Validate(Dictionary<string, string> filters, int page, int pageSize)
        {
            if (page < 1 || pageSize < 1)
                return Result.Failure<Unit>("Page and page size must be greater than zero.");

            var allowedFilterTypes = ProductFieldAllowedValues.AllowedFieldsSearchable;
            var filterTypesWithoutKeyword = new[] { "NewsProducts", "Discount" };

            foreach (var filter in filters)
            {
                if (!allowedFilterTypes.Contains(filter.Key) && !filterTypesWithoutKeyword.Contains(filter.Key))
                {
                    return Result.Failure<Unit>($"Invalid filter type: {filter.Key}");
                }

                if (filterTypesWithoutKeyword.Contains(filter.Key))
                {
                    if (!bool.TryParse(filter.Value, out _))
                        return Result.Failure<Unit>($"Filter '{filter.Key}' must be true or false.");
                }
                else
                {
                    var allowedValues = ProductFieldAllowedValues.Get(filter.Key);
                    if (!allowedValues.Contains(filter.Value))
                    {
                        return Result.Failure<Unit>($"Invalid value '{filter.Value}' for filter '{filter.Key}'. " +
                            $"Accepted: {string.Join(", ", allowedValues)}");
                    }
                }
            }

            return Result.Success();
        }

        private async Task<Result<(List<Product>, int)>> Get(Dictionary<string, string> filters, int page, int pageSize)
        {
            var filterExpression = ProductFilterBuilder.Build(filters);
            var result = await _product.GetFilteredProductsAsync(filterExpression, page, pageSize);

            if (!result.Success)
                return Result.Failure<(List<Product>, int)>(result.Errors);

            var (products, _) = result.Value;

            if (products == null || products.Count == 0)
                return Result.NotFound<(List<Product>, int)>("No products found.");

            return result.Value;
        }

        private Result<ListProductResponseDto> MapperToCompleteResponseDto(List<Product> products, int totalCount, int pageSize)
        {
            var totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
            var result = Mappers.ToListCompleteResponseDto(products);

            return new ListProductResponseDto { numberPages = totalPages, Products = result };
        }
    }
}



