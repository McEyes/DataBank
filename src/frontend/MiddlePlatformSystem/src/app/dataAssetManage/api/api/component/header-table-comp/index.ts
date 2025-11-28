import {
  Component,
  OnInit,
  Input,
  SimpleChanges,
  ChangeDetectorRef,
  Output,
  EventEmitter,
} from '@angular/core';
import DictHttpService from 'src/api/common/dict';
import { MessageService } from 'primeng/api';
import { formatDate } from '@angular/common';
import { LOCALE_ID, Inject } from '@angular/core';
@Component({
  selector: 'headerTableComp',
  templateUrl: './index.html',
  styleUrls: ['./index.scss'],
  providers: [DictHttpService, MessageService],
})
export class HeaderTableCompComponent implements OnInit {
  @Output() headerTable = new EventEmitter();
  @Input() products: any[] = [];
  tableData: any[];
  dataTypeList: any[] = [];
  nullableList: any[] = [
    { label: '是', value: '1' },
    { label: '否', value: '0' },
  ];
  paramTypeList: any[] = [];
  selectedColumns: any[] = [];
  whereTypeList: any[] = [];
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
  lange: any = 'en';
  showDataTime:string=''

  paramsObj: any = {}
  operatorObj: any = {}
  constructor(
    private cdr: ChangeDetectorRef,
    private dictHttpService: DictHttpService,
    private messageService: MessageService,
    @Inject(LOCALE_ID) private locale: string

  ) {}

  ngOnChanges(changes: SimpleChanges): void {
    this.selectedColumns = [];
    this.cdr.detectChanges();
    let columnData: any[] = Array.isArray(changes['products'].currentValue)
      ? changes['products'].currentValue
      : [];
    columnData && columnData.forEach((element: any, index: number) => (element.index = index + 1));
    this.tableData = [...columnData].map((item:any)=>{
      let timeTypeList = ['datetime', 'date', 'year'];
      let jsTypeList = ['tinyint', 'int', 'float', 'double', 'decimal', 'time','bigint'];
      if (
        jsTypeList.includes(item.paramType) &&
        item.whereType == 'between' &&
        item.defaultValue.includes('-')
      ) {
        let arr = item.defaultValue.split('-');
        item.betweenStartValue = arr[0];
        item.betweenEndValue = arr[1];
      }
      if (
        timeTypeList.includes(item.paramType) &&
        item.whereType == 'between' &&
        item.defaultValue.includes('T')
      ) {
        let arr = item.defaultValue.split('T');
        this.showDataTime = `${arr[0]} ${this.lange == 'en'?'To':"至"} ${arr[1]}`
        item.betweenDate = arr.map((v: any) => {
          v = new Date(v);
          return v;
        });
      }
      return item
    })

    this.selectedColumns = this.tableData.filter(
      (item: any) => item.paramName == 'pageSize' || item.paramName == 'pageNum'
    );
    this.getDictFunc();
  }
  ngDoCheck(): void {
    this.headerTable.emit(this.selectedColumns);
  }
  ngOnInit(): void {
    this.lange = localStorage.getItem('lang');
  }

  getDictFunc() {
    this.dictHttpService.code('data_type_mysql').then((res: any) => {
      if (res.code == 200) {
        this.dataTypeList = res.data;

        if(this.dataTypeList.length>0){
          this.dataTypeList.forEach((item:any)=>{
            item.itemValue = localStorage.getItem('lang') == 'en' ?item.itemText:item.itemValue

            this.paramsObj[item.itemText] = item.itemValue
          })
        }

        this.tableData = this.tableData.map(obj => {
          const match = this.dataTypeList.find(item => item.itemText == obj.paramType);
          if (!match) {
            obj.paramType = 'varchar';
          }
          return obj;
        });
      } else {
        // this.commonHttp.dictsRefresh();
      }
    });

    this.dictHttpService.code('data_query_type').then((res: any) => {
      if (res.code == 200) {
        this.whereTypeList = res.data;
        this.whereTypeList.forEach((item:any)=>{
          this.operatorObj[item.itemText] = item.itemValue
        })
        
      } 
    });
  }
  onBlur(e: any) {
    this.headerTable.emit(this.selectedColumns);
  }
  onChangeWhereType(e: any, info: any) {
    this.headerTable.emit(this.selectedColumns);
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
  }
  onBlurStart(e: any, info: any) {
    if(info.betweenStartValue&&info.betweenStartValue <0){
      info.betweenStartValue = 0
    }
    if(info.betweenStartValue&&info.betweenEndValue&&info.betweenStartValue >info.betweenEndValue){
      info.betweenStartValue = info.betweenEndValue
    }
  }
  onBlurEnd(e: any, info: any) {
    let value = e.value;
    if(info.betweenEndValue&&info.betweenEndValue <0){
      info.betweenEndValue = 0
    }
    if(info.betweenStartValue&&info.betweenEndValue&&info.betweenStartValue >info.betweenEndValue){
      info.betweenEndValue = info.betweenStartValue
    }
    this.tableData.forEach((citem: any) => {
      if (citem.index == info.index) {
        citem.defaultValue = citem.betweenStartValue + '-' + citem.betweenEndValue;
      }
    });
  }
  onChangeDefaultDateTime(e: any, info: any) {
    let time: any = formatDate(info.betweenDate[0], 'yyyy-MM-dd hh:mm:ss', this.locale);
    this.tableData.forEach((citem: any) => {
      if (citem.index == info.index) {
        citem.defaultValue = time;
      }
    });
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
    this.tableData.forEach((citem: any) => {
      if (citem.index == info.index) {
        citem.defaultValue = startTime + 'T' + endTime;
      }
    });
  }
}
