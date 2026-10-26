// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}

using Azure.Messaging.EventGrid;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using OnTheFlySettings.AzureKeyVault.Client;
using System.Text.Json;

namespace OnTheFlySettings.AzureFunctions
{
    public class OnTheFlySettingsAzureFunction
    {
        private readonly IOnTheFlySettingsClient _onTheFlySettingsClient;
        private readonly SecretClient _secretClient;
        private readonly ILogger<OnTheFlySettingsAzureFunction>? _logger;

        public OnTheFlySettingsAzureFunction(IOnTheFlySettingsClient onTheFlySettingsClient,
                                                SecretClient secretClient,
                                                ILogger<OnTheFlySettingsAzureFunction>? logger = null)
        {
            _onTheFlySettingsClient = onTheFlySettingsClient;
            _secretClient = secretClient;
            _logger = logger;
        }

        [Function(nameof(OnTheFlySettingsAzureFunction))]
        public async Task Run(
                [EventGridTrigger] EventGridEvent eventGridEvent)
        {
            try
            {
                _logger?.LogInformation("Received Event Grid event: {Id}", eventGridEvent.Id);
                _logger?.LogInformation("Event Type: {EventType}", eventGridEvent.EventType);
                _logger?.LogInformation("Subject: {Subject}", eventGridEvent.Subject);

                // Validate event type
                if (!string.Equals(eventGridEvent.EventType,
                                   "Microsoft.KeyVault.SecretNewVersionCreated",
                                   StringComparison.OrdinalIgnoreCase))
                {
                    _logger?.LogWarning("Event type is not SecretNewVersionCreated. Ignoring.");
                    return;
                }

                // Deserialize event data safely
                var jsonData = eventGridEvent.Data.ToString();
                if (string.IsNullOrWhiteSpace(jsonData))
                {
                    _logger?.LogError("Event data is empty.");
                    return;
                }

                // Parse JSON payload
                using var doc = JsonDocument.Parse(jsonData);
                var vaultName = doc.RootElement.GetProperty("VaultName").GetString();
                var objectName = doc.RootElement.GetProperty("ObjectName").GetString();
                var version = doc.RootElement.GetProperty("Version").GetString();

                _logger?.LogInformation("Vault: {VaultName}, Secret: {ObjectName}, Version: {Version}",
                                   vaultName, objectName, version);

                if (string.IsNullOrWhiteSpace(vaultName) || string.IsNullOrWhiteSpace(objectName) || string.IsNullOrWhiteSpace(version))
                {
                    _logger?.LogError("One or more required properties are missing in the event data.");
                    return;
                }

                await ProcessNewSecretVersionAsync(vaultName, objectName, version);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error processing SecretNewVersionCreated event.");
            }
        }

        private async Task ProcessNewSecretVersionAsync(string vaultName, string secretName, string version)
        {
            _logger?.LogInformation("Processing new secret version for {SecretName} in vault {VaultName}", secretName, vaultName);

            var secretResponse = await _secretClient.GetSecretAsync(secretName, version);

            _logger?.LogInformation("Retrieved secret from Key Vault.");

            _logger?.LogInformation("Replacing setting with new secret.");

            var dictionary = new Dictionary<string, string>
            {
                { secretResponse.Value.Name, secretResponse.Value.Value }
            };

            var response = await _onTheFlySettingsClient.ReplaceSettingsAsync(dictionary);

            if (response)
            {
                _logger?.LogInformation("Setting replaced successfully.");
            }
            else
            {
                _logger?.LogWarning("Setting replacement failed.");
            }
        }
    }
}
