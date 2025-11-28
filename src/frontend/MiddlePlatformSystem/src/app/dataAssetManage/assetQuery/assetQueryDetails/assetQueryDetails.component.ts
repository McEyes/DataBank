import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Location } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';
import AssetQueryHttpService from 'src/api/dataAssetManage/assetQuery'
import { en_US, NzI18nService, zh_CN } from 'ng-zorro-antd/i18n';

@Component({
  selector: 'app-asset-query-details',
  templateUrl: './assetquerydetails.component.html',
  styleUrls: ['./assetquerydetails.component.scss'],
  providers: [
    AssetQueryHttpService,
  ],
})
export class AssetquerydetailsComponent implements OnInit {
  @Output() goBack = new EventEmitter<string>();
  @Input() field: any;
  tableData: any[] = [];
  total: number;
  queryParams: any = {
    pageNum: 1,
    pageSize: 999,
  };
  tableForm: any = {
    title: '',
  };
  first: number = 0;
  id: any;
  loading: boolean = false;
  constructor(
    private location: Location,
    public router: Router,
    private route: ActivatedRoute,
    private httpService: AssetQueryHttpService,
    private i18n: NzI18nService
  ) {}

  ngOnInit(): void {
    //查询表下的列
    this.queryParams.tableId = this.field.id;
    this.getList();
    this.i18n.setLocale(localStorage.getItem('lang') === 'en' ? en_US : zh_CN);
  }
  
  // nzPageIndexChange(index: number) {
  //   this.queryParams.pageNum = index;
  //   this.getList();
  // }

  // nzPageSizeChange(page: number) {
  //   this.queryParams.pageSize = page;
  //   this.getList();
  // }

  getList() {
    this.loading = true;
    this.httpService.page(this.queryParams).then((result: any) => {
      if (result.code == 200) {
        this.tableData = result.data.data;
        this.total = result.data.total;
        this.loading = false;
      }
    });
  }

  // onPageChange(event: any) {
  //   this.first = event.first;
  //   this.queryParams.pageNum = event.page + 1;
  //   this.queryParams.pageSize = event.rows;
  //   this.getList();
  // }

  goback() {
    this.goBack.emit()
  }
}
