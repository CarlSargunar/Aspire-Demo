# Aspire Demo

Ensure the User Secrets are set for the AppHost

    {
        "Parameters:messaging-password": "RMQ_password123",
        "Parameters:sql-password": "SQL_password123"
    }

You can also do that with the following commands

    dotnet user-secrets set "Parameters:messaging-password" "RMQ_password123" --project Aspire-Demo-App.AppHost
    dotnet user-secrets set "Parameters:sql-password" "SQL_password123" --project Aspire-Demo-App.AppHost   

## Add the Orchestrations

Update the AppHost.Program.cs to include the orchestrations


```csharp
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
```

## Run the AppHost

```bash
    dotnet run --project Aspire-Demo-App.AppHost --launch-profile "http"
```

Or if you have the Aspire CLI installed, you can run it with

```bash
    aspire run --launch-profile "http"
```

# Only old things below here, to be updated. Please ignore, these are my scribbles


Run Containers

 **NOTE : On windows, all the line endings need to be set to LF for the Dockerfile, setup.sql and startup.sh**

```bash
    docker run -d --name sql_server -m 2g -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=SQL_password123' -p 1433:1433 mcr.microsoft.com/mssql/server:2022-latest
```

## SLN


```bash
    dotnet new sln --name "Aspire Demo"
    dotnet new aspire-apphost -o AppHost
    dotnet sln add AppHost
    dotnet new aspire-servicedefaults -o DemoServiceDefaults
    dotnet sln add DemoServiceDefaults
```

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

