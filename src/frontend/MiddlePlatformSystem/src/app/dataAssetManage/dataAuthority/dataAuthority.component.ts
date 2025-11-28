import { Component, OnInit } from '@angular/core';
import { ConfirmationService } from 'primeng/api';
import { MessageService } from 'primeng/api';
import { Router } from '@angular/router';
import AuthorityService from 'src/api/dataAssetManage/authority';
import { en_US, NzI18nService, zh_CN } from 'ng-zorro-antd/i18n';
import { TranslateData } from 'src/app/core/translate/translate-data';

@Component({
  selector: 'app-data-authority',
  templateUrl: './dataAuthority.component.html',
  styleUrls: ['./dataAuthority.component.scss'],
  providers: [
    MessageService,
    AuthorityService,
    ConfirmationService,
  ],
})
export class DataAuthorityComponent implements OnInit {
  queryParams: any = {
    keyword: '',
    pageNum: 1,
    pageSize: 20,
  };
  first: number = 0;
  total: number = 0;
  formData: any = {};
  queryParamsToatal: number = 100;
  tableData: any[] = [];
  totalRecords: number;
  loading: boolean = false;
  langType: string = 'en';
  currPanel: string = 'list';
  params: any = {};
  constructor(
    private messageService: MessageService,
    private router: Router,
    private authService: AuthorityService,
    private confirmationService: ConfirmationService,
    private i18n: NzI18nService,
  ) { }

  ngOnInit(): void {
    this.langType = localStorage.getItem('lang') || 'en';
    this.i18n.setLocale(localStorage.getItem('lang') === 'en' ? en_US : zh_CN);
    this.getUserAuth();
  }

  nzPageIndexChange(index: number) {
    this.queryParams.pageNum = index;
    this.getUserAuth();
  }

  nzPageSizeChange(page: number) {
    this.queryParams.pageSize = page;
    this.getUserAuth();
  }

  search(type: string = '') {
    if (type == 'reset') {
      this.queryParams.keyword = '';
    }
    this.first = 0;
    this.queryParams.pageNum = 1;
    this.queryParams.pageSize = 20;
    this.getUserAuth();
  }

  async allTableBtn(type: string, info: any = null) {

    this.currPanel = 'detail'
    this.params =  { title: type, info: JSON.stringify(info) }
  }

  ondelete(event: any, info?: any) {
    this.confirmationService.confirm({
      target: event.target,
      message:TranslateData.commonDelete,
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.authService.deleteUserAuth(info.userId).then((resDel: any) => {
          if (resDel.code == 200) {
            this.messageService.add({
              severity: 'success',
              summary: TranslateData.delete,
              detail: TranslateData.success,
            });
            //查询页面
            this.getUserAuth();
          } else {
            this.messageService.add({
              severity: 'error',
              summary: TranslateData.fail,
              detail: resDel.msg,
            });
          }
        });
      },
      reject: () => {
        this.messageService.add({
          severity: 'info',
          summary: TranslateData.delete,
          detail: TranslateData.cancel,
        });
      },
    });
  }

  getUserAuth() {
    this.loading = true;
    this.authService.userAuthList(this.queryParams).then((res: any) => {
      if (res.code == 200) {
        this.tableData = res.data.data;
        this.total = res.data.total;
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

  goBack(e: string) {
    if(e === 'update') {
      this.search('reset')
    }

    this.currPanel = 'list'
  }
}
