using OnTheFlySettings;
using OnTheFlySettings.Demo;

Console.WriteLine("On The Fly Settings Demo!");

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
       .SetBasePath(Directory.GetCurrentDirectory())
       .AddJsonFile("appsettings.json", false, true);

var appBasicSettings = builder.Configuration.GetSection("MyHealthCheckBasicSettings")
                                            .Get<MyHealthCheckBasicSettings>();

if (appBasicSettings == null)
    throw new ApplicationException("Settings not found.");

// Add the settings to the OnTheFlySettings framework
builder.Services.AddOnTheFlySettings(appBasicSettings);

builder.Services.AddSingleton<IDemoService, DemoService>();
builder.Services.AddScoped<IOptionsDemoService, OptionsDemoService>();

var app = builder.Build();

app.MapGetOnTheFlySettings();
app.MapPutReplaceOnTheFlySettings();

app.MapGet("/print-settings", (IDemoService demoService) =>
{
    demoService.PrintCurrentSettings();
    return Results.Ok("Settings printed to console.");
});

app.MapGet("/print-options-settings", (IOptionsDemoService optionsDemoService) =>
{
    optionsDemoService.PrintCurrentSettings();
    return Results.Ok("Options settings printed to console.");
});

await app.RunAsync();
