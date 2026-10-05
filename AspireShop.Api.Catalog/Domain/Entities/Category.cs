namespace AspireShop.Api.Catalog.Domain.Entities;

public sealed class Category
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Slug { get; set; }
    public ICollection<Product> Products { get; set; } = [];
}
