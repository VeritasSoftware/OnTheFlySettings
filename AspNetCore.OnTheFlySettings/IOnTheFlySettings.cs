namespace AspNetCore.OnTheFlySettings
{
    public interface IOnTheFlySettings
    {
        ILogger? Logger { get; set; }
        object? CurrentObject { get; }
        void Replace(object newSettings);
    }

    public interface IOnTheFlySettings<TSettings>
        where TSettings : class, new()
    {
        ILogger? Logger { get; set; }
        TSettings? Current { get; }
        event Func<TSettings?, TSettings, Task>? OnSettingsChanged;
        void Replace(TSettings newSettings);
    }
}