using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace OnTheFlySettings.Client
{
    public interface IOnTheFlySettingsClient
    {
        Task<T> GetSettingsAsync<T>(string route = "/settings",
                                    Action<HttpRequestHeaders> addHeaders = null);
        Task<bool> ReplaceSettingsAsync<T>(T payload, 
                                            string route = "/settings/replace",
                                            Action<HttpRequestHeaders> addHeaders = null) 
            where T : class;
    }
}