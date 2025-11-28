import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { WorkflowComponent } from '../home/components/DataSourceApplication';
import { FlowFormComponent } from '../home/components/DataSourceApplication/flow-form/flow-form.component';
import { DataDownApplicationComponent } from './dataBank/dataDownApplication/data-down-application.component';


const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: '/flow/workflow' },
  { path: 'workflow', component: WorkflowComponent },
  { path: 'flowform', component: FlowFormComponent },
  { path: 'dataoffline', component: DataDownApplicationComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class WorkFlowRoutingModule { }
