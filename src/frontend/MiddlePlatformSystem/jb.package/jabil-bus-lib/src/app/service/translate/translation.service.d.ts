import { Observable } from 'rxjs';
import { AppInfoService } from '../app-info/app-info.service';
import * as i0 from "@angular/core";
export declare class TranslationApiService {
    private appInfo;
    private _domainUri;
    private translateStatus;
    constructor(appInfo: AppInfoService);
    getTranslationsFromApi(lang: string, translateJson?: object): Observable<Object>;
    private processTranslations;
    private getLanguageByCode;
    private static isJsonString;
    getTranslateStatus(): boolean;
    static ɵfac: i0.ɵɵFactoryDeclaration<TranslationApiService, never>;
    static ɵprov: i0.ɵɵInjectableDeclaration<TranslationApiService>;
}
