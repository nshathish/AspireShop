using AspireShop.Api.Catalog.Api.Contracts.Products;
using AspireShop.Api.Catalog.Application.Products.Services;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

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

        group.MapPut("/{id}", UpdateProduct)
            .Accepts<CreateProductRequest>("application/json")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .WithName("UpdateProduct")
            .WithSummary("Updates an existing product");

        group.MapDelete("/{id}", DeleteProduct)
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .WithName("DeleteProduct")
            .WithSummary("Deletes a product by its ID");

        return endpoints;
    }

    private static async Task<Ok<List<ProductResponse>>> GetAllProducts(
        ProductService productService,
        CancellationToken cancellationToken)
    {
        var results = await productService.GetAllAsync(cancellationToken);
        return TypedResults.Ok(results);
    }

    private static async Task<Results<Ok<ProductResponse>, NotFound<ProblemDetails>>> GetProductById(
        Guid id,
        ProductService productService,
        CancellationToken cancellationToken)
    {
        var product = await productService.GetByIdAsync(id, cancellationToken);

        if (product is null)
            return TypedResults.NotFound(new ProblemDetails
            {
                Title = "Product not found",
                Status = StatusCodes.Status404NotFound
            });

        return TypedResults.Ok(product);
    }

    private static async Task<Results<Created<ProductResponse>, ValidationProblem, BadRequest<ProblemDetails>>>
        CreateProduct(
            CreateProductRequest request,
            IValidator<CreateProductRequest> validator,
            ProductService productService,
            CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return TypedResults.ValidationProblem(validationResult.ToDictionary());

        var result = await productService.CreateAsync(request, cancellationToken);

        if (!result.IsSuccess)
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = result.ValidationError,
                Status = StatusCodes.Status400BadRequest
            });

        var response = result.Value!;
        var uri = $"/api/products/{response.Id}";

        return TypedResults.Created(uri, response);
    }

    private static async Task<Results<NoContent, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>> UpdateProduct(
        Guid id,
        CreateProductRequest request,
        ProductService productService,
        CancellationToken cancellationToken)
    {
        var result = await productService.UpdateAsync(id, request, cancellationToken);

        if (result.IsNotFound)
            return TypedResults.NotFound(new ProblemDetails
            {
                Title = "Product not found",
                Status = StatusCodes.Status404NotFound
            });

        if (!result.IsSuccess)
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = result.ValidationError,
                Status = StatusCodes.Status400BadRequest
            });

        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound<ProblemDetails>>> DeleteProduct(
        Guid id,
        ProductService productService,
        CancellationToken cancellationToken)
    {
        var result = await productService.DeleteAsync(id, cancellationToken);

        if (!result.IsSuccess)
            return TypedResults.NotFound(new ProblemDetails
            {
                Title = "Product not found",
                Status = StatusCodes.Status404NotFound
            });

        return TypedResults.NoContent();
    }
}