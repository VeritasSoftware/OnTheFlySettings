using Microsoft.Extensions.Options;

namespace OnTheFlySettings.Demo
{
    public class OptionsDemoService : IOptionsDemoService
    {
        private readonly MyHealthCheckBasicSettings _settings;

        public OptionsDemoService(IOptionsSnapshot<MyHealthCheckBasicSettings> settings)
        {
            _settings = settings.Value;
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
