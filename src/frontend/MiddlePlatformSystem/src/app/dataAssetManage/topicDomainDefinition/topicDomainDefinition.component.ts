import { Component, OnInit } from '@angular/core';
import { MessageService, ConfirmationService } from 'primeng/api';
import { ActivatedRoute, Router } from '@angular/router';
import { en_US, NzI18nService, zh_CN } from 'ng-zorro-antd/i18n';
import { TranslateData } from 'src/app/core/translate/translate-data'
import TopicHttpService from 'src/api/dataAssetManage/topic';
import DataTableHttpService from 'src/api/dataAssetManage/dataTable';

@Component({
  selector: 'app-topic-domain-definition',
  templateUrl: './topicDomainDefinition.component.html',
  styleUrls: ['./topicDomainDefinition.component.scss'],
  providers: [
    MessageService,
    ConfirmationService,
    TopicHttpService,
    DataTableHttpService,
  ],
})
export class TopicDomainDefinitionComponent implements OnInit {
  queryParams: any = {
    pageSize: 20,
    pageNum: 1,
  };
  total: number = 0;
  first: number = 0;
  loading: boolean = false;
  dataLoading: boolean = false;
  formData: any = {
    test: '',
    invalid: false,
  };
  dropdownList: Array<any> = [
    { name: TranslateData.deactivate, key: '0' },
    { name: TranslateData.enable, key: '1' },
  ];
  dropdownInfo: any = {
    0: TranslateData.deactivate,
    1: TranslateData.enable,
  };
  dataDialog: boolean = false;
  dataTableSelects: any = [];
  saveDataTableSelects: any = [];
  checkDataTableSelects: any = [];
  dataTableData: any[] = [];
  langType: string = 'en';
  tableParams: any = {
    pageSize: 20,
    pageNum: 1,
  };
  saveTableParams: any = {
    tableName: '',
  };
  tableTotal: number = 0;
  tablefirst: number = 0;
  params: any = {}
  selectValueName: string = '';
  tableData: any[] = [];
  currPanel: string = 'list'
  currentRow: any = {}
  constructor(
    public router: Router,
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
    private topicService: TopicHttpService,
    private tableService: DataTableHttpService,
    private i18n: NzI18nService,
    public route: ActivatedRoute,
  ) {
    window.addEventListener('resetPage', (e: any) => {
      this.currPanel = 'list'
    });
  }

  ngOnInit(): void {
    this.langType = localStorage.getItem('lang') || 'en';
    this.i18n.setLocale(localStorage.getItem('lang') === 'en' ? en_US : zh_CN);

    this.getList();

    this.route.queryParams.subscribe((res: any) => {
      if (res.type === 'link') {
        this.params = { title: 'edit', ctlId: res.id, edit: res.edit }
        this.currPanel = 'detail'
      }
    })
  }

  ngOnDestroy(): void {
    // this.allClose();
  }

  nzPageIndexChange(index: number) {
    this.queryParams.pageNum = index;
    this.getList();
  }

  nzPageSizeChange(page: number) {
    this.queryParams.pageSize = page;
    this.getList();
  }

  onQuyery() {
    this.queryParams.pageSize = 20;
    this.queryParams.pageNum = 1;
    this.first = 0;
    this.getList();
  }

  getList() {
    this.loading = true;
    this.topicService.page(this.queryParams).then((result: any) => {
      if (result.code == 200) {
        this.tableData = result.data.data;
        this.total = result.data.total;
      } else {
        this.messageService.add({
          severity: 'error',
          summary: TranslateData.fail,
          detail: result.msg,
        });
      }
      this.loading = false;
    });
  }

  onPageChange(event: any) {
    this.first = event.first;
    this.queryParams.pageNum = event.page + 1;
    this.queryParams.pageSize = event.rows;
    this.getList();
  }

  ontableChange(event: any) {
    this.tablefirst = event.first;
    this.tableParams.pageNum = event.page + 1;
    this.tableParams.pageSize = event.rows;
    this.getTableAll();
  }

  openDialog(item: any) {
    this.currentRow = item;
    this.dataDialog = true;
    //需要查询所有表（分页）dataTableData
    this.getTableAll();
    //需要查询： 当前主题域关联的表 dataTableSelects
    this.getThisTable(item.ctlId);
  }

  onQuyeryTableAll() {
    this.tablefirst = 0;
    this.tableParams = {
      pageSize: 20,
      pageNum: 1,
    };
    this.tableParams.tableName = this.saveTableParams.tableName;
    this.getTableAll();
  }

  getTableAll() {
    this.tableService.page(this.tableParams).then((allRes: any) => {
      this.dataTableData = allRes.data.data;
      this.tableTotal = allRes.data.total;
    });
  }

  getThisTable(ctlId: string) {
    this.dataLoading = true;
    this.tableService.getTablesByTopic(ctlId).then((result: any) => {
      if (result.code == 200) {
        this.dataTableSelects = result.data;
        this.checkDataTableSelects = [...this.dataTableSelects];
        this.saveDataTableSelects = [...this.dataTableSelects];
        this.dataLoading = false;
      }
    });
  }

  onSelectionChange(arr: any) {
    this.checkDataTableSelects = [...arr];
    this.saveDataTableSelects = [...arr];
  }

  onDeleteAssociated(e: any, row: any) {
    this.dataTableSelects = this.saveDataTableSelects.filter((item: any) => item.id != row.id);
    this.saveDataTableSelects = [...this.dataTableSelects];
    this.onBlurSelectList()
  }

  onBlurSelectList() {
    this.checkDataTableSelects = [...this.saveDataTableSelects];
    let name = this.selectValueName;
    if (name) {
      let filteredArr = this.checkDataTableSelects.filter((item: any) =>
        item.tableName.toLowerCase().includes(name.toLowerCase())
      );
      this.checkDataTableSelects = filteredArr;
    } else {
      this.checkDataTableSelects = [...this.saveDataTableSelects];
    }
  }

  onResetSelectList() {
    this.selectValueName = '';
    this.checkDataTableSelects = [...this.saveDataTableSelects];
  }

  onDataDialogSumit() {
    let param = {
      ctlId: this.currentRow.ctlId,
      metadataTableDtoList: this.dataTableSelects,
    };

    if (this.dataTableSelects == null || this.dataTableSelects.length == 0) {
      this.confirmationService.confirm({
        message: TranslateData.clearTopicDomainsRelationship,
        header: TranslateData.confirm,
        icon: 'pi pi-exclamation-triangle',
        key: 'positionDialog',
        rejectLabel: TranslateData.no,
        acceptLabel: TranslateData.yes,
        accept: () => {
          //选择为空，则删除主题域对应的表的关系
          this.topicService.saveMapping(param).then((resDel: any) => {
            if (resDel.code == 200) {
              this.messageService.add({
                severity: 'success',
                summary: TranslateData.clear,
                detail: TranslateData.success,
              });
              this.allClose();
            } else {
              this.messageService.add({
                severity: 'error',
                summary: TranslateData.fail,
                detail: resDel.msg,
              });
            }
          });
        },
      });
    } else {
      this.topicService.saveMapping(param).then((resDel: any) => {
        if (resDel.code == 200) {
          this.messageService.add({
            severity: 'success',
            summary: TranslateData.save,
            detail: TranslateData.success,
          });
          this.allClose();
        } else {
          this.messageService.add({
            severity: 'error',
            summary: TranslateData.fail,
            detail: resDel.msg,
          });
        }
      });
    }
  }

  allClose() {
    this.saveTableParams.tableName = '';
    this.selectValueName = '';
    this.formData.invalid = false;
    this.dataDialog = false;
    this.onQuyeryTableAll();
  }

  goDetail(type: string, info: any = null) {
    this.currPanel = 'detail'
    this.params = info
  }

  ondelete(event: any, info?: any) {
    this.confirmationService.confirm({
      target: event.target,
      message: TranslateData.deleteTopicDomains,
      icon: 'pi pi-exclamation-triangle',
      rejectLabel: TranslateData.no,
      acceptLabel: TranslateData.yes,
      accept: () => {
        this.topicService.delete(info.ctlId).then((resDel: any) => {
          if (resDel.code == 200) {
            this.messageService.add({
              severity: 'success',
              summary: TranslateData.delete,
              detail: TranslateData.success,
            });
            //查询页面
            this.getList();
          } else {
            this.messageService.add({
              severity: 'error',
              summary: TranslateData.fail,
              detail: resDel.msg,
            });
          }
        });
      },
      reject: () => {
        this.messageService.add({
          severity: 'info',
          summary: TranslateData.delete,
          detail: TranslateData.cancel,
        });
      },
    });
  }

  onReset() {
    this.queryParams = {
      pageSize: 20,
      pageNum: 1,
    };
    this.first = 0;
    this.getList();
  }

  onResetDialog() {
    this.saveTableParams.tableName = '';
    this.tableParams.tableName = '';
    this.saveTableParams.pageNum = 1;
    this.tableParams.pageSize = 20;
    this.getTableAll();
  }

  goBack(e: string) {
    if (e === 'update') {
    this.getList();
    }
    this.currPanel = 'list'
  }


}
