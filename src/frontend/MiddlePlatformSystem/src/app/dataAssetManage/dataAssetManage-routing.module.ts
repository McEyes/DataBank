import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AssetqueryComponent } from './assetQuery/assetQuery.component';
import { DataSourceComponent } from './dataSource/dataSource.component';
import { DatatableComponent } from './dataTable/datatable.component';
import { TopicDomainDefinitionComponent } from './topicDomainDefinition/topicDomainDefinition.component';
import { APIComponent } from './api/api.component';
import { SQLQueryComponent } from './sqlQuery/sqlQuery.component'
import { DataAuthorizedComponent } from './dataAuthorized/dataAuthorized.component'
import { DataAuthorityComponent } from './dataAuthority/dataAuthority.component'
import { DataAccessLogComponent } from './dataAccessLog/dataAccessLog.component'
import { PermissionApplicationRecordComponent } from './permissionApplicationRecord/permissionApplicationRecord.component'
import { UserPermissionApplicationRecordComponent } from './userPermissionApplicationRecord/userPermissionApplicationRecord.component'
import { FunctionApiReportComponent } from './report/function-api-report/function-api-report.component';
import { DataListComponent } from './Dashbord/List/dataList.component';
import { IndicatorReportComponent } from './report/IndicatorReport/IndicatorReport.component'
const routes: Routes = [

  { path: '', pathMatch: 'full', redirectTo: '/dataAsset/assetQuery' },
  {
    path: 'assetQuery',
    component: AssetqueryComponent,
    data: { fullScreen: true, keep: true },
  },
  {
    path: 'dataSource',
    component: DataSourceComponent
  },
  {
    path: 'dataTable',
    component: DatatableComponent
  },
  {
    path: 'topicDomainDefinition',
    component: TopicDomainDefinitionComponent
  },
  {
    path: 'api',
    component: APIComponent
  },
  {
    path: 'sqlQuery',
    component: SQLQueryComponent
  },
  {
    path: 'dataAuthorized',
    component: DataAuthorizedComponent
  },
  {
    path: 'dataAuthority',
    component: DataAuthorityComponent
  },
  {
    path: 'dataAccessLog',
    component: DataAccessLogComponent
  },
  {
    path: 'permissionApplicationRecord',
    component: PermissionApplicationRecordComponent
  },
  {
    path: 'userPermissionApplicationRecordComponent',
    component: UserPermissionApplicationRecordComponent
  },
  {
    path: 'dashboard/functionApiReport',
    component: FunctionApiReportComponent
  },
  {
    path: 'dataList',
    component: DataListComponent
  },
  {
    path: 'dashboard/indicatorReport',
    component: IndicatorReportComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class DataAssetManageRoutingModule { }
