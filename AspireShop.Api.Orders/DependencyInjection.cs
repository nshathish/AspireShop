using AspireShop.Api.Orders.Application.Orders.Services;
using AspireShop.Api.Orders.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AspireShop.Api.Orders;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<OrderDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("ordersdb")));

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("cache");
        });
        
        return services;
    }
}
