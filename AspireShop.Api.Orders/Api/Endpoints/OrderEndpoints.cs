using AspireShop.Api.Orders.Api.Contracts.Orders;
using AspireShop.Api.Orders.Application.Orders.Services;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AspireShop.Api.Orders.Api.Endpoints;

public static class OrderEndpoints
{
    public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("/api/orders")
            .WithTags("Orders");

        group.MapGet("/", GetAllOrders)
            .Produces<List<OrderResponse>>()
            .WithName("GetAllOrders")
            .WithSummary("Gets all orders");

        group.MapGet("/{id:guid}", GetOrderById)
            .Produces<OrderResponse>()
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .WithName("GetOrderById")
            .WithSummary("Gets an order by its ID");

        group.MapPost("/", CreateOrder)
            .Accepts<CreateOrderRequest>("application/json")
            .Produces<OrderResponse>(StatusCodes.Status201Created)
            .Produces<ValidationProblem>(StatusCodes.Status400BadRequest)
            .WithName("CreateOrder")
            .WithSummary("Creates a new order");

        group.MapPut("/{id:guid}", UpdateOrder)
            .Accepts<CreateOrderRequest>("application/json")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .WithName("UpdateOrder")
            .WithSummary("Updates an existing order");

        group.MapDelete("/{id:guid}", DeleteOrder)
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .WithName("DeleteOrder")
            .WithSummary("Deletes an order");

        return endpoints;
    }

    private static async Task<Ok<List<OrderResponse>>> GetAllOrders(
        OrderService orderService,
        CancellationToken cancellationToken = default)
        => TypedResults.Ok(await orderService.GetAllAsync(cancellationToken));

    private static async Task<Results<Ok<OrderResponse>, NotFound<ProblemDetails>>> GetOrderById(
        Guid id,
        OrderService orderService,
        CancellationToken cancellationToken = default)
    {
        var order = await orderService.GetByIdAsync(id, cancellationToken);
        return order is null
            ? TypedResults.NotFound(new ProblemDetails { Title = "Order not found", Status = StatusCodes.Status404NotFound })
            : TypedResults.Ok(order);
    }

    private static async Task<Results<Created<OrderResponse>, ValidationProblem>> CreateOrder(
        CreateOrderRequest request,
        IValidator<CreateOrderRequest> validator,
        OrderService orderService,
        CancellationToken cancellationToken = default)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return TypedResults.ValidationProblem(validation.ToDictionary());

        var result = await orderService.CreateAsync(request, cancellationToken);
        var response = result.Value!;
        return TypedResults.Created($"/api/orders/{response.Id}", response);
    }

    private static async Task<Results<NoContent, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>> UpdateOrder(
        Guid id,
        CreateOrderRequest request,
        IValidator<CreateOrderRequest> validator,
        OrderService orderService,
        CancellationToken cancellationToken = default)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return TypedResults.BadRequest(new ProblemDetails { Title = "Invalid order request", Status = StatusCodes.Status400BadRequest });

        var result = await orderService.UpdateAsync(id, request, cancellationToken);
        return result.IsNotFound
            ? TypedResults.NotFound(new ProblemDetails { Title = "Order not found", Status = StatusCodes.Status404NotFound })
            : TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound<ProblemDetails>>> DeleteOrder(
        Guid id,
        OrderService orderService,
        CancellationToken cancellationToken = default)
    {
        var result = await orderService.DeleteAsync(id, cancellationToken);
        return result.IsNotFound
            ? TypedResults.NotFound(new ProblemDetails { Title = "Order not found", Status = StatusCodes.Status404NotFound })
            : TypedResults.NoContent();
    }
}
