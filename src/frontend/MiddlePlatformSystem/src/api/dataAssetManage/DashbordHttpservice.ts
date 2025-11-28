import { Injectable } from '@angular/core';
import { HttpService } from 'jabil-bus-lib';
import { environment } from 'src/environments/environment';
@Injectable()
export default class DashbordHttpservice {
  constructor(private http: HttpService) { }
  baseUrl: string = '/gateway/eureka/data/metadata/assetCatalog';

  public GetAllDashbordForOwnerId(data?: any) {
    return this.http.request({
      url: environment.DashbordServer + '/api/Dashbord/getDashbordListForOwnerId',
      method: 'get',
      data,
    });
  }


   public GetDashbordListForColumnId(data?: any) {
    return this.http.request({
      url: environment.DashbordServer + '/api/Dashbord/getDashbordListForColumnId',
      method: 'post',
      data,
    });
  }


   public GetAllDashbord(data?: any) {
    return this.http.request({
      url: environment.DashbordServer + '/api/Dashbord/getDashbordList',
      method: 'get',
      data,
    });
  }

  public updateStatu(data?: any) {
    return this.http.request({
      url: environment.DashbordServer + '/api/Dashbord/updateStatu',
      method: 'post',
      data,
    });
  }

}
