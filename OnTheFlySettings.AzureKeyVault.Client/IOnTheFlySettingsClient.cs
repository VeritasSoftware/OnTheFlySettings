using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace OnTheFlySettings.AzureKeyVault.Client
{
    public interface IOnTheFlySettingsClient
    {
        Task<IEnumerable<AzureSecret>> GetAllAzureSecretsAsync(CancellationToken cancellationToken = default);
        Task<TSettings> GetSettingsAsync<TSettings>(string route = "/settings",
                                                    Action<HttpRequestHeaders> addHeaders = null);
        Task<bool> ReplaceSettingsAsync(IEnumerable<AzureSecret> newSettings,
                                        string route = "/settings/replace",
                                        Action<HttpRequestHeaders> addHeaders = null);
                                        
    }
}