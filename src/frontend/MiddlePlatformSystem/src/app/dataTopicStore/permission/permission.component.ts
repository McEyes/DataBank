import { Component, OnInit } from '@angular/core';
import { MessageService, ConfirmationService } from 'primeng/api';
import { ViewEncapsulation } from '@angular/core';
import { en_US, NzI18nService, zh_CN } from 'ng-zorro-antd/i18n';
import { TranslateData } from 'src/app/core/translate/translate-data'
import AssetQueryHttpService from 'src/api/dataAssetManage/assetQuery'
import CommonHttpService from 'src/api/common'

import DataTopicStoreHttpService from 'src/api/dataTopicStore';

@Component({
  selector: 'app-data-topic-permission',
  templateUrl: './permission.component.html',
  styleUrls: ['./permission.component.scss'],
  encapsulation: ViewEncapsulation.Emulated,
  providers: [
    MessageService,
    AssetQueryHttpService,
    ConfirmationService,
    CommonHttpService,
    DataTopicStoreHttpService
  ],
})
export class DataTopicPermissionComponent implements OnInit {
  queryParams: any = {
    pageSize: 20,
    pageNum: 1,
    status: null,
    topicName: '',
    ntid: null
  };
  total: number = 0;
  currentUser: any;
  userList: any[] = [];
  loading: boolean = false;
  formData: any = {};
  dialogTitle: string = '新增';
  langType: string = 'en';
  pageData: any[] = []
  currPanel: string = 'list'
  params: any = {}
  approvalData: any = {
    dialogVisible: false
  }
  invalid: boolean = false
  currRow: any = {}
  statusList: any[] = [{name: 'New', id: 0}, {name: 'Approved', id: 1}, {name: 'Reject', id: 2}]
  statusObj: any = {
    New: 'info.status.new',
    Approved: 'info.approved',
    Rejected: 'info.rejected',
  }
  constructor(
    private readonly confirmationService: ConfirmationService,
    private readonly messageService: MessageService,
    private readonly httpService: DataTopicStoreHttpService,
    private readonly commonHttp: CommonHttpService,
    private readonly i18n: NzI18nService,
  ) {
    window.addEventListener('resetPage', (e: any) => {
      this.currPanel = 'list'
    });
  }

  ngOnInit(): void {
    this.getUserList();
    this.langType = localStorage.getItem('lang') || 'en';
    this.getList();
    this.i18n.setLocale(localStorage.getItem('lang') === 'en' ? en_US : zh_CN);
  }

  nzPageIndexChange(index: number) {
    this.queryParams.pageNum = index;
    this.getList();
  }

  nzPageSizeChange(page: number) {
    this.queryParams.pageSize = page;
    this.getList();
  }

  //页面列表-查询按钮
  getList() {
    this.loading = true;

    this.httpService.getApprovalList(this.queryParams).then((result: any) => {
      if (result.succeeded) {
        this.pageData = result.data.data;
        this.total = result.data.totalCount;
        this.loading = false;
      } else {
        this.messageService.add({
          key: 'key',
          severity: 'error',
          summary: localStorage.getItem('lang') == 'en' ? 'error' : '错误',
          detail: result.msg,
        });
      }

      this.loading = false
    });
  }

  onQuery() {
    this.getList();
  }

  //重置按钮
  onReset() {
    this.queryParams = {
      topicName: '',
      status: null,
      ntid: null,
      pageSize: 20,
      pageNum: 1,
    };
    this.getList();
  }

  // 用户列表d
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

  approve(rowData: any) {
    this.currRow = rowData
    this.approvalData.dialogVisible = true
  }

  async applyPermission(status: boolean) {
    if (this.loading) return
    this.loading = true
    const res = await this.httpService.approvePermission({
      accessRequestId: this.currRow.id,
      status: status ? 1 : 2,
      remark: this.approvalData.remark
    })

    if (res.succeeded) {
      this.messageService.add({
        severity: 'success',
        summary: TranslateData.saveSuccess,
      });
      this.approvalData.dialogVisible = false
      this.getList()
    } else {
      this.messageService.add({
        severity: 'error',
        summary: res?.errors || TranslateData.saveFail,
      });
    }
    this.loading = false
  }

}
