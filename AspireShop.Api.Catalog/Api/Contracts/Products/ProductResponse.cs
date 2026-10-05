namespace AspireShop.Api.Catalog.Api.Contracts.Products;

public record ProductResponse(
    Guid Id,
    string Name,
    string? ImageUrl,
    decimal Price,
    int Stock,
    Guid? CategoryId);
