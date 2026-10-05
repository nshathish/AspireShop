var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var postgres = builder
    .AddPostgres("postgres")
    .WithDataVolume();

var catalogDb = postgres
    .AddDatabase("catalogdb");

var catalogApi = builder
    .AddProject<Projects.AspireShop_Api_Catalog>("catalog-api")
    .WithReference(catalogDb)
    .WaitFor(catalogDb)
    .WithHttpHealthCheck("/health");

var catalogMigrations = catalogApi
    .AddEFMigrations("catalog-migrations")
    .WithReference(catalogDb)
    .RunDatabaseUpdateOnStart();

catalogApi.WaitForCompletion(catalogMigrations);

builder
    .AddProject<Projects.AspireShop_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(catalogApi)
    .WaitFor(catalogApi);



builder.Build().Run();
