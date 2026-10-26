using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.DependencyInjection;
using OnTheFlySettings.AzureKeyVault.Client;

namespace OnTheFlySettings.AzureFunctions
{
    public static class Extensions
    {
        public static IServiceCollection AddOnTheFlySettingsFunctionsClient(this IServiceCollection services,
                                                                    Action<ClientSettings> configure)
        {
            var settings = new ClientSettings();

            configure(settings);

            services.AddOnTheFlySettingsClient(configure);

            SecretClient keyVaultClient;

            if (settings.Azure.UseManagedIdentity)
            {
                keyVaultClient = new SecretClient(new Uri(settings.Azure.KeyVaultUrl), new DefaultAzureCredential());
            }
            else
            {
                var credential = new ClientSecretCredential(settings.Azure.Credentials.TenantId,
                                                        settings.Azure.Credentials.ClientId,
                                                        settings.Azure.Credentials.ClientSecret);

                keyVaultClient = new SecretClient(new Uri(settings.Azure.KeyVaultUrl), credential);
            }

            services.AddSingleton(keyVaultClient);

            return services;
        }
    }
}
