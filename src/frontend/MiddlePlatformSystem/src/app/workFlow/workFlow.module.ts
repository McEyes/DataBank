import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DataDownApplicationComponent } from './dataBank/dataDownApplication/data-down-application.component';
import { ToastModule } from 'primeng/toast';
import { DialogModule } from 'primeng/dialog';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { WorkFlowRoutingModule } from './workFlow-routing.module';
import { PaginatorModule } from 'primeng/paginator';
import { TableModule } from 'primeng/table';
import { MultiSelectModule } from 'primeng/multiselect';
import { NzUploadModule } from 'ng-zorro-antd/upload';
import { NzPageHeaderModule } from 'ng-zorro-antd/page-header';
import { NzTabsModule } from 'ng-zorro-antd/tabs';
import { NzDescriptionsModule } from 'ng-zorro-antd/descriptions';
import { NzStatisticModule } from 'ng-zorro-antd/statistic';
import { NzTimelineModule } from 'ng-zorro-antd/timeline';
import { NzAutocompleteModule } from 'ng-zorro-antd/auto-complete';
import { NzSpaceModule } from 'ng-zorro-antd/space';
import { NzStepsModule } from 'ng-zorro-antd/steps';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { translationApiLoaderFactory } from '../app.module';

import {
  TranslationApiService,
  TranslationApiLoader,
  LoadingModule,
} from 'jabil-bus-lib';
import { NzCollapseModule } from 'ng-zorro-antd/collapse';
import { DataAssetManageModule } from '../dataAssetManage/dataAssetManage.module';


@NgModule({
  declarations: [
    DataDownApplicationComponent
  ],
  imports: [ToastModule, DialogModule, ConfirmDialogModule,DataAssetManageModule, LoadingModule, WorkFlowRoutingModule, CommonModule, PaginatorModule, TableModule, MultiSelectModule, NzUploadModule,
    NzPageHeaderModule, NzTabsModule, NzDescriptionsModule, NzStatisticModule, NzTimelineModule, NzAutocompleteModule, NzSpaceModule, NzStepsModule,
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: translationApiLoaderFactory,
        deps: [TranslationApiService],
      },
    }),
    NzCollapseModule
  ],
  exports: [DataDownApplicationComponent]
})
export class WorkFlowModule { }
