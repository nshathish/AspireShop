namespace AspireShop.Api.Catalog.Api.Contracts.Categories;

public sealed record CategoryResponse(Guid Id, string Name, string Slug, int ProductCount);
