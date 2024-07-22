
using Ecommerce.Model;

namespace Ecommerce.Data.IRepository
{
    public interface IPurchaseRepository
    {
        Task<ShipmentInfo> CreateShippingInfo(ShipmentInfo shipment);
        Task<ShipmentInfo> GetShippingInfo(string id);
        Task<bool> UpdateShippingInfo(ShipmentInfo shipmentInfo, Guid id);
        Task<List<ShipmentInfo>> GetShippingInfoByUser(string id);
        Task CreateOrder(Order order);
        Task CreateOrderItem(OrderItem orderItem);
        Task<Order> GetOrder(Guid id);
        Task UpdateOrder(Order order);
        Task CreatePaymentDetails(PaymentDetails paymentDetails);
        Task<List<Order>> GetPurchases(string id);
    }
}
