import { Injectable } from '@angular/core';
import { HttpService } from 'jabil-bus-lib';
import { environment } from 'src/environments/environment';

@Injectable()
export default class DataSourceHttpService {
  constructor(private http: HttpService) { }
  public getById(id: string) {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + `/api/sources/${id}`,
      method: 'get',
    }, header);
  }

  public save(data?: any) {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + '/api/sources/',
      method: 'post',
      data,
    }, header);
  }

  public update(id: string, data?: any) {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + `/api/sources/${id}`,
      method: 'put',
      data,
    }, header);
  }

  public page(data?: any) {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + '/api/sources/page',
      method: 'get',
      data,
    }, header);
  }

  public list(data?: any) {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + '/api/sources/list',
      method: 'get',
      data,
    }, header);
  }

  public delete(id: string) {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + `/api/sources/${id}`,
      method: 'delete',
    }, header);
  }


  public checkConnection(data?: any) {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + `/api/sources/checkConnection`,
      method: 'post',
      data,
    }, header);
  }

// 本地保存过则获取保存的配置，没有就获取数据中表的所有字段集合
  public getSourceColumns(id: string, tableName: string) {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + `/api/sources/${id}/${tableName}/columns`,
      method: 'post',
    }, header);
  }
// 只返回有保存过的字段集合
  public getTableColumns(id: string, tableName: string) {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + `/api/sources/${id}/${tableName}/columns`,
      method: 'post',
    }, header);
  }
  //只返回数据原始表中的字段信息
  public getDbTableColumns(id: string, tableName: string) {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + `/api/sources/${id}/${tableName}/Dbcolumns`,
      method: 'post',
    }, header);
  }

  public refresh(data?: any) {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + '/api/sources/refresh',
      method: 'get',
      data,
    }, header);
  }

  public getDbTablesMergeLocal(id: string) {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + `/api/sources/${id}/getDbTablesMergeLocal`,
      method: 'get',
    }, header);
  }


  // public getDbTables(id: string) {
  //   const header: any = {}
  //   return this.http.request({
  //     url: `${this.baseUrl}/${id}/tables`,
  //     method: 'get',
  //   }, header);
  // }


  // public sync(id: string) {
  //   const header: any = {}
  //   return this.http.request({
  //     url: `${this.baseUrl}/sync/${id}`,
  //     method: 'post',
  //   }, header);
  // }



  // public getCount() {
  //   const header: any = {}
  //   return this.http.request({
  //     url: `${this.baseUrl}/getCount`,
  //     method: 'get',
  //   }, header);
  // }

}
