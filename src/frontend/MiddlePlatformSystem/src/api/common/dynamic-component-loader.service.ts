// import { Injectable, Compiler, Injector, ComponentFactory, ComponentRef, ViewContainerRef, Type } from '@angular/core';
// import { HttpClient } from '@angular/common/http';
// import { Observable, of, from } from 'rxjs';
// import { map, mergeMap } from 'rxjs/operators';
// import { JitCompilerFactory } from '@angular/platform-browser-dynamic';

// // 导入必要的Angular模块和装饰器
// import { NgModule, Component, NO_ERRORS_SCHEMA, Input, Output, EventEmitter } from '@angular/core';

// // 定义一个基础组件，用于动态扩展
// @Component({
//   selector: 'base-dynamic-component',
//   template: '',
//   styles: []
// })
// export class BaseDynamicComponent {
//   @Input() componentData: any;
//   @Output() action = new EventEmitter<any>();

//   onButtonClick() {
//     this.action.emit({ type: 'click', data: this.componentData });
//   }
// }

// @Injectable({
//   providedIn: 'root'
// })
// export class DynamicComponentLoaderService {
//   private compiler: Compiler;

//   constructor(private httpClient: HttpClient, private injector: Injector) {
//     // 创建JIT编译器实例
//     // const compilerFactory = new JitCompilerFactory().createCompiler();
//     // this.compiler = compilerFactory;
//   }

//   // 从API获取组件定义
//   fetchComponentDefinition(): Observable<any> {
//     // 实际项目中这里应该是真实的API调用
//     // 这里使用模拟数据
//     return of({
//       com: `<div class="bg-white p-6 rounded-lg shadow-lg">
//               <h2 class="text-xl font-bold text-gray-800">{{ componentData.title }}</h2>
//               <p class="mt-2 text-gray-600">{{ componentData.content }}</p>
//               <div class="mt-4">
//                 <button class="bg-blue-500 hover:bg-blue-600 text-white px-4 py-2 rounded"
//                   (click)="onButtonClick()">
//                   {{ componentData.buttonText }}
//                 </button>
//               </div>
//             </div>`,
//       style: `h2 { color: #1e40af; }`,
//       data: {
//         title: '动态加载的组件',
//         content: '这是从服务器加载的动态内容',
//         buttonText: '点击我'
//       }
//     });
//   }

//   // 编译并创建组件
//   loadComponent(viewContainerRef: ViewContainerRef, componentDefinition: any): Promise<ComponentRef<BaseDynamicComponent>> {
//     // 创建动态组件类
//     const componentClass = this.createDynamicComponent(componentDefinition);

//     // 创建动态模块
//     const module = this.createComponentModule(componentClass);

//     // 编译模块和组件
//     return this.compiler.compileModuleAndAllComponentsAsync(module)
//       .then(factories => {
//         // 找到组件工厂
//         const componentFactory = factories.componentFactories
//           .find(factory => factory.componentType === componentClass);

//         if (!componentFactory) {
//           throw new Error('无法创建组件工厂');
//         }

//         // 创建组件实例
//         const componentRef = viewContainerRef.createComponent(componentFactory);

//         // 设置输入数据
//         componentRef.instance.componentData = componentDefinition.data;

//         // 订阅输出事件
//         componentRef.instance.action.subscribe((event:any) => {
//           console.log('动态组件事件:', event);
//         });

//         return componentRef;
//       });
//   }

//   // 创建动态组件类
//   private createDynamicComponent(componentDefinition: any): Type<BaseDynamicComponent> {
//     // 创建一个扩展自BaseDynamicComponent的新类
//     class DynamicComponentClass extends BaseDynamicComponent {}

//     // 使用Component装饰器设置模板和样式
//     const componentDecorator = Component({
//       selector: 'app-dynamic-component',
//       template: componentDefinition.com,
//       styles: [componentDefinition.style]
//     });

//     // 应用装饰器到类
//     return componentDecorator(DynamicComponentClass) as Type<BaseDynamicComponent>;
//   }

//   // 创建包含动态组件的模块
//   private createComponentModule(componentClass: Type<BaseDynamicComponent>) {
//     // @NgModule({
//     //   declarations: [componentClass],
//     //   imports: [],
//     //   providers: [],
//     //   schemas: [NO_ERRORS_SCHEMA]
//     // })
//     class DynamicComponentModule {}

//     return DynamicComponentModule;
//   }
// }

