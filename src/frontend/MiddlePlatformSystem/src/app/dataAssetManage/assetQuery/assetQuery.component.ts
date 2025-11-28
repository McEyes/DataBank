import { Component, OnInit } from '@angular/core';
import { MenuItem, SortEvent, TreeNode } from 'primeng/api';
import { MessageService, ConfirmationService } from 'primeng/api';
import { ViewEncapsulation } from '@angular/core';
import { en_US, NzI18nService, zh_CN } from 'ng-zorro-antd/i18n';
import { TranslateService } from '@ngx-translate/core';
import { Router } from '@angular/router';
import { CommonService } from 'jabil-bus-lib';
import { TranslateData } from 'src/app/core/translate/translate-data'
import AssetQueryHttpService from 'src/api/dataAssetManage/assetQuery'
import AuthorityService from 'src/api/dataAssetManage/authority'
import CommonHttpService from 'src/api/common'
import RuleMyDataHttpService from 'src/api/dataQuality/myData';
import DictHttpService from 'src/api/common/dict';
@Component({
  selector: 'app-assetquery',
  templateUrl: './assetquery.component.html',
  styleUrls: ['./assetquery.component.scss'],
  encapsulation: ViewEncapsulation.Emulated,
  providers: [
    MessageService,
    AssetQueryHttpService,
    ConfirmationService,
    AuthorityService,
    CommonHttpService,
    DictHttpService,
    RuleMyDataHttpService
  ],
})
export class AssetqueryComponent implements OnInit {
  loading: boolean = false;

  menuActive: string = '0';
  queryParams: any = {
    pageSize: 20,
    pageNum: 1,
  };
  total: number = 0;
  first: number = 0;
  defaultParams: any = {};
  selectedFile: any;
  treeLoaing: boolean = false;
  treeData: any[] = [];
  tableData: TreeNode[] = [];
  selectedCustomers: any[] = [];
  sendType: string = 'only';
  sendList: any[] = [];
  dialogValue: boolean = false;
  dialogForm: any = {
    userId: '',
    userName: '',
    reason: '',
    ownerId: '',
    ownerName: '',
  };
  dialogInvalid: boolean = false;
  stepNumber: number = 1;
  userInfo: any = {};
  langType: string = 'en';
  sendMessage: string = '';
  dialogviewApiValue: boolean = false;
  viewApiList: any[] = [];
  userList: Array<any> = [];
  levelObj: any = {
    '1': TranslateData.secretLevel1,
    '2': TranslateData.secretLevel2,
    '3': TranslateData.secretLevel3,
    '4': TranslateData.secretLevel4,
    '5': TranslateData.secretLevel5,
  }
  tagList: any[] = [];
  tabsNavNumber: number = 0;
  timeout: any = null;
  baseTreeData: any[] = [];
  treeQuery: string = '';
  updateMethodObj: any = {
    manual: TranslateData.manual,
    automatic: TranslateData.automatic
  }
  currPanel: any = 'list'
  lang: any = 'en'
  params: any = {}
  showTour: boolean = true
  tableName: string = ''
  reportDataTable: any[] = []
  reportDialog: boolean = false
  selectItem: any = {}
  actionItems: MenuItem[] = [
    {
      label: 'Feedback',
      styleClass: 'sp-sub-button',
      tooltipOptions: {
        tooltipLabel: TranslateData.OperateFeedback,
      },
      icon: 'pi pi-fw pi-comment',
      command: () => {
        this.onAuthority(this.selectItem);
      }
    },
    {
      label: 'View Api',
      styleClass: 'sp-sub-button',
      tooltipOptions: {
        tooltipLabel: TranslateData.OperateViewApi,
      },
      icon: 'pi pi-ticket',//pi-ellipsis-h
      command: () => {
        this.viewApi(this.selectItem);
      }
    },
    {
      label: 'View',
      styleClass: 'sp-sub-button',
      tooltipOptions: {
        tooltipLabel: TranslateData.OperateView,
      },
      icon: 'pi pi-list',
      command: () => {
        this.tableBtn('details', this.selectItem);
      }
    },
    {
      label: 'Preview',
      styleClass: 'sp-sub-button',
      tooltipOptions: {
        tooltipLabel: TranslateData.OperatePreview,
      },
      icon: 'pi pi-eye',//pi-eye
      command: () => {
        this.tableBtn('data', this.selectItem);
      }
    }];

  dataCategoryList: any[] = []


  constructor(
    private messageService: MessageService,
    private confirmationService: ConfirmationService,
    public router: Router,
    private commonFunction: CommonService,
    private i18n: NzI18nService,
    private translate: TranslateService,
    private httpService: AssetQueryHttpService,
    private authorityService: AuthorityService,
    private commonHttpService: CommonHttpService,
    private dictService: DictHttpService,
    private readonly ruleMyDataHttpService: RuleMyDataHttpService,
  ) {
    window.addEventListener('resetPage', (e: any) => {
      this.currPanel = 'list'
    });

    this.lang = localStorage.getItem('lang') || 'en'
  }

  ngOnInit(): void {
    this.translate.use(localStorage.getItem('lang') || 'en');
    this.i18n.setLocale(this.langType === 'en' ? en_US : zh_CN);
    this.langType = localStorage.getItem('lang') || 'en';
    this.getDBTypeList();

    this.getTagList();
    //获取树状主题域
    this.getTree();
    //查询右边的列表
    this.queryTable();
    this.getUserInfo();
  }

  async getDBTypeList() {
    let res = await this.dictService.codes('data_category');
    if (res.success) {
      this.dataCategoryList = res.data.filter((f: any) => f.dictCode == 'data_category');
      // this.dataCategoryList.splice(0,0,{itemText:'All',itemValue:'',dictCode:'selectAll'})
      this.dataCategoryList.forEach((f: any) => {
        this.translate.get("dict." + f.dictCode + "." + f.itemValue).subscribe((res: string) => {
          if (res != "dict." + f.dictCode + "." + f.itemValue) f.itemText = res;
        });
      });
    }
  }

  nzPageIndexChange(index: number) {
    this.queryParams.pageNum = index;
    this.queryTable();
  }

  nzPageSizeChange(page: number) {
    this.queryParams.pageSize = page;
    this.queryTable();
  }

  // 标签数据
  getTagList() {
    this.httpService.getTagLists().then((res: any) => {
      if (res.code == 200) {
        let tagList = res.data;
        let newArr = [];
        if (tagList && tagList.length > 0) {
          newArr = tagList.map((item: any, index: number) => {
            return {
              label: item,
              value: index + '',
              checked: true,
            };
          });
        }
        this.tagList = [...newArr];
      } else {
        this.messageService.add({
          severity: 'error',
          summary: TranslateData.error,
          detail: res.msg,
        });
      }
    });
  }

  viewApi(info: any = null) {
    if (!info.data) {
      this.messageService.add({
        severity: 'warn',
        summary: TranslateData.tips,
        detail: TranslateData.noData,
      });
      return
    };
    if (info.data.includes(',')) {
      let apiList: any = info.data.split(',');
      this.viewApiList = apiList;
    } else {
      this.viewApiList = [info.data];
    }
    this.dialogviewApiValue = true;
  }

  copyText(text: string, https?: string): void {
    const textarea = document.createElement('textarea');
    textarea.value = this.replaceText(text, https);
    document.body.appendChild(textarea);
    textarea.select();
    document.execCommand('copy');
    document.body.removeChild(textarea);
    this.messageService.add({
      severity: 'success',
      summary: TranslateData.success,
      detail: TranslateData.copy,
    });
  }

  onCloseAPIDialog() {
    this.viewApiList = [];
    this.dialogviewApiValue = false;
  }

  tableBtn(type: string, info: any = null) {
    if (!info.id || (type == 'data' && info.reviewable != 1)){
      return this.messageService.add({
        severity: 'warn',
        summary: TranslateData.tips,
        detail: TranslateData.unReviewable
      });
    }
    info.title = type
    this.params = info
    this.currPanel = type === 'data' ? 'preview' : 'details'
  }

  async getUserInfo() {
    let { data } = await this.httpService.getUserInfo();
    // let { data: userData } = await this.httpService.userInfoExtend({ ntid: data.id });
    this.userInfo = data;
    this.dialogForm.userName = data.name;//`${userData.englishName}(${userData.workNTID})`
    this.dialogForm.userId = this.userInfo.id;
  }

  async getTree() {
    this.treeLoaing = true;
    let { data } = await this.httpService.getTree('topic');
    this.baseTreeData = data;
    this.treeData = this.transformArray(data);

    const index = this.treeData.findIndex((item: any) => {
      return item.key === '0730990311395954688'
    })
    this.treeData.splice(index, 1)
    this.treeLoaing = false;
  }

  transformArray(items: any[]) {
    return items.map((item: any) => {
      let newItem: any = {
        key: item.key,
        label: localStorage.getItem('lang') == 'en' ? item.code : item.label,
        count: item.count,
        datatype: item.type,
        expanded: !!item.expanded
      };
      if (item.children && item.children.length > 0) {
        newItem.children = this.transformArray(item.children);
      } else {
        newItem.children = [];
      }
      return newItem;
    });
  }

  // 选择标签过滤
  checkChange(e: any, info: any) {
    this.tagList.forEach((item: any) => {
      if (info.value == item.value) {
        item.checked = !item.checked;
      }
    });
    let newTag = this.tagList
      .filter((item: any) => !item.checked)
      .map((item: any) => item.label)
      .join(',');
    this.queryParams = {
      pageNum: 1,
      pageSize: 20,
    };
    this.queryParams.tag = newTag;
    this.queryTable();
  }

  // 选择查询方法
  changeTabset(e: any) {
    this.tabsNavNumber = e['index'];
    this.queryParams.ctlId = '';
    if (this.tabsNavNumber == 0 && this.queryParams.tag) {
      this.queryParams = {
        pageNum: 1,
        pageSize: 20,
      };
      this.tagList.forEach((item: any) => {
        item.checked = true;
      });
      this.queryParams.tag = '';
      this.queryTable();
    } else if (this.tabsNavNumber == 1 && this.queryParams.keyword) {
      this.queryParams.keyword = '';
      this.queryParams.pageNum = 1;
      this.queryParams.pageSize = 20;
      this.queryTable();
    }
  }

  queryTable() {
    this.loading = true;
    // const httpStr = this.tabsNavNumber == 0 ? 'getTopicTable' : 'getTableByTag'
    this.httpService.getTopicTable(this.queryParams).then((res: any) => {
      this.tableData = res.data.data.map((item: any) => {
        item._ctlName = this.langType == 'en' ? item.ctlCode : item.ctlName;
        item._levelName = this.levelObj[item.levelId];
        return item;
      });
      this.total = Number(res.data.total);
      this.loading = false;
    });
  }

  //搜索
  getList() {
    this.queryParams.pageNum = 1;
    this.queryParams.pageSize = 20;
    // this.queryParams.ctlId = '';
    this.queryTable();
  }

  // tree btn
  onClickMenuBtns(type: string) {
    this.menuActive = type;
    this.tabsNavNumber = 0
    if (this.menuActive == '0' && (this.queryParams.keyword || this.queryParams.tag)) {
      if (this.tagList.length > 0) {
        this.tagList.forEach((item: any) => {
          item.checked = true;
        });
      }
      this.queryParams.tag = ''
      this.queryParams.keyword = '';
      this.queryParams.pageNum = 1;
      this.queryParams.pageSize = 20;
      this.queryTable();
    }
    if (this.menuActive == '1' && this.selectedFile) {
      this.queryParams.ctlId = '';
      this.selectedFile = null;
      this.selectedCustomers = [];
      this.queryTable();
    }
  }

  nodeSelect(event: any) {
    if (event.node.datatype == "table") {
      this.queryParams.id = event.node.key;
      this.queryParams.ctlId = ""
    }
    else {
      this.queryParams.ctlId = event.node.key;
      this.queryParams.id = "";
    }
    this.queryParams.name = ''
    this.queryParams.keyword = '';
    console.log(this.selectedFile)
    this.queryTable();
  }

  xdLoading: boolean = false;
  onAuthority(item: any) {
    this.sendList = [];
    // this.xdLoading = true;

    if (!item.ownerId) {
      this.xdLoading = false;
      return this.messageService.add({
        severity: 'warn',
        summary: TranslateData.tips,
        detail: TranslateData.noApplication
      });
    }

    this.dialogForm.ownerId = item.ownerId;
    this.dialogForm.ownerName = item.ownerName;
    this.sendList.push(item);
    this.sendType = 'only';
    this.dialogValue = true;
    // let param = {
    //   userId: this.dialogForm.userId,
    //   userName: this.dialogForm.userName,
    //   tableList: this.sendType == 'only' ? this.sendList : this.selectedCustomers,
    // };
    // this.httpService.checkAuth(param).then((res: any) => {
    //   if (res.code == 200) {
    //     this.dialogValue = true;
    //     this.xdLoading = false;
    //   } else {
    //     this.xdLoading = false;
    //     this.messageService.add({
    //       severity: 'warn',
    //       summary: TranslateData.tips,
    //       detail: res.msg,
    //     });
    //   }
    // });
  }

  async getCurrUser() {
    let userData = { name: '', id: '' }
    try {
      userData = JSON.parse(localStorage.getItem('loginuser') || '')

    } catch (e) {
      console.log(e)
    }
    return userData;
  }
  onBath() {

    if (this.selectedCustomers.length == 0) {
      this.messageService.add({
        severity: 'warn',
        summary: TranslateData.tips,
        detail: TranslateData.selectDataApplication
      });
      return;
    }

    this.dialogForm.ownerId = this.selectedCustomers
      .filter((item: any) => item.ownerId)
      .map((item: any) => item.ownerId)
      .join(',');
    this.dialogForm.ownerName = this.selectedCustomers
      .filter((item: any) => item.ownerName)
      .map((item: any) => item.ownerName)
      .join(',');
    this.sendType = 'all';
    this.dialogValue = true;
  }

  onCloseDialog() {
    this.dialogValue = false;
    this.dialogInvalid = false;
    this.stepNumber = 1;
    this.dialogForm.ownerId = '';
    this.dialogForm.ownerName = '';
    this.dialogForm.reason = '';
  }

  onDataDialogSumit() {
    const valid = ['userName', 'ownerName', 'reason'];
    if (this.commonFunction.isInvalid(this.dialogForm, valid)) {
      this.dialogInvalid = true;
    } else {
      if(this.sendList.length == 0){
        this.messageService.add({
          severity: 'warn',
          summary: TranslateData.tips,
          detail: TranslateData.selectDataApplication
        });
        return;
      }
      this.stepNumber = 2;
      let param = {
        userId: this.dialogForm.userId,
        userName: this.dialogForm.userName,
        // title: this.dialogForm.reason,
        description: this.dialogForm.reason,
        objectId:   this.sendList[0].id,
        objectName:   this.sendList[0].tableComment,
        // tableList: this.sendType == 'only' ? this.sendList : this.selectedCustomers,
      };
      this.httpService.feedBack(param).then((res: any) => {
        if (res.success) {
          setTimeout(() => {
            this.stepNumber = 3;
            this.sendMessage = res.data;
          }, 1000);
        } else {
          this.stepNumber = 4;
          this.messageService.add({
            severity: 'error',
            summary: TranslateData.error,
            detail: res.data,
          });
        }
      });
    }
  }

  // tree search function
  getTreeData() {
    const baseTreeData = JSON.parse(JSON.stringify(this.baseTreeData))
    const filteredData = this.filterTreeData(baseTreeData, this.treeQuery);
    this.treeQuery && this.expandTreeNode(filteredData)
    this.treeData = this.transformArray(filteredData);
  }

  filterTreeData(data: any[], searchTerm: string): any[] {
    if (!searchTerm) {
      data.forEach((node) => {
        node.expanded = false
      })
      return data;
    }
    return data
      .map(node => {
        if (node.children) {
          node.expanded = true
          const filteredChildren = this.filterTreeData(node.children, searchTerm);
          if (filteredChildren.length) {
            return { ...node, children: filteredChildren };
          }
        }
        // const label = localStorage.getItem('lang') == 'en' ? node.code : node.label
        if (node?.code?.toLowerCase().includes(searchTerm.toLowerCase()) || node?.label?.toLowerCase().includes(searchTerm.toLowerCase())) {
          // 全部设为设置，匹配到的节点设为false，其余为true，为了后续构建数据
          node.expanded = false
          return node;
        }
        return null;
      })
      .filter(node => node !== null);
  }

  searchTree() {
    if (this.timeout !== null) {
      clearTimeout(this.timeout); // clear
    }

    this.timeout = setTimeout(() => {
      this.getTreeData();
    }, 1000);
  }

  // 子节点展开设为false
  collapsedTreeNode(tree: any[]) {
    tree.forEach(node => {
      node.expanded = false

      if (node.children && node.children.length > 0) {
        this.collapsedTreeNode(node.children)
      }
    })
  }

  // 为false的展开节点设为true，并将子节点设为false。
  expandTreeNode(tree: any[]): void {
    tree.forEach(node => {
      if (!node.expanded) {
        node.expanded = true
        this.collapsedTreeNode(node.children || [])
      } else {
        if (node.children && node.children.length > 0) {
          this.expandTreeNode(node.children)
        }
      }
    })
  }

  async goPerminssion(item: any) {
    if (!item.ownerId) {
      return this.messageService.add({
        severity: 'warn',
        summary: TranslateData.tips,
        detail: TranslateData.withoutApprover
      });
    }

    this.xdLoading = true;
    const tableList = [item]

    // const { data } = await this.commonHttpService.getUserInfoByNTID(this.userInfo.surName)
    let userData = await this.getCurrUser();

    this.authorityService.checkAuth({ userId: userData.id, tableList }).then((res: any) => {
      if (res.success) {
        this.currPanel = 'permission'
        this.params = item
        this.params.tableColumns = res.data[0]?.tableColumns || []
        this.xdLoading = false;
      } else {
        this.xdLoading = false;
        this.messageService.add({
          severity: 'warn',
          summary: TranslateData.tips,
          detail: res.msg,
        });
      }
    });
  }

  closeTour(e: any) {
    console.log(e)
    this.showTour = false
  }

  step1Click() {
    this.queryParams.ctlId = '0730990311395954688'
    this.selectedFile = {
      key: "0730990311395954688"
    }
    this.queryTable();
  }

  queryBtn() {
    this.queryParams.pageSize=20;
    this.queryParams.pageNum=1;
    this.queryParams = { ...this.queryParams, ...this.defaultParams };
    this.getList();
  }
  onReset() {
    this.defaultParams = {};
    this.queryParams = {
      pageSize: 20,
      pageNum: 1,
    };
    this.queryParams = { ...this.queryParams, ...this.defaultParams };
    this.getList();
  }

  convertHtml(data: string, https?: string) {
    // 正则表达式匹配 [方法]URL 格式，支持POST、GET、DELETE、PUT
    // 分组1: 匹配指定的HTTP方法
    // 分组2: 匹配后续的URL部分（直到逗号或字符串结束）
    const regex = /\[((?:POST|GET|DELETE|PUT))\]([^,]+)/g;

    // 替换函数 - 将匹配到的内容转换为带类名的span标签
    const result = data.replace(regex, (match, method, url) => {
      // 将方法名转为小写作为类名（post-item 或 get-item）
      const className = `${method.toLowerCase()}-item`;
      if (https == "https") url = url.replace('http://', 'https://')
      if (https == "http") url = url.replace('https://', 'http://')
      return `<span class="${className}">${url}</span>`;
    });
    return result;
  }

  replaceText(data: string, https?: string) {
    // 正则表达式匹配 [方法]URL 格式，支持POST、GET、DELETE、PUT
    // 分组1: 匹配指定的HTTP方法
    // 分组2: 匹配后续的URL部分（直到逗号或字符串结束）
    const regex = /\[((?:POST|GET|DELETE|PUT))\]([^,]+)/g;

    // 替换函数 - 将匹配到的内容转换为带类名的span标签
    const result = data.replace(regex, (match, method, url) => {
      if (https == "https") url = url.replace('http://', 'https://')
      if (https == "http") url = url.replace('https://', 'http://')
      return url;
    });
    return result;
  }

  onShowOperate(event: any, item: any) {
    this.selectItem = item;
  }

  async report(row: any) {
    this.tableName = row.tableName
    const res = await this.ruleMyDataHttpService.getReport({ tableId: row.id })
    this.reportDataTable = res?.data ?? []
    this.reportDialog = true
  }

  getScoreColor(score: number) {
    const s = Number(score)

    if (s >= 85) {
      return 'green'
    } else if (s < 70) {
      return 'red'
    } else {
      return 'yellow'
    }
  }
  customSort(event: SortEvent) {
    var sortField = event.field + (event.order == 1 ? " DESC" : " ASC");
    if (this.queryParams.orderField != sortField) {
      this.queryParams.orderField = sortField;
      this.getList()
    }
  }
}
