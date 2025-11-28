import { Component, OnInit } from '@angular/core';
import { ConfirmationService } from 'primeng/api';
import { MessageService } from 'primeng/api';
import AuthorityService from 'src/api/dataAssetManage/authority';
import { en_US, NzI18nService, zh_CN } from 'ng-zorro-antd/i18n';
import CommonHttp from 'src/api/common/index';
import { TranslateData } from 'src/app/core/translate/translate-data';

@Component({
  selector: 'app-user-permission-application-record',
  templateUrl: './userPermissionApplicationRecord.component.html',
  styleUrls: ['./userPermissionApplicationRecord.component.scss'],
  providers: [MessageService, ConfirmationService, AuthorityService, CommonHttp],
})
export class UserPermissionApplicationRecordComponent implements OnInit {
  queryParams: any = {
    pageNum: 1,
    pageSize: 20,
  };
  defualtParams: any = {
    keyword: '',
  };
  queryFirst: number = 0;
  loading: boolean = false;
  totalRecords: number;
  tableHeader!: Column[];
  tableData: any[] = [];
  statusList: any[] = [
    // { label: TranslateData.unsync, value: '-2' },
    // { label: TranslateData.underReview, value: '-1' },
    // { label: TranslateData.approved, value: '0' },
    // { label: TranslateData.rejected, value: '1' },
    // { label: TranslateData.finished, value: '2' },
    // { label: TranslateData.abandon, value: '4' },
    // { label: TranslateData.withdrawn, value: '-5' },
    // { label: TranslateData.becameInvalid, value: '-6' },
    // { label: TranslateData.cancelled, value: '-4' },
    { label: TranslateData.enable, value: true },
    { label: TranslateData.deactivate, value: false },
  ];
  typeLabel: any = {
    1: TranslateData.individual,
    2: TranslateData.application,
  };
  tagLabel: any = {
    // '-2': TranslateData.unsync,
    // '-1': TranslateData.underReview,
    // '0': TranslateData.approved,
    // '1': TranslateData.rejected,
    // '2': TranslateData.finished,
    // '4': TranslateData.abandon,
    // '-5': TranslateData.withdrawn,
    // '-6': TranslateData.becameInvalid,
    // '-4': TranslateData.cancelled,
    true: TranslateData.enable,
    false: TranslateData.deactivate,
  };
  tagColor: any = {
    // '-2': 'warning',
    // '-1': 'warning',
    // '0': 'success',
    // '1': 'danger',
    // '2': 'info',
    // '4': 'info',
    // '-5': 'primary',
    // '-6': 'info',
    // '-4': 'info',
    true: 'success',
    false: 'danger'
  };
  tableTitleShow: any[] = ['index', 'createTime', 'auditTime', 'status'];
  userInfo: any = {};
  viewApiList: any[] = [];
  dialogviewApiValue: boolean = false;
  langType: string = 'en';
  tip: string = '';

  constructor(
    private messageService: MessageService,
    private http: AuthorityService,
    private confirmationService: ConfirmationService,
    private i18n: NzI18nService,
    private commonHttp: CommonHttp
  ) { }
  ngOnInit(): void {
    this.langType = localStorage.getItem('lang') || 'en';
    this.i18n.setLocale(localStorage.getItem('lang') === 'en' ? en_US : zh_CN);
    this.tip = TranslateData.tips;
    this.tableHeader = [
      { field: 'index', header: 'field.number', width: '45px' },
      // { field: 'flowNo', header: 'workflow.no', width: '150px' },
      { field: 'tableName', header: 'field.table.name', width: '200px' },
      { field: 'secrets', header: 'field.api.token', width: '200px' },
      // { field: 'ownerName', header: 'field.owner.name', width: '120px' },
      { field: 'apiList', header: 'field.api.address', width: '350px' },
      { field: 'applyDescription', header: 'field.application.description', width: 'auto' },
      // { field: 'description', header: 'field.application.description', width: '250px' },
      // { field: 'clientName', header: 'field.owner.name', width: '150px' },
      { field: 'clientName', header: 'field.applicant', width: '150px' },
      { field: 'createTime', header: 'apply.date', width: '160px' },
    ];
    this.getUserInfo();
  }

  async getUserInfo() {
    let res = await this.commonHttp.getUserInfo();
    if (res.success) {
      this.userInfo = res.data;
      this.getTableFunc();
    } else {
      this.messageService.add({
        severity: 'error',
        summary: TranslateData.fail,
        detail: res.msg,
      });
    }
  }

  nzPageIndexChange(index: number) {
    this.queryParams.pageNum = index;
    this.getTableFunc();
  }

  nzPageSizeChange(page: number) {
    this.queryParams.pageSize = page;
    this.getTableFunc();
  }

  getTableFunc() {
    this.loading = true;
    this.http.getSelfRecordPage(this.queryParams).then((res: any) => {
      if (res.code == 200) {
        let data = res.data;
        this.tableData = data.data.map((item: any, index: number) => {
          item.index = index + 1;
          return item;
        });
        this.totalRecords = data.total;
        this.loading = false;
      } else {
        this.loading = false;
        this.messageService.add({
          severity: 'error',
          summary: TranslateData.fail,
          detail: res.msg,
        });
      }
    });
  }

  search(type: string = '') {
    if (type == 'reset') {
      this.defualtParams = {};
    }
    this.queryFirst = 0;
    this.queryParams = {
      pageNum: 1,
      pageSize: 20,
    };
    this.queryParams = { ...this.queryParams, ...this.defualtParams };
    this.getTableFunc();
  }

  onPageChange(event: any) {
    this.queryFirst = event.first;
    this.queryParams.pageNum = event.page + 1;
    this.queryParams.pageSize = event.rows;
    this.getTableFunc();
  }

  onCopy(type: string, str: string, info: any = null) {
    if (!str) return;
    if (type == 'apiList') {
      let newInfo = Object.assign({}, info);
      if (info.apiList.includes(',')) {
        this.viewApi(newInfo);
      } else {
        const textarea = document.createElement('textarea');
        textarea.value = str;
        document.body.appendChild(textarea);
        textarea.select();
        document.execCommand('copy');
        document.body.removeChild(textarea);
        this.messageService.add({
          severity: 'success',
          summary: TranslateData.success,
          detail: TranslateData.copy
        });
      }
    } else if (type == 'secrets') {
      const textarea = document.createElement('textarea');
      textarea.value = str;
      document.body.appendChild(textarea);
      textarea.select();
      document.execCommand('copy');
      document.body.removeChild(textarea);
      this.messageService.add({
        severity: 'success',
        summary: TranslateData.success,
        detail: TranslateData.copy
      });
    }
  }

  viewApi(info: any = null) {
    if (!info.apiList) return;
    if (info.apiList.includes(',')) {
      let apiList: any = info.apiList.split(',');
      this.viewApiList = apiList;
    } else {
      this.viewApiList = [info.apiList];
    }
    this.dialogviewApiValue = true;
  }

  copyTextApi(text: string,field:string): void {
    if(field=="apiList"||field=="secrets") this.copyText(text);
  }

  copyText(text: string): void {
    const textarea = document.createElement('textarea');
    textarea.value = this.replaceText( text);
    document.body.appendChild(textarea);
    textarea.select();
    document.execCommand('copy');
    document.body.removeChild(textarea);
    this.messageService.add({
      severity: 'success',
      summary: TranslateData.success,
      detail: TranslateData.copy+text,
    });
  }

  onCloseAPIDialog() {
    this.viewApiList = [];
    this.dialogviewApiValue = false;
  }

  // 禁用 / 撤回
  onRevocation(type: string, info: any) {
    let textMessage: any = {
      '-5': TranslateData.revocation,
      '-6': TranslateData.operateDeactivate,
    };
    this.confirmationService.confirm({
      message:
        localStorage.getItem('lang') == 'en'
          ? `May I ask whether you are sure that ${textMessage[type]} is required for this data `
          : `请问是否确定要${textMessage[type]}该条数据`,
      rejectLabel: TranslateData.no,
      acceptLabel: TranslateData.yes,
      accept: async () => {
        this.loading = true;
        this.http
          .updateRecord({
            applyFormId: info.applyFormId,
            status: Number(type),
          })
          .then((res: any) => {
            if (res.code == 200) {
              this.loading = false;
              this.messageService.add({
                severity: 'success',
                summary: textMessage[type],
                detail: TranslateData.success,
              });
              this.getTableFunc();
            } else {
              this.loading = false;
              this.messageService.add({
                severity: 'error',
                summary: TranslateData.fail,
                detail: res.msg,
              });
            }
          });
      },
    });
  }

  convertHtml(data: string,field:string) {
    if(field!="apiList")return data;
    // 正则表达式匹配 [方法]URL 格式，支持POST、GET、DELETE、PUT
    // 分组1: 匹配指定的HTTP方法
    // 分组2: 匹配后续的URL部分（直到逗号或字符串结束）
    const regex = /\[((?:POST|GET|DELETE|PUT))\]([^,]+)/g;

    // 替换函数 - 将匹配到的内容转换为带类名的span标签
    const result = data.replace(regex, (match, method, url) => {
      // 将方法名转为小写作为类名（post-item 或 get-item）
      const className = `${method.toLowerCase()}-item`;
      return `<span class="${className}">${url}</span>`;
    });
    return result;
  }

  replaceText(data: string) {
    // 正则表达式匹配 [方法]URL 格式，支持POST、GET、DELETE、PUT
    // 分组1: 匹配指定的HTTP方法
    // 分组2: 匹配后续的URL部分（直到逗号或字符串结束）
    const regex = /\[((?:POST|GET|DELETE|PUT))\]([^,]+)/g;

    // 替换函数 - 将匹配到的内容转换为带类名的span标签
    const result = data.replace(regex, (match, method, url) => {
      return url;
    });
    return result;
  }
}

interface Column {
  field: string;
  header: string;
  width: string;
}
