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

            // Get current settings from API
            var oldSettings = await client.GetSettingsAsync<MyHealthCheckBasicSettings>();

            // Get AWS secrets
            var awsSecrets = await client.GetAllAWSSecretManagerSecretsAsync();
           
            Assert.NotNull(awsSecrets);

            // Update AWS secrets
            awsSecrets["HealthCheckIntervalInMinutes"] = "60";
            awsSecrets["HealthCheckServerHubUrl"] = "https://localhost:5001/newlivehealthcheckshub";
            awsSecrets["AddHealthCheckMiddleware"] = "true";
            awsSecrets.Remove("PublishOnlyWhenNotHealthy");
            awsSecrets.Remove("HealthCheckIntervalCronExpression");

            // Act
            // Replace settings
            var response = await client.ReplaceSettingsAsync(awsSecrets);

            // Get current settings from API
            var updatedSettings = await client.GetSettingsAsync<MyHealthCheckBasicSettings>();

            // Asserts
            Assert.True(response);
            Assert.NotNull(updatedSettings);
            //Not updated
            Assert.Equal(updatedSettings.PublishOnlyWhenNotHealthy, oldSettings.PublishOnlyWhenNotHealthy);
            Assert.Equal(updatedSettings.HealthCheckIntervalCronExpression, oldSettings.HealthCheckIntervalCronExpression);
            //Updated
            Assert.Equal(updatedSettings.HealthCheckIntervalInMinutes, int.Parse(awsSecrets["HealthCheckIntervalInMinutes"]));
            Assert.Equal(updatedSettings.HealthCheckServerHubUrl, awsSecrets["HealthCheckServerHubUrl"]);
            Assert.Equal(updatedSettings.AddHealthCheckMiddleware, bool.Parse(awsSecrets["AddHealthCheckMiddleware"]));
        }
    }
}
