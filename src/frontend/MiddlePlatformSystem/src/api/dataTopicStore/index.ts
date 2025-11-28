import { Injectable } from '@angular/core';
import { HttpService } from 'jabil-bus-lib';
import { environment } from 'src/environments/environment';
@Injectable()
export default class DataTopicStoreHttpService {
  constructor(private readonly http: HttpService) { }

  public getCategoryTree(data?: any) {
    return this.http.request({
      url: environment.DataTopicStoreServer + '/api/category/tree-nodes',
      method: 'get',
      data,
    });
  }

  public saveCategoryTree(data?: any) {
    return this.http.request({
      url: environment.DataTopicStoreServer + '/api/category/save',
      method: 'post',
      data,
    });
  }

  public deleteCategoryTree(id: string) {
    return this.http.request({
      url: environment.DataTopicStoreServer + '/api/category/' + id,
      method: 'delete',
      data: {},
    });
  }

  public getTopicDetail(id: string) {
    return this.http.request({
      url: environment.DataTopicStoreServer + '/api/topic/details/' + id,
      method: 'get',
      data: {},
    });
  }

  public getTopicData(data: any) {
    return this.http.request({
      url: environment.DataTopicStoreServer + '/api/topic/paging-list',
      method: 'get',
      data,
    });
  }

  // home
  public getTopicRank(data: any) {
    return this.http.request({
      url: environment.DataTopicStoreServer + '/api/topic/ranking',
      method: 'get',
      data,
    });
  }

  public createDataTopic(data: any) {
    return this.http.request({
      url: environment.DataTopicStoreServer + '/api/topic/newdatatopic',
      method: 'post',
      data,
    });
  }

  public like(data: any) {
    return this.http.request({
      url: environment.DataTopicStoreServer + '/api/topic/like',
      method: 'post',
      data,
    });
  }

  public rating(data: any) {
    return this.http.request({
      url: environment.DataTopicStoreServer + '/api/topic/rating',
      method: 'post',
      data,
    });
  }

  public favorite(data: any) {
    return this.http.request({
      url: environment.DataTopicStoreServer + '/api/topic/favorite',
      method: 'post',
      data,
    });
  }

  public applyPermission(data: any) {
    return this.http.request({
      url: environment.DataTopicStoreServer + '/api/topic-permission/apply',
      method: 'post',
      data,
    });
  }

  public approvePermission(data: any) {
    return this.http.request({
      url: environment.DataTopicStoreServer + '/api/topic-permission/approve',
      method: 'post',
      data,
    });
  }

  public createComment(data: any) {
    return this.http.request({
      url: environment.DataTopicStoreServer + '/api/topic/comment',
      method: 'post',
      data,
    });
  }

  public getCommentList(data: any) {
    return this.http.request({
      url: environment.DataTopicStoreServer + '/api/topic-comments/list',
      method: 'get',
      data,
    });
  }

  public setCommentLike(data: any) {
    return this.http.request({
      url: environment.DataTopicStoreServer + '/api/topic-comments/like',
      method: 'post',
      data,
    });
  }

  public getTopicDraftData(data: any) {
    return this.http.request({
      url: environment.DataTopicStoreServer + '/api/topic-mgr/topic-draft-paging-list',
      method: 'get',
      data,
    });
  }

  public getApprovalList(data: any) {
    return this.http.request({
      url: environment.DataTopicStoreServer + '/api/topic-permission/approval-paging-list',
      method: 'post',
      data,
    });
  }
  
  public editTopicData(data: any) {
    return this.http.request({
      url: environment.DataTopicStoreServer + '/api/topic-mgr/save',
      method: 'post',
      data,
    });
  }

  public getTopicDetailData(id: string) {
    return this.http.request({
      url: environment.DataTopicStoreServer + '/api/topic-mgr/topic-draft-details/' + id,
      method: 'get',
      data: {},
    });
  }

  public setParametersInput(data: any) {
    return this.http.request({
      url: environment.DataTopicStoreServer + '/api/topic-mgr/set-parameters-input',
      method: 'post',
      data,
    });
  }

  public setParametersOutput(data: any) {
    return this.http.request({
      url: environment.DataTopicStoreServer + '/api/topic-mgr/set-parameters-output',
      method: 'post',
      data,
    });
  }

  public setVerificationPassed(data: any) {
    return this.http.request({
      url: environment.DataTopicStoreServer + '/api/topic-mgr/set-verification-passed',
      method: 'post',
      data,
    });
  }

  public setVerificationFailure(data: any) {
    return this.http.request({
      url: environment.DataTopicStoreServer + '/api/topic-mgr/set-verification-failure',
      method: 'post',
      data,
    });
  }

  public saveITDevelopRecords(data: any) {
    return this.http.request({
      url: environment.DataTopicStoreServer + '/api/topic-mgr/save-it-develop-records',
      method: 'post',
      data,
    });
  }

  public publish(data: any) {
    return this.http.request({
      url: environment.DataTopicStoreServer + '/api/topic-mgr/publish',
      method: 'post',
      data,
    });
  }

  // 权限申请列表
  public getMyTopicPermissionList(data: any) {
    return this.http.request({
      url: environment.DataTopicStoreServer + '/api/topic-mgr/my-topic-paging-list',
      method: 'get',
      data,
    });
  }

  // 数据申请列表
  public getMyTopicDataList(data: any) {
    return this.http.request({
      url: environment.DataTopicStoreServer + '/api/topic-mgr/my-topic-draft-paging-list',
      method: 'get',
      data,
    });
  }

  // public getTopicDetails(id: string) {
  //   return this.http.request({
  //     url: environment.DataTopicStoreServer + '/api/topic-mgr/topic-draft-details/' + id,
  //     method: 'get',
  //     data: {},
  //   });
  // }
  
  public verifyData(id: string) {
    return this.http.request({
      url: environment.DataTopicStoreServer + '/api/topic-mgr/verify/' + id,
      method: 'get',
      data: {},
    });
  }

  public previewData(id: string, data: any) {
    return this.http.request({
      url: environment.DataTopicStoreServer + '/api/topic/preview/' + id,
      method: 'get',
      data,
    });
  }

  public getLineageData(data: any) {
    return this.http.request({
      url: environment.DataTopicStoreServer + '/api/datalineage/lineage-data',
      method: 'get',
      data
    });
  }

  public getMyFavorite(url: any) {
    return this.http.request({
      url: environment.DataTopicStoreServer + '/api/topic/my-favorite-topics',
      method: 'get',
      data: {}
    });
  }

  
  public download(url?: string) {
    return this.http.request({
      url,
      method: 'download',
      responseType: 'blob',
      data: {}
    }, 'get');
  }

  public approveWorkflowPermission(data: any) {
    return this.http.request({
      url: environment.DataTopicStoreServer + '/api/topic-mgr/approve',
      method: 'post',
      data,
    });
  }
}
