import { Injectable } from '@angular/core';
import { HttpService } from 'jabil-bus-lib';
import { environment } from 'src/environments/environment';
@Injectable()
export default class WorkflowHttpService {
  constructor(private http: HttpService) { }

  public getSelfTodoList(data?: any) {
    return this.http.request({
      url: environment.ITPortalFlowServer + '/api/Flow/todo/self',
      method: 'post',
      data,
    });
  }

  public getMyRequestsList(data?: any) {
    return this.http.request({
      url: environment.ITPortalFlowServer + '/api/Flow/myrequests',
      method: 'post',
      data,
    });
  }

  public getTodoList(data?: any) {
    return this.http.request({
      url: environment.ITPortalFlowServer + '/api/Flow/todo/self/page',
      method: 'post',
      data,
    });
  }

  // public getitdataSouceApproverList(data?: any) {
  //   return this.http.request({
  //     url: environment.ITPortalFlowServer + '/api/HomeReport/getcategorystats',
  //     method: 'get',
  //     data,
  //   });
  // }

  // public userList(data?: any) {
  //   return this.http.request({
  //     url: environment.LoginBusServer + '/api/emep/gettablestats',
  //     method: 'get',
  //     data,
  //   });
  // }

  public getFlowTemplateList(data?: any) {
    return this.http.request({
      url: environment.ITPortalFlowServer + '/api/FlowTemplate/list',
      method: 'post',
      data,
    });
  }

  public applyAuth(data?: any) {
    return this.http.request({
      url: environment.ITPortalFlowServer + '/api/DataSourceApply/applyauth',
      method: 'post',
      data,
    });
  }


  public startFlow(data?: any) {
    return this.http.request({
      url: environment.ITPortalFlowServer + '/api/Flow/start',
      method: 'post',
      data,
    });
  }

  public submitFlow(data: any, act: string) {
    return this.http.request({
      url: environment.ITPortalFlowServer + '/api/Flow/' + ((act.toLowerCase() == 'save'||act.toLowerCase() == 'draft') ? 'SaveDraft' : 'start'),
      method: 'post',
      data,
    });
  }



  public sendApprove(data?: any) {
    var approveUrl = environment.ITPortalFlowServer + '/api/Flow/Approval';
    data.actOperate = "Approval";
    if (data.acttype == 'return') {
      approveUrl = environment.ITPortalFlowServer + '/api/Flow/Reject';
      data.actOperate = "Reject";
    } else if (data.acttype == 'rejectstart') {
      approveUrl = environment.ITPortalFlowServer + '/api/Flow/RejectStart';
      data.actOperate = "RejectStart";
    } else if (data.acttype == 'reject') {
      approveUrl = environment.ITPortalFlowServer + '/api/Flow/RejectEnd';
      data.actOperate = "RejectEnd";
    } else if (data.acttype == 'cancel') {
      approveUrl = environment.ITPortalFlowServer + '/api/Flow/cancel';
      data.actOperate = "Cancel";
    } else if (data.acttype == 'approve' || data.acttype == 'agree') {
      approveUrl = environment.ITPortalFlowServer + '/api/Flow/Approval';
      data.actOperate = "Approval";
    } else if (data.acttype == 'transfer') {
      approveUrl = environment.ITPortalFlowServer + '/api/Flow/Transfer';
      data.actOperate = "Transfer";
    }else if (data.acttype == 'jump') {
      approveUrl = environment.ITPortalFlowServer + '/api/Flow/GotToAct';
      data.actOperate = "Jump";
    }
    return this.http.request({
      url: approveUrl,
      method: 'post',
      data,
    });
  }

  public itdataSouceDetails(flowid?: any) {
    return this.http.request({
      url: environment.ITPortalFlowServer + '/api/Flow/flowform/' + flowid,
      method: 'get',
    });
  }

  public getFlowFormDetails(id: string, type: string = 'flow') {
    if (type == "todo" || type == "task") return this.getFlowFormDetailsByTaskId(id);
    else return this.getFlowFormDetailsByFlowId(id);
  }

  public getFlowFormDetailsByFlowId(flowid?: any) {
    return this.http.request({
      url: environment.ITPortalFlowServer + '/api/Flow/flowform/' + flowid,
      method: 'get',
    });
  }

  public getFlowFormDetailsByTaskId(taskid?: any) {
    return this.http.request({
      url: environment.ITPortalFlowServer + '/api/Flow/flowform/todo/' + taskid,
      method: 'get',
    });
  }

  public itdataSouceCommit(data?: any) {
    return this.http.request({
      url: environment.BasicServer + '/api/DataSourceApply/applyauth',
      method: 'post',
      data,
    });
  }

  public itdataSouceUpdateAsync(data?: any) {
    return this.http.request({
      url: environment.ITPortalFlowServer + '/api/DataSourceApply/update',
      method: 'post',
      data,
    });
  }

  public itsaveOtherFile(data?: any) {
    return this.http.request({
      url: environment.ITPortalFlowServer + '/api/Flow/getstandardizationstats',
      method: 'post',
      data,
    });
  }

  public itexportList(data?: any) {
    return this.http.request({
      url: environment.ITPortalFlowServer + '/api/HomeReport/getstandardizationstats',
      method: 'post',
      data,
    });
  }

  public itexportOtherList(data?: any) {
    return this.http.request({
      url: environment.ITPortalFlowServer + '/api/HomeReport/getstandardizationstats',
      method: 'post',
      data,
    });
  }

  public getMasterByUserId(ntid?: any) {
    return this.http.request({
      url: environment.LoginBusServer + '/api/Employee/master/' + ntid,
      method: 'get',
    });
  }

  public getWorkflowList(data?: any) {
    return this.http.request({
      url: environment.ITPortalFlowServer + '/api/Flow/page',
      method: 'post',
      data,
    });
  }

  public getFlowTemplate(flowTempName?: string) {
    return this.http.request({
      url: environment.ITPortalFlowServer + '/api/FlowTemplate/Info/name/' + flowTempName,
      method: 'get'
    });
  }
}


export class FlowInstData {
  flowId: string;
  flowname: string;
  flowStatus: string;

}

// export interface FlowFormComponent {
//   flowId: string;
//   flowNo: string;
//   flowStatus: string;
//   flowInstData: any;
//   formData: any;
// }
