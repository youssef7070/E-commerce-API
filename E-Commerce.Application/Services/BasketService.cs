using AutoMapper;
using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOS.Basket;
using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.Baskets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Services
{
    public class BasketService(IBasketRepository basketRepository, IMapper mapper) : IBasketService
    {



        public async Task<Result<BasketDto>> CreateOrUpdateBasketAsync(BasketDto basket, CancellationToken ct = default)
        {

            var CustomerBasket = mapper.Map<CustomerBasket>(basket);

            var basketResult = await basketRepository.CreateOrUpdateBasketAsync(CustomerBasket, ct:ct);

            return basketResult!= null ? Result<BasketDto>.Ok(mapper.Map<BasketDto>(basketResult)) : Result<BasketDto>.Fail(Error.Failure("Basket not found", "Basket not found"));

        }

        public async Task<Result<bool>> DeleteBasketAsync(string Id, CancellationToken ct = default)
        {
            
            var result = await basketRepository.DeleteBasketAsync(Id, ct:ct);

            return result ? Result<bool>.Ok(true) : Result<bool>.Fail( Error.Failure("Basket not found", "Basket not found"));


        }

        public async Task<Result<BasketDto>> GetBasketAsync(string Id, CancellationToken ct = default)
        {
            
            var basket = await basketRepository.GetBasketAsync(Id, ct:ct);

            return basket != null ? Result<BasketDto>.Ok(mapper.Map<BasketDto>(basket)) : Result<BasketDto>.Fail(Error.Failure("Basket not found", "Basket not found"));

        }


    }
}
