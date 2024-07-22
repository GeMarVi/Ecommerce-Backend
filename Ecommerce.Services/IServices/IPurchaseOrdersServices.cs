using Ecommerce.Shared.DTOs;

namespace Ecommerce.Services.IServices
{
    public interface IPurchaseOrdersServices
    {
        Task<ApiResponse> CreatePurchaseOrder(List<ListProductPreferenceMpDto> products, string id);
        Task<ApiResponse> CreateShippingInfoServices(string id, ShipmentInfoDto shipmentInfo);
        Task<ApiResponse> GetShippingInfoServices(string id);
        Task<ApiResponse> UpdateShippingInfoServices(ShipmentInfoDto shipmentInfo, string id);
        Task<ApiResponse> GetShippingInfoByUserServices(string id);
        Task<bool> GetInfoPaymentMPServices(long id);
        Task<ApiResponse> GetPurchasesInfoServices(string id);
    }
}
