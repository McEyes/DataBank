import { Component, OnInit, Input, Output, EventEmitter, SimpleChanges } from '@angular/core';
import { CommonService } from 'jabil-bus-lib';
import { MessageService } from 'primeng/api';
import DictHttpService from 'src/api/common/dict';
import { formatDate } from '@angular/common';
import { LOCALE_ID, Inject } from '@angular/core';
@Component({
  selector: 'parameterComp',
  templateUrl: './index.html',
  styleUrls: ['./index.scss'],
  providers: [MessageService, DictHttpService],
})
export class ParameterCompComponent implements OnInit {
  @Input() formData: any;
  @Input() formType: string = 'add';
  @Input() stepActiveIndex?: number;

  @Output() changeSelectValue = new EventEmitter();
  @Output() checkTableData = new EventEmitter();

  tableIdList: Array<any> = [
    { name: '是', id: '1' },
    { name: '否', id: '2' },
  ];

  dict: any = {
    dataQueryType: [
      { text: '=', value: 'eq' },
      { text: '!=', value: 'ne' },
      { text: '>', value: 'gt' },
      { text: '>=', value: 'ge' },
      { text: '<', value: 'lt' },
      { text: '<=', value: 'le' },
      { text: 'like', value: 'like' },
      { text: 'between', value: 'between' },
    ],
  };

  paramsForm: any = {
    resParams: [],
    reqParams: [],
  };
  dataTypeList: any[] = [];
  whereTypeList: any[] = [];
  lange: any = 'en';
  dateValueStarTime: Date;
  dateValueEndTime: Date;
  inputType: any = {
    tinyint: 'number',
    bigint: 'number',
    int: 'number',
    float: 'number',
    double: 'number',
    decimal: 'number',
    char: 'text',
    varchar: 'text',
    text: 'text',
  };
  betweenDate: any;
  betweenStartValue: any;
  betweenEndValue: any;
  showDataTime:string=''
  constructor(
    private commonService: CommonService,
    private messageService: MessageService,
    private dictHttpService: DictHttpService,
    @Inject(LOCALE_ID) private locale: string
  ) {}
  ngOnInit(): void {
    this.lange = localStorage.getItem('lang');
  }
  ngOnChanges(changes: SimpleChanges): void {
    this.getDictFunc();
    if (this.stepActiveIndex == 2) {
      this.paramsForm = {};
      if (changes['formData'].currentValue == 'null') return;
      let info: any = changes['formData'] && changes['formData'].currentValue;
      if (info === null) {
        return;
      }
      this.paramsForm = { ...info };
      console.log(' this.paramsForm====', this.paramsForm);
      
      if (this.paramsForm.executeConfig.configType == 1) {
        console.log(111111);
        
        let arr = this.paramsForm.executeConfig.columnList;
        let oldInfo: any = sessionStorage.getItem('parameterCompForm');
        if (oldInfo) {
          console.log(666);

          let oldInfoValue: any = JSON.parse(oldInfo);
          // let oldReqParamsString = JSON.stringify(oldInfoValue.reqParams);
          // let newReqParamsString = JSON.stringify(this.paramsForm.reqParams);
          // if (oldReqParamsString == newReqParamsString) return;
          this.paramsForm.reqParams = arr
            .filter((item: any) => item.checked)
            .map((citem: any, index: number) => {
              return {
                index: index + 1,
                paramName: citem.colName,
                nullable: citem.colNullable,
                paramComment: citem.colComment,
                whereType: 'eq',
                paramType: citem.dataType,
                exampleValue: '123',
                defaultValue: citem.dataDefault,
              };
            });
          this.paramsForm.reqParams.forEach((citem: any, index: number) => {
            oldInfoValue.reqParams.forEach((zitem: any) => {
              if (citem.paramName == zitem.paramName) {
                console.log(7777);
                
                citem.whereType = zitem.whereType;
                citem.defaultValue = zitem.defaultValue;
                citem.whereType = zitem.whereType;

                let timeTypeList = ['datetime', 'date', 'year'];
                let jsTypeList = ['tinyint', 'int', 'float', 'double', 'decimal', 'time','bigint'];
                if (
                  jsTypeList.includes(citem.paramType) &&
                  citem.whereType == 'between' &&
                  citem.defaultValue.includes('-')
                ) {
                  let arr = citem.defaultValue.split('-');
                  citem.betweenStartValue = arr[0];
                  citem.betweenEndValue = arr[1];
                }
                if (
                  timeTypeList.includes(citem.paramType) &&
                  citem.whereType == 'between' &&
                  citem.defaultValue.includes('T')
                ) {
                  let arr = citem.defaultValue.split('T');
                  this.showDataTime = `${arr[0]} ${this.lange == 'en'?'To':"至"} ${arr[1]}`
                  citem.betweenDate = arr.map((v: any) => {
                    v = new Date(v);
                    return v;
                  });
                }
              }
            });
          });
          this.paramsForm.resParams = arr
            .filter((item: any) => item.resParams)
            .map((citem: any, index: number) => {
              return {
                index: index + 1,
                fieldName: citem.colName,
                fieldComment: citem.colComment,
                dataType: citem.dataType,
                exampleValue: '123',
                fieldAliasName: citem.colName,
              };
            });
          this.checkTableData.emit({
            arrValue: this.paramsForm.executeConfig.columnList,
          });
        } else {
          this.paramsForm.reqParams = arr
            .filter((item: any) => item.checked)
            .map((citem: any, index: number) => {
              return {
                index: index + 1,
                paramName: citem.colName,
                nullable: citem.colNullable,
                paramComment: citem.colComment,
                whereType: 'eq',
                paramType: citem.dataType,
                exampleValue: '123',
                defaultValue: citem.dataDefault,
              };
            });
          this.paramsForm.resParams = arr
            .filter((item: any) => item.resParams)
            .map((citem: any, index: number) => {
              return {
                index: index + 1,
                fieldName: citem.colName,
                fieldComment: citem.colComment,
                dataType: citem.dataType,
                exampleValue: '123',
                fieldAliasName: citem.colName,
              };
            });
          this.checkTableData.emit({
            arrValue: this.paramsForm.executeConfig.columnList,
          });
        }
      } else {
        console.log(222222);

        this.paramsForm.reqParams.forEach((citem: any, index: number) => {
          citem.index = index + 1;
          citem.whereType = citem.whereType ? citem.whereType : 'eq';
          citem.exampleValue = '123';

          // citem.index = index + 1;
          // citem.paramComment = '123';
          // citem.whereType = 'eq';
          // citem.paramType = 'int';
          // citem.exampleValue = '123';
          // citem.defaultValue = '456';
        });
        this.paramsForm.resParams.forEach((citem: any, index: number) => {
          citem.index = index + 1;
          citem.exampleValue = '123';

          // citem.index = index + 1;
          // citem.fieldName = '11111';
          // citem.nullable = '11111';
          // citem.fieldComment = '11111';
          // citem.dataType = 'eq';
          // citem.paramType = 'int';
          // citem.exampleValue = '123';
          // citem.fieldAliasName = '456';
        });
      }

      // if (this.formType != 'add') {
      //   info.reqParams.forEach((item: any) => {
      //     this.paramsForm.reqParams.forEach((citem: any) => {
      //       if (citem.paramName == item.paramName) {
      //         citem.defaultValue = item.defaultValue;
      //       }
      //     });
      //   });
      //   info.resParams.forEach((item: any) => {
      //     this.paramsForm.resParams.forEach((zitem: any) => {
      //       if (zitem.fieldName == item.fieldName) {
      //         zitem.defaultValue = item.defaultValue;
      //       }
      //     });
      //   });
      // }
    }
  }
  onChangeDefaultDateTime(e: any, info: any) {
    let time: any = formatDate(info.betweenDate[0], 'yyyy-MM-dd hh:mm:ss', this.locale);
    this.paramsForm.reqParams.forEach((citem: any) => {
      if (citem.index == info.index) {
        citem.defaultValue = time;
      }
    });
    let newParamsForm = { ...this.paramsForm };
    newParamsForm.reqParams = newParamsForm.reqParams.map((item: any) => {
      return {
        defaultValue: item.defaultValue,
        exampleValue: item.exampleValue,
        index: item.index,
        nullable: item.nullable,
        paramComment: item.paramComment,
        paramName: item.paramName,
        paramType: item.paramType,
        whereType: item.whereType,
      };
    });
    sessionStorage.setItem('parameterCompForm', JSON.stringify(newParamsForm));
    sessionStorage.setItem('implementCompForm', JSON.stringify(newParamsForm));
  }
  onChangeDate(e: any, info: any) {
    let startTime: any = null;
    let endTime: any = null;
    if (info.paramType == 'datetime') {
      startTime = formatDate(info.betweenDate[0], 'yyyy-MM-dd hh:mm:ss', this.locale);
      endTime = formatDate(info.betweenDate[1], 'yyyy-MM-dd hh:mm:ss', this.locale);
      this.showDataTime = `${startTime} ${this.lange == 'en'?'To':"至"} ${endTime}`

    } else if (info.paramType == 'year') {
      startTime = formatDate(info.betweenDate[0], 'yyyy', this.locale);
      endTime = formatDate(info.betweenDate[1], 'yyyy', this.locale);
    } else if (info.paramType == 'date') {
      startTime = formatDate(info.betweenDate[0], 'yyyy-MM-dd', this.locale);
      endTime = formatDate(info.betweenDate[1], 'yyyy-MM-dd', this.locale);
    }
    this.paramsForm.reqParams.forEach((citem: any) => {
      if (citem.index == info.index) {
        citem.defaultValue = startTime + 'T' + endTime;
      }
    });
    let newParamsForm = { ...this.paramsForm };
    newParamsForm.reqParams = newParamsForm.reqParams.map((item: any) => {
      return {
        defaultValue: item.defaultValue,
        exampleValue: item.exampleValue,
        index: item.index,
        nullable: item.nullable,
        paramComment: item.paramComment,
        paramName: item.paramName,
        paramType: item.paramType,
        whereType: item.whereType,
      };
    });
    sessionStorage.setItem('parameterCompForm', JSON.stringify(newParamsForm));
    sessionStorage.setItem('implementCompForm', JSON.stringify(newParamsForm));
  }
  getDictFunc() {
    this.dictHttpService.code('data_type_mysql').then((res: any) => {
      if (res.code == 200) {
        this.dataTypeList = res.data;
        if(this.dataTypeList.length>0){
          this.dataTypeList.forEach((item:any)=>{
            item.itemValue = localStorage.getItem('lang') == 'en' ?item.itemText:item.itemValue
          })
        }
      } else {
        // this.commonHttp.dictsRefresh();
        // this.getDictFunc();
      }
    });
    this.dictHttpService.code('data_query_type').then((res: any) => {
      if (res.code == 200) {
        this.whereTypeList = res.data;
      } else {
        // this.commonHttp.dictsRefresh();
        // this.getDictFunc();
      }
    });
  }
  paramsCase() {
    return this.paramsForm;
  }
  onChangeWhereType(e: any, info: any) {
    let value = e.value;
    let arr: any = ['char', 'varchar', 'text'];
    if (arr.includes(info.paramType) && value == 'between') {
  
      this.messageService.add({
        severity: 'warn',
        summary: localStorage.getItem('lang') == 'en' ? 'Tip' : '提示',
        detail:
          localStorage.getItem('lang') == 'en'
            ? `The parameter type does not allow the selection of the 'between' operator`
            : '该参数类型不允许选择‘between’操作符',
      });
    }
    info.defaultValue = '';
  }
  onBlur(e: any) {
    let newParamsForm = { ...this.paramsForm };
    newParamsForm.reqParams = newParamsForm.reqParams.map((item: any) => {
      return {
        defaultValue: item.defaultValue,
        exampleValue: item.exampleValue,
        index: item.index,
        nullable: item.nullable,
        paramComment: item.paramComment,
        paramName: item.paramName,
        paramType: item.paramType,
        whereType: item.whereType,
      };
    });
    sessionStorage.setItem('parameterCompForm', JSON.stringify(newParamsForm));
    sessionStorage.setItem('implementCompForm', JSON.stringify(newParamsForm));
  }
  onBlurStart(e: any, info: any) {
    let newParamsForm = { ...this.paramsForm };
    if(info.betweenStartValue&&info.betweenStartValue <0){
      info.betweenStartValue = 0
    }
    if(info.betweenStartValue&&info.betweenEndValue&&info.betweenStartValue >info.betweenEndValue){
      info.betweenStartValue = info.betweenEndValue
    }
    newParamsForm.reqParams = newParamsForm.reqParams.map((item: any) => {
      return {
        defaultValue: item.defaultValue,
        exampleValue: item.exampleValue,
        index: item.index,
        nullable: item.nullable,
        paramComment: item.paramComment,
        paramName: item.paramName,
        paramType: item.paramType,
        whereType: item.whereType,
      };
    });
    sessionStorage.setItem('parameterCompForm', JSON.stringify(newParamsForm));
    sessionStorage.setItem('implementCompForm', JSON.stringify(newParamsForm));
  }
  onBlurEnd(e: any, info: any) {
    let value = e.value;
    if(info.betweenEndValue&&info.betweenEndValue <0){
      info.betweenEndValue = 0
    }
    if(info.betweenStartValue&&info.betweenEndValue&&info.betweenStartValue >info.betweenEndValue){
      info.betweenEndValue = info.betweenStartValue
    }
    this.paramsForm.reqParams.forEach((citem: any) => {
      if (citem.index == info.index) {
        citem.defaultValue = citem.betweenStartValue + '-' + citem.betweenEndValue;
      }
    });
    let newParamsForm = { ...this.paramsForm };
    newParamsForm.reqParams = newParamsForm.reqParams.map((item: any) => {
      return {
        defaultValue: item.defaultValue,
        exampleValue: item.exampleValue,
        index: item.index,
        nullable: item.nullable,
        paramComment: item.paramComment,
        paramName: item.paramName,
        paramType: item.paramType,
        whereType: item.whereType,
      };
    });
    sessionStorage.setItem('parameterCompForm', JSON.stringify(newParamsForm));
    sessionStorage.setItem('implementCompForm', JSON.stringify(newParamsForm));
  }
}
