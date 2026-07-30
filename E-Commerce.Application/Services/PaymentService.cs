using AutoMapper;
using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOS.Basket;
using E_Commerce.Application.Specifications;
using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.Orders;
using E_Commerce.Domain.Entities.Products;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentGetway _paymentGetway;
        private readonly PaymentGetwaySettings _stripeSettings;
        private readonly IMapper _mapper;

        public PaymentService(IBasketRepository basketRepository , IUnitOfWork unitOfWork , IPaymentGetway paymentGetway , IOptions<PaymentGetwaySettings> stripeSettings , IMapper mapper  )
        {
           _basketRepository = basketRepository;
           _unitOfWork = unitOfWork;
           _paymentGetway = paymentGetway;
           _stripeSettings = stripeSettings.Value;
           _mapper = mapper;
        }



        public async Task<Result<BasketDto>> CreateOrUpdatePaymentIntentAsync(string basketId, CancellationToken ct = default)
        {

            var basket = await _basketRepository.GetBasketAsync(basketId,ct);

            if (basket == null)
                return Result<BasketDto>.Fail(Error.NotFound("Basket not found"));

            if (basket.Items.Count == 0)
                return Result<BasketDto>.Fail(Error.Validation("Basket is empty"));

            var productRepo = _unitOfWork.GetRepository<Product , int>();

            var productsIds = basket.Items.Select( i => i.Id ).ToHashSet();

            var products = (await productRepo.GetAllAsync(new ProductWithIdsSpecifications(productsIds), ct)).ToDictionary(x => x.Id);

            foreach ( var item in basket.Items)
            {

                if (!products.TryGetValue(item.Id, out var product))
                    return Result<BasketDto>.Fail(Error.NotFound("product not found"));

                item.Price = product.Price;

            }

            var deliveryRepo = _unitOfWork.GetRepository<DeliveryMethod , int>();

            if (!basket.DeliveryMethodId.HasValue)
                return Result<BasketDto>.Fail(Error.Validation("Basket.DeliveryMethodId", "Delivery method is required"));

            var deliveryMethod = await deliveryRepo.GetByIdAsync(basket.DeliveryMethodId.Value, ct);

            if (deliveryMethod == null)
                return Result<BasketDto>.Fail(Error.NotFound("delivery metod not found"));

            basket.ShippingPrice = deliveryMethod.Cost;

            var subTotal = basket.Items.Sum(i => i.Quantity * i.Price);

            var amount = (long)Math.Round((subTotal + deliveryMethod.Cost) * 100m);

            if (!string.IsNullOrEmpty(basket.PaymentId))
            {

                await _paymentGetway.UpdatePaymentIntentAsync(basket.PaymentId, amount, ct);

            }
            else
            {

                var result = await _paymentGetway.CreatePaymentIndentAsync(amount, _stripeSettings.DefaultCurrency, ct);

                basket.PaymentId = result.PaymentIntendId;

                basket.ClientSecret = result.ClientSecret;

            }

            await _basketRepository.CreateOrUpdateBasketAsync(basket , ct:ct);

            return _mapper.Map<BasketDto>(basket);

        }

       

        public async Task PaymentFailed(string paymentIntentId)
        {

            var OrderRepo = _unitOfWork.GetRepository<Order, Guid>();

            var order = await OrderRepo.GetByIdAsync(new PaymentIntentSpec(paymentIntentId));

            if (order == null)
                return;

            order.Status = OrderStatus.PaymentFailed;

            await _unitOfWork.SaveChangesAsync();




        }

        public async Task PaymentSucceeded(string paymentIntentId)
        {

            var OrderRepo = _unitOfWork.GetRepository<Order , Guid>();

            var order = await OrderRepo.GetByIdAsync( new PaymentIntentSpec( paymentIntentId ) );

            if (order == null)
                return;

            order.Status = OrderStatus.PaymentReceived;

            await _unitOfWork.SaveChangesAsync();

        }
    }
}
