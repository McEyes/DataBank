import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DataQualityRuleComponent } from './rule/rule.component';
import { DataQualityMyDataComponent } from './myData/myData.component';

const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: '/dataQuality/rule' },
  { path: 'rule', component: DataQualityRuleComponent },
  { path: 'myData', component: DataQualityMyDataComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class DataQualityRoutingModule { }
