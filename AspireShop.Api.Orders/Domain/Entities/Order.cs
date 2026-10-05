namespace AspireShop.Api.Orders.Domain.Entities;

public sealed class Order
{
    public Guid Id { get; set; }
    public required string CustomerName { get; set; }
    public required string CustomerEmail { get; set; }
    public decimal Total { get; set; }
    public required string Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
