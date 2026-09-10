using System.Text.Json;

namespace AspNetCore.OnTheFlySettings
{
    public class OnTheFlySettings<TSettings> : IOnTheFlySettings<TSettings>, IOnTheFlySettings 
        where TSettings : class, new()
    {
        private TSettings? _current;
        private TSettings? _old;
        private readonly object _lock = new();
        public event Func<TSettings?, TSettings, Task>? OnSettingsChanged;
        public ILogger? Logger { get; set; }

        public OnTheFlySettings(TSettings settings)
        {
            _current = settings;
        }

        public TSettings? Current
        {
            get { lock (_lock) return _current; }
        }

        public TSettings? Old
        {
            get { lock (_lock) return _old; }
        }

        public object? CurrentObject
        {
            get { lock (_lock) return _current; }
        }

        public void Replace(object newSettings)
        {
            Logger?.LogInformation("Replacing settings with new settings.");

            if (newSettings is JsonElement jsonElement)
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var model = JsonSerializer.Deserialize<TSettings>(jsonElement.GetRawText(), options);
                Replace(model!);
                return;
            }
        }

        public void Replace(TSettings newSettings)
        {
            lock (_lock)
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                var oldSettingsStr = JsonSerializer.Serialize(_current, options);
                var newSettingsStr = JsonSerializer.Serialize(newSettings, options);

                var oldSettings = _current;               
                Logger?.LogInformation("Replacing settings with new settings. Old settings: {OldSettings}, New settings: {NewSettings}", oldSettingsStr, newSettingsStr);
                _old = oldSettings;
                _current = newSettings;
                Logger?.LogInformation("Settings replaced successfully. Old settings: {OldSettings}, New settings: {NewSettings}", oldSettingsStr, newSettingsStr);
                OnSettingsChanged?.Invoke(oldSettings, newSettings);
                Logger?.LogInformation("OnSettingsChanged event invoked successfully. Old settings: {OldSettings}, New settings: {NewSettings}", oldSettingsStr, newSettingsStr);
            }
        }
    }
}
