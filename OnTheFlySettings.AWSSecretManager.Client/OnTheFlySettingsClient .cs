
using Amazon;
using Amazon.Runtime;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using System;
using System.Collections.Generic;
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

        private async Task<IDictionary<string, string>> GetAllAWSSecretManagerSecretsInternalAsync(string secretId, RegionEndpoint region, 
                                                                                                CancellationToken cancellationToken = default)
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

            var settings = JsonSerializer.Deserialize<IDictionary<string, string>>(secretResponse.SecretString, options);

            if (settings == null)
            {
                return new Dictionary<string, string>();
            }

            return settings;
        }

        public async Task<IDictionary<string, string>> GetAllAWSSecretManagerSecretsAsync(CancellationToken cancellationToken = default)
        {
            return await GetAllAWSSecretManagerSecretsInternalAsync(_clientSettings.AWS.SecretId, RegionEndpoint.GetBySystemName(_clientSettings.AWS.Region), cancellationToken);
        }

        public async Task<IDictionary<string, string>> GetAllAWSSecretManagerSecretsAsync(string secretId, string region, CancellationToken cancellationToken = default)
        {
            return await GetAllAWSSecretManagerSecretsInternalAsync(secretId, RegionEndpoint.GetBySystemName(region), cancellationToken);
        }

        public async Task<TSettings> GetSettingsAsync<TSettings>(string route = "/settings",
                                                                 Action<HttpRequestHeaders> addHeaders = null)
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
        public async Task<bool> ReplaceSettingsAsync(IDictionary<string, string> newSettings,
                                                    string route = "/settings/aws/replace",
                                                    Action<HttpRequestHeaders> addHeaders = null)
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