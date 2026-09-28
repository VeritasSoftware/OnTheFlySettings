# OnTheFlySettings.AzureKeyVault.Client

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
using OnTheFlySettings.AzureKeyVault.Client;
```

```csharp
services.AddOnTheFlySettingsClient(settings =>
{
    settings.BaseUrl = "https://localhost:7277";
    settings.TimeoutMilliseconds = 1000;

    settings.Azure.KeyVaultUrl = "<<your key vault url here>>";

    settings.Azure.UseManagedIdentity = true;
    // OR
    settings.Azure.Credentials.TenantId = "<<your tenantId here>>";
    settings.Azure.Credentials.ClientId = "<<your clientId here>>";
    settings.Azure.Credentials.ClientSecret = "<<your clientSecret here>>";
});
```

The library provides an `IOnTheFlySettingsClient` interface.

You can provide your headers (including Auth) if needed.

```csharp
public interface IOnTheFlySettingsClient
{
    Task<IDictionary<string, string>> GetAllAzureKeyVaultSecretsAsync(CancellationToken cancellationToken = default);
    Task<IDictionary<string, string>> GetAllAzureKeyVaultSecretsAsync(string keyVaultUrl, CancellationToken cancellationToken = default);
    Task<TSettings> GetSettingsAsync<TSettings>(string route = "/settings",
                                                Action<HttpRequestHeaders> addHeaders = null)
        where TSettings : class, new();
    Task<bool> ReplaceSettingsAsync(IDictionary<string, string> newSettings,
                                    string route = "/settings/azure/replace",
                                    Action<HttpRequestHeaders> addHeaders = null);
                                        
}
```

You can inject interface and call the `GetAllAzureKeyVaultSecretsAsync`, `GetSettingsAsync` & `ReplaceSettingsAsync` methods:

**Get all Azure Key Vault secrets:**

If you have provided the Key Vault Url in the settings, you can do this:

```csharp
var client = _serviceProvider.GetRequiredService<IOnTheFlySettingsClient>();

var azureSecrets = await client.GetAllAzureKeyVaultSecretsAsync();
```

OR

you can use the overload & provide the Key Vault Url:

```csharp
var client = _serviceProvider.GetRequiredService<IOnTheFlySettingsClient>();

string keyVaultUrl = "<<your key vault url here>>";

var azureSecrets = await client.GetAllAzureKeyVaultSecretsAsync(keyVaultUrl);
```

**Get current settings from API:**

```csharp
var client = _serviceProvider.GetRequiredService<IOnTheFlySettingsClient>();

var response = await client.GetSettingsAsync<MyHealthCheckBasicSettings>();
```

**Update settings in API:**

You can update the settings in `azureSecrets` returned.

and you can filter `azureSecrets` to contain only the settings you want to update.

```csharp
var client = _serviceProvider.GetRequiredService<IOnTheFlySettingsClient>();

var response = await client.ReplaceSettingsAsync(azureSecrets);
```

### Documentation

[Documentation](https://github.com/VeritasSoftware/OnTheFlySettings)

[Sample usage](https://github.com/VeritasSoftware/OnTheFlySettings/blob/master/OnTheFlySettings.Tests/OnTheFlySettingsAzureClientTests.cs)