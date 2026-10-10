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
        public void Replace_Map()
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

            var updatedSettings = new
            {
                IntervalInMinutes = "30",
                IntervalCronExpression = "*/5 * * * *",
                ServerHubUrl = "https://localhost:5001/newlivehealthcheckshub",
                OnlyWhenNotHealthy = "true",
                AddMiddleware = "true"
            };

            var jsonElement = CreateJsonElement(updatedSettings);

            onTheFlySettings.Map = (dict, settings) =>
            {
                settings.HealthCheckIntervalInMinutes = int.Parse(dict["IntervalInMinutes"]);
                settings.HealthCheckIntervalCronExpression = dict["IntervalCronExpression"];
                settings.HealthCheckServerHubUrl = dict["ServerHubUrl"];
                settings.PublishOnlyWhenNotHealthy = bool.Parse(dict["OnlyWhenNotHealthy"]);
                settings.AddHealthCheckMiddleware = bool.Parse(dict["AddMiddleware"]);
            };

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
            Assert.Equal(onTheFlySettings.Current.HealthCheckIntervalInMinutes, int.Parse(updatedSettings.IntervalInMinutes));
            Assert.Equal(onTheFlySettings.Current.HealthCheckIntervalCronExpression, updatedSettings.IntervalCronExpression);
            Assert.Equal(onTheFlySettings.Current.HealthCheckServerHubUrl, updatedSettings.ServerHubUrl);
            Assert.Equal(onTheFlySettings.Current.PublishOnlyWhenNotHealthy, bool.Parse(updatedSettings.OnlyWhenNotHealthy));
            Assert.Equal(onTheFlySettings.Current.AddHealthCheckMiddleware, bool.Parse(updatedSettings.AddMiddleware));
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

        [Fact]
        public void ReplaceDictionary_Map()
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

            updatedSettings.Add("IntervalInMinutes", "30");
            updatedSettings.Add("IntervalCronExpression", "*/5 * * * *");
            updatedSettings.Add("ServerHubUrl", "https://localhost:5001/newlivehealthcheckshub");
            updatedSettings.Add("OnlyWhenNotHealthy", "true");
            updatedSettings.Add("AddMiddleware", "true");

            onTheFlySettings.Map = (dict, settings) =>
            {
                settings.HealthCheckIntervalInMinutes = int.Parse(dict["IntervalInMinutes"]);
                settings.HealthCheckIntervalCronExpression = dict["IntervalCronExpression"];
                settings.HealthCheckServerHubUrl = dict["ServerHubUrl"];
                settings.PublishOnlyWhenNotHealthy = bool.Parse(dict["OnlyWhenNotHealthy"]);
                settings.AddHealthCheckMiddleware = bool.Parse(dict["AddMiddleware"]);
            };

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
            Assert.Equal(onTheFlySettings.Current.HealthCheckIntervalInMinutes, int.Parse(updatedSettings["IntervalInMinutes"]));
            Assert.Equal(onTheFlySettings.Current.HealthCheckIntervalCronExpression, updatedSettings["IntervalCronExpression"]);
            Assert.Equal(onTheFlySettings.Current.HealthCheckServerHubUrl, updatedSettings["ServerHubUrl"]);
            Assert.Equal(onTheFlySettings.Current.PublishOnlyWhenNotHealthy, bool.Parse(updatedSettings["OnlyWhenNotHealthy"]));
            Assert.Equal(onTheFlySettings.Current.AddHealthCheckMiddleware, bool.Parse(updatedSettings["AddMiddleware"]));
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
