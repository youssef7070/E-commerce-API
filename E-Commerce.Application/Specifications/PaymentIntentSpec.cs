using E_Commerce.Domain.Entities.Orders;

namespace E_Commerce.Application.Specifications
{
    public class PaymentIntentSpec : BaseSpecifications<Order, Guid>
    {
        public PaymentIntentSpec(string paymentIntentId)
            : base(o => o.PaymentIntentId == paymentIntentId)
        {
            if (string.IsNullOrWhiteSpace(paymentIntentId))
                throw new ArgumentException("Payment intent ID cannot be null or empty.", nameof(paymentIntentId));
        }
    }
}