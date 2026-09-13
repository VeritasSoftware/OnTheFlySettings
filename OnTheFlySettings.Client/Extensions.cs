using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http.Headers;

namespace OnTheFlySettings.Client
{
    public static class Extensions
    {
        public static IServiceCollection AddOnTheFlySettingsClient(this IServiceCollection services,
                                                                    Action<ClientSettings> configure)
        {
            var settings = new ClientSettings();

            configure(settings);

            if (string.IsNullOrEmpty(settings.BaseUrl)) 
            {
                throw new ArgumentException("BaseUrl not provided.");
            }

            services.AddSingleton(settings);

            services.AddHttpClient<IHttpService, HttpService>(client =>
            {
                client.BaseAddress = new Uri(settings.BaseUrl);
                client.Timeout = TimeSpan.FromMilliseconds(settings.TimeoutMilliseconds);

                if (settings.AuthSettings != null
                        & !string.IsNullOrEmpty(settings.AuthSettings.AuthScheme)
                        & !string.IsNullOrEmpty(settings.AuthSettings.AuthToken))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                                                                                                settings.AuthSettings.AuthScheme, 
                                                                                                settings.AuthSettings.AuthToken
                                                                                              );
                }                
            });

            services.AddScoped<IOnTheFlySettingsClient, OnTheFlySettingsClient>();

            return services;
        }
    }
}
