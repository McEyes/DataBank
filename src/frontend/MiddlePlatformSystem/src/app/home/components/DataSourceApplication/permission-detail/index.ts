import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { MessageService, ConfirmationService } from 'primeng/api';
import { CommonService, BigFileUploadService, LocalStorage } from 'jabil-bus-lib';
import { TranslateData } from 'src/app/core/translate/translate-data';
import WorkflowHttpService from 'src/api/home/workflow';
import HomeHttpService from 'src/api/home';
import { NzUploadChangeParam, NzUploadFile } from 'ng-zorro-antd/upload';
import CommonHttpService from 'src/api/common';
import { environment } from 'src/environments/environment';
import { Router } from '@angular/router';
import { FlowFormComponent } from '../flow-form/flow-form.component';
import { FlowDetailFormComponent, FlowForm } from 'src/app/core/directives/flow-form-directive.directive';
import { before } from 'node:test';

@Component({
  selector: 'it-portal-permission-apply',//jabil-all-data-grant-apply
  templateUrl: './index.html',
  styleUrls: ['./index.scss'],
  providers: [
    LocalStorage,
    MessageService,
    ConfirmationService,
    CommonService,
    BigFileUploadService,
    HomeHttpService,
    WorkflowHttpService,
    CommonService,
  ],
})
export class ITPermissionApplyComponent implements OnInit, FlowDetailFormComponent {
  @Input() flowData: any;
  // @Input() formData: any;
  @Input() flowForm: any;
  @Output() closePanel = new EventEmitter<string>();

  loading!: boolean;
  fileHost: string = environment.FileServer;
  currentLanguage: any = 'en';
  //权限申请
  tip: string = 'Tip';
  formData: any = {
    invalid: false,
    applicant: '',
    applicantName: '',
    reason: '',
  }
  defaultFlowForm: any = {
    enabledUpload: false,
    attachmentPanel: {
      show: false,
    },
    validFields: ['applicant', 'reason'],
    approveList: [
      {
        childList: [
          {
            record: { reason: '' },
          },
        ],
      },
    ],
    beforeSubmit: (formData: any) => {
    },
    AfterSubmit: (formData: any) => {
    },
  }
  userInfo: any;

  hasPromise: boolean = false;
  isApprove: boolean = false;
  remarkValue: string = "";


  constructor(
    private http: WorkflowHttpService,
    private router: Router,
    private translate: TranslateService,
    private messageService: MessageService,
    private commonService: CommonService,
    private itCommonService: CommonHttpService,
    private confirmationService: ConfirmationService
  ) {
  }



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
  public async ngOnInit() {
    this.currentLanguage = localStorage.getItem('lang');
    this.tip = this.currentLanguage == 'en' ? 'Tip' : '提示';
    this.translate.use(this.currentLanguage);
    Object.assign(this.flowForm, this.defaultFlowForm);
    this.initRoute();
  }

  async initRoute() {
    this.userInfo = await this.itCommonService.getCurrUser();
    this.hasPromise = this.userInfo.roles?.length > 0;
    if (this.flowData.id && this.flowData.auditRecords) {
      this.isApprove = true;
      if (this.flowData.auditRecords.length > 0) {
        this.remarkValue = this.flowData.auditRecords[this.flowData.auditRecords.length - 2].auditContent;
      }
    }
    else {
      await this.getUserInfo();
      await this.getCostCenterFMByUserId();
      this.http.getMyRequestsList({ pageNum: 1, pageSize: 1 }).then((res: any) => {
        if (res.data.total > 0) {
          this.http.getFlowFormDetails(res.data.data[0].id).then((res: any) => {
            if (res.data.flowTempName == "ITPortal_UserPermissionApplication") {
              this.flowData = Object.assign(this.flowData, res.data);
              // this.formData = Object.assign(this.formData, JSON.parse(this.flowData.formData));
              if (this.flowData.auditRecords.length > 0) {
                this.remarkValue = this.flowData.auditRecords[this.flowData.auditRecords.length - 2].auditContent;
              }
              this.flowForm.changeFlowData(this.flowData);
            }
          });
        }
      });
    }
  }


  // 当前登录用户信息
  async getUserInfo() {
    const data = await this.itCommonService.getCurrUser();
    this.userInfo = data;
    this.formData.applicant = data.ntid;
    this.formData.applicantName = data.name;
    this.formData.department = data.department;
    this.formData.email = data.email;
  }
  // 获取function主管名
  getCostCenterFMByUserId() {
    this.itCommonService.GetFunctionManager(this.userInfo.id).then((res: any) => {
      if (res.success) {
        if (res.data) {
          this.formData.functionSuperior = res.data.workNTID
          this.formData.functionSuperiorName = res.data.name;
          this.formData.functionSuperiorEMail = res.data.workEmail;
          //设置审批节点
          this.flowForm.setActApprover("DY Approval", [{
            ntid: res.data.workNTID,
            name: res.data.name,
            email: res.data.workEmail,
          }]).bind(this.flowData)
        } else {
          this.messageService.add({
            severity: 'warn',
            summary: localStorage.getItem('lang') == 'en' ? 'Tip' : '提示',
            detail:
              localStorage.getItem('lang') == 'en'
                ? 'Unable to find superior, please complete personal information'
                : '找不到上级,请完善个人信息',
          });
        }
      } else {
        this.messageService.add({
          severity: 'error',
          summary: localStorage.getItem('lang') == 'en' ? 'Error' : '错误',
          detail: res.msg,
        });
      }
    });
  }

  goBack() {
    this.router.navigate(['/'], { queryParams: { ts: new Date().getTime() } });
  }



  logout() {
    localStorage.clear();
    window?.parent?.location?.reload()
    window.location.reload()
  }

  reApply() {
    this.isApprove = false;
    this.formData.reason = "";
    delete this.flowData.id;
    this.flowData.flowNo = "";
    this.flowData.flowStepName = null;
    this.flowData.flowStatusName = null;
    this.flowData.flowStatus = 0;
    this.flowData.taskSubject = null;
    this.flowData.createTime = null;
    this.remarkValue = "";
    this.flowData.formData = JSON.stringify(this.formData);
  }
}
