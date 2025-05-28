using Ecommerce.BackEnd.Data.IRepository;
using Ecommerce.BackEnd.Data.Models;
using Ecommerce.BackEnd.Shared.ProductDto;
using ROP;

namespace Ecommerce.BackEnd.UseCases.Products
{
    public class GetProductByNameOrModel
    {
        private readonly IProductRepository _product;
        public GetProductByNameOrModel(IProductRepository product)
        {
            _product = product;
        }
        public Task<Result<List<CompleteProductResponseDto>>> Execute(string name){
            return Validate(name)
                   .Async()
                   .Bind(_ => Get(name))
                   .Map(p => Mappers.ToListCompleteResponseDto(p));
        }

        private Result<Unit> Validate(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result.Failure<Unit>("Must send name or model");

            return Result.Success();
        }

        private async Task<Result<List<Product>>> Get(string name)
        {
            var result = await _product.GetProductByNameOrModelAsync(name);

            if (!result.Success)
                return Result.Failure<List<Product>>(result.Errors);

            if (result.Value == null || result.Value.Count == 0)
                return Result.NotFound<List<Product>>("Not found products");

            return result.Value;
        }
    }
}
