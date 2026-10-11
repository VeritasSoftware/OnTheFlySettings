namespace OnTheFlySettings.Demo
{
    public class MyHealthCheckBasicSettings
    {
        public int HealthCheckIntervalInMinutes { get; set; }
        public string HealthCheckIntervalCronExpression { get; set; } = string.Empty;
        public string HealthCheckServerHubUrl { get; set; } = string.Empty;
        public bool PublishOnlyWhenNotHealthy { get; set; }
        public bool AddHealthCheckMiddleware { get; set; }
    }
}
