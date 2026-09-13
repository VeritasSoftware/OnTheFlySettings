using Microsoft.Extensions.DependencyInjection;
using OnTheFlySettings.Client;

namespace OnTheFlySettings.Tests
{
    [Collection("SkipInCI")]
    public class OnTheFlySettingsClientTests
    {
        private readonly IServiceProvider _serviceProvider;
        
        public OnTheFlySettingsClientTests()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddOnTheFlySettingsClient(settings =>
            {
                settings.BaseUrl = "https://localhost:7277";
                settings.TimeoutMilliseconds = 1000;
            });

            _serviceProvider = services.BuildServiceProvider();
        }

        [Fact(Skip = "Skipped in CI pipeline")]
        //[Fact]
        public async Task GetSettingsAsync()
        {
            var client = _serviceProvider.GetRequiredService<IOnTheFlySettingsClient>();

            var response = await client.GetSettingsAsync<MyHealthCheckBasicSettings>();

            Assert.NotNull(response);
            Assert.Equal(15, response.HealthCheckIntervalInMinutes);
            Assert.Equal("* * * * *", response.HealthCheckIntervalCronExpression);
            Assert.Equal("https://localhost:5001/livehealthcheckshub", response.HealthCheckServerHubUrl);
            Assert.False(response.PublishOnlyWhenNotHealthy);
            Assert.False(response.AddHealthCheckMiddleware);
        }

        [Fact(Skip = "Skipped in CI pipeline")]
        //[Fact]
        public async Task ReplaceSettingsAsync()
        {
            var client = _serviceProvider.GetRequiredService<IOnTheFlySettingsClient>();

            var newSettings = new MyHealthCheckBasicSettings
            {
                HealthCheckIntervalInMinutes = 30,
                HealthCheckIntervalCronExpression = "*/5 * * * *",
                HealthCheckServerHubUrl = "https://localhost:5001/newlivehealthcheckshub",
                PublishOnlyWhenNotHealthy = true,
                AddHealthCheckMiddleware = true
            };

            var response = await client.ReplaceSettingsAsync(newSettings);

            Assert.True(response);

            var responseGet = await client.GetSettingsAsync<MyHealthCheckBasicSettings>();

            Assert.NotNull(responseGet);
            Assert.Equal(newSettings.HealthCheckIntervalInMinutes, responseGet.HealthCheckIntervalInMinutes);
            Assert.Equal(newSettings.HealthCheckIntervalCronExpression, responseGet.HealthCheckIntervalCronExpression);
            Assert.Equal(newSettings.HealthCheckServerHubUrl, responseGet.HealthCheckServerHubUrl);
            Assert.Equal(newSettings.PublishOnlyWhenNotHealthy, responseGet.PublishOnlyWhenNotHealthy);
            Assert.Equal(newSettings.AddHealthCheckMiddleware, responseGet.AddHealthCheckMiddleware);
        }
    }
}
