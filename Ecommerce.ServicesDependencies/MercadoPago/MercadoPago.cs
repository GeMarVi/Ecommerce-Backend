using Ecommerce.Shared.DTOs;
using MercadoPago.Client;
using MercadoPago.Client.Payment;
using MercadoPago.Client.Preference;
using MercadoPago.Config;
using MercadoPago.Resource.Preference;

namespace Ecommerce.ServicesDependencies.MercadoPago
{
    public class MP : IMercadoPago
    {
        public MP()
        {
            MercadoPagoConfig.AccessToken = "APP_USR-2726499431249145-061601-4da1f64f0c38d6104d851b86a7d1436a-1825700451";
        }

        public async Task<string> CreatePreferentPayment(List<ListProductPreferenceMpDto> products, string orderId)
        {
            var items = new List<PreferenceItemRequest>();

            foreach (var item in products)
            {
                items.Add(new PreferenceItemRequest
                {
                    Title = item.Name,
                    Quantity = item.Quantity,
                    CurrencyId = "MXN",
                    UnitPrice = item.Price - (item.Price * item.Discount / 100),
                    Id = orderId,
                });
            }

            var request = new PreferenceRequest
            {
                Items = items,
                PaymentMethods = new PreferencePaymentMethodsRequest
                {
                    ExcludedPaymentMethods = new List<PreferencePaymentMethodRequest>
                    {
                        new PreferencePaymentMethodRequest { Id = "amex" },
                        new PreferencePaymentMethodRequest { Id = "visa" }
                    },
                    ExcludedPaymentTypes = new List<PreferencePaymentTypeRequest>
                    {
                        new PreferencePaymentTypeRequest { Id = "bank_transfer" },
                        new PreferencePaymentTypeRequest { Id = "ticket" }
                    }
                },
                BinaryMode = true,
                NotificationUrl = "https://tbzr7tpv-7004.usw3.devtunnels.ms/api/Payment/webhook",
                BackUrls = new PreferenceBackUrlsRequest
                {
                    Success = "http://localhost:5173/my-shopping",
                    Failure = ""
                }
            };

            var client = new PreferenceClient();
            Preference preference = await client.CreateAsync(request);
            return preference.InitPoint;
        }

        public async Task<PaymentDetailsDto> InfoPayment(long paymentId)
        {
            var client = new PaymentClient();
            var token = new RequestOptions()
            {
                AccessToken = "APP_USR-2726499431249145-061601-4da1f64f0c38d6104d851b86a7d1436a-1825700451",
            };
           
            try
            {
                var payment = await client.GetAsync(paymentId, token);

                var result = new PaymentDetailsDto()
                {
                    TransactionId = payment.Id,
                    ApprovedDate = payment.DateApproved,
                    PaymentStatus = payment.Status,
                    TransactionAmount = payment.TransactionAmount,
                    NetReceivedAmount = payment.TransactionDetails.NetReceivedAmount,
                    OrderId = payment.AdditionalInfo.Items[0].Id
                };

                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}


