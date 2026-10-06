using AspireShop.Api.Catalog.Api.Contracts.Products;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace AspireShop.Api.Catalog.Application.Products.Services;

public sealed class ProductCacheService(IDistributedCache cache)
{
    private const string AllProductsKey = "catalog:product:all";

    public async Task<IReadOnlyCollection<ProductResponse>?> GetAllAsync(
        CancellationToken cancellationToken)
    {
        var cached = await cache.GetStringAsync(
            AllProductsKey,
            cancellationToken);

        if (cached is null)
            return null;

        return JsonSerializer.Deserialize<List<ProductResponse>>(cached);
    }

    public async Task SetAllAsync(
        IReadOnlyCollection<ProductResponse> products,
        CancellationToken cancellationToken)
    {
        await cache.SetStringAsync(
            AllProductsKey,
            JsonSerializer.Serialize(products),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            },
            cancellationToken);
    }

    public async Task RemoveAllAsync(
        CancellationToken cancellationToken)
    {
        await cache.RemoveAsync(
            AllProductsKey,
            cancellationToken);
    }

    public async Task<ProductResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var cached = await cache.GetStringAsync(
            GetProductKey(id),
            cancellationToken);

        if (cached is null)
            return null;

        return JsonSerializer.Deserialize<ProductResponse>(cached);
    }

    public async Task SetByIdAsync(
        ProductResponse product,
        CancellationToken cancellationToken)
    {
        await cache.SetStringAsync(
            GetProductKey(product.Id),
            JsonSerializer.Serialize(product),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            },
            cancellationToken);
    }

    public async Task RemoveByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        await cache.RemoveAsync(
            GetProductKey(id),
            cancellationToken);
    }

    private static string GetProductKey(Guid id)
        => $"catalog:product:{id}";
}