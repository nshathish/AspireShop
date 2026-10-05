using AspireShop.Api.Orders;
using AspireShop.Api.Orders.Api.Endpoints;
using AspireShop.Api.Orders.Application;
using AspireShop.Api.Orders.Application.Orders.Services;
using FluentValidation;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddProblemDetails();

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddValidatorsFromAssemblyContaining<ApplicationAssemblyMarker>();

builder.Services.AddOpenApi();

builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<OrderCacheService>();

var app = builder.Build();

app.UseExceptionHandler();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options => { options.DarkMode = false; });
}

app.UseHttpsRedirection();

app.MapGet("/health", () => Results.Ok("Healthy"));
app.MapOrderEndpoints();

app.Run();

