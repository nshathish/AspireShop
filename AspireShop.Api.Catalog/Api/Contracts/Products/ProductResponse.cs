namespace AspireShop.Api.Catalog.Api.Contracts.Products;

public record ProductResponse(
    Guid Id,
    string Name,
    decimal Price,
    int Stock);