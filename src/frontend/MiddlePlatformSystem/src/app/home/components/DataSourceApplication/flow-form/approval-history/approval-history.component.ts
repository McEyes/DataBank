import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-approval-history',
  templateUrl: './approval-history.component.html',
  styleUrls: ['./approval-history.component.scss']
})
export class ApprovalHistoryComponent implements OnInit {
  @Input() historyList: any[];

  constructor() {
//status: wait process finish error
    this.historyList = [
            {
                "id": "f316cd28-cec4-4f94-8b94-02cc29886771",
                "flowInstId": "c554e608-501a-4dea-959a-75f15b2dedb8",
                "actId": "43a1bcb7-29ed-4195-a166-f81bd9e19919",
                "actStep": 1,
                "actTitle": "提交申请",
                "actName": "Start",
                "approver": "3419954",
                "approverName": "李阳辉",
                "actOperate": "Submit",
                "auditContent": "Submit",
                "remark": null,
                "transferor": null,
                "transferorName": null,
                "updateTime": "2025-06-25 22:22:51",
                "updateBy": "3419954",
                "createTime": "2025-06-25 22:22:51",
                "createBy": "3419954"
            },
            {
                "id": "4460d420-38b9-4bdd-b52e-459e3f21b9c0",
                "flowInstId": "c554e608-501a-4dea-959a-75f15b2dedb8",
                "actId": null,
                "actStep": 2,
                "actTitle": "主管审批",
                "actName": "Superior Approval",
                "approver": "System.Collections.Generic.List`1[ITPortal.Flow.Application.FlowTempAct.Dtos.StaffInfo]",
                "approverName": "Xie Tian(1296521)",
                "actOperate": "Running",
                "auditContent": null,
                "remark": null,
                "transferor": null,
                "transferorName": null,
                "updateTime": "2025-06-25 22:22:51",
                "updateBy": "3419954",
                "createTime": null,
                "createBy": "3419954"
            },
            {
                "id": "d3c75487-d6f8-45fa-9649-1b2542b8231b",
                "flowInstId": "c554e608-501a-4dea-959a-75f15b2dedb8",
                "actId": null,
                "actStep": 3,
                "actTitle": "SME审批",
                "actName": "SME Approval",
                "approver": "System.Collections.Generic.List`1[ITPortal.Flow.Application.FlowTempAct.Dtos.StaffInfo]",
                "approverName": "Li Yang Hui(3419954)",
                "actOperate": "",
                "auditContent": null,
                "remark": null,
                "transferor": null,
                "transferorName": null,
                "updateTime": "2025-06-25 22:22:51",
                "updateBy": "3419954",
                "createTime": null,
                "createBy": "3419954"
            },
            {
                "id": "d6679100-9d04-4c9f-9917-db4a20613509",
                "flowInstId": "c554e608-501a-4dea-959a-75f15b2dedb8",
                "actId": null,
                "actStep": 4,
                "actTitle": "BSA审批",
                "actName": "BSA Approval",
                "approver": "System.Collections.Generic.List`1[ITPortal.Flow.Application.FlowTempAct.Dtos.StaffInfo]",
                "approverName": "Li Yang Hui(3419954)",
                "actOperate": "",
                "auditContent": null,
                "remark": null,
                "transferor": null,
                "transferorName": null,
                "updateTime": "2025-06-25 22:22:51",
                "updateBy": "3419954",
                "createTime": null,
                "createBy": "3419954"
            },
            {
                "id": "8723946f-c268-4fec-9522-32004925789d",
                "flowInstId": "c554e608-501a-4dea-959a-75f15b2dedb8",
                "actId": null,
                "actStep": 5,
                "actTitle": "IT审批",
                "actName": "IT Approval",
                "approver": "System.Collections.Generic.List`1[ITPortal.Flow.Application.FlowTempAct.Dtos.StaffInfo]",
                "approverName": "李阳辉 IT",
                "actOperate": "",
                "auditContent": null,
                "remark": null,
                "transferor": null,
                "transferorName": null,
                "updateTime": "2025-06-25 22:22:51",
                "updateBy": "3419954",
                "createTime": null,
                "createBy": "3419954"
            },
            {
                "id": "be0172ca-cba8-4168-af87-4efdf4bb88c3",
                "flowInstId": "c554e608-501a-4dea-959a-75f15b2dedb8",
                "actId": null,
                "actStep": 6,
                "actTitle": "End",
                "actName": "End",
                "approver": "System.Collections.Generic.List`1[ITPortal.Flow.Application.FlowTempAct.Dtos.StaffInfo]",
                "approverName": "",
                "actOperate": "",
                "auditContent": null,
                "remark": null,
                "transferor": null,
                "transferorName": null,
                "updateTime": "2025-06-25 22:22:51",
                "updateBy": "3419954",
                "createTime": null,
                "createBy": "3419954"
            }
        ]

        // [
        //     {
        //         "id": "8580517b-2518-434e-9895-33b6312c98dc",
        //         "flowInstId": "4c5b20b4-5f1f-48c6-b047-b884c0ccab80",
        //         "actId": "971d86d5-e223-469a-bd7b-53e16653ddc0",
        //         "actStep": 1,
        //         "actTitle": "提交申请",
        //         "actName": "Start",
        //         "approver": "3419954",
        //         "approverName": "Yang Li9954(3419954)",
        //         "actOperate": "Submit",
        //         "auditContent": "Submit",
        //         "remark": null,
        //         "transferor": null,
        //         "transferorName": null,
        //         "updateTime": "2025-07-24 14:59:57",
        //         "updateBy": "3419954",
        //         "createTime": "2025-07-24 14:59:57",
        //         "createBy": "3419954"
        //     },
        //     {
        //         "id": "9704f265-c648-47cb-be84-9f0ed7b77dd4",
        //         "flowInstId": "4c5b20b4-5f1f-48c6-b047-b884c0ccab80",
        //         "actId": "af825bbe-bd84-489f-9df5-c720c05aba6f",
        //         "actStep": 2,
        //         "actTitle": "主管审批",
        //         "actName": "Superior Approval",
        //         "approver": "3419954",
        //         "approverName": "Yang Li9954(3419954)",
        //         "actOperate": "Cancel",
        //         "auditContent": "取消",
        //         "remark": null,
        //         "transferor": null,
        //         "transferorName": null,
        //         "updateTime": "2025-07-24 15:01:47",
        //         "updateBy": "3419954",
        //         "createTime": "2025-07-24 15:01:47",
        //         "createBy": "3419954"
        //     },
        //     {
        //         "id": "35cecc24-31e7-4822-9d7e-8d8c70ad7f29",
        //         "flowInstId": "4c5b20b4-5f1f-48c6-b047-b884c0ccab80",
        //         "actId": null,
        //         "actStep": 9,
        //         "actTitle": "End",
        //         "actName": "End",
        //         "approver": "",
        //         "approverName": "",
        //         "actOperate": "End",
        //         "auditContent": null,
        //         "remark": null,
        //         "transferor": null,
        //         "transferorName": null,
        //         "updateTime": "2025-07-24 15:01:47",
        //         "updateBy": "3419954",
        //         "createTime": "2025-07-24 15:01:47",
        //         "createBy": "3419954"
        //     }
        // ]
   }

  ngOnInit(): void {
  }

  getStatus(status: string) {
    status=(status||'').toLowerCase();
    // console.log(status);
    switch (status) {
      case '':
        return 'wait';
      case 'running':
        return 'process';
      case 'cancel':
      case 'error':
        return 'error';
      default:
        return 'finish';
    }
  }

}
