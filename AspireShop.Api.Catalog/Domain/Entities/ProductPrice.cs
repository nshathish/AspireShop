namespace AspireShop.Api.Catalog.Domain.Entities;

public sealed class ProductPrice
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public required Product Product { get; set; }
    public decimal Amount { get; set; }
    public required string Currency { get; set; }
    public DateTimeOffset ValidFrom { get; set; }
    public DateTimeOffset? ValidTo { get; set; }
}
