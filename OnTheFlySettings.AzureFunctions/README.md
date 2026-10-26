# OnTheFlySettings.AzureFunctions

[![.NET Build & Test](https://github.com/VeritasSoftware/OnTheFlySettings/actions/workflows/dotnet.yml/badge.svg)](https://github.com/VeritasSoftware/OnTheFlySettings/actions/workflows/dotnet.yml)

|**Packages**|Version|Downloads|
|---------------------------|:---:|:---:|
|*OnTheFlySettings*|[![Nuget Version](https://img.shields.io/nuget/v/OnTheFlySettings)](https://www.nuget.org/packages/OnTheFlySettings)|[![Downloads count](https://img.shields.io/nuget/dt/OnTheFlySettings)](https://www.nuget.org/packages/OnTheFlySettings)|
|*OnTheFlySettings.Client*|[![Nuget Version](https://img.shields.io/nuget/v/OnTheFlySettings.Client)](https://www.nuget.org/packages/OnTheFlySettings.Client)|[![Downloads count](https://img.shields.io/nuget/dt/OnTheFlySettings.Client)](https://www.nuget.org/packages/OnTheFlySettings.Client)|
|*OnTheFlySettings.AzureKeyVault.Client*|[![Nuget Version](https://img.shields.io/nuget/v/OnTheFlySettings.AzureKeyVault.Client)](https://www.nuget.org/packages/OnTheFlySettings.AzureKeyVault.Client)|[![Downloads count](https://img.shields.io/nuget/dt/OnTheFlySettings.AzureKeyVault.Client)](https://www.nuget.org/packages/OnTheFlySettings.AzureKeyVault.Client)|
|*OnTheFlySettings.AWSSecretManager.Client*|[![Nuget Version](https://img.shields.io/nuget/v/OnTheFlySettings.AWSSecretManager.Client)](https://www.nuget.org/packages/OnTheFlySettings.AWSSecretManager.Client)|[![Downloads count](https://img.shields.io/nuget/dt/OnTheFlySettings.AWSSecretManager.Client)](https://www.nuget.org/packages/OnTheFlySettings.AWSSecretManager.Client)|

![On The Fly Settings](https://raw.githubusercontent.com/VeritasSoftware/OnTheFlySettings/master/Images/OnTheFlySettings.jpg)

The .NET Client library allows you to fully automate the update of the target API/App setting when the underlying secret in Azure Key Vault is updated.

## Step 1: Add the library to your Azure Functions App project.

You add the library from Nuget to your Functions App project and in your `Program.cs` or `Startup.cs`:

```csharp
using OnTheFlySettings.AzureFunctions;
```

```csharp
var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services.AddOnTheFlySettingsFunctionsClient(settings =>
{
    // Url of target API/App whose settings you want to update 
    // when the underlying secret in Azure Key Vault is updated.
    settings.BaseUrl = "<<url-of-target-api-app>>";
    settings.TimeoutMilliseconds = 1000 * 60 * 5;

    settings.Azure.KeyVaultUrl = "<<your-key-vault-url>>";

    //settings.Azure.UseManagedIdentity = true;
    // OR                
    settings.Azure.Credentials.TenantId = "<<your-tenant-id>>";
    settings.Azure.Credentials.ClientId = "<<your-client-id>>";
    settings.Azure.Credentials.ClientSecret = "<<your-client-secret>>";
});

builder.Build().Run();
```

Deploy the Azure Function to Azure.

## Step 2: Create an `Event Subscription` in your Azure Key Vault 

Create an `Event Subscription` in your Azure Key Vault and point it to the Azure Function endpoint.

This subscription then calls the OnTheFlySettings Azure Function endpoint when the secret is updated.

![Azure Event Subscription](https://raw.githubusercontent.com/VeritasSoftware/OnTheFlySettings/master/Images/AzureKeyVault_EventSubscription.png)

That's it! Now your Azure Function is ready to receive the event from Azure Key Vault when the secret is updated.

## Step 3: Update underlying secret in Azure Key Vault

Let us say you update the secret `HealthCheckIntervalCronExpression` (for eg.) in Azure Key Vault and create a new version.

This will trigger the Azure Function which will fetch the secret value from the Key Vault 

and call the target API/App endpoint to update the setting.

Complete automation of the setting update is achieved without any manual intervention.

![Azure Secret Update](https://raw.githubusercontent.com/VeritasSoftware/OnTheFlySettings/master/Images/AzureKeyVaultSecrets_Update.png)

### Documentation

[Documentation](https://github.com/VeritasSoftware/OnTheFlySettings)

[Demo Functions App](https://github.com/VeritasSoftware/OnTheFlySettings/blob/master/OnTheFlySettings.AzureFunctions.Demo)