# OnTheFlySettings.Client

[![.NET Build & Test](https://github.com/VeritasSoftware/OnTheFlySettings/actions/workflows/dotnet.yml/badge.svg)](https://github.com/VeritasSoftware/OnTheFlySettings/actions/workflows/dotnet.yml)

The .NET Client library allows you to interact with the OnTheFlySettings GET & PUT endpoints.

You can provide the `Authorization Scheme` & `Token` too.

You add the library to your project:

```csharp
services.AddOnTheFlySettingsClient(settings =>
{
    settings.BaseUrl = "https://localhost:7277";
    settings.TimeoutMilliseconds = 1000;
});
```

Then, you can inject the `IOnTheFlySettingsClient` interface and call the `GetSettingsAsync` & `ReplaceSettingsAsync` methods.

### Documentation

[Documentation](https://github.com/VeritasSoftware/OnTheFlySettings)