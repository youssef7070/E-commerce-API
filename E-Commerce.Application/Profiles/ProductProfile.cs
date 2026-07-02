using E_Commerce.Application.DTOS.Products;
using E_Commerce.Domain.Entities.Products;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Profiles
{
    public class ProductProfile : Profile
    {


        public ProductProfile()
        {

            CreateMap<Product, ProductDto>()
                    .ForMember(d => d.ProductBrand, p => p.MapFrom(s => s.ProductBrand.Name))
                    .ForMember(d => d.ProductType, p => p.MapFrom(s => s.ProductType.Name))
                    .ForMember(d => d.PictureUrl, o => o.MapFrom<PictureUrlResolver>());

            CreateMap<ProductBrand, BrandDto>();

            CreateMap<ProductType, TypeDto>();

        }

        
    }
}
