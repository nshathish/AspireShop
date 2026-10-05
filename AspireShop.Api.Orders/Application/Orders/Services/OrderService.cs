using AspireShop.Api.Orders.Api.Contracts.Orders;
using AspireShop.Api.Orders.Domain.Entities;
using AspireShop.Api.Orders.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AspireShop.Api.Orders.Application.Orders.Services;

public sealed class OrderService(OrderDbContext dbContext, OrderCacheService cache)
{
    public async Task<List<OrderResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var cached = await cache.GetAllAsync(cancellationToken);
        if (cached is not null)
            return cached.ToList();

        var orders = await dbContext.Orders
            .AsNoTracking()
            .OrderByDescending(order => order.CreatedAt)
            .Select(order => new OrderResponse(
                order.Id,
                order.CustomerName,
                order.CustomerEmail,
                order.Total,
                order.Status,
                order.CreatedAt))
            .ToListAsync(cancellationToken);

        await cache.SetAllAsync(orders, cancellationToken);
        return orders;
    }

    public async Task<OrderResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var order = await dbContext.Orders
            .AsNoTracking()
            .SingleOrDefaultAsync(order => order.Id == id, cancellationToken);

        return order is null ? null : ToResponse(order);
    }

    public async Task<OrderOperationResult<OrderResponse>> CreateAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerName = request.CustomerName,
            CustomerEmail = request.CustomerEmail,
            Total = request.Total,
            Status = "Pending",
            CreatedAt = DateTimeOffset.UtcNow
        };

        await dbContext.Orders.AddAsync(order, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        await cache.RemoveAllAsync(cancellationToken);

        return OrderOperationResult<OrderResponse>.Success(ToResponse(order));
    }

    public async Task<OrderOperationResult> UpdateAsync(
        Guid id,
        CreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        var order = await dbContext.Orders.FindAsync([id], cancellationToken);
        if (order is null)
            return OrderOperationResult.NotFound();

        order.CustomerName = request.CustomerName;
        order.CustomerEmail = request.CustomerEmail;
        order.Total = request.Total;

        await dbContext.SaveChangesAsync(cancellationToken);
        await cache.RemoveAllAsync(cancellationToken);
        return OrderOperationResult.Success();
    }

    public async Task<OrderOperationResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var order = await dbContext.Orders.FindAsync([id], cancellationToken);
        if (order is null)
            return OrderOperationResult.NotFound();

        dbContext.Orders.Remove(order);
        await dbContext.SaveChangesAsync(cancellationToken);
        await cache.RemoveAllAsync(cancellationToken);
        return OrderOperationResult.Success();
    }

    private static OrderResponse ToResponse(Order order)
        => new(order.Id, order.CustomerName, order.CustomerEmail, order.Total, order.Status, order.CreatedAt);
}

public sealed record OrderOperationResult(bool IsSuccess, bool IsNotFound = false)
{
    public static OrderOperationResult Success() => new(true);
    public static OrderOperationResult NotFound() => new(false, true);
}

public sealed record OrderOperationResult<T>(T? Value, bool IsSuccess, bool IsNotFound = false)
{
    public static OrderOperationResult<T> Success(T value) => new(value, true);
    public static OrderOperationResult<T> NotFound() => new(default, false, true);
}
