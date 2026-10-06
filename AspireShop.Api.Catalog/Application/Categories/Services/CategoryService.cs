using AspireShop.Api.Catalog.Api.Contracts.Categories;
using AspireShop.Api.Catalog.Domain.Entities;
using AspireShop.Api.Catalog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AspireShop.Api.Catalog.Application.Categories.Services;

public sealed class CategoryService(CatalogDbContext dbContext)
{
    public async Task<List<CategoryResponse>> GetAllAsync(CancellationToken cancellationToken)
        => await dbContext.Categories
            .AsNoTracking()
            .OrderBy(category => category.Name)
            .Select(category => new CategoryResponse(
                category.Id,
                category.Name,
                category.Slug,
                category.Products.Count))
            .ToListAsync(cancellationToken);

    public async Task<CategoryResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        await dbContext.Categories
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new CategoryResponse(c.Id, c.Name, c.Slug, c.Products.Count))
            .SingleOrDefaultAsync(cancellationToken);

    public async Task<CategoryOperationResult<CategoryResponse>> CreateAsync(
        CreateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        if (await dbContext.Categories.AnyAsync(category => category.Slug == request.Slug, cancellationToken))
            return CategoryOperationResult<CategoryResponse>.Conflict("A category with this slug already exists.");

        var category = new Category { Id = Guid.NewGuid(), Name = request.Name, Slug = request.Slug };
        await dbContext.Categories.AddAsync(category, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return CategoryOperationResult<CategoryResponse>.Success(new CategoryResponse(category.Id, category.Name,
            category.Slug, 0));
    }

    public async Task<CategoryOperationResult> UpdateAsync(Guid id, CreateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var category = await dbContext.Categories.FindAsync([id], cancellationToken);
        if (category is null)
            return CategoryOperationResult.NotFound();

        var duplicate =
            await dbContext.Categories.AnyAsync(item => item.Id != id && item.Slug == request.Slug, cancellationToken);
        if (duplicate)
            return CategoryOperationResult.Conflict("A category with this slug already exists.");

        category.Name = request.Name;
        category.Slug = request.Slug;
        await dbContext.SaveChangesAsync(cancellationToken);
        return CategoryOperationResult.Success();
    }

    public async Task<CategoryOperationResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var category = await dbContext.Categories.FindAsync([id], cancellationToken);
        if (category is null)
            return CategoryOperationResult.NotFound();

        dbContext.Categories.Remove(category);
        await dbContext.SaveChangesAsync(cancellationToken);
        return CategoryOperationResult.Success();
    }
}

public sealed record CategoryOperationResult(bool IsSuccess, bool IsNotFound = false, string? Error = null)
{
    public static CategoryOperationResult Success() => new(true);
    public static CategoryOperationResult NotFound() => new(false, true);
    public static CategoryOperationResult Conflict(string error) => new(false, false, error);
}

public sealed record CategoryOperationResult<T>(T? Value, bool IsSuccess, bool IsNotFound = false, string? Error = null)
{
    public static CategoryOperationResult<T> Success(T value) => new(value, true);
    public static CategoryOperationResult<T> Conflict(string error) => new(default, false, false, error);
}