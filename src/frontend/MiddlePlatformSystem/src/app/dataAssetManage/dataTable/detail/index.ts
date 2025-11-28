import { Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { MessageService, ConfirmationService } from 'primeng/api';
import { Router, ActivatedRoute } from '@angular/router';
import { Location } from '@angular/common';
import { CommonService } from 'jabil-bus-lib';
import { TranslateData } from 'src/app/core/translate/translate-data';

import DataSourceHttpService from 'src/api/dataAssetManage/dataSource';
import DataTableHttpService from 'src/api/dataAssetManage/dataTable';
import DataColumnHttpService from 'src/api/dataAssetManage/dataColumn';
import DictHttpService from 'src/api/common/dict';
import TopicHttpService from 'src/api/dataAssetManage/topic';
import CommonHttpService from 'src/api/common';
import { TranslateService } from '@ngx-translate/core';
import DataClientsHttpService from 'src/api/dataAssetManage/dataClients';

@Component({
  selector: 'app-datatable-details',
  templateUrl: './index.html',
  styleUrls: ['./index.scss'],
  providers: [
    MessageService,
    DataSourceHttpService,
    DataTableHttpService,
    CommonService,
    TopicHttpService,
    DictHttpService,
    CommonHttpService,
    DataColumnHttpService,
    ConfirmationService,DataClientsHttpService
  ],
})
export class DatatableDetailsComponent implements OnInit {
  @ViewChild('multiLevelApprovers') multiLevelApprovers!: ElementRef;
  @ViewChild('dataInfo') dataInfo!: ElementRef;
  @Output() goBack = new EventEmitter<string>();
  @Input() field: any;

  // 下拉数据源
  appSource: string = '';
  sourceList: Array<any> = [];
  appSourceList: Array<any> = [];
  tableList: Array<any> = [];
  tableCoumnsList: Array<any> = [];
  userList: Array<any> = [];
  tagList: any[] = [];
  dbTypeList: any[];
  statusList: any[];
  levelList: any[] = []
  reviewableList: any[] = []
  isHaveApproversList: any[] = []
  updateMethod: any[] = []
  dataCategoryList: any[] = []
  updateCategoryList: any[] = []
  // 下拉数据源
  loading: boolean = false;
  dropdownLoading: boolean = false;
  tableLoading: boolean = false;
  formData: any = {
    tableCode: '',
    alias: '',
    appSource: '',
    reviewable: 1,
    needSup: 0,
    sourceId: '',
    tableName: '',
    jsonSqlConfig: {
      limit: 5000,
      countable: true,
      updatable: false,
      insertable: false,
      deletable: false,
    },
    ownerList: [],
    // levelId: '',
    updateFrequency: '',
    dataTimeRange: '',
    updateMethod: 'manual',
    tag: '',
  };
  tagValue: any[] = [];
  source: any = { dbType: '', status: '' };
  //业务描述
  id: any;
  sourceId: any;
  titleName: string = 'add';
  multiLevelApproversList: any[] = [];
  isHaveApprovers: boolean = false;
  noEdit: boolean = false;
  addMultiLevelApproversNum: number = 1;
  userSelectList: any[] = [];
  ownerList: Array<any> = [];
  showOwnerName: string = '';
  constructor(
    private messageService: MessageService,
    private sourceService: DataSourceHttpService,
    private route: ActivatedRoute,
    public router: Router,
    private commonService: CommonService,
    private tableService: DataTableHttpService,
    private dictService: DictHttpService,
    private commonHttpService: CommonHttpService,
    private lang: TranslateService,
        private appHttpService: DataClientsHttpService
  ) { }

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.titleName = params['title'] || this.field?.title;
      this.id = params['id'] || this.field?.id;
      this.noEdit = this.field?.edit === '0'
      // this.sourceId = params['sourceId'];
      this.initSelectData()
    });
  }

  initSelectData() {
    this.getAppList();
    this.getDBTypeList();
    this.getUserList();
    this.getTagList();
    this.getStatusList();
    this.getSource();

    this.levelList = [
      {
        itemText: '5',
        itemValue: TranslateData.secretLevel5
      },
      {
        itemText: '4',
        itemValue: TranslateData.secretLevel4
      },
      {
        itemText: '3',
        itemValue: TranslateData.secretLevel3
      },
      {
        itemText: '2',
        itemValue: TranslateData.secretLevel2
      },
      {
        itemText: '1',
        itemValue: TranslateData.secretLevel1
      },
    ];
    this.reviewableList = [
      { label: TranslateData.yes, value: 1 },
      { label: TranslateData.no, value: 0 },
    ];
    this.isHaveApproversList = [
      { label: TranslateData.yes, value: true },
      { label: TranslateData.no, value: false },
    ];
    this.updateMethod = [
      { name: TranslateData.manual, value: 'manual' },
      { name: TranslateData.automatic, value: 'automatic' }
    ]
  }

  getDetailData() {
    if (this.id) {
      // 初始化数据
      this.tableService.getById(this.id).then((res: any) => {
        this.formData = res.data;
        this.formData.reviewable = Number(this.formData.reviewable);
        this.formData.needSup = Number(this.formData.needSup) == 1 ? 1 : 0;
        if (this.formData.tag) {
          if (this.formData.tag.includes(',')) {
            this.tagValue = this.formData.tag.split(',');
          } else {
            this.tagValue = [this.formData.tag];
          }
        }
        if (typeof this.formData.jsonSqlConfig == 'undefined') {
          this.formData.jsonSqlConfig = {
            limit: 5000,
            countable: true,
            updatable: false,
            insertable: false,
            deletable: false,
          };
        }
        this.formData.tableComment = this.formData.tableComment || '';

        if (!this.formData.appSource) {
          this.appSource = this.sourceList.find(item => item.id == this.formData.sourceId)?.systemName;
          this.formData.appSource = this.formData.appSource || this.appSource;
        } else {
          this.appSource = this.formData.appSource
        }
        // 有多级审批人，赋值
        this.initMultApprover()

        if (this.formData.ownerList.length > 0) {
          this.ownerList = this.formData.ownerList.map((item: any) => item.ownerId);
          this.showOwnerName = this.formData.ownerList.map((item: any) => item.ownerName).join(',')
          this.userSelectList = this.formData.ownerList
          this.getOwnerDepartment()
        }

        // 编辑时赋值数据信息
        if (this.titleName != 'add') {
          console.log(this.formData)
          // @ts-ignore
          this.dataInfo.getMessageList(this.formData.id, this.formData.sourceId);
        }

        // 获取表数据
        this.getTable(res.data.sourceId);
        // 赋值source
        this.setSourceValue(res.data.sourceId);
      });
    }

    //如果是从资源跳转过来就会带 sourceId，
    // if (this.sourceId) {
    //   this.formData.sourceId = this.sourceId;
    //   //查询源库
    //   this.getDbTables(this.sourceId);
    //   this.setSourceValue(this.sourceId);
    // }
  }

  initMultApprover() {
    if (this.formData.approverList && this.formData.approverList.length > 0) {
      this.isHaveApprovers = true;
      this.multiLevelApproversList = this.formData.approverList.map((v: any, index: number) => {
        if (v.userList && v.userList.length > 0) {
          v.idList = v.userList.map((c: any) => c.userId);
          v.nameList = v.userList.map((c: any) => c.userName).join(',');
          v.id = index + 1;
        }
        return v;
      });
    } else {
      this.multiLevelApproversList = []
      this.isHaveApprovers = false;
    }
  }

  async getDBTypeList() {
    let res = await this.dictService.codes('data_db_type,data_category,update_category');
    if (res.success) {
      this.dbTypeList = res.data.filter((f: any) => f.dictCode == 'data_db_type');
      this.dataCategoryList = res.data.filter((f: any) => f.dictCode == 'data_category');
      this.updateCategoryList = res.data.filter((f: any) => f.dictCode == 'update_category');
      this.dataCategoryList.forEach((f: any) => {
        this.lang.get("dict." + f.dictCode + "." + f.itemValue).subscribe((res: string) => {
          if (res != "dict." + f.dictCode + "." + f.itemValue) f.itemText = res;
        });
      });
      this.updateCategoryList.forEach((f: any) => {
        this.lang.get("dict." + f.dictCode + "." + f.itemValue).subscribe((res: string) => {
          if (res != "dict." + f.dictCode + "." + f.itemValue) f.itemText = res;
        });
      });
    }
  }

  getUserList() {
    if (sessionStorage.getItem('userList')) {
      let userListString: any = sessionStorage.getItem('userList');
      let userList = JSON.parse(userListString);
      this.userList = userList;
    } else {
      this.dropdownLoading = true;
      this.commonHttpService.getUserList().then((res: any) => {
        if (res.data && res.data.length > 0) {
          let arr = res.data;
          const uniqueArr = Array.from(
            new Map(arr.map((item: any) => [item['id'], item])).values()
          );
          this.userList = uniqueArr;
          try {
            sessionStorage.setItem('userList', JSON.stringify(this.userList));
          } catch (e) {
            console.error(e)
          }
        }
        this.dropdownLoading = false;
      });
    }
    this.getDetailData()
  }

  getTagList() {
    this.tableService.getTagLists().then((res: any) => {
      if (res.code == 200) {
        this.tagList = res.data.map((item: any) => {
          return {
            label: item,
            value: item,
          };
        });
      } else {
        this.messageService.add({
          severity: 'error',
          summary: TranslateData.fail,
          detail: res.msg,
        });
      }
    });
  }

  async getStatusList() {
    let { data } = await this.dictService.code('sys_common_status');
    this.statusList = data;
  }

  async getSource() {
    let { data } = await this.sourceService.list({});
    this.sourceList = data;
  }

  async getTable(id: string) {
    let { data } = await this.tableService.list({
      sourceId: id,
    });
    this.tableList = data;
  }

  onPanelHideOwner(e: any) {
    this.userList.sort((a: any, b: any) => {
      let aIndex = this.ownerList?.indexOf(a.id) > -1 ? -1 : 0;
      let bIndex = this.ownerList?.indexOf(b.id) > -1 ? -1 : 0;
      return aIndex - bIndex;
    });
  }

  onChangeUserOwner(e: any) {
    if (!e.value) return;
    let arr: any[] = e.value;
    let department: any[] = []
    this.userSelectList = [];
    this.userList.forEach((item: any) => {
      arr.forEach((citem: any) => {
        if (item.id == citem) {
          this.userSelectList.push({
            ownerId: item.id,
            ownerName: item.name,
            // ownerName: item.ntid,
          });
          if (item?.department && !department.includes(item?.department)) {
            department.push(item.department)
          }
        }
      });
    });
    this.userSelectList = Array.from(
      new Map(this.userSelectList.map(item => [item['ownerId'], item])).values()
    );
    this.showOwnerName = this.userSelectList.map(item => item.ownerName).join(',');

    this.formData.ownerDepart = department.join(',')
  }

  // onChangeLevel(e: any) {
  //   if (!e.value) return;
  //   let items = this.levelList.filter((citem: any) => citem.itemText == e.value);
  //   this.formData.levelId = items[0].itemText;
  //   this.formData.levelName = items[0].itemValue;
  // }

  getOwnerDepartment() {
    let department: any[] = []

    this.userList.forEach((item: any) => {
      this.ownerList.forEach((citem: any) => {
        if (item.id == citem) {
          if (item?.department && !department.includes(item?.department)) {
            department.push(item.department)
          }
        }
      });
    });

    this.formData.ownerDepart = department.join(',')
  }

  // 赋值source
  setSourceValue(id: string) {
    this.sourceService.getById(id).then((res: any) => {
      res.data.status = res.data.status.toString()
      res.data.dbType = res.data.dbType.toString()
      this.source = res.data;
    });
  }

  onSourceChange(e: any) {
    this.getDbTables(e.value);
    //选择源后 根据源 查询 表 新增的时候查询源，修改和查询的时候，查询本地库
    // if (this.titleName == 'add') {
    //   this.getDbTables(e.value);
    // } else {
    //   this.getTable(e.value);
    // }
    //查询这条源数据
    this.setSourceValue(e.value);
    this.appSource = this.sourceList.find(item => item.id == e.value)?.systemName;
    this.formData.appSource = this.appSource;
  }

  //刷新
  onRefresh() {
    this.sourceService.refresh();
  }

  //新增的时候，查询源库
  getDbTables(sourceId: any) {
    this.sourceService.getDbTablesMergeLocal(sourceId).then((res: any) => {
      this.tableList = res.data;
      this.tableList.map((item: any) => {
        if (item.type == 1) {
          item.inactive = true;
        } else {
          item.inactive = false;
        }
      });
    });
  }

  onChangeTable(e: any) {
    if (this.formData.alias == '') {
      let items: any = this.tableList.filter((citme: any) => citme.tableName == e.value);
      if (items.length > 0) {
        this.formData.alias = items[0].tableName;
      }
    }

    // 更新数据信息数据
    // @ts-ignore
    this.dataInfo.changeTable(this.formData.sourceId, this.formData.id)
  }

  //提交
  submit() {
    this.formData.ownerList = this.userSelectList;
    // 获取数据信息选中数据
    // @ts-ignore
    this.formData.columnList = this.dataInfo.selectedItems;
    this.formData.tag = this.tagValue.map((item: any) => item).join(',');
    const valid = ['alias', 'sourceId', 'tableName', 'ownerList', 'tableComment'];
    if (this.commonService.isInvalid(this.formData, valid)) {
      this.formData.invalid = true;
    } else {
      if (this.isHaveApprovers) {
        // @ts-ignore
        const res = this.multiLevelApprovers?.onSumitMultiLevelApprovers()
        if (!res) return
      }

      // @ts-ignore
      this.formData.approverList = this.multiLevelApprovers?.approverList || []
      console.log(this.formData)
      if (this.formData.columnList.length == 0) {
        return this.messageService.add({
          severity: 'warn',
          summary: TranslateData.tips,
          detail: TranslateData.checkDataInfo
        });
      }
      this.dropdownLoading = true;

      const httpStr = this.titleName == 'edit' ? 'update' : 'save'
      this.tableService[httpStr](this.formData, this.id).then((resUpdate: any) => {
        if (resUpdate.code == 200) {
          this.onRefresh();
          this.messageService.add({
            severity: 'success',
            summary: TranslateData.save,
            detail: TranslateData.success,
          });
          setTimeout(() => {
            this.dropdownLoading = false;
            this.backClick('update');
          }, 1000);
        } else {
          this.dropdownLoading = false;
          this.messageService.add({
            severity: 'error',
            summary: TranslateData.fail,
            detail: resUpdate.msg,
          });
        }
      });
    }
  }

  backClick(type?: string) {
    this.goBack.emit(type)
  }

  onPanelHide(info: any) {
    this.userList.sort((a: any, b: any) => {
      let aIndex = info.idList.indexOf(a.userId) > -1 ? -1 : 0;
      let bIndex = info.idList.indexOf(b.userId) > -1 ? -1 : 0;
      return aIndex - bIndex;
    });
  }

  clearDataOwner() {
    setTimeout(() => {
      this.ownerList = []
    }, 100)
  }
  async getAppList() {
    let res = await this.appHttpService.AllAppList();
    if (res.success) {
      this.appSourceList = res.data.map((f: any) => ({
        clientName: f.clientName + '(' + f.nickName.toUpperCase() + ')',
        nickName: f.nickName.toUpperCase(),
      }));
    }
  }
  onChangeAllAppSource(event: any) {
    // // var clientName = this.appSourceList.find(item => item.nickName == event.value)?.clientName
    // this.tableData.forEach(element => {
    //   element.appSource = event.value
    //   // element.appSourceName = clientName
    // });
  }
}
