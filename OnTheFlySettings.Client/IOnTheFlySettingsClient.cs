using System.Threading.Tasks;

namespace OnTheFlySettings.Client
{
    public interface IOnTheFlySettingsClient
    {
        Task<T> GetSettingsAsync<T>(string route = "/settings");
        Task<bool> ReplaceSettingsAsync<T>(T payload, 
                                            string route = "/settings/replace",
                                            AuthSettings authSettings = null) 
            where T : class;
    }
}