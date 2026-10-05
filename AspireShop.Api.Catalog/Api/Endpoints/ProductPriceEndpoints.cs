using AspireShop.Api.Catalog.Api.Contracts.ProductPrices;
using AspireShop.Api.Catalog.Application.ProductPrices.Services;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AspireShop.Api.Catalog.Api.Endpoints;

public static class ProductPriceEndpoints
{
    public static IEndpointRouteBuilder MapProductPriceEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/product-prices").WithTags("Product prices");

        group.MapGet("/", GetAll)
            .Produces<List<ProductPriceResponse>>()
            .WithName("GetAllProductPrices")
            .WithSummary("Get All Product Prices");

        group.MapGet("/product/{productId:guid}", GetByProductId)
            .Produces<List<ProductPriceResponse>>()
            .WithName("GetProductPrices")
            .WithSummary("Get Product Prices By Product Id");

        group.MapGet("/{id:guid}", GetById).Produces<ProductPriceResponse>()
            .Produces<ProblemDetails>(404)
            .WithName("GetProductPriceById")
            .WithSummary("Get Product Price By Id");

        group.MapPost("/", Create)
            .Accepts<CreateProductPriceRequest>("application/json")
            .Produces<ProductPriceResponse>(201).WithName("CreateProductPrice")
            .WithSummary("Create a new Product Price");

        group.MapPut("/{id:guid}", Update)
            .Accepts<CreateProductPriceRequest>("application/json")
            .Produces(204)
            .WithName("UpdateProductPrice")
            .WithSummary("Update an existing Product Price");
        
        group.MapDelete("/{id:guid}", Delete)
            .Produces(204)
            .Produces<ProblemDetails>(404)
            .WithName("DeleteProductPrice")
            .WithSummary("Delete an existing Product Price");

        return endpoints;
    }

    private static async Task<Ok<List<ProductPriceResponse>>> GetAll(ProductPriceService service,
        CancellationToken cancellationToken = default)
        => TypedResults.Ok(await service.GetAllAsync(cancellationToken));

    private static async Task<Ok<List<ProductPriceResponse>>> GetByProductId(Guid productId,
        ProductPriceService service, CancellationToken cancellationToken = default)
        => TypedResults.Ok(await service.GetByProductIdAsync(productId, cancellationToken));

    private static async Task<Results<Ok<ProductPriceResponse>, NotFound<ProblemDetails>>> GetById(Guid id,
        ProductPriceService service, CancellationToken cancellationToken = default)
    {
        var result = await service.GetByIdAsync(id, cancellationToken);
        return result is null
            ? TypedResults.NotFound(new ProblemDetails { Title = "Product price not found", Status = 404 })
            : TypedResults.Ok(result);
    }

    private static async Task<Results<Created<ProductPriceResponse>, ValidationProblem, NotFound<ProblemDetails>>>
        Create(
            CreateProductPriceRequest request,
            IValidator<CreateProductPriceRequest> validator,
            ProductPriceService service,
            CancellationToken cancellationToken = default)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return TypedResults.ValidationProblem(validation.ToDictionary());

        var result = await service.CreateAsync(request, cancellationToken);
        return result.IsNotFound
            ? TypedResults.NotFound(new ProblemDetails { Title = "Product not found", Status = 404 })
            : TypedResults.Created($"/api/product-prices/{result.Value!.Id}", result.Value);
    }

    private static async Task<Results<NoContent, ValidationProblem, NotFound<ProblemDetails>>> Update(
        Guid id,
        CreateProductPriceRequest request,
        IValidator<CreateProductPriceRequest> validator,
        ProductPriceService service,
        CancellationToken cancellationToken = default)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return TypedResults.ValidationProblem(validation.ToDictionary());

        var result = await service.UpdateAsync(id, request, cancellationToken);
        return result.IsNotFound
            ? TypedResults.NotFound(new ProblemDetails { Title = "Product price or product not found", Status = 404 })
            : TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound<ProblemDetails>>> Delete(Guid id, ProductPriceService service,
        CancellationToken cancellationToken = default)
    {
        var result = await service.DeleteAsync(id, cancellationToken);
        return result.IsNotFound
            ? TypedResults.NotFound(new ProblemDetails { Title = "Product price not found", Status = 404 })
            : TypedResults.NoContent();
    }
}