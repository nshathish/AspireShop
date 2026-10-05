namespace AspireShop.Api.Orders.Api.Contracts.Orders;

public sealed record CreateOrderRequest(
    string CustomerName,
    string CustomerEmail,
    decimal Total);
