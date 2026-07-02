using E_Commerce.Application.Specifications;
using E_Commerce.Domain.Common;
using E_Commerce.Domain.Contracts;
using E_Commerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.Repositories
{
    public class GenericRepository<TEntity, TKey>(StoreDbContext dbContext) : IGenericRepository<TEntity, TKey> where TEntity : BaseEntity<TKey>
    {


        public void Add(TEntity entity)
            => dbContext.Set<TEntity>().Add(entity);


        public void Remove(TEntity entity)
            => dbContext.Set<TEntity>().Remove(entity);


        public void Update(TEntity entity)
            => dbContext.Set<TEntity>().Update(entity);




        public async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken ct = default)
             => await dbContext.Set<TEntity>().AsNoTracking().ToListAsync(ct);


        public async Task<TEntity?> GetByIdAsync(TKey id, CancellationToken ct = default)
            => await dbContext.Set<TEntity>().FindAsync([id!] , ct).AsTask();


        public async Task<IReadOnlyList<TEntity>> GetAllAsync(ISpecifications<TEntity, TKey> Spec, CancellationToken ct = default)
        {
            
            //// 1 - dbContext.Products
            //IQueryable<TEntity> query = dbContext.Set<TEntity>();

            //// 2- Include(P => P.Products).Include(P => P.ProductsBrand );
            //if(Spec != null)
            //{

            //    if(Spec.IncludeExpressions.Any())
            //    {

            //        foreach(var expressions in Spec.IncludeExpressions)
            //        {

            //            // 1 - dbContext.Products.Include(P => P.Products).Include(P => P.ProductsBrand );
            //            query = query.Include(expressions);


            //        }

            //    }

            //}

            var query = SpecificationEvaluator.CreateQuery(dbContext.Set<TEntity>(),Spec);

            return await query.ToListAsync(ct);

        }

        public async Task<TEntity?> GetByIdAsync(ISpecifications<TEntity, TKey> Spec, CancellationToken ct = default)
        {

            var query = SpecificationEvaluator.CreateQuery(dbContext.Set<TEntity>(), Spec);

            return await query.FirstOrDefaultAsync();

        }

        public Task<int> CountAsync(ISpecifications<TEntity, TKey> Spec, CancellationToken ct = default)
        {

            return SpecificationEvaluator.CreateQuery(dbContext.Set<TEntity>(), Spec).CountAsync(ct);

        }


    }
}
