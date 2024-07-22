using Ecommerce.Shared.DTOs;

namespace Ecommerce.ServicesDependencies.MercadoPago
{
    public interface IMercadoPago
    {
        Task<string> CreatePreferentPayment(List<ListProductPreferenceMpDto> products, string orderId);
        Task<PaymentDetailsDto> InfoPayment(long paymentId);
    }
}
