import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { CommonService, JBService, LocalStorage } from 'jb.package/jabil-bus-lib';
import { ConfirmationService, MessageService } from 'primeng/api';
import CommonHttpService from 'src/api/common';
import DocHttpService from 'src/api/systemManage/docApi';
import { TranslateData } from 'src/app/core/translate/translate-data';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-doc',
  templateUrl: './doc.component.html',
  styleUrls: ['./doc.component.scss'],
    providers: [
      LocalStorage,
      MessageService,
      ConfirmationService,
      CommonService,
      JBService,
      DocHttpService
    ],
})
export class DocComponent implements OnInit {
  @Output() closePanelreturn = new EventEmitter<string>();
  loading!: boolean;
  currentLanguage: any = 'en';
  recordCount: number = 0;
  currPanel: string = 'list';
  submitLoading: boolean = false;
  tableData: Array<any> = [];

  searchData: any = {
    keyword: '',
    pageNum: 1,
    pageSize: 10,
  };

  currRow: any = null;
  formData: any = {
    name: '',
    displayNameCn: '',
    displayNameEn: '',
    extension:'',
    status: true,
    docVer: '1',
    catalog: 'document',
    sort: '99',
  };
  statusList: any[] = [
    { label: TranslateData.enable, value: true },
    { label: TranslateData.disable, value: false },
  ];
 envir: any;


  constructor(
    private readonly http: DocHttpService,
    private readonly translate: TranslateService,
    private readonly confirmationService: ConfirmationService,
    private readonly messageService: MessageService,
    private readonly commonService: CommonService,
    public readonly route: ActivatedRoute,
    private readonly commonHttp:CommonHttpService
  ) {
    this.getData();
    this.envir =environment;
  }

  goBack(type?: string) {
    this.getData();
    this.closePanel(null);
    this.closePanelreturn.emit();
  }

  public ngOnInit() {
    this.currentLanguage = localStorage.getItem('lang');
    this.translate.use(this.currentLanguage);
    this.commonService.translateData(TranslateData, this.translate);
  }

  async getData(isPaginate?: boolean) {
    this.loading = true;
    if (!isPaginate) {
      this.searchData.pageNum = 1;
      this.searchData.first = 0;
    }
    if(this.searchData.owner==null){
       delete  this.searchData.owner
    }
    if(this.searchData.sme==null){
     delete this.searchData.sme
    }
    if(this.searchData.belongArea==null){
       delete  this.searchData.belongArea
    }
    if(this.searchData.ownerDept==null){
        delete this.searchData.ownerDept
    }

    let { data } = await this.http.page(this.searchData);
    this.tableData = data?.data || [];
    this.recordCount = data.total;
    this.loading = false;
  }

  search(isReset?: boolean) {
    if (isReset) {
      this.searchData = {
        keyword: '',
        clientType:2,
        first: 0,
        pageNum: 1,
        pageSize: 10,
      };
    }
    this.getData();
  }

  editData(item?: any, act?: string) {
    this.currPanel = 'edit';
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
            this.getData();
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
    this.getData(true);
  }

  closePanel(e: any) {
    this.currPanel = 'list'
  }

    download(item: any) {
      window.open(environment.FileServer + "/api/file/download/" + item.url + "?category=Document&downName=" + item.displayNameCn, "_blank")
    }

}
