using AspireShop.Api.Catalog.Application.Products.Services;
using AspireShop.Api.Catalog.Application.Categories.Services;
using AspireShop.Api.Catalog.Application.ProductPrices.Services;
using AspireShop.Api.Catalog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AspireShop.Api.Catalog;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<CatalogDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("catalogdb")));

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("cache");
        });

        services.AddScoped<ProductService>();
        services.AddScoped<ProductCacheService>();
        services.AddScoped<CategoryService>();
        services.AddScoped<ProductPriceService>();

        return services;
    }
}
