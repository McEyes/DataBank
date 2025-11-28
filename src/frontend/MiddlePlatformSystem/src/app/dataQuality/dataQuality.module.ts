import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
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

import { DataQualityRuleComponent } from './rule/rule.component';
import { DataQualityMyDataComponent } from './myData/myData.component';
import { DataQualityRoutingModule } from './dataQuality-routing.module';

import { PaginatorModule } from 'primeng/paginator';
import { TableModule } from 'primeng/table';
import { MultiSelectModule } from 'primeng/multiselect';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { DialogModule } from 'primeng/dialog';
import { ToastModule } from 'primeng/toast';
import { CheckboxModule } from 'primeng/checkbox';
import { RadioButtonModule } from 'primeng/radiobutton';
import { NzUploadModule } from 'ng-zorro-antd/upload';

import { DataAssetManageModule } from '../dataAssetManage/dataAssetManage.module'

import {
  TranslationApiService,
  TranslationApiLoader,
  LoadingModule,
} from 'jabil-bus-lib';

import { RuleDetailsComponent } from './rule/detail';

@NgModule({
  declarations: [DataQualityRuleComponent, DataQualityMyDataComponent, RuleDetailsComponent],
  providers: [],
  entryComponents: [DataQualityMyDataComponent, DataQualityMyDataComponent],
  imports: [RadioButtonModule, CheckboxModule, ToastModule, DataAssetManageModule, DialogModule, ConfirmDialogModule, LoadingModule, DataQualityRoutingModule, CommonModule, PaginatorModule, TableModule, MultiSelectModule, NzUploadModule,
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: translationApiLoaderFactory,
        deps: [TranslationApiService],
      },
    }),
  ],
  exports: [],
})
export class DataQualityModule {
  constructor() { }
}
