namespace OnTheFlySettings.AzureKeyVault.Client
{
    public class ClientSettings
    {
        public string BaseUrl { get; set; }
        public int TimeoutMilliseconds { get; set; } = 5000;
        public Azure Azure { get; set; }
    }

    public class Azure
    {
        public string KeyVaultUrl { get; set; }
        public AzureCredentials Credentials { get; set; }
    }

    public class AzureCredentials
    {
        public string TenantId { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }

    public class AzureSecret
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
