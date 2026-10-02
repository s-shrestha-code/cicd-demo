using CicdDemo.Api.Data;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CicdDemo.Api.Helpers.HealthCheck
{
    public sealed class DatabaseHealthCheck : IHealthCheck
    {
        private readonly AppDbContext _context;

        public DatabaseHealthCheck(AppDbContext context)
        {
            _context = context;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
        {
            // Use a transaction so the test row is automatically cleaned up
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                // 1. Attempt to insert a test record
                var testLog = new HealthCheckLog();
                await _context.Set<HealthCheckLog>().AddAsync(testLog, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                // 2. Rollback immediately to prevent database bloat
                await transaction.RollbackAsync(cancellationToken);

                return HealthCheckResult.Healthy("Database write check succeeded and was rolled back cleanly.");
            }
            catch (Exception ex)
            {
                // Transaction auto-rolls back on failure/dispose if not committed
                return HealthCheckResult.Unhealthy("Database write check failed.", ex);
            }
        }
    }
}
