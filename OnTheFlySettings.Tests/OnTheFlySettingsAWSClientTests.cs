using Microsoft.Extensions.DependencyInjection;
using OnTheFlySettings.AWSSecretManager.Client;

namespace OnTheFlySettings.Tests
{
    [Collection("SkipInCI")]
    public class OnTheFlySettingsAWSClientTests
    {
        private readonly IServiceProvider _serviceProvider;

        public OnTheFlySettingsAWSClientTests()
        {
            var services = new ServiceCollection();

            services.AddOnTheFlySettingsClient(settings =>
            {
                settings.BaseUrl = "https://localhost:7277";
                settings.TimeoutMilliseconds = 1000 * 60 * 5;

                var region = Environment.GetEnvironmentVariable("AWS_REGION");
                var accessKeyId = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID");
                var secretAccessKey = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY");

                settings.AWS.SecretId = "MyHealthCheckBasicSettings";
                settings.AWS.Region = region!;

                //settings.AWS.UseDefaultAWSCredentialChain = true;
                // OR
                settings.AWS.Credentials.AccessKeyId = accessKeyId!;
                settings.AWS.Credentials.SecretAccessKey = secretAccessKey!;
            });

            _serviceProvider = services.BuildServiceProvider();
        }

        [Fact(Skip = "Skipped in CI pipeline")]
        //[Fact]
        public async Task ReplaceSettingsAsync()
        {
            // Arrange
            var client = _serviceProvider.GetRequiredService<IOnTheFlySettingsClient>();            

            // Get AWS secrets
            var basicSettings = await client.GetAllAWSSecretManagerSecretsAsync<MyHealthCheckBasicSettings>();
           
            Assert.NotNull(basicSettings);

            // Update settings
            basicSettings.HealthCheckIntervalInMinutes = 30;
            basicSettings.HealthCheckIntervalCronExpression = "*/5 * * * *";
            basicSettings.HealthCheckServerHubUrl = "https://localhost:5001/newlivehealthcheckshub";
            basicSettings.PublishOnlyWhenNotHealthy = true;
            basicSettings.AddHealthCheckMiddleware = true;

            // Act
            // Replace settings
            var response = await client.ReplaceSettingsAsync(basicSettings);

            // Get current settings from API
            var currentBasicSettings = await client.GetSettingsAsync<MyHealthCheckBasicSettings>();

            // Asserts
            Assert.True(response);
            Assert.NotNull(basicSettings);
            Assert.NotNull(currentBasicSettings);            
            Assert.Equal(basicSettings.PublishOnlyWhenNotHealthy, currentBasicSettings.PublishOnlyWhenNotHealthy);
            Assert.Equal(basicSettings.HealthCheckIntervalCronExpression, currentBasicSettings.HealthCheckIntervalCronExpression);
            Assert.Equal(basicSettings.HealthCheckIntervalInMinutes, currentBasicSettings.HealthCheckIntervalInMinutes);
            Assert.Equal(basicSettings.HealthCheckServerHubUrl, currentBasicSettings.HealthCheckServerHubUrl);
            Assert.Equal(basicSettings.AddHealthCheckMiddleware, currentBasicSettings.AddHealthCheckMiddleware);
        }
    }
}
