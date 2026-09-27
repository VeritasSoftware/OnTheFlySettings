using Azure;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OnTheFlySettings.AzureKeyVault.Client
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

        public async Task<IEnumerable<AzureSecret>> GetAllAzureSecretsAsync(CancellationToken cancellationToken = default)
        {
            var credential = new ClientSecretCredential(_clientSettings.Azure.Credentials.TenantId,
                                                        _clientSettings.Azure.Credentials.ClientId,
                                                        _clientSettings.Azure.Credentials.ClientSecret);

            var keyVaultClient = new SecretClient(new Uri(_clientSettings.Azure.KeyVaultUrl), credential);

            AsyncPageable<SecretProperties> secretProperties = keyVaultClient.GetPropertiesOfSecretsAsync(cancellationToken);

            var secrets = new List<KeyVaultSecret>();

            await foreach (var secretProperty in secretProperties)
            {
                var response = await keyVaultClient.GetSecretAsync(secretProperty.Name, cancellationToken: cancellationToken).ConfigureAwait(false);

                secrets.Add(response.Value);
            }

            return secrets.Select(secret => new AzureSecret {  Name = secret.Name, Value = secret.Value });
        }

        public async Task<TSettings> GetSettingsAsync<TSettings>(string route = "/settings",
                                                                 Action<HttpRequestHeaders> addHeaders = null)
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
        public async Task<bool> ReplaceSettingsAsync(IEnumerable<AzureSecret> newSettings,
                                                                string route = "/settings/replace",
                                                                Action<HttpRequestHeaders> addHeaders = null)
        {
            if (newSettings == null)
                throw new ArgumentNullException(nameof(newSettings));

            string json = JsonSerializer.Serialize(newSettings.ToDictionary(x => x.Name, x => x.Value),
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
