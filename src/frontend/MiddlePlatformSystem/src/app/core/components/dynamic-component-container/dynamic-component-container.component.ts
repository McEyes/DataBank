import { Component, ComponentFactoryResolver, ComponentRef, Injector, Input, OnInit, SimpleChanges, Type, ViewChild, ViewContainerRef } from '@angular/core';
import { FlowDetailFormComponent, FlowFormContainer } from '../../directives/flow-form-directive.directive';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-dynamic-component-container',
  templateUrl: './dynamic-component-container.component.html',
  styleUrls: ['./dynamic-component-container.component.scss']
})
export class DynamicComponentContainerComponent implements OnInit , FlowFormContainer{
  @ViewChild('componentContainer', { read: ViewContainerRef, static: true }) container!: ViewContainerRef;

  // 同时获取组件实例和ViewContainerRef
  // @ViewChild('componentContainer', { static: true }) flowForm!: FlowDetailFormComponent;


  @Input() componentType: Type<FlowDetailFormComponent> | null = null;
  @Input() flowData: any;
  @Input() formData: any;
  @Input() flowForm: any;
  @Input() userInfo: any;


  private componentRef: ComponentRef<FlowDetailFormComponent> | null = null;
  private subscriptions = new Subscription();




  constructor(private componentFactoryResolver: ComponentFactoryResolver,
    private injector: Injector) { }

  ngOnInit(): void {
  }

  // 监听输入属性变化，当 componentType 或 flowData 变化时重新加载组件
  ngOnChanges(changes: SimpleChanges): void {
   // 如果是组件类型变化，需要重新创建组件
    if (changes['componentType'] &&
        changes['componentType'].previousValue !== changes['componentType'].currentValue) {
      this.clearComponent();
      if (this.componentType) {
        this.createComponent();
      }
    }
    // 如果只是数据变化（如flowData），更新现有组件的数据
    else if (this.componentRef) {
      // 检查flowData是否变化
      if (changes['flowData']) {
        this.componentRef.instance.flowData = this.flowData;
      }

      // 同步其他输入数据的变化
      if (changes['formData']) {
        this.componentRef.instance.formData = this.formData;
      }
      if (changes['flowForm']) {
        this.componentRef.instance.flowForm = this.flowForm;
      }
      if (changes['userInfo']) {
        this.componentRef.instance.userInfo = this.userInfo;
      }

      // 手动触发变更检测，确保视图更新
      this.componentRef.changeDetectorRef.detectChanges();
    }
  }



  // 创建动态组件
  private createComponent(): void {
    if (!this.componentType) return;

    // 获取组件工厂
    const factory = this.componentFactoryResolver.resolveComponentFactory(this.componentType);

    // 清除容器（双重保障）
    this.container.clear();

    // 创建组件实例
    this.componentRef = this.container.createComponent(factory, 0, this.injector);

    if (this.componentRef == null) return;
    // 设置输入属性
    this.componentRef.instance.flowData = this.flowData;
    this.componentRef.instance.flowForm = this.flowForm;
    this.componentRef.instance.formData = this.formData;
    this.componentRef.instance.userInfo = this.userInfo;

    // 触发变更检测
    this.componentRef.changeDetectorRef.detectChanges();
  }

  // 清除现有组件
  private clearComponent(): void {
    // 清除容器
    this.container.clear();

    // 销毁组件引用
    if (this.componentRef) {
      this.componentRef.destroy();
      this.componentRef = null;
    }
  }

  /**
   * 更新组件数据
   */
  updateData(newData: any): void {
    if (this.componentRef) {
      this.componentRef.instance.flowData = newData;
      this.componentRef.changeDetectorRef.detectChanges();
    }
  }

  ngOnDestroy(): void {
    this.clearComponent();
    this.subscriptions.unsubscribe();
  }

}
