using Ecommerce.BackEnd.Data.Data;
using Ecommerce.BackEnd.Data.IRepository;
using Ecommerce.BackEnd.Data.Models;
using Microsoft.EntityFrameworkCore;
using ROP;

namespace Ecommerce.BackEnd.Data.Repository
{
    public class PurchaseRepository : IPurchaseRepository
    {
        private readonly ApplicationDbContext _db;

        public PurchaseRepository(ApplicationDbContext db)
        {
            _db = db;
        }
      
        public async Task<Result<ShipmentInfo>> GetShippingInfo(string id)
        {
            try
            {
                var result = await _db.ShipmentInfo.FirstOrDefaultAsync(s => s.ShipmentInfo_Id.ToString() == id);

                if (result == null)
                    return Result.NotFound<ShipmentInfo>("Shippment not found");

                return result;
            }
            catch(Exception ex)
            {
                return Result.Failure<ShipmentInfo>("An error occurred while fetching products from the database" + ex.InnerException);
            }
        }
        public async Task<Result<Unit>> UpdateShippingInfo(ShipmentInfo shipmentInfo, Guid id)
        {
            try
            {
                var info = await _db.ShipmentInfo.FirstOrDefaultAsync(i => i.ShipmentInfo_Id == id);

                if (info == null)
                    return Result.Failure<Unit>("Shipping information not found.");

                info.Name = shipmentInfo.Name;
                info.LastName = shipmentInfo.LastName;
                info.Address = shipmentInfo.Address;
                info.CodigoPostal = shipmentInfo.CodigoPostal;
                info.AditionalInformation = shipmentInfo.AditionalInformation;
                info.State = shipmentInfo.State;
                info.Country = shipmentInfo.Country;
                info.Locality = shipmentInfo.Locality;
                info.Colony = shipmentInfo.Colony;
                info.Phone = shipmentInfo.Phone;
                info.Email = shipmentInfo.Email;

                _db.ShipmentInfo.Update(info);

                var success = await Save();
                return !success
                    ? Result.Failure<Unit>("Failed to update shipping information")
                    : Result.Unit;
            }
            catch (Exception ex)
            {
                return Result.Failure<Unit>($"Error updating shipping information: {ex.Message}");
            }
        }

        public async Task<bool> IsShipmentInfoFromUser(string shipmentId, string userId)
        {
            var result = await _db.ShipmentInfo.Where(s => s.User_Id == userId && s.ShipmentInfo_Id == Guid.Parse(shipmentId))
                        .ToListAsync();
            if(result == null)
            {
                return false;
            }
            return true;
        }

        public async Task<Result<List<ShipmentInfo>>> GetShippingInfoByUser(string userId)
        {
            try
            {
                var result = await _db.ShipmentInfo
                    .Where(s => s.User_Id == userId)
                    .ToListAsync();

                return result == null || result.Count == 0
                    ? Result.Failure<List<ShipmentInfo>>("No se encontraron envíos para el usuario especificado")
                    : Result.Success(result);
            }
            catch (Exception ex)
            {
                return Result.Failure<List<ShipmentInfo>>($"An error occurred while fetching data from the database: {ex.Message}");
            }
        }

        public async Task<Result<Order>> GetOrder(Guid id)
        {
            try
            {
                var result = await _db.Orders.FirstOrDefaultAsync(o => o.OrderId == id);
                return result is not null
                    ? result
                    : Result.NotFound<Order>("Order is not found");
            }
            catch(Exception ex)
            {
                return Result.Failure<Order>("An error occurred while fetching data from the database" + ex.InnerException);
            }
        }

        public async Task<bool> UpdateOrder(Order order)
        {
            var result = await _db.Orders.FirstOrDefaultAsync(o => o.OrderId == order.OrderId);
            if (result == null)
                return false;
           
            result.OrderStatusId = order.OrderStatusId;
            result.IsPaid = order.IsPaid;
            _db.Orders.Update(result);
            return true;
        }

        public async Task<Result<List<Order>>> GetPurchases(string id)
        {
            try
            {
                var result = await _db.Orders.Where(o => o.UserId.ToUpper() == id.ToUpper() && o.IsPaid == true)
                        .Include(i => i.OrderItems)
                           .ThenInclude(iv => iv.product)
                               .ThenInclude(pv => pv.ImagesProduct)
                        .Include(o => o.PaymentDetails)
                        .Include(s => s.OrderStatus)
                        .OrderByDescending(o => o.PurchaseDate)
                        .ToListAsync();

                return result == null || result.Count == 0
                    ? Result.NotFound<List<Order>>("Product not found")
                    : result;
            }
            catch(Exception ex)
            {
                return Result.Failure<List<Order>>("An error occurred while fetching orders from the database" + ex.InnerException);
            }
        }

        public async Task<Result<List<Order>>> GetPurchaseByOrder(Guid orderId)
        {
            try
            {
                var result = await _db.Orders
                    .Where(o => o.OrderId == orderId && o.IsPaid)
                    .Include(i => i.OrderItems)
                        .ThenInclude(iv => iv.product)
                            .ThenInclude(pv => pv.ImagesProduct)
                    .Include(o => o.PaymentDetails)
                    .Include(s => s.OrderStatus)
                    .OrderByDescending(o => o.PurchaseDate)
                    .ToListAsync();

                if (result == null || result.Count == 0)
                    return Result.Failure<List<Order>>("No se encontraron compras para el pedido especificado.");

                return Result.Success(result);
            }
            catch (Exception ex)
            {
                return Result.Failure<List<Order>>($"An error occurred while fetching data from the database: {ex.Message}");
            }
        }

        public async Task<Result<DiscountCoupon>> GetDiscountCoupon(string coupon)
        {
            try
            {
                var couponEntity = await _db.DiscountCoupons
               .FirstOrDefaultAsync(d => d.Coupon == coupon);

                return couponEntity is not null
                    ? couponEntity
                    : Result.NotFound<DiscountCoupon>("Discount not found");
            }catch(Exception ex)
            {
                return Result.Failure<DiscountCoupon>("An error occurred while fetching products from the database" + ex.InnerException);
            }
        }

        public async Task<bool> Save()
        {
            return await _db.SaveChangesAsync() > 0 ? true : false;
        }
    }
}
