using AspireShop.Api.Orders.Api.Contracts.Orders;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace AspireShop.Api.Orders.Application.Orders.Services;

public sealed class OrderCacheService(IDistributedCache cache)
{
    private const string AllOrdersKey = "orders:order:all";

    public async Task<IReadOnlyCollection<OrderResponse>?> GetAllAsync(CancellationToken cancellationToken)
    {
        var cached = await cache.GetStringAsync(AllOrdersKey, cancellationToken);
        return cached is null ? null : JsonSerializer.Deserialize<List<OrderResponse>>(cached);
    }

    public async Task SetAllAsync(IReadOnlyCollection<OrderResponse> orders, CancellationToken cancellationToken)
    {
        await cache.SetStringAsync(
            AllOrdersKey,
            JsonSerializer.Serialize(orders),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            },
            cancellationToken);
    }

    public Task RemoveAllAsync(CancellationToken cancellationToken)
        => cache.RemoveAsync(AllOrdersKey, cancellationToken);
}
