import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { MessageService } from 'primeng/api';
import { Router, ActivatedRoute } from '@angular/router';
import { Location } from '@angular/common';
import DataApiHttpService from 'src/api/dataAssetManage/api';
import ApiLogsHttpService from 'src/api/dataAssetManage/api/log';
import { formatDate } from '@angular/common';
import { TranslateData } from 'src/app/core/translate/translate-data';
@Component({
  selector: 'app-dataapi-api',
  templateUrl: './index.html',
  styleUrls: ['./index.scss'],
  providers: [MessageService, DataApiHttpService, ApiLogsHttpService],
})
export class DataapiApiComponent implements OnInit {
  @Output() goBack = new EventEmitter<string>();
  @Input() field: any;

  loading: boolean = false;
  btnActive: string = '1';
  stepList: any[];
  stepActiveIndex: number = 0;
  formData: any = {
    type: '',
    reqMethod: 'GET',
    apiName: '',
    reqParams: [],
    resParams: [],
    invalid: false,
  };

  titleName: string = '新增';
  changeSelectValue: string = '1';

  products: any[];
  isShowSelect: string = '1';
  tabMenuList: any[] = [
    { label: 'label.request.header', value: '0' },
    { label: 'label.request.parameter', value: '1' },
    { label: 'label.response.parameter', value: '2' },
  ];
  tabActive: number = 0;
  backTableData: any[] = [];
  backHeaderData: any[] = [];
  queryParams: any = {
    pageSize: 10,
    pageIndex: 1,
  };
  total: number = 0;
  headerTable: any[] = [];
  headerTableString: string = '';
  executeForm: any = {
    header: '',
    param: '',
    serviceNo: '',
    soap: '',
  };
  isShowNodata: boolean = false;
  constructor(
    public router: Router,
    private route: ActivatedRoute,
    private messageService: MessageService,
    private location: Location,
    private dataApiHttpService: DataApiHttpService,
    private apiLogsHttpService: ApiLogsHttpService
  ) {
    // this.route.queryParams.subscribe(params => {
    //   this.titleName = params['title'];
    //   const info = params['info'];
    //   this.formData = JSON.parse(info);
    //   this.getDetails(this.formData.id);
    // });
  }

  ngOnInit(): void {
    this.titleName = this.field?.title;
    this.formData = JSON.parse(JSON.stringify(this.field));
    this.getDetails(this.formData.id);
  }

  getDetails(id: any) {
    let executeData = {
      serviceNo: '',
      header: '',
      param: '',
      soap: '',
    };
    this.loading = true;
    this.dataApiHttpService.detailsApi(id).then((res: any) => {
      if (res.code == 200) {
        executeData.header = res.data.header.apiKey;
        const detail = res?.data?.data;
        const header = res?.data?.header;
        for (const key in header) {
          this.headerTable.push({ key: key, value: header[key] });
        }
        this.headerTable.forEach((item: any, index: number) => {
          item.index = index + 1;
        });

        if (detail) {
          this.formData.executeConfig = detail.executeConfig ? detail.executeConfig : {};
          this.formData.rateLimit = detail.rateLimit ? detail.rateLimit : {};
          this.formData.reqParams = detail.reqParams ? detail.reqParams : [];
          this.formData.resParams = detail.resParams ? detail.resParams : [];
          let arr = [
            {
              nullable: '1',
              paramName: 'pageSize',
              paramType: 'int',
              whereType: 'eq',
              defaultValue: 20,
              exampleValue: '20',
              paramComment: '每页数量',
              index: 2,
            },
            {
              nullable: '1',
              paramName: 'pageNum',
              paramType: 'int',
              whereType: 'eq',
              defaultValue: 1,
              exampleValue: '1',
              paramComment: '页码',
              index: 1,
            },
          ];
          this.formData.reqParams = [...arr, ...this.formData.reqParams];
          let list: any[] = [];
          list = JSON.parse(JSON.stringify(this.formData.reqParams));
          if (list.length == 0) return;
          let newList: any[] = this.processParams(list);
          let result = newList
            .map((item: any) => `${item.paramName}=${item.defaultValue}`)
            .join('&');
          this.executeForm.param = result;
        }
        executeData.param = JSON.stringify(this.formData.reqParams);
        this.headerTableString = JSON.stringify(this.headerTable);
        this.loading = false;
      } else {
        this.loading = false;
        this.messageService.add({
          severity: 'error',
          summary: res.msg,
        });
      }
    });
  }
  onBtns(info: any) {
    this.btnActive = info.value;
  }
  goback() {
    this.goBack.emit()
  }
  getHeaderParamsValue(value: any) {
    const result = value.reduce((obj: any, item: any) => {
      obj[item.key] = item.value.replace(/=/g, '');
      return obj;
    }, {});
    this.executeForm.header = result;
  }
  getHeaderValue(value: any) {
    let list: any[] = [];
    list = value;
    if (list.length == 0) return;
    let newList: any[] = this.processParams(list);
    let result = newList.map((item: any) => `${item.paramName}=${item.defaultValue}`).join('&');
    this.executeForm.param = result;
  }
  getParamsTable(value: any) { }
  processParams(params: any) {
    let result: any = [];
    params.forEach((param: any) => {
      if (param.whereType === 'between') {
        let startParam = { ...param };
        let endParam = { ...param };
        if (param.paramType === 'datetime') {
          startParam.paramName += '_start';
          endParam.paramName += '_end';
          startParam.paramType += '_start';
          endParam.paramType += '_end';
          let values = param.defaultValue.split('T');
          startParam.defaultValue = values[0];
          endParam.defaultValue = values[1];
        } else if (['time', 'tinyint', 'int', 'float', 'double'].includes(param.paramType)) {
          let values = param.defaultValue.split('-');
          startParam.defaultValue = values[0];
          endParam.defaultValue = values[1];
          startParam.paramName += '_start';
          endParam.paramName += '_end';
        }
        result.push(startParam);
        result.push(endParam);
      } else {
        result.push(param);
      }
    });

    return result;
  }
  processReqParams(formData: any): void {
    formData.reqParams.forEach((param: any) => {
      if (param.whereType != 'between') {
        if (param.paramType === 'year') {
          param.defaultValue = formatDate(param.defaultValue, 'yyyy', 'en-US');
        } else if (param.paramType === 'date') {
          param.defaultValue = formatDate(param.defaultValue, 'yyyy-MM-dd', 'en-US');
        } else if (param.paramType === 'datetime') {
          param.defaultValue = formatDate(param.defaultValue, 'yyyy-MM-dd HH:mm:ss', 'en-US');
        }
      }
    });
  }
  onSumit() {
    if (this.formData.reqParams && this.formData.reqParams.length > 0) {
      this.processReqParams(this.formData);
    }
    this.loading = true;
    this.apiLogsHttpService
      // 示例调用
      // .apiFunc(this.formData.apiVersion , this.formData.apiUrl, this.executeForm.header, {pageNum: 1, pageSize: 20, NTID: 105730})
      .apiFunc(
        this.formData.apiVersion,
        this.formData.apiUrl,
        this.executeForm.header,
        this.executeForm.param,
        this.formData.reqMethod
      )
      .then((res: any) => {
        if (res.code == 200) {
          this.backHeaderData = [];
          this.backTableData = res.data.data;
          for (const item of this.backTableData) {
            for (const key in item) {
              this.backHeaderData.push({ field: key, header: key });
            }
          }
          this.backHeaderData = this.backHeaderData.filter(
            (item, index, self) =>
              index === self.findIndex(t => t.field === item.field && t.header === item.header)
          );
          this.messageService.add({
            severity: 'success',
            summary: localStorage.getItem('lang') == 'en' ? 'success' : '成功',
          });
          this.isShowNodata = true;
          this.loading = false;
        } else {
          this.loading = false;
          this.messageService.add({ severity: 'error', summary: res.msg?.statusText || res.msg });
        }
      });
  }
  nextClick() {
    if (this.stepActiveIndex < 2) {
      this.stepActiveIndex += 1;
    }
  }
  backClick() {
    if (this.stepActiveIndex > 0) {
      this.stepActiveIndex -= 1;
    }
  }
  onChangeSelectValue(e: any) {
    this.changeSelectValue = e['selectValue'].value;
  }
  onChange(e: any) {
    this.isShowSelect = e.value;
    // this.changeSelectValue.emit({
    //   selectValue:e
    // })
  }
  tabClick(index: number, item: any) {
    this.tabActive = index;
  }
  onPageChange(event: any) {
    this.queryParams.pageIndex = event.first;
    this.queryParams.pageSize = event.rows;
  }
}
