import { Injectable } from '@angular/core';
import { HttpService } from 'jabil-bus-lib';
import { environment } from 'src/environments/environment';

@Injectable()
export default class DataTableHttpService {
  constructor(private http: HttpService) { }

  baseUrl: string = '/gateway/eureka/data/metadata/sources';

  public page(data?: any) {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + `/api/tables/page`,
      method: 'get',
      data,
    }, header);
  }

  public delete(id: string) {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + `/api/tables/${id}`,
      method: 'delete',
    }, header);
  }

  public list(data?: any) {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + `/api/tables/list`,
      method: 'get',
      data,
    }, header);
  }

  public getById(id: string) {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + `/api/tables/${id}`,
      method: 'get',
    }, header);
  }

  public getTagLists() {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + `/api/tables/getTableTag`,
      method: 'post',
    }, header);
  }

  public getTablesByTopic(id: string) {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + `/api/tables/topic/${id}`,
      method: 'get',
    }, header);
  }

  public getTablesById(id: string) {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + `/api/tables/${id}`,
      method: 'get',
    }, header);
  }

  public save(data: any, id?: '') {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + `/api/tables`,
      method: 'post',
      data,
    }, header);
  }

  public update(data: any, id: string,) {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + `/api/tables`,
      method: 'put',
      data,
    }, header);
  }

  public getUserTable(data?: any) {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + `/api/tables/getUserTable`,
      method: 'post',
      data,
    }, header);
  }

  public getTablesByUserId(id: string) {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + `/api/tables/getTablesByUserId/${id}`,
      method: 'get',
    }, header);
  }

  public GetOwnerTables(data?: any) {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + `/api/tables/GetOwnerTables`,
      method: 'post',
      data,
    }, header);
  }

  // ------------------------------





  public batchDelete(ids: Array<any>) {
    const header: any = {}
    return this.http.request({
      url: `${this.baseUrl}/batch/${ids}`,
      method: 'delete',
    }, header);
  }





  public getSourceTable() {
    const header: any = {}
    return this.http.request({
      url: `${this.baseUrl}/getSourceTable`,
      method: 'get',
    }, header);
  }

  public getCount() {
    const header: any = {}
    return this.http.request({
      url: `${this.baseUrl}/getCount`,
      method: 'get',
    }, header);
  }


  public getTopicTable(data?: any) {
    const header: any = {}
    return this.http.request({
      url: `${this.baseUrl}/getTopicTable`,
      method: 'post',
      data,
    }, header);
  }
  public getTableByTag(data?: any) {
    const header: any = {}
    return this.http.request({
      url: `${this.baseUrl}/getTableByTag`,
      method: 'post',
      data,
    }, header);
  }
}
