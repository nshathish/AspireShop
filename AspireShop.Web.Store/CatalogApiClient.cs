using System.Net.Http.Json;

namespace AspireShop.Web;

public sealed class CatalogApiClient(HttpClient httpClient)
{
    public async Task<IReadOnlyList<CatalogProduct>> GetProductsAsync(
        CancellationToken cancellationToken = default)
    {
        return await httpClient.GetFromJsonAsync<List<CatalogProduct>>(
                   "api/products/",
                   cancellationToken)
               ?? [];
    }

    public async Task<CatalogProduct> CreateProductAsync(
        CreateCatalogProductRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync(
            "api/products/",
            request,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<CatalogProduct>(cancellationToken)
               ?? throw new InvalidOperationException("The catalog API returned an empty product response.");
    }
}

public sealed record CreateCatalogProductRequest(string Name, decimal Price, int Stock, string? ImageUrl = null);

public sealed record CatalogProduct(Guid Id, string Name, string? ImageUrl, decimal Price, int Stock);
