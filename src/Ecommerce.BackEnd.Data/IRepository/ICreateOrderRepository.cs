using Ecommerce.BackEnd.Data.Models;

namespace Ecommerce.BackEnd.Data.IRepository
{
    public interface ICreateOrderRepository
    {
        Task CreateOrderItem(List<OrderItem> orderItem);
        Task CreateOrder(Order order);
        Task CreatePaymentDetails(PaymentDetails paymentDetails);
        Task CreateShippingInfo(ShipmentInfo shipment);
        Task UpdateProductStock(List<SizeStock> sizeStock);
    }
}
