import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { MessageService } from 'primeng/api';
import DataTopicStoreHttpService from 'src/api/dataTopicStore';
import { CommonService } from 'jabil-bus-lib';
import { TranslateData } from 'src/app/core/translate/translate-data';
import * as ace from 'brace';
import { NzUploadFile } from 'ng-zorro-antd/upload';
import { Observable, Observer } from 'rxjs';
import { environment } from 'src/environments/environment';
import DictHttpService from 'src/api/common/dict';
import { TranslateService } from '@ngx-translate/core';
import CommonHttpService from 'src/api/common';

@Component({
  selector: 'app-data-topic-edit',
  templateUrl: './edit.html',
  styleUrls: ['./edit.scss'],
  providers: [
    MessageService,
    DataTopicStoreHttpService,
    CommonService,
    DictHttpService,
    // TranslateService
  ],
})
export class DataTopicEditComponent implements OnInit {
  @Output() goBack = new EventEmitter<string>();
  @Input() field: any;

  changeParams: any = {
    pageSize: 5,
    changeParams: 1,
  };
  formData: any = {
    sourceName: '',
    dbType: '',
    owner_id: '',
    status: 1,
    dbSchema: {
      dbName: '',
      host: '',
      port: '',
      username: '',
      password: '',
    },
    category_id: '',
    tags: null
  };
  loading: boolean = false;
  invalid: boolean = false;
  currLang: string = '';
  avatarUrl: string = '';
  fileHost: string = environment.FileServer;
  token: string = ''
  tagList: any[] = [{ name: 'KPI' }, { name: 'Plant' }, { name: 'Realtime' }]
  categoryData: any = []
  userList: any = []
  categoryItem: any = {}
  constructor(
    private readonly messageService: MessageService,
    private readonly httpService: DataTopicStoreHttpService,
    private readonly commonService: CommonService,
    private dictService: DictHttpService,
    private readonly translate: TranslateService,
    private itCommonService: CommonHttpService,
  ) {
    this.currLang = localStorage.getItem('lang') || 'en'
    this.token = localStorage.getItem('jwt') || ''
  }

  async ngOnInit(): Promise<void> {
    await this.getCategoryTree()
    this.getFormData()
    this.getTagList()
    this.getUserList()
    // sql editor
    // this.initEditor()
  }

  initEditor() {
    let editStyle: any = document.getElementById('mysql-edit');
    editStyle.style.fontSize = '12px';
    this.formData.sql_scripts = ace.edit('mysql-edit');
    let editor: any = ace.edit('mysql-edit');
    editor.resize();
    editor.getSession().setMode('ace/mode/sql');
    editor.setTheme('ace/theme/sqlserver');
    // 添加自动滚动到视图的行为
    editor.getSession().selection.on('changeSelection', function () {
      editor.renderer.scrollCursorIntoView(null, 0.5);
    });
    editor.setOptions({
      enableBasicAutocompletion: true,
      enableLiveAutocompletion: true,
    });
    editor.$blockScrolling = Infinity;
    editor.setFontSize(16);
    editor.setOption('enableEmmet', true);

  }

  async getCategoryTree() {
    let { data } = await this.httpService.getCategoryTree();
    this.categoryData = this.transformArray(data);

    console.log('categoryData', this.categoryData);
  }

  transformArray(items: any[]) {
    return items.map((item: any) => {
      let newItem: any = {
        key: item.id,
        label: item.name,
        expanded: false
      };
      if (item.children && item.children.length > 0) {
        newItem.children = this.transformArray(item.children);
      } else {
        newItem.children = [];
      }
      return newItem;
    });
  }

  async getTagList() {
    let res = await this.dictService.codes('BusinessModelTag');
    if (res.success) {
      this.tagList = res.data
      this.tagList.forEach((f: any) => {
        this.translate.get("dict." + f.dictCode + "." + f.itemValue).subscribe((res: string) => {
          if (res != "dict." + f.dictCode + "." + f.itemValue) f.itemText = res;
        });
      });

      console.log('tagList', this.tagList);
    }
  }

  async getFormData() {
    const res = await this.httpService.getTopicDetailData(this.field.id)

    if (!res.succeeded) {
      this.messageService.add({
        severity: 'error',
        summary: res.errors,
      });
      this.loading = false
      return
    }
    
    this.formData = res.data
    if (this.formData.tags) {
      const tags = this.formData.tags.split(',')
      this.formData.tags = []
      tags.forEach((item: any) => {
        this.formData.tags.push(item?.trim())
      })
    } else {
      this.formData.tags = []
    }

    this.categoryItem = this.findTreeItem(this.categoryData, 'key', this.formData.category_id) || {}
  }

  findTreeItem(treeData: any, key: any, value: any, childrenKey = 'children'): any {
    if (!Array.isArray(treeData)) return null;

    for (const element of treeData) {
      const node = element;
      if (node[key] === value) {
        return node;
      }
      if (node[childrenKey] && node[childrenKey].length > 0) {
        const result = this.findTreeItem(node[childrenKey], key, value, childrenKey);
        if (result) return result;
      }
    }
    return null;
  }

  paginate(event: any) {
    this.changeParams.pageNum = event.page + 1;
    this.changeParams.pageSize = event.rows;
    this.changeParams.first = event.first;
  }

  backClick(type?: string) {
    this.goBack.emit(type)
  }

  pushClick() {

    const valid = [
      'name',
      'version',
      'owner_id'
    ];
    if (this.commonService.isInvalid(this.formData, valid) || this.formData?.tags?.length == 0 || !this.categoryItem.key) {
      this.messageService.add({ severity: 'warn', summary: TranslateData.formValidMsg });
      this.invalid = true;
    } else {
      this.loading = true;
      this.formData.tagList = this.formData.tags.join(',')
      this.formData.category_id = this.categoryItem.key || ''
      this.httpService.editTopicData(this.formData).then((resSave: any) => {
        if (resSave.succeeded) {
          this.messageService.add({
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
            severity: 'error',
            summary: TranslateData.fail,
            detail: resSave.errors,
          });
        }
        this.loading = false;
      });
    }
  }

  handlePicChange(info: { file: NzUploadFile }, type?: string): void {
    switch (info.file.status) {
      case 'uploading':
        this.loading = true;
        break;
      case 'done':
        // Get this url from response in real world.
        this.getBase64(info.file!.originFileObj!, (img: string) => {
          this.loading = false;
          this.avatarUrl = img;
        });

        this.formData.cover =
          info.file?.response?.data?.fileUrl || info.file?.response?.msg;
        break;
      case 'error':
        this.messageService.add({
          severity: 'error',
          summary: 'Network error!',
          detail: '',
        });
        this.loading = false;
        break;
    }
  }

  private getBase64(img: File, callback: (img: string) => void): void {
    const reader = new FileReader();
    reader.addEventListener('load', () => callback(reader.result!.toString()));
    reader.readAsDataURL(img);
  }

  beforePicUpload = (file: NzUploadFile, _fileList: NzUploadFile[]): Observable<boolean> =>
    new Observable((observer: Observer<boolean>) => {
      const isJpgOrPng = file.type === 'image/jpeg' || file.type === 'image/png';
      if (!isJpgOrPng) {
        this.messageService.add({
          severity: 'error',
          summary: TranslateData.jpgPng,
          detail: '',
        });
        observer.complete();
        return;
      }
      const isLt20M = file.size! / 1024 / 1024 < 20;
      if (!isLt20M) {
        this.messageService.add({
          severity: 'error',
          summary: TranslateData.imageLimit20mb,
          detail: '',
        });
        observer.complete();
        return;
      }
      observer.next(isJpgOrPng && isLt20M);
      observer.complete();
    });

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
}
