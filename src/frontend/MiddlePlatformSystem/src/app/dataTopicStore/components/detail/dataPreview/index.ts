import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { MessageService } from 'primeng/api';
import CommonHttpService from 'src/api/common';
import DataTopicStoreHttpService from 'src/api/dataTopicStore';
import { TranslateData } from 'src/app/core/translate/translate-data'
import { environment } from 'src/environments/environment';
@Component({
  selector: 'app-data-topic-preview',
  templateUrl: './index.html',
  styleUrls: ['./index.scss'],
  providers: [MessageService, DataTopicStoreHttpService],
})
export class DatapreviewComponent implements OnInit {
  @Output() goBack = new EventEmitter<string>();
  @Input() field: any;
  tableData: any = {
    columnList: [],
    dataList: [],
  };
  total: number = 0;
  loading: boolean = false;
  updateMethodObj: any = {
    manual: TranslateData.manual,
    automatic: TranslateData.automatic
  }
  isArray: boolean = true
  objectKeks = Object.keys
  headerData: any = []
  bodyData: any = []

  listForm: any = []
  dataValue: any = null
  queryParams: any = {}
  constructor(
    private readonly messageService: MessageService,
    private readonly httpService: DataTopicStoreHttpService,
    private readonly commonHttpService: CommonHttpService,
  ) { }

  ngOnInit(): void {
    if (this.field?.id) {
      this.getPageData()
    }
  }

  goback() {
    this.goBack.emit()
  }

  async getPageData() {
    this.loading = true;
    let queryParams: any = {}
    for (let item in this.queryParams) {
      queryParams['field_' + item] = this.queryParams[item]
    }

    const res = await this.httpService.previewData(this.field.id, queryParams);
    if (res.succeeded) {
      this.showDataValue(res.data)
      this.loading = false;
    } else {
      this.messageService.add({
        severity: 'error',
        summary: TranslateData.dataFetchingFailed,
        detail: res.errors,
      });
    }
  }

  showDataValue(data: any) {
    this.dataValue = data
    this.isArray = Array.isArray(this.dataValue)
    if (this.isArray && data.length > 0) {
      const data = this.dataValue[0] || this.dataValue?.data[0]
      this.headerData = Object.keys(data)
    }

    if (this.dataValue?.totalCount) {
      this.isArray = true
      const data = this.dataValue?.data[0]
      this.dataValue = this.dataValue?.data
      this.headerData = Object.keys(data)
    }
  }

  async downloadDataTable() {
    this.httpService.download(environment.DataTopicStoreServer + "/api/topic/preview/" + this.field.id + "/export")
  }

  onConfirmClick(item: any, filter: any) {
    filter.overlayVisible = false;
    this.getPageData();
  }
  
  onClearClick(item: any, filter: any) {
    this.queryParams[item.filterName] = null;
    filter.overlayVisible = false;
    this.getPageData();
  }
  
  onClearAll() {
    this.queryParams = {};
    this.getPageData();
  }
}
