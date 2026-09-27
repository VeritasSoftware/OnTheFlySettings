export class OnTheFlySettings<TSettings> {
    Current: TSettings;
    Old?: TSettings;
    CurrentObj: any;

    constructor(settings: TSettings) {
        this.Current = settings;
        this.CurrentObj = settings;
    }

    replaceSettingsAsync(newSettings: TSettings): void {
        let oldSettings = this.Current;
        this.Current = newSettings;
        this.Old = oldSettings;

        const event = new CustomEvent<TSettings>('onSettingsChanged', {
            detail: newSettings,
            bubbles: true,
            cancelable: true
        });
      
        dispatchEvent(event);
    }
}