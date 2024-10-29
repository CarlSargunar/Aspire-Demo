var builder = DistributedApplication.CreateBuilder(args);

// Add services to the container.

var api = builder.AddProject<Projects.WeatherApi>("api");

var app = builder.AddProject<Projects.AspireApp>("app");

builder.Build().Run();
