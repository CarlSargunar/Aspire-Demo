using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);
 
var sqlPassword = builder.AddParameter("sql-password");
var sqlContainer = builder.AddSqlServer("sql", sqlPassword, 1433)
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase("reportingdb");

var rmqpassword = builder.AddParameter("messaging-password", secret: true);
var rabbitMq = builder.AddRabbitMQ("messaging", password: rmqpassword)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithManagementPlugin();

var umbraco = builder.AddProject<Projects.UmbracoSite>("umbwebsite")
    .WithReference(rabbitMq)
    .WaitFor(rabbitMq);

var processor = builder.AddProject<Projects.MessageProcessor>("processor")
    .WithReference(sqlContainer)
    .WaitFor(sqlContainer)
    .WithReference(rabbitMq)
    .WaitFor(rabbitMq);

var analyticsapi = builder.AddProject<Projects.DemoApi>("analyticsapi")
    .WithReference(sqlContainer)
    .WaitFor(sqlContainer);

var analytics = builder.AddProject<Projects.AnalyticsApp>("analyticsapp")
    .WithReference(analyticsapi);

builder.Build().Run();
