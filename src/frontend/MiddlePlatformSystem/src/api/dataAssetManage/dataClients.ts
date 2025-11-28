import { Injectable } from '@angular/core';
import { HttpService } from 'jabil-bus-lib';
import { environment } from 'src/environments/environment';

@Injectable()
export default class DataClientsHttpService {
  constructor(private http: HttpService) { }
  public getById(id: string) {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + `/api/clients/${id}`,
      method: 'get',
    }, header);
  }

  public save(data?: any) {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + '/api/clients/',
      method: 'post',
      data,
    }, header);
  }

  public update(id: string, data?: any) {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + `/api/clients/${id}`,
      method: 'put',
      data,
    }, header);
  }
  public PageList(data?: any) {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + '/api/clients/PageList',
      method: 'POST',
      data,
    }, header);
  }

  public SelfAppList(appName?: string) {
    let data = {
      appName:appName,
    }
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + '/api/clients/SelfAppList',
      method: 'POST',
      data,
    }, header);
  }

  public page(data?: any) {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + '/api/clients/page',
      method: 'POST',
      data,
    }, header);
  }

  public list(data?: any) {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + '/api/clients/list',
      method: 'POST',
      data,
    }, header);
  }

  public delete(id: string) {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + `/api/clients/${id}`,
      method: 'delete',
    }, header);
  }


  public AllAppList(appName?: string) {
    let data = {
      appName:appName??"",
    }
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + '/api/clients/AllAppList',
      method: 'get',
      data,
    }, header);
  }

  public PageSelfList(data?: any) {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + '/api/clients/PageSelfList',
      method: 'POST',
      data,
    }, header);
  }

}
