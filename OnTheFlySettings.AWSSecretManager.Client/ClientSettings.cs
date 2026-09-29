namespace OnTheFlySettings.AWSSecretManager.Client
{
    public class ClientSettings
    {
        public string BaseUrl { get; set; }
        public int TimeoutMilliseconds { get; set; } = 5000;
        public AWS AWS { get; set; } = new AWS();
    }

    public class AWS
    {
        public string Region { get; set; }
        public bool UseDefaultAWSCredentialChain { get; set; }
        public Credentials Credentials { get; set; } = new Credentials();
    }

    public class Credentials
    {
        public string AccessKeyId { get; set; }
        public string SecretAccessKey { get; set; }
    }    
}
