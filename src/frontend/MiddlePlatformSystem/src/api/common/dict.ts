import { Injectable } from '@angular/core';
import { HttpService } from 'jabil-bus-lib';
import { environment } from 'src/environments/environment';
@Injectable()
export default class DictHttpService {
  constructor(private http: HttpService) { }

  private preffix: string = environment.BasicServer + '/api/dicts';
  private itemPpreffix: string = environment.BasicServer + '/api/dict';

  public code(code: string) {
    const header: any = {}
    return this.http.request({
      url: `${this.preffix}/code/${code}`,
      method: 'get',
    }, header);
  }
  public codes(codes: string) {
    const header: any = {}
    return this.http.request({
      url: `${this.preffix}/codes/${encodeURIComponent(codes)}`,

      method: 'get',
    }, header);
  }


  public save(data?: any) {
    const header: any = {}
    return this.http.request({
      url: `${this.preffix}`,
      method: 'post',
      data,
    }, header);
  }

  public page(data?: any) {
    const header: any = {}
    return this.http.request({
      url: `${this.preffix}/page`,
      method: 'get',
      data,
    }, header);
  }


  public list(data?: any) {
    const header: any = {}
    return this.http.request({
      url: `${this.preffix}/list`,
      method: 'get',
      data,
    }, header);
  }

  public batchDelete(ids: Array<any>) {
    const header: any = {}
    var data = { ids: ids }
    return this.http.request({
      url: `${this.preffix}/batch/`,
      method: 'delete',
      data,
    }, header);
  }

  public update(id: string, data?: any) {
    const header: any = {}
    return this.http.request({
      url: `${this.preffix}/${id}`,
      method: 'put',
      data,
    }, header);
  }

  public delete(id: string) {
    const header: any = {}
    return this.http.request({
      url: `${this.preffix}/${id}`,
      method: 'delete',
    }, header);
  }

  // getDict(dict?: string) {
  //   return this.http.request({
  //     url: `/gateway/eureka/data/sys/dicts/code/${dict}`,
  //     method: 'get',
  //   });
  // }


  public getItemByCode(code: string) {
    const header: any = {}
    return this.http.request({
      url: `${this.itemPpreffix}/code/${code}`,
      method: 'get',
    }, header);
  }
  public getItemByCodes(codes: string) {
    const header: any = {}
    return this.http.request({
      url: `${this.itemPpreffix}/codes/${encodeURIComponent(codes)}`,
      method: 'get',
    }, header);
  }


  public saveItem(data?: any) {
    const header: any = {}
    return this.http.request({
      url: `${this.itemPpreffix}`,
      method: 'post',
      data,
    }, header);
  }

  public itemPage(data?: any) {
    const header: any = {}
    return this.http.request({
      url: `${this.itemPpreffix}/page`,
      method: 'get',
      data,
    }, header);
  }


  public itemList(data?: any) {
    const header: any = {}
    return this.http.request({
      url: `${this.itemPpreffix}/list`,
      method: 'get',
      data,
    }, header);
  }

  public itemBatchDelete(ids: Array<any>) {
    const header: any = {}
    var data = { ids: ids }
    return this.http.request({
      url: `${this.itemPpreffix}/batch/`,
      method: 'delete',
      data,
    }, header);
  }

  public itemUpdate(data?: any) {
    const header: any = {}
    return this.http.request({
      url: `${this.itemPpreffix}/`,
      method: 'put',
      data,
    }, header);
  }

  public itemDelete(id: string) {
    const header: any = {}
    return this.http.request({
      url: `${this.itemPpreffix}/${id}`,
      method: 'delete',
    }, header);
  }


}
