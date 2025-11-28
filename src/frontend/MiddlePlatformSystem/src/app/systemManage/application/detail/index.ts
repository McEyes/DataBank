import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { MessageService } from 'primeng/api';
import { CommonService } from 'jabil-bus-lib';
import { TranslateData } from 'src/app/core/translate/translate-data';
import UserHttpService from 'src/api/systemManage/userApi';
import CommonHttpService from 'src/api/common';
import RoleHttpService from 'src/api/systemManage/roleApi';
import ApplicationHttpService from 'src/api/systemManage/applicationApi';
import DictHttpService from 'src/api/common/dict';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-system-application-details',
  templateUrl: './index.html',
  styleUrls: ['./index.scss'],
  providers: [
    MessageService,
    UserHttpService,
    CommonService,
    ApplicationHttpService,
    DictHttpService,
  ],
})
export class ApplicationDetailsComponent implements OnInit {
  @Output() goBack = new EventEmitter<string>();
  @Input() field: any;

  userList: any[] = [];
  formData: any = {
    id: '',
    clientId: '',
    clientType: 2,
    clientName: '',
    clientNameEn: '',
    nickName: '',

    smeList: null,
    smeIdList: null,
    ownerId: '',
    ownerNtid: '',
    ownerName: '',
    ownerDept: '',
    belongArea: '',
    masterDataTypes: null,
    clientUrl: '',
    description: '',
    useType: 'DataAnalysis',
    enabled: true,
  };

  titleName: string = 'add';
  titleType: any = {
    add: 'Add',
    edit: 'Edit',
    look: 'Look',
  };
  id: any;
  changeParams: any = {
    pageSize: 5,
    changeParams: 1,
  };
  statusList: any[] = [
    { label: TranslateData.disable, value: 0 },
    { label: TranslateData.enable, value: 1 }
  ];
  typeList: any[] = [
    { name: 'Global', id: 'global' },
    { name: 'Regional', id: 'regional' },
    { name: 'Site', id: 'site' },
  ]
  masterDataTypeList: any[] = []
  useTypeList: any[] = []
  invalid: boolean = false;
  loading: boolean = false;
  noEdit: boolean = false;
  currLang: string = ''
  constructor(
    private readonly messageService: MessageService,
    private readonly http: ApplicationHttpService,
    private readonly itCommonService: CommonHttpService,
    private readonly commonService: CommonService,
    private readonly dictService: DictHttpService,
    private lang: TranslateService
  ) {
    this.currLang = localStorage.getItem('lang') || 'en'
  }

  ngOnInit(): void {
    this.getMasterTypeList();
    this.getUserList();
    this.getDictList()
    this.initData()
  }

  paginate(event: any) {
    this.changeParams.pageNum = event.page + 1;
    this.changeParams.pageSize = event.rows;
    this.changeParams.first = event.first;
  }

  initData() {
    this.formData = {
      id: this.field?.id ?? undefined,
      clientId: this.field?.clientId ?? undefined,
      clientType: 2,
      clientName: this.field?.clientName ?? '',
      clientNameEn: this.field?.clientNameEn ?? this.field?.clientName ?? '',
      nickName: this.field?.nickName ?? '',

      smeList: this.field?.smeList ?? null,
      smeIdList: null,
      owner: this.field?.owner ?? null,
      ownerNtid: this.field?.ownerNtid ?? '',
      ownerName: this.field?.ownerName ?? '',
      ownerDept: this.field?.ownerDept ?? '',
      belongArea: this.field?.belongArea ?? '',
      masterDataTypes: this.field?.masterDataTypes ?? null,
      clientUrl: this.field?.clientUrl ?? '',
      description: this.field?.description ?? '',
      enabled: this.field ? this.field.enabled : true,
      useType: this.field ? (this.field.useType ?? 'DataAnalysis') : 'DataAnalysis',
    }

    if (this.field) {
      this.formData.smeIdList = []
      this.formData.smeList.forEach((ele: any) => {
        this.formData.smeIdList.push(ele.userId)
      });

    }

  }

  async getMasterTypeList() {
    const res = await this.http.getMasterType()
    this.masterDataTypeList = res.data.data
  }

  // 用户列表
  getUserList() {
    if (sessionStorage.getItem('userList')) {
      let userListString: any = sessionStorage.getItem('userList');
      let userList = JSON.parse(userListString);
      this.userList = userList;
    } else {
      this.loading = true;
      this.itCommonService.getUserList().then((res: any) => {
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

  backClick(type?: string) {
    this.goBack.emit(type)
  }

  buildSmeData() {
    this.formData.smeList = []
    this.userList.forEach((item: any) => {
      if (this.formData.smeIdList.includes(item.id)) {
        this.formData.smeList.push({
          userId: item.id,
          userName: item.name
        })
      }
    })
  }

  saveClick() {

    const valid = [
      'clientName',
      'nickName',
      'belongArea',
      'owner',
    ];
    if (this.commonService.isInvalid({ ...this.formData }, valid)) {
      this.invalid = true;
    } else {
      this.buildSmeData()

      this.loading = true;
      const httpStr = this.formData.id ? 'update' : 'save'
      this.http[httpStr](this.formData).then((resSave: any) => {
        if (resSave.success) {
          this.messageService.add({
            key: 'key',
            severity: 'success',
            summary: TranslateData.save,
            detail: TranslateData.success,
          });
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
    }
  }

  changeBSA() {
    const obj = this.userList.find((item: any) => {
      return this.formData.owner === item.id
    })

    this.formData.ownerName = obj.name
    this.formData.ownerDept = obj.department
    this.formData.ownerNtid = obj.id
  }
  async getDictList() {
    let res = await this.dictService.codes('DataUseType');
    if (res.success) {
      this.useTypeList = res.data.filter((f: any) => f.dictCode == 'DataUseType');
      this.useTypeList.forEach((f: any) => {
        if (f.itemText && f.itemTextEn) {
         this.lang.get("dict." + f.dictCode + "." + f.itemValue).subscribe((res: string) => {
            if (res != "dict." + f.dictCode + "." + f.itemValue) f.itemText = res;
            else if (this.currLang == 'en') f.itemText = f.itemTextEn;
          });
        } else {
          this.lang.get("dict." + f.dictCode + "." + f.itemValue).subscribe((res: string) => {
            if (res != "dict." + f.dictCode + "." + f.itemValue) f.itemText = res;
            else if (this.currLang == 'en') f.itemText = f.itemValue;
          });
        }
      });
    }
  }
}
