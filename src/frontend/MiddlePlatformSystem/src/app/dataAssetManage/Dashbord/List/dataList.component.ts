import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { MessageService, ConfirmationService } from 'primeng/api';
import { CommonService, ExcelExportService } from 'jabil-bus-lib';
import { ActivatedRoute } from '@angular/router';
import { en_US, NzI18nService, zh_CN } from 'ng-zorro-antd/i18n';
import { TranslateData } from 'src/app/core/translate/translate-data'
import DataTableHttpService from 'src/api/dataAssetManage/dataTable';
import DataSourceHttpService from 'src/api/dataAssetManage/dataSource';
import CommonHttpService from 'src/api/common';
import DictHttpService from 'src/api/common/dict';
import { TranslateService } from '@ngx-translate/core';
import DashbordHttpservice from 'src/api/dataAssetManage/DashbordHttpservice';
@Component({
  selector: 'Dashbord',
  templateUrl: './dataList.component.html',
  styleUrls: ['./dataList.component.scss'],
  providers: [
    MessageService,
    CommonService,
    ConfirmationService,
    DataTableHttpService,
    DataSourceHttpService,
    DictHttpService,
    DashbordHttpservice
  ],
})
export class DataListComponent implements OnInit {
  @ViewChild('dataTable', { static: true, read: ElementRef }) dataTable: any;

  queryParams: any = {
    pageSize: 20,
    pageNum: 1,
  };

  total: number = 0;
  oldpageNum: number = 0;
  currentUser: any;
  loading: boolean = false;
  formData: any = {};
  dialogTitle: string = '新增';
  sourceId: any;
  langType: string = 'en';
  levelInfo: any = {
    1: TranslateData.secretLevel1,
    2: TranslateData.secretLevel2,
    3: TranslateData.secretLevel3,
    4: TranslateData.secretLevel4,
    5: TranslateData.secretLevel5,
  }

  updateMethodObj: any = {
    manual: TranslateData.manual,
    automatic: TranslateData.automatic
  }
  deptList: any[] = []
  pageData: any[] = []
  selRowData: any[] = []
  userList: any[] = []
  currPanel: string = 'list'
  params: any = {}
  expandedRowKeys: any = {}
  constructor(
    public route: ActivatedRoute,
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
    private tableService: DataTableHttpService,
    private sourceService: DataSourceHttpService,
    private commonHttp: CommonHttpService,
    private i18n: NzI18nService,
    private dictService: DictHttpService,
    private lang: TranslateService,
    private dashbordHttpservice: DashbordHttpservice,
  ) {
    this.route.queryParams.subscribe(params => {
      this.sourceId = params['sourceId'];
    });

    window.addEventListener('resetPage', (e: any) => {
      this.currPanel = 'list'
    });
  }
  ngOnInit(): void {
    this.getUserInfo();
    this.getDeptList();
    this.getUserList();
    this.langType = localStorage.getItem('lang') || 'en';
    if (this.sourceId) {
      this.queryParams.sourceId = this.sourceId;
    }
    this.i18n.setLocale(localStorage.getItem('lang') === 'en' ? en_US : zh_CN);
    this.route.queryParams.subscribe((res: any) => {
      if (res.type === 'link') {
        this.params = { title: 'edit', id: res.id, edit: res.edit }
        this.currPanel = 'detail'
      }
    })
  }
  async getUserInfo() {
    this.commonHttp.getUserInfo().then((res: any) => {
      this.currentUser = res.data;
      this.getList();
    });
  }

  async getDeptList() {
    const res = await this.commonHttp.getDepartments()
    this.deptList = res.data.map((item: string) => { return { name: item, id: item } });
  }
  // 用户列表d
  async getUserList() {
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
  async onlodChild(item: any) {
    this.getAllList(item);
  }
  nzPageIndexChange(index: number) {


    // this.dataTable.expandedRows = []
    if (this.selRowData.length > 0) {
      this.confirmationService.confirm({
        message:
          localStorage.getItem('lang') == 'en'
            ? 'You have made data changes. If you proceed to the next page, the data will be lost. Are you sure you want to abandon the save?'
            : '您有数据更改，直接下一页将会丢失数据，是否确认放弃保存？',
        icon: 'pi pi-exclamation-triangle',
        rejectLabel: localStorage.getItem('lang') == 'en' ? 'No' : '否',
        acceptLabel: localStorage.getItem('lang') == 'en' ? 'Yes' : '是',
        accept: () => {
          this.queryParams.pageNum = index;
          this.getList();
          this.selRowData = [];
          this.expandedRowKeys = {}
        },
        reject: () => {
          this.onSave();
          this.expandedRowKeys = {}
          this.queryParams.pageNum = index;
          this.getList();
        },
      });
    } else {
      this.expandedRowKeys = {}
      this.queryParams.pageNum = index;
      this.getList();
    }
  }

  nzPageSizeChange(page: number) {
    this.expandedRowKeys = {}
    if (this.selRowData.length > 0) {
      this.confirmationService.confirm({
        message:
          localStorage.getItem('lang') == 'en'
            ? 'You have made data changes. If you proceed to the next page, the data will be lost. Are you sure you want to abandon the save?'
            : '您有数据更改，直接下一页将会丢失数据，是否确认放弃保存？',
        icon: 'pi pi-exclamation-triangle',
        rejectLabel: localStorage.getItem('lang') == 'en' ? 'No' : '否',
        acceptLabel: localStorage.getItem('lang') == 'en' ? 'Yes' : '是',
        accept: () => {
          this.queryParams.pageSize = page;
          this.getList();
          this.selRowData = [];
        },
        reject: () => {
          this.onSave()
          this.expandedRowKeys = {}
          this.queryParams.pageSize = page;
          this.getList();
        },
      });
    } else {
      this.queryParams.pageSize = page;
      this.getList();
    }
  }
  goPage(type: string, data: any = null) {
    this.currPanel = type;
    this.params = data || {}
    this.params.title = data ? 'edit' : 'add'

  }

  goBack(e: any) {
    if (e === 'update') {
      this.getList();
    }
    this.currPanel = 'list';
  }

  onSave() {

    this.loading = true;
    let date = {
      list: this.selRowData
    }
    this.dashbordHttpservice.updateStatu(date).then((res: any) => {
      if (res?.code == 200) {
        this.messageService.add({
          severity: 'success',
          summary: localStorage.getItem('lang') == 'en' ? 'success' : '更新成功',
          detail: 'success',
        });
      } else {
        this.messageService.add({
          severity: 'error',
          summary: localStorage.getItem('lang') == 'en' ? 'error' : '错误',
          detail: res.msg,
        });

      }
      this.loading = false;
      this.selRowData = [];
    })
  }

  onStatuChange(e: any) {
    let selModel = this.selRowData.filter((s: any) => s.column_id != e.column_id);
    selModel.push(e);
    if (!e.IsIndicator) {
      e.IndicatorCode = "";
    }
    this.selRowData = selModel;
  }

  getAllList(item: any) {
    if (item.children)
      return;

    //distinct table_name,column_name,column_id,column_comment,owner_id,source_id
    this.loading = true;
    let sql = ` select distinct * from (  select  distinct table_name,column_name,column_id,column_comment,owner_id,source_id,indicator_code  from metadata_table_column_view where 1=1 `
    if (item.owner_id) {
      sql += ' and LOWER(owner_id) =LOWER(\'' + item.owner_id + '\')';
    }
    if (item.table_name) {
      sql += ' and table_name =\'' + item.table_name + '\''; 
    }
    sql += ") aa order by indicator_code"

    console.info(sql);
    this.commonHttp.DashbordCallSqlQueryApi("/services/v1.0.0/ITPortal/DataBank/metadata_table_column_view/sqlQuery", sql, 1, 1000
    ).then((res: any) => {
      if (res?.code == 200) {
        if (res?.data?.data) {
          // let arr = res?.data?.data.map((s: any) => s.column_id);
          // let DashbordList = {
          //   listColumnId: arr
          // }
          // this.dashbordHttpservice.GetDashbordListForColumnId(DashbordList).then((dashbordRes: any) => {
          //   if (dashbordRes?.code == 200 && dashbordRes?.data) {
          //     res?.data?.data.forEach((item: any) => {
          //       item.IsDashboard = false;
          //       item.IsIndicator = false;
          //       let m = dashbordRes?.data.filter((s: any) => s.columnId == item.column_id);
          //       if (m.length > 0) {
          //         item.IsDashboard = m[0].isDashboard;
          //         item.IsIndicator = m[0].isIndicator;
          //         item.IndicatorCode = m[0].indicatorCode;
          //       }
          //     })
          //   }
          // });
          item.children = res?.data?.data;
        }
      } else {
        this.messageService.add({
          severity: 'error',
          summary: localStorage.getItem('lang') == 'en' ? 'error' : '错误',
          detail: res.msg,
        });
      }
      this.loading = false;
    });
  }

  //页面列表-查询按钮
  getList() {
    this.loading = true;
    let sql = `select * from metadata_table_view where 1=1 `
    // if (this.currentUser.ntId) {
    // this.currentUser.ntId = 84320;
    this.currentUser.ntId = 3815794;
    if (this.currentUser.ntId != 3815794 && this.currentUser.ntId != 84320 && this.currentUser.ntId != 3359954) {
      sql += ' and owner_id =\'' + this.currentUser.ntId + '\'';
    }
    // }
    if (this.queryParams.ctl_name) {
      sql += ' and ctl_name like \'%' + this.queryParams.ctl_name.trim() + '%\' ';
    }
    if (this.queryParams.table_comment) {
      sql += ' and table_comment like \'%' + this.queryParams.table_comment.trim() + '%\' ';
    }
    if (this.queryParams.table_name) {
      sql += ' and table_name like \'%' + this.queryParams.table_name.trim() + '%\' ';
    }
    if (this.queryParams.owner) {
      sql += ' and  LOWER(owner_id) =LOWER(\'' + this.queryParams.owner + '\')';
    }

    if (this.queryParams.ownerDept) {
      sql += ' and owner_depart =\'' + this.queryParams.ownerDept + '\'';
    }
    this.commonHttp.DashbordCallSqlQueryApi("/services/v1.0.0/ITPortal/DataBank/metadata_table_view/sqlQuery", sql, this.queryParams.pageNum, this.queryParams.pageSize
    ).then((res: any) => {
      if (res?.code == 200) {
        if (res?.data?.data) {
          this.pageData = res?.data?.data;
          this.total = res?.data?.total;
        }
      } else {
        this.messageService.add({
          severity: 'error',
          summary: localStorage.getItem('lang') == 'en' ? 'error' : '错误',
          detail: res.msg,
        });
      }
      this.loading = false;
    });
  }
  onQuery() {
    this.expandedRowKeys = {}
    this.getList();
  }
  //重置按钮
  onReset() {
    this.queryParams.ctl_name = '';
    this.queryParams.table_comment = '';
    this.queryParams.table_name = ''
    this.queryParams = {
      pageSize: 20,
      pageNum: 1,
    };
    this.getList();
  }
}
