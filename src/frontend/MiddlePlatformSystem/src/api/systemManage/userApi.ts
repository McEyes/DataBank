import { Injectable } from '@angular/core';
import { HttpService } from 'jabil-bus-lib';
import { environment } from 'src/environments/environment';
@Injectable()
export default class UserHttpService {
  constructor(private http: HttpService) { }

  public getById(id: string) {
    const header: any = {}
    return this.http.request({
      url: environment.LoginBusServer + `/api/user/${id}`,
      method: 'get',
    }, header);
  }

  public save(data?: any) {
    const header: any = {}
    return this.http.request({
      url: environment.LoginBusServer + '/api/user/',
      method: 'post',
      data,
    }, header);
  }

  public update(data?: any) {
    const header: any = {}
    return this.http.request({
      url: environment.LoginBusServer + `/api/user`,
      method: 'put',
      data,
    }, header);
  }

  public page(data?: any) {
    const header: any = {}
    return this.http.request({
      url: environment.LoginBusServer + '/api/user/page',
      method: 'get',
      data,
    }, header);
  }

  public list(data?: any) {
    const header: any = {}
    return this.http.request({
      url: environment.LoginBusServer + '/api/user/list',
      method: 'get',
      data,
    }, header);
  }

  public delete(id: string) {
    const header: any = {}
    return this.http.request({
      url: environment.LoginBusServer + `/api/user/${id}`,
      method: 'delete',
    }, header);
  }


}
