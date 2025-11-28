import { TranslateLoader } from '@ngx-translate/core';
import { TranslationApiService } from './translation.service';
export declare class TranslationApiLoader implements TranslateLoader {
    private api;
    private translateJson?;
    constructor(api: TranslationApiService, translateJson?: object | undefined);
    getTranslation(lang: string): any;
}
