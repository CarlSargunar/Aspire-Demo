namespace WeatherApi.Services
{
    public class TemperatureService : ITemperatureService
    {
        private readonly ILogger<TemperatureService> _logger;

        public TemperatureService(ILogger<TemperatureService> logger)
        {
            _logger = logger;
        }


        public void ReactToTemperature(int maxTemp)
        {
            if (maxTemp > 28)
            {
                _logger.LogCritical("Overheating!!! Temp : {0}", maxTemp);
            }
        }

        public void ReactToTrend(int hotDays, int coldDays)
        {
            if (hotDays > coldDays)
            {
                // Sleep for a random amount of time between 0.3 and 0.8 seconds
                var sleep = Random.Shared.Next(300, 800);
                Thread.Sleep(sleep);
                int heatwaveDays = hotDays - coldDays;
                _logger.LogWarning("Heatwave!!! Things are slowing down: {0}", heatwaveDays);
            }
        }
    }

    public interface ITemperatureService
    {
        void ReactToTemperature(int maxTemp);
        void ReactToTrend(int hotDays, int coldDays);
    }

}
