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
docker run --rm -it -d -p 18888:18888 -p 4317:18889 -e DOTNET_DASHBOARD_UNSECURED_ALLOW_ANONYMOUS=True
    --name aspire-dashboard mcr.microsoft.com/dotnet/aspire-dashboard:9.0
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


And update the Program.cs file to include the following code

```csharp

// Configure OTLP exporter
var openTelemetryUri = new Uri(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);

// Configure Logging
builder.Logging.AddOpenTelemetry(log =>
{
   log.AddOtlpExporter(opt => opt.Endpoint = openTelemetryUri);
   log.IncludeScopes = true;
   log.IncludeFormattedMessage = true;
});


// Configure OpenTelemetry
builder.Services.AddOpenTelemetry()
  .ConfigureResource(res => res
      .AddService(WeatherMetrics.ServiceName))
  .WithMetrics(metrics =>
  {
      metrics
          .AddHttpClientInstrumentation()
          .AddAspNetCoreInstrumentation()
          .AddRuntimeInstrumentation();

      metrics.AddMeter(WeatherMetrics.Meter.Name);

      metrics.AddOtlpExporter(opt => opt.Endpoint = openTelemetryUri);
  })
  .WithTracing(tracing =>
      {using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using WeatherApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(); // Add this to support controllers
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Inject my test service
builder.Services.AddScoped<ITemperatureService, TemperatureService>();

// Configure logging
builder.Logging.SetMinimumLevel(LogLevel.Information); // Change to LogLevel.Debug, Trace, etc., as needed

// Configure OTLP exporter
var openTelemetryUri = new Uri(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);

// Configure Logging
builder.Logging.AddOpenTelemetry(log =>
{
   log.AddOtlpExporter(opt => opt.Endpoint = openTelemetryUri);
   log.IncludeScopes = true;
   log.IncludeFormattedMessage = true;
});


// Configure OpenTelemetry
builder.Services.AddOpenTelemetry()
  .ConfigureResource(res => res
      .AddService(WeatherMetrics.ServiceName))
  .WithMetrics(metrics =>
  {
      metrics
          .AddHttpClientInstrumentation()
          .AddAspNetCoreInstrumentation()
          .AddRuntimeInstrumentation();

      metrics.AddMeter(WeatherMetrics.Meter.Name);

      metrics.AddOtlpExporter(opt => opt.Endpoint = openTelemetryUri);
  })
  .WithTracing(tracing =>
      {

          tracing
              .AddAspNetCoreInstrumentation()
              .AddHttpClientInstrumentation();

          tracing.AddOtlpExporter(opt => opt.Endpoint = openTelemetryUri);

      }
  );


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization(); // Add authorization middleware if needed

app.MapControllers(); // Use controllers

app.Run();


```



Start the app with known environment variables (stored in appsettings.json - this is taken from Docker):

 - OTEL_EXPORTER_OTLP_ENDPOINT with a value of http://localhost:4317.

Then copy the contents of Program.cs from the Files into the WeatherApi project




## 2 - Add the Service Defaults Project

Run the following Comand

```bash
dotnet new classlib -n ServiceDefaults
dotnet sln add .\ServiceDefaults\
```

Import the ServiceDefaults class into that project and add a reference to it in the WeatherApi project

```bash
dotnet add .\WeatherApi\WeatherApi.csproj reference .\ServiceDefaults\ServiceDefaults.csproj
```

And Add the required open telemetry nuget packages



```bash
    dotnet add ./ServiceDefaults package OpenTelemetry.Exporter.OpenTelemetryProtocol
    dotnet add ./ServiceDefaults package OpenTelemetry.Extensions.Hosting
    dotnet add ./ServiceDefaults package OpenTelemetry.Instrumentation.AspNetCore
    dotnet add ./ServiceDefaults package OpenTelemetry.Instrumentation.Http
    dotnet add ./ServiceDefaults package OpenTelemetry.Instrumentation.Runtime
    dotnet add ./ServiceDefaults package Microsoft.Extensions.Http.Resilience
    dotnet add ./ServiceDefaults package Microsoft.Extensions.ServiceDiscovery
```

## References

 - Aspire Standalong Dashboard
    - https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/dashboard/standalone?tabs=bash
 - Build a Blazor App
    - https://learn.microsoft.com/en-us/aspnet/core/blazor/tutorials/build-a-blazor-app


