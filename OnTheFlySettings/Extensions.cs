namespace OnTheFlySettings
{
    public static class Extensions
    {
        public static IServiceCollection AddOnTheFlySettings<TSettings>(this IServiceCollection services, TSettings settings, Action<IDictionary<string, string>, TSettings>? map = null)
            where TSettings : class, new()
        {
            return services.AddOnTheFlySettingsInternal(settings, map);
        }

        public static IServiceCollection AddOnTheFlySettings<TSettings>(this IServiceCollection services, Action<TSettings> configure, Action<IDictionary<string, string>, TSettings>? map = null)
            where TSettings : class, new()
        {
            var settings = new TSettings();

            configure(settings);

            return services.AddOnTheFlySettingsInternal(settings, map);
        }

        private static IServiceCollection AddOnTheFlySettingsInternal<TSettings>(this IServiceCollection services, TSettings settings, Action<IDictionary<string, string>, TSettings>? map = null)
            where TSettings : class, new()
        {
            services.AddSingleton(settings);

            var onTheFlySettings = new OnTheFlySettings<TSettings>(settings);
            onTheFlySettings.Map = map;

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

        public static RouteHandlerBuilder MapGetOnTheFlySettings(this WebApplication app, string route = "/settings")
        {
            var routeHandler = app.MapGet(route, (IOnTheFlySettings settingsHolder) => {
                return settingsHolder.CurrentObject;
            });

            return routeHandler;
        }

        public static RouteHandlerBuilder MapPutReplaceOnTheFlySettings(this WebApplication app, string route = "/settings/replace")
        {
            var routeHandler = app.MapPut(route, (object newSettings, IOnTheFlySettings settingsHolder) => {
                settingsHolder.Replace(newSettings);
                return Results.Ok("Settings replaced");
            });

            return routeHandler;
        }

        public static RouteHandlerBuilder MapAzurePutReplaceOnTheFlySettings(this WebApplication app, string route = "/settings/azure/replace")
        {
            var routeHandler = app.MapPut(route, (IDictionary<string, string> newSettings, IOnTheFlySettings settingsHolder) => {
                settingsHolder.Replace(newSettings);
                return Results.Ok("Settings replaced");
            });

            return routeHandler;
        }

        public static RouteHandlerBuilder MapAWSPutReplaceOnTheFlySettings(this WebApplication app, string route = "/settings/aws/replace")
        {
            var routeHandler = app.MapPut(route, (IDictionary<string, string> newSettings, IOnTheFlySettings settingsHolder) => {
                settingsHolder.Replace(newSettings);
                return Results.Ok("Settings replaced");
            });

            return routeHandler;
        }
    }
}
