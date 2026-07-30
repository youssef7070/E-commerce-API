using E_Commerce.Application.Contracts;
using E_Commerce.Domain.Contracts;
using E_Commerce.Infrastructure.Data;
using E_Commerce.Infrastructure.Identity.Data;
using E_Commerce.Infrastructure.Identity.Entities;
using E_Commerce.Infrastructure.Identity.Services;
using E_Commerce.Infrastructure.Payments;
using E_Commerce.Infrastructure.Repositories;
using E_Commerce.Infrastructure.Seeding;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure
{
    public static class InfrastructureServicesRegistrations
    {
            public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register DbContexts FIRST before any other services that depend on them
            services.AddDbContext<StoreDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddDbContext<StoreIdentityDbContexts>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("IdentityConnection"));
            });

            // Register Identity
            services.AddIdentityCore<ApplicationUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<StoreIdentityDbContexts>();

            services.AddScoped<IIdentityService, IdentityService>();

            // Register Data Seeders
            services.AddKeyedScoped<IDataSeeder, CatalogDataSeeder>("Catalog");
            services.AddKeyedScoped<IDataSeeder, IdentityDataSeeding>("Identity");

            // Register Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Register Redis (with proper error handling)
            services.AddSingleton<IConnectionMultiplexer>(config =>
            {
                var redisConnection = configuration.GetConnectionString("RedisConnection");
                if (string.IsNullOrEmpty(redisConnection))
                {
                    throw new InvalidOperationException("Redis connection string is missing from configuration.");
                }

                try
                {
                    return ConnectionMultiplexer.Connect(redisConnection);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to connect to Redis at '{redisConnection}'", ex);
                }
            });

            services.AddScoped<IBasketRepository, BasketRepository>();
            services.AddSingleton<ICasheRepository, CasheRepository>();



            services.AddSingleton<IPaymentGetway, StripePaymentGetway>();


            return services;
        }
    }
}
 