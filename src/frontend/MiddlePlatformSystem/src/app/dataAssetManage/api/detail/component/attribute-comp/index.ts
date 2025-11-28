import { Component, OnInit, Input, SimpleChanges, Output, EventEmitter } from '@angular/core';
import { CommonService } from 'jabil-bus-lib';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'attributeComp',
  templateUrl: './index.html',
  styleUrls: ['./index.scss'],
  providers: [MessageService],
})
export class AttributeCompComponent implements OnInit {
  @Input() formData: any;
  @Input() formType?: string = '新增';
  @Input() stepActiveIndex?: number;

  // @Output() attributeCompDetails = new EventEmitter();
  attributeForm: any = {
    apiName: '',
    apiVersion: 'v1.0.0',
    apiUrl: '',
    reqMethod: 'GET',
    resType: 'JSON',
    deny: '',
    remark: '',
    status: '2', // 1: 待发布, 2: 已发布, 3: 已下线
    sourceId: '',
    executeConfig: {
      pageSizeLimit: 1000,
      configType: '1',
      sourceId: '',
      tableId: '',
      fieldParams: [],
    },
    rateLimit: {
      enable: '1',
      times: 3,
      seconds: 60,
    }
  };
  isEnable: boolean = true;
  attributeInvalid: Boolean = false;

  // 字典
  dict: any = {
    // 请求方式
    dataReqMethod: [
      { text: 'GET', value: 'GET' },
      { text: 'POST', value: 'POST' },
      // { text: 'PUT', value: 'PUT' },
      // { text: 'DELETE', value: 'DELETE' },
    ],
    // 返回方式
    dataResType: [
      { text: 'JSON', value: 'JSON' },
      { text: 'XML', value: 'XML' },
      { text: 'TEXT', value: 'TEXT' },
    ],
    secondsList: [
      { text: localStorage.getItem('lang') == 'en' ? '1 s' : '1秒', value: 1 },
      { text: localStorage.getItem('lang') == 'en' ? '1 min' : '1分钟', value: 60 },
      { text: localStorage.getItem('lang') == 'en' ? '5 min' : '5分钟', value: 300 },
      { text: localStorage.getItem('lang') == 'en' ? '10 min' : '10分钟', value: 600 },
      { text: localStorage.getItem('lang') == 'en' ? '1 hour' : '1小时', value: 3600 },
      { text: localStorage.getItem('lang') == 'en' ? '1 day' : '1天', value: 86400 },
      { text: localStorage.getItem('lang') == 'en' ? '30 days' : '30天', value: 2592000 },
    ]
  };

  currLang: string = 'en'
  constructor(
    private commonService: CommonService,
    private messageService: MessageService
  ) { 
    this.currLang = localStorage.getItem('lang') || 'en'
  }
  ngOnChanges(changes: SimpleChanges): void {
    if (sessionStorage.getItem('attributeCompForm')) {
      let getItem: any = sessionStorage.getItem('attributeCompForm');
      if (!getItem) return
      let formInfo: any = JSON.parse(getItem);
      this.attributeForm = { ...formInfo };
      this.attributeForm.enable = this.attributeForm?.rateLimit?.enable
      this.attributeForm.seconds = this.attributeForm?.rateLimit?.seconds
      this.attributeForm.times = this.attributeForm?.rateLimit?.times
    } else {
      if (this.formType != 'add' && changes['formData']) {
        let info: any = changes['formData'].currentValue;
        this.attributeForm = { ...info };
        if (!this.attributeForm.executeConfig.pageSizeLimit) {
          this.attributeForm.executeConfig.pageSizeLimit = 1000
        }
        if (!this.attributeForm.rateLimit) {
          this.attributeForm.rateLimit = {
            enable: '1',
            times: 3,
            seconds: 60,
          }
        }
      }
    }
  }
  ngOnInit(): void { }
  changeEnable(e: any) {
    this.attributeForm.rateLimit.enable = e.checked ? '1' : '0';
  }
  nextCase() {
    const valid = [
      'apiName',
      'apiUrl',
      'apiVersion',
      'reqMethod',
      'resType',
      'remark',
      'status',
      'rateLimit.enable',
      'rateLimit.times',
      'rateLimit.seconds',
    ];
    if (this.commonService.isInvalid(this.attributeForm, valid)) {
      this.messageService.add({ severity: 'warn', summary: 'warn' });
      this.attributeInvalid = true;
      return false;
    } else {
      return JSON.stringify(this.attributeForm);
    }
  }
  onInput(e: any) {
    sessionStorage.setItem('attributeCompForm', this.attributeForm);
  }
  onChange(e: any) {
    sessionStorage.setItem('attributeCompForm', this.attributeForm);
  }
}
