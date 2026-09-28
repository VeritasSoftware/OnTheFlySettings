namespace OnTheFlySettings.AzureKeyVault.Client
{
    public class ClientSettings
    {
        public string BaseUrl { get; set; }
        public int TimeoutMilliseconds { get; set; } = 5000;
        public Azure Azure { get; set; } = new Azure();
    }

    public class Azure
    {
        public string KeyVaultUrl { get; set; }
        public bool UseManagedIdentity { get; set; }
        public Credentials Credentials { get; set; } = new Credentials();
    }

    public class Credentials
    {
        public string TenantId { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }    
}
