var builder = DistributedApplication.CreateBuilder(args);
 

var sqlContainer = builder.AddSqlServer("sql")
    .AddDatabase("reportingdb");

var rabbitMq = builder.AddRabbitMQ("messaging")
    .WithManagementPlugin();

var umbraco = builder.AddProject<Projects.UmbWebsite>("umbwebsite")
    .WithReference(rabbitMq);

var processor = builder.AddProject<Projects.MessageProcessor>("processor")
    .WithReference(sqlContainer)
    .WithReference(rabbitMq);

var demoapi = builder.AddProject<Projects.DemoApi>("demoapi")
    .WithReference(sqlContainer);

var analytics = builder.AddProject<Projects.AnalyticsApp>("analytics")
    .WithReference(demoapi);

builder.Build().Run();
