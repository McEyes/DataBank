// angular core
import { APP_INITIALIZER, NgModule } from '@angular/core';
import { RouterModule, RouteReuseStrategy } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HashLocationStrategy, LocationStrategy } from '@angular/common';
import { OverlayModule } from '@angular/cdk/overlay';

// translate
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { TranslateModule, TranslateLoader, TranslateService } from '@ngx-translate/core';
import {
  TranslateCacheModule,
  TranslateCacheService,
  TranslateCacheSettings,
} from 'ngx-translate-cache';

// modules
import { ApiModule } from '../api/api.module';
import { CommonPageModule } from './common/common.module';
// common
import {
  AppInitService,
  SimpleReuseStrategy,
  TranslationApiService,
  TranslationApiLoader,
  CommonService,
  DefaultRouteGuardService,
  LoadingModule,
} from 'jabil-bus-lib';

// import { LayoutModule } from './layout/layout.module'

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoginComponent } from './login/login.component';
import { JabilHeaderComponent } from './core/components/header';

export function HttpLoaderFactory(httpClient: HttpClient) {
  return new TranslateHttpLoader(httpClient);
}

import { TieredMenuModule } from 'primeng/tieredmenu';
import { CheckboxModule } from 'primeng/checkbox';
import { ToastModule } from 'primeng/toast';
import { TooltipModule } from 'primeng/tooltip';
import { NzDropDownModule } from 'ng-zorro-antd/dropdown';
import { NzPopoverModule } from 'ng-zorro-antd/popover';
import { NzAutocompleteModule } from 'ng-zorro-antd/auto-complete';

// If need local translate
import en from 'src/app/core/translate/en.json';
import zh from 'src/app/core/translate/zh.json';
import { TranslateData } from './core/translate/translate-data';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { FlowFormDirective } from './core/directives/flow-form-directive.directive';

export function translationApiLoaderFactory(api: TranslationApiService) {
  const currLang = localStorage.getItem('lang') || 'en';
  const langJson = currLang === 'en' ? en : zh;
  return new TranslationApiLoader(api, langJson);
}

// If do not need local translate
// export function translationApiLoaderFactory(api: TranslationApiService) {
//   return new TranslationApiLoader(api);
// }

export function translateCacheFactory(
  translateService: TranslateService,
  translateCacheSettings: TranslateCacheSettings
) {
  return new TranslateCacheService(translateService, translateCacheSettings);
}

export function initialize(appInitService: AppInitService) {
  return () => appInitService.init();
}

@NgModule({
  declarations: [AppComponent, LoginComponent, JabilHeaderComponent, FlowFormDirective],
  imports: [
    BrowserModule,
    AppRoutingModule,
    RouterModule,
    FormsModule,
    HttpClientModule,
    BrowserAnimationsModule,
    ApiModule,
    CommonPageModule,
    AutoCompleteModule,
    // LayoutModule,
    OverlayModule,
    LoadingModule,
    TieredMenuModule,
    CheckboxModule,
    ToastModule,
    NzDropDownModule,
    NzPopoverModule,
    NzAutocompleteModule,
    TooltipModule,
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: HttpLoaderFactory,
        deps: [HttpClient],
      },
    }),
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: translationApiLoaderFactory,
        deps: [TranslationApiService],
      },
    }),
    TranslateCacheModule.forRoot({
      cacheService: {
        provide: TranslateCacheService,
        useFactory: translateCacheFactory,
        deps: [TranslateService, TranslateCacheSettings],
      },
      cacheMechanism: 'LocalStorage', // default value is 'LocalStorage'.
      //cookieExpiry: 1, // default value is 720, a month.
    }),
  ],
  // provide: NZ_I18N, useValue: en_US ,}
  providers: [
    CommonService,
    DefaultRouteGuardService,
    { provide: RouteReuseStrategy, useClass: SimpleReuseStrategy },
    {
      provide: APP_INITIALIZER,
      useFactory: initialize,
      deps: [AppInitService],
      multi: true,
    },
    { provide: LocationStrategy, useClass: HashLocationStrategy },
    AppInitService,
  ],
  bootstrap: [AppComponent],
})
export class AppModule {
    constructor(commonService: CommonService, translate: TranslateService) {
      commonService.translateData(TranslateData, translate);
    }
}
