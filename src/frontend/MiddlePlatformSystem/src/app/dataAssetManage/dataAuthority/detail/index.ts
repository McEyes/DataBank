import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { TreeNode } from 'primeng/api';
import { Location } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { MessageService } from 'primeng/api';
import TopicHttpService from 'src/api/dataAssetManage/topic';
import DataTableHttpService from 'src/api/dataAssetManage/dataTable';
import AuthorityService from 'src/api/dataAssetManage/authority';
import { NzFormatEmitEvent } from 'ng-zorro-antd/tree';
import CommonHttpService from 'src/api/common/index';
import { TranslateData } from 'src/app/core/translate/translate-data';

@Component({
  selector: 'app-data-authority-detail',
  templateUrl: './index.html',
  styleUrls: ['./index.scss'],
  providers: [
    TopicHttpService,
    DataTableHttpService,
    MessageService,
    AuthorityService,
  ],
})
export class DataAuthorityDetailComponent implements OnInit {
  @Output() goBack = new EventEmitter<string>();
  @Input() field: any;

  loading: boolean = false;
  titleName: string = 'add';
  userList!: any[];
  queryParams: any = {
    id: [],
    username: '',
  };
  treeInfo: any = {};
  treeData: any[];
  searchResult: any = [];
  nodes: any = [];
  defaultCheckedKeys: any = [];
  defaultExpandedKeys: any = [];
  isSearch: boolean = false;
  treeSearchValue: string = '';
  selectedFile: any[] = [];
  userId: string;
  treeCheckList: any[] = [];
  userSelectList: any[] = [];

  private lastQuery: string = '';
  private debounceTimeout?: any;
  searchLoading: boolean = false;
  constructor(
    private messageService: MessageService,
    private location: Location,
    private route: ActivatedRoute,
    private topicService: TopicHttpService,
    private tableService: DataTableHttpService,
    private authorityService: AuthorityService,
    private commonHttpService: CommonHttpService
  ) {
    this.route.queryParams.subscribe(params => {
      // this.titleName = params['title'];
      // let info = params['info'];
      // this.treeInfo = JSON.parse(info);
      // if (this.titleName != 'add') {
      //   this.userId = this.treeInfo.userId;
      //   this.queryParams.id = [this.userId];
      //   this.getUserTableDetails();
      // }
    });

  }

  ngOnInit(): void {
    let info = this.field.info;
    this.treeInfo = JSON.parse(info);

    if (this.treeInfo?.userId) {
      this.userId = this.treeInfo.userId;
      this.queryParams.id = [this.userId];
      this.getUserTableDetails();
      this.titleName = 'update'
    } else {
      this.titleName = 'add'
      this.getTree()
    }

    this.getUserList();
  }

  onSearchTree(e: any) {
    let value = e.target.value;
    if (!value) return;
    setTimeout(() => {
      if (value === this.lastQuery) {
        return;
      }
      this.searchLoading = true;
      this.lastQuery = value;
      clearTimeout(this.debounceTimeout);
      this.debounceTimeout = setTimeout(() => {
        if (value) {
          this.searchResult = this.searchNodes(this.treeData, value, new Map<string, TreeNode>());
        } else {
          this.searchResult = this.treeData;
        }
      }, 300);
      this.searchLoading = false;
    }, 500);
  }

  searchNodes(nodes: any[], query: string, map = new Map<string, any>()): any[] {
    const queryLower = query.toLowerCase();
    nodes.forEach(node => {
      if (node.title && node.title.toLowerCase().includes(queryLower)) {
        if (!map.has(node.key)) {
          map.set(node.key, { ...node, children: node.children ? node.children : [] });
        }
      }
      if (node.children && node.children.length) {
        this.searchNodes(node.children, query, map);
      }
    });
    if (map.size > 0 && nodes === this.treeData) {
      return Array.from(map.values());
    }
    return [];
  }

  // 详情接口
  getUserTableDetails() {
    this.tableService.getTablesByUserId(this.userId).then((result: any) => {
      if (result.code == 200) {
        this.treeCheckList = result.data;
        this.getTree();
      }
    });
  }

  // 树形列表
  async getTree() {
    this.loading = true;
    let data = await this.topicService.getTree('table');
    let lang: any = localStorage.getItem('lang');
    let arr = data.data;
    let newArr = this.transformArray(arr, lang);
    this.treeData = newArr;
    this.searchResult = this.treeData;
    if (this.treeCheckList.length > 0 && this.treeData.length > 0 && this.titleName != 'add') {
      let treeCheckArr: any[] = [];
      this.clearTreeData(this.treeData, this.treeCheckList, [], treeCheckArr);
      this.defaultCheckedKeys = treeCheckArr.map((item: any) => item.key);
      this.selectedFile = treeCheckArr
    }
    this.loading = false;
  }

  transformArray(arr: any, lang: string) {
    const transformItem = (item: any) => {
      const newLabel = lang === 'en' ? item.code : item.value;
      const newItem: any = {
        title: newLabel,
        // key: item.key,
        key: `${item.key}/${item.pId}`,
        type: item.type,
        pId: item.pId,
        selectable: false,
      };
      if (item.children && item.children.length > 0) {
        newItem.children = item.children.map(transformItem);
      } else {
        newItem.isLeaf = true;
      }
      return newItem;
    };
    return arr.map(transformItem);
  }

  clearTreeData(tree: any, checklist: any, parents: any = [], newArr: any = []) {
    return tree.map((node: any) => {
      node.expanded = false;
      const isInChecklist = checklist.some((item: any) => `${item.id}/${item.ctlId}` === node.key);
      if (isInChecklist) {
        node.expanded = true;
        parents.forEach((parent: any) => (parent.expanded = true));
        newArr.push({ ...node, children: [] });
      }
      if (node.children && node.children.length > 0) {
        this.clearTreeData(node.children, checklist, [...parents, node], newArr);
      }
      return node;
    });
  }

  nzEvent(event: NzFormatEmitEvent): void {
    let nodes: any = event.nodes?.map((item: any) => {
      return item.origin;
    });
    let newNodes: any = this.formatSelectTree(nodes);
    this.selectedFile = newNodes;
  }

  formatSelectTree(arr: any[]) {
    let result: any = [];
    function processNode(node: any) {
      result.push({
        title: node.title,
        key: node.key,
        type: node.type,
        pId: node.pId,
      });
      if (node.children && node.children.length) {
        node.children.forEach((child: any) => processNode(child));
      }
    }
    arr.forEach((node: any) => processNode(node));
    return result;
  }

  backClick(type?: string) {
    this.goBack.emit(type);
  }

  onSumbit() {
    if (this.userSelectList.length == 0) {
      return this.messageService.add({
        severity: 'warn',
        summary: TranslateData.tips,
        detail: TranslateData.selectUser,
      });
    }
    if (this.selectedFile.length == 0) {
      return this.messageService.add({
        severity: 'warn',
        summary: TranslateData.tips,
        detail: TranslateData.selectTree,
      });
    }
    this.loading = true;
    let newSelectedFile = this.changeSelectFileFunc(this.selectedFile);
    this.authorityService
      .saveAuth({
        type: this.titleName == 'add' ? 'add' : 'update',
        userList: this.userSelectList,
        node: newSelectedFile,
      })
      .then((result: any) => {
        if (result.code == 200) {
          this.messageService.add({
            severity: 'success',
            summary: TranslateData.save,
            detail: TranslateData.success,
          });
          setTimeout(() => {
            this.backClick('update');
            this.loading = false;
          }, 800);
        } else {
          this.loading = false;
          this.messageService.add({
            severity: 'error',
            summary: TranslateData.fail,
            detail: result.msg,
          });
        }
      });
  }

  changeSelectFileFunc(arr: any[] = []) {
    let newArr: any = [];
    if (arr.length > 0) {
      newArr = arr.map((item: any) => {
        return {
          key: item.key.split('/')[0],
          label: item.label,
          type: item.type,
          pId: item.pId,
        };
      });
    }
    return newArr;
  }

  // 用户列表
  getUserList() {
    if (sessionStorage.getItem('userList')) {
      let userListString: any = sessionStorage.getItem('userList');
      let userList = JSON.parse(userListString);
      this.userList = userList;
      if (this.titleName != 'add') {
        this.userList.forEach((item: any) => {
          if (item.userId == this.userId) {
            this.userSelectList.push({
              id: item.userId,
              username: item.name,
            });
          }
        });
        this.userSelectList = Array.from(
          new Map(this.userSelectList.map(item => [item['id'], item])).values()
        );
      }
    } else {
      this.loading = true;
      this.commonHttpService.getUserList().then((res: any) => {
        let arr = res.data;
        const uniqueArr = Array.from(
          new Map(arr.map((item: any) => [item['id'], item])).values()
        );
        this.userList = uniqueArr;
        try {
          sessionStorage.setItem('userList', JSON.stringify(this.userList));
        } catch (e) {
          console.error(e)
        }

        this.loading = false;
        if (this.titleName != 'add') {
          this.userList.forEach((item: any) => {
            if (item.userId == this.userId) {
              this.userSelectList.push({
                id: item.userId,
                username: item.name,
              });
            }
          });
          this.userSelectList = Array.from(
            new Map(this.userSelectList.map(item => [item['id'], item])).values()
          );
        }
      });
    }
  }

  onPanelHide(e: any) {
    this.userList.sort((a: any, b: any) => {
      let aIndex = this.queryParams.id.indexOf(a.userId) > -1 ? -1 : 0;
      let bIndex = this.queryParams.id.indexOf(b.userId) > -1 ? -1 : 0;
      return aIndex - bIndex;
    });
  }

  onChangeUser(e: any) {
    if (!e.value) return;
    let arr: any[] = e.value;
    this.userList.forEach((item: any) => {
      arr.forEach((citem: any) => {
        if (item.userId == citem) {
          this.userSelectList.push({
            id: item.userId,
            username: item.name,
          });
        }
      });
    });
    this.userSelectList = Array.from(
      new Map(this.userSelectList.map(item => [item['id'], item])).values()
    );
  }

  searchNode(node: any, nodeId: string) {
    if (node.key === nodeId) {
      this.selectedFile.push(node);
    } else if (node.children !== undefined) {
      for (let i = 0; i < node.children.length; i++) {
        this.searchNode(node.children[i], nodeId);
      }
    }
  }
}
