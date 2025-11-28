import { Component, Input, OnInit, Output, EventEmitter, Inject, LOCALE_ID } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { MessageService, ConfirmationService } from 'primeng/api';
import { ActivatedRoute, Router } from '@angular/router';
import WorkflowHttpService from 'src/api/home/workflow';
import HomeHttpService from 'src/api/home';
import { TranslateData } from 'src/app/core/translate/translate-data';
import { CommonService, LocalStorage, JBService } from 'jabil-bus-lib';
import { DataDownApplicationComponent } from 'src/app/workFlow/dataBank/dataDownApplication/data-down-application.component';
import CommonHttpService from 'src/api/common';

@Component({
  selector: 'jabil-home-workflow',
  templateUrl: './index.html',
  styleUrls: ['./index.scss'],
  providers: [
    LocalStorage,
    MessageService,
    ConfirmationService,
    CommonService,
    WorkflowHttpService,
    JBService,
    HomeHttpService,
    DataDownApplicationComponent
  ],
})
export class WorkflowComponent implements OnInit {
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
    flowNo: '',
    taskSubject: '',
    // flowTempId: "5a56691e-8f9b-4d81-917c-048f0802a06a",
    flowTempName: this.workflowType,
    applyStatus: '',
    flowStatus: '',
    pageNum: 1,
    pageSize: 10,
  };
  selectedRows: Array<any> = [];
  workflowList: Array<any> = [];
  flowTempList: Array<any> = [];
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
    Cancel: 'label.workflow.status.canceled',
  };
  applyStatusValue: any = {
    Draft: 0,
    Running: 1,
    Completed: 99,
    // Reject: 'home.workflow.status.reject',
    Cancel: -1,
    Stop: -99,
  };

  items: Array<any> = [];
  hideReturn: boolean = false;
  dialogDisplay: boolean = false;
  formData: any = { name: '', password: '' };
  isShowReturn: boolean = true;
  hasConfigAuth: boolean = false;
  workflowName: string = ''
  urlParams: any = {};

  constructor(
    private http: WorkflowHttpService,
    private translate: TranslateService,
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
    private commonService: CommonService,
    public route: ActivatedRoute,
    private router: Router,
    private homeHttpService: HomeHttpService,
    private jbService: JBService,
    private commonHttpService: CommonHttpService
  ) {
    this.urlParams = this.commonHttpService.getUrlParams(window.location.hash)
    if (this.urlParams['flowtempname']) {
      this.searchData.flowTempName = this.workflowType = this.urlParams['flowtempname'];
    }
  }

  goBack() {
    this.closePanelreturn.emit();
  }

  // getUrlParams(url: string) {
  //   let urlStr = url.split('?')[1];
  //   let obj: any = {};
  //   let paramsArr = urlStr.split('&');
  //   for (let i = 0, len = paramsArr.length; i < len; i++) {
  //     let arr = paramsArr[i].split('=');
  //     obj[arr[0]] = arr[1];
  //   }
  //   return obj;
  // }

  isEmail() {
    if (window.location.hash.toString().indexOf('type=email') >= 0) {
      const jwt = localStorage.getItem('jwt') || '';
      this.currPanel = 'edit';
      // const params = this.getUrlParams(window.location.hash);

      let isCallbak = this.urlParams?.callbak ?? "" // flowNo
      this.workflowName = this.urlParams?.formNo // flowNo
      this.currPanel = 'edit';
      this.workflowType = this.urlParams?.flowtempname; // ITPortal_LakeEntryApplication
      if (!jwt) {
        this.router.navigate(['/home/page']);
        this.dialogDisplay = true;
      } else if (isCallbak) {
        if (this.urlParams.taskid)
          this.router.navigate(['/home/page'], { queryParams: { flowTempName: this.workflowType, taskid: this.urlParams.taskid, type: "email" } }).then(() => { });
        else
          this.router.navigate(['/home/page'], { queryParams: { flowTempName: this.workflowType, id: this.urlParams.id, type: "email" } }).then(() => { });
        this.dialogDisplay = true;
      }
      return true;
    }
    return false;
  }

  public async ngOnInit() {
    await this.getFlowTemplateList();

    this.route.queryParams.subscribe(async res => {
      if (res['type']) {
        this.currPanel = 'edit';
        // this.workflowType = Number(res['type']);
        this.fromPage = res['name'];

        if (res['id']) {
          this.currRow = {
            dataId: res['id'],
            id: res['id'],
            flowTempName: res['flowtempname'],
          };
        } else if (res['taskid']) {
          this.currRow = {
            dataId: res['taskid'],
            taskid: res['taskid'],
            flowTempName: res['flowtempname'],
          };
        }
      }

      if (res['search']) {
        this.searchData.flowTempName = res['toDoType']
        this.searchData.applyStatus = res['status'] === 'true' ? 'Completed' : 'Running'
      }
    });

    if (this.isEmail()) {
      return;
    }

    if (this.urlParams['flowtempname']) {
      this.searchData.flowTempName = this.urlParams['flowtempname'];
    }


    this.currentLanguage = localStorage.getItem('lang');
    this.translate.use(this.currentLanguage);
    this.getData();
    await this.getUserFlowView();
    this.commonService.translateData(TranslateData, this.translate);
  }

  async getUserFlowView() {
    let { data } = await this.homeHttpService.getUserFlowView();
    this.workflowList = data;
  }

  async getFlowTemplateList() {
    let filter = { pageNum: 1, pageSize: 100, status: 1 };
    await this.http.getFlowTemplateList(filter).then((res: any) => {
      this.flowTempList = res.data || [];
      this.flowTempList.splice(0, 0, { flowName: 'label.all', id: '' })
    });
  }

  async getData(isPaginate?: boolean) {
    this.loading = true;
    if (!isPaginate) {
      this.searchData.pageNum = 1;
      this.searchData.first = 0;
    }
    this.searchData.flowStatus = this.applyStatusValue[this.searchData.applyStatus] || '';
    let { data } = await this.http.getWorkflowList(this.searchData);
    this.tableData = data?.data || [];
    this.recordCount = data?.total;
    this.loading = false;
  }

  search(isReset?: boolean) {
    if (isReset) {
      this.searchData = {
        flowNo: '',
        taskSubject: '',
        flowTempName: this.workflowType,
        applyStatus: "",
        flowStatus: "",
        first: 0,
        pageNum: 1,
        pageSize: 10,
      };
    }
    this.getData();
  }

  editData(item?: any, workflowType?: string) {
    if (item) {
      this.workflowType = item.flowTempName;
    } else if (workflowType) {
      this.workflowType = workflowType;
    }
    if (!item && this.workflowType == "ITPortal_DataAuthApplication") {
      return this.messageService.add({
        severity: 'error',
        summary: localStorage.getItem('lang') == 'en' ? 'Error' : '错误',
        detail: localStorage.getItem('lang') == 'en' ? "Please apply through the data asset query interface." : "请到数据资产查询界面申请",
      });
      return;
    } else
    if (!item && this.workflowType == "ITPortal_BusinessModelApplication") {
      return this.messageService.add({
        severity: 'error',
        summary: localStorage.getItem('lang') == 'en' ? 'Error' : '错误',
        detail: localStorage.getItem('lang') == 'en' ? "Please apply through the business model interface." : "请到业务模型界面申请",
      });
      return;
    } else if (item?.formUrl) {
      let url = item?.formUrl;
      let queryParams = { id: item.id, flowTempName: item.flowTempName, type: 'email' }
      //检查url是否带参数
      if (url.indexOf('?') > -1) {
        queryParams = { ...this.commonHttpService.getUrlParams(url), ...queryParams }
        url = url.split('?')[0];
      }
      this.router.navigate([url], { queryParams: queryParams });
      return;
    }
    this.workflowName = item?.formNo
    this.currPanel = 'edit';
    this.isCommon = item?.isCommon;
    this.currRow = item;
  }

  paginate(event: any) {
    this.searchData.pageNum = event.page + 1;
    this.searchData.pageSize = event.rows;
    this.searchData.first = event.first;
    this.getData(true);
  }


  onchangeFlowTemp(event: any) {
    this.searchData.flowTempId = event.value;
    this.searchData.flowTempName = event.label;
  }

  goHome() {
    if (this.currPanel === 'edit') {
      this.currPanel = 'list'
      this.search()
      return
    }
    this.router.navigate(['/home/page']);
  }

  login() {
    if (!this.formData.name || !this.formData.password) {
      return;
    }
    this.submitLoading = true;
    this.jbService.setToken(
      '/gateway/basic/accounts/login',
      {
        name: this.formData.name,
        password: this.formData.password,
      },
      (data: any) => {
        this.submitLoading = false;
        if (data.code !== 0 && !data.data) {
          this.messageService.add({ severity: 'warn', summary: TranslateData.loginFail });
          return;
        }
        location.reload();
      }
    );
  }

  closePanel(e: any) {
    this.currPanel = 'list'
    this.search()
  }
}
