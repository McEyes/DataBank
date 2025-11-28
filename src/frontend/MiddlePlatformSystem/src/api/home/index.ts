import { Injectable } from '@angular/core';
import { HttpService } from 'jabil-bus-lib';
import { environment } from 'src/environments/environment';
@Injectable()
export default class HomeHttpService {
  constructor(private http: HttpService) { }

  public getCategoryStats(data?: any) {
    return this.http.request({
      url: environment.BasicServer + '/api/HomeReport/getcategorystats',
      method: 'get',
      data,
    });
  }

  public getTableStats(data?: any) {
    return this.http.request({
      url: environment.BasicServer + '/api/HomeReport/gettablestats',
      method: 'get',
      data,
    });
  }

  public getStandardizationStats(data?: any) {
    return this.http.request({
      url: environment.BasicServer + '/api/HomeReport/getstandardizationstats',
      method: 'get',
      data,
    });
  }


  public getApiDailyStats(data?: any) {
    return this.http.request({
      url: environment.BasicServer + '/api/HomeReport/GetApiDailyStats',
      method: 'get',
      data,
    });
  }


  public getUserFlowView(data?: any) {
    return this.http.request({
      url: environment.BasicServer + '/api/HomeReport/getstandardizationstats',
      method: 'get',
      data,
    });
  }



}
