import { Injectable } from '@angular/core';
import { HttpService } from 'jabil-bus-lib';
import { environment } from 'src/environments/environment';

@Injectable()
export default class ApiHttpService {
  constructor(private http: HttpService) { }

  public getById(id: string) {
    const header: any = {};
    return this.http.request(
      {
        url: environment.BasicServer + `/api/dataApis/${id}`,
        method: 'GET',
      },
      header
    );
  }

  /**
 *
 * @param id 接口 ID
 * @param action 动作：注册发布接口(release)、注销接口(cancel)，复制接口(copy)
 * @returns Pomise
 */
  public interfaceOperation(id: string, action: string) {
    const header: any = {};
    return this.http.request(
      {
        url: environment.BasicServer + `/api/dataApis/${id}/${action}`,
        method: 'POST',
      },
      header
    );
  }

  public list() {
    const header: any = {};
    return this.http.request(
      {
        url: environment.BasicServer + `/api/dataApis/list`,
        method: 'GET'
      },
      header
    );
  }

  public delete(id: string) {
    const header: any = {};
    return this.http.request(
      {
        url: environment.BasicServer + `/api/dataApis/${id}`,
        method: 'DELETE',
      },
      header
    );
  }

  public page(data?: any) {
    const header: any = {};
    return this.http.request(
      {
        url: environment.BasicServer + `/api/dataApis/page`,
        method: 'GET',
        data,
      },
      header
    );
  }

  public detailsApi(id: string) {
    const header: any = {};
    return this.http.request(
      {
        url: environment.BasicServer + `/api/dataApis/detail/${id}`,
        method: 'get',
      },
      header
    );
  }

  public sqlParse(data: any) {
    const header: any = {};
    return this.http.request(
      {
        url: environment.BasicServer + `/api/dataApis/sql/parse`,
        method: 'POST',
        data,
      },
      header
    );
  }

  public addOrUpdate(data?: any) {
    const suffix = data.id ? `/${data.id}` : '';
    const header: any = {};
    return this.http.request(
      {
        url: environment.BasicServer + `/api/dataApis${suffix}`,
        method: suffix ? 'PUT' : 'POST',
        data,
      },
      header
    );
  }

  public addSql(data?: any) {
    const header: any = {};
    return this.http.request(
      {
        url: environment.BasicServer + `/api/dataApis/fastSave`,
        method: 'POST',
        data,
      },
      header
    );
  }

  public dataPreview(tableId: string, pageSize?: number) {
    const header: any = {};
    return this.http.request(
      {
        url: environment.BasicServer + `/api/access/dataPreview/${tableId}`,
        method: 'GET',
        data: pageSize
      },
      header
    );
  }

}
