using Microsoft.AspNetCore.Mvc.Testing;

namespace OnTheFlySettings.Tests
{
    public class OnTheFlySettingsIntegrationTests : IClassFixture<WebApplicationFactory<Demo.Program>>
    {
        private readonly HttpClient _client;

        public OnTheFlySettingsIntegrationTests(WebApplicationFactory<Demo.Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task ReplaceSettingsAsync()
        {
            // Get the current settings
            var response = await _client.GetFromJsonAsync<MyHealthCheckBasicSettings>("/settings");

            // Assert the current settings
            Assert.NotNull(response);
            Assert.Equal(15, response.HealthCheckIntervalInMinutes);
            Assert.Equal("* * * * *", response.HealthCheckIntervalCronExpression);
            Assert.Equal("https://localhost:5001/livehealthcheckshub", response.HealthCheckServerHubUrl);
            Assert.False(response.PublishOnlyWhenNotHealthy);
            Assert.False(response.AddHealthCheckMiddleware);

            var newSettings = new MyHealthCheckBasicSettings
            {
                HealthCheckIntervalInMinutes = 30,
                HealthCheckIntervalCronExpression = "*/5 * * * *",
                HealthCheckServerHubUrl = "https://localhost:5001/newlivehealthcheckshub",
                PublishOnlyWhenNotHealthy = true,
                AddHealthCheckMiddleware = true
            };

            // Replace the settings
            var putResponse = await _client.PutAsJsonAsync("/settings/replace", newSettings);

            Assert.True(putResponse.IsSuccessStatusCode);

            // Get the updated settings
            var responseGet = await _client.GetFromJsonAsync<MyHealthCheckBasicSettings>("/settings");

            // Assert the updated settings
            Assert.NotNull(responseGet);
            Assert.Equal(newSettings.HealthCheckIntervalInMinutes, responseGet.HealthCheckIntervalInMinutes);
            Assert.Equal(newSettings.HealthCheckIntervalCronExpression, responseGet.HealthCheckIntervalCronExpression);
            Assert.Equal(newSettings.HealthCheckServerHubUrl, responseGet.HealthCheckServerHubUrl);
            Assert.Equal(newSettings.PublishOnlyWhenNotHealthy, responseGet.PublishOnlyWhenNotHealthy);
            Assert.Equal(newSettings.AddHealthCheckMiddleware, responseGet.AddHealthCheckMiddleware);
        }
    }
}
