import { Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { MessageService, ConfirmationService } from 'primeng/api';
import { Router, ActivatedRoute } from '@angular/router';
import { CommonService } from 'jabil-bus-lib';
import { TranslateData } from 'src/app/core/translate/translate-data';
import DataTableHttpService from 'src/api/dataAssetManage/dataTable';
import AuthorityService from 'src/api/dataAssetManage/authority';
import CommonHttpService from 'src/api/common';
import { AssetQueryDataInfoComponent } from './dataInformation'
import { AnyNaptrRecord } from 'node:dns';
import WorkflowHttpService from 'src/api/home/workflow';
import { isCheckDisabled } from 'ng-zorro-antd/core/tree';
import DictHttpService from 'src/api/common/dict';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-asset-query-permission',
  templateUrl: './index.html',
  styleUrls: ['./index.scss'],
  providers: [
    MessageService,
    DataTableHttpService,
    CommonService,
    CommonHttpService,
    AuthorityService,
    DictHttpService
  ],
})
export class AssetQueryPermissionComponent implements OnInit {
  @ViewChild('dataInfo') dataInfo!: AssetQueryDataInfoComponent;
  @Output() goBack = new EventEmitter<string>();
  @Input() field: any;

  // 下拉数据源
  loading: boolean = false;
  currentLanguage: any = 'en';
  tip: string = '提示';
  formData: any = {
    applyType: 2,
    appId: '',
    appName: '',
    reason: '',
    userId: '',
    userName: '',
    userType: '',
    tableList: [],
    invalid: false
  };
  userInfo: any = {}
  appList: any[] = []
  useTypeList: any[] = []
  titleName: string = 'add'
  remarkValue: string = ''
  detailData: any = null
  flowData: any = {}
  allSelectColumns: any[] = []
  historyHeader: any[] = [
    { width: '5%', field: 'index', header: 'No' },
    { width: '15%', field: 'actTitle', header: 'datasource.actName' },
    { width: '25%', field: 'approverName', header: 'datasource.doneby' },
    { width: '10%', field: 'actOperate', header: 'datasource.status' },
    { width: '10%', field: '_createTime', header: 'datasource.datetime' },
    { width: '45%', field: 'auditContent', header: 'datasource.comment' },
  ];
  historyData: any[] = [];
  urlParams: any = {}
  workflowType: string = ''
  constructor(
    private messageService: MessageService,
    private route: ActivatedRoute,
    public router: Router,
    // private translate: TranslateService,
    private commonService: CommonService,
    private tableService: DataTableHttpService,
    private commonHttpService: CommonHttpService,
    private flowhttp: WorkflowHttpService,
    private http: AuthorityService,
    private confirmationService: ConfirmationService,
    private dictService: DictHttpService,
    private lang: TranslateService
  ) {
    this.urlParams = this.commonHttpService.getUrlParams(window.location.hash)
    if (this.urlParams['flowtempname']) {
      this.workflowType = this.urlParams['flowtempname'];
    }
    this.currentLanguage = localStorage.getItem('lang') || 'en'
    this.getApplication()
    this.getCurrUser()
  }

  ngOnInit(): void {
    this.tip = this.currentLanguage == 'en' ? 'Tip' : '提示';
    this.getDictList();
    if (this.urlParams['type'] == "email") {
      this.field = {
        flowTempName: this.workflowType,
        id: this.urlParams['id'],
        taskid: this.urlParams['taskid'],
      }
    }
    if (this.field.flowTempName) {
      debugger
      if (this.field.id)
        this.getDetail(this.field.id);
      else if (this.field.taskid)
        this.getDetail(this.field.taskid, 'todo');
    } else {
      this.checkAuth()
    }
  }

  // 详情
  async getDetail(id: string, type: string = 'flow') {
    this.loading = true;
    let res = await this.flowhttp.getFlowFormDetails(id, type);
    if (res.success) {
      this.loading = false;
      this.flowData = res.data;
      console.log(res.data.formData)
      this.formData = JSON.parse(res.data.formData);
      console.log(this.formData)
      this.detailData = { columnList: this.formData.columnList || [], isDisabled: true };
      if (this.flowData.auditRecords && this.flowData.auditRecords.length > 0) {
        this.historyData = this.flowData.auditRecords.map((item: any, index: number) => {
          item.index = index + 1;
          item._createTime = this.formatDateTime(item.createTime);
          return item;
        });
      }
    } else {
      this.loading = false;
      return this.messageService.add({
        severity: 'error',
        summary: localStorage.getItem('lang') == 'en' ? 'Error' : '错误',
        detail: res.msg,
      });
    }
  }

  async checkAuth() {
    this.allSelectColumns = []
    this.field.tableColumns.forEach((item: any) => {
      this.allSelectColumns.push(item.id)
    })
    this.getDetailData()
  }

  getDetailData() {
    if (this.field.id) {
      // 初始化数据
      this.tableService.getById(this.field.id).then((res: any) => {
        if (!this.field.ownerDepart) this.field.ownerDepart = res.data.ownerDepart;
        res.data.columnList.forEach((item: any) => {
          item.unSet = this.allSelectColumns.length === 0 ? false : this.allSelectColumns.includes(item.id)
        })

        this.detailData = res.data;
      });
    }
  }

  async getCurrUser() {
    try {
      this.userInfo = JSON.parse(localStorage.getItem('loginuser') || '')
      this.formData.userId = this.userInfo.id
      this.formData.userName = this.userInfo.name
    } catch (e) {
      console.log(e)
    }
  }

  async getApplication() {
    const res = await this.commonHttpService.getApplication()
    this.appList = res.data
  }

  selectApp() {
    const data = this.appList.find((item: any) => {
      return this.formData.appId === item.clientId
    })
    this.formData.appName = data.clientName
    this.formData.useType = data.useType
  }

  //提交
  submit() {
    // 获取数据信息选中数据
    this.formData.tableList = [this.field]
    this.formData.tableList[0].columnList = this.dataInfo.selectedItems;
    const valid = this.formData.applyType === 2 ? ['appId', 'reason', 'useType'] : ['reason','useType'];
    if (this.commonService.isInvalid(this.formData, valid)) {
      this.formData.invalid = true;
      // @ts-ignore
    } else {
      this.loading = true;
      this.http.applyAPIColumn(this.formData).then((resUpdate: any) => {
        if (resUpdate.code == 200) {
          this.messageService.add({
            severity: 'success',
            summary: TranslateData.save,
            detail: TranslateData.success,
          });
          setTimeout(() => {
            this.loading = false;
            this.backClick();
          }, 1000);
        } else {
          this.loading = false;
          this.messageService.add({
            severity: 'error',
            summary: TranslateData.fail,
            detail: resUpdate.msg,
          });
        }
      });
    }
  }


  // 审批按钮
  async sendApprove(type: string) {
    let typeInfo: any = {
      "agree": this.currentLanguage == 'en' ? 'agree' : '同意',
      "reject": this.currentLanguage == 'en' ? 'reject' : '拒绝',
      "transfer": this.currentLanguage == 'en' ? 'transfer' : '转办',
      "return": this.currentLanguage == 'en' ? 'return' : '退回',
      "cancel": this.currentLanguage == 'en' ? 'cancel' : '取消',
    };
    if (!this.remarkValue) {
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
          flowInstId: this.formData.id,
          acttype: type,
          actOperate: type,
          auditContent: this.remarkValue,
        };
        const res = await this.flowhttp.sendApprove(data);
        if (res.success) {
          this.loading = false;
          this.messageService.add({
            severity: 'success',
            summary: TranslateData.saveSuccess,
          });
          this.getDetail(this.formData.id);
        } else {
          this.loading = false;
          this.commonErrorFunc('Error', '错误', res.msg);
        }
      },
    });
  }

  commonErrorFunc(errorE: string = 'Error', errorZ: string = '错误', msg: any = '') {
    this.messageService.add({
      severity: 'error',
      summary: localStorage.getItem('lang') == 'en' ? errorE : errorZ,
      detail: msg,
    });
  }
  backClick() {
    this.goBack.emit()
  }
  formatDateTime(isoStr: string) {
    if (isoStr == null || isoStr == '0001-01-01 00:00:00' || isoStr == '0001-01-01T00:00:00' || isoStr == '1970-01-01 08:00:00') return '';
    const date = new Date(isoStr);
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    const hour = String(date.getHours()).padStart(2, '0');
    const minute = String(date.getMinutes()).padStart(2, '0');
    const second = String(date.getSeconds()).padStart(2, '0');
    return `${year}-${month}-${day} ${hour}:${minute}:${second}`;
  }

  IsApprover() {
    var flag = this.flowData.approver && this.flowData.approver.some((item: any) => {
      return item.ntid.toLowerCase() === this.userInfo.id.toLowerCase()
    })
    return flag;
  }
  async getDictList() {
    let res = await this.dictService.codes('DataUseType');
    if (res.success) {
      this.useTypeList = res.data.filter((f: any) => f.dictCode == 'DataUseType');
      this.useTypeList.forEach((f: any) => {
        if (f.itemText && f.itemTextEn) {
          this.lang.get("dict." + f.dictCode + "." + f.itemValue).subscribe((res: string) => {
            if (res != "dict." + f.dictCode + "." + f.itemValue) f.itemText = res;
            else if (this.currentLanguage == 'en') f.itemText = f.itemTextEn;
          });
        } else {
          this.lang.get("dict." + f.dictCode + "." + f.itemValue).subscribe((res: string) => {
            if (res != "dict." + f.dictCode + "." + f.itemValue) f.itemText = res;
            else if (this.currentLanguage == 'en') f.itemText = f.itemValue;
          });
        }
      });
    }
  }
}
