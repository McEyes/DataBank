import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { MessageService } from 'primeng/api';
import DataTopicStoreHttpService from 'src/api/dataTopicStore';
import { TranslateData } from 'src/app/core/translate/translate-data';
import { DataTopicEditStep1Component } from './components/step1';
import { DataTopicEditStep2Component } from './components/step2';
import { DataTopicEditStep3Component } from './components/step3';
import { DataTopicEditStep4Component } from './components/step4';
import { formatDate } from '@angular/common';

@Component({
  selector: 'app-data-topic-create',
  templateUrl: './create.component.html',
  styleUrls: ['./create.component.scss'],
  providers: [MessageService, DataTopicStoreHttpService],
})
export class DataTopicCreateComponent implements OnInit {
  @ViewChild('step1') step1: DataTopicEditStep1Component;
  @ViewChild('step2') step2: DataTopicEditStep2Component;
  @ViewChild('step3') step3: DataTopicEditStep3Component;
  @ViewChild('step4') step4: DataTopicEditStep4Component;

  @Output() goBack = new EventEmitter<string>();
  @Input() categoryId: any;
  @Input() field: any;
  @Input() status: 'create' | 'edit' | 'view';

  currStep: number = 1
  editorValue: string = ''
  approvalRemark: string = ''
  formData: any = null
  loading: boolean = false
  approvalDialogVisible: boolean = false
  pogressObj: any = {
    BusinessModeling: 1,
    ITDeveloping: 2,
    Validation: 3,
    Publish: 4,
    Completed: 81,
    businessmodeling: 1,
    itdeveloping: 2,
    validation: 3,
    publish: 4,
    completed: 81,
  }
  constructor(
    public readonly httpService: DataTopicStoreHttpService,
    private readonly messageService: MessageService,
  ) { }

  public async ngOnInit() {
    if (this.status === 'edit' || this.status === 'view') {
      this.loading = true
      const res = await this.httpService.getTopicDetailData(this.field.id)
      if (!res.succeeded) {
        this.messageService.add({
          severity: 'error',
          summary: res.errors,
        });
        this.loading = false
        return
      }

      this.formData = res.data || {}

      if (this.formData.progress === 'Completed') {
        this.currStep = 1
      } else {
        this.currStep = this.pogressObj[this.formData.progress] || 1
      }

      setTimeout(() => {
        this.loading = false
        this.init()
      }, 500)
    }
  }

  async init() {
    this.step1.editorValue = this.formData.content
    this.step1.readonly = this.status === 'view'

    this.step2.isView = this.status === 'view'
    const step2Data = this.formData?.itDevelopingSetting?.data
    if (step2Data) {
      step2Data.dataSourceDefinition.process_time = step2Data.dataSourceDefinition.process_time ? new Date(step2Data.dataSourceDefinition.process_time) : ''
      step2Data.dataSourceIngestion.process_time = step2Data.dataSourceIngestion.process_time ? new Date(step2Data.dataSourceIngestion.process_time) : ''
      step2Data.physicalModeling.process_time = step2Data.physicalModeling.process_time ? new Date(step2Data.physicalModeling.process_time) : ''
      step2Data.tScriptGeneration.process_time = step2Data.tScriptGeneration.process_time ? new Date(step2Data.tScriptGeneration.process_time) : ''
      step2Data.apiGeneration.process_time = step2Data.apiGeneration.process_time ? new Date(step2Data.apiGeneration.process_time) : ''
      this.step2.formData = this.formData?.itDevelopingSetting?.data
    }

    this.step2.inputSetting = this.formData?.inputParameterSetting?.inputSetting || {
      parameters: []
    }
    this.step2.outputSetting = this.formData?.outputParameterSetting?.outputSetting || {
      type: "array",
      isPaged: true,
      parameters: []
    }

    this.step3.isView = this.status === 'view'

    this.step4.isView = this.status === 'view'
  }

  next() {
    // BusinessModeling = 1
    // ITDeveloping = 2
    // Validation = 3
    // Publish = 9
    // Completed = 81


    // 到达当前流程步骤则不可以向下跳转
    // if (this.currStep === this.pogressObj[this.formData.progress] && this.status === 'view') {
    //   this.messageService.add({
    //     severity: 'warn',
    //     summary: TranslateData.dataNoPublish,
    //   });
    //   return
    // }

    // 没有通过验证
    // if (!this.formData.is_verification_passed && this.currStep === 3) {
    //   this.messageService.add({
    //     severity: 'warn',
    //     summary: TranslateData.dataNoPublish,
    //   });
    //   return
    // }
    this.currStep++
  }

  previous() {
    this.currStep--
  }

  goBackEvent() {
    this.goBack.emit()
  }

  async publish() {
    if (this.loading) return
    this.loading = true
    const res = await this.httpService.publish({
      id: this.field.id,
    })

    this.loading = false
    if (res.succeeded) {
      this.messageService.add({
        severity: 'success',
        summary: TranslateData.saveSuccess,
      });

      this.goBackEvent()
    } else {
      this.messageService.add({
        severity: 'error',
        summary: res.errors || TranslateData.saveFail,
      });
    }
  }

  async createData() {
    if(!this.categoryId) {
      this.messageService.add({
        severity: 'warn',
        summary: TranslateData.dataTopicSelectCategory
      });
      return
    }
    if (this.loading) return
    this.loading = true
    const res = await this.httpService.createDataTopic({
      category_id: this.categoryId,
      contents: this.step1.editorValue
    })

    this.loading = false
    if (res.succeeded) {
      this.messageService.add({
        severity: 'success',
        summary: TranslateData.saveSuccess,
      });

      setTimeout(() => {
        this.goBackEvent()
      }, 1000)

    } else {
      this.messageService.add({
        severity: 'error',
        summary: TranslateData.saveFail,
      });
    }
  }

  async saveITDev() {
    if (this.loading) return
    this.loading = true
    const formData = { ...this.step2.formData }
    if (
      !formData.dataSourceDefinition.process_time || !formData.dataSourceIngestion.process_time || !formData.physicalModeling.process_time || !formData.tScriptGeneration.process_time || !formData.apiGeneration.process_time
      || !formData.dataSourceDefinition.pic_ntid || !formData.dataSourceIngestion.pic_ntid || !formData.physicalModeling.pic_ntid || !formData.tScriptGeneration.pic_ntid || !formData.apiGeneration.pic_ntid
    ) {
      this.messageService.add({
        severity: 'warn',
        summary: TranslateData.datePicValid,
      });
      this.loading = false
      return
    }

    let res1, res2, res3 = null

    try {
      formData.dataSourceDefinition.process_time = formData.dataSourceDefinition.process_time ? formatDate(formData.dataSourceDefinition.process_time, 'yyyy-MM-dd HH:mm:ss', 'en-US') : ''
      formData.dataSourceIngestion.process_time = formData.dataSourceIngestion.process_time ? formatDate(formData.dataSourceIngestion.process_time, 'yyyy-MM-dd HH:mm:ss', 'en-US') : ''
      formData.physicalModeling.process_time = formData.physicalModeling.process_time ? formatDate(formData.physicalModeling.process_time, 'yyyy-MM-dd HH:mm:ss', 'en-US') : ''
      formData.tScriptGeneration.process_time = formData.tScriptGeneration.process_time ? formatDate(formData.tScriptGeneration.process_time, 'yyyy-MM-dd HH:mm:ss', 'en-US') : ''
      formData.apiGeneration.process_time = formData.apiGeneration.process_time ? formatDate(formData.apiGeneration.process_time, 'yyyy-MM-dd HH:mm:ss', 'en-US') : ''

      res1 = await this.httpService.saveITDevelopRecords({
        topic_id: this.field.id,
        data: this.step2.formData
      })

      res2 = await this.httpService.setParametersInput({
        topicId: this.field.id,
        inputSetting: this.step2.inputSetting
      })

      res3 = await this.httpService.setParametersOutput({
        topicId: this.field.id,
        outputSetting: this.step2.outputSetting
      })
    } catch (error) {
      this.messageService.add({
        severity: 'error',
        summary: TranslateData.saveFail,
      });
    } finally {
      this.loading = false
    }

    if (res1.succeeded && res2.succeeded && res3.succeeded) {
      this.messageService.add({
        severity: 'success',
        summary: TranslateData.saveSuccess,
      });

      setTimeout(() => {
        this.goBackEvent()
      }, 1000)
    } else {
      this.messageService.add({
        severity: 'error',
        summary: TranslateData.saveFail,
      });
    }
  }

  getValidation(e: any) {
    this.formData.is_verification_passed = e
  }

  approve() {
    this.approvalRemark = ''
    this.approvalDialogVisible = true
  }

  async applyPermission(type: number) {
    if (this.loading) return
    this.loading = true
    const res = await this.httpService.approveWorkflowPermission({
      topicId: this.field.id,
      action: type,
      remark: this.approvalRemark
    })

    if (res.succeeded) {
      this.messageService.add({
        severity: 'success',
        summary: TranslateData.saveSuccess,
      });
      this.approvalDialogVisible = false

      setTimeout(() => {
        this.goBackEvent()
      }, 1000)
    } else {
      this.messageService.add({
        severity: 'error',
        summary: res?.errors || TranslateData.saveFail,
      });
    }
    this.loading = false
  }

  showApprove() {
    return this.currStep === this.pogressObj[this.formData?.progress] && this.formData.can_edit
  }

  showNext() {
    return this.currStep < this.pogressObj[this.formData?.progress]
  }
}
