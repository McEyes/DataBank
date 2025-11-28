import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DataTopicComponent } from './dataTopic/dataTopic.component';
import { DataTopicPermissionComponent } from './permission/permission.component';
import { DataTopicRecordComponent } from './record/record.component';
import { MyDataTopicComponent } from './myData/myData.component';
import { MyDataTopicApplyComponent } from './myApply/myApply.component';
import { BusinessModelCategoryManageComponent } from './categoryManage/categoryManage.component';

const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: '/dataTopicStore/page' },
  { path: 'page', component: DataTopicComponent },
  { path: 'create', component: DataTopicComponent },
  { path: 'permission', component: DataTopicPermissionComponent },
  { path: 'record', component: DataTopicRecordComponent },
  { path: 'myData', component: MyDataTopicComponent },
  { path: 'myApply', component: MyDataTopicApplyComponent },
  { path: 'categoryManagement', component: BusinessModelCategoryManageComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class DataTopicStoreRoutingModule {}
