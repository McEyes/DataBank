import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule, TranslateLoader, TranslateService } from '@ngx-translate/core';
// TS Translate data

//todo Temporarily add translation data; Remove after adding to the server
import en from '../core/translate/en.json';
import zh from '../core/translate/zh.json';
export function translationApiLoaderFactory(api: TranslationApiService) {
  const currLang = localStorage.getItem('lang') || 'en';
  const langJson = currLang === 'en' ? en : zh;
  return new TranslationApiLoader(api, langJson);
}

import { DataTopicComponent } from './dataTopic/dataTopic.component';
import { DataTopicPermissionComponent } from './permission/permission.component';
import { DataTopicRecordComponent } from './record/record.component';
import { MyDataTopicComponent } from './myData/myData.component';
import { MyDataTopicApplyComponent } from './myApply/myApply.component';
import { DataTopicStoreRoutingModule } from './dataTopicStore-routing.module';

import { PaginatorModule } from 'primeng/paginator';
import { TableModule } from 'primeng/table';
import { MultiSelectModule } from 'primeng/multiselect';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ConfirmPopupModule } from 'primeng/confirmpopup';
import { DialogModule } from 'primeng/dialog';
import { ToastModule } from 'primeng/toast';
import { TreeModule } from 'primeng/tree';
import { SpeedDialModule } from 'primeng/speeddial';
import { EditorModule } from 'primeng/editor';
import { RatingModule } from 'primeng/rating';
import { ProgressBarModule } from 'primeng/progressbar';
import { CalendarModule } from 'primeng/calendar';
import { TagModule } from 'primeng/tag';
import { ImageModule } from 'primeng/image';
import { TreeSelectModule } from 'primeng/treeselect';
import { TreeTableModule } from 'primeng/treetable';
import { NzUploadModule } from 'ng-zorro-antd/upload';
import { NzPageHeaderModule } from 'ng-zorro-antd/page-header';
import { NzTabsModule } from 'ng-zorro-antd/tabs';
import { NzDescriptionsModule } from 'ng-zorro-antd/descriptions';
import { NzStatisticModule } from 'ng-zorro-antd/statistic';
import { NzStepsModule } from 'ng-zorro-antd/steps';
import { NzTimelineModule } from 'ng-zorro-antd/timeline';
import { NzAutocompleteModule } from 'ng-zorro-antd/auto-complete';
import { NzSpaceModule } from 'ng-zorro-antd/space';
import { NzCollapseModule } from 'ng-zorro-antd/collapse';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { NzPaginationModule } from 'ng-zorro-antd/pagination';
import { NzPopoverModule } from 'ng-zorro-antd/popover';
import { NzMenuModule } from 'ng-zorro-antd/menu';
import { NzCascaderModule } from 'ng-zorro-antd/cascader';
import { DataAssetManageModule } from '../dataAssetManage/dataAssetManage.module'

// components
import { DataTopicStoreHomeComponent } from 'src/app/dataTopicStore/components/home/home.component'
import { DataTopicCreateComponent } from 'src/app/dataTopicStore/components/create/create.component'
import { DataTopicEditStep1Component } from 'src/app/dataTopicStore/components/create/components/step1'
import { DataTopicEditStep2Component } from 'src/app/dataTopicStore/components/create/components/step2'
import { DataTopicEditStep3Component } from 'src/app/dataTopicStore/components/create/components/step3'
import { DataTopicEditStep4Component } from 'src/app/dataTopicStore/components/create/components/step4'
import { DataTopicStoreDetailComponent } from 'src/app/dataTopicStore/components/detail/detail.component'
import { DatapreviewComponent } from 'src/app/dataTopicStore/components/detail/dataPreview'
import { DataTopicEditComponent } from 'src/app/dataTopicStore/components/edit/edit'
import { DataLineageComponent } from 'src/app/core/components/sql-flow/data-lineage'
import { BusinessModelCategoryManageComponent } from './categoryManage/categoryManage.component';

import {
  TranslationApiService,
  TranslationApiLoader,
  LoadingModule,
} from 'jabil-bus-lib';

import { register } from 'swiper/element/bundle';
register();
@NgModule({
  declarations: [
    DataTopicComponent, 
    DataTopicPermissionComponent,
    DataTopicRecordComponent,
    DataTopicStoreHomeComponent,
    DataTopicCreateComponent,
    DataTopicEditStep1Component,
    DataTopicEditStep2Component,
    DataTopicEditStep3Component,
    DataTopicEditStep4Component,
    DataTopicStoreDetailComponent,
    DataTopicEditComponent,
    MyDataTopicComponent,
    MyDataTopicApplyComponent,
    DatapreviewComponent,
    DataLineageComponent,
    BusinessModelCategoryManageComponent
  ],
  providers: [],
  entryComponents: [
    DataTopicComponent, 
    DataTopicPermissionComponent,
    DataTopicRecordComponent,
    DataTopicStoreHomeComponent,
    DataTopicCreateComponent,
    DataTopicEditStep1Component,
    DataTopicEditStep2Component,
    DataTopicEditStep3Component,
    DataTopicEditStep4Component,
    DataTopicStoreDetailComponent,
    DataTopicEditComponent,
    MyDataTopicComponent,
    MyDataTopicApplyComponent,
    DatapreviewComponent,
    BusinessModelCategoryManageComponent
  ],
  imports: [
    DataTopicStoreRoutingModule,
    TreeTableModule, TreeSelectModule, ConfirmPopupModule, ImageModule, TagModule, CalendarModule, ProgressBarModule, RatingModule, EditorModule, SpeedDialModule, TreeModule, ToastModule, DataAssetManageModule, DialogModule, ConfirmDialogModule, LoadingModule, CommonModule, PaginatorModule, TableModule, MultiSelectModule, 
    NzCascaderModule, NzMenuModule, NzPopoverModule, NzPaginationModule, NzUploadModule, NzTagModule, NzPageHeaderModule, NzTabsModule, NzDescriptionsModule, NzStatisticModule, NzTimelineModule, NzAutocompleteModule,NzSpaceModule,NzStepsModule,
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: translationApiLoaderFactory,
        deps: [TranslationApiService],
      },
    }),
    NzCollapseModule,
    // LevelOnePanelComponent,LevelTwoPanelComponent
  ],
  exports: [NzPageHeaderModule, NzTabsModule, NzDescriptionsModule, NzStatisticModule, NzTimelineModule, NzAutocompleteModule],
  schemas: [ CUSTOM_ELEMENTS_SCHEMA ]
})
export class DataTopicStoreModule {
  constructor() { }
}
