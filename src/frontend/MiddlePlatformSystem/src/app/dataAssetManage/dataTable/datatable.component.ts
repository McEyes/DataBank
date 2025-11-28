import { Component, OnInit } from '@angular/core';
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
@Component({
  selector: 'app-datatable',
  templateUrl: './datatable.component.html',
  styleUrls: ['./datatable.component.scss'],
  providers: [
    MessageService,
    CommonService,
    ConfirmationService,
    DataTableHttpService,
    DataSourceHttpService,
        DictHttpService,
  ],
})
export class DatatableComponent implements OnInit {
  queryParams: any = {
    pageSize: 20,
    pageNum: 1,
  };
  total: number = 0;
  currentUser: any;
  sourceList: any[];
  userList:any[] = [];
  dataCategoryList: any[];
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
  currPanel: string = 'list'
  params: any = {}
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
    this.getUserList();
    this.getDeptList();
    this.langType = localStorage.getItem('lang') || 'en';
    if (this.sourceId) {
      this.queryParams.sourceId = this.sourceId;
    }
    this.getList();
    this.getSource();
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
    });
  }


  nzPageIndexChange(index: number) {
    this.queryParams.pageNum = index;
    this.getList();
  }

  nzPageSizeChange(page: number) {
    this.queryParams.pageSize = page;
    this.getList();
  }

  async getSource() {
    let { data } = await this.sourceService.list();
    this.sourceList = data;
  }

  goPage(type: string, data: any = null) {
    if (type === 'preview' && data.reviewable!=1) {
      return this.messageService.add({
        severity: 'warn',
        summary: TranslateData.tips,
        detail: TranslateData.unReviewable
      });
    }
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

  //删除按钮
  ondelete(event: any, info?: any) {
    this.confirmationService.confirm({
      target: event.target,
      message:
        localStorage.getItem('lang') == 'en'
          ? 'We will synchronously remove the associated topic domains and APIs. Are you sure you want to delete them?'
          : '会同步移除已关联的主题域和API，是否确认删除？',
      icon: 'pi pi-exclamation-triangle',
      rejectLabel: localStorage.getItem('lang') == 'en' ? 'No' : '否',
      acceptLabel: localStorage.getItem('lang') == 'en' ? 'Yes' : '是',

      accept: () => {
        this.tableService.delete(info.id).then((resDel: any) => {
          if (resDel.code == 200) {
            this.messageService.add({
              severity: 'success',
              summary: localStorage.getItem('lang') == 'en' ? 'Delete' : '删除',
              detail: localStorage.getItem('lang') == 'en' ? 'Success' : '成功',
            });
            //查询页面
            this.getList();
          } else {
            this.messageService.add({
              severity: 'error',
              summary: localStorage.getItem('lang') == 'en' ? 'error' : '错误',
              detail: resDel.msg,
            });
          }
        });
      },
      reject: () => {
        this.messageService.add({
          severity: 'info',
          summary: localStorage.getItem('lang') == 'en' ? 'Delete' : '删除',
          detail: localStorage.getItem('lang') == 'en' ? 'Canceled' : '取消',
        });
      },
    });
  }

  //页面列表-查询按钮
  getList() {
    this.loading = true;
    if (this.queryParams.isowner === true) {
      this.queryParams.ownerId = this.currentUser.id;
    } else {
      this.queryParams.ownerId = this.queryParams.owner || "";
    }
     if (this.queryParams.ownerDept ==null) {
      this.queryParams.ownerDept=''
    }
    this.tableService.page(this.queryParams).then((result: any) => {
      if (result.code == 200) {
        this.pageData = result.data.data;
        this.total = result.data.total;
        this.loading = false;
      } else {
        this.messageService.add({
          key: 'key',
          severity: 'error',
          summary: localStorage.getItem('lang') == 'en' ? 'error' : '错误',
          detail: result.msg,
        });
      }
    });
  }

  onQuery() {
    this.getList();
  }

  //重置按钮
  onReset() {
    this.queryParams.sourceId = '';
    this.queryParams.tableName = '';
    this.queryParams.isowner = ''
    this.queryParams = {
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
  // async initDictList() {
  //   let res = await this.dictService.codes('data_category');
  //   if (res.success) {//.filter((f: any) => f.dictCode == 'data_category');
  //     this.dataCategoryList = res.data.map((f: any) => {
  //       var obj: Record<string, any> = new Object();
  //       obj["key"] = f.itemValue;
  //       obj[f.itemValue] = f.itemText;
  //       return obj;
  //     });
  //     this.dataCategoryList.forEach((f: any) => {
  //       this.lang.get("dict." + f.dictCode + "." + f.itemValue).subscribe((res: string) => {
  //         if (res != "dict." + f.dictCode + "." + f.itemValue) f[f["key"]] = res;
  //       });
  //     });
  //   }
  // }
  async getDeptList() {
    const res = await this.commonHttp.getDepartments()
    this.deptList = res.data.map((item: string) => { return { name: item, id: item } });
  }
}
