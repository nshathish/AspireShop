using AspireShop.Api.Catalog.Api.Contracts.Categories;
using AspireShop.Api.Catalog.Application.Categories.Services;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AspireShop.Api.Catalog.Api.Endpoints;

public static class CategoryEndpoints
{
    public static IEndpointRouteBuilder MapCategoryEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/categories").WithTags("Categories");

        group.MapGet("/", GetAll)
            .Produces<List<CategoryResponse>>()
            .WithName("GetAllCategories")
            .WithSummary("Get All Categories");

        group.MapGet("/{id:guid}", GetById)
            .Produces<CategoryResponse>()
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .WithName("GetCategoryById")
            .WithSummary("Get Category By Id");

        group.MapPost("/", Create)
            .Accepts<CreateCategoryRequest>("application/json")
            .Produces<CategoryResponse>(StatusCodes.Status201Created)
            .Produces<ValidationProblem>(StatusCodes.Status400BadRequest)
            .WithName("CreateCategory")
            .WithSummary("Create a new Category");

        group.MapPut("/{id:guid}", Update)
            .Accepts<CreateCategoryRequest>("application/json")
            .Produces(StatusCodes.Status204NoContent)
            .WithName("UpdateCategory")
            .WithSummary("Update an existing Category");

        group.MapDelete("/{id:guid}", Delete)
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .WithName("DeleteCategory")
            .WithSummary("Delete an existing Category");

        return endpoints;
    }

    private static async Task<Ok<List<CategoryResponse>>> GetAll(CategoryService service,
        CancellationToken cancellationToken)
        => TypedResults.Ok(await service.GetAllAsync(cancellationToken));

    private static async Task<Results<Ok<CategoryResponse>, NotFound<ProblemDetails>>> GetById(Guid id,
        CategoryService service, CancellationToken cancellationToken)
    {
        var result = await service.GetByIdAsync(id, cancellationToken);
        return result is null
            ? TypedResults.NotFound(new ProblemDetails
                { Title = "Category not found", Status = StatusCodes.Status404NotFound })
            : TypedResults.Ok(result);
    }

    private static async Task<Results<Created<CategoryResponse>, ValidationProblem, BadRequest<ProblemDetails>>> Create(
        CreateCategoryRequest request,
        IValidator<CreateCategoryRequest> validator,
        CategoryService service,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return TypedResults.ValidationProblem(validation.ToDictionary());

        var result = await service.CreateAsync(request, cancellationToken);
        if (!result.IsSuccess)
            return TypedResults.BadRequest(new ProblemDetails
                { Title = result.Error, Status = StatusCodes.Status400BadRequest });

        return TypedResults.Created($"/api/categories/{result.Value!.Id}", result.Value);
    }

    private static async
        Task<Results<NoContent, ValidationProblem, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>> Update(
            Guid id,
            CreateCategoryRequest request,
            IValidator<CreateCategoryRequest> validator,
            CategoryService service,
            CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return TypedResults.ValidationProblem(validation.ToDictionary());

        var result = await service.UpdateAsync(id, request, cancellationToken);
        if (result.IsNotFound)
            return TypedResults.NotFound(new ProblemDetails
                { Title = "Category not found", Status = StatusCodes.Status404NotFound });
        if (!result.IsSuccess)
            return TypedResults.BadRequest(new ProblemDetails
                { Title = result.Error, Status = StatusCodes.Status400BadRequest });

        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound<ProblemDetails>>> Delete(Guid id, CategoryService service,
        CancellationToken cancellationToken)
    {
        var result = await service.DeleteAsync(id, cancellationToken);
        return result.IsNotFound
            ? TypedResults.NotFound(new ProblemDetails
                { Title = "Category not found", Status = StatusCodes.Status404NotFound })
            : TypedResults.NoContent();
    }
}