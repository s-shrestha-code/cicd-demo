using CicdDemo.Api;

namespace CicdDemo.Tests
{
    public class AppSettingsTest
    {
        private readonly AppSettings appSettings = new();

        [Fact]
        public void AppSettings_ApiKey_ReturnsSetValue()
        {
            string apiKeyValue = "api-key";
            appSettings.ApiKey = apiKeyValue;
            Assert.Equal(apiKeyValue, appSettings.ApiKey);
        }
    }
}
