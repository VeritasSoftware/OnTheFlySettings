namespace AspNetCore.OnTheFlySettings
{
    public interface IOnTheFlySettingsBase
    {
        internal ILogger? Logger { get; set; }
    }

    internal interface IOnTheFlySettings : IOnTheFlySettingsBase
    {        
        object? CurrentObject { get; }
        void Replace(object newSettings);
    }

    public interface IOnTheFlySettings<TSettings> :IOnTheFlySettingsBase
        where TSettings : class, new()
    {
        TSettings? Current { get; }
        TSettings? Old { get; }
        event Func<TSettings?, TSettings, Task>? OnSettingsChanged;
    }
}