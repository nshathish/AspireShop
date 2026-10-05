using AspireShop.Api.Catalog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AspireShop.Api.Catalog;

public static class DependencyInjection
{
    public static IServiceCollection AddCatalogInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<CatalogDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("catalogdb")));

        return services;
    }
}