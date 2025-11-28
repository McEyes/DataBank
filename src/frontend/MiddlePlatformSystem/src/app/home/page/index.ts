import { Component, Inject, OnInit, ViewChild, ViewContainerRef } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import { TranslateService } from '@ngx-translate/core';
import HomeHttpService from 'src/api/home';
import CommonHttpService from 'src/api/common';
import { TranslateData } from 'src/app/core/translate/translate-data';
import { environment } from 'src/environments/environment';
import { Router } from '@angular/router';
import * as echart from 'echarts';
import WorkflowHttpService from 'src/api/home/workflow';
import { AnyARecord } from 'dns';
import { DOCUMENT } from '@angular/common';
import { debounceTime, fromEvent } from 'rxjs';
import { CommonService } from 'jabil-bus-lib';
import { ConfirmationService, MessageService } from 'primeng/api';
import DictHttpService from 'src/api/common/dict';
import DocHttpService from 'src/api/systemManage/docApi';
// import { DynamicComponentLoaderService } from 'src/api/common/DynamicLoaderService';

@Component({
  selector: 'jabil-bus',
  templateUrl: './index.html',
  styleUrls: ['./index.scss'],
  providers: [HomeHttpService,
    ConfirmationService, CommonHttpService, DictHttpService, DocHttpService]
})
export class HomeComponent implements OnInit {

  currentLanguage: any = 'en';
  loading: boolean = false;

  userInfo: any = {}
  hasPromise: boolean = true
  standardList: any[] = []
  callList: any[] = []
  myTodoList: any[] = []
  myRequestList: any[] = []
  todoList: any[] = []
  departmentList: any[] = []
  documentList: any[] = []
  department: string = ''
  statsObj: any = {
    topicCount: 0,
    dataSourceCount: 0,
    tableCount: 0,
    columnCount: 0,
    apiCount: 0,
    apiVisited: 0
  }
  chart: any = null
  echartOption: any = {
    color: ['#008651', '#0164A1'],
    title: {
      show: false
    },
    tooltip: {
      trigger: 'axis',
      axisPointer: {
        type: 'shadow'
      }
    },
    legend: {},
    grid: {
      left: '3%',
      right: '4%',
      bottom: '3%',
      containLabel: true
    },
    yAxis: {
      type: 'value',
      boundaryGap: [0, 0.01]
    },
    xAxis: {
      type: 'category',
      data: ['03/01/2025', '03/02/2025', '03/03/2025', '03/04/2025', '03/05/2025', '03/06/2025']
    },
    series: [
      // {
      //   name: 'Business Service',
      //   type: 'bar',
      //   barMaxWidth: '30',
      //   data: [18203, 23489, 29034, 104970, 131744, 630230]
      // },
      {
        name: 'Data Service',
        type: 'bar',
        barMaxWidth: '30',
        data: [19325, 23438, 31000, 121594, 134141, 681807]
      }
    ]
  };
  reqStatusList: any[] = [{ name: this.currentLanguage == 'zh' ? '全部' : 'All', value: '' }, { name: this.currentLanguage == 'zh' ? '审核中' : 'Pending', value: '1' }, { name: this.currentLanguage == 'zh' ? '已完成' : 'Completed', value: '99' }]
  resSearchData: any = { flowStatus: '1', taskSubject: '', pageindex: 1, pagesize: 5 }

  taskStatusList: any[] = [{ name: this.currentLanguage == 'zh' ? '全部' : 'All', value: '' }, { name: this.currentLanguage == 'zh' ? '待审批' : 'Pending', value: '0' }, { name: this.currentLanguage == 'zh' ? '已审批' : 'Approved', value: '2' }]
  taskSearchData: any = { status: '0', title: '', pageindex: 1, pagesize: 5 }

  applyStatusI18NKey: any = {
    Draft: 'label.workflow.status.draft',
    Running: 'home.workflow.status.processing',
    Stop: 'label.workflow.status.stop',
    Aprd: 'home.workflow.status.processed',
    Completed: 'home.workflow.status.finish',
    Reject: 'home.workflow.status.reject',
    Cancel: 'label.workflow.status.canceled',
  };
  taskStatusI18NKey: any = {
    Todo: 'label.task.status.approval',
    Audit: 'label.task.status.approved',
    Transfer: 'label.task.status.transfer',
    Error: 'label.task.status.Error',
    RejectStart: 'label.task.status.RejectStart',
    Reject: 'label.task.status.reject',
    Cancel: 'label.task.status.canceled',
  };
  applyStatusValue: any = {
    Draft: 0,
    Running: 1,
    Completed: 99,
    // Reject: 'home.workflow.status.reject',
    Cancel: -1,
    Stop: -99,
  };

  //权限申请
  formData: any = {
    invalid: false,
    applicant: '',
    applicantName: '',
    reason: '',
  }
  tip: string = 'Tip';
  flowData: any = {}
  isApprove: boolean = false
  remarkValue: string = ""

  @ViewChild('flowformHost', { read: ViewContainerRef }) flowformHost: ViewContainerRef;
  constructor(
    private translate: TranslateService,
    private httpService: HomeHttpService,
    private flowhttpService: WorkflowHttpService,
    private commonHttpService: CommonHttpService,
    private commonService: CommonService,
    private messageService: MessageService,
    private sanitizer: DomSanitizer,
    private router: Router,
    private dictHttp: DictHttpService,
    private confirmationService: ConfirmationService,
    private docHttpService: DocHttpService,
    // private dynamicComponentLoader: DynamicComponentLoaderService
    //@Inject(DOCUMENT) private document: Document
  ) {

  }
  async loadComponentFromDatabase() {
    try {
      // 从数据库获取组件代码（实际应用中替换为API调用）
      const { componentCode, styleCode, templateCode } = await this.fetchComponentFromDatabase();

      // // 动态加载组件
      // const componentRef = await this.dynamicComponentLoader.loadComponent(
      //   this.flowformHost,
      //   componentCode,
      //   styleCode,
      //   templateCode
      // );

      // console.log('组件加载成功:', componentRef);
    } catch (error) {
      console.error('加载组件失败:', error);
    }
  }
  private async fetchComponentFromDatabase() {
    // 模拟从数据库获取组件代码
    // 实际应用中应替换为HTTP请求
    return {
      componentCode: `console.log('动态组件初始化');
                      this.dynamicProperty = '动态属性值';
                      this.doSomething = () => alert('动态方法调用');`,
      styleCode: `h3 { color: blue; }`,
      templateCode: `<h3>动态加载的组件</h3>
                    <p>属性值: {{dynamicProperty}}</p>
                    <button (click)="doSomething()">调用动态方法</button>`
    };
  }


  public async ngOnInit() {
    this.currentLanguage = localStorage.getItem('lang');
    this.translate.use(this.currentLanguage);
    this.reqStatusList = [{ name: this.currentLanguage == 'zh' ? '全部' : 'All', value: '' }, { name: this.currentLanguage == 'zh' ? '审批中' : 'Pending', value: '1' }, { name: this.currentLanguage == 'zh' ? '已完成' : 'Completed', value: '99' }]
    this.taskStatusList = [{ name: this.currentLanguage == 'zh' ? '全部' : 'All', value: '' }, { name: this.currentLanguage == 'zh' ? '待审批' : 'Pending', value: '0' }, { name: this.currentLanguage == 'zh' ? '已审批' : 'Approved', value: '2' }]

    setTimeout(() => {
      this.initRoute();
      fromEvent(window, 'resize')
        .pipe(debounceTime(1000))
        .subscribe(res => {
          this.initEChart()
        });
      this.initEChart()
    }, 100)
  }

  async initRoute() {

    this.userInfo = await this.commonHttpService.getCurrUser();
    this.hasPromise = true;// this.userInfo.roles?.length > 0;
    if (this.hasPromise) {
      var urlParams = this.getUrlParams(window.location.hash);
      if (urlParams["type"] == "email") {
        this.isApprove = true;
        var id = urlParams["id"] ?? urlParams["taskid"];
        var type = urlParams["taskid"] ? "task" : "flow";
        this.flowhttpService.getFlowFormDetails(id, type).then((res: any) => {
          this.flowData = res.data;
          this.formData = JSON.parse(this.flowData.formData);
          if (this.flowData.auditRecords.length > 0) {
            this.remarkValue = this.flowData.auditRecords[this.flowData.auditRecords.length - 1].auditContent;
          }
        });
      }
      else
        this.initData()
    }
    else {
      this.formData.applicant = this.userInfo.id
      this.formData.applicantName = this.userInfo.name
      this.flowhttpService.getMyRequestsList({ pageNum: 1, pageSize: 1 }).then((res: any) => {
        if (res.data.total > 0) {
          this.flowhttpService.getFlowFormDetails(res.data.data[0].id).then((res: any) => {
            if (res.data.flowTempName == "ITPortal_UserPermissionApplication") {
              this.flowData = res.data;
              this.formData = JSON.parse(this.flowData.formData);
              if (this.flowData.auditRecords.length > 0) {
                this.remarkValue = this.flowData.auditRecords[this.flowData.auditRecords.length - 1].auditContent;
              }
            }
          });
        }
      });
    }
  }


  goBack() {
    this.router.navigate(['/'], { queryParams: { ts: new Date().getTime() } });
  }

  getUrlParams(url: string) {
    let urlStr = url.split('?')[1];
    if (!urlStr) return {};
    let obj: any = {};
    let paramsArr = urlStr.split('&');
    for (let i = 0, len = paramsArr.length; i < len; i++) {
      let arr = paramsArr[i].split('=');
      obj[arr[0]] = arr[1];
    }
    return obj;
  }
  initData() {
    this.getDepartment()
    this.getCategoryStats()
    this.getStandardizationStats()
    this.getTableStats()
    this.getApiDailyStats()
    this.getSelfTodoList();
    this.getMyRequestsList();
    this.getDocumentList();



    setTimeout(() => {
      this.initEChart()
    }, 1000)

  }

  async getDocumentList() {
    let _this = this;
    await this.docHttpService.list({ pageSize: 20, pageIndex: 1, catalog: "Document", status: true }).then((res: any) => {
      _this.documentList = res.data
      _this.documentList.forEach((item: any) => {
        item.docIconUrl = item.docIconUrl || 'icon_doc_default.png'
        item.displayName = this.currentLanguage == 'zh' ? item.displayNameCn : item.displayNameEn
      })
    });
  }

  async getSelfTodoList() {
    let _this = this;
    await this.flowhttpService.getSelfTodoList({ pageSize: 5, pageIndex: 1 }).then((res: any) => {
      _this.myTodoList = res.data.data
    });
  }
  async getMyRequestsList() {
    let _this = this;
    await this.flowhttpService.getMyRequestsList(this.resSearchData).then((res: any) => {
      _this.myRequestList = res.data.data
    });
  }
  async getTodoList() {
    let _this = this;
    await this.flowhttpService.getTodoList(this.taskSearchData).then((res: any) => {
      _this.myTodoList = res.data.data
    });
  }
  onChangeResData(event: any) {
    this.getMyRequestsList();
  }

  onChangeTaskData(event: any) {
    this.getTodoList();
  }


  async getDepartment() {
    const res = await this.commonHttpService.getDepts();
    this.departmentList = [{ name: TranslateData.all, id: '' }]
    res.data.forEach((ele: any) => {
      this.departmentList.push({ name: ele, id: ele })
    });
  }

  async getCategoryStats() {
    const res = await this.httpService.getCategoryStats();
    this.statsObj = res.data
  }

  async getStandardizationStats() {
    const res = await this.httpService.getStandardizationStats({ dept: this.department })
    this.standardList = res.data
  }

  async getTableStats() {
    const res = await this.httpService.getTableStats()
    this.callList = res.data
  }
  async getApiDailyStats() {
    await this.httpService.getApiDailyStats({ day: 25 }).then((res: any) => {
      this.echartOption.xAxis.data = res.data.map((item: any) => item.dailyStr)
      this.echartOption.series[0].data = res.data.map((item: any) => item.visited)
      this.initEChart();
    });
  }

  numberToThousands = (num: string | number) => {
    num = (num || 0).toString()
    let result = '';
    while (num.length > 3) {
      result = ',' + num.slice(-3) + result;
      num = num.slice(0, num.length - 3);
    }

    if (num) {
      result = num + result;
    }

    return result;
  }

  goWorkFlow(flowType: string = 'ITPortal_LakeEntryApplication') {
    this.router.navigate(['/home/workflow'], { queryParams: { flowTempName: flowType } }).then(() => { });
    // window.open(environment.JabilBusServer + '/modules/home/#/common/workflow?from=dataGovernance&type=10', '_blank')
  }

  async goWorkFlowDetail(item: any, type: string = 'flow') {
    let url = '/home/workflow'
    let queryParams = { flowTempName: item.flowTempName, type: "email" }
    if ((type == 'todo' || type == 'task') && item.flowTempName != "ITPortal_UserPermissionApplication") {
      let res = await this.flowhttpService.getFlowFormDetails(item.id, type);
      item.formUrl = res.data.formUrl
    }
    if (item.formUrl) {
      url = item.formUrl
    }
    //检查url是否带参数
    if (url.indexOf('?') > -1) {
      queryParams = { ...this.getUrlParams(url), ...queryParams }
      url = url.split('?')[0];
    }

    if (type == 'flow') {
      if (item.flowTempName == "ITPortal_UserPermissionApplication") {
        this.router.navigate(["/home/flowform"], { queryParams: { ...queryParams, flowTempName: item.flowTempName, id: item.id, type: "email", callbak: "/home/page" } }).then(() => { });
      } else
        this.router.navigate([url], { queryParams: { ...queryParams, flowTempName: item.flowTempName, id: item.id, type: "email" } }).then(() => { });
    } else if (type == 'todo' || type == 'task') {
      if (item.flowTempName == "ITPortal_UserPermissionApplication") {
        this.router.navigate(["/home/flowform"], { queryParams: { ...queryParams, flowTempName: item.flowTempName, taskid: item.id, type: "email", callbak: "/home/page" } }).then(() => { });
      } else
        this.router.navigate([url], { queryParams: { ...queryParams, flowTempName: item.flowTempName, taskid: item.id, type: "email" } }).then(() => { });
    } else this.router.navigate([url], { queryParams: { ...queryParams, flowTempName: item?.flowTempName ?? "" } }).then(() => { });
  }

  goAssetQuery() {
    this.router.navigate(['/dataAsset/assetQuery'], { queryParams: {} }).then(() => { });
    // window.open(environment.JabilBusServer + '/modules/home/#/common/workflow?from=dataGovernance&type=10', '_blank')
  }

  initEChart() {
    let edom = document.getElementById('chart') as HTMLDivElement;
    if (edom) {
      if (!this.chart) this.chart = echart.init(edom);
      this.chart.setOption(this.echartOption);
    }
  }

  submitApply() {
    const valid = ['applicant', 'reason'];
    if (this.commonService.isInvalid(this.formData, valid)) {
      this.formData.invalid = true;
    } else {
      this.loading = true;
      var flowData = {
        "flowTempName": "ITPortal_UserPermissionApplication",
        "formData": this.formData,
        "applicant": this.userInfo.id,
        "applicantName": this.userInfo.name,
      };
      this.flowhttpService.startFlow(flowData).then((res: any) => {
        this.formData.result = res;
        this.loading = false;
        this.messageService.add({
          severity: res.success ? 'success' : 'error',
          summary: TranslateData.save,
          detail: res.success ? TranslateData.success : res.msg,
        });
      });
    }
  }

  // 审批按钮
  async submit(type: string) {
    let typeInfo: any = {
      "agree": this.currentLanguage == 'en' ? 'agree' : '同意',
      "reject": this.currentLanguage == 'en' ? 'reject' : '拒绝',
      "return": this.currentLanguage == 'en' ? 'return' : '驳回',
      "cancel": this.currentLanguage == 'en' ? 'cancel' : '取消',
    };
    if (!this.remarkValue) {
      this.messageService.add({ severity: 'warn', summary: TranslateData.fillComment ?? 'Please fill in the comment' });
      return;
    }

    let params = { ...this.formData };

    this.confirmationService.confirm({
      message:
        this.currentLanguage == 'en'
          ? `May I ask if you are sure you want the approval for ${typeInfo[type]}`
          : `请问是否确定要${typeInfo[type]}该条审批`,
      rejectLabel: localStorage.getItem('lang') == 'en' ? 'No' : '否',
      acceptLabel: localStorage.getItem('lang') == 'en' ? 'Yes' : '是',
      accept: async () => {
        this.loading = true;

        const data = {
          flowInstId: this.flowData.id,
          acttype: type,
          actOperate: type,
          auditContent: this.remarkValue,
        };
        const res = await this.flowhttpService.sendApprove(data);
        if (res.success) {
          this.loading = false;
          this.goBack();
          this.messageService.add({
            severity: 'success',
            summary: TranslateData.success,
          });
        } else {
          this.loading = false;
          this.messageService.add({
            severity: 'fail',
            summary: TranslateData.fail + ". " + res.msg,
          });
        }
      },
    });
  }


  logout() {
    localStorage.clear();
    window?.parent?.location?.reload()
    window.location.reload()
  }


  reApply() {
    this.flowData = {};
    this.isApprove = false;
    this.formData.reason = "";
  }

  downDoc(item: any) {
    window.open(environment.FileServer + "/api/file/download/" + item.url + "?category=Document&downName=" + item.displayName, "_blank")
  }
  docIcon(url: string) {
    if (url.indexOf('icon_doc') >= 0) return './assets/images/home/' + url
    return environment.FileServer + "/api/file/download/" + url + "?category=Document/Icon";
  }


  downloadStandardizationStats() {
    this.commonHttpService.download(environment.BasicServer + "/api/HomeReport/downstandardizationstats", '数据标准化统计');
  }

  downloadTableStats() {
    this.commonHttpService.download(environment.BasicServer + "/api/HomeReport/downTableStats", '数据调用统计');
  }

  async downloadExcel() {
    const params = {
      pageNum: "1",
      pageSize: "10000",
      sqlText: "select distinct table_name 表名称啊,table_comment 备注啊 from metadata_table_view",
      total: 1,
      to_excel: "1",// 必填，且必须为字符串
      //excel_name:"下载文件名",//可为空， 为空时，默认api名称
      //column_info:"格式化配置：  列名1=列导出名称1,字符串格式;列名2=列导出名称2;列名3;" //可为空  ;分号分隔列，=号列名和显示名映射配置，逗号(,)后面时字符串格式化配置，为空时采用table配置的字段名解析
    };
    // http://hua-databank.corp.JABIL.ORG/gateway/dataasset/services/v1.0.0/ITPortal/DataBank/metadata_table_view/sqlQuery
    const url = "http://localhost:6010/services/v1.0.0/ITPortal/DataBank/metadata_table_view/sqlQuery";
    this.commonHttpService.exportExcel(url, '数据调用统计222', params, "POST", "660423642c7b070cab3e9f271ce5d662");
  }
}

