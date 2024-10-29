using System.Diagnostics.Metrics;

public static class WeatherMetrics
{
    // Resource name for Aspire Dashboard
    public const string ServiceName = "British Weather API";

    public static Meter Meter = new(ServiceName);

    // Metric to track the number of weather reports
    public static Counter<int> Count = Meter.CreateCounter<int>("weather.report.callcount", "Count", "Number of Calls");
    public static Histogram<int> MaxTemp = Meter.CreateHistogram<int>("weather.report.maxtemp", "Temperature in Celsius");
    public static Histogram<int> MinTemp = Meter.CreateHistogram<int>("weather.report.mintemp", "Temperature in Celsius");
}