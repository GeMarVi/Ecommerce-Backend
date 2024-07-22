using Ecommerce.Model;
using Ecommerce.Shared.DTOs;

namespace Ecommerce.Shared.Mapper
{
    public interface IMapperDto
    {
        ApplicationUser ToApplicationUser(RegisterUserDto registerUserDto);
        ApplicationUser ToApplicationUser(LoginAdminDto loginAdmin);
        UserResponseDto ToUserResponseDto(ApplicationUser user, JwtGeneratorResponseDto token);
        Product ToProduct(CreateProductDto createProductDto);
        CompleteProductResponseDto ToCompleteProductResponseDto(Product product);
        ProductResponseDto ToProductResponseDto(Product product);
        ShipmentInfo ToShipmentInfo(ShipmentInfoDto shipmentInfo);
        ShippmentInfoResponseDto ToShipmentInfoResponseDto(ShipmentInfo shipmentInfo);
        PaymentDetails ToPaymentDetails(PaymentDetailsDto paymentDetailsDto);
        PurchasesInfoDto ToPurchaseInfoDto(Order order);
    }
}
