namespace AspireShop.Api.Catalog.Api.Contracts.ProductPrices;

public sealed record CreateProductPriceRequest(
    Guid ProductId,
    decimal Amount,
    string Currency,
    DateTimeOffset ValidFrom,
    DateTimeOffset? ValidTo);
