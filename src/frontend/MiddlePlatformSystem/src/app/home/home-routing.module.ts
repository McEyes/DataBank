import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './page';
import { WorkflowComponent } from './components/DataSourceApplication';
import { FlowFormComponent } from './components/DataSourceApplication/flow-form/flow-form.component';


const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: '/home/page' },
  { path: 'page', component: HomeComponent },
  { path: 'workflow', component: WorkflowComponent },
  { path: 'flowform', component: FlowFormComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class HomeRoutingModule {}
