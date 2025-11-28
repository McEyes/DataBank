import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { MessageService, ConfirmationService } from 'primeng/api';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslateData } from 'src/app/core/translate/translate-data';
import { CommonService, LocalStorage, JBService } from 'jabil-bus-lib';
import ApplicationHttpService from 'src/api/systemManage/applicationApi';
import CommonHttpService from 'src/api/common';
import DictHttpService from 'src/api/common/dict';

@Component({
  selector: 'jabil-system-application',
  templateUrl: './application.component.html',
  styleUrls: ['./application.component.scss'],
  providers: [
    LocalStorage,
    MessageService,
    ConfirmationService,
    CommonService,
    JBService,
    ApplicationHttpService,
    CommonHttpService,
    DictHttpService,
  ],
})
export class SystemApplicationComponent implements OnInit {
  @Output() closePanelreturn = new EventEmitter<string>();
  loading!: boolean;
  currentLanguage: any = 'en';
  recordCount: number = 0;
  currPanel: string = 'list';
  submitLoading: boolean = false;
  tableData: Array<any> = [];
  useTypeList: any[] = []

  searchData: any = {
    keyword: '',
    useType: '',
    belongArea:'',
    pageNum: 1,
    pageSize: 10,
    clientType: '2',
  };

  currRow: any = null;
  formData: any = {
    id: '',
    clientId: '',
    clientName: '',
    nickName: '',
    smeList: '',
    ownerName: '',
    ownerDept: '',
    belongArea: '',
    masterDataTypes: '',
    clientUrl: '',
    description: ''
  };
  statusList: any[] = [
    { label: TranslateData.enable, value: true },
    { label: TranslateData.disable, value: false },
  ];
  userList: any[] = []
  deptList: any[] = []

  typeList: any[] = [
    { name: 'Global', id: 'global' },
    { name: 'Regional', id: 'regional' },
    { name: 'Site', id: 'site' },
  ]

  constructor(
    private readonly http: ApplicationHttpService,
    private readonly translate: TranslateService,
    private readonly confirmationService: ConfirmationService,
    private readonly messageService: MessageService,
    private readonly commonService: CommonService,
    public readonly route: ActivatedRoute,
    private readonly commonHttp: CommonHttpService,
    private readonly dictService: DictHttpService,
    private lang: TranslateService
  ) {
    this.currentLanguage = localStorage.getItem('lang') || 'en'
    this.getData();
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
    this.getUserList();
    this.getDeptList();
    this.getDictList();

  }

  async getData(isPaginate?: boolean) {
    this.loading = true;
    if (!isPaginate) {
      this.searchData.pageNum = 1;
      this.searchData.first = 0;
    }
    if (this.searchData.owner == null) {
      delete this.searchData.owner
    }
    if (this.searchData.sme == null) {
      delete this.searchData.sme
    }
    if (this.searchData.belongArea == null) {
      delete this.searchData.belongArea
    }
    if (this.searchData.ownerDept == null) {
      delete this.searchData.ownerDept
    }
    if (this.searchData.useType == null) {
      delete this.searchData.useType
    }
    if (this.searchData.belongArea == null) {
      delete this.searchData.belongArea
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
        clientType: 2,
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

  getSMEName(list: any) {
    let res = ''
    list?.forEach((item: any) => {
      res += item.userName + ' '
    })

    return res
  }
  // 用户列表d
  getUserList() {
    if (sessionStorage.getItem('userList')) {
      let userListString: any = sessionStorage.getItem('userList');
      let userList = JSON.parse(userListString);
      this.userList = userList;
    } else {
      this.loading = true;
      this.commonHttp.getUserList().then((res: any) => {
        let arr = res.data;
        const uniqueArr = Array.from(
          new Map(arr.map((item: any) => [item['id'], item])).values()
        );
        this.userList = uniqueArr;
        sessionStorage.setItem('userList', JSON.stringify(this.userList));
        this.loading = false;
      });
    }
  }
  async getDeptList() {
    const res = await this.commonHttp.getDepartments()
    this.deptList = res.data.map((item: string) => { return { name: item, id: item } });
  }
  async getDictList() {
    let res = await this.dictService.codes('DataUseType');
    if (res.success) {
      this.useTypeList = res.data.filter((f: any) => f.dictCode == 'DataUseType');
      this.useTypeList.forEach((f: any) => {
        this.lang.get("dict." + f.dictCode + "." + f.itemValue).subscribe((res: string) => {
          if (res != "dict." + f.dictCode + "." + f.itemValue) f.itemText = res;
          else if (this.currentLanguage == 'en') f.itemText = f.itemValue;
        });
      });
    }
  }
}
