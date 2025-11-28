import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RedirectComponent } from './redirect';
import { SearchComponent } from './search';

const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: '/common/saerch' },
  { path: 'search', component: SearchComponent },
  // { path: 'redirect', component: RedirectComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class CommonRoutingModule {}
