import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { MessageService, ConfirmationService } from 'primeng/api';
import { ActivatedRoute } from '@angular/router';
import WorkflowHttpService from 'src/api/home/workflow';
import HomeHttpService from 'src/api/home';
import { TranslateData } from 'src/app/core/translate/translate-data';
import { CommonService, LocalStorage, JBService } from 'jabil-bus-lib';
import RuleHttpService from 'src/api/dataQuality/rule';

@Component({
  selector: 'jabil-data-quality-rule',
  templateUrl: './rule.component.html',
  styleUrls: ['./rule.component.scss'],
  providers: [
    LocalStorage,
    MessageService,
    ConfirmationService,
    CommonService,
    WorkflowHttpService,
    JBService,
    HomeHttpService,
    RuleHttpService
  ],
})
export class DataQualityRuleComponent implements OnInit {
  @Output() closePanelreturn = new EventEmitter<string>();
  loading!: boolean;
  currentLanguage: any = 'en';
  recordCount: number = 0;
  progress: number = 0;
  isCommon: Boolean = false;
  currPanel: string = 'list';
  fromPage: string = 'list';
  submitLoading: Boolean = false;
  tableData: Array<any> = [];

  searchData: any = {
    ruleName: '',
    pageNum: 1,
    pageSize: 10,
  };
  selectedRows: Array<any> = [];
  workflowList: Array<any> = [];
  timeout: any = null;
  currRow: any = null;

  items: Array<any> = [];
  hideReturn: boolean = false;
  dialogDisplay: boolean = false;
  formData: any = { name: '', password: '' };
  isShowReturn: boolean = true;
  hasConfigAuth: boolean = false;
  workflowName: string = ''
  constructor(
    private readonly http: RuleHttpService,
    private readonly translate: TranslateService,
    private readonly confirmationService: ConfirmationService,
    private readonly messageService: MessageService,
    private readonly commonService: CommonService,
    public route: ActivatedRoute) {
    this.getData();
  }

  goBack() {
    this.getData();
  }

  public async ngOnInit() {
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
    let data = await this.http.getAllRules(this.searchData);
    this.tableData = data?.data || [];
    this.recordCount = data.total;
    this.loading = false;
  }

  search(isReset?: boolean) {
    if (isReset) {
      this.searchData = {
        ruleName: '',
        first: 0,
        pageNum: 1,
        pageSize: 10,
      };
    }
    this.getData();
  }

  editData(item?: any, act?: string) {
    this.currPanel = 'edit';
    item.title = act;
    this.currRow = item;
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

  deleteData(item?: any) {
    // this.confirmationService.confirm({
    //   message: TranslateData.commonDelete,
    //   icon: 'pi pi-exclamation-triangle',
    //   rejectLabel: TranslateData.no,
    //   acceptLabel: TranslateData.yes,
    //   accept: async () => {
    //     this.http.delete(item.id).then((res: any) => {
    //       if (res.succeeded) {
    //         this.messageService.add({
    //           severity: 'success',
    //           summary: TranslateData.save,
    //           detail: TranslateData.success,
    //         });
    //         this.getData();
    //       } else {
    //         this.messageService.add({
    //           severity: 'error',
    //           summary: TranslateData.fail,
    //           detail: res.errors,
    //         });
    //       }
    //     });
    //   },
    // });
  }

  publish(row: any) {
    // this.http.save(this.formData).then((resSave: any) => {
    //   if (resSave.succeeded) {
    //     this.messageService.add({
    //       severity: 'success',
    //       summary: TranslateData.save,
    //       detail: TranslateData.success,
    //     });
    //     this.getData()
    //   } else {
    //     this.messageService.add({
    //       severity: 'error',
    //       summary: TranslateData.fail,
    //       detail: resSave.errors,
    //     });
    //   }
    //   this.loading = false;
    // });
  }
}
