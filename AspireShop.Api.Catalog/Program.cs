using AspireShop.Api.Catalog;
using AspireShop.Api.Catalog.Api.Endpoints;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddProblemDetails();

builder.Services.AddCatalogInfrastructure(builder.Configuration);

builder.Services.AddOpenApi();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.DarkMode = false;
    });
}

app.MapGet("/health", () => Results.Ok("Healthy"));

app.MapProductEndpoints();

app.Run();

