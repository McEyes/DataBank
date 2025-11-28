import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import DataSourceHttpService from 'src/api/dataAssetManage/dataSource';

@Component({
  selector: 'app-datatable-field',
  templateUrl: './index.html',
  styleUrls: ['./index.scss'],
  providers: [
    DataSourceHttpService
  ],
})
export class DatatableFieldComponent implements OnInit {
  @Output() goBack = new EventEmitter<string>();
  @Input() field: any;

  tableData: any[] = [];
  loading: boolean = false;
  params: any = {}
  currPanel: string = 'list'
  constructor(
    private sourceService: DataSourceHttpService,
  ) {}

  ngOnInit(): void {
    this.loading = true;
    this.getData()
  }

  async getData() {
    this.sourceService.getTableColumns(this.field.sourceId, this.field.tableName).then((res: any) => {
      this.tableData = res.data;
      this.loading = false;
    });
  }

  goback() {
    this.goBack.emit()
  }

  goPage(data: any) {
    this.currPanel = 'detail'
    data.sourceId = this.field.sourceId
    data.tableId = this.field.id
    this.params = data
  }

  backEvent(e: string) {
    if(e === 'update') {
      this.getData()
    }
    this.currPanel = 'list';
  }
}
