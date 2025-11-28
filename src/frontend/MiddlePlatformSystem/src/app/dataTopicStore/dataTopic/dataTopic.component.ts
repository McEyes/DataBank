import { Component, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { TreeNode, MessageService, ConfirmationService } from 'primeng/api';
import { en_US, NzI18nService, zh_CN } from 'ng-zorro-antd/i18n';
import { TranslateService } from '@ngx-translate/core';
import { TranslateData } from 'src/app/core/translate/translate-data'
import CommonHttpService from 'src/api/common'
import DataTopicStoreHttpService from 'src/api/dataTopicStore';
import { DataTopicStoreHomeComponent } from '../components/home/home.component';
import { DataTopicStoreDetailComponent } from '../components/detail/detail.component';
import { DataTopicCreateComponent } from '../components/create/create.component';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { filter, map, takeUntil } from 'rxjs/operators';
import { Subject } from 'rxjs';

@Component({
  selector: 'app-data-topic',
  templateUrl: './dataTopic.component.html',
  styleUrls: ['./dataTopic.component.scss'],
  encapsulation: ViewEncapsulation.Emulated,
  providers: [
    MessageService,
    ConfirmationService,
    CommonHttpService,
    DataTopicStoreHttpService
  ],
})
export class DataTopicComponent implements OnInit {
  @ViewChild('home', { static: false }) home: DataTopicStoreHomeComponent;
  @ViewChild('detail', { static: false }) detail: DataTopicStoreDetailComponent;
  @ViewChild('create', { static: false }) create: DataTopicCreateComponent;

  loading: boolean = false;
  total: number = 0;
  first: number = 0;
  treeLoaing: boolean = false;
  treeData: any[] = [];
  tableData: TreeNode[] = [];
  timeout: any = null;
  baseTreeData: any[] = [];
  treeQuery: string = '';
  lang: any = 'en'
  currPanel: string = 'home'
  currRow: any = {}
  currBreadcrumbs: any = {}
  selectedFile: any = null
  listForm: any = {
    categoryId: '',
    topicName: '',
    pageSize: 20,
    pageNum: 1,
  };
  categoryId: string = ''
  goDetailType: any = ''


  isCollapsed: boolean = false;
  isCollapseMenu: boolean = false;
  searchMenu: string = ''
  currMenu: any = {};
  constructor(
    private readonly messageService: MessageService,
    private readonly i18n: NzI18nService,
    private readonly translate: TranslateService,
    private readonly confirmationService: ConfirmationService,
    private readonly dataTopicStoreHttpService: DataTopicStoreHttpService
  ) {
    window.addEventListener('resetPage', (e: any) => {
      if (location.href.includes('create')) {
        this.currPanel = 'create'
        return
      }
      this.currPanel = 'home'
    });

    this.lang = localStorage.getItem('lang') || 'en'
    this.translate.use(this.lang);
    this.i18n.setLocale(this.lang === 'en' ? en_US : zh_CN);

    if (location.href.includes('create')) {
      this.currPanel = 'create'
    }
  }

  ngOnInit(): void {
    //获取树状主题域
    this.getTree();
  }

  async queryTable() {
    this.loading = true;
    const res = await this.dataTopicStoreHttpService.getTopicData(
      this.listForm
    )
    if (res.succeeded) {
      this.tableData = res?.data?.data ?? []
      this.total = res?.data?.totalCount ?? 0
    } else {
      this.messageService.add({
        severity: 'warn',
        summary: TranslateData.tips,
        detail: res?.errors
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

  nodeSelect(e: any, item: any) {
    if (this.currPanel === 'create' && this.create) {
      this.create.categoryId = item.key
      this.categoryId = item.key
      this.currMenu = item
      this.listForm.categoryId = item.key
      return
    }

    this.currBreadcrumbs = { name: item.label }
    this.categoryId = item.key
    this.currMenu = item
    this.listForm.categoryId = item.key
    this.queryTable();
    this.currPanel = 'list'
    e.stopPropagation();
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

  openDetail(rowData: any, type?: string) {
    this.goDetailType = type
    this.currRow = rowData
    this.currBreadcrumbs = { ...rowData }
    this.currPanel = 'detail'
  }

  goHome() {
    this.selectedFile = null
    this.currMenu = {}
    this.currPanel = 'home'
  }

  createData() {
    if (!this.categoryId) {
      this.messageService.add({
        severity: 'warn',
        summary: TranslateData.tips,
        detail: TranslateData.selectCategory
      });

      return
    }
    this.currPanel = 'create'
  }

  backEvent() {
    this.currPanel = 'list'
    this.listForm.pageSize = 20
    this.listForm.pageNum = 1
    this.queryTable()
  }

  editData(row: any) {
    this.currRow = row
    this.currPanel = 'edit'
  }

  onReset() {
    this.listForm.topicName = ''
    this.listForm.pageSize = 20
    this.listForm.pageNum = 1

    this.queryTable()
  }

  backList() {
    this.currPanel = 'list'
  }

  askPermission(rowData: any) {
    this.currRow = rowData
    this.confirmationService.confirm({
      message: TranslateData.askPermissionTip + '(' + rowData.name + ')',
      header: TranslateData.confirm,
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: TranslateData.yes,
      rejectLabel: TranslateData.no,
      accept: async () => {
        this.applyPermission()
      },
    });
  }

  async applyPermission() {
    if (this.loading) return
    this.loading = true
    const res = await this.dataTopicStoreHttpService.applyPermission({
      topicId: this.currRow.id
    })

    if (res.succeeded) {
      this.messageService.add({
        severity: 'success',
        summary: TranslateData.saveSuccess,
      });
    } else {
      this.messageService.add({
        severity: 'error',
        summary: res?.errors || TranslateData.saveFail,
      });
    }
    this.loading = false
  }


  showMenu(status: boolean) {
    if (!this.isCollapsed) {
      return
    }

    this.isCollapseMenu = status
  }

  toggleCollapsed(): void {
    this.isCollapsed = !this.isCollapsed;
    this.isCollapseMenu = !this.isCollapseMenu;
  }

  goBack() {
    if (this.currPanel === 'detail') {
      if (this.detail.currPanel === 'preview' || this.detail.currPanel === 'dataLineage') {
        this.detail.goDetail()
        return
      } else {
        // 主页面的切换
        if (this.goDetailType) {
          this.currPanel = 'list'
          this.currBreadcrumbs = { name: this.currMenu.label }
        } else {
          this.currPanel = 'home'
          this.currBreadcrumbs = {}
          this.currMenu = {}
        }

        this.goDetailType = ''
        return
      }
    }

    if (this.currPanel === 'home') {
      if (this.home.currMorePanel) {
        // home页面返回事件
        this.home.goBack()
      }


      // 设置子级名字,还原数据
      this.currBreadcrumbs = {}
      this.currMenu = {}
      // 主页面的切换
      this.currPanel = 'home'
      return
    }

    if (this.currPanel === 'list') {
      // 设置子级名字,还原数据
      this.currBreadcrumbs = {}
      this.currMenu = {}
      // 主页面的切换
      this.currPanel = 'home'
    }

    if (this.currPanel === 'create') {
      this.currPanel = 'list'
    }
  }

  goCreateData() {
    this.currPanel = 'create'
  }
}