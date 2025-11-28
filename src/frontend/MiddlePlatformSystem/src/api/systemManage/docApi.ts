import { Injectable } from '@angular/core';
import { HttpService } from 'jabil-bus-lib';
import { environment } from 'src/environments/environment';
@Injectable()
export default class DocHttpService {
  constructor(private readonly http: HttpService) { }

  public getById(id: string) {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + `/api/doc/${id}`,
      method: 'get',
    }, header);
  }

  public save(data?: any) {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + '/api/doc/',
      method: 'post',
      data,
    }, header);
  }

  public update(data?: any) {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + `/api/doc/${data.id}`,
      method: 'put',
      data,
    }, header);
  }

  public page(data?: any) {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + '/api/doc/page',
      method: 'post',
      data,
    }, header);
  }

  public list(data?: any) {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + '/api/doc/list',
      method: 'post',
      data,
    }, header);
  }

  public delete(id: string) {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + `/api/doc/${id}`,
      method: 'delete',
    }, header);
  }

  public getMasterType(data?: any) {
    const header: any = {
      'x-token': '77373d7e-ec37-4c61-9071-3c2d58966133'
    }
    return this.http.request({
      url: 'http://hua-databank.corp.JABIL.ORG/gateway/dataasset/services/v1.0.0/MASTER/DATA/masterdata_entity_map_kind/query?pagesize=999&pagenum=1',
      method: 'get',
      data,
    }, header);
  }


}
