import { Injectable } from '@angular/core';
import { HttpService } from 'jabil-bus-lib';
import { environment } from 'src/environments/environment';

@Injectable()
export default class ApiLogsHttpService {
  private gatewayUrl: string = '/gateway/eureka/data/api/apiLogs';

  constructor(private http: HttpService) { }

  public apiFunc(apiVersion: string, apiUrl: string, headers: any, data: any, method: string = 'get') {
    const header: any = headers;
    console.log('header===', header);
    return this.http.request(
      {
        url: environment.BasicServer + `/services/${apiVersion}${apiUrl}?${data}`,
        method: method,
      },
      header
    );
  }
  public sqlFunc(apiVersion: string, apiUrl: string, headers: any, data: any) {
    const header: any = headers;
    return this.http.request(
      {
        url: environment.BasicServer + `/services/${apiVersion}${apiUrl}`,
        method: 'post',
        data,
      },
      header
    );
  }

  public resquestMappingAdd(id: string) {
    const header: any = {};
    return this.http.request(
      {
        url: environment.BasicServer + `/api/apiLogs/registerMapping/${id}`,
        method: 'GET',
      },
      header
    );
  }

  // ---------------------------------------------------------------------------------------

  public getUsageStats() {
    const header: any = {};
    return this.http.request(
      {
        url: `${this.gatewayUrl}/getUsageStats/`,
        method: 'GET',
      },
      header
    );
  }

  public resquestMappingAdelete(id: string) {
    const header: any = {};
    return this.http.request(
      {
        url: `${this.gatewayUrl}/deleteMapping/${id}`,
        method: 'GET',
      },
      header
    );
  }
}
