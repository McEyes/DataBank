import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';
import { TranslateModule, TranslateLoader, TranslateService } from '@ngx-translate/core';
import { DataAssetManageRoutingModule } from './dataAssetManage-routing.module';
// primeng
import { ButtonModule } from 'primeng/button';
import { PaginatorModule } from 'primeng/paginator';
import { TableModule } from 'primeng/table';
import { ToastModule } from 'primeng/toast';
import { DialogModule } from 'primeng/dialog';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { MultiSelectModule } from 'primeng/multiselect';
import { CalendarModule } from 'primeng/calendar';
import { RadioButtonModule } from 'primeng/radiobutton';
import { CascadeSelectModule } from 'primeng/cascadeselect';
// import { InputTextareaModule } from 'primeng/inputtextarea';
import { InputSwitchModule } from 'primeng/inputswitch';
import { TreeModule } from 'primeng/tree';
import { ConfirmPopupModule } from 'primeng/confirmpopup';
import { PasswordModule } from 'primeng/password';
import { TagModule } from 'primeng/tag';
import { TooltipModule } from 'primeng/tooltip';
import { TreeSelectModule } from 'primeng/treeselect';
import { CheckboxModule } from 'primeng/checkbox';
// ant
import { NzUploadModule } from 'ng-zorro-antd/upload';
import { NzCascaderModule } from 'ng-zorro-antd/cascader';
import { NzTabsModule } from 'ng-zorro-antd/tabs';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { NzPaginationModule } from 'ng-zorro-antd/pagination';
import { NzInputNumberModule } from 'ng-zorro-antd/input-number';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzCheckboxModule } from 'ng-zorro-antd/checkbox';
import { NzSwitchModule } from 'ng-zorro-antd/switch';
import { SpeedDialModule } from 'primeng/speeddial';
import { NzTreeModule } from 'ng-zorro-antd/tree';
import { NzDatePickerModule } from 'ng-zorro-antd/date-picker';
import {
  TranslationApiService,
  TranslationApiLoader,
  LoadingModule,
  QRModule,
  VideoDialogModule,
  SecondToHourModule,
  FormatTimeModule,
  CommonService,
} from 'jabil-bus-lib';

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

// components
import { DatapreviewComponent } from './components/dataPreview/dataPreview.component'
import { LevelOnePanelComponent } from './components/level-one-panel'
import { LevelTwoPanelComponent } from './components/level-two-panel'

// Assetquery
import { AssetqueryComponent } from './assetQuery/assetQuery.component';
import { AssetquerydetailsComponent } from './assetQuery/assetQueryDetails/assetQueryDetails.component';
import { AssetQueryPermissionComponent } from './assetQuery/permission'
import { AssetQueryDataInfoComponent } from './assetQuery/permission/dataInformation'
// Datasource
import { DataSourceComponent } from './dataSource/dataSource.component';
import { DataSourceDetailsComponent } from './dataSource/detail'
// DataTable
import { DatatableComponent } from './dataTable/datatable.component'
import { DatatableDetailsComponent } from './dataTable/detail'
import { DataInfoComponent } from './dataTable/detail/dataInformation'
import { MultiLevleApproversComponent } from './dataTable/detail/multiLevelApprovers'
import { DatatableFieldComponent } from './dataTable/field'
import { DatatableFieldDetailsComponent } from './dataTable/field/detail'
// Topic Domain Definition
import { TopicDomainDefinitionComponent } from './topicDomainDefinition/topicDomainDefinition.component';
import { TopicDomainDefinitionDetailsComponent } from './topicDomainDefinition/detail';
// API
import { APIComponent } from './api/api.component';
import { APIDetailComponent } from './api/detail'
import { AttributeCompComponent } from './api/detail/component/attribute-comp'
import { ImplementCompComponent } from './api/detail/component/implement-comp'
import { ParameterCompComponent } from './api/detail/component/parameter-comp'
import { DataapiApiComponent } from './api/api'
import { ParamsTableCompComponent } from './api/api/component/params-table-comp'
import { HeaderTableCompComponent } from './api/api/component/header-table-comp'
import { HeaderParamsTableComponent } from './api/api/component/header-params-table'
import { DataapiSqlComponent } from './api/sql'
// SQL Query
import { SQLQueryComponent } from './sqlQuery/sqlQuery.component'
// Data Authorized
import { DataAuthorizedComponent } from './dataAuthorized/dataAuthorized.component'
// Data Authority
import { DataAuthorityComponent } from './dataAuthority/dataAuthority.component'
import { DataAuthorityDetailComponent } from './dataAuthority/detail'
//Data Log
import { DataAccessLogComponent } from './dataAccessLog/dataAccessLog.component'
import { DataAccesssLogDetailComponent } from './dataAccessLog/detail'
// PermissionApplicationRecord
import { PermissionApplicationRecordComponent } from './permissionApplicationRecord/permissionApplicationRecord.component'
// UserPermissionApplicationRecord
import { UserPermissionApplicationRecordComponent } from './userPermissionApplicationRecord/userPermissionApplicationRecord.component'
// tour
import { SystemTourComponent } from './components/tour';
import { FunctionApiReportComponent } from './report/function-api-report/function-api-report.component'
import { DataListComponent } from './Dashbord/List/dataList.component';
import { IndicatorReportComponent } from './report/IndicatorReport/IndicatorReport.component'
@NgModule({
  imports: [
    CommonModule,
    FormsModule,

    // jabil-bus-lib
    LoadingModule,
    QRModule,
    VideoDialogModule,
    SecondToHourModule,
    FormatTimeModule,
    SpeedDialModule,
    // primeng
    ButtonModule,
    PaginatorModule,
    TableModule,
    ToastModule,
    DialogModule,
    ConfirmDialogModule,
    MultiSelectModule,
    CalendarModule,
    RadioButtonModule,
    CascadeSelectModule,
    // InputTextareaModule,
    InputSwitchModule,
    TreeModule,
    ConfirmPopupModule,
    PasswordModule,
    TagModule,
    TooltipModule,
    TreeSelectModule,
    CheckboxModule,
    // ant
    NzUploadModule,
    NzCascaderModule,
    NzTabsModule,
    NzTagModule,
    NzPaginationModule,
    NzInputNumberModule,
    NzSelectModule,
    NzCheckboxModule,
    NzSwitchModule,
    NzTreeModule,
    NzDatePickerModule,

    DataAssetManageRoutingModule,
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: translationApiLoaderFactory,
        deps: [TranslationApiService],
      },
    }),
  ],
  exports: [RouterModule, LevelOnePanelComponent, LevelTwoPanelComponent, AssetQueryPermissionComponent, AssetQueryDataInfoComponent],
  declarations: [
    DatapreviewComponent,
    LevelOnePanelComponent,
    LevelTwoPanelComponent,

    AssetqueryComponent,
    AssetquerydetailsComponent,
    AssetQueryPermissionComponent,
    AssetQueryDataInfoComponent,

    DataSourceComponent,
    DataSourceDetailsComponent,

    DatatableComponent,
    DatatableDetailsComponent,
    DataInfoComponent,
    MultiLevleApproversComponent,
    DatatableFieldComponent,
    DatatableFieldDetailsComponent,

    TopicDomainDefinitionComponent,
    TopicDomainDefinitionDetailsComponent,

    APIComponent,
    APIDetailComponent,
    AttributeCompComponent,
    ImplementCompComponent,
    ParameterCompComponent,
    DataapiApiComponent,
    ParamsTableCompComponent,
    HeaderTableCompComponent,
    HeaderParamsTableComponent,
    DataapiSqlComponent,

    SQLQueryComponent,

    DataAuthorizedComponent,

    DataAuthorityComponent,
    DataAuthorityDetailComponent,

    DataAccessLogComponent,
    DataAccesssLogDetailComponent,

    PermissionApplicationRecordComponent,

    UserPermissionApplicationRecordComponent,

    SystemTourComponent,
    FunctionApiReportComponent,
    DataListComponent,
    IndicatorReportComponent
  ],
})
export class DataAssetManageModule {
  constructor(commonService: CommonService, translate: TranslateService) {
    commonService.translateData(TranslateData, translate);
  }
}
