import {
  Component,
  OnInit,
  Input,
  Output,
  EventEmitter,
  SimpleChanges,
  ChangeDetectorRef,
} from '@angular/core';
import { format } from 'sql-formatter';
import * as ace from 'brace';
import 'brace/mode/sql';
import 'brace/theme/sqlserver';
import { CommonService } from 'jabil-bus-lib';
import { MessageService } from 'primeng/api';
// HttpService
import DataApiHttpService from 'src/api/dataAssetManage/api';
import DataSourceHttpService from 'src/api/dataAssetManage/dataSource';
import DataTableHttpService from 'src/api/dataAssetManage/dataTable';
import DataColumnHttpService from 'src/api/dataAssetManage/dataColumn';
import DictHttpService from 'src/api/common/dict';

@Component({
  selector: 'implementComp',
  templateUrl: './index.html',
  styleUrls: ['./index.scss'],
  providers: [
    MessageService,
    DictHttpService,
    DataApiHttpService,
    DataSourceHttpService,
    DataTableHttpService,
    DataColumnHttpService
  ],
})
export class ImplementCompComponent implements OnInit {
  @Output() implementCompTable = new EventEmitter();
  @Input() formData: any;
  @Input() formType?: string = 'add';
  @Input() stepActiveIndex?: number;
  @Output() changeSelectValue = new EventEmitter();
  @Output() isShowSqlBtn = new EventEmitter();

  dropdownList: Array<any> = [
    { name: 'select1', id: '10002' },
    { name: 'select2', id: '10003' },
  ];
  dict: any = {
    dataConfigType: [
      {
        text: localStorage.getItem('lang') == 'en' ? 'Table bootstrap mode' : '表引导模式',
        value: '1',
      },
      { text: localStorage.getItem('lang') == 'en' ? 'SQL mode' : 'SQL模式', value: '3' },
      // { text: localStorage.getItem('lang') == 'en' ? 'Script mode' : '脚本模式', value: '2' },
      { text: localStorage.getItem('lang') == 'en' ? 'Json mode' : 'Json模式', value: '4' },
    ],
  };
  // 数据源
  sourceList: Array<any> = [];
  // 数据源表
  sourceTableList: Array<any> = [];
  // 数据源表字段
  sourceTableColumList: any[];
  implementForm: any = {
    executeConfig: {
      configType: '',
      sourceId: '',
      tableId: '',
    },
  };
  isShowSelect: string = '1';
  codeValue: string = '';
  editor: any;
  implementFormInvalid: boolean = false;
  implementSelectColumns: any[] = []; // 页面选中字段
  isOrNoList: any[] = [
    { label: localStorage.getItem('lang') == 'en'?'Yes': '是', value: '1' },
    { label: localStorage.getItem('lang') == 'en'?'No': '否', value: '0' },
  ];
  dataTypeList: any[] = [];
  checkTableData: any[] = [];
  consultValue: string = '';
  allReqParams: boolean = false;
  allResParams: boolean = false;
  dataViewData: any[] = [];
  sourceTableLoading: boolean = false;
  loading: boolean = false;
  lang: string = 'en'
  constructor(
    private commonService: CommonService,
    private messageService: MessageService,
    private dataApiHttpService: DataApiHttpService,
    private dataSourceHttpService: DataSourceHttpService,
    private dataTableHttpService: DataTableHttpService,
    private dataColumnHttpService: DataColumnHttpService,
    private dictHttpService: DictHttpService
  ) {
    this.lang = localStorage.getItem('lang') || 'en'
  }
  ngOnChanges(changes: SimpleChanges): void {
    if (sessionStorage.getItem('implementCompForm')) {
      let getItem: any = sessionStorage.getItem('implementCompForm');
      let formInfo: any = JSON.parse(getItem);
      this.implementForm = { ...formInfo };
      this.isShowSqlBtn.emit({
        configType: this.implementForm.executeConfig.configType,
        sourceId: this.implementForm.executeConfig.sourceId,
        tableId:
          this.implementForm.executeConfig.configType == 1
            ? this.implementForm.executeConfig.tableId
            : '',
      });
      this.initSourceList();
      if (this.implementForm.executeConfig.sourceId) {
        this.initSourceTableList({ sourceId: this.implementForm.executeConfig.sourceId });
        if (this.implementForm.executeConfig.tableId) {
          this.initSourceTableColumList({
            sourceId: this.implementForm.executeConfig.sourceId,
            tableId: this.implementForm.executeConfig.tableId,
          });
        }
      }
    } else {
      if (this.stepActiveIndex == 1 && changes['formData']) {
        let info: any = changes['formData'].currentValue;
        if (info === null) {
          return;
        }
        this.implementForm = { ...info };
        this.initSourceList();
        if (this.implementForm.executeConfig.sourceId) {
          this.initSourceTableList({ sourceId: this.implementForm.executeConfig.sourceId });

          if (this.implementForm.executeConfig.tableId) {
            this.initSourceTableColumList(
              {
                sourceId: this.implementForm.executeConfig.sourceId,
                tableId: this.implementForm.executeConfig.tableId,
              },
              this.formType != 'add' ? 'check' : 'new'
            );
          }
        }
      }
    }
  }
  ngAfterViewInit() {
    if (sessionStorage.getItem('editorValue')) {
      let editorValue = sessionStorage.getItem('editorValue');
      this.editor.setValue(editorValue);
    }
    if (sessionStorage.getItem('consultValue')) {
      let consultValue = sessionStorage.getItem('consultValue');
      this.consultValue = consultValue + '';
    }
    this.isShowSelect = this.implementForm.executeConfig.configType;
    // this.editor.setValue(defaultSQL);
    this.setMysqlValue();
  }

  doSomethingAfterViewInitialized() {
    if (this.isShowSelect == '2') {
      this.setMysqlValue();
    }
  }
  ngOnInit(): void {
    this.editor = ace.edit('implement-mysql-edit');
    this.editor.getSession().setMode('ace/mode/sql');
    this.editor.setTheme('ace/theme/sqlserver');
    this.editor.session.getTextRange(this.editor.getSelectionRange());
    this.editor.setHighlightActiveLine(true);
    this.isShowSelect = '1';
    // this.getDictFunc('data_type_mysql');
  }
  onChangeAllParams(e: any, type: string) {
    let value = e.checked;
    this.sourceTableColumList.forEach((item: any) => {
      if (type == 'req') {
        item.checked = value;
      } else {
        item.resParams = value;
      }
    });
  }
  getDictFunc(name: string) {
    this.dictHttpService.code(name).then((res: any) => {
      if (res.code == 200) {
        this.dataTypeList = res.data
        if(this.dataTypeList.length>0){
          this.dataTypeList.forEach((item:any)=>{
            item.itemValue = localStorage.getItem('lang') == 'en' ?item.itemText:item.itemValue
          })
        }
      } 
    });
  }
  // 通过执行 SQL 获取 列字段信息
  SQLQuery() {
    this.sqlParse();
  }

  formatCode() {
    let value = this.editor.getValue();
    value = format(value, {
      language: 'mysql',
      tabWidth: 2,
      keywordCase: 'upper',
      linesBetweenQueries: 2,
    });
    this.editor.setValue(value);
  }
  setMysqlValue() {
    if (
      this.isShowSelect == '2' &&
      this.sourceList.length > 0 &&
      this.implementForm.executeConfig.sourceId
    ) {
      const source = this.sourceList.find(
        (source: any) => source.id === this.implementForm.executeConfig.sourceId
      );
      const defaultSQL = `SELECT * FROM ${source?.sourceName}. LIMIT 10;`;
      this.consultValue =
        defaultSQL + ' //' + (localStorage.getItem('lang') == 'en' ? 'reference' : '参考');
    }
  }
  onSetsessionStorage() {
    if (this.isShowSelect != '2') {
      this.implementForm.executeConfig.fieldParams = this.implementSelectColumns;
      this.implementForm.executeConfig.columnList = this.sourceTableColumList;
    }
    let implementForm = JSON.stringify(this.implementForm);
    sessionStorage.setItem('implementCompForm', implementForm);
    // sessionStorage.removeItem('parameterCompForm');
    sessionStorage.removeItem('editorValue');
    sessionStorage.removeItem('consultValue');
  }
  onChange(e: any) {
    this.isShowSelect = e.value;
    this.editor.setValue('');
    this.isShowSqlBtn.emit({
      configType: e.value,
      sourceId: this.implementForm.executeConfig.sourceId,
      tableId:
        this.implementForm.executeConfig.configType == 1
          ? this.implementForm.executeConfig.tableId
          : '',
    });
    this.onSetsessionStorage();
  }
  onSaveChange(e: any) {
    this.onSetsessionStorage();
  }
  onCheckChange(e: any, info: any) {
    if (info.checked) {
      this.implementSelectColumns.push(info);
    } else {
      this.implementSelectColumns = this.implementSelectColumns.filter(item => item !== info);
    }
    let isAllReq = this.sourceTableColumList.every((item: any) => item.checked);
    let isAllRes = this.sourceTableColumList.every((item: any) => item.resParams);
    this.allReqParams = isAllReq;
    this.allResParams = isAllRes;
    this.implementCompTable.emit(this.sourceTableColumList);
    this.onSetsessionStorage();
  }
  setEditorText() {
    sessionStorage.setItem('editorValue', this.editor.getValue());
    return this.editor.getValue();
  }
  nextCases() {
    let valid = ['configType', 'sourceId', 'tableId'];
    if (this.isShowSelect == '2') {
      valid = ['configType', 'sourceId'];
    }
    if (this.commonService.isInvalid(this.implementForm.executeConfig, valid)) {
      this.implementFormInvalid = true;
      return false;
    } else {
      if (this.isShowSelect == '2') {
        sessionStorage.setItem('editorValue', this.editor.getValue());
        sessionStorage.setItem('consultValue', this.consultValue);
      }
      let fieldParams = this.implementSelectColumns.filter(
        (item: any, index: number, arr: any[]) => {
          return index == arr.findIndex((i: any) => i.id == item.id);
        }
      );
      this.implementForm.executeConfig.fieldParams = fieldParams;
      this.implementForm.executeConfig.columnList = this.sourceTableColumList;
      return JSON.stringify(this.implementForm);
    }
  }

  selectSource(even: any) {
    let dbtypeList = this.sourceList.filter((item: any) => item.id == even);
    let dbName = dbtypeList[0].dbType;
    // 1：mysql、3：sql_server、5：postgre_sql、8：other(unkonwn db)
    let dbtypeInfo: any = {
      1: 'data_type_mysql',
      3: 'data_type_mssql',
      5: 'data_type_postgresql',
      8: '',
    };
    if (dbName != 8) {
      this.getDictFunc(dbtypeInfo[dbName]);
    }
    this.initSourceTableList({ sourceId: even });
    if (this.isShowSelect == '2') {
      this.implementForm.executeConfig.tableId = '';
      this.sourceTableList = [];
      this.consultValue = '';
      this.setMysqlValue();
    }
    this.onSetsessionStorage();
  }
  // 数据预览list
  getViewList(tableId: string) {
    if (!tableId) return;
    this.loading = true
    this.dataApiHttpService.dataPreview(tableId, 20).then((res: any) => {
      if (res.code == 200) {
        let data = res.data.dataList;
        if (data.length > 0) {
          this.dataViewData = data.filter((item: any, index: number) => index == 0);
          this.initSourceTableColumList(
            {
              sourceId: this.implementForm.executeConfig.sourceId,
              tableId: tableId,
            },
            'new'
          );
        }
      } else {
        this.initSourceTableColumList(
          {
            sourceId: this.implementForm.executeConfig.sourceId,
            tableId: tableId,
          },
          'new'
        );
        this.messageService.add({
          severity: 'error',
          summary: localStorage.getItem('lang') == 'en' ? 'Error' : '错误',
          detail: res.msg,
        });
      }

      this.loading = false
    });
  }
  selectTable(even: any) {
    this.getViewList(even);

    this.implementForm.reqParams = [];
    this.implementForm.resParams = [];
    this.onSetsessionStorage();
  }
  onInput(e: any) {
    this.implementCompTable.emit(this.sourceTableColumList);
    this.onSetsessionStorage();
  }
  onBlur(e: any) {
    this.implementCompTable.emit(this.sourceTableColumList);
    this.onSetsessionStorage();
  }
  // ================================ HTTP Requerts =============================

  async initSourceList() {
    const res = await this.dataSourceHttpService.list();
    if (res.data) {
      this.sourceList = [];
      res.data.forEach((element: any) =>
        this.sourceList.push({ ...element, sourceName: element.sourceName, id: element.id })
      );
      if (this.implementForm.executeConfig.sourceId) {
        let dbtypeList = this.sourceList.filter(
          (item: any) => item.id == this.implementForm.executeConfig.sourceId
        );
        let dbName = dbtypeList[0].dbType;
        // 1：mysql、3：sql_server、5：postgre_sql、8：other(unkonwn db)
        let dbtypeInfo: any = {
          1: 'data_type_mysql',
          3: 'data_type_mssql',
          5: 'data_type_postgresql',
          8: '',
        };
        if (dbName != 8) {
          this.getDictFunc(dbtypeInfo[dbName]);
        }
      }
    }
  }

  async initSourceTableList(data?: any) {
    const res = await this.dataTableHttpService.list(data);
    if (res.data) {
      this.sourceTableList = [];
      res.data.forEach((element: any) =>
        this.sourceTableList.push({ tableName: element.tableName, id: element.id })
      );
    }
  }
  async initSourceTableColumList(data?: any, type: string = '') {
    if (!data.sourceId || !data.tableId) return;
    this.sourceTableLoading = true;
    const res = await this.dataColumnHttpService.list(data);
    if (res.data) {
      this.sourceTableColumList = [];
      const checkedFileds = this.implementForm.executeConfig.fieldParams;
      res.data.forEach((element: any, index: number) => {
        element.index = index + 1;
        if (checkedFileds) {
          // 返回数据中存在字段则在页面选中该字段
          element.checked =
            checkedFileds.findIndex(
              (isChecked: any) => isChecked.colName === element.colName
            ) !== -1;
          // 并添加到页面选中字段集合中
          if (element.checked) this.implementSelectColumns.push(element);
        }
        this.sourceTableColumList.push(element);
      });
      const selectedTable = this.sourceTableList.find(
        (table: any) => table.id === this.implementForm.executeConfig.tableId
      );
      this.implementForm.executeConfig.tableName = selectedTable?.tableName;
    }
    if (type == '') {
      this.sourceTableColumList = this.implementForm.executeConfig.columnList;
      if (
        this.sourceTableColumList.length > 0 &&
        this.implementForm.reqParams &&
        this.implementForm.reqParams.length > 0
      ) {
        this.sourceTableColumList.forEach((item: any) => {
          item.checked = false;
          this.implementForm.reqParams &&
            this.implementForm.reqParams.forEach((citem: any) => {
              if (citem.paramName == item.colName) {
                item.checked = true;
                item.columnComment = citem.paramComment;
                item.dataDefault = citem.defaultValue;
                item.dataType = citem.paramType;
              }
            });
        });
      }
      if (
        this.sourceTableColumList.length > 0 &&
        this.implementForm.resParams &&
        this.implementForm.resParams.length > 0
      ) {
        this.sourceTableColumList.forEach((item: any) => {
          item.resParams = false;
          this.implementForm.resParams &&
            this.implementForm.resParams.forEach((zitem: any) => {
              if (zitem.fieldName == item.colName) {
                item.resParams = true;
                item.colComment = zitem.fieldComment;
                item.dataType = zitem.dataType;
              }
            });
        });
      }
    } else if (type == 'check') {
      this.sourceTableColumList.forEach((item: any) => {
        item.checked = false;
        item.resParams = false;
        this.implementForm.reqParams.forEach((citem: any) => {
          if (citem.paramName == item.colName) {
            item.checked = true;
            item.colComment = citem.paramComment;
            item.dataDefault = citem.defaultValue;
            item.dataType = citem.paramType;
          }
        });
        this.implementForm.resParams.forEach((zitem: any) => {
          if (zitem.fieldName == item.colName) {
            item.resParams = true;
            item.colComment = zitem.fieldComment;
            item.dataType = zitem.dataType;
          }
        });
      });
      this.sourceTableColumList = this.sourceTableColumList.filter(
        (item: any, index: number, arr: any[]) => {
          return index == arr.findIndex((i: any) => i.id == item.id);
        }
      );
    } else {
      this.sourceTableColumList.forEach((item: any) => {
        item.resParams = true;
        item.checked = true;
        item.dataType = 'varchar';
        item.colComment = item.colComment ? item.colComment : item.columnName;
        this.dataViewData.forEach((citem: any) => {
          item.dataDefault = citem[item.colName];
        });
      });
    }
    let isAllReq = this.sourceTableColumList.every((item: any) => item.checked);
    let isAllRes = this.sourceTableColumList.every((item: any) => item.resParams);
    this.allReqParams = isAllReq;
    this.allResParams = isAllRes;
    this.sourceTableLoading = false;
  }

  async sqlParse() {
    const res = await this.dataApiHttpService.sqlParse({
      sourceId: this.implementForm.executeConfig.sourceId,
      sqlText: this.editor.getValue(),
    });
    if (res.code == 200) {
    } else {
      this.messageService.add({
        severity: 'error',
        summary: localStorage.getItem('lang') == 'en' ? 'Error' : '错误',
        detail: res.msg,
      });
    }
  }
}
