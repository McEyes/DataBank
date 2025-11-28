import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RedirectComponent } from './redirect';
import { SearchComponent } from './search';
import { CommonRoutingModule } from './common-routing.module';
import { NzAutocompleteModule } from 'ng-zorro-antd/auto-complete';
import { PaginatorModule } from 'primeng/paginator';
import { TranslateModule, TranslateLoader, TranslateService } from '@ngx-translate/core';

// TS Translate data
import { TranslateData } from '../core/translate/translate-data';
//todo Temporarily add translation data; Remove after adding to the server
import en from '../core/translate/en.json';
import zh from '../core/translate/zh.json';
export function translationApiLoaderFactory(api: TranslationApiService) {
  const currLang = localStorage.getItem('lang') || 'en';
  const langJson = currLang === 'en' ? en : zh;
  return new TranslationApiLoader(api, langJson);
  // return new TranslationApiLoader(api);
}

import {
  TranslationApiService,
  TranslationApiLoader,
  LoadingModule
} from 'jabil-bus-lib';

@NgModule({
  declarations: [RedirectComponent, SearchComponent],
  providers: [],
  entryComponents: [RedirectComponent, SearchComponent],
  imports: [CommonRoutingModule, CommonModule, NzAutocompleteModule, PaginatorModule, LoadingModule,
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: translationApiLoaderFactory,
        deps: [TranslationApiService],
      },
    }),
  ],
  exports: [RedirectComponent, SearchComponent],
})
export class CommonPageModule {
  constructor() { }
}
