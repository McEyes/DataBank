import { Injectable } from '@angular/core';
import { HttpService } from 'jabil-bus-lib';
import { environment } from 'src/environments/environment';

@Injectable()
export default class DataAccessLogHttpService {
  private gatewayUrl: string = '/gateway/eureka/data/api/apiLogs';

  constructor(private http: HttpService) { }

  public delete(id: string) {
    const header: any = {};
    return this.http.request(
      {
        url: environment.BasicServer + `/api/apiLogs/${id}`,
        method: 'DELETE',
      },
      header
    );
  }

  public page(data?: any) {
    const header: any = {};
    return this.http.request(
      {
        url: environment.BasicServer + `/api/apiLogs/page`,
        method: 'POST',
        data,
      },
      header
    );
  }

  public getById(id: string) {
    const header: any = {};
    return this.http.request(
      {
        url: environment.BasicServer + `/api/apiLogs/${id}`,
        method: 'GET',
      },
      header
    );
  }


  public save(data: any) {
    const header: any = {};
    return this.http.request(
      {
        url: environment.BasicServer + `/api/apiLogs`,
        method: 'POST',
        data,
      },
      header
    );
  }

  public update(data: any) {
    const header: any = {};
    return this.http.request(
      {
        url: environment.BasicServer + `/api/apiLogs`,
        method: 'PUT',
        data,
      },
      header
    );
  }


  // public getUsageStats() {
  //   const header: any = {};
  //   return this.http.request(
  //     {
  //       url: `${this.gatewayUrl}/getUsageStats/`,
  //       method: 'GET',
  //     },
  //     header
  //   );
  // }

  // public resquestMappingAdd(id: string) {
  //   const header: any = {};
  //   return this.http.request(
  //     {
  //       url: `${this.gatewayUrl}/registerMapping/${id}`,
  //       method: 'GET',
  //     },
  //     header
  //   );
  // }

  // public resquestMappingAdelete(id: string) {
  //   const header: any = {};
  //   return this.http.request(
  //     {
  //       url: `${this.gatewayUrl}/deleteMapping/${id}`,
  //       method: 'GET',
  //     },
  //     header
  //   );
  // }
}
