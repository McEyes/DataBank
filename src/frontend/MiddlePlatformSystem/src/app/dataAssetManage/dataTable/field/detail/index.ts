import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { MessageService } from 'primeng/api';
import { CommonService } from 'jabil-bus-lib';
import { TranslateData } from 'src/app/core/translate/translate-data';
import DataColumnHttpService from 'src/api/dataAssetManage/dataColumn';
import DictHttpService from 'src/api/common/dict';

@Component({
  selector: 'app-datatable-field-details',
  templateUrl: './index.html',
  styleUrls: ['./index.scss'],
  providers: [
    MessageService,
    CommonService,
    DataColumnHttpService,
    DictHttpService,
  ],
})
export class DatatableFieldDetailsComponent implements OnInit {
  @Output() goBack = new EventEmitter<string>();
  @Input() field: any;
  formData: any = {
    colName: '',
    colComment: '',
    colKey: '1',
    nullable: '1',
  };

  typeList: any[];

  detailsTitlte: string = '';
  loading: boolean = false;
  levelList: any[] = []
  constructor(
    private messageService: MessageService,
    private commonService: CommonService,
    private columnService: DataColumnHttpService,
    private dictService: DictHttpService,
  ) { }

  buildFormData() {
    console.log(this.field)
    this.formData = {
      id: this.field?.id,
      tableId: this.field?.tableId,
      sourceId: this.field?.sourceId,
      colPosition: this.field?.colPosition,
      colName: this.field?.colName,
      colComment: this.field?.colComment,
      colKey: this.field?.colKey == 1 ? '1' : '0',
      nullable: this.field?.nullable == 1 ? '1' : '0',
      dataType: this.field?.dataType,
      dataLength: this.field?.dataLength,
      dataPrecision: this.field?.dataPrecision,
      dataScale: this.field?.dataScale,
      dataDefault: this.field?.dataDefault,
    }
  }

  ngOnInit(): void {
    this.buildFormData()
    this.getTypeList();

    this.levelList = [
      {
        itemText: '5',
        itemValue: TranslateData.secretLevel5
      },
      {
        itemText: '4',
        itemValue: TranslateData.secretLevel4
      },
      {
        itemText: '3',
        itemValue: TranslateData.secretLevel3
      },
      {
        itemText: '2',
        itemValue: TranslateData.secretLevel2
      },
      {
        itemText: '1',
        itemValue: TranslateData.secretLevel1
      },
    ];
  }

  async getTypeList() {
    let { data } = await this.dictService.code('data_type_mysql');
    try {
      this.typeList = data;
    } catch (error: any) {
      this.messageService.add({
        severity: 'error',
        summary: TranslateData.fail,
        detail: error.msg,
      });
    }
  }

  goBackPage(type?: string) {
    this.goBack.emit(type)
  }

  submit() {
    const valid = ['colName', 'colComment'];
    if (this.commonService.isInvalid(this.formData, valid)) {
      this.formData.invalid = true;
    } else {
      this.loading = true;
      this.columnService
        .update(this.formData.id, this.formData)
        .then((resSave: any) => {
          if(resSave.code === 200) {
            this.messageService.add({
              severity: 'success',
              summary: 'success',
              detail: TranslateData.success,
            });
          }

          setTimeout(() => {
            this.loading = false;
            this.goBackPage('update');
          }, 800);
        })
        .catch((error: any) => {
          this.loading = false;
          this.messageService.add({
            severity: 'error',
            summary: TranslateData.fail,
            detail: error.msg,
          });
        });
    }
  }

}
