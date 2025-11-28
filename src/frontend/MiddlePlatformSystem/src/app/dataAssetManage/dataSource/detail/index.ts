import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { MessageService } from 'primeng/api';
import DataSourceHttpService from 'src/api/dataAssetManage/dataSource';
import { Location } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';
import { CommonService } from 'jabil-bus-lib';
import { TranslateData } from 'src/app/core/translate/translate-data';
import DictHttpService from 'src/api/common/dict';
import { TranslateService } from '@ngx-translate/core';
import DataClientsHttpService from 'src/api/dataAssetManage/dataClients';

@Component({
  selector: 'app-data-source-details',
  templateUrl: './index.html',
  styleUrls: ['./index.scss'],
  providers: [
    MessageService,
    DataSourceHttpService,
    CommonService,
    DictHttpService,
    DataClientsHttpService,
  ],
})
export class DataSourceDetailsComponent implements OnInit {
  @Output() goBack = new EventEmitter<string>();
  @Input() field: any;

  queryParams: any = {
    test: '',
  };
  typeList: any[] = [
    {
      id: '1255037816378994690',
      dictId: '1255037349741703169',
      itemText: 8,
      itemValue: TranslateData.otherDatabase,
    },
    {
      id: '1255037772984725506',
      dictId: '1255037349741703169',
      itemText: 7,
      itemValue: TranslateData.SQLServer,
    },
    {
      id: '1255037682886881282',
      dictId: '1255037349741703169',
      itemText: 5,
      itemValue: TranslateData.PostgreSql,
    },
    {
      id: '1255037499587407874',
      dictId: '1255037349741703169',
      itemText: 2,
      itemValue: TranslateData.MariaDB,
    },
    {
      id: '1255037454632857602',
      dictId: '1255037349741703169',
      itemText: 1,
      itemValue: TranslateData.MySql,
    },
  ];
  countries: any[];
  formData: any = {
    sourceName: '',
    dbType: '',
    status: 1,
    dbSchema: {
      dbName: '',
      host: '',
      port: '',
      username: '',
      password: '',
    },
  };
  isEdit: boolean = true;
  titleName: string = 'add';
  titleType: any = {
    add: 'Add',
    edit: 'Edit',
    look: 'Look',
  };
  id: any;
  changeTableData: any[] = [];
  changeParams: any = {
    pageSize: 5,
    changeParams: 1,
  };
  changeTableToatal: number = 0;
  statusList: any[] = [
    { label: TranslateData.deactivate, value: 0 },
    { label: TranslateData.enable, value: 1 }
  ];
  systemNameList: any[] = [];
  invalid: boolean = false;
  loading: boolean = false;
  noEdit: boolean = false;
  currLang: string = ''
  constructor(
    private messageService: MessageService,
    private sourceService: DataSourceHttpService,
    public router: Router,
    private appHttpService: DataClientsHttpService,
    private commonService: CommonService,
    private dictService: DictHttpService,
    private lang: TranslateService
  ) {
    this.currLang = localStorage.getItem('lang') || 'en'
  }

  ngOnInit(): void {
    this.id = this.field?.id
    this.titleName = this.field?.title
    this.noEdit = this.field?.edit === '0'
    if (this.titleName == 'adformInfo?.d') {
      this.formData = {
        sourceName: '',
        systemName: '',
        dbType: '1',
        status: 1,
        dbSchema: {
          dbName: '',
          host: '',
          port: '3306',
          username: '',
          password: '',
        },
      };
    }
    //获取这条记录
    this.getThis();
    this.getAppList();
  }
  onInputSourceName(e: any) {
    this.formData.dbSchema.dbName = e.target.value;
  }
  paginate(event: any) {
    this.changeParams.pageNum = event.page + 1;
    this.changeParams.pageSize = event.rows;
    this.changeParams.first = event.first;
  }
  getThis() {
    if (this.id != null && this.id != undefined) {
      this.sourceService.getById(this.id).then((res: any) => {
        this.formData = res.data;
      });
    }
  }

  backClick(type?: string) {
    this.goBack.emit(type)
  }
  pushClick() {
    const valid = [
      'sourceName',
      'dbType',
      'dbName',
      'host',
      'port',
      'username',
      'password',
    ];
    if (this.commonService.isInvalid({ ...this.formData, ...this.formData.dbSchema }, valid)) {
      this.invalid = true;
    } else {
      this.loading = true;
      this.formData.dbSchema.dbName = this.formData.dbSchema.dbName.trim();
      if (this.titleName == 'add') {
        this.sourceService.save(this.formData).then((resSave: any) => {
          if (resSave.success) {
            this.messageService.add({
              key: 'key',
              severity: 'success',
              summary: TranslateData.save,
              detail: TranslateData.success,
            });
            // this.commonRouterServiceService.addRouterFunc();
            setTimeout(() => {
              this.loading = false;
              this.backClick('update');
            }, 800);
          } else {
            this.messageService.add({
              key: 'key',
              severity: 'error',
              summary: TranslateData.fail,
              detail: resSave.msg,
            });
          }
          this.loading = false;
        });
      } else {
        this.sourceService
          .update(this.id, this.formData)
          .then((resUpdate: any) => {
            if (resUpdate.success) {
              this.messageService.add({
                key: 'key',
                severity: 'success',
                summary: TranslateData.save,
                detail: TranslateData.success,
              });
              // this.commonRouterServiceService.addRouterFunc();
              setTimeout(() => {
                this.loading = false;
                this.backClick('update');
              }, 800);
            } else {
              this.messageService.add({
                key: 'key',
                severity: 'error',
                summary: TranslateData.fail,
                detail: resUpdate.msg,
              });
            }
            this.loading = false
          })
          .catch((error: any) => {
            this.messageService.add({
              key: 'key',
              severity: 'error',
              summary: TranslateData.fail,
              detail: error.msg,
            });

            this.loading = false
          });
      }
    }
  }
  testClick() {
    this.sourceService.checkConnection(this.formData).then((result: any) => {
      if (result.code == 200) {
        this.messageService.add({
          key: 'key',
          severity: 'success',
          summary: TranslateData.success,
          detail: result.msg,
        });
      } else {
        this.messageService.add({
          key: 'key',
          severity: 'error',
          summary: TranslateData.fail,
          detail: result.msg,
        });
      }
    });
  }
  async getDictList() {
    let res = await this.dictService.codes('source_system_name');
    if (res.success) {
      this.systemNameList = res.data.filter((f: any) => f.dictCode == 'source_system_name');
      this.systemNameList.forEach((f: any) => {
        this.lang.get("dict." + f.dictCode + "." + f.itemValue).subscribe((res: string) => {
          if (res != "dict." + f.dictCode + "." + f.itemValue) f.itemText = res;
          else if (this.currLang == 'en') f.itemText = f.itemValue;
        });
      });
    }
  }
  async getAppList() {
    let res = await this.appHttpService.AllAppList();
    if (res.success) {
      this.systemNameList = res.data.map((f: any) => ({
        clientName: f.clientName+'('+f.nickName.toUpperCase()+')',
        nickName: f.nickName.toUpperCase(),
      }));
    }
  }
}
