using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace OnTheFlySettings.AzureKeyVault.Client
{
    public interface IOnTheFlySettingsClient
    {
        Task<IDictionary<string, string>> GetAllAzureKeyVaultSecretsAsync(CancellationToken cancellationToken = default);
        Task<IDictionary<string, string>> GetAllAzureKeyVaultSecretsAsync(string keyVaultUrl, CancellationToken cancellationToken = default);
        Task<TSettings> GetSettingsAsync<TSettings>(string route = "/settings",
                                                    Action<HttpRequestHeaders> addHeaders = null);
        Task<bool> ReplaceSettingsAsync(IDictionary<string, string> newSettings,
                                        string route = "/settings/azure/replace",
                                        Action<HttpRequestHeaders> addHeaders = null);
                                        
    }
}