import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { MessageService } from 'primeng/api';
import { Router, ActivatedRoute } from '@angular/router';
import { CommonService } from 'jabil-bus-lib';
import TopicHttpService from 'src/api/dataAssetManage/topic';
import { TranslateData } from 'src/app/core/translate/translate-data';

@Component({
  selector: 'app-topicdomaindefinition-detail',
  templateUrl: './index.html',
  styleUrls: ['./index.scss'],
  providers: [MessageService, TopicHttpService],
})
export class TopicDomainDefinitionDetailsComponent implements OnInit {
  @Output() goBack = new EventEmitter<string>();
  @Input() field: any;

  queryParams: any = {
    pageSize: 10,
    pageIndex: 1,
  };
  formData: any = {
    name: '',
    code: '',
    status: 1,
    remark: '',
  };
  parentCtlIdInfo: any;
  operate: string = 'add';
  total: number = 0;
  topicTree: any;
  invalid: boolean = false;
  loading: boolean = false;
  noEdit: boolean = false;
  statusList: any[] = [
    { label: TranslateData.deactivate, value: 0 },
    { label: TranslateData.enable, value: 1 },
  ];

  constructor(
    public router: Router,
    private route: ActivatedRoute,
    private messageService: MessageService,
    private commonService: CommonService,
    private topicService: TopicHttpService
  ) {
    // this.route.queryParams.subscribe(params => {
    //   this.operate = params['title'];
    //   this.id = params['id'];
    //   //获取这条记录
    //   this.getTopic();
    // });
  }

  ngOnInit(): void {
    this.operate = this.field?.ctlId ? 'edit' : 'add';
    this.noEdit = this.field?.edit === '0'
    //获取这条记录
    this.getTopic();
  }

  getTopic() {
    this.getTree();
    
    if (this.field?.ctlId) {
      this.topicService.getById(this.field?.ctlId).then((res: any) => {
        this.formData = res.data;
      });
    }
  }

  findObjectById(arr: any[], id: string) {
    const result = arr.find(obj => obj.key === id);
    if (result) {
      return result;
    }
    for (let obj of arr) {
      if (obj.children && obj.children.length > 0) {
        const nestedResult: any = this.findObjectById(obj.children, id);
        if (nestedResult) {
          return nestedResult;
        }
      }
    }
  }

  async getTree() {
    let data = await this.topicService.getTree('topic');
    this.topicTree = data.data;
    if (this.operate != 'add') {
      this.parentCtlIdInfo = this.findObjectById(this.topicTree, this.formData.parentCtlId);
    }

    console.log(this.topicTree)
  }

  onPageChange(event: any) {
    this.queryParams.pageIndex = event.page + 1;
    this.queryParams.pageSize = event.rows;
  }

  goback(type?: string) {
    this.goBack.emit(type)
  }

  onSumit() {
    this.formData.parentCtlId = this.parentCtlIdInfo?.key || '';
    const valid = ['name', 'code', 'remark'];
    if (this.commonService.isInvalid(this.formData, valid)) {
      this.invalid = true;
    } else {
      this.loading = true;
      const httpStr = this.operate == 'add' ? 'save' : 'update'

      this.topicService[httpStr](this.field?.ctlId, this.formData).then((resSave: any) => {
        if (resSave.code == 200) {
          this.messageService.add({
            severity: 'success',
            summary: TranslateData.save,
            detail: TranslateData.success,
          });
          setTimeout(() => {
            this.loading = false;
            this.goback('update');
          }, 800);
        } else {
          this.loading = false;
          this.messageService.add({
            severity: 'error',
            summary: TranslateData.fail,
            detail: resSave.msg,
          });
        }
      });
    }
  }
}
