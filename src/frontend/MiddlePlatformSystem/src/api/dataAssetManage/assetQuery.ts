import { Injectable } from '@angular/core';
import { HttpService } from 'jabil-bus-lib';
import { environment } from 'src/environments/environment';
@Injectable()
export default class AssetQueryHttpService {
  constructor(private http: HttpService) { }
  baseUrl: string = '/gateway/eureka/data/metadata/assetCatalog';

  public getTree(key: String) {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + `/api/assetCatalog/tree/${key}`,
      method: 'get',
    }, header);
  }

  //
  public getTopicTable(data?: any) {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + `/api/tables/getTopicTable`,
      method: 'post',
      data,
    }, header);
  }


  public dataPreview(tableId: string, pageSize?: number, sortField?: string) {

    const header: any = {};
    return this.http.request(
      {
        url: environment.BasicServer + `/api/access/dataPreview/${tableId}`,
        data: {
          pageSize: pageSize,
          sort: sortField
        },
        method: 'GET',
      },
      header
    );
  }

  public page(data?: any) {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + `/api/columns/page`,
      method: 'get',
      data,
    }, header);
  }

  // 标签
  public getTagLists() {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + `/api/tables/getTableTag`,
      method: 'post',
    }, header);
  }

  getUserInfo(data?: any) {
    return this.http.request({
      url: environment.LoginBusServer + '/api/account/userinfo',
      method: 'get',
      data,
    });
  }

  // 中英文登录人名
  // useId
  userInfoExtend(data?: any) {
    return this.http.request({
      url: environment.LoginBusServer + '/api/Employee/info?WorkNTID=' + data.ntid,
      method: 'get',
    });
  }

  public checkAuth(data?: any) {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + `/api/auditRecord/checkAuth`,
      method: 'post',
      data,
    }, header);
  }

  public applyAuth(data?: any) {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + `/api/auditRecord/applyAuth`,
      method: 'post',
      data,
    }, header);
  }

  public feedBack(data?: any) {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + `/api/ApiFeedback`,
      method: 'post',
      data,
    }, header);
  }
  public userList() {
    const header: any = {}
    return this.http.request({
      url: `${this.baseUrl}/userList`,
      method: 'get',
    }, header);
  }

}
