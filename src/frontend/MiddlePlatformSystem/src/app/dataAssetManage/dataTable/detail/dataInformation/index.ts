import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { CommonService } from 'jb.package/jabil-bus-lib';
import { ConfirmationService, MessageService } from 'primeng/api';
import DataColumnHttpService from 'src/api/dataAssetManage/dataColumn';
import DataSourceHttpService from 'src/api/dataAssetManage/dataSource';
import DictHttpService from 'src/api/common/dict';
import { TranslateData } from 'src/app/core/translate/translate-data';
import CommonHttpService from 'src/api/common';
import DataClientsHttpService from 'src/api/dataAssetManage/dataClients';

@Component({
  selector: 'app-datatable-data-info',
  templateUrl: './index.html',
  styleUrls: ['./index.scss'],
  providers: [
    DataSourceHttpService,
    DataColumnHttpService,
    DictHttpService,
    ConfirmationService,
    MessageService,
    CommonService,
    DataClientsHttpService
  ],
})
export class DataInfoComponent implements OnInit {
  @Output() goBack = new EventEmitter<string>();
  @Input() formData: any;
  @Input() titleName: any;
  @Input() appSource: string;

  tableData: any[] = [];
  loading: boolean = false;
  commonMessageTableData: any[] = [];
  commonMessageHeaderData: any[] = [
    { field: 'colName', header: 'field.field.name' },
    { field: 'colComment', header: 'field.field.comment' },
    { field: 'levelName', header: 'field.security.level' },
    { field: 'masterdataType', header: 'field.master.data.type' },
    { field: 'standardized', header: 'field.standardized' },
    { field: 'appSource', header: 'field.app.source' },
    { field: 'indicatorCode', header: 'field.indicator.code' },
    { field: 'qualityScore', header: 'field.quality.score' },
    { field: 'colKey', header: 'field.primary.key' },
    { field: 'nullable', header: 'field.null' },
    { field: 'dataType', header: 'field.data.type' },
    { field: 'dataLength', header: 'field.data.length' },
    { field: 'dataPrecision', header: 'field.data.accuracy' },
    { field: 'dataScale', header: 'field.decimal.places' },
    { field: 'dataDefault', header: 'field.default.value' },
    { field: 'requiredAsCondition', header: 'field.required.as.condition' },
    { field: 'sortable', header: 'field.sortable' },
  ];
  selectedItems: any[] = [];
  dataMessageParams: any = {
    pageSize: 10,
    pageNum: 1,
  };
  dataMessageTotal: number = 0
  tableLoading: boolean = false
  messageForm: any = {
    colName: '',
    colComment: '',
    colKey: '1',
    columnNullable: '0',
    dataType: '',
  };
  columnKeyValue: boolean = false;
  columnNullableValue: boolean = false;
  messageDialog: boolean = false;
  messageInvalid: boolean = false;
  typeList: any[] = [];
  dbTypeMap: any = {
    "1": "data_type_mysql",
    "5": "data_type_postgresql",
    "6": "data_type_oracle",
    "7": "data_type_mssql"
  };
  levelList: any[] = [];
  appSourceList: any[] = [];
  constructor(
    private sourceService: DataSourceHttpService,
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
    private columnService: DataColumnHttpService,
    private dictHttpService: DictHttpService,
    private commonService: CommonService,
    private commonHttp: CommonHttpService,
    private appHttpService: DataClientsHttpService
  ) {

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
  }

  ngOnInit(): void {
    this.dataMessageParams.pageNum = 1;
    this.dataMessageParams.pageSize = 10;
    this.getTypeList('data_type_mysql');
    this.getAppList();
  }

  async getTypeList(typeName: string) {
    console.log(this.messageForm)
    let { data } = await this.dictHttpService.code(typeName);//'data_type_mysql'
    this.typeList = data;
  }

  goback() {
    this.goBack.emit()
  }

  nzPageIndexChange(index: number) {
    this.dataMessageParams.pageNum = index;
    // this.titleName == 'add' ? this.getMessageFarList() : this.getMessageList();
    this.commonMessageTableData = this.commonHttp.pageArray(this.tableData, this.dataMessageParams.pageNum, this.dataMessageParams.pageSize)
  }
  nzPageSizeChange(page: number) {
    this.dataMessageParams.pageSize = page;
    // this.titleName == 'add' ? this.getMessageFarList() : this.getMessageList();
    this.commonMessageTableData = this.commonHttp.pageArray(this.tableData, this.dataMessageParams.pageNum, this.dataMessageParams.pageSize)
  }

  onChangeLevel(event: any) {
    this.messageForm.levelName = this.levelList.find(item => item.itemText == event.value)?.itemValue
  }
  onChangeAllLevel(event: any) {
    var levelName = this.levelList.find(item => item.itemText == event.value)?.itemValue
    this.tableData.forEach(element => {
      element.levelId = event.value
      element.levelName = levelName
    });
  }

  onChangeAllAppSource(event: any) {
    // var clientName = this.appSourceList.find(item => item.nickName == event.value)?.clientName
    this.tableData.forEach(element => {
      element.appSource = event.value
      // element.appSourceName = clientName
    });
  }
  // 数据信息列表--本地
  getMessageList(id?: string, sourceId?: string) {
    this.dataMessageParams.tableId = id || this.formData?.id || '';
    this.dataMessageParams.sourceId = sourceId || this.formData?.sourceId || '';
    const params = {
      ...this.dataMessageParams,
      pageNum: 1,
      pageSize: 1000,
    }

    this.columnService.page(params).then((result: any) => {
      if (result.code == 200) {
        this.tableData = result.data.data.map((item: any) => ({
          ...item,
          appSource: item.appSource || this.appSource
          // appSourceName: this.appSourceList.find(app => app.clientName == this.appSource)?.nickName
        }))
        this.commonMessageTableData = this.tableData
        this.dataMessageTotal = result.data.total;
        this.selectedItems = this.commonMessageTableData.slice();
        this.getSourceColumnsList()
      } else {
        this.messageService.add({
          severity: 'error',
          summary: TranslateData.fail,
          detail: result.msg,
        });
      }
    });
  }

  // 数据信息列表--远程,保存过后值获取保存后的数据
  getMessageFarList() {
    this.tableLoading = true;
    this.sourceService
      .getSourceColumns(this.formData.sourceId, this.formData.tableName)
      .then((res: any) => {
        this.tableData = res.data.map((item: any) => ({
          ...item,
          appSource: item.appSource || this.appSource,
          // appSourceName: this.appSourceList.find(app => app.clientName == this.appSource)?.nickName
        }))
        this.dataMessageTotal = this.tableData.length;
        this.commonMessageTableData = this.commonHttp.pageArray(this.tableData, this.dataMessageParams.pageNum, this.dataMessageParams.pageSize)
        this.selectedItems = this.commonMessageTableData;
        this.tableLoading = false;
      });
  }

  // 数据信息列表--远程,原始表中字段
  getSourceColumnsList() {
    this.sourceService
      .getDbTableColumns(this.formData.sourceId, this.formData.tableName)
      .then((res: any) => {// 假设res.data和this.commonMessageTableData都是对象数组，每个对象都有code属性
        res.data.forEach((item: any) => {
          // 检查this.commonMessageTableData中是否已存在具有相同code的元素
          const exists = this.commonMessageTableData.some(
            tableItem => tableItem.colName === item.colName
          );
          item.appSource = item.appSource || this.appSource;
          // 如果不存在，则添加到数组中
          if (!exists) {
            this.tableData.push(item);
          }
        });
        this.dataMessageTotal = this.tableData.length;
        this.commonMessageTableData = this.commonHttp.pageArray(this.tableData, this.dataMessageParams.pageNum, this.dataMessageParams.pageSize)
      });
  }

  // 数据信息
  allTableBtn(i: number, info: any = null) {
    info.index = i
    this.messageForm = { ...info };
    this.columnKeyValue = this.messageForm.colKey == '1' ? true : false;
    this.columnNullableValue = (this.messageForm.nullable == '1' || this.messageForm.nullable == true) ? true : false;
    this.messageForm.requiredAsCondition = !!this.messageForm.requiredAsCondition
    this.messageForm.sortable = !!this.messageForm.sortable
    this.messageDialog = true;
  }

  //数据信息-删除按钮
  ondelete(event: any, info?: any) {
    this.confirmationService.confirm({
      target: event.target,
      message: TranslateData.commonDelete,
      icon: 'pi pi-exclamation-triangle',
      rejectLabel: TranslateData.no,
      acceptLabel: TranslateData.yes,

      accept: () => {
        this.tableLoading = true;
        this.columnService.delete(info.id).then((resDel: any) => {
          if (resDel.code == 200) {
            this.messageService.add({
              severity: 'success',
              summary: TranslateData.delete,
              detail: TranslateData.success,
            });
            //查询页面
            this.getMessageList();
            this.tableLoading = false;
          } else {
            this.tableLoading = false;
            this.messageService.add({
              severity: 'error',
              summary: TranslateData.fail,
              detail: resDel.msg,
            });
          }
        });
      },
      reject: () => {
        this.messageService.add({
          severity: 'info',
          summary: TranslateData.delete,
          detail: TranslateData.cancel,
        });
      },
    });
  }

  switchChange(e: any, info: any, key: any) {
    info[key] = e ? '1' : '0';
  }

  onCloseDialog() {
    this.messageDialog = false;
    this.messageForm = {};
  }

  // 点击弹框保存
  onMessageSumbit() {
    const valid = ['colName', 'colComment', 'dataType'];
    if (this.commonService.isInvalid(this.messageForm, valid)) {
      this.messageInvalid = true;
    } else {
      this.messageForm.colKey = this.columnKeyValue ? '1' : '0';
      this.messageForm.nullable = this.columnNullableValue ? '1' : '0';
      this.updateData();
      this.messageService.add({
        severity: 'success',
        summary: TranslateData.update,
        detail: TranslateData.success,
      });
      this.messageDialog = false;
    }
  }

  updateData() {
    let index = this.selectedItems.findIndex(item => item.colName == this.messageForm.colName);
    let tbindex = this.tableData.findIndex(item => item.colName == this.messageForm.colName);
    this.selectedItems[index] = this.commonMessageTableData[this.messageForm.index] = this.tableData[tbindex] = this.messageForm
  }

  // 父层调用
  changeTable(sourceId: string, id: string) {
    this.dataMessageParams.sourceId = sourceId;
    this.dataMessageParams.pageNum = 1;
    this.dataMessageParams.pageSize = 500;
    if (this.titleName == 'add') {
      this.getMessageFarList();
    } else if (this.titleName == 'edit') {
      this.dataMessageParams.tableId = id;
      this.getMessageList();
    }
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
}
