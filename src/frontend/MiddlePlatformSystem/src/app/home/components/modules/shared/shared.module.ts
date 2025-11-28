import { ANALYZE_FOR_ENTRY_COMPONENTS, NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { ITPermissionApplyComponent } from 'src/app/home/components/DataSourceApplication/permission-detail';
import { LevelOnePanelComponent } from 'src/app/dataAssetManage/components/level-one-panel';
import { DynamicComponentContainerComponent } from 'src/app/core/components/dynamic-component-container/dynamic-component-container.component';
import { LevelTwoPanelComponent } from 'src/app/dataAssetManage/components/level-two-panel';



@NgModule({
  declarations: [
    DynamicComponentContainerComponent,
    // 声明自定义组件
    // ITPermissionApplyComponent,
  ],
  imports: [
    CommonModule,
    TranslateModule.forChild(), // 确保翻译管道可用
    // 导入第三方组件模块
    // LevelOnePanelComponent,
    // LevelTwoPanelComponent
  ], exports: [//导出模块
    DynamicComponentContainerComponent,
    // ITPermissionApplyComponent
  ],
  entryComponents: [
    // 动态加载的组件需要在这里声明
    // LevelOnePanelComponent,
  ]
})
export class DynamicFormSharedModule {
  /**
   * 允许动态添加组件到entryComponents
   */
  static forRoot(components: any[] = []): any {
    return {
      ngModule: DynamicFormSharedModule,
      providers: [
        {
          provide: ANALYZE_FOR_ENTRY_COMPONENTS,
          useValue: components,
          multi: true
        }
      ]
    };
  }
}
