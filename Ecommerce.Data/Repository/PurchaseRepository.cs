using Ecommerce.Data.Context;
using Ecommerce.Data.IRepository;
using Ecommerce.Model;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Data.Repository
{
    public class PurchaseRepository : IPurchaseRepository
    {
        private readonly ApplicationDbContext _db;

        public PurchaseRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<ShipmentInfo> CreateShippingInfo(ShipmentInfo shipment)
        {
            await _db.ShipmentInfo.AddAsync(shipment);
            var result = await Save();

            if (result)
            {
                return shipment;
            }
            return null;
        }

        public async Task<ShipmentInfo> GetShippingInfo(string id)
        {
            var result = await _db.ShipmentInfo.FirstOrDefaultAsync(s => s.ShipmentInfo_Id.ToString() == id);

            if(result == null)
            {
                return null;
            }
            return result;
        }

        public async Task<bool> UpdateShippingInfo(ShipmentInfo shipmentInfo, Guid id)
        {
            var info = await _db.ShipmentInfo.FirstOrDefaultAsync(i => i.ShipmentInfo_Id == id);
            info.Name = shipmentInfo.Name;
            info.LastName = shipmentInfo.LastName;
            info.street = shipmentInfo.street;
            info.ExteriorNumber = shipmentInfo.ExteriorNumber;
            info.InteriorNumber = shipmentInfo.InteriorNumber;
            info.CodigoPostal = shipmentInfo.CodigoPostal;
            info.AditionalInformation = shipmentInfo.AditionalInformation;
            info.City = shipmentInfo.City;
            info.State = shipmentInfo.State;
            info.Country = shipmentInfo.Country;
            info.Municipality = shipmentInfo.Municipality;
            info.Colony = shipmentInfo.Colony;
            info.Phone = shipmentInfo.Phone;
            info.Email = shipmentInfo.Email;
            _db.ShipmentInfo.Update(info);
            
            return await Save();
        }

        public async Task<List<ShipmentInfo>> GetShippingInfoByUser(string id)
        {
            var result = await _db.ShipmentInfo.Where(s => s.User_Id == id).ToListAsync();
            return result;
        }

        public async Task CreateOrderItem(OrderItem orderItem)
        {
            await _db.AddAsync(orderItem);
        }

        public async Task CreateOrder(Order order)
        {
            await _db.AddAsync(order);
        }

        public async Task CreatePaymentDetails(PaymentDetails paymentDetails)
        {
            await _db.PaymentDetails.AddAsync(paymentDetails);
        }

        public async Task<Order> GetOrder(Guid id)
        {
            var result = await _db.Orders.FirstOrDefaultAsync(o => o.OrderId == id);
            if(result == null)
            {
                return null;
            }
            return result;
        } 
        
        public async Task UpdateOrder(Order order)
        {
            var result = await _db.Orders.FirstOrDefaultAsync(o => o.OrderId == order.OrderId);
            result.OrderStatusId = order.OrderStatusId;
            result.IsPaid = order.IsPaid;
            _db.Orders.Update(result);
        }

        public async Task<List<Order>>? GetPurchases(string id)
        {
            var result = await _db.Orders.Where(o => o.UserId.ToUpper() == id.ToUpper() && o.IsPaid == true)
                         .Include(i => i.OrderItems)
                            .ThenInclude(iv => iv.product)
                                .ThenInclude(pv => pv.ImagesProduct)
                         .Include(o => o.paymentDetails)
                         .Include(s => s.OrderStatus)
                         .OrderByDescending(o => o.PurchaseDate)
                         .ToListAsync();

            return result;
        }

        public async Task<bool> Save()
        {
            return await _db.SaveChangesAsync() > 0 ? true : false;
        }
    }
}
