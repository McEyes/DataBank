import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { MessageService, ConfirmationService, SortEvent } from 'primeng/api';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonService, ExcelExportService } from 'jabil-bus-lib';
import DictHttpService from 'src/api/common/dict';
import { en_US, NzI18nService, zh_CN } from 'ng-zorro-antd/i18n';
import ApiHttpService from 'src/api/dataAssetManage/api';
import ApiLogHttpService from 'src/api/dataAssetManage/api/log';
import { TranslateData } from 'src/app/core/translate/translate-data';
interface column {
  itemValue: string;
  itemText: string;
  index?: string;
  id?: string;
  status?: string;
  createTime?: string;
  apiName?: string;
  apiVersion?: string;
  apiUrl?: string;
  reqMethod?: string;
  resType?: string;
}

@Component({
  selector: 'app-api',
  templateUrl: './api.component.html',
  styleUrls: ['./api.component.scss'],
  providers: [
    MessageService,
    CommonService,
    ExcelExportService,
    ConfirmationService,
    DictHttpService,
    ApiHttpService,
    ApiLogHttpService,
  ],
})
export class APIComponent implements OnInit {
  @Output() goBack = new EventEmitter<string>();
  @Input() field: any;
  loading: boolean = false;
  // 查询参数
  queryParams: any = {
    pageNum: 1,
    pageSize: 20,
    orderField: '',
  };
  queryFirst: number = 0;
  totalRecords: number;
  defaultParams: any = {};
  isEdit: boolean = false;
  selectedColumns: any[];
  columns: column[] = [];
  columnInfo: any = {};
  apiStateObj: any = {
    1: TranslateData.toBeReleased,
    2: TranslateData.haveReleased,
    3: TranslateData.beOffline,
  }
  apiState: any[] = [
    {
      tagText: TranslateData.toBeReleased,
      value: '1',
      tagClass: 'primary',
    },
    {
      tagText: TranslateData.haveReleased,
      value: '2',
      tagClass: 'info',
    },
    {
      tagText: TranslateData.beOffline,
      value: '3',
      tagClass: 'danger',
    },
  ];
  statusColor: any = {
    1: 'primary',
    2: 'success',
    3: 'danger',
  };
  statusList: any[] = [
    {
      label: TranslateData.toBeReleased,
      value: '1',
      tagClass: 'primary',
    },
    {
      label: TranslateData.haveReleased,
      value: '2',
      tagClass: 'info',
    },
    {
      label: TranslateData.beOffline,
      value: '3',
      tagClass: 'danger',
    },
  ];
  reqMethodType: any = {
    1: TranslateData.apiMode1,
    2: TranslateData.apiMode2,
    3: TranslateData.apiMode3,
    4: TranslateData.apiMode4,
  };
  reqMethodTypeList: any[] = [
    {
      label: TranslateData.apiMode1,
      value: '1',
    },
    {
      label: TranslateData.apiMode2,
      value: '2',
    },
    {
      label: TranslateData.apiMode3,
      value: '3',
    },
    {
      label: TranslateData.apiMode4,
      value: '4',
    },
  ];
  langType: string = 'en';
  currPanel: string = 'list';
  params: any = {}
  constructor(
    public router: Router,
    private messageService: MessageService,
    private confirmationService: ConfirmationService,
    private apiHttpService: ApiHttpService,
    private apiLogHttpService: ApiLogHttpService,
    private dictHttpService: DictHttpService,
    private i18n: NzI18nService,
    private route: ActivatedRoute
  ) {
    sessionStorage.removeItem('attributeCompForm');
    sessionStorage.removeItem('implementCompForm');
    sessionStorage.removeItem('parameterCompForm');
    sessionStorage.removeItem('editorValue');
    sessionStorage.removeItem('consultValue');
  }

  ngOnInit(): void {
    this.langType = localStorage.getItem('lang') || 'en';
    this.getPageData();
    // this.getDictFunc();
    this.i18n.setLocale(localStorage.getItem('lang') === 'en' ? en_US : zh_CN);

    this.route.queryParams.subscribe((res: any) => {
      if (res.type === 'link') {
        this.params = { title: 'edit', info: JSON.stringify({ id: res.id, edit: res.edit }) }
        this.currPanel = 'detail'
      }
    })
  }

  nzPageIndexChange(index: number) {
    this.queryParams.pageNum = index;
    this.getPageData();
  }

  nzPageSizeChange(page: number) {
    this.queryParams.pageSize = page;
    this.getPageData();
  }

  getDictFunc() {
    this.dictHttpService.code('data_api_status').then((res: any) => {
      if (res.code == 200) {
        this.statusList = (res.data && res.data.length > 0 ? res.data : []).map((item: any) => {
          return {
            label: item.itemValue,
            value: item.itemText,
          };
        });
      } else {
        // this.dictHttpService.dictsRefresh();
        // this.getDictFunc();
      }
    });
    this.dictHttpService.code('data_config_type').then((res: any) => {
      if (res.code == 200) {
        this.reqMethodTypeList = (res.data && res.data.length > 0 ? res.data : []).map(
          (item: any) => {
            return {
              label: item.itemValue,
              value: item.itemText,
            };
          }
        );
      } else {
        // this.dictHttpService.dictsRefresh();
        // this.getDictFunc();
      }
    });
  }

  queryBtn() {
    this.queryParams = {
      pageSize: 20,
      pageNum: 1,
    };
    this.queryFirst = 0;
    this.queryParams = { ...this.queryParams, ...this.defaultParams };
    this.getPageData();
  }

  onReset() {
    this.defaultParams = {};
    this.queryParams = {
      pageSize: 20,
      pageNum: 1,
    };
    this.queryParams = { ...this.queryParams, ...this.defaultParams };
    this.queryFirst = 0;
    this.getPageData();
  }

  // customSort(e: any) {
  //   // 倒序排序
  //   const descendingArr = [...e.data].sort(
  //     (a: any, b: any) => Number(new Date(b.createTime)) - Number(new Date(a.createTime))
  //   );
  //   // 顺序排序
  //   const ascendingArr = [...e.data].sort(
  //     (a: any, b: any) => Number(new Date(a.createTime)) - Number(new Date(b.createTime))
  //   );
  //   this.columns = e.order > 0 ? ascendingArr : descendingArr;
  // }

  customSort(event: SortEvent) {
    var sortField = event.field + (event.order == 1 ? " DESC" : " ASC");
    if (this.queryParams.orderField != sortField) {
      this.queryParams.orderField = sortField;
      this.getPageData()
    }
  }

  onPageChange(event: any) {
    this.queryFirst = event.first;
    this.queryParams.pageNum = event.page + 1;
    this.queryParams.pageSize = event.rows;
    this.getPageData();
  }

  // 增改查
  async allTableBtn(type: string, info: any = null) {
    if (type != 'add') {
      const res = await this.apiHttpService.getById(info.id);
      this.columnInfo = res.data;
    }
    // this.router.navigate(['/dataasset/standard/dataapiDetails'], {
    //   queryParams: { title: type, info: JSON.stringify(info) },
    // });

    this.currPanel = 'detail'
    this.params = { title: type, info: JSON.stringify(info) }
  }

  // 调用
  async onApi(info: any) {
    if (info.status == 2) {
      const res = await this.apiLogHttpService.resquestMappingAdd(info.id);
      if (res.code == 200) {
        if (info.executeConfig.configType == '3') {
          this.currPanel = 'sql'
          this.params = info
        } else {
          this.currPanel = 'api'
          this.params = info
        }
      } else {
        this.messageService.add({
          severity: 'error',
          summary: localStorage.getItem('lang') == 'en' ? 'Error' : '错误',
          detail: res.msg,
        });
      }
    } else {
      this.messageService.add({
        severity: 'warn',
        summary: localStorage.getItem('lang') == 'en' ? 'Tip' : '提示',
        detail:
          localStorage.getItem('lang') == 'en' ? 'Please publish the API first' : '请先发布API',
      });
    }
  }

  // 注册发布接口(Publish)、注销接口(cancel)
  apiOperation(event: any, action: string, info: any) {
    if (action == 'cancel' && info.status == 3) return;
    let title: any = {
      release: localStorage.getItem('lang') == 'en' ? 'Publish' : '发布',
      cancel: localStorage.getItem('lang') == 'en' ? 'Cancel' : '注销'
    };
    this.confirmationService.confirm({
      target: event.target,
      message:
        localStorage.getItem('lang') == 'en'
          ? `Do you want to ${title[action]} this content?`
          : `确定是否要${title[action]}该条内容?`,
      icon: 'pi pi-exclamation-triangle',
      rejectLabel: localStorage.getItem('lang') == 'en' ? 'No' : '否',
      acceptLabel: localStorage.getItem('lang') == 'en' ? 'Yes' : '是',
      accept: async () => {
        const res = await this.apiHttpService.interfaceOperation(info.id, action);
        if (res.code == 200) {
          this.messageService.add({
            severity: 'success',
            summary: title[action],
            detail: localStorage.getItem('lang') == 'en' ? 'success' : '成功',
          });
          // 刷新列表
          this.getPageData();
        }

        // 添加接口映射，只有调用成功后才能访问接口
        let res_apiLogs: any;
        if (action == 'release') {
          res_apiLogs = await this.apiLogHttpService.resquestMappingAdd(info.id);
        } else if (action == 'cancel') {
          res_apiLogs = await this.apiLogHttpService.resquestMappingAdelete(info.id);
        }
      },
      reject: () => {
        this.messageService.add({
          severity: 'info',
          summary: `${title[action]}`,
          detail:
            (localStorage.getItem('lang') == 'en' ? 'Canceled' : '已取消') +
            ' ' +
            `${title[action]}`,
        });
      },
    });
  }

  // 删
  ondelete(event: any, id: any) {
    this.confirmationService.confirm({
      target: event.target,
      message: TranslateData.commonDelete,
      icon: 'pi pi-exclamation-triangle',
      rejectLabel: TranslateData.no,
      acceptLabel: TranslateData.yes,
      accept: () => {
        this.apiHttpService.delete(id).then((res: any) => {
          this.messageService.add({
            severity: 'success',
            summary: TranslateData.delete,
            detail: TranslateData.success,
          });
          // 刷新列表
          this.getPageData();
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

  // -------------------------------------------- HTTP Request --------------------------------------------

  async getPageData() {
    this.loading = true;
    const res = await this.apiHttpService.page(this.queryParams);
    let data = res?.data || {};
    this.columns = data?.data?.map((item: any, index: number) => {
      item.index = index + 1;
      // 设置数据状态字典
      item.statetag = this.apiState.find((sta: any) => sta.value == item.status);
      return item;
    });
    this.loading = false;
    this.totalRecords = data?.total;
  }

  backEvent(e: any) {
    if (e === 'update') {
      this.getPageData();
    }
    this.currPanel = 'list'
  }
}
