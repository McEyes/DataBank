import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { stat } from 'fs';
import { CommonService, JBService, LocalStorage } from 'jb.package/jabil-bus-lib';
import { ConfirmationService, MessageService } from 'primeng/api';
import CommonHttpService from 'src/api/common';
import DictHttpService from 'src/api/common/dict';
import { TranslateData } from 'src/app/core/translate/translate-data';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-dict',
  templateUrl: './dict.component.html',
  styleUrls: ['./dict.component.scss'],
  providers: [
    LocalStorage,
    MessageService,
    ConfirmationService,
    CommonService,
    JBService,
    DictHttpService
  ],
})
export class DictComponent implements OnInit {
  @Output() closePanelreturn = new EventEmitter<string>();
  loading!: boolean;
  currentLanguage: any = 'en';
  recordCount: number = 0;
  currPanel: string = 'list';
  submitLoading: boolean = false;
  tableData: Array<any> = [];
  dictList: Array<any> = [];
  selectedFile: any;
  treeLoaing: boolean = false;

  searchData: any = {
    keyword: '',
    dictId: '',
    pageNum: 1,
    pageSize: 10,
  };

  currRow: any = {status: 1,status2:true};
  formData: any = {
    id: '',
    dictCode: '',
    dictName: '',
    itemText: '',
    itemValue: '',
    itemData: '',
    status: 1,
    status2:true,
    remark: '',
    sort: '99',
  };
  statusList: any[] = [
    { label: TranslateData.enable, value: 1 },
    { label: TranslateData.disable, value: 0 },
  ];
  // dictList: any[] = [
  //   { label: TranslateData.enable, value: true },
  //   { label: TranslateData.disable, value: false },
  // ];
  envir: any;


  constructor(
    private readonly http: DictHttpService,
    private readonly translate: TranslateService,
    private readonly confirmationService: ConfirmationService,
    private readonly messageService: MessageService,
    private readonly commonService: CommonService,
    public readonly route: ActivatedRoute,
    private readonly commonHttp: CommonHttpService
  ) {
    this.getData();
    this.getItemsData();
    this.envir = environment;
  }

  goBack(type?: string) {
    this.getData();
    this.getItemsData();
    this.closePanel(null);
    this.closePanelreturn.emit();
  }

  public ngOnInit() {
    this.currentLanguage = localStorage.getItem('lang');
    this.translate.use(this.currentLanguage);
    this.commonService.translateData(TranslateData, this.translate);
  }

  async getData(isPaginate?: boolean) {
    this.treeLoaing = true;
    let { data } = await this.http.page({pageNum:1,pageSize:1000,status:1});
    this.dictList = data?.data || [];
    this.recordCount = data.total;
    this.treeLoaing = false;
  }

  async getItemsData(isPaginate?: boolean) {
    this.loading = true;
    if (!isPaginate) {
      this.searchData.pageNum = 1;
      this.searchData.first = 0;
    }
    if (this.searchData.dictId == null) {
      delete this.searchData.dictId
    }

    let { data } = await this.http.itemPage(this.searchData);
    this.tableData = data?.data || [];
    this.recordCount = data.total;
    this.loading = false;
  }

  search(isReset?: boolean) {
    if (isReset) {
      this.searchData = {
        keyword: '',
        dictId: '',
        first: 0,
        pageNum: 1,
        pageSize: 10,
      };
    }
    this.getItemsData();
  }

  editData(item?: any, act?: string) {
    if(!this.formData.dictId&&this.selectedFile){
      this.formData.dictId = this.selectedFile.id;
      this.formData.dictCode  = this.selectedFile.dictCode;
    }
    this.currPanel = act == 'additem' ? 'edititem' : 'edit';
    item = item || null;
    this.currRow = item;
  }

  deleteData(item?: any) {
    this.confirmationService.confirm({
      message: this.currentLanguage == 'en'
        ? `Are you sure you want to delete ${item.clientName} user data?`
        : `确定删除${item.clientName}用户数据？`,
      rejectLabel: localStorage.getItem('lang') == 'en' ? 'No' : '否',
      acceptLabel: localStorage.getItem('lang') == 'en' ? 'Yes' : '是',
      accept: async () => {
        this.http.delete(item.id).then((res: any) => {
          if (res.success) {
            this.messageService.add({
              key: 'key',
              severity: 'success',
              summary: TranslateData.save,
              detail: TranslateData.success,
            });
            this.getItemsData();
          } else {
            this.messageService.add({
              key: 'key',
              severity: 'error',
              summary: TranslateData.fail,
              detail: res.msg,
            });
          }
        });
      },
    });
  }
  deleteItemData(item?: any) {
    this.confirmationService.confirm({
      message: this.currentLanguage == 'en'
        ? `Are you sure you want to delete ${item.clientName} user data?`
        : `确定删除${item.clientName}用户数据？`,
      rejectLabel: localStorage.getItem('lang') == 'en' ? 'No' : '否',
      acceptLabel: localStorage.getItem('lang') == 'en' ? 'Yes' : '是',
      accept: async () => {
        this.http.itemDelete(item.id).then((res: any) => {
          if (res.success) {
            this.messageService.add({
              key: 'key',
              severity: 'success',
              summary: TranslateData.save,
              detail: TranslateData.success,
            });
            this.getItemsData();
          } else {
            this.messageService.add({
              key: 'key',
              severity: 'error',
              summary: TranslateData.fail,
              detail: res.msg,
            });
          }
        });
      },
    });
  }

  paginate(event: any) {
    this.searchData.pageNum = event.page + 1;
    this.searchData.pageSize = event.rows;
    this.searchData.first = event.first;
    this.getItemsData(true);
  }

  closePanel(e: any) {
    this.currPanel = 'list'
  }

  download(item: any) {
    window.open(environment.FileServer + "/api/file/download/" + item.url + "?category=Document&downName=" + item.displayNameCn, "_blank")
  }

  nodeSelect(event: any) {
    this.searchData.dictId = event.node.id;
    this.searchData.dictCode = event.node.dictCode;
    // this.searchData.name = ''
    // this.searchData.keyword = '';
    // console.log(this.selectedFile)
    this.search();
  }

}
