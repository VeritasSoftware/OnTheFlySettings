using System.Text.Json;

namespace OnTheFlySettings.Tests
{
    public class OnTheFlySettingsTests
    {
        [Fact]
        public void Replace()
        {
            // Arrange
            var originalSettings = new MyHealthCheckBasicSettings
            {
                HealthCheckIntervalInMinutes = 15,
                HealthCheckIntervalCronExpression = "* * * * *",
                HealthCheckServerHubUrl = "https://localhost:5001/livehealthcheckshub",
                PublishOnlyWhenNotHealthy = false,
                AddHealthCheckMiddleware = false
            };            

            var onTheFlySettings = new OnTheFlySettings<MyHealthCheckBasicSettings>(originalSettings);

            var updatedSettings = new MyHealthCheckBasicSettings
            {
                HealthCheckIntervalInMinutes = 30,
                HealthCheckIntervalCronExpression = "*/5 * * * *",
                HealthCheckServerHubUrl = "https://localhost:5001/newlivehealthcheckshub",
                PublishOnlyWhenNotHealthy = true,
                AddHealthCheckMiddleware = true
            };

            var jsonElement = CreateJsonElement(updatedSettings);

            // Act
            onTheFlySettings.Replace(jsonElement);

            // Assert
            Assert.NotNull(onTheFlySettings.Old);
            Assert.Equal(onTheFlySettings.Old.HealthCheckIntervalInMinutes, originalSettings.HealthCheckIntervalInMinutes);
            Assert.Equal(onTheFlySettings.Old.HealthCheckIntervalCronExpression, originalSettings.HealthCheckIntervalCronExpression);
            Assert.Equal(onTheFlySettings.Old.HealthCheckServerHubUrl, originalSettings.HealthCheckServerHubUrl);
            Assert.Equal(onTheFlySettings.Old.PublishOnlyWhenNotHealthy, originalSettings.PublishOnlyWhenNotHealthy);
            Assert.Equal(onTheFlySettings.Old.AddHealthCheckMiddleware, originalSettings.AddHealthCheckMiddleware);

            Assert.NotNull(onTheFlySettings.Current);
            Assert.Equal(onTheFlySettings.Current.HealthCheckIntervalInMinutes, updatedSettings.HealthCheckIntervalInMinutes);
            Assert.Equal(onTheFlySettings.Current.HealthCheckIntervalCronExpression, updatedSettings.HealthCheckIntervalCronExpression);
            Assert.Equal(onTheFlySettings.Current.HealthCheckServerHubUrl, updatedSettings.HealthCheckServerHubUrl);
            Assert.Equal(onTheFlySettings.Current.PublishOnlyWhenNotHealthy, updatedSettings.PublishOnlyWhenNotHealthy);
            Assert.Equal(onTheFlySettings.Current.AddHealthCheckMiddleware, updatedSettings.AddHealthCheckMiddleware);
        }

        [Fact]
        public void ReplaceDictionary()
        {
            // Arrange
            var originalSettings = new MyHealthCheckBasicSettings
            {
                HealthCheckIntervalInMinutes = 15,
                HealthCheckIntervalCronExpression = "* * * * *",
                HealthCheckServerHubUrl = "https://localhost:5001/livehealthcheckshub",
                PublishOnlyWhenNotHealthy = false,
                AddHealthCheckMiddleware = false
            };

            var onTheFlySettings = new OnTheFlySettings<MyHealthCheckBasicSettings>(originalSettings);

            var updatedSettings = new Dictionary<string, string>();

            updatedSettings.Add("HealthCheckIntervalInMinutes", "30");
            updatedSettings.Add("HealthCheckIntervalCronExpression", "*/5 * * * *");
            updatedSettings.Add("HealthCheckServerHubUrl", "https://localhost:5001/newlivehealthcheckshub");
            updatedSettings.Add("PublishOnlyWhenNotHealthy", "true");
            updatedSettings.Add("AddHealthCheckMiddleware", "true");                        

            // Act
            onTheFlySettings.Replace(updatedSettings);

            // Assert
            Assert.NotNull(onTheFlySettings.Old);
            Assert.Equal(onTheFlySettings.Old.HealthCheckIntervalInMinutes, originalSettings.HealthCheckIntervalInMinutes);
            Assert.Equal(onTheFlySettings.Old.HealthCheckIntervalCronExpression, originalSettings.HealthCheckIntervalCronExpression);
            Assert.Equal(onTheFlySettings.Old.HealthCheckServerHubUrl, originalSettings.HealthCheckServerHubUrl);
            Assert.Equal(onTheFlySettings.Old.PublishOnlyWhenNotHealthy, originalSettings.PublishOnlyWhenNotHealthy);
            Assert.Equal(onTheFlySettings.Old.AddHealthCheckMiddleware, originalSettings.AddHealthCheckMiddleware);

            Assert.NotNull(onTheFlySettings.Current);
            Assert.Equal(onTheFlySettings.Current.HealthCheckIntervalInMinutes, int.Parse(updatedSettings["HealthCheckIntervalInMinutes"]));
            Assert.Equal(onTheFlySettings.Current.HealthCheckIntervalCronExpression, updatedSettings["HealthCheckIntervalCronExpression"]);
            Assert.Equal(onTheFlySettings.Current.HealthCheckServerHubUrl, updatedSettings["HealthCheckServerHubUrl"]);
            Assert.Equal(onTheFlySettings.Current.PublishOnlyWhenNotHealthy, bool.Parse(updatedSettings["PublishOnlyWhenNotHealthy"]));
            Assert.Equal(onTheFlySettings.Current.AddHealthCheckMiddleware, bool.Parse(updatedSettings["AddHealthCheckMiddleware"]));
        }

        private JsonElement CreateJsonElement<T>(T settings)
        {
            string json = JsonSerializer.Serialize(settings);

            using JsonDocument doc = JsonDocument.Parse(json);
            JsonElement element = doc.RootElement;

            return element.Clone();
        }
    }
}
