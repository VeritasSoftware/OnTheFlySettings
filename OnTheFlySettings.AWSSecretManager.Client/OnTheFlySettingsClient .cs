
using Amazon;
using Amazon.Runtime;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OnTheFlySettings.AWSSecretManager.Client
{
    public class OnTheFlySettingsClient : IOnTheFlySettingsClient
    {
        private readonly HttpClient _httpClient;
        private readonly ClientSettings _clientSettings;

        public OnTheFlySettingsClient(IHttpService httpService, ClientSettings clientSettings)
        {
            var httpClient = httpService.Client;
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _clientSettings = clientSettings ?? throw new ArgumentNullException(nameof(clientSettings));
        }

        private async Task<TSettings?> GetAllAWSSecretManagerSecretsInternalAsync<TSettings>(string secretId, RegionEndpoint region, 
                                                                                                CancellationToken cancellationToken = default)
            where TSettings: class, new()
        {
            AmazonSecretsManagerClient client;

            if (_clientSettings.AWS.UseDefaultAWSCredentialChain)
            {
                client = new AmazonSecretsManagerClient(region);
            }
            else
            {
                var credentials = new BasicAWSCredentials(_clientSettings.AWS.Credentials.AccessKeyId, _clientSettings.AWS.Credentials.SecretAccessKey);
                client = new AmazonSecretsManagerClient(credentials, region);
            }

            var secretRequest = new GetSecretValueRequest
            {
                SecretId = secretId,
                VersionStage = "AWSCURRENT"
            };

            var secretResponse = await client.GetSecretValueAsync(secretRequest);

            var options = new JsonSerializerOptions
            {
                 PropertyNameCaseInsensitive = true
            };
            options.Converters.Add(new UniversalPrimitiveConverterFactory());

            var settings = JsonSerializer.Deserialize<TSettings>(secretResponse.SecretString, options);

            return settings;
        }

        public async Task<TSettings?> GetAllAWSSecretManagerSecretsAsync<TSettings>(CancellationToken cancellationToken = default)
            where TSettings : class, new()
        {
            return await GetAllAWSSecretManagerSecretsInternalAsync<TSettings>(_clientSettings.AWS.SecretId, RegionEndpoint.GetBySystemName(_clientSettings.AWS.Region), cancellationToken);
        }

        public async Task<TSettings?> GetAllAWSSecretManagerSecretsAsync<TSettings>(string secretId, string region, CancellationToken cancellationToken = default)
            where TSettings : class, new()
        {
            return await GetAllAWSSecretManagerSecretsInternalAsync<TSettings>(secretId, RegionEndpoint.GetBySystemName(region), cancellationToken);
        }

        public async Task<TSettings?> GetSettingsAsync<TSettings>(string route = "/settings",
                                                                 Action<HttpRequestHeaders>? addHeaders = null)
            where TSettings: class, new()
        {
            var request = new HttpRequestMessage(HttpMethod.Get,  route);

            // Add Authorization or other headers if needed
            addHeaders?.Invoke(request.Headers);

            HttpResponseMessage response;

            try
            {
                response = await _httpClient.SendAsync(request);
            }
            catch (TaskCanceledException ex)
            {
                throw new HttpRequestException("Request timed out.", ex);
            }
            catch (HttpRequestException ex)
            {
                throw new HttpRequestException("Error sending GET request.", ex);
            }

            response.EnsureSuccessStatusCode();

            var responseStr = await response.Content.ReadAsStringAsync();

            if (responseStr == null)
                throw new ApplicationException("Could not get settings.");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            return JsonSerializer.Deserialize<TSettings>(responseStr, options);
        }

        /// <summary>
        /// Sends a PUT request to /settings/replace with a generic newSettings.
        /// </summary>
        public async Task<bool> ReplaceSettingsAsync<TSettings>(TSettings newSettings,
                                                                string route = "/settings/replace",
                                                                Action<HttpRequestHeaders>? addHeaders = null)
            where TSettings : class, new()
        {
            if (newSettings == null)
                throw new ArgumentNullException(nameof(newSettings));

            string json = JsonSerializer.Serialize(newSettings, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Create request
            var request = new HttpRequestMessage(HttpMethod.Put, route)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            // Add Authorization or other headers if needed
            addHeaders?.Invoke(request.Headers);

            HttpResponseMessage response;

            try
            {
                response = await _httpClient.SendAsync(request);
            }
            catch (TaskCanceledException ex)
            {
                throw new HttpRequestException("Request timed out.", ex);
            }
            catch (HttpRequestException ex)
            {
                throw new HttpRequestException("Error sending PUT request.", ex);
            }

            response.EnsureSuccessStatusCode();

            var responseStr = await response.Content.ReadAsStringAsync();

            if (responseStr == null)
                throw new ApplicationException("Settings not replaced.");

            if (string.Compare(responseStr, "\"Settings replaced\"", true) == 0)
            {
                return true;
            }

            return false;
        }
    }
}