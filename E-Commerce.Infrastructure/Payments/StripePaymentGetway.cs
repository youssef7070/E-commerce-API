using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using Microsoft.Extensions.Options;
using Stripe;

namespace E_Commerce.Infrastructure.Payments
{
    public class StripePaymentGetway : IPaymentGetway
    {
        private readonly PaymentIntentService _paymentIntentService = new();

        public StripePaymentGetway(IOptions<PaymentGetwaySettings> options)
        {
            StripeConfiguration.ApiKey = options.Value.SecretKey;
        }

        public async Task<PaymentIntentResult> CreatePaymentIndentAsync(decimal amount, string currency, CancellationToken ct = default)
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)amount,
                Currency = currency.ToLowerInvariant(),
                PaymentMethodTypes = ["card"]
            };

            var requestOptions = new RequestOptions { IdempotencyKey = Guid.NewGuid().ToString() };
            var intent = await _paymentIntentService.CreateAsync(options, requestOptions, ct);

            return new PaymentIntentResult(intent.Id, intent.ClientSecret);
        }

        public async Task<PaymentIntentResult> UpdatePaymentIntentAsync(string paymentIntentId, decimal amount, CancellationToken ct = default)
        {
            var options = new PaymentIntentUpdateOptions
            {
                Amount = (long)amount
            };

            var requestOptions = new RequestOptions { IdempotencyKey = Guid.NewGuid().ToString() };
            var intent = await _paymentIntentService.UpdateAsync(paymentIntentId, options, requestOptions, ct);

            return new PaymentIntentResult(intent.Id, intent.ClientSecret);
        }
    }
}