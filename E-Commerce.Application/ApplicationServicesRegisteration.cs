using E_Commerce.Application.Contracts;
using E_Commerce.Application.Services;
using E_Commerce.Domain.Contracts;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application
{
    public static class ApplicationServicesRegisteration
    {

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {

           services.AddAutoMapper(typeof(ApplicationServicesRegisteration).Assembly);

           services.AddScoped<IProductService, ProductService>();

           services.AddScoped<IBasketService, BasketService>();

           services.AddSingleton<ICasheService, CasheService>();

           services.AddScoped<IAuthenticationService, AuthenticationService>();

            services.AddScoped<IOrderService, OrderService>();

            services.AddScoped<IPaymentService, PaymentService>();

           return services;

        }




    }
}
