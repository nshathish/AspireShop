using AspireShop.Api.Catalog.Application.Products.Services;
using AspireShop.Api.Catalog.Application.Categories.Services;
using AspireShop.Api.Catalog.Application.ProductPrices.Services;
using AspireShop.Api.Catalog.Common;
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

        services.AddCors(options =>
        {
            options.AddPolicy(CorsPolicies.Admin, policy =>
            {
                var adminOrigin =
                    configuration["Cors:WebAdminOrigin"];

                Console.WriteLine($"Admin Origin: {adminOrigin}");

                policy
                    .WithOrigins(adminOrigin!)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        services.AddScoped<ProductService>();
        services.AddScoped<ProductCacheService>();
        services.AddScoped<CategoryService>();
        services.AddScoped<ProductPriceService>();

        return services;
    }
}