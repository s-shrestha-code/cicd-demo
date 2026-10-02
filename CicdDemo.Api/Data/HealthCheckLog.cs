namespace CicdDemo.Api.Data
{
    public sealed class HealthCheckLog
    {
        public int Id { get; set; }
        public DateTime CheckedAt { get; set; } = DateTime.UtcNow;
    }
}
