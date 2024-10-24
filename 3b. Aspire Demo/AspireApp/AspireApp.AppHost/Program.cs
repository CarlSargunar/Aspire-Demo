var builder = DistributedApplication.CreateBuilder(args);

// Add RabbitMQ to the application
var rabbitMq = builder.AddRabbitMQ("messaging")
    .WithManagementPlugin();

// Add Umbraco to the application, and configure references
builder.AddProject<Projects.UmbWebsite>("umbwebsite")
    .WithReference(rabbitMq);

// API Project
builder.AddProject<Projects.DemoApi>("demoapi");

// Add Analytics Project
builder.AddProject<Projects.AnalyticsApp>("analytics");

builder.Build().Run();
