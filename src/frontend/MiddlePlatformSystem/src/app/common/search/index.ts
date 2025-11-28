import { Component, OnInit, Sanitizer } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { Location } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import SearchHttpService from 'src/api/common/search';
import { DomSanitizer } from '@angular/platform-browser';

@Component({
  selector: 'app-search',
  templateUrl: './index.html',
  styleUrls: ['./index.scss'],
  providers: [SearchHttpService]
})
export class SearchComponent implements OnInit {
  currentLanguage: any = 'en';
  loading: boolean = false;
  userInfo: any = {};
  searchData: any = {
    keyword: '',
    first: 0,
    pageIndex: 1,
    pageSize: 10,
  };
  listData: Array<any> = [];
  searchList: Array<any> = [];
  historySearchList: Array<any> = [];
  recordCount: number = 0
  path: string = ''
  timeout: any = null

  topicObj: any = {}
  constructor(
    private translate: TranslateService,
    private http: SearchHttpService,
    private location: Location,
    private sanitizer: DomSanitizer,
    private router: Router,
    private route: ActivatedRoute,) { }

  public async ngOnInit() {
    this.currentLanguage = localStorage.getItem('lang');
    this.translate.use(this.currentLanguage);

    this.path = this.path ? this.path : history.state.path

    this.searchData.keyword = history.state.keyword

    if (this.searchData.keyword) {
      this.getSearchData()
    } else {
      this.getHistorySearchData()
    }

    // this.route.queryParams.subscribe((res: any) => {
    //   this.searchData.keyword = res.keyword
    //   this.path = this.path ? this.path : res.path
    // })
  }

  paginate(event: any) {
    this.searchData.pageIndex = event.page + 1;
    this.searchData.pageSize = event.rows;
    this.searchData.first = event.first;
    this.getSearchData(true);
    document.getElementsByClassName('search-list')[0].scrollTop = 0
  }

  async getSearchData(isPaginate?: boolean) {
    this.loading = true;
    if (!isPaginate) {
      this.searchData.first = 0;
      this.searchData.pageIndex = 1;
    }

    const res = await this.http.searchData(this.searchData);
    this.listData = res?.data?.data || [];
    this.listData?.forEach((item: any) => {
      item.description = this.sanitizer.bypassSecurityTrustHtml(item.description)
    });

    this.buildTopicData(res?.data?.topics)
    this.getHistorySearchData()
    this.recordCount = res?.data?.total;
    this.loading = false;
  }

  buildTopicData(topics: any[]) {
    this.topicObj = {}
    topics?.forEach((item: any)=>{
      this.topicObj[item.topic] = item.name
    })
  }

  search() {
    setTimeout(() => {
      this.getSearchData();
    }, 500)
  }

  enterSearch(event: any) {
    if (event.keyCode === 13) {
      this.search();
    }
  }

  async deleteHistroySearch(e: any, id: string) {
    e.stopPropagation()
    e.preventDefault()
    await this.http.deleteUserSearchHistory(id)
    this.getHistorySearchData()
  }

  async getHistorySearchData() {
    const res = await this.http.getUserSearchHistory()
    this.searchList = res?.data || []
    this.historySearchList = res?.data || []
  }

  focusInput() {
    this.searchList = []
    if (!this.searchData.keyword) {
      this.searchList = JSON.parse(JSON.stringify(this.historySearchList))
    } else {
      this.getRecommendData()
    }
  }

  onInput(event: Event): void {
    if (this.timeout !== null) {
      clearTimeout(this.timeout);
    }

    this.timeout = setTimeout(() => {
      this.getRecommendData();
    }, 500);
  }

  async getRecommendData() {
    if (!this.searchData.keyword) {
      this.searchList = JSON.parse(JSON.stringify(this.historySearchList));
      return
    }
    const res = await this.http.getRecommendData({
      keyword: this.searchData.keyword
    })

    this.searchList = res.data
  }

  goDetail(item: any) {

    if (item.linkUrl) {
      // location.href = item.linkUrl
      window.open(item.linkUrl, '_blank')
    } else {
      console.warn('no link')
    }

    // location.href = '/#/dataAsset/api?id=0708602692863397888&type=link&edit=0'

    // this.router.navigate(['/dataAsset/api'], {
    //   queryParams: {id: '0708602692863397888', type: 'link', edit: '0'},
    // });

    // if (item.linkUrl) {
    //   this.currPanel = 'detail'
    //   this.currLinkType = 3
    //   this.appUrl = this.sanitizer.bypassSecurityTrustResourceUrl(item.linkUrl);
    //   return
    // }
    // this.currPanel = 'detail'
    // this.currLinkType = 1
    // this.currHtml = this.sanitizer.bypassSecurityTrustHtml(item.content)
  }

  goBack() {
    if (!this.path) {
      this.router.navigate(['/home/page'])
      return
    }

    this.router.navigate([this.path])
    this.path = ''

    // this.location.back()
  }
}
