using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOS.Products;
using E_Commerce.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{
  



    public class ProductController(IProductService Productservice) : APIBaseController
    {

        #region Get All Products

        [HttpGet]

        [ProducesResponseType(typeof(ProductDto) , StatusCodes.Status200OK)]

        public async Task<ActionResult<PaginatedResult<ProductDto>>>GetAllProducts(ProductQueryParams queryParams , CancellationToken ct)
        {

            var Products = await Productservice.GetAllProductAsync(queryParams , ct);

            return ToActionResult(Products);


        }



        #endregion


        #region Get Product

        [HttpGet("{id:int}")]

        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]

        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<ProductDto>>GetProduct(int id , CancellationToken ct)
        {

            var product = await Productservice.GetProductAsync(id ,ct);

            return ToActionResult(product);

        }

        [HttpGet("brands")]

        public async Task<ActionResult<IReadOnlyList<BrandDto>>>GetAllBrands(CancellationToken ct)
          =>ToActionResult(await Productservice.GetAllBrandsAsync(ct));



        [HttpGet("types")]

        public async Task<ActionResult<IReadOnlyList<TypeDto>>> GetAllTypes(CancellationToken ct)
        => ToActionResult(await Productservice.GetAllTypesAsync(ct));



        #endregion




    }
}
