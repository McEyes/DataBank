import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { MessageService, ConfirmationService } from 'primeng/api';
import DataTopicStoreHttpService from 'src/api/dataTopicStore';
import { TranslateData } from 'src/app/core/translate/translate-data';
import { CommonService, LocalStorage } from 'jabil-bus-lib';

@Component({
  selector: 'business-model-category-manage',
  templateUrl: './categoryManage.component.html',
  styleUrls: ['./categoryManage.component.scss'],
  providers: [
    LocalStorage,
    MessageService,
    ConfirmationService,
    CommonService,
  ],
})
export class BusinessModelCategoryManageComponent implements OnInit {
  @Input() field: any;
  @Output() closeManageDialog = new EventEmitter<string>();
  @Output() updateCategory = new EventEmitter<string>();
  @Output() editData = new EventEmitter<string>();

  loading!: boolean;
  editing = false;
  dialogDisplay = false;
  currentLanguage: any = 'en';
  recordCount: number = 0;
  formData: any = {
    id: 0,
    name: '',
    parent_id: '',
    invalid: false,
  };
  addType: number = 0; //0:new 1:edit
  tableData: Array<any> = [];

  searchData: any = {
    pageIndex: 1,
    pageSize: 10,
  };

  categoryTreeData: Array<any> = [];
  treeData: Array<any> = [];

  constructor(
    private readonly http: DataTopicStoreHttpService,
    private readonly translate: TranslateService,
    private readonly confirmationService: ConfirmationService,
    private readonly messageService: MessageService,
    private readonly commonFunction: CommonService
  ) { }

  public async ngOnInit() {
    this.currentLanguage = localStorage.getItem('lang');
    this.translate.use(this.currentLanguage);
    this.getData();
  }

  async getData() {
    this.loading = true;
    let { data } = await this.http.getCategoryTree(this.searchData);
    this.tableData = data?.items || [];
    this.recordCount = data?.totalCount || 0;
    this.getSelectData();
    this.loading = false;
  }

  buildTreeData(data: any) {
    for (const element of data) {
      if (element.children && element.children.length > 0) {
        this.buildTreeData(element.children);
      } else {
        element.isLeaf = true;
      }
    }
  }

  buildTableTreeData(data: any) {
    for (const element of data) {
      element.data = {
        id: element.id,
        name: element.name,
        parent: element.parent,
      };
      if (element.children && element.children.length > 0) {
        this.buildTableTreeData(element.children);
      }
    }
  }

  async getSelectData() {
    let { data, code } = await this.http.getCategoryTree(this.searchData);
    if (!data || code === -1) {
      return
    }
    const tempData = JSON.parse(JSON.stringify(data));
    this.buildTableTreeData(tempData);
    this.treeData = tempData;
    this.buildTreeData(data);
    this.categoryTreeData = data;
  }

  categoryTreeChanges(value: any) {
    // console.log(value, this.formData.parentId);
  }

  getParentId(treeData: any, id: string) {
    if (treeData.length === 0) return;
    for (const element of treeData) {
      if (element.id === id) {
        return [];
      } else if (element.children) {
        let res: any = this.getParentId(element.children, id);
        if (res !== undefined) {
          return res.concat(element.id).reverse();
        }
      }
    }
  }

  editCategoryData(item?: any) {
    this.formData = {
      id: item?.id || null,
      name: item?.name || '',
      parent_id: item?.parent || null
    }
    this.dialogDisplay = true
    if (item) {
      this.formData.parent_id = this.getParentId(this.categoryTreeData, item.id);
    }
  }

  async submitForm() {
    const valid = ['name'];
    if (this.commonFunction.isInvalid(this.formData, valid)) {
      this.messageService.add({ severity: 'warn', summary: TranslateData.formValidMsg });
      this.formData.invalid = true;
    } else {
      this.formData.parent_id = !this.formData.parent_id || this.formData.parent_id.length === 0
        ? undefined
        : this.formData.parent_id[this.formData.parent_id.length - 1];
      let data = await this.http.saveCategoryTree(this.formData);
      if (data.succeeded) {
        this.messageService.add({
          severity: 'success',
          summary: TranslateData.saveSuccess,
        });
        this.dialogDisplay = false;
        this.getData()
      } else {
        this.messageService.add({ severity: 'error', summary: data.errors || TranslateData.saveFail });
      }
    }
  }

  deleteData(item?: any) {
    this.confirmationService.confirm({
      message: TranslateData.isDelete,
      header: TranslateData.confirm,
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: TranslateData.yes,
      rejectLabel: TranslateData.no,
      accept: async () => {
        let data = await this.http.deleteCategoryTree(item.id);
        if (data.succeeded) {
          this.updateCategory.emit();
          this.getData();
        } else {
          this.messageService.add({
            severity: 'error',
            summary: data.errors || TranslateData.deleteFail
          });
        }
      },
    });
  }

  closeDialog() {
    this.dialogDisplay = false;
  }
}
