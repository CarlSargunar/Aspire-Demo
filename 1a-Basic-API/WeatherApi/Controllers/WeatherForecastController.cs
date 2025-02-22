using Microsoft.AspNetCore.Mvc;
using WeatherApi.Models;
using WeatherApi.Services;
namespace YourNamespace.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly ILogger<WeatherForecastController> _logger;
    private readonly ITemperatureService _temperatureService;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, ITemperatureService temperatureService)
    {
        _logger = logger;
        _temperatureService = temperatureService;
    }

    private static readonly string[] Summaries = new[]
    {
        "Wet", "Rainy", "Drizzly", "Mizzly", "Cats and Dogs", "Spitting", "Bucketing down", "Pissing", "Smirr", "Plothering"
    };

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        var range = Random.Shared.Next(5, 30);
        var forecasts = Enumerable.Range(1, range).Select(index => new WeatherForecast
            (
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Random.Shared.Next(-5, 30),  
                Summaries[Random.Shared.Next(Summaries.Length)]
            ))
            .ToArray();

        var hotDays = forecasts.Count(x => x.TemperatureC > 20);
        var coldDays = forecasts.Count(x => x.TemperatureC < 5);
        var maxTemp = forecasts.Max(x => x.TemperatureC);
        var minTemp = forecasts.Min(x => x.TemperatureC);

        // Some logging
        _logger.LogInformation("Generated {0} weather reports. {1} hot days, {2} cold days.", forecasts.Length, hotDays, coldDays);

        // Calling another service
        _temperatureService.ReactToTemperature(maxTemp);
        _temperatureService.ReactToTrend(hotDays, coldDays);

        //// Record Metrics
        //WeatherMetrics.Count.Add(1);
        //WeatherMetrics.MaxTemp.Record(maxTemp);
        //WeatherMetrics.MinTemp.Record(minTemp);


        return forecasts;
    }
}


