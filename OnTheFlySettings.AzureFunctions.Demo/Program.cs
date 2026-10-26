using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Hosting;
using OnTheFlySettings.AzureFunctions;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

// Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
// builder.Services
//     .AddApplicationInsightsTelemetryWorkerService()
//     .ConfigureFunctionsApplicationInsights();

builder.Services.AddOnTheFlySettingsFunctionsClient(settings =>
{
    settings.BaseUrl = "https://localhost:7277";
    settings.TimeoutMilliseconds = 1000 * 60 * 5;

    var tenantId = Environment.GetEnvironmentVariable("AZURE_TENANT_ID");
    var clientId = Environment.GetEnvironmentVariable("AZURE_CLIENT_ID");
    var clientSecret = Environment.GetEnvironmentVariable("AZURE_CLIENT_SECRET");

    settings.Azure.KeyVaultUrl = "https://ontheflysettings.vault.azure.net/";

    //settings.Azure.UseManagedIdentity = true;
    // OR                
    settings.Azure.Credentials.TenantId = tenantId;
    settings.Azure.Credentials.ClientId = clientId;
    settings.Azure.Credentials.ClientSecret = clientSecret;
});

builder.Build().Run();