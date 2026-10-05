using AspireShop.Api.Catalog.Api.Contracts.ProductPrices;
using AspireShop.Api.Catalog.Domain.Entities;
using AspireShop.Api.Catalog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AspireShop.Api.Catalog.Application.ProductPrices.Services;

public sealed class ProductPriceService(CatalogDbContext dbContext)
{
    public Task<List<ProductPriceResponse>> GetAllAsync(CancellationToken cancellationToken)
        => dbContext.ProductPrices
            .AsNoTracking()
            .OrderByDescending(price => price.ValidFrom)
            .Select(price => new ProductPriceResponse(
                price.Id,
                price.ProductId,
                price.Amount,
                price.Currency,
                price.ValidFrom,
                price.ValidTo))
            .ToListAsync(cancellationToken);

    public Task<List<ProductPriceResponse>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken)
        => dbContext.ProductPrices
            .AsNoTracking()
            .Where(price => price.ProductId == productId)
            .OrderByDescending(price => price.ValidFrom)
            .Select(price => new ProductPriceResponse(
                price.Id,
                price.ProductId,
                price.Amount,
                price.Currency,
                price.ValidFrom,
                price.ValidTo))
            .ToListAsync(cancellationToken);

    public async Task<ProductPriceResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var price = await dbContext.ProductPrices.FindAsync([id], cancellationToken);
        return price is null ? null : ToResponse(price);
    }

    public async Task<ProductPriceOperationResult<ProductPriceResponse>> CreateAsync(CreateProductPriceRequest request, CancellationToken cancellationToken)
    {
        if (!await dbContext.Products.AnyAsync(product => product.Id == request.ProductId, cancellationToken))
            return ProductPriceOperationResult<ProductPriceResponse>.NotFound();

        var price = new ProductPrice
        {
            Id = Guid.NewGuid(),
            ProductId = request.ProductId,
            Amount = request.Amount,
            Currency = request.Currency.ToUpperInvariant(),
            ValidFrom = request.ValidFrom,
            ValidTo = request.ValidTo
        };

        await dbContext.ProductPrices.AddAsync(price, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ProductPriceOperationResult<ProductPriceResponse>.Success(ToResponse(price));
    }

    public async Task<ProductPriceOperationResult> UpdateAsync(Guid id, CreateProductPriceRequest request, CancellationToken cancellationToken)
    {
        var price = await dbContext.ProductPrices.FindAsync([id], cancellationToken);
        if (price is null)
            return ProductPriceOperationResult.NotFound();

        if (!await dbContext.Products.AnyAsync(product => product.Id == request.ProductId, cancellationToken))
            return ProductPriceOperationResult.NotFound();

        price.ProductId = request.ProductId;
        price.Amount = request.Amount;
        price.Currency = request.Currency.ToUpperInvariant();
        price.ValidFrom = request.ValidFrom;
        price.ValidTo = request.ValidTo;
        await dbContext.SaveChangesAsync(cancellationToken);
        return ProductPriceOperationResult.Success();
    }

    public async Task<ProductPriceOperationResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var price = await dbContext.ProductPrices.FindAsync([id], cancellationToken);
        if (price is null)
            return ProductPriceOperationResult.NotFound();

        dbContext.ProductPrices.Remove(price);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ProductPriceOperationResult.Success();
    }

    private static ProductPriceResponse ToResponse(ProductPrice price)
        => new(price.Id, price.ProductId, price.Amount, price.Currency, price.ValidFrom, price.ValidTo);
}

public sealed record ProductPriceOperationResult(bool IsSuccess, bool IsNotFound = false)
{
    public static ProductPriceOperationResult Success() => new(true);
    public static ProductPriceOperationResult NotFound() => new(false, true);
}

public sealed record ProductPriceOperationResult<T>(T? Value, bool IsSuccess, bool IsNotFound = false)
{
    public static ProductPriceOperationResult<T> Success(T value) => new(value, true);
    public static ProductPriceOperationResult<T> NotFound() => new(default, false, true);
}
