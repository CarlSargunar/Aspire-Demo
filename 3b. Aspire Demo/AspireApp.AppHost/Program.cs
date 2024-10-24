var builder = DistributedApplication.CreateBuilder(args);

var rabbitMq = builder.AddRabbitMQ("messaging")
    .WithManagementPlugin();

var umbraco = builder.AddProject<Projects.UmbWebsite>("umbwebsite").WithReference(rabbitMq);

var processor = builder.AddProject<Projects.Processor>("processor").WithReference(rabbitMq);

var demoapi = builder.AddProject<Projects.DemoApi>("demoapi");

var analytics = builder.AddProject<Projects.AnalyticsApp>("analytics");

builder.Build().Run();
