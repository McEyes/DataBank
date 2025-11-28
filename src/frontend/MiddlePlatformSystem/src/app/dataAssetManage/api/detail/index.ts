import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { MessageService } from 'primeng/api';
import { Router, ActivatedRoute } from '@angular/router';
import { Location } from '@angular/common';
import { AttributeCompComponent } from './component/attribute-comp';
import { ImplementCompComponent } from './component/implement-comp';
import { ParameterCompComponent } from './component/parameter-comp';
import DataApiHttpService from 'src/api/dataAssetManage/api';

@Component({
  selector: 'app-api-detail',
  templateUrl: './index.html',
  styleUrls: ['./index.scss'],
  providers: [
    MessageService,
    DataApiHttpService
  ],
})
export class APIDetailComponent implements OnInit {
  @Output() goBack = new EventEmitter<string>();
  @Input() field: any;

  @ViewChild('attributeCompId') getAttributeComp: AttributeCompComponent;
  @ViewChild('implementCompId') getImplementComp: ImplementCompComponent;
  @ViewChild('parameterCompId') getParameterComp: ParameterCompComponent;

  btnActive: string = '1';
  stepList: any[];
  stepActiveIndex: number = 0;
  formData: any = {
    id: null,
    apiName: '',
    apiVersion: '',
    apiUrl: '',
    reqMethod: '',
    resType: '',
    // deny: '123.123.123.123',
    remark: '',
    status: '1', // 1: 待发布, 2: 已发布, 3: 已下线
    sourceId: '',
    executeConfig: {
      pageSizeLimit: 1000,
      configType: '1',
      sourceId: '',
      tableId: '',
      fieldParams: [],
    },
  };
  dropdownList: Array<any> = [
    { name: 'select1', id: 'select1' },
    { name: 'select2', id: 'select2' },
  ];
  titleName: string = 'add';
  titleType: any = {
    add: 'Add',
    edit: 'Edit',
    look: 'Look',
  };
  changeSelectValue: string = '1';
  langType: string = 'en';
  loading: boolean = false;
  editorText: string = '';
  sqlloading: boolean = false;
  // 配置类型
  isCollocationType: boolean = true;
  isSqlOver: boolean = false;
  noEdit: boolean = false;
  constructor(
    public router: Router,
    private route: ActivatedRoute,
    private messageService: MessageService,
    private dataApiHttpService: DataApiHttpService
  ) {

  }

  ngOnInit(): void {
    this.langType = localStorage.getItem('lang') || 'en';
    this.route.queryParams.subscribe(params => {
      this.titleName = params['title'] || this.field?.title;
      this.stepList = [
        { label: 'label.attribute.configuration', value: '1' },
        { label: 'label.execute.configuration', value: '2' },
        { label: 'label.parameter.configuration', value: '3' },
      ];
      if (this.titleName == 'add') return;
      let info = params['info'] || this.field.info;
      let formInfo = JSON.parse(info);
      this.noEdit = formInfo?.edit === '0'
      this.dataApiHttpService.detailsApi(formInfo.id).then((res: any) => {
        this.formData = res.data.data;
        let defaultStep = [
          { label: 'label.attribute.configuration', value: '1' },
          { label: 'label.execute.configuration', value: '2' },
          { label: 'label.parameter.configuration', value: '3' },
        ];
        let sqlStep = [
          { label: 'label.attribute.configuration', value: '1' },
          { label: 'label.execute.configuration', value: '2' },
        ];
        this.stepList = this.formData.executeConfig.configType == '3' ? sqlStep : defaultStep;
      });
    });
  }

  ngOnDestroy(): void {
    sessionStorage.removeItem('attributeCompForm');
    sessionStorage.removeItem('implementCompForm');
    sessionStorage.removeItem('parameterCompForm');
    sessionStorage.removeItem('editorValue');
    sessionStorage.removeItem('consultValue');
  }

  onBtns(info: any) {
    this.btnActive = info.value;
  }

  goback(type?: string) {
    this.goBack.emit(type)
  }

  async SQLQuery() {
    this.sqlloading = true;
    const res = await this.dataApiHttpService.sqlParse({
      sourceId: this.formData.executeConfig.sourceId,
      sqlText: this.getImplementComp.setEditorText(),
    });
    if (res.code == 200) {
      let data = res.data;
      this.formData.reqParams = [];
      this.formData.resParams = [];
      this.formData.reqParams = data.reqParams;
      this.formData.resParams = data.resParams;
      this.messageService.add({
        severity: 'success',
        summary: localStorage.getItem('lang') == 'en' ? 'SQL analysis' : 'SQL 解析',
        detail: localStorage.getItem('lang') == 'en' ? 'Success' : '成功',
      });
      setTimeout(() => {
        this.sqlloading = false;
        this.stepActiveIndex += 1;
      }, 500);
    } else {
      this.sqlloading = false;
      this.messageService.add({
        severity: 'error',
        summary: localStorage.getItem('lang') == 'en' ? 'Error' : '错误',
        detail: res.msg,
      });
    }
  }

  async onSumit() {
    if (this.stepActiveIndex == 2) {
      this.formData = this.getParameterComp.paramsCase();
      this.defalutSumbit();
    } else {
      this.sqlSumbit();
    }
  }

  async defalutSumbit() {
    this.loading = true;
    this.formData.reqParams = this.formData.reqParams.map((item: any) => {
      return {
        index: item.index,
        paramName: item.paramName,
        nullable: item.nullable,
        paramComment: item.paramComment,
        whereType: item.whereType,
        paramType: item.paramType,
        exampleValue: item.exampleValue,
        defaultValue: item.defaultValue,
      };
    });
    let params: any = JSON.parse(JSON.stringify(this.formData))
    params.executeConfig.fieldParams = params.executeConfig.columnList.map((item: any) => {
      return {
        reqable: item.checked,
        resable: item.resParams,
        dataType: item.dataType,
        colKey: item.colKey,
        dataScale: item.dataScale,
        colName: item.colName,
        dataLength: item.dataLength,
        dataDefault: item.dataDefault,
        colComment: item.colComment,
        dataPrecision: item.dataPrecision,
        colNullable: item.colNullable,
      }
    })

    const res = await this.dataApiHttpService.addOrUpdate(params);
    if (res.code == 200) {
      this.messageService.add({
        severity: 'success',
        summary: localStorage.getItem('lang') == 'en' ? 'Save' : '保存',
        detail: localStorage.getItem('lang') == 'en' ? 'Success' : '成功',
      });
      setTimeout(() => {
        this.goback('update');
        this.loading = false;
      }, 800);
    } else {
      this.loading = false;
      this.messageService.add({
        severity: 'error',
        summary: localStorage.getItem('lang') == 'en' ? 'Error' : '错误',
        detail: res.msg,
      });
    }
  }

  async sqlSumbit() {
    this.loading = true;
    const res = await this.dataApiHttpService.addSql(this.formData);
    if (res.code == 200) {
      this.messageService.add({
        severity: 'success',
        summary: localStorage.getItem('lang') == 'en' ? 'Save' : '保存',
        detail: localStorage.getItem('lang') == 'en' ? 'Success' : '成功',
      });

      setTimeout(() => {
        this.goback('update');
        this.loading = false;
      }, 800);
    } else {
      this.loading = false;
      this.messageService.add({
        severity: 'error',
        summary: localStorage.getItem('lang') == 'en' ? 'Error' : '错误',
        detail: res.msg,
      });
    }
  }

  nextClick() {
    if (this.stepActiveIndex < 2) {
      switch (this.stepActiveIndex) {
        case 0:
          let attributeForm: any = this.getAttributeComp.nextCase();
          if (attributeForm) {
            let attributeFormValue = JSON.parse(attributeForm);
            this.formData.apiName = attributeFormValue.apiName;
            this.formData.deny = attributeFormValue.deny;
            this.formData.apiUrl = attributeFormValue.apiUrl;
            this.formData.apiVersion = attributeFormValue.apiVersion;
            this.formData.remark = attributeFormValue.remark;
            this.formData.reqMethod = attributeFormValue.reqMethod;
            this.formData.resType = attributeFormValue.resType;
            this.formData.status = attributeFormValue.status;
            this.formData.times = attributeFormValue.times;
            this.formData.enable = attributeFormValue.enable;
            this.formData.seconds = attributeFormValue.seconds;
            this.formData.executeConfig.pageSizeLimit = attributeFormValue.executeConfig.pageSizeLimit
            sessionStorage.setItem('attributeCompForm', attributeForm);
            this.stepActiveIndex += 1;
          }
          break;
        case 1:
          let implementFrom: any = this.getImplementComp.nextCases();
          if (implementFrom) {
            let implementFormData = JSON.parse(implementFrom);
            implementFormData.apiName = this.formData.apiName;
            implementFormData.deny = this.formData.deny;
            implementFormData.apiUrl = this.formData.apiUrl;
            implementFormData.apiVersion = this.formData.apiVersion;
            implementFormData.remark = this.formData.remark;
            implementFormData.reqMethod = this.formData.reqMethod;
            implementFormData.resType = this.formData.resType;
            implementFormData.status = this.formData.status;
            implementFormData.times = this.formData.times;
            implementFormData.enable = this.formData.enable;
            implementFormData.seconds = this.formData.seconds;
            implementFormData.executeConfig.pageSizeLimit = this.formData.executeConfig.pageSizeLimit
            this.formData = implementFormData;
            sessionStorage.setItem('implementCompForm', implementFrom);
            this.formData.sourceId = this.formData.sourceId
              ? this.formData.sourceId
              : this.formData.executeConfig.sourceId;
            this.stepActiveIndex += 1;
            if (this.titleName == 'edit') {
              sessionStorage.setItem('parameterCompForm', JSON.stringify(this.formData));
            }
          }

          break;
        default:
          break;
      }
    }
  }

  backClick() {
    if (this.stepActiveIndex == 2) {
      let parameterForm: any = this.getParameterComp.paramsCase();
      this.formData.reqParams = parameterForm.reqParams;
      this.formData.resParams = parameterForm.resParams;
      sessionStorage.setItem('implementCompForm', JSON.stringify(parameterForm));
    } else if (this.stepActiveIndex == 1) {
      let implementFrom: any = this.getImplementComp.nextCases();
      if (implementFrom) {
        let implementFormData = JSON.parse(implementFrom);
        implementFormData.apiName = this.formData.apiName;
        implementFormData.deny = this.formData.deny;
        implementFormData.apiUrl = this.formData.apiUrl;
        implementFormData.apiVersion = this.formData.apiVersion;
        implementFormData.remark = this.formData.remark;
        implementFormData.reqMethod = this.formData.reqMethod;
        implementFormData.resType = this.formData.resType;
        implementFormData.status = this.formData.status;
        implementFormData.times = this.formData.times;
        implementFormData.enable = this.formData.enable;
        implementFormData.seconds = this.formData.seconds;
        implementFormData.executeConfig.pageSizeLimit = this.formData.executeConfig.pageSizeLimit;
        this.formData = implementFormData;
        sessionStorage.setItem('implementCompForm', implementFrom);
        this.formData.sourceId = this.formData.sourceId
          ? this.formData.sourceId
          : this.formData.executeConfig.sourceId;
      }
    } else if (this.stepActiveIndex == 0) {
      let attributeForm: any = this.getAttributeComp.nextCase();
      if (attributeForm) {
        let attributeFormValue = JSON.parse(attributeForm);
        this.formData.apiName = attributeFormValue.apiName;
        this.formData.deny = attributeFormValue.deny;
        this.formData.apiUrl = attributeFormValue.apiUrl;
        this.formData.apiVersion = attributeFormValue.apiVersion;
        this.formData.remark = attributeFormValue.remark;
        this.formData.reqMethod = attributeFormValue.reqMethod;
        this.formData.resType = attributeFormValue.resType;
        this.formData.status = attributeFormValue.status;
        this.formData.times = attributeFormValue.times;
        this.formData.enable = attributeFormValue.enable;
        this.formData.seconds = attributeFormValue.seconds;
        this.formData.executeConfig.pageSizeLimit = attributeFormValue.executeConfig.pageSizeLimit;
        sessionStorage.setItem('attributeCompForm', attributeForm);
      }
    }
    if (this.stepActiveIndex > 0) {
      this.stepActiveIndex -= 1;
    }
  }

  toCamelCase(columnName: string): string {
    const target = columnName.replace(/\_(\w)/g, (_match: string, letter: string) =>
      letter.toUpperCase()
    );
    return target ? target : columnName;
  }

  onChangeSelectValue(e: any) {
    this.changeSelectValue = e['selectValue'].value;
  }

  isShowSqlBtn(e: any) {
    this.formData.executeConfig.configType = e.configType;
    this.formData.executeConfig.sourceId = e.sourceId;
    let defaultStep = [
      { label: 'dataapi.attribute.configuration', value: '1' },
      { label: 'dataapi.execute.configuration', value: '2' },
      { label: 'dataapi.parameter.configuration', value: '3' },
    ];
    let sqlStep = [
      { label: 'dataapi.attribute.configuration', value: '1' },
      { label: 'dataapi.execute.configuration', value: '2' },
    ];
    this.stepList = e.configType == '3' ? sqlStep : defaultStep;
  }
}
