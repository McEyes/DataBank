import { Directive, ViewContainerRef, Input, Type } from '@angular/core';

@Directive({
  selector: '[flowform-host]'
})
export class FlowFormDirective {

  constructor(public viewContainerRef: ViewContainerRef) { }

  @Input() set FlowFormDirective(value: any) {
    this.viewContainerRef.clear();
  }

}
export interface FlowFormContainer {
  flowForm: any;//表单界面配置
  flowData: any;//流程数据
  formData: any;//表单数据
  userInfo: any;//用户信息
}


export interface FlowDetailFormComponent {
  flowForm: any;//表单界面配置
  flowData: any;//流程数据
  formData: FlowForm;//表单数据
  userInfo: any;//用户信息
}


// 优化后的FlowForm接口
export interface FlowForm {
  // 使用布尔类型定义，而非直接赋值
  enabledUpload: boolean;

  // 附件面板配置对象
  attachmentPanel: {
    show: boolean;
    active: boolean; // 修正拼写错误：ayctive -> active
  };

  // 定义方法类型，指定参数和返回值类型
  setActApprover(this: FlowDetailFormComponent, actName: string, userList: Array<StaffInfo>): void;
  changeFlowData(this: FlowDetailFormComponent, flowData: any): void;
  beforeSubmit(formData: any): boolean;
  afterSubmit(formData: any): boolean;
  // 更严谨的做法是明确approver类型，例如:
  // setActApprover: (approver: User | string[]) => void;
}

export class FlowFormItem {
  constructor(public component: Type<any>, public data: any) { }
}


export class StaffInfo {
  ntid: string;//表单界面配置
  name: string;//流程数据
  email: string;//用户信息
  department: string
}
