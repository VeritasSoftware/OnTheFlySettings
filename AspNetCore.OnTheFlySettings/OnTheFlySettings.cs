using System.Text.Json;

namespace AspNetCore.OnTheFlySettings
{
    public class OnTheFlySettings<TSettings> : IOnTheFlySettings<TSettings>, IOnTheFlySettings 
        where TSettings : class, new()
    {
        private TSettings _current;
        private TSettings? _old;
        private readonly object _lock = new();
        public event Func<TSettings, TSettings, Task>? OnSettingsChanged;

        public OnTheFlySettings(TSettings initial)
        {
            _current = initial;
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
                _old = oldSettings;
                _current = newSettings;
                OnSettingsChanged?.Invoke(oldSettings, newSettings);
            }
        }
    }
}
