import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { HttpService } from 'jabil-bus-lib';
import { environment } from 'src/environments/environment';
@Injectable()
export default class LoginHttpService {
  constructor(private http: HttpService, private httpClient: HttpClient) { }

  public login(data?: any) {
    return this.http.request({
      url: environment.LoginBusServer + '/api/account/login-ad',
      method: 'post',
      data,
    });
  }

  // public getOKTAUrl(data?: any) {
  //   return this.http.request({
  //     url: environment.JabilBusServer + '/gateway/basic/okta/GetAuthorizeUrl',
  //     method: 'post',
  //     data,
  //   });
  // }

  // public getOKTAToken(data?: any) {
  //   return this.http.request({
  //     url: environment.JabilBusServer + '/gateway/basic/okta/GetAccessToken',
  //     method: 'post',
  //     data,
  //   });
  // }

  // public getUserToken(data?: any) {
  //   return this.http.request({
  //     url: 'http://cnhuam0webstg85:6103' + '/api/account/getaduserinfo',
  //     method: 'get',
  //     data,
  //   });
  // }

  getUserToken() {
    const httpOptions = {
      withCredentials: true, // 重要：启用凭证
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
      })
    };

    return this.httpClient.get('http://cnhuam0webstg85:6103/api/account/getaduserinfo', httpOptions);
  }
}
