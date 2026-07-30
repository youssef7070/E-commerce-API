using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.Orders;
using E_Commerce.Domain.Entities.Products;
using E_Commerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.Seeding
{
    public class CatalogDataSeeder(StoreDbContext dbContext, ILogger<CatalogDataSeeder> logger) : IDataSeeder
    {
        public async Task SeedAsync(CancellationToken ct = default)
        {
            try
            {
                var seedRoot = Path.Combine(AppContext.BaseDirectory, "DataSeed");

                await SeedIfEmptyAsync<ProductBrand>(seedRoot, "brands.json", ct);

                await SeedIfEmptyAsync<ProductType>(seedRoot, "types.json", ct);

                await SeedIfEmptyAsync<Product>(seedRoot, "product.json", ct);

                await SeedIfEmptyAsync<DeliveryMethod>(seedRoot, "delivery.json", ct);

                await dbContext.SaveChangesAsync(ct);

                logger.LogInformation("Catalog data seeding completed successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Catalog Data Seeding Failed");
                throw;
            }
        }

        private async Task SeedIfEmptyAsync<T>(string root, string fileName, CancellationToken ct = default) where T : class
        {
            if (await dbContext.Set<T>().AnyAsync(ct))
            {
                return;
            }

            var path = Path.Combine(root, fileName);

            if (!File.Exists(path))
            {
                logger.LogWarning("Seed File Not Found: {Path}", path);
                return;
            }

            await using var stream = File.OpenRead(path);

            var items = await JsonSerializer.DeserializeAsync<List<T>>(
                stream,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
                ct);

            if (items?.Count > 0)
            {
                await dbContext.Set<T>().AddRangeAsync(items, ct);
            }
        }
    }
}