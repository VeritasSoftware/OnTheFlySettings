# OnTheFlySettings.Client

[![.NET Build & Test](https://github.com/VeritasSoftware/OnTheFlySettings/actions/workflows/dotnet.yml/badge.svg)](https://github.com/VeritasSoftware/OnTheFlySettings/actions/workflows/dotnet.yml)

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
    Task<T> GetSettingsAsync<T>(string route = "/settings",
                                Action<HttpRequestHeaders> addHeaders = null);
    Task<bool> ReplaceSettingsAsync<T>(T payload, 
                                        string route = "/settings/replace",
                                        Action<HttpRequestHeaders> addHeaders = null) 
        where T : class;
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