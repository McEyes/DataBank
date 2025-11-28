import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { CommonService } from 'jabil-bus-lib';
import { NzUploadChangeParam, NzUploadFile } from 'ng-zorro-antd/upload';
import { MessageService } from 'primeng/api';
import CommonHttpService from 'src/api/common';
import DocHttpService from 'src/api/systemManage/docApi';
import { TranslateData } from 'src/app/core/translate/translate-data';
import { environment } from 'src/environments/environment';



@Component({
  selector: 'app-system-doc-details',
  templateUrl: './detail.component.html',
  styleUrls: ['./detail.component.scss'],
  providers: [
    MessageService,
    CommonService
  ],
})
export class DocDetailComponent implements OnInit {
  @Output() goBack = new EventEmitter<string>();
  @Input() field: any;


  fileHost: string = environment.FileServer;
  token: string = ''

  formData: any = {
    name: '',
    displayNameCn: '',
    displayNameEn: '',
    extension: '',
    status: true,
    docver: '1',
    catalog: 'document',
    sort: '99',
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
    private readonly http: DocHttpService,
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
    this.formData = { ...this.field }

  }

  backClick(type?: string) {
    this.goBack.emit(type)
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

  // 文件流操作
  handleChange({ file, fileList }: NzUploadChangeParam): void {
    const status = file.status;
    if (status === 'done') {
      this.formData.name = this.formData.displayNameCn = this.formData.displayNameEn = file.name;
      this.formData.url = file.response?.data?.fileUrl;
      this.formData.extension = file.response?.data?.fileType;
      this.formData.fileType = file.type;
      var msg =  TranslateData.fileUploadSuccess ;
      this.messageService.add({ severity: 'success', summary:msg });
    } else if (status === 'removed') {
      var msg2 =  TranslateData.deleteSuccess ;
      this.messageService.add({ severity: 'success', summary: msg2 });
    } else if (status === 'error') {
      file.error.statusText = TranslateData.fileUploadFail;
      var data:any=file;
      this.messageService.add({key: 'key',severity: 'error', summary: file.error.statusText, detail:data?.message, });
    }
  }

  // 文件流操作
  handleChangeIcon({ file, fileList }: NzUploadChangeParam): void {
    const status = file.status;
    if (status === 'done') {
      this.formData.docIconUrl = file.response?.data?.fileUrl;
      var msg =  TranslateData.fileUploadSuccess ;
      this.messageService.add({ severity: 'success', summary:msg });
    } else if (status === 'removed') {
      var msg2 =  TranslateData.deleteSuccess ;
      this.messageService.add({ severity: 'success', summary: msg2 });
    } else if (status === 'error') {
      file.error.statusText = TranslateData.fileUploadFail;
      var data:any=file;
      this.messageService.add({key: 'key',severity: 'error', summary: file.error.statusText, detail:data?.message, });
    }
  }
  handleDownloadFile = async (file: NzUploadFile): Promise<void> => {
    if (file?.originFileObj) {
      window.open(URL.createObjectURL(file?.originFileObj), '_blank');
    } else {
      this.download({
        url: file['fileUrl'],
        displayNameCn: file['fileName'],
      });
    }
  };

  download(item: any) {
    window.open(environment.FileServer + "/api/file/download/" + item.url + "?category=Document&downName=" + item.displayNameCn, "_blank")
  }
  // download(item: any) {
  //   let nameList = item.fileName.split('.');
  //   let name = nameList[0];
  //   this.getBlob(item.fileUrl).then(blob => {
  //     this.saveAs(blob, name);
  //   });
  // }
  // getBlob(url: string) {
  //   return new Promise(resolve => {
  //     const xhr = new XMLHttpRequest();
  //     xhr.open('GET', this.fileHost + "/api/file/download/" + url + "?category=Document", true);
  //     xhr.setRequestHeader('Accept', 'application/pdf');
  //     xhr.responseType = 'blob';
  //     xhr.onload = () => {
  //       if (xhr.status === 200) {
  //         resolve(xhr.response);
  //       }
  //     };
  //     xhr.send();
  //   });
  // }
  // saveAs(blob: any, filename: string) {
  //   const link = document.createElement('a');
  //   link.href = window.URL.createObjectURL(blob);
  //   link.download = filename + '.xlsx';
  //   link.click();
  // }
}
