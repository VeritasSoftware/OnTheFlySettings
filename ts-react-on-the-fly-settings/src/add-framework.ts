import { Context, createContext } from 'react';
import { OnTheFlySettings } from "./on-the-fly-settings";

export var OnTheFlySettingsContext: Context<any> = createContext(null);

export class Framework {    
    addOnTheFlySettings<TSettings>(settings: TSettings) : void {
        let onTheFlySettings = new OnTheFlySettings(settings);

        OnTheFlySettingsContext = createContext(onTheFlySettings);
    }
}

export const framework = new Framework();