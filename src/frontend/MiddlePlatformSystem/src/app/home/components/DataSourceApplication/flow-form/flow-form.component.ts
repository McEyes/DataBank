
import { Component, Compiler, NgModule, OnInit, Type, ViewChild, ViewContainerRef, Injector, AfterViewInit, Input, ComponentFactoryResolver, SimpleChanges, OnChanges, DoCheck } from '@angular/core';
import { FlowDetailFormComponent, FlowForm, FlowFormContainer, FlowFormDirective, FlowFormItem, StaffInfo } from 'src/app/core/directives/flow-form-directive.directive';
import WorkflowHttpService from 'src/api/home/workflow';
import { TranslateService } from '@ngx-translate/core';
import CommonHttpService from 'src/api/common';
import { ITPermissionApplyComponent } from '../permission-detail';
import { ITDatasourceComponent } from '../detail';
import { AssetQueryPermissionComponent } from 'src/app/dataAssetManage/assetQuery/permission';
import { ConfirmationService, MessageService } from 'primeng/api';
import { TranslateData } from 'src/app/core/translate/translate-data';
import { CommonService } from 'jb.package/jabil-bus-lib';
import { DynamicComponentContainerComponent } from 'src/app/core/components/dynamic-component-container/dynamic-component-container.component';
import { Router } from '@angular/router';
import { NzUploadChangeParam } from 'ng-zorro-antd/upload';
import { environment } from 'src/environments/environment';
import { BehaviorSubject, Subscription } from 'rxjs';
import { debug } from 'console';
import { DataDownApplicationComponent } from 'src/app/workFlow/dataBank/dataDownApplication/data-down-application.component';

@Component({
  selector: 'app-flow-form',
  templateUrl: './flow-form.component.html',
  styleUrls: ['./flow-form.component.scss'],
  providers: [
    MessageService,
    ConfirmationService,
    CommonService,
  ],
})
export class FlowFormComponent implements OnInit, AfterViewInit {//, OnChanges
  @ViewChild('formComponent', { static: true }) formContainer: FlowFormContainer;// FlowDetailFormComponent;
  // @ViewChild(FlowFormDirective, { static: false }) flowFormHost!: FlowFormDirective;
  currentComponentType: Type<FlowDetailFormComponent> | null = null;
  fileHost: string = environment.FileServer;
  token: string = '';
  userInfo: any = {}
  // flowForm: FlowFormItem;
  flowData: any = { flowStatus: 0, flowTempName: '', flowTempTitle: '', formData: '', attacchments: [] };
  formData: any = {};
  flowForm: FlowForm = {
    enabledUpload: true,
    attachmentPanel: {
      show: true,
      active: true,
    },
    setActApprover: this.handleSetActApprover.bind(this),
    changeFlowData: this.handleChangeFlowData.bind(this),
    beforeSubmit(formData: any): boolean {
      return true;
    },
    afterSubmit(formData: any): boolean {
      return true;
    },
    // this.handleSetActApprover = this.handleSetActApprover.bind(this);
  };
  approvalData: any = { dialogVisible: false, flowInstId: '', acttype: '', actOperate: '', auditContent: '' };//审批对象

  historyList: any[] = [];
  currFileList: Array<any> = [];

  //权限申请
  tip: string = 'Tip';
  currentLanguage: any = 'en';
  loading: boolean = false;
  invalid: boolean = false;
  userList: any[] = [];
  selectedUser: any = {};
  selectedUserIndex: number = -1;


  flowFormMap: Record<string, Type<any>> = {
    // "itportal_dataauthapplication": ITPermissionApplyComponent,//AssetQueryPermissionComponent
    "itportal_lakeentryapplication": ITDatasourceComponent,
    "itportal_userpermissionapplication": ITPermissionApplyComponent,
    "itportal_dataoffline": DataDownApplicationComponent,
  }

  applyStatusI18NKey: any = {
    Draft: 'label.workflow.status.draft',
    Running: 'home.workflow.status.processing',
    Stop: 'label.workflow.status.stop',
    Aprd: 'home.workflow.status.processed',
    Completed: 'home.workflow.status.finish',
    Reject: 'home.workflow.status.reject',
    Cancel: 'label.workflow.status.canceled',
  };


  commonSuccessFunc(
    successE: string = 'Commit',
    successZ: string = '申请',
    msgE: string = 'Success',
    msgZ: string = '成功'
  ) {
    this.messageService.add({
      severity: 'success',
      summary: localStorage.getItem('lang') == 'en' ? successE : successZ,
      detail: localStorage.getItem('lang') == 'en' ? msgE : msgZ,
    });
  }
  commonErrorFunc(errorE: string = 'Error', errorZ: string = '错误', msg: any = '') {
    this.messageService.add({
      severity: 'error',
      summary: localStorage.getItem('lang') == 'en' ? errorE : errorZ,
      detail: msg,
    });
  }

  constructor(private http: WorkflowHttpService,
    private router: Router,
    private translate: TranslateService,
    public commonHttp: CommonHttpService,
    private commonService: CommonService,
    private messageService: MessageService,
    private confirmationService: ConfirmationService
  ) {

  }

  ngOnInit(): void {
    this.token = localStorage.getItem('jwt') || ''
    //获取组件和数据
    this.getUserInfo();
    const params = this.commonHttp.getUrlParams(window.location.hash);
    if (params.id)
      this.getDetail(params.id);
    else if (params.taskid)
      this.getDetail(params.taskid, 'todo');
    else {
      this.getFlowTemplate(params.flowtempname);
    }
  }


  ngAfterViewInit() { }

  ngOnDestroy() {
  }

  loadComponent(flowFormComponent: Type<any>): void {
    // 如果类型有效，则设置新组件和数据
    if (flowFormComponent) {
      this.currentComponentType = flowFormComponent;
    } else {
      // 清空当前状态
      this.currentComponentType = null;
    }
  }


  getForm(flowData: any) {
    if (flowData != null && flowData.flowTempName != null) {
      this.loadComponent(this.flowFormMap[flowData.flowTempName.toLowerCase()]);
    }
  }

  // 使用箭头函数作为类属性，确保this指向当前组件实例
  handleSetActApprover(actName: string, userList: StaffInfo[]) {
    // 这里的this会正确指向组件实例，且类型明确
    this.flowData.actApprovers = this.flowData.actApprovers ?? [];
    const act = this.flowData.actApprovers.find((item: any) => item.actName === actName);
    if (act) {
      act.actorParms = userList;
    } else {
      this.flowData.actApprovers.push({ actName, actorParms: userList });
    }
  }

  // 使用箭头函数作为类属性，确保this指向当前组件实例
  handleChangeFlowData(flowData: any) {
    if (this.flowData != flowData) {
      this.flowData = flowData;
    }
    this.formData = JSON.parse(flowData.formData);
    if (flowData.auditRecords && flowData.auditRecords.length > 0) {
      this.historyList = flowData.auditRecords;
    }
    this.currFileList = flowData.attacchments.map((item: any) => {
      item.showDownload = true;
      item.status = 'done';
      item.uid = item.id;
      item.isBack = true;
      return item;
    });
  }

  // 当前登录用户信息
  async getUserInfo() {
    const data = await this.commonHttp.getCurrUser();
    this.userInfo = data;
    this.flowData.applicant = this.formData.applicant = data.id;
    this.flowData.applicantName = this.formData.applicantName = data.name;
    this.flowData.applicantEMail = this.formData.applicantEMail = data.email;
  }

  // 详情
  async getDetail(id: string, type: string = 'flow') {
    this.loading = true;
    let res = await this.http.getFlowFormDetails(id, type);
    if (res.success) {
      this.flowData = res.data;
      this.flowData.formData = this.flowData.formData ?? {};
      this.formData = JSON.parse(this.flowData.formData);
      this.handleChangeFlowData(this.flowData);
      this.getForm(this.flowData);
      this.loading = false;
    } else if (res.code == -10) {
      this.loading = false;

    } else {
      this.loading = false;
      return this.messageService.add({
        severity: 'error',
        summary: localStorage.getItem('lang') == 'en' ? 'Error' : '错误',
        detail: res.msg,
      });
    }
  }


  // 详情
  async getFlowTemplate(name: string) {
    this.loading = true;
    let res = await this.http.getFlowTemplate(name);
    if (res.success) {
      if (!res.data) {
        this.loading = false;
        return this.messageService.add({
          severity: 'error',
          summary: localStorage.getItem('lang') == 'en' ? 'Error' : '错误',
          detail: "当前流程模板不存在",
        });
      }
      this.flowData.flowTempName = res.data.flowName;
      this.flowData.flowTempTitle = res.data.flowTitle;
      this.formData = this.flowData.formData = {};
      if (res.data.flowActs && res.data.flowActs.length > 0) {
        this.historyList = res.data.flowActs.map((f: any) => {
          f.actOperate = null;
          f.status = null;
          f.createTime = null;
          f.auditContent = null;
          return f;
        });
      }
      this.getForm(this.flowData);
      this.loading = false;
    } else if (res.code == -10) {
      this.loading = false;
    } else {
      this.loading = false;
      return this.messageService.add({
        severity: 'error',
        summary: localStorage.getItem('lang') == 'en' ? 'Error' : '错误',
        detail: res.msg,
      });
    }
  }

  async openApproveDialog(acttype: string) {
    this.approvalData.flowInstId = this.flowData.id;
    this.approvalData.acttype = acttype;
    this.approvalData.dialogVisible = true;
    if (acttype == 'transfer') {
      this.getUserList();
    }
  }

  // 审批按钮
  submit(act: string) {
    const valid = this.formContainer.flowForm?.validFields || ['applicant', 'reason'];
    this.formData = this.formContainer.flowForm?.formData || this.formContainer.formData;
    if (this.commonService.isInvalid(this.formData, valid) && this.flowForm.beforeSubmit(this.formData)) {
      this.formData.invalid = true;
    } else {
      this.loading = true;
      var flowData = { ...this.flowData, ...{ formData: JSON.stringify(this.formData) } };
      this.http.submitFlow(flowData, act).then((res: any) => {
        if (res.success) {
          this.flowData = res.data;
          this.flowForm.afterSubmit(this.formData)
          this.handleChangeFlowData(this.flowData);
        }
        this.loading = false;
        this.messageService.add({
          severity: res.success ? 'success' : 'error',
          summary: TranslateData.save,
          detail: res.success ? TranslateData.success : this.commonHttp.getFailMsg(res),
        });
      });
    }
  }

  async approval(type: string) {
    let typeInfo: any = {
      "agree": this.currentLanguage == 'en' ? 'agree' : '同意',
      "reject": this.currentLanguage == 'en' ? 'reject' : '拒绝',
      "return": this.currentLanguage == 'en' ? 'return' : '驳回',
      "cancel": this.currentLanguage == 'en' ? 'cancel' : '取消',
      "transfer": this.currentLanguage == 'en' ? 'transfer' : '转办',
      "jump": this.currentLanguage == 'en' ? 'jump' : '跳转',
    };
    if (!this.approvalData.auditContent) {
      this.messageService.add({ severity: 'warn', summary: TranslateData.fillComment ?? 'Please fill in the comment' });
      return;
    }

    let params = { ...this.formData };

    this.confirmationService.confirm({
      message:
        this.currentLanguage == 'en'
          ? `May I ask if you are sure you want the approval for ${typeInfo[type]}`
          : `请问是否确定要${typeInfo[type]}该条审批`,
      rejectLabel: localStorage.getItem('lang') == 'en' ? 'No' : '否',
      acceptLabel: localStorage.getItem('lang') == 'en' ? 'Yes' : '是',
      accept: async () => {
        this.loading = true;

        const data = {
          ...this.approvalData, ...{
            flowInstId: this.flowData.id,
            acttype: type,
            actOperate: type,
            attacchments: this.flowData.attacchments
          }
        };
        const res = await this.http.sendApprove(data);
        if (res.success) {
          this.loading = false;
          this.goBack();
          this.messageService.add({
            severity: 'success',
            summary: TranslateData.success,
          });
        } else {
          this.loading = false;
          this.messageService.add({
            severity: 'fail',
            summary: TranslateData.fail + ". " + res.msg,
          });
        }
      },
    });
  }

  goBack() {
    this.router.navigate(['/home/workflow'], { queryParams: { ts: new Date().getTime() } });
  }


  // 用户列表
  getUserList() {
    if (sessionStorage.getItem('userList')) {
      let userListString: any = sessionStorage.getItem('userList');
      let userList = JSON.parse(userListString);
      this.userList = userList;
    } else {
      this.loading = true;
      this.commonHttp.getUserList().then((res: any) => {
        let arr = res.data;
        const uniqueArr = Array.from(
          new Map(arr.map((item: any) => [item['id'], item])).values()
        );
        this.userList = uniqueArr;
        sessionStorage.setItem('userList', JSON.stringify(this.userList));
        this.loading = false;
      });
    }
  }

  // 文件流操作
  handleChange({ file, fileList }: NzUploadChangeParam): void {
    const status = file.status;
    if (status === 'done') {
      this.flowData.attacchments = fileList.map((item: any) => {
        return ({ ...item?.response?.data ?? item, createrBy: this.userInfo.id, createrByName: this.userInfo.name, createTime: new Date(), showDownload: true, status: 'done', uid: item.uid, isBack: true })
      });
      this.messageService.add({ severity: 'success', summary: TranslateData.fileUploadSuccess });
    } else if (status === 'removed') {
      this.flowData.attacchments = fileList.map((item: any) => {
        return ({ ...item?.response?.data ?? item, createrBy: this.userInfo.id, createrByName: this.userInfo.name, createTime: new Date(), showDownload: true, status: 'done', uid: item.uid, isBack: true })
      });
      this.messageService.add({ severity: 'success', summary: TranslateData.deleteSuccess });
    } else if (status === 'error') {
      file.error.statusText = TranslateData.fileUploadFail;
      this.messageService.add({ severity: 'error', summary: TranslateData.fileUploadFail });
    }
  }
  deleteFile(file: any) {
    this.confirmationService.confirm({
      message: this.currentLanguage == 'en' ? 'Are you sure to delete this file?' : '确定要删除该文件吗？',
      rejectLabel: localStorage.getItem('lang') == 'en' ? 'No' : '否',
      acceptLabel: localStorage.getItem('lang') == 'en' ? 'Yes' : '是',
      accept: async () => {
        this.flowData.attacchments = this.commonHttp.remove(this.flowData.attacchments, file, (a, b) => a.uid === b.uid);
        this.currFileList = this.flowData.attacchments.map((item: any) => {
          item.showDownload = true;
          item.status = 'done';
          item.uid = item.id || item.uid;
          item.isBack = true;
          return item;
        });
      }
    });
  }

  // // 通过 getter/setter 包装属性
  // get flowData() {
  //   return this._flowData;
  // }

  // set flowData(newValue) {
  //   // 当值变化时自动执行（无需手动触发）
  //   console.log('父组件检测到 flowData 变更');
  //   this._flowData = newValue;
  //   this.handleAuditRecordsChange(newValue.auditRecords)
  //   // Angular 会自动触发变更检测，更新视图
  // }
  // // 当输入属性变化时触发
  //   ngOnChanges(changes: SimpleChanges) {
  //     debugger;
  //     // 检查 flowData 或其 auditRecords 属性是否变化
  //     if (changes['flowData']) {
  //       const current = changes['flowData'].currentValue;
  //       const previous = changes['flowData'].previousValue;

  //       // 比较 auditRecords 是否变化（浅比较）
  //       if (current?.auditRecords !== previous?.auditRecords) {
  //         this.handleAuditRecordsChange(current.auditRecords);
  //       }
  //     }
  //   }

  // private handleAuditRecordsChange(newAuditRecords: any[]) {
  //   console.log('auditRecords 已更新:', newAuditRecords);
  //   // 处理变化的逻辑

  //   if (this.flowData.auditRecords && this.flowData.auditRecords.length > 0) {
  //     this.historyList = this.flowData.auditRecords;
  //   }
  // }

}
