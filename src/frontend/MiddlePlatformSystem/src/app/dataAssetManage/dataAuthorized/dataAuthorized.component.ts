import { Component, OnInit } from '@angular/core';
import { ConfirmationService } from 'primeng/api';
import { MessageService } from 'primeng/api';
import { Router } from '@angular/router';
import CommonHttp from 'src/api/common/index';
import DataTableHttpService from 'src/api/dataAssetManage/dataTable';
import { en_US, NzI18nService, zh_CN } from 'ng-zorro-antd/i18n';

@Component({
  selector: 'app-data-authorized',
  templateUrl: './dataAuthorized.component.html',
  styleUrls: ['./dataAuthorized.component.scss'],
  providers: [MessageService, ConfirmationService, DataTableHttpService, CommonHttp],
})

export class DataAuthorizedComponent implements OnInit {
  queryParams: any = {
    userId: '',
    pageNum: 1,
    pageSize: 20,
  };
  queryFirst: number = 0;
  loading: boolean = false;
  totalRecords: number;
  tableData: any[] = [];
  userInfo: any = {};
  dialogviewApiValue: boolean = false;
  viewApiList: any[] = [];
  currPanel: string = 'list'
  params: any = {}

  constructor(
    private messageService: MessageService,
    private router: Router,
    private httpService: DataTableHttpService,
    private commonHttp: CommonHttp,
    private i18n: NzI18nService
  ) { }

  ngOnInit(): void {
    this.getUserInfo()
    this.i18n.setLocale(localStorage.getItem('lang') === 'en' ? en_US : zh_CN);
  }

  nzPageIndexChange(index: number) {
    this.queryParams.pageNum = index
    this.getTableFunc();
  }

  nzPageSizeChange(page: number) {
    this.queryParams.pageSize = page
    this.getTableFunc();
  }

  async getUserInfo() {
    let res = await this.commonHttp.getUserInfo();
    if (res.success) {
      this.userInfo = res.data;
      this.queryParams.userId = this.userInfo.id
      this.getTableFunc();
    } else {
      this.messageService.add({
        severity: 'error',
        summary: localStorage.getItem('lang') == 'en' ? 'Error' : '错误',
        detail: res.data,
      });
    }
  }

  getTableFunc() {
    this.loading = true;
    this.httpService.getUserTable(this.queryParams).then((res: any) => {
      let data = res?.data
      this.tableData = data?.data.map((item: any, index: number) => {
        item.index = index + 1;
        return item;
      });
      this.totalRecords = data.total;
      this.loading = false;
    });
  }

  allTableBtn(type: string, info: any = null) {
    if (!info.id || (type == 'data' && info.reviewable != 1)) {
      this.messageService.add({
        severity: 'warn',
        summary: localStorage.getItem('lang') == 'en' ? 'No preview available' : '不可预览',
        detail: localStorage.getItem('lang') == 'en' ? 'The current data cannot be previewed' : '当前数据不可预览',
      });
      return;
    }
    info.title = type
    this.params = info
    this.currPanel = type === 'data' ? 'preview' : 'details'
    // let url =
    //   type == 'data'
    //     ? 'dataasset/visualization/datapreview'
    //     : 'dataasset/visualization/assetquerydetails';
    // this.router.navigate([url], {
    //   queryParams: { title: type, id: info.id, info: JSON.stringify(info) },
    // });
  }

  onPageChange(event: any) {
    this.queryFirst = event.first;
    this.queryParams.pageNum = event.page + 1;
    this.queryParams.pageSize = event.rows;
  }

  onCopy(type: string, str: string, info: any = null) {
    if (!str) return;
    if (type == 'data') {
      let newInfo = Object.assign({}, info);
      if (info.data.includes(',')) {
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
          summary: localStorage.getItem('lang') == 'en' ? 'Success' : '成功',
          detail: localStorage.getItem('lang') == 'en' ? 'Copy' : '复制',
        });
      }
    }
  }

  viewApi(info: any = null) {
    if (!info.data) return;
    if (info.data.includes(',')) {
      let apiList: any = info.data.split(',');
      this.viewApiList = apiList;
    } else {
      this.viewApiList = [info.data];
    }
    this.dialogviewApiValue = true;
  }

  copyText(text: string): void {
    const textarea = document.createElement('textarea');
    textarea.value = this.replaceText(text);
    document.body.appendChild(textarea);
    textarea.select();
    document.execCommand('copy');
    document.body.removeChild(textarea);
    this.messageService.add({
      severity: 'success',
      summary: localStorage.getItem('lang') == 'en' ? 'Success' : '成功',
      detail: localStorage.getItem('lang') == 'en' ? 'Copy' : '复制',
    });
  }

  onCloseAPIDialog() {
    this.viewApiList = [];
    this.dialogviewApiValue = false;
  }
  convertHtml(data: string) {
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
