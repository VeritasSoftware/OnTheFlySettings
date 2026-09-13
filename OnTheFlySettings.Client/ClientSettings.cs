namespace OnTheFlySettings.Client
{
    public class ClientSettings
    {
        public string BaseUrl { get; set; }
        public int TimeoutMilliseconds { get; set; } = 5000;
        public AuthSettings AuthSettings { get; set; } = null;
    }

    public class AuthSettings
    {
        public string AuthScheme { get; set; }
        public string AuthToken { get; set; }
    }
}
