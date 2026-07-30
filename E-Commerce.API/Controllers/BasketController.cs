using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOS.Basket;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{
   
    public class BasketController(IBasketService basketService) : APIBaseController
    {

        #region Get

        [HttpGet("({id}")]

        [ProducesResponseType(typeof(BasketDto), StatusCodes.Status200OK)]

        [ProducesResponseType(statusCode: StatusCodes.Status404NotFound)]


        public async Task<ActionResult<BasketDto>> GetBasket(string Id , CancellationToken ct)
        {

            var basket = await basketService.GetBasketAsync(Id, ct);

            return ToActionResult(basket);


        }


        #endregion



        #region Create Or Update

        [HttpPost]

        public async Task<ActionResult<BasketDto>> CreateOrUpdateBasket(BasketDto basketDto, CancellationToken ct)
        {

            var Saved = await basketService.CreateOrUpdateBasketAsync(basketDto, ct);
        
            return ToActionResult(Saved);
       
        }



        #endregion



        #region Delete

        [HttpDelete("({id}")]

        public async Task<ActionResult<bool>> DeleteBasket(string Id, CancellationToken ct)
        {

            var Deleted = await basketService.DeleteBasketAsync(Id, ct);
        
            return ToActionResult(Deleted);
       
        }




        #endregion





    }
}
