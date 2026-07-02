using E_Commerce.Application.Common;
using E_Commerce.Domain.Entities.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Specifications
{
    public class ProductWithBrandAndTypeSpecification:BaseSpecifications<Product,int>
    {

        // Get All

        public ProductWithBrandAndTypeSpecification(ProductQueryParams queryParams) : base
            (P => (!queryParams.BrandId.HasValue || P.BrandId == queryParams.BrandId.Value) && (!queryParams.TypeId.HasValue || P.TypeId == queryParams.TypeId.Value)
            && (string.IsNullOrEmpty(queryParams.SearchValue) || P.Name.ToLower().Contains(queryParams.SearchValue)))
        {

            AddInclude(P => P.ProductBrand);

            AddInclude(P => P.ProductType);

            switch(queryParams.Sort)
            {
                case ProductSortingOptions.NameAsc:
                    AddOrderBy(P => P.Name);
                    break;
                case ProductSortingOptions.NameDesc:
                    AddOrderByDescending(P => P.Name);
                    break;
                case ProductSortingOptions.PriceAsc:
                    AddOrderBy(P => P.Price);
                    break;
                case ProductSortingOptions.PriceDesc:
                    AddOrderByDescending(P => P.Price);
                    break;
                default:
                    AddOrderBy(P => P.Id);
                    break;
            }

            ApplyPagination(queryParams.PageSize, queryParams.PageIndex);

        }

        // Get By Id

        public ProductWithBrandAndTypeSpecification(int id):base(x=>x.Id==id)
        {

            AddInclude(P => P.ProductBrand);

            AddInclude(P => P.ProductType);

        }



    }
}
