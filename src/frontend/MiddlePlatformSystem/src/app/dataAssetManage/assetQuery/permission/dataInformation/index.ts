import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { CommonService } from 'jb.package/jabil-bus-lib';
import { ConfirmationService, MessageService } from 'primeng/api';
import DataColumnHttpService from 'src/api/dataAssetManage/dataColumn';
import DataSourceHttpService from 'src/api/dataAssetManage/dataSource';
import DataTableHttpService from 'src/api/dataAssetManage/dataTable';

@Component({
  selector: 'app-asset-query-data-info',
  templateUrl: './index.html',
  styleUrls: ['./index.scss'],
  providers: [
    DataSourceHttpService,
    DataColumnHttpService,
    ConfirmationService,
    MessageService,
    CommonService
  ],
})
export class AssetQueryDataInfoComponent implements OnInit {
  @Output() goBack = new EventEmitter<string>();
  @Input() field: any;
  @Input() detailData: any;
  @Input() titleName: any;

  tableData: any[] = [];
  loading: boolean = false;
  tableHeaderData: any[] = [
    { field: 'colName', header: 'field.field.name' },
    { field: 'colComment', header: 'field.field.comment' },
    { field: 'colKey', header: 'field.primary.key' },
    { field: 'levelName', header: 'field.security.level' },
    { field: 'nullable', header: 'field.null' },
    { field: 'dataType', header: 'field.data.type' },
    { field: 'dataLength', header: 'field.data.length' },
    { field: 'dataPrecision', header: 'field.data.accuracy' },
    { field: 'dataScale', header: 'field.decimal.places' },
    { field: 'dataDefault', header: 'field.default.value' },
  ];
  selectedItems: any[] = [];
  params: any = {
    pageSize: 10,
    pageNum: 1,
  };
  total: number = 0
  constructor(
    private sourceService: DataSourceHttpService,
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
    private columnService: DataColumnHttpService,
    private commonService: CommonService,
    private dataTableHttpService: DataTableHttpService
  ) {

  }

  ngOnInit(): void {
    // this.params.tableId = this.formData?.id || '';
    // this.params.sourceId = this.formData?.sourceId || '';
    // this.params.pageNum = 1;
    // this.params.pageSize = 10;
    // console.log(this.field)
    // this.getMessageList()

    this.tableData = this.detailData.columnList
    // this.total = result.data.total;
    this.tableData.forEach((item: any)=>{
      if(!item.unSet) {
        this.selectedItems.push(item)
      }
    })
    // this.selectedItems = this.tableData;
  }

  goback() {
    this.goBack.emit()
  }

  nzPageIndexChange(index: number) {
    this.params.pageNum = index;
    this.getMessageList();
  }
  nzPageSizeChange(page: number) {
    this.params.pageSize = page;
    this.getMessageList();
  }

  getMessageList() {
    this.dataTableHttpService.getTablesById(this.field.id).then((result: any) => {
      if (result.code == 200) {
        this.tableData = result.data.data
        this.total = result.data.total;
        this.selectedItems = this.tableData;
      } else {
        this.messageService.add({
          severity: 'error',
          summary: localStorage.getItem('lang') == 'en' ? 'error' : '错误',
          detail: result.msg,
        });
      }
    });
  }
}
