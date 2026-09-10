using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.OnTheFlySettings
{
    public static class Extensions
    {
        public static IOnTheFlySettings<TSettings> AddOnTheFlySettings<TSettings>(this IServiceCollection services, Action<TSettings> configure)
            where TSettings : class, new()
        {
            var settings = new TSettings();

            configure(settings);

            var onTheFlySettings = new OnTheFlySettings<TSettings>(settings);

            services.AddSingleton<IOnTheFlySettings>(onTheFlySettings);
            services.AddSingleton<IOnTheFlySettings<TSettings>>(onTheFlySettings);

            return onTheFlySettings;
        }

        public static RouteHandlerBuilder MapGetOnTheFlySettings(this WebApplication app)
        {
            var routeHandler = app.MapGet("/settings", (IOnTheFlySettings settingsHolder) => { 
                return settingsHolder.CurrentObject;
            });

            return routeHandler;
        }

        public static RouteHandlerBuilder MapGetOnTheFlySettings(this WebApplication app, string route)
        {
            var routeHandler = app.MapGet(route, (IOnTheFlySettings settingsHolder) => {
                return settingsHolder.CurrentObject;
            });

            return routeHandler;
        }

        public static RouteHandlerBuilder MapPutReplaceOnTheFlySettings(this WebApplication app)
        {
            var routeHandler = app.MapPut("/settings/replace", (object newSettings, IOnTheFlySettings settingsHolder) => {                
                settingsHolder.Replace(newSettings);
                return Results.Ok("Settings replaced");
            });

            return routeHandler;
        }

        public static RouteHandlerBuilder MapPutReplaceOnTheFlySettings(this WebApplication app, string route)
        {
            var routeHandler = app.MapPut(route, (object newSettings, IOnTheFlySettings settingsHolder) => {
                settingsHolder.Replace(newSettings);
                return Results.Ok("Settings replaced");
            });

            return routeHandler;
        }
    }
}
