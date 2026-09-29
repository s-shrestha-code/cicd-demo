namespace CicdDemo.Api.Services
{
    public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
    {
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }

    public class WeatherService
    {
        private static readonly string[] Summaries =
        [
            "Freezing", "Bracing", "Chilly", "Cool", "Mild",
            "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        ];

        public IEnumerable<WeatherForecast> GetForecast(DateTime startDate, DateTime endDate)
        {
            var result = new List<WeatherForecast>();

            DateTime _startDate = (startDate < endDate) ? startDate.Date : endDate.Date;
            DateTime _endDate = (startDate < endDate) ? endDate.Date : startDate.Date;

            for (var dt = _startDate; dt <= _endDate; dt = dt.AddDays(1))
            {
                result.Add(new WeatherForecast(
                        DateOnly.FromDateTime(dt),
                        Random.Shared.Next(-20, 55),
                        Summaries[Random.Shared.Next(Summaries.Length)]
                ));
            }

            return result;
        }

        public IEnumerable<WeatherForecast> GetForecast(int days = 6)
        {
            return this.GetForecast(DateTime.Now.AddDays(1), DateTime.Now.AddDays(days));
        }

        public IEnumerable<WeatherForecast> GetHistoricalForecast(int days = 6)
        {
            return this.GetForecast(DateTime.Now.AddDays(days * -1), DateTime.Now.AddDays(-1));
        }
    }
}
