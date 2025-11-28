import { Injectable } from '@angular/core';
import { HttpService } from 'jabil-bus-lib';
import { environment } from 'src/environments/environment';
@Injectable()
export default class RuleMyDataHttpService {
  constructor(private readonly http: HttpService) { }

  public addToEvaluate(data: any) {
    const header: any = {}
    return this.http.request({
      url: environment.DataQualityServer + `/api/qualityrating/addtoevaluate`,
      method: 'post',
      data
    }, header);
  }

  getTableData(data:any) {
    const header: any = {}
    return this.http.request({
      url: environment.DataQualityServer + `/api/assessment/tables`,
      method: 'post',
      data
    }, header);
  }

  getDepartments(data:any) {
    const header: any = {}
    return this.http.request({
      url: environment.DataQualityServer + `/api/common/departments`,
      method: 'get',
      data
    }, header);
  }

  getReport(data:any) {
    const header: any = {}
    return this.http.request({
      url: environment.DataQualityServer + `/api/assessment/report`,
      method: 'get',
      data
    }, header);
  }
}
