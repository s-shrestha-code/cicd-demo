using CicdDemo.Api.Data;
using CicdDemo.Api.Helpers.HealthCheck;
using CicdDemo.Tests.Helper;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CicdDemo.Tests
{
    public class HealthCheckLogTest
    {
        private readonly HealthCheckLog healthCheckLog = new();

        [Fact]
        public void HealthCheckLog_Id_ReturnsSetValue()
        {
            int id = 1;
            healthCheckLog.Id = id;
            Assert.Equal(id, healthCheckLog.Id);
        }

        [Fact]
        public async Task DatabaseHealthCheck_Returns_HealthCheckResult()
        {
            var context = TestHelper.CreateDbContext();

            var databaseHealthCheck = new DatabaseHealthCheck(context);
            Task<HealthCheckResult> result = databaseHealthCheck.CheckHealthAsync(new HealthCheckContext(), new CancellationToken());
            Assert.NotNull(result);
        }
    }
}
