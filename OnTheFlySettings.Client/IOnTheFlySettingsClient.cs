using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace OnTheFlySettings.Client
{
    public interface IOnTheFlySettingsClient
    {
        Task<TSettings> GetSettingsAsync<TSettings>(string route = "/settings",
                                                    Action<HttpRequestHeaders> addHeaders = null)
            where TSettings : class, new();
        Task<bool> ReplaceSettingsAsync<TSettings>(TSettings newSettings, 
                                                    string route = "/settings/replace",
                                                    Action<HttpRequestHeaders> addHeaders = null) 
            where TSettings : class, new();
    }
}