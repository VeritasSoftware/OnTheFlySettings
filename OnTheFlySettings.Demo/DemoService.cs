namespace OnTheFlySettings.Demo
{
    public class DemoService : IDemoService
    {
        private readonly MyHealthCheckBasicSettings _settings;

        public DemoService(IOnTheFlySettings<MyHealthCheckBasicSettings> settings)
        {
            _settings = settings.Current;
        }

        public void PrintCurrentSettings()
        {
            Console.WriteLine("Current Settings:");
            Console.WriteLine($"HealthCheckIntervalInMinutes: {_settings.HealthCheckIntervalInMinutes}");
            Console.WriteLine($"HealthCheckIntervalCronExpression: {_settings.HealthCheckIntervalCronExpression}");
            Console.WriteLine($"HealthCheckServerHubUrl: {_settings.HealthCheckServerHubUrl}");
            Console.WriteLine($"PublishOnlyWhenNotHealthy: {_settings.PublishOnlyWhenNotHealthy}");
            Console.WriteLine($"AddHealthCheckMiddleware: {_settings.AddHealthCheckMiddleware}");
        }
    }
}
