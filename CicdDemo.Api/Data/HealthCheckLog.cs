namespace CicdDemo.Api.Data
{
    internal sealed class HealthCheckLog
    {
        public int Id { get; set; }
        public DateTime CheckedAt { get; set; } = DateTime.UtcNow;
    }
}
