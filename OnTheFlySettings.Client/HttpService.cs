using System.Net.Http;

namespace OnTheFlySettings.Client
{
    public interface IHttpService
    {
        HttpClient Client { get; }
    }

    public class HttpService : IHttpService
    {
        public HttpClient Client { get; private set; }
        public HttpService(HttpClient client)
        {
            Client = client;
        }
    }
}
