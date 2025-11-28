import { Component, OnInit } from '@angular/core';
import { MessageService, ConfirmationService } from 'primeng/api';
import { Router } from '@angular/router';
import { CommonService, ExcelExportService } from 'jabil-bus-lib';
import DataAccessLogHttpService from 'src/api/dataAssetManage/dataAccessLog';
import commonHttp from 'src/api/common/index';
import { en_US, NzI18nService, zh_CN } from 'ng-zorro-antd/i18n';
import { TranslateData } from 'src/app/core/translate/translate-data';
import CommonHttpService from 'src/api/common/index';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-data-access-log',
  templateUrl: './dataAccessLog.component.html',
  styleUrls: ['./dataAccessLog.component.scss'],
  providers: [MessageService, CommonService, ExcelExportService, ConfirmationService, commonHttp, DataAccessLogHttpService],
})
export class DataAccessLogComponent implements OnInit {
  queryParams: any = {
    pageSize: 20,
    pageNum: 1,
  };
  searchData: any = {};
  queryFirst: number = 0;
  totalRecords: number;
  loading: boolean = false;
  isEdit: boolean = false;
  tableData: any[] = [];
  selectedRows: any[];
  displayBasic: boolean;
  dialogTitle: string = 'add';
  statusColor: any = {
    1: 'success',
    0: 'danger',
  };
  langType: string = 'en';
  currPanel: string = 'list'
  params: any = {}
  statusList: any[] = [
    { label: TranslateData.success, value: '1' },
    { label: TranslateData.fail, value: '0' },
  ];
  userList: any[] = []
  deptList: any[] = []


  constructor(
    public router: Router,
    private messageService: MessageService,
    private confirmationService: ConfirmationService,
    private apiLogsHttpService: DataAccessLogHttpService,
    private commonHttp: CommonHttpService,
    private i18n: NzI18nService,
    private commonService: CommonService

  ) {
    this.getPageData();
  }

  ngOnInit(): void {
    this.langType = localStorage.getItem('lang') || 'en';
    this.i18n.setLocale(localStorage.getItem('lang') === 'en' ? en_US : zh_CN);
    this.getUserList();
    this.getDeptList();
  }

  nzPageIndexChange(index: number) {
    this.queryParams.pageNum = index;
    this.getPageData();
  }

  nzPageSizeChange(page: number) {
    this.queryParams.pageSize = page;
    this.getPageData();
  }

  queryBtn() {
    this.queryParams = {
      pageSize: 20,
      pageNum: 1,
    };
    this.queryFirst = 0;
    if (this.searchData.dateRange && this.searchData.dateRange.length == 2) {
      this.searchData.startDate = this.searchData.dateRange[0];
      this.searchData.endDate = this.searchData.dateRange[1];
    } else {
      delete this.searchData.startDate
      delete this.searchData.endDate
    }
    this.queryParams = { ...this.queryParams, ...this.searchData };
    delete this.queryParams.dateRange;
    this.getPageData();
  }

  onReset() {
    this.searchData = {};
    this.queryParams = {
      pageSize: 20,
      pageNum: 1,
    };
    this.queryFirst = 0;
    this.getPageData();
  }

  onPageChange(event: any) {
    this.queryFirst = event.first;
    this.queryParams.pageNum = event.page + 1;
    this.queryParams.pageSize = event.rows;
    this.getPageData();
  }

  showBasicDialog() {
    this.displayBasic = true;
  }

  allTableBtn(type: string, info: any = null) {
    // this.router.navigate(['dataasset/dataaccess/dataapilogDetails'], {
    //   queryParams: { title: type, id: info?.id },
    // });
    this.currPanel = 'detail'
    this.params = { title: type, id: info?.id }
  }

  ondelete(event: any, row: any) {
    this.confirmationService.confirm({
      target: event.target,
      message: TranslateData.commonDelete,
      icon: 'pi pi-exclamation-triangle',
      rejectLabel: TranslateData.no,
      acceptLabel: TranslateData.yes,
      accept: () => {
        this.apiLogsHttpService.delete(row.id).then((res: any) => {
          if (res.code == 200) {
            this.messageService.add({
              severity: 'success',
              summary: TranslateData.delete,
              detail: TranslateData.success,
            });
            this.getPageData();
          } else {
            this.messageService.add({
              severity: 'success',
              summary: TranslateData.delete,
              detail: res.msg,
            });
          }
        });
      },
      reject: () => {
        this.messageService.add({
          severity: 'info',
          summary: TranslateData.delete,
          detail: TranslateData.cancel
        });
      },
    });
  }
  // -------------------------------- Http Request --------------------------------
  async getPageData() {
    this.loading = true;
    const res = await this.apiLogsHttpService.page(this.queryParams);
    let data = res.data;
    this.loading = false;
    if (Array.isArray(data.data)) {
      this.tableData = data.data;
      this.tableData.forEach((element: any, index: number) => (element.index = index + 1));
      this.totalRecords = data.total;
    }
  }

  goBack(e: string) {
    if (e === 'update') {
      this.getPageData();
    }

    this.currPanel = 'list'
  }
  exportExcel() {
    let queryParams = {
      pageSize: 100000000,
      pageNum: 1
    };
    if (this.searchData.dateRange && this.searchData.dateRange.length == 2) {
      this.searchData.startDate = this.searchData.dateRange[0];
      this.searchData.endDate = this.searchData.dateRange[1];
    }else {
      delete this.searchData.startDate
      delete this.searchData.endDate
    }
    queryParams = { ...this.searchData, ...queryParams };
    this.commonHttp.exportExcel(environment.BasicServer + "/api/apiLogs/ExportExcel", 'API访问记录', queryParams);
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
  async getDeptList() {
    const res = await this.commonHttp.getDepartments()
    this.deptList = res.data.map((item: string) => { return { name: item, id: item } });
  }
}
