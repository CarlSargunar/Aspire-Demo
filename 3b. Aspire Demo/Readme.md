# External Services

Run Containers

 **NOTE : On windows, all the line endings need to be set to LF for the Dockerfile, setup.sql and startup.sh**

    docker run -d --name sql_server -m 2g -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=SQL_password123' -p 1433:1433 mcr.microsoft.com/mssql/server:2022-latest

## Aspire App Host

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