# OnTheFlySettings.Client

[![.NET Build & Test](https://github.com/VeritasSoftware/OnTheFlySettings/actions/workflows/dotnet.yml/badge.svg)](https://github.com/VeritasSoftware/OnTheFlySettings/actions/workflows/dotnet.yml)

|**Packages**|Version|Downloads|
|---------------------------|:---:|:---:|
|*OnTheFlySettings*|[![Nuget Version](https://img.shields.io/nuget/v/OnTheFlySettings)](https://www.nuget.org/packages/OnTheFlySettings)|[![Downloads count](https://img.shields.io/nuget/dt/OnTheFlySettings)](https://www.nuget.org/packages/OnTheFlySettings)|
|*OnTheFlySettings.Client*|[![Nuget Version](https://img.shields.io/nuget/v/OnTheFlySettings.Client)](https://www.nuget.org/packages/OnTheFlySettings.Client)|[![Downloads count](https://img.shields.io/nuget/dt/OnTheFlySettings.Client)](https://www.nuget.org/packages/OnTheFlySettings.Client)|
|*OnTheFlySettings.AzureKeyVault.Client*|[![Nuget Version](https://img.shields.io/nuget/v/OnTheFlySettings.AzureKeyVault.Client)](https://www.nuget.org/packages/OnTheFlySettings.AzureKeyVault.Client)|[![Downloads count](https://img.shields.io/nuget/dt/OnTheFlySettings.AzureKeyVault.Client)](https://www.nuget.org/packages/OnTheFlySettings.AzureKeyVault.Client)|

![On The Fly Settings](https://raw.githubusercontent.com/VeritasSoftware/OnTheFlySettings/master/Images/OnTheFlySettings.jpg)

The .NET Client library allows you to interact with the OnTheFlySettings GET & PUT endpoints.

You add the library to your project in your `Program.cs` or `Startup.cs`:

```csharp
using OnTheFlySettings.Client;
```

```csharp
services.AddOnTheFlySettingsClient(settings =>
{
    settings.BaseUrl = "https://localhost:7277";
    settings.TimeoutMilliseconds = 1000;
});
```

The library provides an `IOnTheFlySettingsClient` interface.

You can provide your headers (including Auth) if needed.

```csharp
public interface IOnTheFlySettingsClient
{
    Task<TSettings> GetSettingsAsync<TSettings>(string route = "/settings",
                                                Action<HttpRequestHeaders> addHeaders = null)
        where TSettings : class, new();
    Task<bool> ReplaceSettingsAsync<TSettings>(TSettings newSettings, 
                                                string route = "/settings/replace",
                                                Action<HttpRequestHeaders> addHeaders = null) 
        where TSettings : class, new();
}
```

You can inject interface and call the `GetSettingsAsync` & `ReplaceSettingsAsync` methods:

```csharp
var client = _serviceProvider.GetRequiredService<IOnTheFlySettingsClient>();

var response = await client.GetSettingsAsync<MyHealthCheckBasicSettings>();
```

```csharp
var client = _serviceProvider.GetRequiredService<IOnTheFlySettingsClient>();

var newSettings = new MyHealthCheckBasicSettings
{
    HealthCheckIntervalInMinutes = 30,
    HealthCheckIntervalCronExpression = "*/5 * * * *",
    HealthCheckServerHubUrl = "https://localhost:5001/newlivehealthcheckshub",
    PublishOnlyWhenNotHealthy = true,
    AddHealthCheckMiddleware = true
};

var response = await client.ReplaceSettingsAsync(newSettings);
```

### Documentation

[Documentation](https://github.com/VeritasSoftware/OnTheFlySettings)

[Sample usage](https://github.com/VeritasSoftware/OnTheFlySettings/blob/master/OnTheFlySettings.Tests/OnTheFlySettingsClientTests.cs)