var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var messaging = builder.AddRabbitMQ("messaging");

var postgres = builder
    .AddPostgres("postgres")
    .WithDataVolume();

var catalogDb = postgres.AddDatabase("catalogdb");

var ordersDb = postgres.AddDatabase("ordersdb");

var catalogApi = builder
    .AddProject<Projects.AspireShop_Api_Catalog>("catalog-api")
    .WithReference(catalogDb)
    .WaitFor(catalogDb)
    .WithReference(cache)
    .WaitFor(cache)
    // .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health");

var catalogMigrations = catalogApi
    .AddEFMigrations("catalog-migrations")
    .WithReference(catalogDb)
    .RunDatabaseUpdateOnStart();

catalogApi.WaitForCompletion(catalogMigrations);

var ordersApi = builder
    .AddProject<Projects.AspireShop_Api_Orders>("orders-api")
    .WithReference(ordersDb)
    .WaitFor(ordersDb)
    .WithReference(messaging)
    .WaitFor(messaging)
    .WithReference(cache)
    .WaitFor(cache)
    // .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health");

var ordersMigrations = ordersApi
    .AddEFMigrations("orders-migrations")
    .WithReference(ordersDb)
    .RunDatabaseUpdateOnStart();

ordersApi.WaitForCompletion(ordersMigrations);

builder
    .AddProject<Projects.AspireShop_Web_Store>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(catalogApi)
    .WaitFor(catalogApi);

builder.Build().Run();
