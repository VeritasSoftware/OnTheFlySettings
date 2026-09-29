# OnTheFlySettings.AWSSecretManager.Client

[![.NET Build & Test](https://github.com/VeritasSoftware/OnTheFlySettings/actions/workflows/dotnet.yml/badge.svg)](https://github.com/VeritasSoftware/OnTheFlySettings/actions/workflows/dotnet.yml)

|**Packages**|Version|Downloads|
|---------------------------|:---:|:---:|
|*OnTheFlySettings*|[![Nuget Version](https://img.shields.io/nuget/v/OnTheFlySettings)](https://www.nuget.org/packages/OnTheFlySettings)|[![Downloads count](https://img.shields.io/nuget/dt/OnTheFlySettings)](https://www.nuget.org/packages/OnTheFlySettings)|
|*OnTheFlySettings.Client*|[![Nuget Version](https://img.shields.io/nuget/v/OnTheFlySettings.Client)](https://www.nuget.org/packages/OnTheFlySettings.Client)|[![Downloads count](https://img.shields.io/nuget/dt/OnTheFlySettings.Client)](https://www.nuget.org/packages/OnTheFlySettings.Client)|
|*OnTheFlySettings.AzureKeyVault.Client*|[![Nuget Version](https://img.shields.io/nuget/v/OnTheFlySettings.AzureKeyVault.Client)](https://www.nuget.org/packages/OnTheFlySettings.AzureKeyVault.Client)|[![Downloads count](https://img.shields.io/nuget/dt/OnTheFlySettings.AzureKeyVault.Client)](https://www.nuget.org/packages/OnTheFlySettings.AzureKeyVault.Client)|
|*OnTheFlySettings.AWSSecretManager.Client*|[![Nuget Version](https://img.shields.io/nuget/v/OnTheFlySettings.AWSSecretManager.Client)](https://www.nuget.org/packages/OnTheFlySettings.AWSSecretManager.Client)|[![Downloads count](https://img.shields.io/nuget/dt/OnTheFlySettings.AWSSecretManager.Client)](https://www.nuget.org/packages/OnTheFlySettings.AWSSecretManager.Client)|

![On The Fly Settings](https://raw.githubusercontent.com/VeritasSoftware/OnTheFlySettings/master/Images/OnTheFlySettings.jpg)

The .NET Client library allows you to interact with the OnTheFlySettings GET & PUT endpoints.

Let us say your settings are secrets in AWS Secret Manager.

![AWS Secrets](https://raw.githubusercontent.com/VeritasSoftware/OnTheFlySettings/master/Images/AWSSecretManagerSecrets.png)

You add the library to your project in your `Program.cs` or `Startup.cs`:

```csharp
using OnTheFlySettings.AWSSecretManager.Client;
```

```csharp
services.AddOnTheFlySettingsClient(settings =>
{
    settings.BaseUrl = "https://localhost:7277";
    settings.TimeoutMilliseconds = 1000 * 60 * 5;

    var region = Environment.GetEnvironmentVariable("AWS_REGION");
    var accessKeyId = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID");
    var secretAccessKey = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY");

    settings.AWS.SecretId = "MyHealthCheckBasicSettings";
    settings.AWS.Region = region;

    //settings.AWS.UseDefaultAWSCredentialChain = true;
    // OR
    settings.AWS.Credentials.AccessKeyId = accessKeyId;
    settings.AWS.Credentials.SecretAccessKey = secretAccessKey;
});
```

The library provides an `IOnTheFlySettingsClient` interface.

You can provide your headers (including Auth) if needed.

```csharp
public interface IOnTheFlySettingsClient
{
    Task<IDictionary<string, string>> GetAllAWSSecretManagerSecretsAsync(CancellationToken cancellationToken = default);
    Task<IDictionary<string, string>> GetAllAWSSecretManagerSecretsAsync(string secretId, string region, CancellationToken cancellationToken = default);
    Task<TSettings> GetSettingsAsync<TSettings>(string route = "/settings",
                                                Action<HttpRequestHeaders> addHeaders = null)
        where TSettings : class, new();
    Task<bool> ReplaceSettingsAsync(IDictionary<string, string> newSettings,
                                    string route = "/settings/aws/replace",
                                    Action<HttpRequestHeaders> addHeaders = null);


}
```

You can inject interface and call the `GetAllAWSSecretManagerSecretsAsync`, `GetSettingsAsync` & `ReplaceSettingsAsync` methods:

**Get all AWS Secret Manager secrets:**

If you have provided the Region & SecretId in the settings, you can do this:

```csharp
var client = _serviceProvider.GetRequiredService<IOnTheFlySettingsClient>();

var awsSecrets = await client.GetAllAWSSecretManagerSecretsAsync();
```

OR

you can use the overload & provide the Region & SecretId:

```csharp
var client = _serviceProvider.GetRequiredService<IOnTheFlySettingsClient>();

string region = "<<your aws region here>>";
string secretId = "<<your aws secretId here>>";

var awsSecrets = await client.GetAllAWSSecretManagerSecretsAsync(secretId, region);
```

**Get current settings from API:**

```csharp
var client = _serviceProvider.GetRequiredService<IOnTheFlySettingsClient>();

var response = await client.GetSettingsAsync<MyHealthCheckBasicSettings>();
```

**Update settings in API:**

You can update the settings in `awsSecrets` returned.

and you can filter `awsSecrets` to contain only the settings you want to update.

```csharp
var client = _serviceProvider.GetRequiredService<IOnTheFlySettingsClient>();

var response = await client.ReplaceSettingsAsync(awsSecrets);
```

### Documentation

[Documentation](https://github.com/VeritasSoftware/OnTheFlySettings)

[Sample usage](https://github.com/VeritasSoftware/OnTheFlySettings/blob/master/OnTheFlySettings.Tests/OnTheFlySettingsAWSClientTests.cs)