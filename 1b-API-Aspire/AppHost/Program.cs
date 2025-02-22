var builder = DistributedApplication.CreateBuilder(args);

// Add services to the container.
var cache = builder.AddRedis("cache")
    .WithLifetime(ContainerLifetime.Persistent);

var api = builder.AddProject<Projects.WeatherApi>("api");

var app = builder.AddProject<Projects.AspireApp>("app")
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(api)
    .WaitFor(api);

builder.Build().Run();
