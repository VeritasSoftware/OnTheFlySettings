namespace OnTheFlySettings.Demo
{
    public class DemoService : IDemoService
    {
        private readonly IOnTheFlySettings<MyHealthCheckBasicSettings> _settings;

        public DemoService(IOnTheFlySettings<MyHealthCheckBasicSettings> settings)
        {
            _settings = settings;
        }

        public void PrintCurrentSettings()
        {
            var currentSettings = _settings.Current;
            Console.WriteLine("Current Settings:");
            Console.WriteLine($"HealthCheckIntervalInMinutes: {currentSettings.HealthCheckIntervalInMinutes}");
            Console.WriteLine($"HealthCheckIntervalCronExpression: {currentSettings.HealthCheckIntervalCronExpression}");
            Console.WriteLine($"HealthCheckServerHubUrl: {currentSettings.HealthCheckServerHubUrl}");
            Console.WriteLine($"PublishOnlyWhenNotHealthy: {currentSettings.PublishOnlyWhenNotHealthy}");
            Console.WriteLine($"AddHealthCheckMiddleware: {currentSettings.AddHealthCheckMiddleware}");
        }
    }
}
