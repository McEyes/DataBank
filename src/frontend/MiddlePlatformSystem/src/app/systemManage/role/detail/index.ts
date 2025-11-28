import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { MessageService } from 'primeng/api';
import { Location } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';
import { CommonService } from 'jabil-bus-lib';
import { TranslateData } from 'src/app/core/translate/translate-data';
import UserHttpService from 'src/api/systemManage/userApi';
import CommonHttpService from 'src/api/common';
import RoleHttpService from 'src/api/systemManage/roleApi';

@Component({
  selector: 'app-data-role-details',
  templateUrl: './index.html',
  styleUrls: ['./index.scss'],
  providers: [
    MessageService,
    UserHttpService,
    CommonService,
  ],
})
export class RoleDetailsComponent implements OnInit {
  @Output() goBack = new EventEmitter<string>();
  @Input() field: any;

  queryParams: any = {
    keyword: '',
  };
  userList: any[] = [];
  formData: any = {
    id: '',
    code: '',
    name: '',
    status: 1,
  };
  isEdit: boolean = true;
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
  invalid: boolean = false;
  loading: boolean = false;
  noEdit: boolean = false;
  currLang: string = ''
  constructor(
    private messageService: MessageService,
    private location: Location,
    private http: RoleHttpService,
    private itCommonService: CommonHttpService,
    public router: Router,
    private route: ActivatedRoute,
    private commonService: CommonService
  ) {
    this.currLang = localStorage.getItem('lang') || 'en'
  }

  ngOnInit(): void {
    this.id = this.field?.id
    this.titleName = this.field?.title
    this.noEdit = this.field?.edit === '0'
    if (this.titleName == 'add') {
      this.formData = {
        id: '',
        code: '',
        name: '',
        status: 1,
      };
    } else {
      this.formData = this.field
    }
  }
  paginate(event: any) {
    this.changeParams.pageNum = event.page + 1;
    this.changeParams.pageSize = event.rows;
    this.changeParams.first = event.first;
  }
  getThis() {
    if (this.id != null && this.id != undefined) {
      this.http.getById(this.id).then((res: any) => {
        this.formData = res.data;
      });
    }
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

  saveClick() {

    const valid = [
      'code',
      'name',
    ];
    if (this.commonService.isInvalid({ ...this.formData }, valid)) {
      this.invalid = true;
    } else {
      this.loading = true;

      if (this.titleName == 'add') {
        this.http.save(this.formData).then((resSave: any) => {
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
      }else{

        this.http.update(this.formData).then((resSave: any) => {
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
      }
    }
  }
}
