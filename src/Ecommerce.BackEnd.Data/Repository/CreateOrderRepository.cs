using Ecommerce.BackEnd.Data.Data;
using Ecommerce.BackEnd.Data.IRepository;
using Ecommerce.BackEnd.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.BackEnd.Data.Repository
{
    public class CreateOrderRepository : ICreateOrderRepository
    {
        private readonly ApplicationDbContext _db;

        public CreateOrderRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task CreateOrderItem(List<OrderItem> orderItem)
        {
            foreach (var item in orderItem)
            {
                await _db.OrderItem.AddAsync(item);
            }
        }

        public async Task CreateOrder(Order order)
        {
            await _db.Orders.AddAsync(order);
        }

        public async Task CreatePaymentDetails(PaymentDetails paymentDetails)
        {
            await _db.PaymentDetails.AddAsync(paymentDetails);
        }

        public async Task CreateShippingInfo(ShipmentInfo shipment)
        {
            await _db.ShipmentInfo.AddAsync(shipment);
        }

        public async Task UpdateProductStock(List<SizeStock> sizeStock)
        {
            foreach (var item in sizeStock)
            {
                var result = await _db.SizeStocks.FirstOrDefaultAsync(s => s.SizeStock_Id == item.SizeStock_Id);

                if (result == null)
                {
                    throw new Exception($"No se encontró el tamaño de producto con ID {item.SizeStock_Id}");
                }

                result.Stock = item.Stock;
            }
        }

    }
}
