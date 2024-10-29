var builder = DistributedApplication.CreateBuilder(args);

var sqlPassword = builder.AddParameter("sql-password");
var sqlContainer = builder.AddSqlServer("sql", sqlPassword, 1433)
    .AddDatabase("reportingdb");

var rmqpassword = builder.AddParameter("messaging-password", secret: true);
var rabbitMq = builder.AddRabbitMQ("messaging", password: rmqpassword)
    .WithManagementPlugin();

var umbraco = builder.AddProject<Projects.UmbWebsite>("umbwebsite")
    .WithReference(rabbitMq);

//var processor = builder.AddProject<Projects.MessageProcessor>("processor")
//    .WithReference(sqlContainer)
//    .WithReference(rabbitMq);

var analyticsapi = builder.AddProject<Projects.DemoApi>("analyticsapi")
    .WithReference(sqlContainer);

var analytics = builder.AddProject<Projects.AnalyticsApp>("analyticsapp")
    .WithReference(analyticsapi);

builder.Build().Run();
