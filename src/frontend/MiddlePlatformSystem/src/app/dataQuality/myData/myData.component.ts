import { Component, OnInit, Output, EventEmitter, OnDestroy } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { MessageService, ConfirmationService } from 'primeng/api';
import { ActivatedRoute } from '@angular/router';
import WorkflowHttpService from 'src/api/home/workflow';
import { TranslateData } from 'src/app/core/translate/translate-data';
import { CommonService, LocalStorage, JBService } from 'jabil-bus-lib';
import RuleMyDataHttpService from 'src/api/dataQuality/myData';
import RuleHttpService from 'src/api/dataQuality/rule';
import CommonHttpService from 'src/api/common';

@Component({
  selector: 'jabil-data-quality-my-data',
  templateUrl: './myData.component.html',
  styleUrls: ['./myData.component.scss'],
  providers: [
    LocalStorage,
    MessageService,
    ConfirmationService,
    CommonService,
    WorkflowHttpService,
    JBService,
    RuleMyDataHttpService,
    RuleHttpService
  ],
})
export class DataQualityMyDataComponent implements OnInit, OnDestroy {
  @Output() closePanelreturn = new EventEmitter<string>();
  loading!: boolean;
  currentLanguage: any = 'en';
  recordCount: number = 0;
  progress: number = 0;
  workflowType: string = "ITPortal_LakeEntryApplication";
  isCommon: boolean = false;
  currPanel: string = 'list';
  fromPage: string = 'list';
  submitLoading: boolean = false;
  tableData: Array<any> = [];

  searchData: any = {
    tableName: '',
    owner: null,
    dept: null,
    scoreFrom: null,
    scoreTo: null,
    pageIndex: 1,
    pageSize: 10,
  };
  selectedRows: Array<any> = [];
  workflowList: Array<any> = [];
  flowTempList: Array<any> = [];
  timeout: any = null;
  currRow: any = null;

  applyStatusI18NKey: any = {
    0: 'label.disable',
    1: 'label.enable'
  };

  items: Array<any> = [];
  hideReturn: boolean = false;
  dialogDisplay: boolean = false;
  formData: any = { id: '', userName: '', name: '', password: '' };
  isShowReturn: boolean = true;
  hasConfigAuth: boolean = false;
  dataDialog: boolean = false;
  dataLoading: boolean = false;
  workflowName: string = ''
  selectValueName: string = ''
  checkDataTableSelects: any[] = []

  currTablRuleIds: any[] = []
  timeoutId: any = null
  dialogRuleName: string = ''
  tableName: string = ''
  userList: any[] = []
  deptList: any[] = []
  reportDialog: boolean = false
  reportDataTable: any[] = []
  constructor(
    private readonly http: RuleMyDataHttpService,
    private readonly translate: TranslateService,
    private readonly messageService: MessageService,
    private readonly commonService: CommonService,
    public readonly route: ActivatedRoute,
    private readonly ruleHttpService: RuleHttpService,
    private readonly commonHttpService: CommonHttpService,
  ) {
    this.getUserList()
    this.getDeptList()
  }

  public async ngOnInit() {
    this.currentLanguage = localStorage.getItem('lang');
    this.translate.use(this.currentLanguage);
    this.getData(false);
    this.commonService.translateData(TranslateData, this.translate);
  }

  ngOnDestroy(): void {
    this.timeoutId && clearInterval(this.timeoutId)
  }

  async getData(isPaginate: boolean, noLoading?: boolean) {
    this.loading = !noLoading;
    if (!isPaginate) {
      this.searchData.pageIndex = 1;
      this.searchData.first = 0;
    }

    const searchData = JSON.parse(JSON.stringify(this.searchData))
    searchData.dept = searchData.dept ?? ''
    searchData.owner = searchData.owner ?? ''
    searchData.scoreFrom = searchData.scoreFrom ?? null
    searchData.scoreTo = searchData.scoreTo ?? null
    let { data } = await this.http.getTableData(searchData);
    //  如果是检查结果，局部更新状态

    if (noLoading && this.tableData.length > 0) {
      // 匹配数据，防止页面跳动刷新
      for (let i = 0; i < this.tableData.length; i++) {
        if (this.tableData[i]?.tableId === data?.data[i]?.tableId) {
          this.tableData[i].status = data?.data[i]?.status
          this.tableData[i].score = data?.data[i]?.score
          this.tableData[i].lastScore = data?.data[i]?.lastScore
        } else {
          // 查询数据出现变化整体替换数据
          this.tableData = data?.data || [];
          break
        }
      }
    } else {
      this.tableData = data?.data || [];
    }
    if (data?.data) {
      this.searchResult()
    }

    this.recordCount = data.totalCount;
    this.loading = false;
  }

  search(isReset?: boolean) {
    if (isReset) {
      this.searchData = {
        tableName: '',
        scoreFrom: null,
        scoreTo: null,
        dept: null,
        owner: null,
        first: 0,
        pageIndex: 1,
        pageSize: 10,
      };
    }
    this.getData(false);
  }

  async editData(item?: any, act?: string) {
    this.currRow = item;
    this.getRulesTableData()
    this.dataDialog = true
  }

  async getRulesTableData() {
    const res = await this.ruleHttpService.getAllRules({ ruleName: this.dialogRuleName })
    this.checkDataTableSelects = res?.data
    this.getSelectRulesId()
  }

  async getSelectRulesId() {
    const res = await this.ruleHttpService.getTableRules({ tableId: this.currRow.tableId })
    this.currTablRuleIds = []
    this.checkDataTableSelects.forEach((item: any) => {
      res.data.forEach((cItem: any) => {
        if (item.id === cItem.ruleNo) {
          item.weight = cItem.weight
          item.bind = true
        }
      })
    });
  }

  paginate(event: any) {
    this.searchData.pageIndex = event.page + 1;
    this.searchData.pageSize = event.rows;
    this.searchData.first = event.first;
    this.getData(true);
  }

  allClose() {
    this.dataDialog = false;
  }

  onBlurSelectList() {
    this.getRulesTableData()
  }
  onResetSelectList() {
    this.dialogRuleName = ''
    this.getRulesTableData()
  }

  executeCheck(row: any) {
    if (row.status === 'InEvaluation') {
      this.messageService.add({
        severity: 'info',
        summary: TranslateData.checkingWait,
      });
      return
    }

    row.status = 'InEvaluation'

    this.http.addToEvaluate({ tableId: row.tableId }).then((resSave: any) => {
      if (resSave.succeeded) {
        this.messageService.add({
          severity: 'success',
          summary: TranslateData.checkingWait,
          detail: TranslateData.success,
        });
        this.searchResult()
      } else {
        this.messageService.add({
          severity: 'error',
          summary: TranslateData.executionFail,
          detail: resSave.msg,
        });
      }
      this.loading = false;
    });
  }

  checkStopSearch() {
    let res = true
    this.tableData.forEach((item: any) => {
      if (item.status === 'Waiting' || item.status === 'InEvaluation') {
        res = false
      }
    })
    return res
  }

  searchResult() {
    this.timeoutId && clearInterval(this.timeoutId)
    this.timeoutId = setInterval(() => {
      if (this.checkStopSearch()) {
        clearInterval(this.timeoutId)
        this.getData(true, true)
        return
      }

      this.getData(true, true)
    }, 5 * 1000)
  }

  async bindRule(row: any) {
    let rules: any[] = []

    if (!row.bind) {
      rules = [{
        ruleNo: row.id,
        weight: row.weight
      }]
    } else {
      row.bind = false
    }


    this.checkDataTableSelects.forEach((item: any) => {
      if (item.bind) {
        rules.push({
          ruleNo: item.ruleNo,
          weight: item.weight
        })
      }
    })
    this.ruleHttpService.saveTableRules({
      tableId: this.currRow.tableId,
      rules
    }).then((resSave: any) => {
      if (resSave.succeeded) {
        this.messageService.add({
          severity: 'success',
          summary: TranslateData.save,
          detail: TranslateData.success,
        });
        this.getSelectRulesId()
      } else {
        this.messageService.add({
          severity: 'error',
          summary: TranslateData.fail,
          detail: resSave.errors,
        });
      }
      this.loading = false;
    });
  }

  submitRule() {
    this.loading = true;
    let rules: any[] = []
    this.checkDataTableSelects.forEach((item: any) => {
      if (item.weight) {
        rules.push({
          ruleNo: item.id,
          weight: item.weight
        })
      }
    })

    this.ruleHttpService.saveTableRules({
      tableId: this.currRow.tableId,
      rules
    }).then((resSave: any) => {
      if (resSave.succeeded) {
        this.messageService.add({
          severity: 'success',
          summary: TranslateData.save,
          detail: TranslateData.success,
        });
        this.dataDialog = false
        // this.getSelectRulesId()
      } else {
        this.messageService.add({
          severity: 'error',
          summary: TranslateData.fail,
          detail: resSave.errors,
        });
      }
      this.loading = false;
    });
  }

  // 用户列表
  getUserList() {
    if (sessionStorage.getItem('userList')) {
      let userListString: any = sessionStorage.getItem('userList');
      let userList = JSON.parse(userListString);
      this.userList = userList;
    } else {
      this.loading = true;
      this.commonHttpService.getUserList().then((res: any) => {
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
    this.deptList = []
    const res = await this.http.getDepartments({})
    res.data.forEach((item: string) => {
      this.deptList.push({
        name: item,
        id: item
      })
    })
  }

  async report(row: any) {
    this.tableName = row.tableName
    const res = await this.http.getReport({ tableId: row.tableId })
    this.reportDataTable = res?.data ?? []
    this.reportDialog = true
  }

  getScoreColor(score: number) {
    const s = Number(score)

    if(s >= 85) {
      return 'green'
    }else if(s < 70) {
      return 'red'
    }else {
      return 'yellow'
    }
  }
}
