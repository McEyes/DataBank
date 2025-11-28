import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Location } from '@angular/common';
import * as ace from 'brace';
import 'brace/mode/sql';
import 'brace/theme/sqlserver';
import { format } from 'sql-formatter';
import { MessageService } from 'primeng/api';

// HttpService
import DataSourceHttpService from 'src/api/dataAssetManage/dataSource';
import DataTableHttpService from 'src/api/dataAssetManage/dataTable';
import DataApiHttpService from 'src/api/dataAssetManage/api';
import ApiLogsHttpService from 'src/api/dataAssetManage/api/log';
import CommonHttpService from 'src/api/common';
import { environment } from 'src/environments/environment';

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
  selector: 'app-dataapi-sql',
  templateUrl: './index.html',
  styleUrls: ['./index.scss'],
  providers: [MessageService, DataSourceHttpService, DataTableHttpService, DataApiHttpService, ApiLogsHttpService],
})
export class DataapiSqlComponent implements OnInit {
  @Output() goBack = new EventEmitter<string>();
  @Input() field: any;

  queryParams: any = {};
  sourceList: Array<any> = [];
  sourceTableList: any[] = [];
  editorValue: any;
  sqlText: QueryModel;
  tabActive: number = 0;
  products: any[];
  tablesData: QueryResultModel[];
  total: number = 0;
  tabType: string;
  resualtTableInfo: any = {
    columnList: [],
    dataList: [],
  };
  exampleValue: string = '';
  messageInfo: any = {};
  loading: boolean = false;
  formData: any = {
    executeConfig: {
      sourceId: '',
      tableId: '',
    },
  };
  headers: any
  lange: any = 'en';
  isRun: boolean = false;
  sqlResultForm: any = {
    pageNum: 1,
    pageSize: 30,
  };
  sqlResultTotal: number = 0;
  constructor(
    public router: Router,
    private messageService: MessageService,
    private dataSourceHttpService: DataSourceHttpService,
    private dataApiHttpService: DataApiHttpService,
    private dataTableHttpService: DataTableHttpService,
    private apiLogsHttpService: ApiLogsHttpService,
    private commonhttpService: CommonHttpService,
  ) { }

  ngOnInit(): void {
    this.field?.id && this.getDetailData()
    this.lange = localStorage.getItem('lang');
    this.editorValue = ace.edit('mysql-edit');
    const editor = ace.edit('mysql-edit');
    editor.getSession().setMode('ace/mode/sql');
    editor.setTheme('ace/theme/sqlserver');
  }

  getDetailData() {
    this.dataApiHttpService.detailsApi(this.field.id).then((res: any) => {
      this.formData = res.data.data;
      this.headers = res.data.header;
      this.initSourceTableList(this.formData.executeConfig.sourceId);
      this.initSourceList();
    });
  }

  ngOnDestroy() {
    this.resetEditor();
  }

  nzPageIndexChange(index: number) {
    this.sqlResultForm.pageNum = index;
    this.executeQuery();
  }

  nzPageSizeChange(page: number) {
    this.sqlResultForm.pageSize = page;
    this.executeQuery();
  }

  /** 运行 */
  executeQuery() {
    this.loading = true;
    this.resualtTableInfo.dataList = [];
    this.apiLogsHttpService
      .sqlFunc(
        this.formData.apiVersion,
        this.formData.apiUrl,
        this.headers,
        {
          // sqlKey: this.formData.id,
          // sourceId: this.formData.executeConfig.sourceId,
          ...this.sqlResultForm,
          sqlText: this.editorValue.getValue(),
        }
      )
      .then((res: any) => {
        if (res.code == 200) {
          this.isRun = true;
          if (res.data.data && res.data.data.length > 0) {
            let keys = Object.keys(res.data.data[0]);
            this.resualtTableInfo.columnList = keys.map(key => ({
              field: key,
              header: key,
            }));
            this.resualtTableInfo.dataList = res.data.data;
          }
          this.sqlResultTotal = res.data.total;
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

  /** 导出excel */
  exportExcel() {
    // this.loading = true;
    // this.resualtTableInfo.dataList = [];
    this.commonhttpService
      .exportExcel(environment.BasicServer + `/services/${this.formData.apiVersion}${this.formData.apiUrl}`, this.formData.tableName, {
        pageNum: 1,
        pageSize: 200000,
        total: 1,
        to_excel: "1",
        sqlText: this.editorValue.getValue(),
      }, this.formData.reqMethod, this.headers);
  }
  backClick() {
    this.goBack.emit();
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
    this.resualtTableInfo.columnList = [];
    this.resualtTableInfo.dataList = [];
  }

  async initSourceTableList(sourceId: any) {
    const res = await this.dataTableHttpService.list({ sourceId: sourceId });

    if (res.data) {
      for (let i = 0; i < res.data.length; i++) {
        if (res.data[i].id === this.formData.executeConfig.tableId) {
          this.formData.tableName = res.data[i].tableName
          break
        }
      }
    }
  }
  async initSourceList() {
    const res = await this.dataSourceHttpService.list();

    if (res.data) {
      for (let i = 0; i < res.data.length; i++) {
        if (res.data[i].id === this.formData.executeConfig.sourceId) {
          this.formData.sourceName = res.data[i].sourceName
          break
        }
      }
    }
  }
}
