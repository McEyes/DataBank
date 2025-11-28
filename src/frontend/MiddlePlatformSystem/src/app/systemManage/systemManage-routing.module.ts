import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SystemRoleComponent } from './role/role.component';
import { SystemUserComponent } from './user/user.component';
import { SystemApplicationComponent } from './application/application.component';
import { DocComponent } from './doc/doc.component';
import { DictComponent } from './dict/dict.component';

const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: '/system/user' },
  { path: 'user', component: SystemUserComponent },
  { path: 'role', component: SystemRoleComponent },
  { path: 'application', component: SystemApplicationComponent },
  { path: 'doc', component: DocComponent },
  { path: 'dict', component: DictComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class SystemManageRoutingModule {}
