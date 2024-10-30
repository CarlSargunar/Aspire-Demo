# External Services


## Run the AppHost

    dotnet run --project ./AspireApp.AppHost/AspireApp.AppHost.csproj --launch-profile "http"

# Only old things below here, to be updated. Please ignore


Run Containers

 **NOTE : On windows, all the line endings need to be set to LF for the Dockerfile, setup.sql and startup.sh**

    docker run -d --name sql_server -m 2g -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=SQL_password123' -p 1433:1433 mcr.microsoft.com/mssql/server:2022-latest

## SLN

    dotnet new sln --name "Aspire Demo"
    dotnet new aspire-apphost -o AppHost
    dotnet sln add AppHost
    dotnet new aspire-servicedefaults -o DemoServiceDefaults
    dotnet sln add DemoServiceDefaults

### Umbraco

    dotnet new install Umbraco.Templates::13.5.2 --force
    dotnet new umbraco --force -n "UmbWebsite" --friendly-name "Administrator" --email "admin@example.com" --password "1234567890" --development-database-type SQLite
    dotnet add "UmbWebsite" package Umbraco.TheStarterKit --version 13.0
    dotnet sln add UmbWebsite

### References

    dotnet add UmbWebsite reference DemoServiceDefaults
    dotnet add AppHost reference UmbWebsite


## Program.cs

Add nuget Aspire.RammitMQ.Client

Update Program.cs (See 3b Aspire Demo Files)
    
Add Services
Add middleware
Add Configuration

## Projects

Copy over 
- DemoLib
- DemoApi
- AnalyticsApp

## MessageProcessor

New Console App - Processor
- Program.cs
- Worker

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

