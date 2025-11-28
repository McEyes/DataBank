// import { Injectable, ViewContainerRef, Injector, Type, ComponentRef, NgModuleRef, Component, NgModule, AfterViewInit, ViewChild, Renderer2, ElementRef } from '@angular/core';
// import { CommonModule } from '@angular/common';

// @Injectable({
//   providedIn: 'root'
// })
// export class DynamicComponentLoaderService {
//   constructor(private injector: Injector, private renderer: Renderer2) { }

//   async loadComponent(
//     viewContainerRef: ViewContainerRef,
//     componentCode: string,
//     styleCode: string,
//     templateCode: string
//   ): Promise<ComponentRef<any>> {
//     viewContainerRef.clear();

//     try {
//       const componentType = this.createComponent(componentCode, styleCode, templateCode);
//       const moduleType = this.createModule(componentType);//componentType
//       const moduleRef = this.createModuleRef(moduleType);

//       const componentRef = viewContainerRef.createComponent(
//         componentType,
//         { injector: this.injector, ngModuleRef: moduleRef }
//       );

//       return componentRef;
//     } catch (error) {
//       console.error('动态加载组件失败:', error);
//       throw error;
//     }
//   }

//   private createComponent(componentCode: string, styleCode: string, templateCode: string) {
//     @Component({
//       template: `<div #dynamicContent></div>`,
//       styles: []
//     })
//     class DynamicComponent implements AfterViewInit {
//       @ViewChild('dynamicContent', { static: true, read: ElementRef }) dynamicContent: ElementRef;

//       constructor(private renderer: Renderer2) { }

//       ngAfterViewInit() {
//         // 创建style元素
//         if (styleCode) {
//           const styleElement = this.renderer.createElement('style');
//           this.renderer.setProperty(styleElement, 'type', 'text/css');
//           // this.renderer.setProperty(styleElement, 'type', 'text/tailwindcss');
//           this.renderer.setProperty(styleElement, 'innerHTML', styleCode);
//           this.renderer.appendChild(document.head, styleElement);
//         }

//         // 创建内容元素
//         const element = this.renderer.createElement('div');
//         this.renderer.setProperty(element, 'innerHTML', templateCode);
//         this.renderer.appendChild(this.dynamicContent.nativeElement, element);

//         // 执行组件代码
//         try {
//           const dynamicCode = new Function('componentContext', 'renderer', componentCode);
//           dynamicCode.call(this, this, this.renderer);
//         } catch (error) {
//           console.error('执行组件代码失败:', error);
//         }
//       }
//     }

//     // 完整的组件元数据定义
//     (DynamicComponent as any).ɵcmp = {
//       type: DynamicComponent,
//       selectors: [['dynamic-component']],
//       factory: () => new DynamicComponent(this.renderer),
//       template: () => { },
//       styles: [],
//       inputs: [],
//       outputs: [],
//       host: {
//         attributes: {},
//         listeners: {}
//       },
//       viewQueries: [],
//       contentQueries: [],
//       providers: [],
//       changeDetection: 0, // 默认ChangeDetectionStrategy.Default
//       encapsulation: 2,   // 默认ViewEncapsulation.Emulated
//       exportAs: null,
//       entryComponents: []
//     };

//     return DynamicComponent as Type<any>;
//   }

//   private createModule(componentType: any) {
//     @NgModule({
//       declarations: [componentType],
//       imports: [CommonModule],
//       exports: [componentType]
//     })
//     class DynamicModule { }

//     // 完整的模块元数据定义
//     (DynamicModule as any).ɵmod = {
//       type: DynamicModule,
//       declarations: [componentType],
//       imports: [CommonModule],
//       exports: [componentType],
//       providers: [],
//       schemas: [],
//       entryComponents: [componentType],
//       bootstrap: [],
//       id: 'dynamic-module',
//       transitiveCompileComponents: [componentType],
//       jit: true,
//       declarationsOffset: 0,
//       exportsOffset: 1,
//       containsForwardDecls: false
//     };

//     // 修正NgModuleDef结构（重点修复injectorDef）
//     (DynamicModule as any).ngModuleDef = {
//       factory: () => new DynamicModule(),
//       type: DynamicModule,
//       injectorDef: {
//         factory: () => ({
//           instance: new DynamicModule(),
//           get(token: any, notFoundValue?: any) {
//             // 使用根注入器处理依赖查找
//             return this.injector.get(token, notFoundValue);
//           },
//           injector: this.injector // 使用服务的注入器
//         }),
//         providers: [],
//         imports: [CommonModule]
//       },
//       bootstrap: [],
//       declarations: [componentType],
//       exports: [componentType],
//       schemas: [],
//       entryComponents: [componentType]
//     };

//     return DynamicModule;
//   }


//   private createModuleRef(moduleType: Type<any>): NgModuleRef<any> {
//     return Reflect.construct(moduleType, [], moduleType) as NgModuleRef<any>;
//   }
// }
