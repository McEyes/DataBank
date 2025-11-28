import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { MessageService } from 'primeng/api';
import DataTopicStoreHttpService from 'src/api/dataTopicStore';

@Component({
  selector: 'app-data-topic-edit-step3',
  templateUrl: './index.html',
  styleUrls: ['./index.scss'],
  providers: [MessageService, DataTopicStoreHttpService],
})
export class DataTopicEditStep3Component implements OnInit {
  @Input() field: any;
  @Output() getValidation = new EventEmitter<any>();

  listForm: any[] = []
  loading: boolean = false
  isView: boolean = false
  isSuccess: any = null
  errorMsg: any = null
  dataValue: any = null
  isArray: boolean = true
  objectKeks = Object.keys
  headerData: any = []
  bodyData: any = []
  constructor(
    private readonly messageService: MessageService,
    private readonly httpService: DataTopicStoreHttpService,
  ) { }

  public ngOnInit() {
    this.errorMsg = this.field.verification_failure_reason === '' ? null : this.field.verification_failure_reason

    if (this.field.json_validation_result) {
      let res: any = {}
      try {
        res = JSON.parse(this.field.json_validation_result) || {}
      } catch (e: any) {
        console.error(e)
      }

      this.showDataValue(res?.Data)
    }
  }

  showDataValue(data: any) {
    this.listForm = data || []
    this.dataValue = data
    this.isArray = Array.isArray(this.dataValue)
    if (this.isArray) {
      const data = this.dataValue[0] || this.dataValue?.data[0]
      this.headerData = Object.keys(data)
    }

    if(this.dataValue?.TotalCount) {
      this.isArray = true
      const data = this.dataValue?.Data[0]
      this.dataValue = this.dataValue?.Data
      this.headerData = Object.keys(data)
    }
  }

  async validation() {
    const res = await this.httpService.verifyData(this.field.id)

    if (res.succeeded) {
      this.errorMsg = ''
      this.showDataValue(res.data)
      this.getValidation.emit(true)
    } else {
      this.errorMsg = res.errors
      this.messageService.add({
        severity: 'error',
        summary: res.errors,
      });

      this.getValidation.emit(false)
    }
  }

}
