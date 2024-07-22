using Ecommerce.Data.IRepository;
using Ecommerce.Model;
using Ecommerce.Services.IServices;
using Ecommerce.ServicesDependencies.MercadoPago;
using Ecommerce.Shared.DTOs;
using Ecommerce.Shared.Mapper;
using System.Net;

namespace Ecommerce.Services
{
    public class PurchaseOrdersServices : IPurchaseOrdersServices
    {
        private readonly IPurchaseRepository _purchase;
        private readonly IUserRepository _user;
        private readonly IProductRepository _product;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapperDto _mapper;
        private readonly IMercadoPago _mp;
        private readonly HttpClient _httpClient;

        public PurchaseOrdersServices(IPurchaseRepository Purchase, 
                                      IMapperDto mapper, 
                                      IUserRepository user, 
                                      IUnitOfWork unitOfWork, 
                                      IProductRepository product, 
                                      IMercadoPago mp)
        {
            _purchase = Purchase;
            _mapper = mapper;
            _user = user;
            _product = product;
            _unitOfWork = unitOfWork;
            _mp = mp;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://api.mercadopago.com");
        }

        public async Task<ApiResponse> CreatePurchaseOrder(List<ListProductPreferenceMpDto> products, string shippmentId)
        {
            List<SizeStock> sizeStocks = new List<SizeStock>();
            List<OrderItem> orderItems = new List<OrderItem>();

            foreach (var product in products)
            {
                //Verify Stock
                var result = await _product.GetProductStock(product.Product_Id, product.Size);
                if (result.Stock < product.Quantity || result == null)
                {
                    return ApiResponse.Error("No stock for any product", HttpStatusCode.NotFound);
                }

                result.Stock -= product.Quantity;
                sizeStocks.Add(result);
            } 
            
            foreach (var product in products)
            {
                //Verify Price
                var result = await _product.GetProductById(product.Product_Id);
                if (product.Price != result.Price && product.Discount != result.DiscountRate)
                {
                    return ApiResponse.Error("Inconsistency in prices submitted", HttpStatusCode.BadRequest);
                }
            }

            Guid idOrder = Guid.NewGuid();
            Order Order = new()
            {
                OrderId = idOrder,
                IsPaid = false,
                UserId = products[0].User_Id,
                OrderStatusId = 1,
                ShippmentInfo_Id = Guid.Parse(shippmentId)
            };

            // Create Order Item
            foreach (var p in products)
            {
                var UnitPrice = p.Price;
                if (p.Discount > 0)
                {
                    UnitPrice = UnitPrice - (UnitPrice * p.Discount / 100);
                }
                OrderItem order = new OrderItem()
                {
                    OrderId = idOrder,
                    ProductId = p.Product_Id,
                    Size = p.Size,
                    Quantity = p.Quantity,
                    UnitPrice = UnitPrice,
                    TotalPrice = UnitPrice * p.Quantity,
                };
                orderItems.Add(order);
            }

            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    // Reservar el stock
                    foreach(var ss in sizeStocks)
                    {
                        await _unitOfWork.Products.UpdateProductStock(ss);    
                    }

                    // Create order and order item
                    await _unitOfWork.Orders.CreateOrder(Order);
                    await _unitOfWork.SaveAsync();

                    foreach (var o in orderItems)
                    {
                        await _unitOfWork.Orders.CreateOrderItem(o);
                    }

                    // Guardar los cambios
                    var url = await _mp.CreatePreferentPayment(products, idOrder.ToString());
                    
                    if(url == null)
                    {
                        await transaction.RollbackAsync();
                        return ApiResponse.Error("Error to generate purchase order", HttpStatusCode.InternalServerError);
                    }
                    var saveResult = await _unitOfWork.SaveAsync();                 
                    
                    await transaction.CommitAsync();
                    return ApiResponse.Success("", new { PaymentUrl = url });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return ApiResponse.Error("Error to generate purchase order", HttpStatusCode.InternalServerError);
                }
            }
        }

        public async Task<bool> GetInfoPaymentMPServices(long id)
        {
            var paymentInfo = await _mp.InfoPayment(id);

            if(paymentInfo == null)
            {
                return false;
            }

            var order = await _purchase.GetOrder(Guid.Parse(paymentInfo.OrderId));

            if(order == null)
            {
                return false;
            }

            order.IsPaid = true;
            order.OrderStatusId = 2;

            var paymentDetails = _mapper.ToPaymentDetails(paymentInfo);
            paymentDetails.OrderId = order.OrderId;

            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    await _unitOfWork.Orders.UpdateOrder(order);
                    await _unitOfWork.Orders.CreatePaymentDetails(paymentDetails);

                    var saveResult = await _unitOfWork.SaveAsync();
                    await transaction.CommitAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return false;
                }
            }
        }

        public async Task<ApiResponse> CreateShippingInfoServices(string id, ShipmentInfoDto shipmentInfo)
        {
            var user = await _user.GetUserById(id);
            if(user == null)
            {
                return ApiResponse.Error("Id does not correspond to any user", HttpStatusCode.BadRequest);
            }

            var info = _mapper.ToShipmentInfo(shipmentInfo);
            info.User_Id = id;
                
            var result = await _purchase.CreateShippingInfo(info);
            if(result == null)
            {
                return ApiResponse.Error("Server error to save info", HttpStatusCode.InternalServerError);
            }

            var response = _mapper.ToShipmentInfoResponseDto(result);

            return ApiResponse.Success("", response);
        }

        public async Task<ApiResponse> GetShippingInfoServices(string id)
        {
            var result = await _purchase.GetShippingInfo(id);

            if(result == null)
            {
                return ApiResponse.Error("An error occurred while getting the address", HttpStatusCode.InternalServerError);
            }

            var response = _mapper.ToShipmentInfoResponseDto(result);

            return ApiResponse.Success("", response);
        }

        public async Task<ApiResponse> UpdateShippingInfoServices(ShipmentInfoDto shipmentInfo, string id)
        {
            var infoMapper = _mapper.ToShipmentInfo(shipmentInfo);

            var result = await _purchase.UpdateShippingInfo(infoMapper, Guid.Parse(id));

            if (!result)
            {
                return ApiResponse.Error("Internal error to update shipment info", HttpStatusCode.InternalServerError);
            }

            return ApiResponse.Success("");
        }

        public async Task<ApiResponse> GetShippingInfoByUserServices(string id)
        {
            var result = await _purchase.GetShippingInfoByUser(id);

            if(result == null)
            {
                return ApiResponse.Error("An error occurred while getting the address", HttpStatusCode.InternalServerError);
            }

            var response = new List<ShippmentInfoResponseDto>();
            foreach (var item in result)
            {
                response.Add(_mapper.ToShipmentInfoResponseDto(item));
            }
            
            return ApiResponse.Success("", response);
        }

        public async Task<ApiResponse> GetPurchasesInfoServices(string id)
        {
            var result = await _purchase.GetPurchases(id);

            if(result == null || !result.Any())
            {
                return ApiResponse.Error("Not found orders", HttpStatusCode.NotFound);
            } 
          
            var response = new List<PurchasesInfoDto>();

            foreach (var item in result)
            {
                var itemMapper = _mapper.ToPurchaseInfoDto(item);
                response.Add(itemMapper);
            }
            
            return ApiResponse.Success("", response);
        }
    }
}
