
using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using System;
using System.Collections.Generic;
using System.IO;
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

        private async Task<IDictionary<string, string>> GetAllAWSSecretManagerSecretsInternalAsync(RegionEndpoint region, 
                                                                                                CancellationToken cancellationToken = default)
        {
            // Create AWS Secrets Manager client
            // Ensure AWS credentials are configured via environment variables, profile, or IAM role
            var client = new AmazonSecretsManagerClient(_clientSettings.AWS.Credentials.AccessKeyId, _clientSettings.AWS.Credentials.SecretAccessKey, region); // Change region if needed

            Console.WriteLine("Listing all secrets from AWS Secrets Manager...\n");

            string nextToken = null;

            IDictionary<string, string> secrets = new Dictionary<string, string>();

            do
            {
                var request = new ListSecretsRequest
                {
                    MaxResults = 100, // AWS max per page
                    NextToken = nextToken
                };

                var response = await client.ListSecretsAsync(request, cancellationToken);

                foreach (var secret in response.SecretList)
                {
                    var secretRequest = new GetSecretValueRequest
                    {
                        SecretId = secret.Name,
                        VersionStage = "AWSCURRENT"
                    };

                    var secretResponse = await client.GetSecretValueAsync(secretRequest, cancellationToken);

                    string secretString;
                    if (secretResponse.SecretString != null)
                    {
                        secretString = secretResponse.SecretString;
                    }
                    else
                    {
                        var memoryStream = secretResponse.SecretBinary;
                        var reader = new StreamReader(memoryStream);
                        secretString = Encoding.UTF8.GetString(Convert.FromBase64String(reader.ReadToEnd()));
                    }

                    secrets.Add(secret.Name, secretString);
                }

                nextToken = response.NextToken; // Continue if more pages exist

            } while (!string.IsNullOrEmpty(nextToken));

            return secrets;
        }

        public async Task<IDictionary<string, string>> GetAllAWSSecretManagerSecretsAsync(CancellationToken cancellationToken = default)
        {
            return await GetAllAWSSecretManagerSecretsInternalAsync(RegionEndpoint.GetBySystemName(_clientSettings.AWS.Region), cancellationToken);
        }

        public async Task<IDictionary<string, string>> GetAllAWSSecretManagerSecretsAsync(string region, CancellationToken cancellationToken = default)
        {
            return await GetAllAWSSecretManagerSecretsInternalAsync(RegionEndpoint.GetBySystemName(region), cancellationToken);
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
        /// Sends a PUT request to /settings/replace with a list of keyvaultsecrets.
        /// </summary>
        public async Task<bool> ReplaceSettingsAsync(IDictionary<string, string> newSettings,
                                                    string route = "/settings/azure/replace",
                                                    Action<HttpRequestHeaders> addHeaders = null)
        {
            if (newSettings == null)
                throw new ArgumentNullException(nameof(newSettings));

            string json = JsonSerializer.Serialize(newSettings,
            new JsonSerializerOptions
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