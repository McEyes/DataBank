import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { CommonService } from 'jabil-bus-lib';
import { NzUploadChangeParam, NzUploadFile } from 'ng-zorro-antd/upload';
import { MessageService } from 'primeng/api';
import CommonHttpService from 'src/api/common';
import DictHttpService from 'src/api/common/dict';
import { TranslateData } from 'src/app/core/translate/translate-data';
import { environment } from 'src/environments/environment';



@Component({
  selector: 'app-system-dict-details',
  templateUrl: './detail.component.html',
  styleUrls: ['./detail.component.scss'],
  providers: [
    MessageService,
    CommonService
  ],
})
export class DictDetailComponent implements OnInit {
  @Output() goBack = new EventEmitter<string>();
  @Input() field: any;


  fileHost: string = environment.FileServer;
  token: string = ''

  formData: any = {
    id: '',
    dictCode: '',
    dictName: '',
    status: 1,
    status2:true,
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
    this.formData = {...this.formData, ...this.field }
    this.formData.status2 = this.formData.status == 1 ? true : false
  }

  backClick(type?: string) {
    this.goBack.emit(type)
  }


  saveClick() {

    const valid = [
      'dictCode',
      'dictName',
      'status',
    ];
    if (this.commonService.isInvalid({ ...this.formData }, valid)) {
      this.invalid = true;
    } else {
      this.loading = true;
      const httpStr = this.formData.id ? 'update' : 'save'
      this.formData.status = this.formData.status2 ? 1 : 0
      this.http[httpStr](this.formData).then((resSave: any) => {
        if (resSave.success) {
          this.field=this.formData;
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


}
