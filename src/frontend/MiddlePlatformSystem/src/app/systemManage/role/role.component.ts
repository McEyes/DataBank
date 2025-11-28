import { Component, Input, OnInit, Output, EventEmitter, Inject, LOCALE_ID } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { MessageService, ConfirmationService } from 'primeng/api';
import { ActivatedRoute, Router } from '@angular/router';
import WorkflowHttpService from 'src/api/home/workflow';
import HomeHttpService from 'src/api/home';
import { TranslateData } from 'src/app/core/translate/translate-data';
import { CommonService, LocalStorage, JBService } from 'jabil-bus-lib';
import RoleHttpService from 'src/api/systemManage/roleApi';

@Component({
  selector: 'jabil-system-role',
  templateUrl: './role.component.html',
  styleUrls: ['./role.component.scss'],
  providers: [
    LocalStorage,
    MessageService,
    ConfirmationService,
    CommonService,
    WorkflowHttpService,
    JBService,
    HomeHttpService
  ],
})
export class SystemRoleComponent implements OnInit {
  @Output() closePanelreturn = new EventEmitter<string>();
  loading!: boolean;
  currentLanguage: any = 'en';
  recordCount: number = 0;
  progress: number = 0;
  workflowType: string = "ITPortal_LakeEntryApplication";
  isCommon: Boolean = false;
  currPanel: string = 'list';
  fromPage: string = 'list';
  submitLoading: Boolean = false;
  tableData: Array<any> = [];

  searchData: any = {
    keyword: '',
    status: '',
    pageNum: 1,
    pageSize: 10,
  };
  selectedRows: Array<any> = [];
  workflowList: Array<any> = [];
  timeout: any = null;
  currRow: any = null;


  workflowStatusList: Array<any> = [
    // {
    //   key: 'label.workflow.status.draft',
    //   id: 'Draft',
    // },
    {
      key: 'home.workflow.status.processing',
      id: 'Running',
    },
    // {
    //   key: 'home.workflow.status.processed',
    //   id: 'Aprd',
    // },
    {
      key: 'home.workflow.status.finish',
      id: 'Completed',
    },
    // {
    //   key: 'label.workflow.status.reject',
    //   id: 'Reject',
    // },
    {
      key: 'label.workflow.status.canceled',
      id: 'Cancel',
    },
    {
      key: 'label.workflow.status.stop',
      id: 'Stop',
    },
  ];

  applyStatusI18NKey: any = {
    Draft: 'label.workflow.status.draft',
    Running: 'home.workflow.status.processing',
    Stop: 'label.workflow.status.stop',
    Aprd: 'home.workflow.status.processed',
    Completed: 'home.workflow.status.finish',
    Reject: 'home.workflow.status.reject',
    Cancel: 'home.workflow.status.canceled',
  };

  items: Array<any> = [];
  hideReturn: boolean = false;
  dialogDisplay: boolean = false;
  formData: any = { name: '', password: '' };
  isShowReturn: boolean = true;
  hasConfigAuth: boolean = false;
  workflowName: string = ''
  constructor(
    private http: RoleHttpService,
    private translate: TranslateService,
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
    private commonService: CommonService,
    public route: ActivatedRoute,
    private router: Router
  ) { }

  goBack() {
    this.getData();
    this.closePanel(null);
    this.closePanelreturn.emit();
  }

  getUrlParams(url: string) {
    let urlStr = url.split('?')[1];
    let obj: any = {};
    if(!urlStr)return obj;
    let paramsArr = urlStr.split('&');
    for (let i = 0, len = paramsArr.length; i < len; i++) {
      let arr = paramsArr[i].split('=');
      obj[arr[0]] = arr[1];
    }
    return obj;
  }


  public async ngOnInit() {
    const params = this.getUrlParams(window.location.hash);
    if (params['flowTempName']) {
      this.searchData.flowTempName = params['flowTempName'];
    }

    this.route.queryParams.subscribe(async res => {
      if (res['type']) {
        this.currPanel = 'edit';
        // this.workflowType = Number(res['type']);
        this.fromPage = res['name'];

        if (res['id']) {
          this.currRow = {
            dataId: res['id'],
          };
        }
      }
    });


    this.currentLanguage = localStorage.getItem('lang');
    this.translate.use(this.currentLanguage);
    this.getData();
    this.commonService.translateData(TranslateData, this.translate);
  }

  async getData(isPaginate?: boolean) {
    this.loading = true;
    if (!isPaginate) {
      this.searchData.pageNum = 1;
      this.searchData.first = 0;
    }
    // this.searchData.flowStatus = this.applyStatusValue[this.searchData.applyStatus] || '';
    let { data } = await this.http.page(this.searchData);
    this.tableData = data?.data || [];
    this.recordCount = data.total;
    this.loading = false;
  }

  search(isReset?: boolean) {
    if (isReset) {
      this.searchData = {
        keyword: '',
        status: "",
        first: 0,
        pageNum: 1,
        pageSize: 10,
      };
    }
    this.getData();
  }

  editData(item?: any, act?: string) {
    this.currPanel = 'edit';
    item = item || {  };
    item.title= act;
    this.currRow = item;
  }

  paginate(event: any) {
    this.searchData.pageNum = event.page + 1;
    this.searchData.pageSize = event.rows;
    this.searchData.first = event.first;
    this.getData(true);
  }


  goHome() {
    if (this.currPanel === 'edit') {
      this.currPanel = 'list'
      return
    }
    this.router.navigate(['/home/page']);
  }


  closePanel(e: any) {
    this.currPanel = 'list'
  }


  deleteData(item?: any) {
    this.confirmationService.confirm({
      message: this.currentLanguage == 'en'
          ? `Are you sure you want to delete ${item.name} user data?`
          : `确定删除${item.name}用户数据？`,
      rejectLabel: localStorage.getItem('lang') == 'en' ? 'No' : '否',
      acceptLabel: localStorage.getItem('lang') == 'en' ? 'Yes' : '是',
      accept: async () => {
        this.http.delete(item.id).then((res: any) => {
          if (res.success) {
            this.messageService.add({
              key: 'key',
              severity: 'success',
              summary: TranslateData.save,
              detail: TranslateData.success,
            });
            this.getData();
          } else {
            this.messageService.add({
              key: 'key',
              severity: 'error',
              summary: TranslateData.fail,
              detail: res.msg,
            });
          }
        });
      },
    });
  }
}
