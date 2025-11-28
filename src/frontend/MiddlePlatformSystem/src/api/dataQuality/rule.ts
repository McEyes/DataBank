import { Injectable } from '@angular/core';
import { HttpService } from 'jabil-bus-lib';
import { environment } from 'src/environments/environment';
@Injectable()
export default class RuleHttpService {
  constructor(private readonly http: HttpService) { }

  public getAllRules(data:any) {
    const header: any = {}
    return this.http.request({
      url: environment.DataQualityServer + `/api/rules/all`,
      method: 'get',
      data
    }, header);
  }

  public getTableRules(data: any) {
    const header: any = {}
    return this.http.request({
      url: environment.DataQualityServer + `/api/tablerules/list`,
      method: 'get',
      data
    }, header);
  }

  public saveTableRules(data: any) {
    const header: any = {}
    return this.http.request({
      url: environment.DataQualityServer + `/api/tablerules/save`,
      method: 'post',
      data
    }, header);
  }

}
