import { MissingTranslationHandler, MissingTranslationHandlerParams } from '@ngx-translate/core';
import { AppInfoService } from '../app-info/app-info.service';
import * as i0 from "@angular/core";
export declare class MissingTranslationsService implements MissingTranslationHandler {
    private appInfo;
    constructor(appInfo: AppInfoService);
    handle(params: MissingTranslationHandlerParams): string;
    static ɵfac: i0.ɵɵFactoryDeclaration<MissingTranslationsService, never>;
    static ɵprov: i0.ɵɵInjectableDeclaration<MissingTranslationsService>;
}
