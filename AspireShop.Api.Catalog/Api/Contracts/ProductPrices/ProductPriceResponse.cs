namespace AspireShop.Api.Catalog.Api.Contracts.ProductPrices;

public sealed record ProductPriceResponse(
    Guid Id,
    Guid ProductId,
    decimal Amount,
    string Currency,
    DateTimeOffset ValidFrom,
    DateTimeOffset? ValidTo);
