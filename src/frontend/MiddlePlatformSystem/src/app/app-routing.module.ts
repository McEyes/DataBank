import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    redirectTo: '/home/page',
  },
  {
    path: 'dataAsset',
    loadChildren: () => import('./dataAssetManage/dataAssetManage.module').then(m => m.DataAssetManageModule),
  },
  // {
  //   path: 'login',
  //   loadChildren: () =>
  //     import('./login/login-routing.module').then(m => m.LoginRoutingModule),
  // },
   // default page
  {
    path: 'common',
    // canActivate: [DefaultRouteGuardService],
    loadChildren: () => import('./common/common.module').then(m => m.CommonPageModule),
  },
  {
    path: 'home',
    loadChildren: () => import('./home/home.module').then(m => m.HomePageModule),
  },
  {
    path: 'system',
    loadChildren: () => import('./systemManage/systemManage.module').then(m => m.SystemManageModule),
  },
  {
    path: 'dataQuality',
    loadChildren: () => import('./dataQuality/dataQuality.module').then(m => m.DataQualityModule),
  },
  {
    path: 'dataTopicStore',
    loadChildren: () => import('./dataTopicStore/dataTopicStore.module').then(m => m.DataTopicStoreModule),
  },
  {
    path: 'workFlow',
    loadChildren: () => import('./workFlow/workFlow.module').then(m => m.WorkFlowModule),
  },
];
// disabled
// enabled
@NgModule({
  imports: [RouterModule.forRoot(routes, {scrollPositionRestoration: 'disabled',anchorScrolling: 'disabled'})],
  exports: [RouterModule],
})
export class AppRoutingModule {}
