using Ecommerce.Shared.DTOs;

namespace Ecommerce.Services.IServices

{
    public interface IProductServices
    {
        Task<ListCompleteProductResponseDto> GetProductsServices(int page, int pageZize);
        Task<List<CompleteProductResponseDto>> GetProductsByNameServices(string name);
        Task<ListCompleteProductResponseDto> GetProductsByGenderServices (string gender, int page, int pageSize);
        Task<ListCompleteProductResponseDto> GetProductsByBrandServices (string gender, int page, int pageSize);
        Task<ListCompleteProductResponseDto> GetProductsByTypeDeportServices (string gender, int page, int pageSize);
        Task<ListCompleteProductResponseDto> GetProductsByDiscountServices (int page, int pageSize);
        Task<ListCompleteProductResponseDto> GetProductsByNewProductsServices (int page, int pageSize);
        Task<List<CompleteProductResponseDto>> GetProductsShoppingCart (List<int> ids);
        Task<ProductResponseDto> GetProductsByIdServices (int id);
        Task<List<string>> GetImagesUrlByIdServices (int id);
        Task<ApiResponse> CreateProductServices(CreateProductDto createProduct);
        Task<ApiResponse> UpdateProductServices(UpdateProductDto updateProduct, int Id);
        Task<ApiResponse> UpdateImageServices(int Id, UpdateImageProductVariantDto image);
        Task<ApiResponse> DeleteProductServices(int Id);
    }
}
