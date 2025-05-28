using Ecommerce.BackEnd.Data.Models;
using Ecommerce.BackEnd.Shared.ProductDto;

namespace Ecommerce.BackEnd.UseCases.Products
{
    public static class Mappers
    {
        private static CompleteProductResponseDto ToCompleteProductResponseDto(Product product)
        {
            return new CompleteProductResponseDto
            {
                Product_Id = product.Product_Id,
                ProductName = product.ProductName,
                Description = product.Description,
                Brand = product.Brand,
                Model = product.Model,
                ClosureType = product.ClosureType,
                SoleMaterial = product.SoleMaterial,
                OuterMaterial = product.OuterMaterial,
                TypeDeport = product.TypeDeport,
                Gender = product.Gender,
                Color = product.Color,
                Price = product.Price,
                hasDiscount = product.hasDiscount,
                ProductStatus = product.ProductStatus,
                DiscountRate = product.DiscountRate,
                image = product.ImagesProduct[0].Url,
                SizeStocksDto = product.sizeStocks
                    .Select(s => new SizeStockResponseDto
                    {
                        Size = s.Size,
                        Stock = s.Stock
                    }).ToList()
            };
        }

        public static List<CompleteProductResponseDto> ToListCompleteResponseDto(List<Product> products)
        {
            var result = new List<CompleteProductResponseDto>();

            foreach (var product in products)
            {
                var mapperProduct = ToCompleteProductResponseDto(product);
                result.Add(mapperProduct);
            }

            return result;
        }
    }
}
