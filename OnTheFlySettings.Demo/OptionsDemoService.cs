using Microsoft.Extensions.Options;

namespace OnTheFlySettings.Demo
{
    public class OptionsDemoService : IOptionsDemoService
    {
        private readonly IOptionsSnapshot<MyHealthCheckBasicSettings> _settings;

        public OptionsDemoService(IOptionsSnapshot<MyHealthCheckBasicSettings> settings)
        {
            _settings = settings;
        }

        public void PrintCurrentSettings()
        {
            var currentSettings = _settings.Value;
            Console.WriteLine("Current Settings:");
            Console.WriteLine($"HealthCheckIntervalInMinutes: {currentSettings.HealthCheckIntervalInMinutes}");
            Console.WriteLine($"HealthCheckIntervalCronExpression: {currentSettings.HealthCheckIntervalCronExpression}");
            Console.WriteLine($"HealthCheckServerHubUrl: {currentSettings.HealthCheckServerHubUrl}");
            Console.WriteLine($"PublishOnlyWhenNotHealthy: {currentSettings.PublishOnlyWhenNotHealthy}");
            Console.WriteLine($"AddHealthCheckMiddleware: {currentSettings.AddHealthCheckMiddleware}");
        }
    }
}
