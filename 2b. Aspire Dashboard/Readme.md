# Running the Aspire Dashboard Standalonw

Start by copying all files from [1. Aspire App] to the current folder.

## Add Aspire Dashboard to the Solution

    dotnet new aspire-apphost -n AppHost
    dotnet new aspire-servicedefaults -n ServiceDefaults

### Add the new projects to the WeatherApi.sln

    dotnet sln add .\AppHost\AppHost.csproj
    dotnet sln add .\ServiceDefaults\ServiceDefaults.csproj

### Add the references to the WeatherApi project

    dotnet add .\AppHost\AppHost.csproj reference .\WeatherApi\WeatherApi.csproj
    dotnet add .\AppHost\AppHost.csproj reference .\AspireApp\AspireApp.csproj
    dotnet add .\WeatherApi\WeatherApi.csproj reference .\ServiceDefaults\ServiceDefaults.csproj
    dotnet add .\AspireApp\AspireApp.csproj reference .\ServiceDefaults\ServiceDefaults.csproj


### Configure the Aspire App


    // Add services to the container.
    //var cache = builder.AddRedis("cache");

    var api = builder.AddProject<Projects.WeatherApi>("api");

    var app = builder.AddProject<Projects.AspireApp>("app")
        //.WithReference(cache)
        .WithReference(api);




## Add the Aspire Dashboard to the Solution

    

To start the container, run the following command: 

    docker run --rm -it -d -p 18888:18888 -p 4317:18889 --name aspire-dashboard mcr.microsoft.com/dotnet/aspire-dashboard:8.2

The two ports exposed by the conainer are :
- 4317:18889 - the port for Open Telemetry Protocol (OTLP)
- 18888:18888 - the port for the Aspire Dashboard

*Note:* You can optionally allow anonymous access to the dashboard by adding the the following environmental variable

    -e DOTNET_DASHBOARD_UNSECURED_ALLOW_ANONYMOUS=True

So 

    docker run --rm -it -d -p 18888:18888 -p 4317:18889 -e DOTNET_DASHBOARD_UNSECURED_ALLOW_ANONYMOUS=True --name aspire-dashboard mcr.microsoft.com/dotnet/aspire-dashboard:8.2


## Exporting Telemetry to the Aspire Dashboard

To configure applications:

Use the OpenTelemetry SDK APIs within the application, or
Start the app with known environment variables:
 - OTEL_EXPORTER_OTLP_PROTOCOL with a value of grpc.
 - OTEL_EXPORTER_OTLP_ENDPOINT with a value of http://localhost:4317.

## References

 - Aspire Standalong Dashboard
    - https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/dashboard/standalone?tabs=bash
 - Build a Blazor App
    - https://learn.microsoft.com/en-us/aspnet/core/blazor/tutorials/build-a-blazor-app


## Notes : Mostly for me

From the Aspire Dashboard folder, run the weather api and the blazor app in two terminals

    dotnet run --project .\WeatherApi\
    dotnet run --project .\AspireApp\

Swagger endpoint for the API is available from [http://localhost:5074/SWAGGER/index.html](http://localhost:5074/SWAGGER/index.html)
