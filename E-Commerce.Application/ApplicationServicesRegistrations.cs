using E_Commerce.Application.Contracts;
using E_Commerce.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace E_Commerce.Application
{
    public static class ApplicationServicesRegistrations
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Application services are already registered in InfrastructureServicesRegistrations
            // This method is kept for organizational purposes and future application-level services

            return services;
        }
    }
}