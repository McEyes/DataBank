import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import * as ace from 'brace';
import 'brace/mode/sql';
import 'brace/theme/sqlserver';
import { format } from 'sql-formatter';
import { MessageService } from 'primeng/api';

// HttpService
import SQLQueryHttpService from 'src/api/dataAssetManage/sqlQuery';
import DataSourceHttpService from 'src/api/dataAssetManage/dataSource';

// SQL Query Models
interface QueryModel {
  sqlKey: string;
  sourceId: string;
  sqlText: string;
}
// 查询结果
interface QueryResultModel {
  sql: string;
  time: string;
  count: number;
  success: boolean;
  columnList: string[];
  dataList: any[];
}

@Component({
  selector: 'app-sqlquery',
  templateUrl: './sqlquery.component.html',
  styleUrls: ['./sqlquery.component.scss'],
  providers: [MessageService, SQLQueryHttpService, DataSourceHttpService],
})
export class SQLQueryComponent implements OnInit {
  queryParams: any = {
    test: '',
  };

  sourceList: Array<any> = [];
  selectedSource: any;
  editorValue: any;
  sqlText: QueryModel;

  tabMenuList: any[] = [];
  tabActive: number = 0;
  products: any[];
  tablesData: QueryResultModel[];
  total: number = 0;
  tabType: string;
  resualtTableInfo: any = {
    columnList: [],
    dataList: [],
    keyValue: '',
    sql: '',
    success: true,
    time: '',
  };
  exampleValue: string = '';
  messageInfo: any = {};
  loading: boolean = false;
  lange: any = 'en';

  constructor(
    public router: Router,
    private sqlQueryHttpService: SQLQueryHttpService,
    private messageService: MessageService,
    private dataSourceHttpService: DataSourceHttpService
  ) { }

  ngOnInit(): void {
    this.lange = localStorage.getItem('lang');
    this.initEditor()
    this.initSources();
  }

  initEditor() {
    let editStyle: any = document.getElementById('mysql-edit');
    editStyle.style.fontSize = '12px';
    this.editorValue = ace.edit('mysql-edit');
    let editor: any = ace.edit('mysql-edit');
    editor.resize();
    editor.getSession().setMode('ace/mode/sql');
    editor.setTheme('ace/theme/sqlserver');
    // 添加自动滚动到视图的行为
    editor.getSession().selection.on('changeSelection', function () {
      editor.renderer.scrollCursorIntoView(null, 0.5);
    });
    editor.setOptions({
      enableBasicAutocompletion: true,
      enableLiveAutocompletion: true,
    });
    editor.$blockScrolling = Infinity;
    editor.setFontSize(16);
    editor.setOption('enableEmmet', true);

  }


  selectSourceModelChange(even: any) {
    const source = this.sourceList.find(item => item.id === even);
    this.exampleValue = `SELECT * FROM ${source.name}.tableName LIMIT 1000; //${localStorage.getItem('lang') == 'en' ? 'reference' : '参考'
      }  SQL`;
  }

  /** 运行 */
  executeQuery() {
    this.sqlText = this.getQueryModel();
    this.query(this.sqlText);
    this.tabActive = 0;
    this.tabType = 'message';
  }

  executeQueryStop() {
    this.loading = true;
    this.sqlQueryHttpService.stopQuery(this.sqlText).then((res: any) => {
      if (res.code == 200) {
        this.messageService.add({
          severity: 'success',
          summary: localStorage.getItem('lang') == 'en' ? 'Stop' : '停止',
          detail: localStorage.getItem('lang') == 'en' ? 'Success' : '成功',
        });
        this.loading = false;
      } else {
        this.loading = false;
        this.messageService.add({
          severity: 'error',
          summary: localStorage.getItem('lang') == 'en' ? 'Error' : '错误',
          detail: res.msg,
        });
      }
    });
  }

  SQLFormat() {
    const formatValue = format(this.editorValue.getValue(), {
      language: 'mysql',
      tabWidth: 2,
      keywordCase: 'upper',
      linesBetweenQueries: 2,
    });
    this.editorValue.setValue(formatValue);
  }

  resetEditor() {
    this.editorValue.setValue('');
    this.exampleValue = '';
    this.tabMenuList = [];
    this.selectedSource = '';
    this.resualtTableInfo.columnList = [];
    this.resualtTableInfo.dataList = [];
  }

  /**
   * 获取查询模型
   * @returns QueryModel
   */
  getQueryModel(): QueryModel {
    let sqlDto: QueryModel = {
      sqlKey: new Date().getTime().toString(),
      sourceId: this.selectedSource,
      sqlText: this.editorValue.getValue(),
      //sourceId: '10002',
      //sqlText: 'SELECT * FROM HR_Attendance.AttendanceRecord LIMIT 10;SELECT * FROM HR_Base.Sys_User LIMIT 10;SELECT COUNT(*) FROM HR_Base.Sys_User;',
    };
    return sqlDto;
  }

  /**
   * 执行查询
   * @param data QueryModel，直接调用 getQueryModel() 即可获取该参数
   */
  async query(data: any) {
    if (data.sourceId == '') {
      return this.messageService.add({
        severity: 'warn',
        summary: localStorage.getItem('lang') == 'en' ? 'Tips' : '提示',
        detail:
          localStorage.getItem('lang') == 'en' ? 'Please select a data source' : '请选择数据源',
      });
    }
    if (data.sqlText == '') {
      return this.messageService.add({
        severity: 'warn',
        summary: localStorage.getItem('lang') == 'en' ? 'Tips' : '提示',
        detail:
          localStorage.getItem('lang') == 'en' ? 'Please input SQL statement' : '请输入SQL语句',
      });
    }
    this.tablesData = [];
    this.tabMenuList = [];
    this.loading = true;
    const res = await this.sqlQueryHttpService.query(data);
    if (res.code == 200) {
      this.tablesData = res.data;
      this.messageInfo = this.tablesData.length > 0 && this.tablesData[0];
      this.tablesData.forEach((item: any, index: number) => {
        item.keyValue = index + '1';
        let arr = [
          { label: `信息${index + 1}`, value: index + '1', type: 'message' },
          { label: ` 结果${index + 1}`, value: index + '1', type: 'result' },
        ];
        this.tabMenuList.push(...arr);
      });
      this.messageService.add({
        severity: 'success',
        summary: localStorage.getItem('lang') == 'en' ? 'Run' : '运行',
        detail: localStorage.getItem('lang') == 'en' ? 'Success' : '成功',
      });
      this.loading = false;
    } else {
      this.loading = false;
      this.messageService.add({
        severity: 'error',
        summary: localStorage.getItem('lang') == 'en' ? 'Error' : '错误',
        detail: res.msg,
      });
    }
  }

  /** 获取数据源列表 */
  async initSources() {
    const sourceRes = await this.dataSourceHttpService.list();
    if (sourceRes.data) {
      this.sourceList = [];
      sourceRes.data.forEach((element: any) => {
        const el = { name: element.sourceName, id: element.id };
        this.sourceList.push(el);
      });
    }
  }

  tabClick(index: number, item: any) {
    this.tabType = item.type;
    this.tabActive = index;
    const citems = this.tablesData.filter((citem: any) => item.value == citem.keyValue);
    let info: any = citems[0];
    this.messageInfo = info;
    this.resualtTableInfo = {
      columnList: [],
      dataList: info.dataList,
      keyValue: info.keyValue,
      sql: info.sql,
      success: info.success,
      time: info.time,
    };
    info.columnList.forEach((zitem: any) => {
      let obj = { field: zitem, header: zitem };
      this.resualtTableInfo.columnList.push(obj);
    });
  }
}
