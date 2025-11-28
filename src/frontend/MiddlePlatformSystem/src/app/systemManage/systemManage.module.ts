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

import { SystemRoleComponent } from './role/role.component';
import { SystemUserComponent } from './user/user.component';
import { SystemApplicationComponent } from './application/application.component';
import { SystemManageRoutingModule } from './systemManage-routing.module';

import { PaginatorModule } from 'primeng/paginator';
import { TableModule } from 'primeng/table';
import { MultiSelectModule } from 'primeng/multiselect';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { DialogModule } from 'primeng/dialog';
import { ToastModule } from 'primeng/toast';
import { TagModule } from 'primeng/tag';
import { InputSwitchModule } from 'primeng/inputswitch';
import { NzUploadModule } from 'ng-zorro-antd/upload';

import { DataAssetManageModule } from '../dataAssetManage/dataAssetManage.module'

import {
  TranslationApiService,
  TranslationApiLoader,
  LoadingModule,
} from 'jabil-bus-lib';
import { UserDetailsComponent } from './user/detail';
import { RoleDetailsComponent } from './role/detail';
import { ApplicationDetailsComponent } from './application/detail';
import { DocComponent } from './doc/doc.component';
import { DocDetailComponent } from './doc/detail/detail.component'
import { NzInputModule } from "ng-zorro-antd/input";
import { DictComponent } from './dict/dict.component';
import { DictItemDetailComponent } from './dict/detail/detail.component'
import { DictDetailComponent } from './dict/dictDetail/detail.component'
import { TreeModule } from 'primeng/tree';

@NgModule({
  declarations: [SystemRoleComponent, SystemUserComponent, UserDetailsComponent, RoleDetailsComponent, SystemApplicationComponent, ApplicationDetailsComponent, DocComponent, DocDetailComponent, DictComponent, DictItemDetailComponent, DictDetailComponent],
  providers: [],
  entryComponents: [SystemRoleComponent, SystemUserComponent],
  imports: [InputSwitchModule, ToastModule, DataAssetManageModule, DialogModule, ConfirmDialogModule, LoadingModule, SystemManageRoutingModule, CommonModule, PaginatorModule, TableModule, MultiSelectModule, NzUploadModule, TagModule,

    TreeModule,
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: translationApiLoaderFactory,
        deps: [TranslationApiService],
      },
    }), NzInputModule],
  exports: [],
})
export class SystemManageModule {
  constructor() { }
}
