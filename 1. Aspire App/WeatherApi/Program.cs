//using OpenTelemetry.Logs;
//using OpenTelemetry.Metrics;
//using OpenTelemetry.Resources;
//using OpenTelemetry.Trace;
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

//// Configure OTLP exporter
//var openTelemetryUri = new Uri(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);

//// Configure Logging
//builder.Logging.AddOpenTelemetry(log =>
//{
//    log.AddOtlpExporter(opt => opt.Endpoint = openTelemetryUri);
//    log.IncludeScopes = true;
//    log.IncludeFormattedMessage = true;
//});


//// Configure OpenTelemetry
//builder.Services.AddOpenTelemetry()
//   .ConfigureResource(res => res
//       .AddService(WeatherMetrics.ServiceName))
//   .WithMetrics(metrics =>
//   {
//       metrics
//           .AddHttpClientInstrumentation()
//           .AddAspNetCoreInstrumentation()
//           .AddRuntimeInstrumentation();

//       metrics.AddMeter(WeatherMetrics.Meter.Name);

//       metrics.AddOtlpExporter(opt => opt.Endpoint = openTelemetryUri);
//   })
//   .WithTracing(tracing =>
//       {

//           tracing
//               .AddAspNetCoreInstrumentation()
//               .AddHttpClientInstrumentation();

//           tracing.AddOtlpExporter(opt => opt.Endpoint = openTelemetryUri);

//       }
//   );


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
