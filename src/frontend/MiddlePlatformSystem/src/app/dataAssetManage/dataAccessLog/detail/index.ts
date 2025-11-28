import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Location } from '@angular/common';
import DataAccessLogHttpService from 'src/api/dataAssetManage/dataAccessLog';
import DataApiHttpService from 'src/api/dataAssetManage/api';
import { MessageService, ConfirmationService } from 'primeng/api';
import { Router, ActivatedRoute } from '@angular/router';
import { CommonService } from 'jabil-bus-lib';
import { TranslateData } from 'src/app/core/translate/translate-data';

@Component({
  selector: 'app-data-access-log-details',
  templateUrl: './index.html',
  styleUrls: ['./index.scss'],
  providers: [MessageService, CommonService, ConfirmationService, DataAccessLogHttpService, DataApiHttpService],
})
export class DataAccesssLogDetailComponent implements OnInit {
  @Output() goBack = new EventEmitter<string>();
  @Input() field: any;

  titleName: string = '';
  titleType: any = {
    add: 'Add',
    edit: 'Edit',
    look: 'Look',
  };
  formData: any = {
    apiId: null,
    callerIp: null,
    callerUrl: null,
    callerParams: null,
    callerDate: null,
    time: null,
    status: 1,
    msg: null,
  };
  invalid: boolean = false;
  apiList: any[] = [];
  selectedCategory: any = null;

  constructor(
    private apiLogsHttpService: DataAccessLogHttpService,
    private dataApiHttpService: DataApiHttpService,
    public router: Router,
    private route: ActivatedRoute,
  ) {

  }

  ngOnInit(): void {

    this.titleName = this.field.title;
    let id = this.field.id;
    if (this.titleName != 'add' && id) {
      this.apiLogsHttpService.getById(id).then((res: any) => {
        this.formData = res.data;
      });
    }

    this.dataApiHttpService.list().then((res: any) => {
      if (Array.isArray(res.data)) {
        this.apiList = [];
        res.data.forEach((element: any) =>
          this.apiList.push({ id: element.id, apiName: element.apiName })
        );
      }
    });
  }

  backClick() {
    this.goBack.emit()
  }
}
