import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Location } from '@angular/common';
import { MessageService, SortEvent } from 'primeng/api';
import { Router, ActivatedRoute } from '@angular/router';

import AssetQueryHttpService from 'src/api/dataAssetManage/assetQuery';
import { TranslateData } from 'src/app/core/translate/translate-data'


@Component({
  selector: 'app-data-preview',
  templateUrl: './datapreview.component.html',
  styleUrls: ['./datapreview.component.scss'],
  providers: [MessageService, AssetQueryHttpService],
})
export class DatapreviewComponent implements OnInit {
  @Output() goBack = new EventEmitter<string>();
  @Input() field: any;
  tableData: any = {
    columnList: [],
    dataList: [],
  };
  total: number = 0;
  sortField: string = '';
  isSorted: boolean | null = null;
  loading: boolean = false;
  updateMethodObj: any = {
    manual: TranslateData.manual,
    automatic: TranslateData.automatic
  }
  constructor(
    public router: Router,
    private messageService: MessageService,
    private httpService: AssetQueryHttpService
  ) { }

  ngOnInit(): void {
    this.getPageData()
  }

  goback() {
    this.goBack.emit()
  }

  async getPageData() {
    this.loading = true;
    const res = await this.httpService.dataPreview(this.field.id, 20, this.sortField);
    if (res.success) {
      res.data.dataList.forEach((item: any) => {
        for (var key in item)
          if (item[key] && item[key].replace) {
            item[key] = this.processDateString(item[key])
          }
      })
      this.tableData.dataList = res.data.dataList;
      this.tableData.columnList = res.data.columnList;
      this.total = this.tableData.length;
      this.loading = false;
    } else {
      this.messageService.add({
        severity: 'error',
        summary: TranslateData.dataFetchingFailed,
        detail: res.msg,
      });
    }
  }
  // 正则表达式匹配 YYYY-MM-DD HH:MM:SS.xxx 格式的日期字符串
  dateRegex = /^\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}\.\d+$/;
  processDateString(str: any) {
    if (this.dateRegex.test(str)) {
      return str.split('.')[0].replace(" 00:00:00", "");
    } else {
      return str;
    }
  }
  customSort(event: SortEvent) {
    var sortField = event.field + (event.order == 1 ? " DESC" : " ASC");
    if (this.sortField != sortField) {
      this.sortField = sortField;
      this.getPageData()
    }
  }
}
