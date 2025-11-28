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
    const res = await this.httpService.dataPreview(this.field.id);
    if (res.success) {
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
  customSort(event: SortEvent) {
    // if (this.isSorted == null || this.isSorted === undefined) {
    //   this.isSorted = true;
    //   this.sortTableData(event);
    // } else if (this.isSorted == true) {
    //   this.isSorted = false;
    //   this.sortTableData(event);
    // } else if (this.isSorted == false) {
    //   this.isSorted = null;
    //   this.products = [...this.initialValue];
    //   this.dt.reset();
    // }
  }
}
