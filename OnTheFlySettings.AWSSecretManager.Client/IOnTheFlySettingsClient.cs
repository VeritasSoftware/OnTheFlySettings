using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace OnTheFlySettings.AWSSecretManager.Client
{
    public interface IOnTheFlySettingsClient
    {
        Task<TSettings?> GetAllAWSSecretManagerSecretsAsync<TSettings>(CancellationToken cancellationToken = default)
            where TSettings : class, new();
        Task<TSettings?> GetAllAWSSecretManagerSecretsAsync<TSettings>(string secretId, string region, CancellationToken cancellationToken = default)
            where TSettings : class, new();
        Task<TSettings?> GetSettingsAsync<TSettings>(string route = "/settings",
                                                    Action<HttpRequestHeaders>? addHeaders = null)
            where TSettings : class, new();
        Task<bool> ReplaceSettingsAsync<TSettings>(TSettings newSettings,
                                                   string route = "/settings/replace",
                                                   Action<HttpRequestHeaders>? addHeaders = null)
            where TSettings : class, new();


    }
}