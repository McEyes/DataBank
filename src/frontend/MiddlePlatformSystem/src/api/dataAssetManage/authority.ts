import { Injectable } from '@angular/core';
import { HttpService } from 'jabil-bus-lib';
import { environment } from 'src/environments/environment';

@Injectable()
export default class AuthorityService {
  constructor(private http: HttpService) { }

  baseUrl: string = '/gateway/eureka/data/metadata/authorizes';

  public applyAPIColumn(data: any) {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + `/api/AuditApply/applyauth`,
      method: 'post',
      data
    }, header);
  }

  public checkAuth(data: any) {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + `/api/AuditApply/checkauth`,
      method: 'post',
      data
    }, header);
  }

  public userAuthList(data?: any) {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + `/api/authorizes/userAuthList`,
      method: 'get',
      data,
    }, header);
  }

  public deleteUserAuth(id: string) {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + `/api/authorizes/${id}`,
      method: 'delete',
    }, header);
  }

  public saveAuth(data?: any) {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + `/api/authorizes/saveAuth`,
      method: 'post',
      data,
    }, header);
  }

  public getRecordPage(data?: any) {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + `/api/Clients/pagelist`,
      method: 'post',
      data,
    }, header);
  }

  public getSelfRecordPage(data?: any) {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + `/api/Clients/PageSelfList`,
      method: 'post',
      data,
    }, header);
  }

  public updateRecord(data?: any) {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + `/api/auditRecord/updateRecord`,
      method: 'post',
      data,
    }, header);
  }

  // ----------------------------------

  public getAuth(id: String) {
    const header: any = {}
    return this.http.request({
      url: `${this.baseUrl}/getAuthorizedMetadata/${id}`,
      method: 'get',
    }, header);
  }

  public metadata(data?: any) {
    const header: any = {}
    return this.http.request({
      url: `${this.baseUrl}/metadata`,
      method: 'post',
      data,
    }, header);
  }

  public getAuthByName(name: String) {
    const header: any = {}
    return this.http.request({
      url: `${this.baseUrl}/getAuthorizedMetadataByRoleName/${name}`,
      method: 'get',
    }, header);
  }

  public refresh() {
    const header: any = {}
    return this.http.request({
      url: `${this.baseUrl}/refresh`,
      method: 'get',
    }, header);
  }

  public page(data?: any) {
    const header: any = {}
    return this.http.request({
      url: `${this.baseUrl}/page`,
      method: 'get',
      data,
    }, header);
  }

  public userAuthPage(data?: any) {
    const header: any = {}
    return this.http.request({
      url: `${this.baseUrl}/userAuthPage`,
      method: 'get',
      data,
    }, header);
  }

  public delete(id: string) {
    const header: any = {}
    return this.http.request({
      url: `${this.baseUrl}/${id}`,
      method: 'delete',
    }, header);
  }


}
