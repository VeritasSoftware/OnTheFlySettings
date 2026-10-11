using Amazon.Runtime;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using OnTheFlySettings;
using OnTheFlySettings.Demo;
using System.Text.Json;

Console.WriteLine("On The Fly Settings Demo!");

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
       .SetBasePath(Directory.GetCurrentDirectory())
       .AddJsonFile("appsettings.json", false, true);

var appBasicSettings = builder.Configuration.GetSection("MyHealthCheckBasicSettings")
                                            .Get<MyHealthCheckBasicSettings>();

if (appBasicSettings == null)
    throw new ApplicationException("Settings not found.");

#region Azure

//string keyVaultName = "ontheflysettings";
//var kvUri = "https://" + keyVaultName + ".vault.azure.net";

//var tenantId = Environment.GetEnvironmentVariable("AZURE_TENANT_ID");
//var clientId = Environment.GetEnvironmentVariable("AZURE_CLIENT_ID");
//var clientSecret = Environment.GetEnvironmentVariable("AZURE_CLIENT_SECRET");

//var credential = new ClientSecretCredential(tenantId,
//                                            clientId,
//                                            clientSecret);

//var client = new SecretClient(new Uri(kvUri), credential);

//var appBasicSettings = new MyHealthCheckBasicSettings
//{
//    HealthCheckIntervalInMinutes = int.Parse((await client.GetSecretAsync("HealthCheckIntervalInMinutes")).Value.Value),
//    HealthCheckIntervalCronExpression = (await client.GetSecretAsync("HealthCheckIntervalCronExpression")).Value.Value,
//    HealthCheckServerHubUrl = (await client.GetSecretAsync("HealthCheckServerHubUrl")).Value.Value,
//    PublishOnlyWhenNotHealthy = bool.Parse((await client.GetSecretAsync("PublishOnlyWhenNotHealthy")).Value.Value),
//    AddHealthCheckMiddleware = bool.Parse((await client.GetSecretAsync("AddHealthCheckMiddleware")).Value.Value)
//};

#endregion

#region AWS

//var accessKeyId = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID");
//var secretAccessKey = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY");

//var credentials = new BasicAWSCredentials(accessKeyId, secretAccessKey);
//var client = new AmazonSecretsManagerClient(credentials, Amazon.RegionEndpoint.APSoutheast2);

//var secretRequest = new GetSecretValueRequest
//{
//    SecretId = "MyHealthCheckBasicSettings",
//    VersionStage = "AWSCURRENT"
//};

//var secretResponse = await client.GetSecretValueAsync(secretRequest);

//var options = new JsonSerializerOptions
//{
//    RespectNullableAnnotations = true
//};

//var appBasicSettingsDictionary = JsonSerializer.Deserialize<IDictionary<string, string>>(secretResponse.SecretString);

//if (appBasicSettingsDictionary == null)
//    throw new ApplicationException("Settings not found");

//var appBasicSettings = new MyHealthCheckBasicSettings
//{
//    HealthCheckIntervalInMinutes = int.Parse(appBasicSettingsDictionary["HealthCheckIntervalInMinutes"]),
//    HealthCheckIntervalCronExpression = appBasicSettingsDictionary["HealthCheckIntervalCronExpression"],
//    HealthCheckServerHubUrl = appBasicSettingsDictionary["HealthCheckServerHubUrl"],
//    PublishOnlyWhenNotHealthy = bool.Parse(appBasicSettingsDictionary["PublishOnlyWhenNotHealthy"]),
//    AddHealthCheckMiddleware = bool.Parse(appBasicSettingsDictionary["AddHealthCheckMiddleware"])
//};

#endregion

// Add the settings to the OnTheFlySettings framework
builder.Services.AddOnTheFlySettings(appBasicSettings);

builder.Services.AddSingleton<IDemoService, DemoService>();
builder.Services.AddScoped<IOptionsDemoService, OptionsDemoService>();

var app = builder.Build();

app.MapGetOnTheFlySettings();
app.MapPutReplaceOnTheFlySettings();
//app.MapAzurePutReplaceOnTheFlySettings();
//app.MapAWSPutReplaceOnTheFlySettings();

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
