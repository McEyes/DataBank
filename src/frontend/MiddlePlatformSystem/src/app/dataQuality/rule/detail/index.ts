import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { MessageService } from 'primeng/api';
import { Location } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';
import { CommonService } from 'jabil-bus-lib';
import { TranslateData } from 'src/app/core/translate/translate-data';
import RuleMyDataHttpService from 'src/api/dataQuality/myData';
import CommonHttpService from 'src/api/common';
import RuleHttpService from 'src/api/dataQuality/rule';

@Component({
  selector: 'app-data-quality-rule-details',
  templateUrl: './index.html',
  styleUrls: ['./index.scss'],
  providers: [
    MessageService,
    CommonService,
    RuleHttpService,
    RuleMyDataHttpService
  ],
})
export class RuleDetailsComponent implements OnInit {
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

  typeList: any[] = []
  levelList: any[] = []
  constructor(
    private messageService: MessageService,
    private http: RuleHttpService,
    private itCommonService: CommonHttpService,
    public router: Router,
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
    // if (this.id != null && this.id != undefined) {
    //   this.http.getById(this.id).then((res: any) => {
    //     this.formData = res.data;
    //   });
    // }
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
        // arr.forEach((item: any) => {
        //   item.employeeName = `${item.englishName}(${item.userId})`;
        // });
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
  }
}
