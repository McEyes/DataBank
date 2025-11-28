import { Injectable } from '@angular/core';
import { HttpService } from 'jabil-bus-lib';
import { environment } from 'src/environments/environment';
@Injectable()
export default class SearchHttpService {
  constructor(private http: HttpService) { }

  public searchData(data: any) {
    const header: any = {}
    return this.http.request({
      url: environment.SearchServer + `/api/OpenSearch`,
      method: 'get',
      data
    }, header);
  }

  public getRecommendData(data: any) {
    const header: any = {}
    return this.http.request({
      url: environment.SearchServer + `/api/OpenSearch/candidatewords`,
      method: 'get',
      data
    }, header);
  }

  public getUserSearchHistory() {
    const header: any = {}
    return this.http.request({
      url: environment.SearchServer + `/api/UserSearchHistory`,
      method: 'get',
    }, header);
  }

  public deleteUserSearchHistory(id: string) {
    const header: any = {}
    return this.http.request({
      url: environment.SearchServer + `/api/UserSearchHistory/${id}`,
      method: 'delete',
    }, header);
  }
}
