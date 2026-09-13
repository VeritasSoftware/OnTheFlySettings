using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OnTheFlySettings.Client
{
    public class OnTheFlySettingsClient : IOnTheFlySettingsClient
    {
        private readonly HttpClient _httpClient;
        private readonly ClientSettings _clientSettings;

        public OnTheFlySettingsClient(IHttpService httpService, ClientSettings settings)
        {
            var httpClient = httpService.Client;
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _clientSettings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public async Task<T> GetSettingsAsync<T>(string route = "/settings")
        {
            using var request = new HttpRequestMessage(HttpMethod.Get,  route);

            // Add Authorization header if provided
            var authSettings = _clientSettings.AuthSettings;

            if (authSettings != null)
            {
                if (!string.IsNullOrWhiteSpace(authSettings.AuthScheme) && !string.IsNullOrWhiteSpace(authSettings.AuthToken))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue(authSettings.AuthScheme, authSettings.AuthToken);
                }
            }

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

            return JsonSerializer.Deserialize<T>(responseStr, options);
        }

        /// <summary>
        /// Sends a PUT request to /settings/replace with a generic payload.
        /// </summary>
        public async Task<bool> ReplaceSettingsAsync<T>(T payload,
                                                        string route = "/settings/replace",
                                                        AuthSettings authSettings = null)
            where T : class
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));

            string json = JsonSerializer.Serialize(payload, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Create request
            using var request = new HttpRequestMessage(HttpMethod.Put, route)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            // Add Authorization header if provided
            var myAuthSettings = authSettings ?? _clientSettings.AuthSettings;

            if (myAuthSettings != null)
            {
                if (!string.IsNullOrWhiteSpace(myAuthSettings.AuthScheme) && !string.IsNullOrWhiteSpace(myAuthSettings.AuthToken))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue(myAuthSettings.AuthScheme, myAuthSettings.AuthToken);
                }
            }

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
