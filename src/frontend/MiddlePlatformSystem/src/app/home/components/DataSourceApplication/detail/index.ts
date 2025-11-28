import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { MessageService, ConfirmationService } from 'primeng/api';
import { CommonService, BigFileUploadService, LocalStorage } from 'jabil-bus-lib';
import { TranslateData } from 'src/app/core/translate/translate-data';
import WorkflowHttpService from 'src/api/home/workflow';
import HomeHttpService from 'src/api/home';
import { NzUploadChangeParam, NzUploadFile } from 'ng-zorro-antd/upload';
import CommonHttpService from 'src/api/common';
import { arrayBuffer } from 'node:stream/consumers';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'it-portal-datasource-apply',//jabil-all-data-grant-apply
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
export class ITDatasourceComponent implements OnInit {
  @Input() fields: any;
  @Output() closePanel = new EventEmitter<string>();

  loading!: boolean;
  fileHost: string = environment.FileServer;
  token: string = ''
  currentLanguage: any = 'en';
  flowData: any = {};
  formData: any = {
    id: null,
    formNo: '',
    userId: '',
    userName: '',
    userMail: '',
    dataName: '',
    dataSourse: '',
    syncType: '',
    updateMethod: '',
    syncFrequency: '',
    superior: '',
    superiorName: '',
    sMEUserId: '',
    sMEUserName: '',
    bASUserId: '',
    bASUserName: '',
    // masterData: '',
    masterData: [{ masterData: null, sourceSystem: '', needStandard: null, isMapping: null }],
    applyPurpose: '',
    // smeList: [],
    // bsaList: [],
    fileList: [],
    // otherFileList: [],
  };
  invalid: boolean = false;
  currFileList: Array<any> = [];
  // currOtherFileList: Array<any> = [];
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
    // {
    //   label: 'cs',
    //   value: '1456879',
    // },
  ];
  // sourcesystemsmeSelectList: any[] = [];
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
  // dataSouceApproverInfo: string = '';
  isStop: boolean = false;

  masterDataList: any[] = [
    { name: 'Bay', sourse: 'MDM', sort: 1 },
    { name: 'Equipment', sourse: 'EAM', sort: 2 },
    { name: 'Employee', sourse: 'Workday', sort: 3 },
    { name: 'Location', sourse: 'MDM', sort: 4 },
    { name: 'Organization', sourse: 'MDM', sort: 5 },
    { name: 'Function Group', sourse: 'MDM', sort: 6 },
    { name: 'Vendor', sourse: 'SAP', sort: 7 },
    { name: 'Station', sourse: 'Flow Chart', sort: 8 },
    { name: 'Customer', sourse: 'My Analytics', sort: 9 },
    { name: 'Material', sourse: 'SAP', sort: 10 },
    { name: 'Sold to', sourse: 'SAP', sort: 11 },
    { name: 'Ship to', sourse: 'SAP', sort: 12 },
    { name: 'Profit Center', sourse: 'SAP', sort: 13 },
    { name: 'Cost Center', sourse: 'SAP', sort: 14 },
    { name: 'Defect Code', sourse: 'MES', sort: 15 },
    { name: 'Skills', sourse: 'Igrow', sort: 16 },
  ];
  yesOrNoList: any[] = [{ label: 'datasource.yesOrNo.yes', value: true }, { label: 'datasource.yesOrNo.no', value: false }];
  //同步类型:全量，增量
  syncTypeList: any[] = [{ label: 'datasource.syncType.full', value: 1 }, { label: 'datasource.syncType.increment', value: 2 }];
  //手动更新 or 自动更新
  updateMethodList: any[] = [{ label: 'datasource.updateMethod.manual', value: 1 }, { label: 'datasource.updateMethod.auto', value: 2 }];

  constructor(
    private http: WorkflowHttpService,
    private homeHttpService: HomeHttpService,
    private translate: TranslateService,
    private messageService: MessageService,
    private commonFunction: CommonService,
    private itCommonService: CommonHttpService,
    private confirmationService: ConfirmationService
  ) {
    this.token = localStorage.getItem('jwt') || ''
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

    this.yesOrNoList.forEach((item: any) => {
      item.label = this.translate.instant(item.label);
    });
    this.syncTypeList.forEach((item: any) => {
      item.label = this.translate.instant(item.label);
    });
    this.updateMethodList.forEach((item: any) => {
      item.label = this.translate.instant(item.label);
    });
    await this.getUserInfo();
    this.getUserList();

    if (this.fields?.id)
      return this.getDetail(this.fields.id);
    else if (this.fields?.taskid)
      return this.getDetail(this.fields.taskid, 'todo');
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
    this.formData.userId = this.userInfo.id;
    this.formData.userName = this.userInfo.name;// `${this.userInfo.englishName}(${this.userInfo.id})`;
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
      if (!this.formData.bASUserId && this.formData.basUserId) {
        this.formData.bASUserId = this.formData.basUserId;
        this.formData.bASUserName = this.formData.basUserName;
      }
      this.datasourceTitle = !this.formData.id ? 'label.datasource.application' : 'label.datasource.approval';
      if (this.flowData.auditRecords && this.flowData.auditRecords.length > 0) {
        this.historyData = this.flowData.auditRecords.map((item: any, index: number) => {
          item.index = index + 1;
          item._createTime = this.formatDateTime(item.createTime);
          return item;
        });
      }
      // this.sourcesystemsmeSelectList = [this.formData.sMEUserId];// this.formData.smeList.map((item: any) => item.userId);
      // this.sourcesystemsmeList.sort((a, b) => {
      //   let aIndex = this.sourcesystemsmeSelectList.indexOf(a.workNTID) > -1 ? -1 : 0;
      //   let bIndex = this.sourcesystemsmeSelectList.indexOf(b.workNTID) > -1 ? -1 : 0;
      //   return aIndex - bIndex;
      // });
      this.currFileList = this.flowData.attacchments.map((item: any) => {
        item.showDownload = true;
        item.status = 'done';
        item.uid = item.id;
        item.isBack = true;
        return item;
      });
      this.formData.fileList = this.flowData.attacchments.map((item: any) => {
        item.showDownload = true;
        item.status = 'done';
        item.name = item.fileName;
        item.uid = item.id;
        return item;
      });

      // this.currOtherFileList = data.otherFileList.map((item: any) => {
      //   item.showDownload = true;
      //   item.status = 'done';
      //   item.uid = item.id;
      //   item.isBack = true;
      //   return item;
      // });
      // this.formData.otherFileList = this.formData.otherFileList.map((item: any) => {
      //   item.showDownload = true;
      //   item.status = 'done';
      //   item.name = item.fileName;
      //   item.uid = item.id;
      //   return item;
      // });
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
  // // sme选择
  // onChangeUser(e: any) {
  //   if (!e.value) return;
  //   this.formData.smeList = [];
  //   let arr: any[] = e.value;
  //   this.sourcesystemsmeList.forEach((item: any) => {
  //     arr.forEach((citem: any) => {
  //       if (item.workNTID == citem) {
  //         this.formData.smeList.push({
  //           userId: item.workNTID,
  //           userName: item.englishName,
  //         });
  //       }
  //     });
  //   });
  //   this.formData.smeList = Array.from(
  //     new Map(this.formData.smeList.map((item: any) => [item['workNTID'], item])).values()
  //   );
  // }

  // // bsa选择
  // onChangeBasUser(e: any) {
  //   if (!e.value) return;
  //   // this.formData.bsaList = [];
  //   // let arr: any[] = e.value;
  //   // this.sourcesystemsmeList.forEach((item: any) => {
  //   //   arr.forEach((citem: any) => {
  //   //     if (item.workNTID == citem) {
  //   //       this.formData.bsaList.push({
  //   //         userId: item.workNTID,
  //   //         userName: item.englishName,
  //   //       });
  //   //     }
  //   //   });
  //   // });
  //   // this.formData.bsaList = Array.from(
  //   //   new Map(this.formData.bsaList.map((item: any) => [item['workNTID'], item])).values()
  //   // );
  // }

  onPanelHide() {
    // this.sourcesystemsmeList.sort((a, b) => {
    //   let aIndex = this.sourcesystemsmeSelectList.indexOf(a.workNTID) > -1 ? -1 : 0;
    //   let bIndex = this.sourcesystemsmeSelectList.indexOf(b.workNTID) > -1 ? -1 : 0;
    //   return aIndex - bIndex;
    // });
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
    params.fileList = [];
    this.currFileList.forEach((item: any) => {
      if (item.isBack) {
        params.fileList.push(item);
      } else {
        params.fileList.push(item?.response?.data);
      }
    });

    this.formData.masterData.forEach((item: any) => {
      if (item.masterData) {
        if (!item.dataSource) {
          _this.invalid = true;
          return;
        }
        if (item.needStandard && (item.isMapping === "" || item.isMapping === null)) {
          _this.invalid = true;
          return;
        }
      }
    });
    if (this.invalid === true)
      return;

    // params.smeList = this.sourcesystemsmeSelectList;
    // params.userId = this.userInfo.id;
    // params.userName = this.userInfo.userName;
    params.comment = this.remarkValue;
    const valid = ['dataSourse', 'syncFrequency', 'syncType', 'updateMethod', 'applyPurpose', 'smeUserId', 'bASUserId'];
    if (this.commonFunction.isInvalid(params, valid)) {
      this.invalid = true;
      return;
    } else {
      this.loading = true;
      // params.masterData = JSON.stringify(params.masterData);
      if (!params.id) {
        this.http.itdataSouceCommit(params).then((res: any) => {
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
      } else {
        this.http.itdataSouceUpdateAsync(params).then((res: any) => {
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
    }
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
    let typeInfo: any = {
      "agree": this.currentLanguage == 'en' ? 'agree' : '同意',
      "reject": this.currentLanguage == 'en' ? 'reject' : '拒绝',
      "transfer": this.currentLanguage == 'en' ? 'transfer' : '转办',
      "return": this.currentLanguage == 'en' ? 'return' : '退回',
      "cancel": this.currentLanguage == 'en' ? 'cancel' : '取消',
    };
    //BAS 必须上传附件
    // 提取formData.fileList中所有的fileName，存入Set便于快速查找
    const formFileNames = new Set(
      this.formData.fileList.map((file: any) => file.fileName)
    );// 筛选出currFileList中存在但formData.fileList中不存在的文件对象
    const hasMissingFiles = this.currFileList.some(file => {
      return !formFileNames.has(file.isBack ? file.fileName : file?.response?.data);
    });

    if (type == 'agree' && this.flowData.flowStepName == 'BSA Approval' && !hasMissingFiles) {
      this.messageService.add({ severity: 'warn', summary: TranslateData.uploadAttachment ?? 'Please upload an attachment' });
      return;
    }
    if (this.approvalData.auditContent) {
      this.remarkValue = this.approvalData.auditContent;
    }
    if (!this.remarkValue) {
      this.messageService.add({ severity: 'warn', summary: TranslateData.fillComment ?? 'Please fill in the comment' });
      return;
    }

    let attacchmentList = new Array<any>();
    this.currFileList.forEach((item: any) => {
      if (item.isBack) {
        attacchmentList.push(item);
      } else {
        attacchmentList.push(item?.response?.data);
      }
    });

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
          attacchments: attacchmentList,
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

  // // 其他文件流操作
  // handleOtherChange({ file, fileList }: NzUploadChangeParam): void {
  //   const status = file.status;
  //   if (status === 'done') {
  //     this.currOtherFileList = fileList;
  //     this.messageService.add({ severity: 'success', summary: TranslateData.fileUploadSuccess });
  //   } else if (status === 'removed') {
  //     this.currOtherFileList = fileList;
  //     this.messageService.add({ severity: 'success', summary: TranslateData.deleteSuccess });
  //   } else if (status === 'error') {
  //     file.error.statusText = TranslateData.fileUploadFail;
  //     this.messageService.add({ severity: 'error', summary: TranslateData.fileUploadFail });
  //   }
  // }

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

  addRow() {
    this.formData.masterData.push({ dataSource: this.formData.dataSourse, needStandard: null, isMapping: null });
  }

  deleteRow(item: any, i: number) {
    this.formData.masterData.splice(i, 1);
    if (this.formData.masterData.length == 0) this.invalid = false;
  }

  // 主数据选择
  onChangeMasterData(e: any, item: any) {
    if (!e.value) return;
    var data = this.masterDataList.find(f => f.name == e.value);
    item.sourceSystem = data.sourse;
    item.needStandard = item.dataSource != item.sourceSystem;
  }
  // 主数据选择
  onchangeDataSourse() {
    this.formData.masterData.forEach((item: any) => {
      item.dataSource = this.formData.dataSourse;
      item.needStandard = item.dataSource != item.sourceSystem;
    });
  }
  // 主数据选择
  onchangeItemDataSourse(item: any) {
    item.needStandard = item.dataSource != item.sourceSystem;
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
    if (acttype == 'transfer') {
      this.getUserList();
    }
  }
}
