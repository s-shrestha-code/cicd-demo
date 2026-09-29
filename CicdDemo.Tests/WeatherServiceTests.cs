using CicdDemo.Api.Services;

namespace CicdDemo.Tests
{
    public class WeatherServiceTests
    {
        private readonly WeatherService _svc = new();

        #region Tests - GetForecast

        [Fact]
        public void GetForecast_EachForecast_HasRequestedDateOneTime()
        {
            var _startDate = DateTime.Today.Date;
            var _endDate = DateTime.Today.AddDays(8).Date;

            var dates = new List<DateTime>();
            for (var dt = _startDate; dt <= _endDate; dt = dt.AddDays(1))
            {
                dates.Add(dt);
            }

            var result = _svc.GetForecast(_startDate, _endDate);
            Assert.All(result, f => Assert.True(dates.Count(d => (DateOnly.FromDateTime(d.Date) == f.Date)) == 1));
        }

        [Fact]
        public void GetForecast_DefaultDays_ReturnsSixItems()
        {
            var result = _svc.GetForecast();
            Assert.Equal(6, result.Count());
        }

        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(10)]
        public void GetForecast_CustomDays_ReturnsCorrectCount(int days)
        {
            var result = _svc.GetForecast(days);
            Assert.Equal(days, result.Count());
        }

        [Fact]
        public void GetForecast_EachForecast_HasFutureDate()
        {
            var result = _svc.GetForecast();
            var today = DateOnly.FromDateTime(DateTime.Now);
            Assert.All(result, f => Assert.True(f.Date > today));
        }

        [Fact]
        public void GetForecast_EachForecast_HasValidTemperature()
        {
            var result = _svc.GetForecast(20);
            Assert.All(result, f =>
            {
                Assert.InRange(f.TemperatureC, -20, 55);
                Assert.True(f.TemperatureF == 32 + (int)(f.TemperatureC / 0.5556));
            });
        }

        [Fact]
        public void GetForecast_EachForecast_HasNonNullSummary()
        {
            var result = _svc.GetForecast();
            Assert.All(result, f => Assert.NotNull(f.Summary));
        }

        #endregion

        #region Tests - GetHistoricalForecast

        [Fact]
        public void GetHistoricalForecast_DefaultDays_ReturnsSixItems()
        {
            var result = _svc.GetHistoricalForecast();
            Assert.Equal(6, result.Count());
        }

        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(10)]
        public void GetHistoricalForecast_CustomDays_ReturnsCorrectCount(int days)
        {
            var result = _svc.GetHistoricalForecast(days);
            Assert.Equal(days, result.Count());
        }

        [Fact]
        public void GetHistoricalForecast_EachForecast_HasPastDate()
        {
            var result = _svc.GetHistoricalForecast();
            var today = DateOnly.FromDateTime(DateTime.Now);
            Assert.All(result, f => Assert.True(f.Date < today));
        }

        [Fact]
        public void GetHistoricalForecast_EachForecast_HasValidTemperature()
        {
            var result = _svc.GetHistoricalForecast(20);
            Assert.All(result, f =>
            {
                Assert.InRange(f.TemperatureC, -20, 55);
                Assert.True(f.TemperatureF == 32 + (int)(f.TemperatureC / 0.5556));
            });
        }

        [Fact]
        public void GetHistoricalForecast_EachForecast_HasNonNullSummary()
        {
            var result = _svc.GetHistoricalForecast();
            Assert.All(result, f => Assert.NotNull(f.Summary));
        }

        #endregion
    }
}
