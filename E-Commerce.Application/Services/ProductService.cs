using AutoMapper;
using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOS.Products;
using E_Commerce.Application.Specifications;
using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Services
{
    public class ProductService:IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public IMapper _mapper { get; }


        public ProductService(IUnitOfWork unitOfWork , IMapper mapper)
        {
            
            _unitOfWork = unitOfWork;
           
            _mapper = mapper;

        }






        public async Task<Result<PaginatedResult<ProductDto>>> GetAllProductAsync(ProductQueryParams queryParams, CancellationToken ct = default)
        {

            var Spec = new ProductWithBrandAndTypeSpecification( queryParams);

            var Repo = _unitOfWork.GetRepository<Product, int>();

            var Products = await Repo.GetAllAsync(Spec , ct);

            var Data = _mapper.Map<IReadOnlyList<ProductDto>>(Products);

            var Count = new ProductCountSpecifications(queryParams);

            var TotalCount = await _unitOfWork.GetRepository<Product, int>().CountAsync(Count );

            var result = new PaginatedResult<ProductDto>(queryParams.PageIndex, queryParams.PageSize, TotalCount, Data);
        
            return Result<PaginatedResult<ProductDto>>.Ok(result);

        }



        public async Task<Result<ProductDto>> GetProductAsync(int id, CancellationToken ct = default)
        {

            var Spec = new ProductWithBrandAndTypeSpecification(id);
            
            var product = await _unitOfWork.GetRepository<Product, int>().GetByIdAsync(id , ct);

            if(product is null)
            {

                return Result<ProductDto>.Fail(Error.NotFound("Product.NotFound", $"Product With Id:{id} Was Not Found"));

            }

            return _mapper.Map<ProductDto>(product);

        }

        public async Task<Result<IReadOnlyList<TypeDto>>> GetAllTypesAsync(CancellationToken ct = default)
        {
            
            var types = await _unitOfWork.GetRepository<ProductType, int>().GetAllAsync(ct);

            return Result<IReadOnlyList<TypeDto>>.Ok(_mapper.Map<IReadOnlyList<TypeDto>>(types));


        }








        public async Task<Result<IReadOnlyList<BrandDto>>> GetAllBrandsAsync(CancellationToken ct = default)
        {
            
            var brands = await _unitOfWork.GetRepository<ProductBrand , int>().GetAllAsync(ct);

            return Result<IReadOnlyList<BrandDto>>.Ok(_mapper.Map<IReadOnlyList<BrandDto>>(brands));

        }


    }
}
