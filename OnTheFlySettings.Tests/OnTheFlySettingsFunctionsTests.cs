using Azure;
using Azure.Messaging.EventGrid;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Logging;
using Moq;
using OnTheFlySettings.AzureKeyVault.Client;

namespace OnTheFlySettings.Tests
{
    public class OnTheFlySettingsFunctionsTests
    {
        [Fact]
        public async Task OnTheFlySettingsAzureFunction_Run_Pass()
        {
            // Arrange
            var secretName = "HealthCheckIntervalCronExpression";
            var secretVersion = Guid.NewGuid().ToString();

            var putPayload = new Dictionary<string, string> { { secretName, "*/30 * * * *" } };

            var secretClientMock = new Mock<SecretClient>();
            secretClientMock.Setup(x => x.GetSecretAsync(secretName, secretVersion))
                            .ReturnsAsync(Response.FromValue(new KeyVaultSecret(secretName, "*/30 * * * *"), null));

            var onTheFlySettingsClientMock = new Mock<IOnTheFlySettingsClient>();
            onTheFlySettingsClientMock.Setup(x => x.ReplaceSettingsAsync(putPayload))
                                      .ReturnsAsync(true);

            var loggerMock = new Mock<ILogger<AzureFunctions.OnTheFlySettingsAzureFunction>>();

            var function = new AzureFunctions.OnTheFlySettingsAzureFunction(
                onTheFlySettingsClientMock.Object,
                secretClientMock.Object,
                loggerMock.Object
            );            

            var payload = new
            {
                VaultName = "test-vault",
                ObjectName = secretName,
                Version = secretVersion
            };
            var binaryData = BinaryData.FromObjectAsJson(payload);

            var evt = new EventGridEvent(secretName,
                                            "Microsoft.KeyVault.SecretNewVersionCreated",
                                            "1.0",
                                            binaryData);

            // Act
            await function.Run(evt);

            // Assert
            secretClientMock.Verify(x => x.GetSecretAsync(secretName, secretVersion), Times.Once);

            onTheFlySettingsClientMock.Verify(x => x.ReplaceSettingsAsync(putPayload), Times.Once);           
        }

        [Fact]
        public async Task OnTheFlySettingsAzureFunction_Run_InvalidEventType_Fail()
        {
            // Arrange
            var secretName = "HealthCheckIntervalCronExpression";
            var secretVersion = Guid.NewGuid().ToString();

            var putPayload = new Dictionary<string, string> { { secretName, "*/30 * * * *" } };

            var secretClientMock = new Mock<SecretClient>();
            secretClientMock.Setup(x => x.GetSecretAsync(secretName, secretVersion))
                            .ReturnsAsync(Response.FromValue(new KeyVaultSecret(secretName, "*/30 * * * *"), null));

            var onTheFlySettingsClientMock = new Mock<IOnTheFlySettingsClient>();
            onTheFlySettingsClientMock.Setup(x => x.ReplaceSettingsAsync(putPayload))
                                      .ReturnsAsync(true);

            var loggerMock = new Mock<ILogger<AzureFunctions.OnTheFlySettingsAzureFunction>>();

            var function = new AzureFunctions.OnTheFlySettingsAzureFunction(
                onTheFlySettingsClientMock.Object,
                secretClientMock.Object,
                loggerMock.Object
            );

            var payload = new
            {
                VaultName = "test-vault",
                ObjectName = secretName,
                Version = secretVersion
            };
            var binaryData = BinaryData.FromObjectAsJson(payload);

            var evt = new EventGridEvent(secretName,
                                            "Microsoft.KeyVault.SecretNearExpiry",
                                            "1.0",
                                            binaryData);

            // Act
            await function.Run(evt);

            // Assert
            secretClientMock.Verify(x => x.GetSecretAsync(secretName, secretVersion), Times.Never);

            onTheFlySettingsClientMock.Verify(x => x.ReplaceSettingsAsync(putPayload), Times.Never);
        }
    }
}
