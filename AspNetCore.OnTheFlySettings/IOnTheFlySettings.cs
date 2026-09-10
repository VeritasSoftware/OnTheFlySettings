namespace AspNetCore.OnTheFlySettings
{
    public interface IOnTheFlySettings
    {
        object CurrentObject { get; }
        void Replace(object newSettings);
    }

    public interface IOnTheFlySettings<TSettings>
        where TSettings : class, new()
    {
        TSettings Current { get; }

        event Func<TSettings, TSettings, Task>? OnSettingsChanged;

        void Replace(TSettings newSettings);
    }
}