import { AppInfoService } from '../app-info/app-info.service';
import { TranslateService } from '@ngx-translate/core';
import { TranslateCacheService } from 'ngx-translate-cache';
import * as i0 from "@angular/core";
export declare class AppInitService {
    private appInfoService;
    private translateService;
    private translateCacheService;
    constructor(appInfoService: AppInfoService, translateService: TranslateService, translateCacheService: TranslateCacheService);
    init(): Promise<unknown>;
    initApp(app: any, resolve: any, reject: any): Promise<void>;
    private localeInitializer;
    private setupTranslation;
    private sleep;
    static ɵfac: i0.ɵɵFactoryDeclaration<AppInitService, never>;
    static ɵprov: i0.ɵɵInjectableDeclaration<AppInitService>;
}
