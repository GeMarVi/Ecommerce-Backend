using Ecommerce.Model;
using Ecommerce.Shared.DTOs;

namespace Ecommerce.Shared.Mapper
{
    public class MapperDto : IMapperDto
    {
        public ApplicationUser ToApplicationUser(RegisterUserDto registerUserDto)
        {
            return new ApplicationUser
            {
                UserName = registerUserDto.Email,
                Email = registerUserDto.Email,
                EmailConfirmed = false,
            };
        }

        public ApplicationUser ToApplicationUser(LoginAdminDto loginAdmin)
        {
            return new ApplicationUser
            {
                UserName = loginAdmin.Email,
                Email = loginAdmin.Email,
                EmailConfirmed = false,
            };
        }

        public UserResponseDto ToUserResponseDto(ApplicationUser user, JwtGeneratorResponseDto token)
        {
            return new UserResponseDto
            {
                User_Id = user.Id,
                Email = user.Email,
                Token = token.Token,
                RefreshToken = token.RefreshToken,
            };
        }

        public Product ToProduct(CreateProductDto createProductDto)
        {
            return new Product
            {
                ProductName = createProductDto.ProductName,
                Description = createProductDto.Description,
                Model = createProductDto.Model,
                Brand = createProductDto.Brand,
                Gender = createProductDto.Gender,
                ClosureType = createProductDto.ClosureType,
                OuterMaterial = createProductDto.OuterMaterial,
                SoleMaterial = createProductDto.SoleMaterial,
                TypeDeport = createProductDto.TypeDeport,
                Color = createProductDto.Color,
                Price = createProductDto.Price,
                hasDiscount = createProductDto.HasDiscount,
                ProductStatus = createProductDto.ProductStatus,
                DiscountRate = createProductDto.DiscountRate,
            };
        }

        public CompleteProductResponseDto ToCompleteProductResponseDto(Product product)
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

        public ProductResponseDto ToProductResponseDto(Product product)
        {
            return new ProductResponseDto
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
                ImagesProduct = product.ImagesProduct
                    .Select(i => new ImagesProductDto
                    {
                        Image_Id = i.Images_Id,
                        Url = i.Url
                    }).ToList(),
                SizeStocks = product.sizeStocks
                    .Select(s => new SizeStockResponseDto
                    {
                        Size = s.Size,
                        Stock = s.Stock
                    }).ToList()
            };
        }

        public ShipmentInfo ToShipmentInfo(ShipmentInfoDto shipmentInfo)
        {
            return new ShipmentInfo
            {
                Name = shipmentInfo.Name,
                LastName = shipmentInfo.LastName,
                street = shipmentInfo.street,
                ExteriorNumber = shipmentInfo.ExteriorNumber,
                InteriorNumber = shipmentInfo.InteriorNumber,
                CodigoPostal = shipmentInfo.CodigoPostal,
                City = shipmentInfo.City,
                Municipality = shipmentInfo.Municipality,
                Colony = shipmentInfo.Colony,
                State = shipmentInfo.State,
                Country = shipmentInfo.Country,
                Phone = shipmentInfo.Phone,
                Email = shipmentInfo.Email,
                AditionalInformation = shipmentInfo.AditionalInformation
            };
        } 
        
        public ShippmentInfoResponseDto ToShipmentInfoResponseDto(ShipmentInfo shipmentInfo)
        {
            return new ShippmentInfoResponseDto
            {
                ShipmentInfo_Id = shipmentInfo.ShipmentInfo_Id,
                Name = shipmentInfo.Name,
                LastName = shipmentInfo.LastName,
                street = shipmentInfo.street,
                ExteriorNumber = shipmentInfo.ExteriorNumber,
                InteriorNumber = shipmentInfo.InteriorNumber,
                CodigoPostal = shipmentInfo.CodigoPostal,
                City = shipmentInfo.City,
                Municipality = shipmentInfo.Municipality,
                Colony = shipmentInfo.Colony,
                State = shipmentInfo.State,
                Country = shipmentInfo.Country,
                Phone = shipmentInfo.Phone,
                Email = shipmentInfo.Email,
                user_Id = shipmentInfo.User_Id,
                AditionalInformation = shipmentInfo.AditionalInformation                
            };
        }

        public PaymentDetails ToPaymentDetails(PaymentDetailsDto paymentDetailsDto)
        {
            return new PaymentDetails
            {
                TransactionId = paymentDetailsDto.TransactionId,
                ApprovedDate = paymentDetailsDto.ApprovedDate,
                PaymentStatus = paymentDetailsDto.PaymentStatus,
                TransactionAmount = paymentDetailsDto.TransactionAmount,
                NetReceivedAmount = paymentDetailsDto.NetReceivedAmount,
            };
        }

        public PurchasesInfoDto ToPurchaseInfoDto(Order order)
        {
            return new PurchasesInfoDto
            {
                OrderId = order.OrderId,
                PurchaseDate = order.PurchaseDate,
                IsPaid = order.IsPaid,
                UserId = order.UserId,
                OrderStatus = order.OrderStatus.Status,
                OrderItems = order.OrderItems.Select(oi => new ItemDto
                {
                    OrderId = oi.OrderId,
                    Size = oi.Size,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    TotalPrice = oi.TotalPrice,
                    product = new OrderItemProductDto
                    {
                        ProductName = oi.product.ProductName,
                        Brand = oi.product.Brand,
                        Model = oi.product.Model,
                        Color = oi.product.Color,
                        Price = oi.product.Price,
                        ProductStatus = oi.product.ProductStatus,
                        DiscountRate = oi.product.DiscountRate,
                        image = oi.product.ImagesProduct[0].Url
                    }
                }).ToList(),
                paymentDetails = new DetailsDto
                {
                    ApprovedDate = order.paymentDetails.ApprovedDate,
                    PaymentStatus = order.paymentDetails.PaymentStatus,
                    TransactionAmount = order.paymentDetails.TransactionAmount,
                }
            };
        }
    }
}
