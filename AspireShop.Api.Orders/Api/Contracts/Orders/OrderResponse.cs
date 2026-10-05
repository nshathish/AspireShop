namespace AspireShop.Api.Orders.Api.Contracts.Orders;

public sealed record OrderResponse(
    Guid Id,
    string CustomerName,
    string CustomerEmail,
    decimal Total,
    string Status,
    DateTimeOffset CreatedAt);
