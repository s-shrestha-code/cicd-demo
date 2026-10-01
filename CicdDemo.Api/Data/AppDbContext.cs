using Microsoft.EntityFrameworkCore;

namespace CicdDemo.Api.Data
{
    public class AppDbContext : DbContext
    {
        internal DbSet<HealthCheckLog> HealthCheckLogs => Set<HealthCheckLog>();

        public DbSet<Customer> Customers => Set<Customer>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }
}