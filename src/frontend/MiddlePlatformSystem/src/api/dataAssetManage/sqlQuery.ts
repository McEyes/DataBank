import { Injectable } from '@angular/core';
import { HttpService } from 'jabil-bus-lib';
import { environment } from 'src/environments/environment';

@Injectable()
export default class SQLQueryHttpService {
  constructor(private http: HttpService) { }

  public query(data?: any) {
    const header: any = {};
    return this.http.request({
      url: environment.BasicServer + `/api/sql/run`,
      method: 'POST',
      data,
    }, header);
  }

  public stopQuery(data?: any) {
    const header: any = {}
    return this.http.request({
      url: environment.BasicServer + `/api/sql/stop`,
      method: 'POST',
      data,
    }, header);
  }

}
