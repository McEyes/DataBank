import { Injectable } from '@angular/core';
import { HttpService } from 'jabil-bus-lib';
import { environment } from 'src/environments/environment';
@Injectable()
export default class RoleHttpService {
  constructor(private http: HttpService) { }

  public getById(id: string) {
    const header: any = {}
    return this.http.request({
      url: environment.LoginBusServer + `/api/role/${id}`,
      method: 'get',
    }, header);
  }

  public save(data?: any) {
    const header: any = {}
    return this.http.request({
      url: environment.LoginBusServer + '/api/role/',
      method: 'post',
      data,
    }, header);
  }

  public update(data?: any) {
    const header: any = {}
    return this.http.request({
      url: environment.LoginBusServer + `/api/role`,
      method: 'put',
      data,
    }, header);
  }

  public page(data?: any) {
    const header: any = {}
    return this.http.request({
      url: environment.LoginBusServer + '/api/role/page',
      method: 'get',
      data,
    }, header);
  }

  public list(data?: any) {
    const header: any = {}
    return this.http.request({
      url: environment.LoginBusServer + '/api/role/list',
      method: 'get',
      data,
    }, header);
  }

  public delete(id: string) {
    const header: any = {}
    return this.http.request({
      url: environment.LoginBusServer + `/api/role/${id}`,
      method: 'delete',
    }, header);
  }



}
