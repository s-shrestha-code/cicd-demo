using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using CicdDemo.Api.Services;

namespace CicdDemo.Tests
{
    public class WeatherServiceTests
    {
        [Fact]
        public void GetForecast_ReturnsCorrectCount()
        {
            var svc = new WeatherService();
            var result = svc.GetForecast(days: 5);
            Assert.Equal(5, result.Count());
        }
    }
}
