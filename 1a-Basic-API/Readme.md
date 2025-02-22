# Running the Aspire Dashboard Standalone

Following are the notes for running this demo

Check it works before Aspire

    dotnet run --project WeatherApi
    dotnet run --project AspireApp


## Starting the Aspire Dashboard Container

To start the container, run the following command: 

```bash
docker run --rm -it -d -p 18888:18888 -p 4317:18889 --name aspire-dashboard mcr.microsoft.com/dotnet/aspire-dashboard:9.0
```

The two ports exposed by the conainer are :
- 4317:18889 - the port for Open Telemetry Protocol (OTLP). This will be used later in appsettings.json
- 18888:18888 - the port for the Aspire Dashboard

*Note:* You can optionally allow anonymous access to the dashboard by adding the the following environmental variable DOTNET_DASHBOARD_UNSECURED_ALLOW_ANONYMOUS

```bash
docker run --rm -it -d -p 18888:18888 -p 4317:18889 -e DOTNET_DASHBOARD_UNSECURED_ALLOW_ANONYMOUS=True --name aspire-dashboard mcr.microsoft.com/dotnet/aspire-dashboard:9.0
```


## 1 - Exporting Telemetry to the Aspire Dashboard

To configure applications, do the following to both projects

You can do that by running the following commands. Note - some of these are marked as pre-release, but can still be used

```bash
    dotnet add ./AspireApp package OpenTelemetry.Exporter.OpenTelemetryProtocol
    dotnet add ./AspireApp package OpenTelemetry.Extensions.Hosting
    dotnet add ./AspireApp package OpenTelemetry.Instrumentation.AspNetCore
    dotnet add ./AspireApp package OpenTelemetry.Instrumentation.Http
    dotnet add ./AspireApp package OpenTelemetry.Instrumentation.Runtime

    dotnet add ./WeatherApi package OpenTelemetry.Exporter.OpenTelemetryProtocol
    dotnet add ./WeatherApi package OpenTelemetry.Extensions.Hosting
    dotnet add ./WeatherApi package OpenTelemetry.Instrumentation.AspNetCore
    dotnet add ./WeatherApi package OpenTelemetry.Instrumentation.Http
    dotnet add ./WeatherApi package OpenTelemetry.Instrumentation.Runtime
```


And update the Relevant Program.cs file for the WeatherApi and the Blazor App



## 2 - Add the Service Defaults Project

Run the following Comand

```bash
dotnet new aspire-servicedefaults -n ServiceDefaults
dotnet sln add .\ServiceDefaults\
```

The OTel references can now be removed from the other projects

Import the ServiceDefaults class into that project and add a reference to it in the WeatherApi project

```bash
dotnet add .\WeatherApi\WeatherApi.csproj reference .\ServiceDefaults\ServiceDefaults.csproj
dotnet add .\AspireApp\AspireApp.csproj reference .\ServiceDefaults\ServiceDefaults.csproj
```

## References

 - Aspire Standalong Dashboard
    - https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/dashboard/standalone?tabs=bash
 - Build a Blazor App
    - https://learn.microsoft.com/en-us/aspnet/core/blazor/tutorials/build-a-blazor-app
 - Metrics and Tracing with OpenTelemetry
    - https://learn.microsoft.com/en-us/samples/dotnet/aspire-samples/aspire-metrics/



