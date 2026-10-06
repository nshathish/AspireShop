namespace AspireShop.Api.Catalog.Domain.Entities;

public sealed class Product
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? ImageUrl { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }

    public Guid? CategoryId { get; set; }
    
    public Category? Category { get; set; }
    public ICollection<ProductPrice> Prices { get; set; } = [];
}
