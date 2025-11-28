import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { CommonService } from 'jabil-bus-lib';
import { NzUploadChangeParam, NzUploadFile } from 'ng-zorro-antd/upload';
import { MessageService } from 'primeng/api';
import CommonHttpService from 'src/api/common';
import DictHttpService from 'src/api/common/dict';
import { TranslateData } from 'src/app/core/translate/translate-data';
import { environment } from 'src/environments/environment';



@Component({
  selector: 'app-system-dict-item-details',
  templateUrl: './detail.component.html',
  styleUrls: ['./detail.component.scss'],
  providers: [
    MessageService,
    CommonService
  ],
})
export class DictItemDetailComponent implements OnInit {
  @Output() goBack = new EventEmitter<string>();
  @Input() field: any;
  @Input() dictList: any;

  currPanel: string = 'edit';
  fileHost: string = environment.FileServer;
  token: string = ''
  currentDictItem: any = { status: 1, status2: true };

  formData: any = {
    id: '',
    dictId: '',
    dictCode: '',
    dictName: '',
    itemText: '',
    itemValue: '',
    itemTextEn: '',
    itemData: '',
    status: 1,
    status2: true,
    remark: '',
    itemSort: '99',
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

  masterDataTypeList: any[] = []
  invalid: boolean = false;
  loading: boolean = false;
  noEdit: boolean = false;
  currLang: string = ''
  constructor(
    private readonly messageService: MessageService,
    private readonly http: DictHttpService,
    private readonly itCommonService: CommonHttpService,
    private readonly commonService: CommonService,
  ) {
    this.currLang = localStorage.getItem('lang') || 'en'
    this.token = localStorage.getItem('jwt') || ''
  }

  ngOnInit(): void {
    this.initData()
  }

  paginate(event: any) {
    this.changeParams.pageNum = event.page + 1;
    this.changeParams.pageSize = event.rows;
    this.changeParams.first = event.first;
  }

  initData() {
    this.formData = { ...this.formData, ...this.field }
    this.formData.status2 = this.formData.status == 1 ? true : false
  }

  backClick(type?: string) {
    this.goBack.emit(type)
  }


  saveClick() {

    const valid = [
      'itemText',
      'itemValue',
      'itemId',
      'status',
    ];
    if (this.commonService.isInvalid({ ...this.formData }, valid)) {
      this.invalid = true;
    } else {
      this.loading = true;
      this.formData.status = this.formData.status2 ? 1 : 0
      const httpStr = this.formData.id ? 'itemUpdate' : 'saveItem'
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


  goBackDict(type?: string) {
    this.getData();
    this.currPanel = 'edit';
  }

  async getData(isPaginate?: boolean) {
    let { data } = await this.http.page({ pageNum: 1, pageSize: 1000, status: 1 });
    this.dictList = data?.data || [];
  }

  async dictChange(event: any) {
    let dict = this.dictList.find((item: any) => item.id == event.value)
    this.formData.dictCode = dict?.dictCode || ''
    this.formData.dictName = dict?.dictName || ''
  }
}
