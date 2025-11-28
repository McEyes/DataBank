import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ConfirmationService, MessageService } from 'primeng/api';
import { TranslateData } from 'src/app/core/translate/translate-data';

@Component({
  selector: 'app-datatable-multi-level-approvers',
  templateUrl: './index.html',
  styleUrls: ['./index.scss'],
  providers: [
    ConfirmationService,
    MessageService
  ],
})
export class MultiLevleApproversComponent implements OnInit {
  @Output() goBack = new EventEmitter<string>();
  @Output() onPanelHideEvent = new EventEmitter<string>();
  @Input() field: any;
  @Input() userList: any;
  @Input() multiLevelApproversList: any;

  tableData: any[] = [];
  addMultiLevelApproversNum: number = 1;
  approverList: any = []
  constructor(
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
  ) { }

  ngOnInit(): void {
    this.addMultiLevelApproversNum = this.multiLevelApproversList.length;
    this.tableData = this.multiLevelApproversList.length > 0 ? this.multiLevelApproversList : [{ sort: 1, idList: [], nameList: '', multiLevelInvalid: false, id: 1 }]
  }

  goback() {
    this.goBack.emit()
  }

  // 新增
  onAddMultiLevelApprovers() {
    const maxSort = Math.max(...this.tableData.map(approver => approver.sort));
    this.addMultiLevelApproversNum += 1;
    this.tableData.unshift({
      sort: (maxSort == -Infinity ? 0 : maxSort) + 1,
      idList: [],
      nameList: '',
      multiLevelInvalid: false,
      id: this.addMultiLevelApproversNum,
    });
  }

  // 排序
  onSlotMultiLevelApprovers() {
    this.tableData.sort((a, b) => a.sort - b.sort);
  }

  // 删除
  ondeleteMultiLevelApprovers(event: any, info: any) {
    this.confirmationService.confirm({
      target: event.target,
      message: TranslateData.commonDelete,
      icon: 'pi pi-exclamation-triangle',
      rejectLabel: TranslateData.no,
      acceptLabel: TranslateData.yes,
      accept: () => {
        this.tableData = this.tableData.filter(
          (item: any) => item.sort != info.sort
        );
        this.messageService.add({
          severity: 'success',
          summary: TranslateData.delete,
          detail: TranslateData.success,
        });
      },
      reject: () => {
        this.messageService.add({
          severity: 'info',
          summary: TranslateData.delete,
          detail: TranslateData.cancel,
        });
      },
    });
  }

  onBlurSort(e: any, info: any) {
    let value: any = e.target.value;
    if (!value) return;
    let isHaveSort = this.tableData.some(
      (item: any) => Number(item.sort) == Number(value) && item.id != info.id
    );
    if (isHaveSort && this.tableData.length > 1) {
      info.sort = null;
      return this.messageService.add({
        severity: 'warn',
        summary: TranslateData.tips,
        detail: TranslateData.levelExists
      });
    }
  }

  onPanelHide(info: any) {
    this.onPanelHideEvent.emit(info);
  }

  onSumitMultiLevelApprovers() {
    let arr: any = [];
    let ishave = false;
    this.tableData.forEach((approver: any) => {
      approver.multiLevelInvalid = approver.idList.length === 0 || !approver.sort;
      if (approver.multiLevelInvalid) {
        ishave = true;
      }
      let list = approver.idList
        .map((id: any) => {
          let matchedUser = this.userList.find((user: any) => user.userId === id);
          return matchedUser ? { userName: matchedUser.name, userId: matchedUser.userId } : null;
        })
        .filter((user: any) => user !== null);
      if (list.length > 0) {
        arr.push({
          sort: approver.sort,
          userList: list,
        });
      }
    });
    if (ishave) {
      this.messageService.add({
        severity: 'warn',
        summary: TranslateData.tips,
        detail: TranslateData.fillLevelApprover
      });
      return false;
    } else {
      console.log(this.approverList)
      this.approverList = arr;
      return true;
    }
  }
}
