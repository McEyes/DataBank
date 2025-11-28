import { Injectable } from '@angular/core';
import { HttpService } from 'jabil-bus-lib';
import { environment } from 'src/environments/environment';

@Injectable()
export default class DataColumnHttpService {
  constructor(private http: HttpService) { }

  baseUrl: string = '/gateway/eureka/data/metadata/columns';

  public save(data?: any) {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + '/api/columns',
      method: 'post',
      data,
    }, header);
  }

  public page(data?: any) {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + '/api/columns/page',
      method: 'get',
      data,
    }, header);
  }

  public list(data?: any) {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + `/api/columns/list`,
      method: 'get',
      data,
    }, header);
  }

  public delete(id: string) {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + `/api/columns/${id}`,
      method: 'delete',
    }, header);
  }

  public update(id: string, data?: any) {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + `/api/columns/${id}`,
      method: 'put',
      data,
    }, header);
  }



  // ------------------------------------------------------------
  public getById(id: string) {
    const header: any = {}
    return this.http.request({
      url: `${this.baseUrl}/${id}`,
      method: 'get',
    }, header);
  }

  public batchDelete(ids: Array<any>) {
    const header: any = {}
    return this.http.request({
      url: `${this.baseUrl}/batch/${ids}`,
      method: 'delete',
    }, header);
  }

  public tree(level: string, data?: any) {
    const header: any = {}
    return this.http.request({
      url: `${this.baseUrl}/tree/${level}`,
      method: 'get',
      data,
    }, header);
  }



  public getCount() {
    const header: any = {}
    return this.http.request({
      url: `${this.baseUrl}/getCount`,
      method: 'get',
    }, header);
  }
}
