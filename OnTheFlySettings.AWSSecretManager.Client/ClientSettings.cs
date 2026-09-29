namespace OnTheFlySettings.AWSSecretManager.Client
{
    public class ClientSettings
    {
        public string BaseUrl { get; set; } = string.Empty;
        public int TimeoutMilliseconds { get; set; } = 5000;
        public AWS AWS { get; set; } = new AWS();
    }

    public class AWS
    {
        public string Region { get; set; } = string.Empty;
        public string SecretId { get; set; } = string.Empty;
        public bool UseDefaultAWSCredentialChain { get; set; }
        public Credentials Credentials { get; set; } = new Credentials();
    }

    public class Credentials
    {
        public string AccessKeyId { get; set; } = string.Empty;
        public string SecretAccessKey { get; set; } = string.Empty;
    }    
}
