using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOS.Basket;
using E_Commerce.Application.Services;
using E_Commerce.Domain.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;

namespace E_Commerce.API.Controllers
{
    public class PaymentController : APIBaseController
    {
        private readonly IPaymentService _paymentService;
        private readonly PaymentGetwaySettings _stripeSettings;

        public PaymentController(IPaymentService paymentService, IOptions<PaymentGetwaySettings> options)
        {
            _paymentService = paymentService;
            _stripeSettings = options.Value;
        }

        [Authorize]
        [HttpPost("{basketId}")]
        [ProducesResponseType(typeof(BasketDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<BasketDto>> CreateOrUpdatePaymentIntent(string basketId, CancellationToken ct)
            => ToActionResult(await _paymentService.CreateOrUpdatePaymentIntentAsync(basketId, ct));

        [AllowAnonymous]
        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            try
            {
                if (!Request.Headers.TryGetValue("Stripe-Signature", out var stripeSignature))
                {
                    return BadRequest("Missing Stripe-Signature header.");
                }

                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    stripeSignature,
                    _stripeSettings.WebhookSecret);

                switch (stripeEvent.Type)
                {
                    case EventTypes.PaymentIntentSucceeded:
                        if (stripeEvent.Data.Object is PaymentIntent succeededPaymentIntent)
                        {
                            await _paymentService.PaymentSucceeded(succeededPaymentIntent.Id);
                        }
                        break;

                    case EventTypes.PaymentIntentPaymentFailed:
                        if (stripeEvent.Data.Object is PaymentIntent failedPaymentIntent)
                        {
                            await _paymentService.PaymentFailed(failedPaymentIntent.Id);
                        }
                        break;
                }

                return Ok();
            }
            catch (StripeException ex)
            {
                return BadRequest($"Stripe error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}



