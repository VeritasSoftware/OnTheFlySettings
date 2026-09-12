namespace OnTheFlySettings.Tests
{
    public class MyHealthCheckBasicSettings
    {
        public int? HealthCheckIntervalInMinutes { get; set; } = 15;
        public string? HealthCheckIntervalCronExpression { get; set; }
        public string HealthCheckServerHubUrl { get; set; } = string.Empty;
        public bool PublishOnlyWhenNotHealthy { get; set; }
        public bool AddHealthCheckMiddleware { get; set; } = false;
    }
}
