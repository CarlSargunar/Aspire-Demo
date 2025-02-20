# Running the Aspire Dashboard Standalonw

Check it works before Aspire

    dotnet run --project WeatherApi
    dotnet run --project AspireApp


## Starting the Aspire Dashboard Container

To start the container, run the following command: 

```bash
docker run --rm -it -d \
    -p 18888:18888 \
    -p 4317:18889 \
    --name aspire-dashboard \
    mcr.microsoft.com/dotnet/aspire-dashboard:9.0
```

The two ports exposed by the conainer are :
- 4317:18889 - the port for Open Telemetry Protocol (OTLP). This will be used later in appsettings.json
- 18888:18888 - the port for the Aspire Dashboard

*Note:* You can optionally allow anonymous access to the dashboard by adding the the following environmental variable DOTNET_DASHBOARD_UNSECURED_ALLOW_ANONYMOUS

```bash
docker run --rm -it -d \
    -p 18888:18888 \
    -p 4317:18889 \
    -e DOTNET_DASHBOARD_UNSECURED_ALLOW_ANONYMOUS=True
    --name aspire-dashboard \
    mcr.microsoft.com/dotnet/aspire-dashboard:9.0
```


## Exporting Telemetry to the Aspire Dashboard

To configure applications, do the following to both projects

Use the OpenTelemetry SDK APIs within the application by modifying the csproj file to include references to

- OpenTelemetry.Exporter.OpenTelemetryProtocol
- OpenTelemetry.Extensions.Hosting
- OpenTelemetry.Instrumentation.AspNetCore
- OpenTelemetry.Instrumentation.Http
- OpenTelemetry.Instrumentation.Runtime

You can do that by running the following commands. Note - some of these are marked as pre-release, but can still be used

```bash
    dotnet add package OpenTelemetry.Exporter.OpenTelemetryProtocol
    dotnet add package OpenTelemetry.Extensions.Hosting
    dotnet add package OpenTelemetry.Instrumentation.AspNetCore
    dotnet add package OpenTelemetry.Instrumentation.Http
    dotnet add package OpenTelemetry.Instrumentation.Runtime
```


Start the app with known environment variables (stored in appsettings.json - this is taken from Docker):

 - OTEL_EXPORTER_OTLP_ENDPOINT with a value of http://localhost:4317.

## References

 - Aspire Standalong Dashboard
    - https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/dashboard/standalone?tabs=bash
 - Build a Blazor App
    - https://learn.microsoft.com/en-us/aspnet/core/blazor/tutorials/build-a-blazor-app


# Notes : Mostly for me

## Run the weather api

From the Aspire Dashboard folder, run the weather api and the blazor app in two terminals

```bash
    dotnet run --project WeatherApi
    dotnet run --project AspireApp
```

Swagger endpoint for the API is available from [http://localhost:5074/SWAGGER/index.html](http://localhost:5074/SWAGGER/index.html)
