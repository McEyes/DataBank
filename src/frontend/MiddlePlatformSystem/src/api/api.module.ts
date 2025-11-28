import { NgModule } from '@angular/core';
import { HttpService } from 'jabil-bus-lib';
import CommonHttpService from './common'
import WorkflowHttpService from './home/workflow';
import RoleHttpService from './systemManage/roleApi';
import UserHttpService from './systemManage/userApi';
import DataTopicStoreHttpService from './dataTopicStore';
@NgModule({
  declarations: [],
  providers: [HttpService, CommonHttpService, WorkflowHttpService, RoleHttpService, UserHttpService, DataTopicStoreHttpService],
  entryComponents: [],
  imports: [],
  exports: [],
})
export class ApiModule {
  constructor() { }
}
