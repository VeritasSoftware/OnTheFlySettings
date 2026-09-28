using Microsoft.Extensions.DependencyInjection;
using OnTheFlySettings.AzureKeyVault.Client;

namespace OnTheFlySettings.Tests
{
    [Collection("SkipInCI")]
    public class OnTheFlySettingsAzureClientTests
    {
        private readonly IServiceProvider _serviceProvider;

        public OnTheFlySettingsAzureClientTests()
        {
            var services = new ServiceCollection();

            services.AddOnTheFlySettingsClient(settings =>
            {
                settings.BaseUrl = "https://localhost:7277";
                settings.TimeoutMilliseconds = 1000 * 60 * 5;

                var tenantId = Environment.GetEnvironmentVariable("AZURE_TENANT_ID");
                var clientId = Environment.GetEnvironmentVariable("AZURE_CLIENT_ID");
                var clientSecret = Environment.GetEnvironmentVariable("AZURE_CLIENT_SECRET");

                settings.Azure.KeyVaultUrl = "https://ontheflysettings.vault.azure.net/";
                
                settings.Azure.UseManagedIdentity = true;
                // OR                
                settings.Azure.Credentials.TenantId = tenantId;
                settings.Azure.Credentials.ClientId = clientId;
                settings.Azure.Credentials.ClientSecret = clientSecret;
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

            // Get Azure secrets
            var azureSecrets = await client.GetAllAzureKeyVaultSecretsAsync();

            // Update Azure secrets
            azureSecrets["HealthCheckIntervalInMinutes"] = "60";
            azureSecrets["HealthCheckServerHubUrl"] = "https://localhost:5001/newlivehealthcheckshub";
            azureSecrets["AddHealthCheckMiddleware"] = "true";
            azureSecrets.Remove("PublishOnlyWhenNotHealthy");
            azureSecrets.Remove("HealthCheckIntervalCronExpression");

            // Act
            // Replace settings
            var response = await client.ReplaceSettingsAsync(azureSecrets);

            // Get current settings from API after update
            var updatedSettings = await client.GetSettingsAsync<MyHealthCheckBasicSettings>();

            // Asserts
            Assert.True(response);
            Assert.NotNull(updatedSettings);
            //Not updated
            Assert.Equal(updatedSettings.PublishOnlyWhenNotHealthy, oldSettings.PublishOnlyWhenNotHealthy);
            Assert.Equal(updatedSettings.HealthCheckIntervalCronExpression, oldSettings.HealthCheckIntervalCronExpression);
            //Updated
            Assert.Equal(updatedSettings.HealthCheckIntervalInMinutes, int.Parse(azureSecrets["HealthCheckIntervalInMinutes"]));
            Assert.Equal(updatedSettings.HealthCheckServerHubUrl, azureSecrets["HealthCheckServerHubUrl"]);
            Assert.Equal(updatedSettings.AddHealthCheckMiddleware, bool.Parse(azureSecrets["AddHealthCheckMiddleware"]));
        }
    }
}
