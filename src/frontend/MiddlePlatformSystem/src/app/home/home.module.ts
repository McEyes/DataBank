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

import { HomeComponent } from './page';
import { ITDatasourceComponent } from './components/DataSourceApplication/detail';
import { WorkflowComponent } from './components/DataSourceApplication';
import { HomeRoutingModule } from './home-routing.module';

import { PaginatorModule } from 'primeng/paginator';
import { TableModule } from 'primeng/table';
import { MultiSelectModule } from 'primeng/multiselect';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { DialogModule } from 'primeng/dialog';
import { ToastModule } from 'primeng/toast';
import { NzUploadModule } from 'ng-zorro-antd/upload';
import { NzPageHeaderModule } from 'ng-zorro-antd/page-header';
import { NzTabsModule } from 'ng-zorro-antd/tabs';
import { NzDescriptionsModule } from 'ng-zorro-antd/descriptions';
import { NzStatisticModule } from 'ng-zorro-antd/statistic';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzCheckboxModule } from 'ng-zorro-antd/checkbox';
import { NzRadioModule } from 'ng-zorro-antd/radio';
import { NzStepsModule } from 'ng-zorro-antd/steps';
import { NzDividerModule } from 'ng-zorro-antd/divider';
import { NzTimelineModule } from 'ng-zorro-antd/timeline';
import { NzAutocompleteModule } from 'ng-zorro-antd/auto-complete';
import { NzSpaceModule } from 'ng-zorro-antd/space';
import { NzCollapseModule } from 'ng-zorro-antd/collapse';


import { DataAssetManageModule } from '../dataAssetManage/dataAssetManage.module'

import {
  TranslationApiService,
  TranslationApiLoader,
  LoadingModule,
} from 'jabil-bus-lib';
import { DataAuthDetailComponent } from './components/DataSourceApplication/data-auth-detail/data-auth-detail.component';
import { FlowFormComponent } from './components/DataSourceApplication/flow-form/flow-form.component';
import { ApprovalHistoryComponent } from './components/DataSourceApplication/flow-form/approval-history/approval-history.component';
import { DynamicFormSharedModule } from './components/modules/shared/shared.module';
import { ITPermissionApplyComponent } from './components/DataSourceApplication/permission-detail';
import { WorkFlowModule } from '../workFlow/workFlow.module';


@NgModule({
  declarations: [HomeComponent, ITDatasourceComponent,ITPermissionApplyComponent, WorkflowComponent, DataAuthDetailComponent, FlowFormComponent, ApprovalHistoryComponent],
  providers: [],
  entryComponents: [HomeComponent, ITDatasourceComponent,ITPermissionApplyComponent, WorkflowComponent],
  imports: [ToastModule, DataAssetManageModule, DialogModule, ConfirmDialogModule, LoadingModule, HomeRoutingModule, CommonModule, PaginatorModule, TableModule, MultiSelectModule, NzUploadModule,
    NzPageHeaderModule, NzTabsModule, NzDescriptionsModule, NzStatisticModule, NzTimelineModule, NzAutocompleteModule,NzSpaceModule,NzStepsModule,
    DynamicFormSharedModule.forRoot(),
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: translationApiLoaderFactory,
        deps: [TranslationApiService],
      },
    }),
    NzCollapseModule,
    WorkFlowModule
    // LevelOnePanelComponent,LevelTwoPanelComponent
  ],
  exports: [NzPageHeaderModule, NzTabsModule, NzDescriptionsModule, NzStatisticModule, NzTimelineModule, NzAutocompleteModule],
})
export class HomePageModule {
  constructor() { }
}
