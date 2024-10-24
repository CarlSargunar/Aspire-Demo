var builder = DistributedApplication.CreateBuilder(args);

var rabbitMq = builder.AddRabbitMQ("messaging")
        .WithManagementPlugin();

builder.Build().Run();
