using AutoMapper;
using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOS.Order;
using E_Commerce.Application.Specifications;
using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.Orders;
using E_Commerce.Domain.Entities.Products;

namespace E_Commerce.Application.Services
{
    public class OrderService(IMapper mapper, IUnitOfWork unitOfWork, IBasketRepository basketRepository) : IOrderService
    {
        public async Task<Result<OrderToReturnDto>> CreateOrderAsync(OrderDto orderDto, string email, CancellationToken ct = default)
        {
            var basket = await basketRepository.GetBasketAsync(orderDto.BasketId, ct);

            if (basket == null)
                return Result<OrderToReturnDto>.Fail(Error.NotFound("Basket Not Found", $"Basket with Id{orderDto.BasketId}Is Not Found"));

            if (basket.Items.Count == 0)
                return Result<OrderToReturnDto>.Fail(Error.NotFound("Basket Is Empty", $"Can Not Create with basket with Id : {orderDto.BasketId}"));

            var orderRepo = unitOfWork.GetRepository<Order, Guid>();

            var productRepo = unitOfWork.GetRepository<Product, int>();

            // Only check for existing order if PaymentId is not null
            if (!string.IsNullOrEmpty(basket.PaymentId))
            {
                var existingOrder = await orderRepo.GetByIdAsync(new PaymentIntentSpec(basket.PaymentId), ct);
                if (existingOrder is not null) orderRepo.Remove(existingOrder);
            }

            var productIds = basket.Items.Select(i => i.Id).ToHashSet();

            var products = (await productRepo.GetAllAsync(new ProductWithIdsSpecifications(productIds), ct)).ToDictionary(x => x.Id);

            var orderItems = new List<OrderItem>(basket.Items.Count);

            foreach (var item in basket.Items)
            {
                if (!products.TryGetValue(item.Id, out var product))
                    return Result<OrderToReturnDto>.Fail(Error.NotFound("product not found", $"product with id {item.Id} Is not Found"));

                orderItems.Add(new OrderItem
                {
                    Price = product.Price,
                    Quantity = item.Quantity,
                    Product = new ProductItemOrder
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        PictureUrl = product.PictureUrl
                    }
                });
            }

            var orderAddress = mapper.Map<OrderAddress>(orderDto.ShipToAddress);

            var deliveryRepo = unitOfWork.GetRepository<DeliveryMethod, int>();

            var deliveryMethod = await deliveryRepo.GetByIdAsync(orderDto.DeliveryMethodId, ct);

            if (deliveryMethod == null)
                return Result<OrderToReturnDto>.Fail(Error.NotFound("Delivery Method Not Found", $"DeliveryMathod wirh Id {orderDto.DeliveryMethodId} Is Not Found"));

            var subTotal = orderItems.Sum(x => x.Quantity * x.Price);
            var order = new Order(email, orderItems, orderAddress, deliveryMethod, subTotal);
            orderRepo.Add(order);
            var result = await unitOfWork.SaveChangesAsync(ct);
            if (result <= 0)
            {
                return Result<OrderToReturnDto>.Fail(Error.Failure("Order Save Faild", " Can not Create Order"));
            }
            await basketRepository.DeleteBasketAsync(orderDto.BasketId, ct);
            return mapper.Map<OrderToReturnDto>(order);
        }

        public async Task<Result<IReadOnlyList<DeliveryMethodDto>>> GetAllDeliveryMethodsAsync(CancellationToken ct = default)
        {
            var deliveryMethods = await unitOfWork.GetRepository<DeliveryMethod, int>().GetAllAsync(ct);
            return Result<IReadOnlyList<DeliveryMethodDto>>.Ok(mapper.Map<IReadOnlyList<DeliveryMethodDto>>(deliveryMethods));
        }

        public async Task<Result<IReadOnlyList<OrderToReturnDto>>> GetAllOrdersAsync(string email, CancellationToken ct = default)
        {
            var Orders = await unitOfWork.GetRepository<Order, Guid>()
                .GetAllAsync(new OrderSpecifications(email), ct);

            return Result<IReadOnlyList<OrderToReturnDto>>.Ok(mapper.Map<IReadOnlyList<OrderToReturnDto>>(Orders));
        }

        public async Task<Result<OrderToReturnDto>> GetOrderByIdAndEmailAsync(Guid Id, string email, CancellationToken ct = default)
        {
            var order = await unitOfWork.GetRepository<Order, Guid>().GetByIdAsync(new OrderSpecifications(Id, email), ct);

            if (order == null)
                return Result<OrderToReturnDto>.Fail(Error.NotFound("Order Not Found", $"Order with Id:{Id} Not Found "));

            return mapper.Map<OrderToReturnDto>(order);
        }
    }
}
