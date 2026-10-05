using AspireShop.Api.Catalog.Api.Contracts.Products;
using AspireShop.Api.Catalog.Domain.Entities;
using AspireShop.Api.Catalog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AspireShop.Api.Catalog.Application.Products.Services;

public sealed class ProductService(CatalogDbContext dbContext, ProductCacheService cache)
{
    public async Task<List<ProductResponse>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        var cached = await cache.GetAllAsync(cancellationToken);

        if (cached is not null)
            return cached.ToList();

        var products = await dbContext.Products
            .AsNoTracking()
            .Select(p => new ProductResponse(
                p.Id,
                p.Name,
                p.Price,
                p.Stock))
            .ToListAsync(cancellationToken);

        await cache.SetAllAsync(products, cancellationToken);

        return products;
    }

    public async Task<ProductResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var cached = await cache.GetByIdAsync(id, cancellationToken);

        if (cached is not null)
            return cached;

        var product = await dbContext.Products.FindAsync([id], cancellationToken);

        if (product is null)
            return null;

        var result = new ProductResponse(product.Id, product.Name, product.Price, product.Stock);
        await cache.SetByIdAsync(result, cancellationToken);

        return result;
    }

    public async Task<ProductOperationResult<ProductResponse>> CreateAsync(
        CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return ProductOperationResult<ProductResponse>.ValidationFailed("Name is required");
        }

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Price = request.Price,
            Stock = request.Stock
        };

        await dbContext.Products.AddAsync(product, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        await cache.RemoveAllAsync(cancellationToken);

        var response = new ProductResponse(product.Id, product.Name, product.Price, product.Stock);
        return ProductOperationResult<ProductResponse>.Success(response);
    }

    public async Task<ProductOperationResult> UpdateAsync(
        Guid id,
        CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return ProductOperationResult.ValidationFailed("Name is required");
        }

        var product = await dbContext.Products.FindAsync([id], cancellationToken);

        if (product is null)
            return ProductOperationResult.NotFound();

        product.Name = request.Name;
        product.Price = request.Price;
        product.Stock = request.Stock;

        await dbContext.SaveChangesAsync(cancellationToken);
        await cache.RemoveByIdAsync(id, cancellationToken);
        await cache.RemoveAllAsync(cancellationToken);

        return ProductOperationResult.Success();
    }

    public async Task<ProductOperationResult> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var product = await dbContext.Products.FindAsync([id], cancellationToken);

        if (product is null)
            return ProductOperationResult.NotFound();

        dbContext.Products.Remove(product);
        await dbContext.SaveChangesAsync(cancellationToken);
        await cache.RemoveByIdAsync(id, cancellationToken);
        await cache.RemoveAllAsync(cancellationToken);

        return ProductOperationResult.Success();
    }
}

public sealed record ProductOperationResult(bool IsSuccess, bool IsNotFound = false, string? ValidationError = null)
{
    public static ProductOperationResult Success() => new(true);

    public static ProductOperationResult NotFound() => new(false, true);

    public static ProductOperationResult ValidationFailed(string error) => new(false, false, error);
}

public sealed record ProductOperationResult<T>(T? Value, bool IsSuccess, bool IsNotFound = false, string? ValidationError = null)
{
    public static ProductOperationResult<T> Success(T value) => new(value, true);

    public static ProductOperationResult<T> NotFound() => new(default, false, true);

    public static ProductOperationResult<T> ValidationFailed(string error) => new(default, false, false, error);
}
