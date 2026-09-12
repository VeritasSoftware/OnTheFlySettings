namespace OnTheFlySettings
{
    public static class Extensions
    {
        public static IServiceCollection AddOnTheFlySettings<TSettings>(this IServiceCollection services, TSettings settings)
            where TSettings : class, new()
        {
            return services.AddOnTheFlySettingsInternal(settings);
        }

        public static IServiceCollection AddOnTheFlySettings<TSettings>(this IServiceCollection services, Action<TSettings> configure)
            where TSettings : class, new()
        {
            var settings = new TSettings();

            configure(settings);

            return services.AddOnTheFlySettingsInternal(settings);
        }

        private static IServiceCollection AddOnTheFlySettingsInternal<TSettings>(this IServiceCollection services, TSettings settings)
            where TSettings : class, new()
        {
            services.AddSingleton(settings);

            var onTheFlySettings = new OnTheFlySettings<TSettings>(settings);

            services.AddSingleton<IOnTheFlySettings>(sp => {
                var factory = sp.GetService<ILoggerFactory>();
                onTheFlySettings.Logger = factory?.CreateLogger(nameof(OnTheFlySettings<TSettings>));
                return onTheFlySettings;
            });
            services.AddSingleton<IOnTheFlySettings<TSettings>>(sp => {
                var factory = sp.GetService<ILoggerFactory>();
                onTheFlySettings.Logger = factory?.CreateLogger(nameof(OnTheFlySettings<TSettings>));
                return onTheFlySettings;
            });

            return services;
        }

        public static RouteHandlerBuilder MapGetOnTheFlySettings(this WebApplication app, string? route = null)
        {
            var routeHandler = app.MapGet(route ?? "/settings", (IOnTheFlySettings settingsHolder) => {
                return settingsHolder.CurrentObject;
            });

            return routeHandler;
        }

        public static RouteHandlerBuilder MapPutReplaceOnTheFlySettings(this WebApplication app, string? route = null)
        {
            var routeHandler = app.MapPut(route ?? "/settings/replace", (object newSettings, IOnTheFlySettings settingsHolder) => {
                settingsHolder.Replace(newSettings);
                return Results.Ok("Settings replaced");
            });

            return routeHandler;
        }
    }
}
