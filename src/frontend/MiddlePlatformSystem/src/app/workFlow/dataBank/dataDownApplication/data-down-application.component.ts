import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { BigFileUploadService, CommonService, LocalStorage } from 'jb.package/jabil-bus-lib';
import { NzUploadChangeParam, NzUploadFile } from 'ng-zorro-antd/upload';
import { ConfirmationService, MessageService } from 'primeng/api';
import CommonHttpService from 'src/api/common';
import DataSourceHttpService from 'src/api/dataAssetManage/dataSource';
import DataTableHttpService from 'src/api/dataAssetManage/dataTable';
import HomeHttpService from 'src/api/home';
import WorkflowHttpService from 'src/api/home/workflow';
import { TranslateData } from 'src/app/core/translate/translate-data';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'dataBank-dataDown-apply',//it-portal-datasource-apply app-data-down-application
  templateUrl: './data-down-application.component.html',
  styleUrls: ['./data-down-application.component.scss'],
  providers: [
    LocalStorage,
    MessageService,
    ConfirmationService,
    CommonService,
    BigFileUploadService,
    HomeHttpService,
    WorkflowHttpService,
    CommonService,
    DataSourceHttpService,
    DataTableHttpService
  ],
})
export class DataDownApplicationComponent implements OnInit {
  @Input() fields: any;
  @Output() closePanel = new EventEmitter<string>();

  loading!: boolean;
  fileHost: string = environment.FileServer;
  token: string = ''
  currentLanguage: any = 'en';
  flowData: any = {
    applicant: '',
    applicantName: '',
    flowTempName: "ITPortal_DataOffline",
    ActApprovers: [],
    formData: {}
  };
  formData: any = {
    formNo: '',
    userId: '',
    userName: '',
    userMail: '',
    changeType: '',
    sourceId: '',
    tableName: '',
    ownerId: '',
    ownerName: '',
    changePurpose: '',
    descrption: '',
    superior: '',
    superiorName: '',
  };
  invalid: boolean = false;
  currFileList: Array<any> = [];
  userInfo: any = {};
  approveList: any[] = [
    {
      childList: [
        {
          record: { reason: '' },
        },
      ],
    },
  ];
  approvalLevelInfo: any = {};
  tip: string = 'Tip';
  datasourceTitle: string = 'datasource.application';
  sourcesystemsmeList: any[] = [
  ];
  historyHeader: any[] = [
    { width: '5%', field: 'index', header: 'No' },
    { width: '20%', field: 'actTitle', header: 'datasource.actName' },
    { width: '10%', field: 'approverName', header: 'datasource.doneby' },
    { width: '10%', field: 'actOperate', header: 'datasource.status' },
    { width: '10%', field: '_createTime', header: 'datasource.datetime' },
    { width: '45%', field: 'auditContent', header: 'datasource.comment' },
  ];
  historyData: any[] = [];
  approvalData: any = { dialogVisible: false, flowInstId: '', acttype: '', actOperate: '', auditContent: '' };//审批对象
  remarkValue: string = '';
  isStop: boolean = false;
  changeTypeList: any[] = [{ label: 'datasource.changeType.change', value: 1 }, { label: 'datasource.changeType.offline', value: 2 }];

  // 下拉数据源
  sourceList: Array<any> = [];
  tableList: Array<any> = [];
  source: any = { dbType: '', status: '' };
  urlParams: any = {};
  workflowType: string = '';

  typeInfo: any = {
    "agree": this.currentLanguage == 'en' ? 'agree' : '同意',
    "reject": this.currentLanguage == 'en' ? 'reject' : '拒绝',
    "transfer": this.currentLanguage == 'en' ? 'transfer' : '转办',
    "return": this.currentLanguage == 'en' ? 'return' : '退回',
    "cancel": this.currentLanguage == 'en' ? 'cancel' : '取消',
  };

  constructor(
    private http: WorkflowHttpService,
    private sourceService: DataSourceHttpService,
    private tableService: DataTableHttpService,
    private translate: TranslateService,
    private messageService: MessageService,
    private commonFunction: CommonService,
    private itCommonService: CommonHttpService,
    private confirmationService: ConfirmationService
  ) {
    this.token = localStorage.getItem('jwt') || ''

    this.urlParams = this.itCommonService.getUrlParams(window.location.hash)
    if (this.urlParams['flowtempname']) {
      this.flowData.flowTempName = this.workflowType = this.urlParams['flowtempname'];
    }
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


    this.initSelectData();

    if (this.fields?.id)
      return this.getDetail(this.fields.id);
    else if (this.fields?.taskid)
      return this.getDetail(this.fields.taskid, 'todo');

    this.typeInfo = {
      "agree": this.currentLanguage == 'en' ? 'agree' : '同意',
      "reject": this.currentLanguage == 'en' ? 'reject' : '拒绝',
      "transfer": this.currentLanguage == 'en' ? 'transfer' : '转办',
      "return": this.currentLanguage == 'en' ? 'return' : '退回',
      "cancel": this.currentLanguage == 'en' ? 'cancel' : '取消',
    };
  }


  async initSelectData() {
    this.getUserInfo();
    // this.getUserList();
    this.getDataTables();
    this.getSource();

    this.changeTypeList.forEach((item: any) => {
      item.label = this.translate.instant(item.label);
    });

  }

  async getSource() {
    let { data } = await this.sourceService.list({});
    this.sourceList = data;
  }


  // 用户列表
  getUserList() {
    if (sessionStorage.getItem('userList')) {
      let userListString: any = sessionStorage.getItem('userList');
      let userList = JSON.parse(userListString);
      this.sourcesystemsmeList = userList;
    } else {
      this.loading = true;
      this.itCommonService.getUserList().then((res: any) => {
        let arr = res.data;
        const uniqueArr = Array.from(
          new Map(arr.map((item: any) => [item['id'], item])).values()
        );
        this.sourcesystemsmeList = uniqueArr;
        sessionStorage.setItem('id', JSON.stringify(this.sourcesystemsmeList));
        this.loading = false;
      });
    }
  }
  // 当前登录用户信息
  async getUserInfo() {
    const data = await this.itCommonService.getCurrUser();
    this.userInfo = data;
    this.formData.ownerId = this.formData.userId = this.userInfo.id;
    this.formData.ownerName = this.formData.userName = this.userInfo.name;// `${this.userInfo.englishName}(${this.userInfo.id})`;
    this.formData.userMail = this.userInfo.email;
    this.getMasterByUserId();
  }
  // 详情
  async getDetail(id: string, type: string = 'flow') {
    this.loading = true;
    let res = await this.http.getFlowFormDetails(id, type);
    if (res.success) {
      this.flowData = res.data;
      this.flowData.formData = this.flowData.formData ?? {};
      this.formData = JSON.parse(this.flowData.formData);
      this.formData.id = this.flowData.id;
      this.datasourceTitle = !this.formData.id ? 'label.datasource.application' : 'label.datasource.approval';
      if (this.flowData.auditRecords && this.flowData.auditRecords.length > 0) {
        this.historyData = this.flowData.auditRecords.map((item: any, index: number) => {
          item.index = index + 1;
          item._createTime = this.formatDateTime(item.createTime);
          return item;
        });
      }
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


  // 申请按钮
  async onCommit() {
    let _this = this;
    _this.invalid = false;
    if (!this.isStop) {
      return this.messageService.add({
        severity: 'warn',
        summary: localStorage.getItem('lang') == 'en' ? 'Tip' : '提示',
        detail:
          localStorage.getItem('lang') == 'en'
            ? 'Unable to find superior, please complete personal information'
            : '找不到上级,请完善个人信息',
      });
    }
    let params = { ...this.formData };
    const valid = ['sourceId', 'userName', 'userMail', 'changeType', 'tableName', 'ownerId', 'changePurpose'];
    if (this.commonFunction.isInvalid(params, valid)) {
      this.invalid = true;
      return;
    } else {
      this.loading = true;
      await this.submitApply(params);
      // params.masterData = JSON.stringify(params.masterData);
      // if (!params.id) {
      //   this.http.startFlow(params).then((res: any) => {
      //     if (res.success) {
      //       this.commonSuccessFunc();
      //       setTimeout(() => {
      //         this.loading = false;
      //         this.goBack();
      //       }, 500);
      //     } else {
      //       this.loading = false;
      //       this.commonErrorFunc('Error', '错误', res.msg);
      //     }
      //   });
      // } else {
      //   this.http.startFlow(params).then((res: any) => {
      //     if (res.success) {
      //       this.commonSuccessFunc();
      //       setTimeout(() => {
      //         this.loading = false;
      //         this.goBack();
      //       }, 500);
      //     } else {
      //       this.loading = false;
      //       this.commonErrorFunc('Error', '错误', res.msg);
      //     }
      //   });
      // }
    }
  }


  submitApply(data: any) {
    this.flowData.formData = data;
    this.flowData.applicant = this.userInfo.id;
    this.flowData.applicantName = this.userInfo.name;
    this.flowData.ActApprovers = [{
      ActStep: '2',
      ActName: "Superior Approval",
      ActTitle: "主管审核",
      ActorParms: [{
        "Ntid": data.superior,
        "Name": data.superiorName,
        "Email": data.email,
        "Department": data.dept,
        "PhoneNumber": ""
      }],
      ActorParmsName: data.superiorName
    }];
    this.http.startFlow(this.flowData).then((res: any) => {
      if (res.success) {
        this.commonSuccessFunc();
        setTimeout(() => {
          this.loading = false;
          this.goBack();
        }, 500);
      } else {
        this.loading = false;
        this.commonErrorFunc('Error', '错误', res.msg);
      }
    });
  }

  // 取消按钮
  onCancelAsync() {
    this.confirmationService.confirm({
      message:
        this.currentLanguage == 'en'
          ? `May I ask whether you are sure to cancel the approval of this article ?`
          : `请问是否确定要取消该条审批？`,
      rejectLabel: localStorage.getItem('lang') == 'en' ? 'No' : '否',
      acceptLabel: localStorage.getItem('lang') == 'en' ? 'Yes' : '是',
      accept: async () => {
        this.loading = true;
        const data = {
          flowInstId: this.formData.id,
          acttype: 'cancel',
          actOperate: 'cancel',
          auditContent: this.remarkValue,
        };
        const res = await this.http.sendApprove(data);
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
  // 审批按钮
  async submit(type: string) {

    if (this.approvalData.dialogVisible) {
      if (type == "transfer" && !this.approvalData.transferor) {
        return this.messageService.add({
          severity: 'warn',
          summary: localStorage.getItem('lang') == 'en' ? 'Tip' : '提示',
          detail:
            localStorage.getItem('lang') == 'en'
              ? 'Please fill in the transferor.'
              : '请填写转办人',
        });
      }
      if (!this.approvalData.auditContent) {
        // return this.messageService.add({ severity: 'warn', summary: TranslateData.fillComment ?? 'Please fill in the comment' });
        return this.messageService.add({
          severity: 'warn',
          summary: localStorage.getItem('lang') == 'en' ? 'Tip' : '提示',
          detail:
            localStorage.getItem('lang') == 'en'
              ? `Please fill in the ${this.typeInfo[type]} comments.`
              : `请填写${this.typeInfo[type]}意见`,
        });
      }
    }


    this.confirmationService.confirm({
      message:
        this.currentLanguage == 'en'
          ? `May I ask if you are sure you want the approval for ${this.typeInfo[type]}`
          : `请问是否确定要${this.typeInfo[type]}该条审批`,
      rejectLabel: localStorage.getItem('lang') == 'en' ? 'No' : '否',
      acceptLabel: localStorage.getItem('lang') == 'en' ? 'Yes' : '是',
      accept: async () => {
        this.loading = true;
        this.approvalData.dialogVisible = false;
        const data = {
          flowInstId: this.formData.id,
          acttype: type,
          actOperate: type,
          auditContent: this.approvalData.auditContent,
          transferor: this.approvalData.transferor,
          transferorName: this.approvalData.transferorName,
        };
        const res = await this.http.sendApprove(data);
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
  goBack() {
    this.closePanel.emit();
  }

  // 文件流操作
  handleChange({ file, fileList }: NzUploadChangeParam): void {
    const status = file.status;
    if (status === 'done') {
      this.currFileList = fileList;
      this.messageService.add({ severity: 'success', summary: TranslateData.fileUploadSuccess });
    } else if (status === 'removed') {
      this.currFileList = fileList;
      this.messageService.add({ severity: 'success', summary: TranslateData.deleteSuccess });
    } else if (status === 'error') {
      file.error.statusText = TranslateData.fileUploadFail;
      this.messageService.add({ severity: 'error', summary: TranslateData.fileUploadFail });
    }
  }

  deleteFile(item: any, i: number) {
    this.currFileList.splice(i, 1);
    this.messageService.add({ severity: 'success', summary: TranslateData.deleteSuccess });
  }

  handleDownloadFile = async (file: NzUploadFile): Promise<void> => {
    if (file?.originFileObj) {
      window.open(URL.createObjectURL(file?.originFileObj), '_blank');
    } else {
      this.download({
        fileUrl: file['fileUrl'],
        fileName: file['fileName'],
      });
    }
  };
  onDownload() {
    this.http.itexportList().then((res: any) => { });
  }
  onOtherDownload() {
    this.http.itexportOtherList().then((res: any) => { });
  }
  downloadTemplate() {
    this.download({
      fileUrl: 'a2c6ca06-a2a3-46f1-b3e4-9fab187e53cf.xlsx',
      fileName: 'DataSourceTemplate.xlsx',
    });
    return false;
  }
  download(item: any) {
    let nameList = item.fileName.split('.');
    let name = nameList[0];
    this.getBlob(item.fileUrl).then(blob => {
      this.saveAs(blob, name);
    });
  }
  getBlob(url: string) {
    return new Promise(resolve => {
      const xhr = new XMLHttpRequest();
      xhr.open('GET', this.fileHost + "/api/file/download/" + url + "?category=DataSourceFlow", true);
      xhr.setRequestHeader('Accept', 'application/pdf');
      xhr.responseType = 'blob';
      xhr.onload = () => {
        if (xhr.status === 200) {
          resolve(xhr.response);
        }
      };
      xhr.send();
    });
  }
  saveAs(blob: any, filename: string) {
    const link = document.createElement('a');
    link.href = window.URL.createObjectURL(blob);
    link.download = filename + '.xlsx';
    link.click();
  }
  // 获取主管名
  getMasterByUserId() {
    this.http.getMasterByUserId(this.userInfo.id).then((res: any) => {
      if (res.success) {
        if (res.data) {
          this.formData.superior = res.data.workNTID
          this.formData.superiorName = `${res.data.englishName}(${res.data.workNTID})`
          this.formData.dept = `${res.data.departmentName}`
          this.formData.email = `${res.data.workEmail}`
          this.isStop = true;
        } else {
          this.isStop = false;
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

  IsApprover() {
    var flag = this.flowData.approver && this.flowData.approver.some((item: any) => {
      return item.ntid.toLowerCase() === this.userInfo.id.toLowerCase()
    })
    return flag;
  }
  async openApproveDialog(acttype: string) {
    this.approvalData.flowInstId = this.flowData.id;
    this.approvalData.acttype = acttype;
    this.approvalData.dialogVisible = true;
    this.approvalData.auditContent = '';
    this.approvalData.transferor = '';
    if (acttype == 'transfer') {
      this.getUserList();
    }
  }
  onSourceChange(e: any) {
    this.getDbTables(e.value);
    //查询这条源数据
    this.setSourceValue(e.value);
  }


  // 赋值source
  setSourceValue(id: string) {
    this.sourceService.getById(id).then((res: any) => {
      res.data.status = res.data.status.toString()
      res.data.dbType = res.data.dbType.toString()
      this.source = res.data;
    });
  }

  //新增的时候，查询源库
  getDbTables(sourceId: any) {
    this.sourceService.getDbTablesMergeLocal(sourceId).then((res: any) => {
      this.tableList = res.data.filter((item: any) => item.type == 1);
    });
  }

  //新增的时候，查询源库
  getDataTables() {
    this.tableService.GetOwnerTables({ PageNum: 1, PageSize: 3000, }).then((res: any) => {
      if (res.success)
        this.tableList = res.data.data;
    });
  }
  onChangeTable(e: any) {
    let items: any = this.tableList.filter((citme: any) => citme.tableName == e.value);
    if (items.length > 0 && items[0].sourceName) {
      this.formData.sourceId = items[0].sourceId;
      this.formData.tableComment = items[0].tableComment;
      this.formData.visitedTimes = items[0].visitedTimes;
      this.formData.sourceName = items[0].sourceName;
      this.formData.tableId = items[0].id;
      this.formData.ownerId = items[0].ownerId;
      this.formData.ownerName = items[0].ownerName;
      this.formData.ownerDept = items[0].ownerDept;
    }
  }
}
