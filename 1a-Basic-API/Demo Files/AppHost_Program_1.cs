var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.WeatherApi>("api");

var app = builder.AddProject<Projects.AspireApp>("app")
    .WithExternalHttpEndpoints()
    .WithReference(api)
    .WaitFor(api);

builder.Build().Run();
