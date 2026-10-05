using AspireShop.Api.Catalog;
using AspireShop.Api.Catalog.Api.Endpoints;
using AspireShop.Api.Catalog.Application;
using AspireShop.Api.Catalog.Common;
using FluentValidation;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddProblemDetails();
builder.Services.AddValidatorsFromAssemblyContaining<ApplicationAssemblyMarker>();

builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddOpenApi();

var app = builder.Build();

app.UseExceptionHandler();
app.UseCors(CorsPolicies.Admin);

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options => { options.DarkMode = false; });
}

app.MapGet("/health", () => Results.Ok("Healthy"));

app.MapProductEndpoints();
app.MapCategoryEndpoints();
app.MapProductPriceEndpoints();

app.Run();