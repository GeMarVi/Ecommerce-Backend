
using Ecommerce.BackEnd.Data.Models;
using ROP;

namespace Ecommerce.BackEnd.Data.IRepository
{
    public interface IPurchaseRepository
    {
        Task<bool> IsShipmentInfoFromUser(string shipmentId, string userId);
        Task<Result<ShipmentInfo>> GetShippingInfo(string id);
        Task<Result<Unit>> UpdateShippingInfo(ShipmentInfo shipmentInfo, Guid id);
        Task<Result<List<ShipmentInfo>>> GetShippingInfoByUser(string id);
        Task<Result<Order>> GetOrder(Guid id);
        Task<bool> UpdateOrder(Order order);
        Task<Result<List<Order>>> GetPurchases(string id);
        Task<Result<List<Order>>> GetPurchaseByOrder(Guid id);
        Task<Result<DiscountCoupon>> GetDiscountCoupon(string Coupon);
    }
}
