import { Injectable } from '@angular/core';
import { HttpService } from 'jabil-bus-lib';
import { environment } from 'src/environments/environment';

@Injectable()
export default class TopicHttpService {
  constructor(private http: HttpService) { }

  baseUrl: string = '/gateway/eureka/data/metadata/assetCatalog';

  public list(data?: any) {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + `/api/assetCatalog/list`,
      method: 'get',
      data,
    }, header);
  }

  public page(data?: any) {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + `/api/assetCatalog/page`,
      method: 'get',
      data,
    }, header);
  }

  public saveMapping(data?: any) {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + `/api/assetCatalog/saveMapping`,
      method: 'post',
      data,
    }, header);
  }

  public delete(id: string) {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + `/api/assetCatalog/${id}`,
      method: 'delete',
    }, header);
  }

  public getById(id: string) {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + `/api/assetCatalog/${id}`,
      method: 'get',
    }, header);
  }

  public getTree(key: String) {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + `/api/assetCatalog/tree/${key}`,
      method: 'get',
    }, header);
  }

  public save(id: string, data?: any) {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + `/api/assetCatalog`,
      method: 'post',
      data,
    }, header);
  }


  public update(id: string, data?: any) {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + `/api/assetCatalog/${id}`,
      method: 'put',
      data,
    }, header);
  }
  // ------------------------------------------------------


  public batchDelete(ids: Array<any>) {
    const header: any = {}
    return this.http.request({
      url: `${this.baseUrl}/batch/${ids}`,
      method: 'delete',
    }, header);
  }




  public download(data?: any) {
    const header: any = {}
    return this.http.request({
      url: '/demo/demo/demo',
      method: 'download',
      responseType: 'blob',
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



  public hotCatalog(headerValue: any) {
    const header: any = {
      "Accept-Language": headerValue
    }
    return this.http.request({
      url: `${this.baseUrl}/hotCatalog`,
      method: 'get',
    }, header);
  }

}
