import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { MessageService, ConfirmationService, TreeNode } from 'primeng/api';
import { en_US, NzI18nService, zh_CN } from 'ng-zorro-antd/i18n';
import { TranslateData } from 'src/app/core/translate/translate-data'
import AssetQueryHttpService from 'src/api/dataAssetManage/assetQuery'
import CommonHttpService from 'src/api/common'

import DataTopicStoreHttpService from 'src/api/dataTopicStore';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-my-data-topic',
  templateUrl: './myData.component.html',
  styleUrls: ['./myData.component.scss'],
  encapsulation: ViewEncapsulation.Emulated,
  providers: [
    MessageService,
    AssetQueryHttpService,
    ConfirmationService,
    CommonHttpService,
    DataTopicStoreHttpService
  ],
})
export class MyDataTopicComponent implements OnInit {
  loading: boolean = false;
  total: number = 0;
  first: number = 0;
  defaultParams: any = {};
  selectedFile: any;
  treeLoaing: boolean = false;
  treeData: any[] = [];
  tableData: TreeNode[] = [];
  userInfo: any = {};
  userList: Array<any> = [];
  tabsNavNumber: number = 0;
  timeout: any = null;
  baseTreeData: any[] = [];
  treeQuery: string = '';
  lang: any = 'en'
  currPanel: string = 'list'
  currRow: any = {}
  listForm: any = {
    categoryId: '',
    topicName: '',
    pageSize: 20,
    pageNum: 1,
  };
  categoryId: string = ''
  currStatus: 'create' | 'edit' | 'view' = 'view'

  constructor(
    private readonly messageService: MessageService,
    private readonly i18n: NzI18nService,
    private readonly translate: TranslateService,
    private readonly httpService: AssetQueryHttpService,
    private readonly dataTopicStoreHttpService: DataTopicStoreHttpService,
  ) {
    window.addEventListener('resetPage', (e: any) => {
      this.currPanel = 'list'
    });

    this.lang = localStorage.getItem('lang') || 'en'
    this.translate.use(this.lang);
    this.i18n.setLocale(this.lang === 'en' ? en_US : zh_CN);
  }

  ngOnInit(): void {

    //获取树状主题域
    this.getTree();
    this.queryTable()
  }

  async queryTable() {
    this.loading = true;
    const res = await this.dataTopicStoreHttpService.getMyTopicDataList(
      this.listForm
    )
    if (res.succeeded) {
      this.tableData = res?.data?.data ?? []
      this.total = res?.data?.totalCount ?? 0
    } else {
      this.messageService.add({
        severity: 'warn',
        summary: TranslateData.tips,
        detail: res?.errors?.id[0]
      });
    }
    this.loading = false;
  }

  async getTree() {
    this.treeLoaing = true;
    let { data } = await this.dataTopicStoreHttpService.getCategoryTree();
    this.baseTreeData = data;
    this.treeData = this.transformArray(data);
    this.treeLoaing = false;
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

  nodeSelect(event: any) {
    this.categoryId = event.node.key
    this.listForm.categoryId = event.node.key
    this.queryTable();
    this.currPanel = 'list'
  }

  // tree search function
  getTreeData() {
    const baseTreeData = JSON.parse(JSON.stringify(this.baseTreeData))
    const filteredData = this.filterTreeData(baseTreeData, this.treeQuery);
    this.treeQuery && this.expandTreeNode(filteredData)
    this.treeData = this.transformArray(filteredData);
  }

  filterTreeData(data: any[], searchTerm: string): any[] {
    if (!searchTerm) {
      data.forEach((node) => {
        node.expanded = false
      })
      return data;
    }
    return data
      .map(node => {
        if (node.children) {
          node.expanded = true
          const filteredChildren = this.filterTreeData(node.children, searchTerm);
          if (filteredChildren.length) {
            return { ...node, children: filteredChildren };
          }
        }

        if (node?.name?.toLowerCase().includes(searchTerm.toLowerCase())) {
          // 全部设为设置，匹配到的节点设为false，其余为true，为了后续构建数据
          node.expanded = false
          return node;
        }
        return null;
      })
      .filter(node => node !== null);
  }

  searchTree() {
    if (this.timeout !== null) {
      clearTimeout(this.timeout); // clear
    }

    this.timeout = setTimeout(() => {
      this.getTreeData();
    }, 1000);
  }

  // 子节点展开设为false
  collapsedTreeNode(tree: any[]) {
    tree.forEach(node => {
      node.expanded = false

      if (node.children && node.children.length > 0) {
        this.collapsedTreeNode(node.children)
      }
    })
  }

  // 为false的展开节点设为true，并将子节点设为false。
  expandTreeNode(tree: any[]): void {
    tree.forEach(node => {
      if (!node.expanded) {
        node.expanded = true
        this.collapsedTreeNode(node.children || [])
      } else {
        if (node.children && node.children.length > 0) {
          this.expandTreeNode(node.children)
        }
      }
    })
  }

  nzPageIndexChange(index: number) {
    this.listForm.pageNum = index;
    this.queryTable();
  }

  nzPageSizeChange(page: number) {
    this.listForm.pageSize = page;
    this.queryTable();
  }

  openDetail(rowData: any, type: 'create' | 'edit' | 'view') {
    this.currRow = rowData
    this.currPanel = 'create'
    this.currStatus = type
  }

  editData(rowData: any) {
    this.currRow = rowData
    this.currPanel = 'edit'
  }

  backEvent() {
    this.currPanel = 'list'
    this.listForm.pageSize = 20
    this.listForm.pageNum = 1
    this.queryTable()
  }

  onQuery() {
    this.queryTable()
  }

  onReset() {
    this.listForm.topicName = ''
    this.listForm.pageSize = 20
    this.listForm.pageNum = 1

    this.queryTable()
  }

  goBack() {
    this.currPanel = 'list'
  }
}
