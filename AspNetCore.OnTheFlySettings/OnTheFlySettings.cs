using System.Text.Json;

namespace AspNetCore.OnTheFlySettings
{
    public class OnTheFlySettings<TSettings> : IOnTheFlySettings<TSettings>, IOnTheFlySettings 
        where TSettings : class, new()
    {
        private TSettings _current;
        private TSettings? _old;
        private readonly object _lock = new();
        private readonly ILogger<OnTheFlySettings<TSettings>>? _logger;
        public event Func<TSettings, TSettings, Task>? OnSettingsChanged;

        public OnTheFlySettings(TSettings initial, ILogger<OnTheFlySettings<TSettings>>? logger = null)
        {
            _current = initial;
            _logger = logger;
        }

        public TSettings Current
        {
            get { lock (_lock) return _current; }
        }

        public TSettings? Old
        {
            get { lock (_lock) return _old; }
        }

        public object CurrentObject
        {
            get { lock (_lock) return _current; }
        }

        public void Replace(object newSettings)
        {
            _logger?.LogInformation("Replacing settings with new settings.");

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
                var oldSettings = _current;
                _logger?.LogInformation("Replacing settings with new settings. Old settings: {OldSettings}, New settings: {NewSettings}", oldSettings, newSettings);
                _old = oldSettings;
                _current = newSettings;
                _logger?.LogInformation("Settings replaced successfully. Old settings: {OldSettings}, New settings: {NewSettings}", oldSettings, newSettings);
                OnSettingsChanged?.Invoke(oldSettings, newSettings);
                _logger?.LogInformation("OnSettingsChanged event invoked successfully. Old settings: {OldSettings}, New settings: {NewSettings}", oldSettings, newSettings);
            }
        }
    }
}
