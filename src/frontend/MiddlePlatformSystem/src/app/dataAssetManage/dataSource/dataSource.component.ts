import { Component, OnInit } from '@angular/core';
import { MessageService, ConfirmationService } from 'primeng/api';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonService, ExcelExportService } from 'jabil-bus-lib';
import DataSourceHttpService from 'src/api/dataAssetManage/dataSource';
import CommonHttp from 'src/api/common/index';
import { en_US, NzI18nService, zh_CN } from 'ng-zorro-antd/i18n';
import { TranslateData } from 'src/app/core/translate/translate-data';

@Component({
  selector: 'app-data-source',
  templateUrl: './dataSource.component.html',
  styleUrls: ['./dataSource.component.scss'],
  providers: [
    MessageService,
    CommonService,
    ExcelExportService,
    ConfirmationService,
    DataSourceHttpService,
    CommonHttp
  ],
})
export class DataSourceComponent implements OnInit {
  listForm: any = {
    sourceName: '',
    pageSize: 20,
    pageNum: 1,
  };
  total: number = 0;
  isEdit: boolean = false;
  loading: boolean = false;
  formData: any = {
    test: '',
    invalid: false,
  };

  newDataType: any = {
    1: TranslateData.MySql,
    2: TranslateData.MariaDB,
    5: TranslateData.PostgreSql,
    7: TranslateData.SQLServer,
    8: TranslateData.otherDatabase
  }

  detailParams: any = {}
  currPanel: string = 'list'
  pageData: any[] = []
  constructor(
    public router: Router,
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
    private sourceService: DataSourceHttpService,
    private i18n: NzI18nService,
    private route: ActivatedRoute
  ) {
    window.addEventListener('resetPage', (e: any) => {
      this.currPanel = 'list'
    });
  }

  ngOnInit(): void {
    this.getList();
    this.i18n.setLocale(localStorage.getItem('lang') === 'en' ? en_US : zh_CN);

    this.route.queryParams.subscribe((res: any) => {
      if (res.type === 'link') {
        this.detailParams = { title: 'edit', id: res.id, edit: res.edit }
        this.currPanel = 'detail'
      }
    })
  }

  nzPageIndexChange(index: number) {
    this.listForm.pageNum = index;
    this.getList();
  }

  nzPageSizeChange(page: number) {
    this.listForm.pageSize = page;
    this.getList();
  }

  //跳转
  allTableBtn(type: string, info: any = null) {
    let id = null;
    if (type != 'add') {
      id = info.id;
    }

    this.detailParams = { title: type, id: id, info: info }
    this.currPanel = 'detail'
  }

  //删除按钮
  ondelete(event: any, info?: any) {
    this.confirmationService.confirm({
      target: event.target,
      message: TranslateData.deleteConfirm,
      icon: 'pi pi-exclamation-triangle',
      rejectLabel: TranslateData.no,
      acceptLabel: TranslateData.yes,
      accept: () => {
        this.sourceService.delete(info.id).then((resDel: any) => {
          if (resDel.code == 200) {
            this.messageService.add({
              severity: 'success',
              summary: TranslateData.delete,
              detail: TranslateData.success,
            });
            //查询页面
            this.getList();
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

  //页面列表-查询按钮
  getList() {
    this.loading = true;
    this.sourceService.page(this.listForm).then((result: any) => {
      console.log(result);
      if (result.code == 200) {
        this.pageData = result.data.data.map((item: any) => {
          item._isSync =
            item.isSync == 1
              ? TranslateData.unsynchronized
              : TranslateData.synchronized
          return item;
        });
        this.total = result.data.total;
        this.loading = false;
      } else {
        this.messageService.add({
          key: 'key',
          severity: 'error',
          summary: TranslateData.fail,
          detail: result.msg,
        });
      }
    });
  }

  onQuery() {
    this.listForm.pageNum = 1;
    this.listForm.pageSize = 20;
    this.getList();
  }

  //重置按钮
  onReset() {
    this.listForm.sourceName = '';
    this.listForm.pageNum = 1;
    this.listForm.pageSize = 20;
    this.getList();
  }

  goBack(e: string) {
    if (e === 'update') {
      this.getList();
    }

    this.currPanel = 'list'
  }
}
