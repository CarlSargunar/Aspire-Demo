// using OpenTelemetry.Logs;
// using OpenTelemetry.Metrics;
// using OpenTelemetry.Resources;
// using OpenTelemetry.Trace;
using AspireApp.Components;
using AspireApp.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register WeatherApiClient to be used for making HTTP requests
builder.Services.AddHttpClient<WeatherApiClient>(client =>
{
    // This URL uses "https+http://" to indicate HTTPS is preferred over HTTP.
    // Learn more about service discovery scheme resolution at https://aka.ms/dotnet/sdschemes.
    client.BaseAddress = new("http://localhost:5074/weatherforecast");
});

// Configure logging
builder.Logging.SetMinimumLevel(LogLevel.Information); // Change to LogLevel.Debug, Trace, etc., as needed

// // Configure OTLP exporter
// var openTelemetryUri = new Uri(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);

// // Configure OpenTelemetry Logging
// builder.Logging.AddOpenTelemetry(log =>
// {
//     log.AddOtlpExporter(opt => opt.Endpoint = openTelemetryUri);
//     log.IncludeScopes = true;
//     log.IncludeFormattedMessage = true;
// });

// // Give it a name
// builder.Services.AddOpenTelemetry()
//   .ConfigureResource(res => res
//       .AddService("Weather Dashboard"))
//     .WithTracing(tracing =>
//       {

//           tracing
//               .AddAspNetCoreInstrumentation()
//               .AddHttpClientInstrumentation();

//           tracing.AddOtlpExporter(opt => opt.Endpoint = openTelemetryUri);

//       }
//   );


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
