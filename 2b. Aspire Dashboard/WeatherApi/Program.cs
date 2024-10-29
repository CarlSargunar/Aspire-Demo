//using OpenTelemetry.Logs;
//using OpenTelemetry.Metrics;
//using OpenTelemetry.Resources;
//using OpenTelemetry.Trace;
using WeatherApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();
//builder.AddRedisOutputCache("cache");

// Add services to the container.
builder.Services.AddControllers(); // Add this to support controllers
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Inject my test service
builder.Services.AddScoped<ITemperatureService, TemperatureService>();


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
