using AspireShop.Api.Catalog.Api.Contracts.Products;
using AspireShop.Api.Catalog.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace AspireShop.Api.Catalog.Api.Endpoints;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("/api/products")
            .WithTags("Products");

        group.MapGet("/", GetAllProducts)
            .Produces<List<ProductResponse>>()
            .WithName("GetAllProducts")
            .WithSummary("Gets all products");

        group.MapGet("/{id}", GetProductById)
            .Produces<ProductResponse>()
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .WithName("GetProductById")
            .WithSummary("Gets a product by its ID");

        group.MapPost("/", CreateProduct)
            .Accepts<CreateProductRequest>("application/json")
            .Produces<ProductResponse>(StatusCodes.Status201Created)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .WithName("CreateProduct")
            .WithSummary("Creates a new product");

        return endpoints;
    }

    private static async Task<Ok<IEnumerable<ProductResponse>>> GetAllProducts(
        CatalogDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        var products = await dbContext.Products
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var response = products.Select(p => new ProductResponse(
            p.Id,
            p.Name,
            p.Price,
            p.Stock));

        return TypedResults.Ok(response);
    }

    private static async Task<Results<Ok<ProductResponse>, NotFound<ProblemDetails>>> GetProductById(
        Guid id,
        CatalogDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        var product = await dbContext.Products.FindAsync([id], cancellationToken);

        if (product is null)
            return TypedResults.NotFound(new ProblemDetails
            {
                Title = "Product not found",
                Status = StatusCodes.Status404NotFound
            });

        var response = new ProductResponse(product.Id, product.Name, product.Price, product.Stock);

        return TypedResults.Ok(response);
    }

    private static async Task<Results<Created<ProductResponse>, BadRequest<ProblemDetails>>> CreateProduct(
        CreateProductRequest request,
        CatalogDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Name is required",
                Status = StatusCodes.Status400BadRequest
            });

        var product = new Domain.Entities.Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Price = request.Price,
            Stock = request.Stock
        };

        await dbContext.Products.AddAsync(product, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var response = new ProductResponse(product.Id, product.Name, product.Price, product.Stock);

        var uri = $"/api/products/{product.Id}";
        return TypedResults.Created(uri, response);
    }
}