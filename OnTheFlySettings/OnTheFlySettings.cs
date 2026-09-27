using System.Reflection;
using System.Text.Json;

namespace OnTheFlySettings
{
    public class OnTheFlySettings<TSettings> : IOnTheFlySettings<TSettings>, IOnTheFlySettings 
        where TSettings : class, new()
    {
        private TSettings _current;
        private TSettings? _old;
        private readonly object _lock = new();
        public event Func<TSettings?, TSettings, Task>? OnSettingsChanged;
        public ILogger? Logger { get; set; }

        public OnTheFlySettings(TSettings settings)
        {
            _current = settings;
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

        private void Replace(TSettings newSettings)
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

        public void Replace(Dictionary<string, string> newSettings)
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
                MapToClass(newSettings);
                Logger?.LogInformation("Settings replaced successfully. Old settings: {OldSettings}, New settings: {NewSettings}", oldSettingsStr, newSettingsStr);
                OnSettingsChanged?.Invoke(oldSettings, _current);
                Logger?.LogInformation("OnSettingsChanged event invoked successfully. Old settings: {OldSettings}, New settings: {NewSettings}", oldSettingsStr, newSettingsStr);
            }
        }

        /// <summary>
        /// Maps a dictionary of property names and values to an instance of type TSettings.
        /// </summary>
        private void MapToClass(Dictionary<string, string> dict)
        {
            if (dict == null) throw new ArgumentNullException(nameof(dict));

            Type type = typeof(TSettings);

            foreach (var kvp in dict)
            {
                // Find property (case-insensitive)
                PropertyInfo? prop = type.GetProperty(kvp.Key, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (prop != null && prop.CanWrite)
                {
                    try
                    {
                        // Convert value to property type if needed
                        object convertedValue = Convert.ChangeType(kvp.Value, prop.PropertyType);
                        prop.SetValue(_current, convertedValue);
                    }
                    catch
                    {
                        // Ignore if conversion fails
                    }
                }
            }
        }
    }
}
