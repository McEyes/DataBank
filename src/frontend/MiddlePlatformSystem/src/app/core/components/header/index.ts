import { Component, OnInit, ViewChild, Input, Output, EventEmitter } from '@angular/core';
import { Router, ActivatedRoute, NavigationEnd } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { JBEventBusService, CommonService, AppMenuService } from 'jabil-bus-lib';
import { MessageService } from 'primeng/api';
import CommonHttpService from 'src/api/common'
import { map, filter } from 'rxjs/operators';
import { Location } from '@angular/common';

@Component({
  selector: 'jabil-header',
  templateUrl: './index.html',
  styleUrls: ['./index.scss'],
  providers: [JBEventBusService, CommonService, MessageService, AppMenuService],
})
export class JabilHeaderComponent implements OnInit {

  loading: boolean = false;
  currLang: string = '';
  curTab: string = '';
  userInfo: any = null;
  // hasPromise: boolean = false;
  dataAssetMenu: string[] = ['/dataAsset/userPermissionApplicationRecordComponent', '/dataAsset/permissionApplicationRecord', '/dataAsset/dataAccessLog', '/dataAsset/dataAuthority', '/dataAsset/dataAuthorized', '/dataAsset/sqlQuery', '/dataAsset/assetQuery', '/dataAsset/dataSource', '/dataAsset/dataTable', '/dataAsset/topicDomainDefinition', '/dataAsset/api']
  adminMenu: string[] = ['/system/user', '/system/role', '/system/application']
  dataQualityMenu: string[] = ['/dataQuality/rule', '/dataQuality/myData']
  dataTopicMenu: string[] = ['/dataTopicStore/create', '/dataTopicStore/myApply', '/dataTopicStore/myData', '/dataTopicStore/page', '/dataTopicStore/record', '/dataTopicStore/permission']
  isFull: boolean = false
  showSearch: boolean = false
  searchLoading: boolean = false
  keyword: string = ''
  searchResults: Array<any> = [];
  adminMenuList: any = [
    {
      title: 'menu.system.user',
      route: '/system/user',
      role: ['ADMIN']
    },
    {
      title: 'menu.system.role',
      route: '/system/role',
      role: ['ADMIN']
    },
    {
      title: 'menu.system.application',
      route: '/system/application',
      role: ['ADMIN']
    },
    {
      title: 'menu.system.doc',
      route: '/system/doc',
      role: ['ADMIN']
    },
    {
      title: 'menu.system.dict',
      route: '/system/dict',
      role: ['ADMIN']
    }
  ];
  dataQualityMenuList: any = [
    {
      title: 'menu.data.quality.rule',
      route: '/dataQuality/rule',
      role: ['ADMIN']
    },
    {
      title: 'menu.data.quality.my.data',
      route: '/dataQuality/myData',
      role: ['ADMIN']
    }
  ]
  dataAssetMenuList: any = [
    {
      title: 'menu.data.asset.query',
      route: '/dataAsset/assetQuery',
      role: ['DEFAULTROLE', 'ADMIN', 'BSAROLE','DASHBORD']
    },
    {
      title: 'menu.data.source',
      route: '/dataAsset/dataSource',
      role: ['ADMIN', 'BSAROLE']
    },
    {
      title: 'menu.data.table',
      route: '/dataAsset/dataTable',
      role: ['ADMIN', 'BSAROLE']
    },
    {
      title: 'menu.topic.domain.definition',
      route: '/dataAsset/topicDomainDefinition',
      role: ['ADMIN', 'BSAROLE']
    },
    {
      title: 'menu.api',
      route: '/dataAsset/api',
      role: ['ADMIN', 'BSAROLE']
    },
    {
      title: 'menu.sql.query',
      route: '/dataAsset/sqlQuery',
      role: ['ADMIN']
    },
    {
      title: 'menu.data.authorized',
      route: '/dataAsset/dataAuthorized',
      role: ['DEFAULTROLE', 'ADMIN', 'BSAROLE','DASHBORD']
    },
    {
      title: 'menu.data.authority',
      route: '/dataAsset/dataAuthority',
      role: ['ADMIN']
    },
    {
      title: 'menu.data.access.log',
      route: '/dataAsset/dataAccessLog',
      role: ['ADMIN', 'BSAROLE']
    },
    {
      title: 'menu.permission.application.record',
      route: '/dataAsset/permissionApplicationRecord',
      role: ['ADMIN']
    },
    {
      title: 'menu.user.permission.application.record',
      route: '/dataAsset/userPermissionApplicationRecordComponent',
      role: ['DEFAULTROLE', 'ADMIN', 'BSAROLE','DASHBORD']
    }
  ]
  dataTopicMenuList: any = [
    {
      title: 'menu.home',
      route: '/dataTopicStore/page',
      role: ['ADMIN','BSAROLE']
    },
    {
      title: 'menu.new.data.topic',
      route: '/dataTopicStore/create',
      role: ['ADMIN']
    },
    {
      title: 'menu.application.record',
      route: '/dataTopicStore/record',
      role: ['ADMIN','BSAROLE']
    },
    {
      title: 'menu.permission.approval',
      route: '/dataTopicStore/permission',
      role: ['ADMIN','BSAROLE']
    },
    {
      title: 'menu.my.data.application',
      route: '/dataTopicStore/myData',
      role: ['ADMIN','BSAROLE']
    },
    {
      title: 'menu.my.permission.application',
      route: '/dataTopicStore/myApply',
      role: ['ADMIN','BSAROLE']
    },
    {
      title: 'menu.category.management',
      route: '/dataTopicStore/categoryManagement',
      role: ['ADMIN','BSAROLE']
    }
  ]
  dashboardMenuList: any = [
    {
      title: 'menu.dashboard.functionApiReport',
      route: '/dataAsset/dashboard/functionApiReport',
      role: ['ADMIN','DASHBOARD']
    },
    {
      title: 'menu.dashboard.indicatorReport',
      route: '/dataAsset/dashboard/indicatorReport',
      role: ['ADMIN','DASHBOARD']
    }
  ]
  constructor(
    private readonly translate: TranslateService,
    private readonly router: Router,
    private readonly route: ActivatedRoute,
    private readonly commonHttpService: CommonHttpService,
    private readonly activatedRoute: ActivatedRoute,
    private readonly location: Location,
  ) {
    this.currLang = localStorage.getItem('lang') || 'en'
    this.translate.use(localStorage.getItem('lang') || 'en');

    setTimeout(() => {
      this.getData()
    }, 200)

  }

  ngOnInit(): void {
    setTimeout(() => {
      // @ts-ignore
      const url = this.activatedRoute.snapshot['_routerState'].url || '/home/page'
      this.curTab = url.split('?')[0]
    }, 500)

    this.router.events
      .pipe(
        filter((event: any) => event instanceof NavigationEnd),
        map(() => this.route),
        map((route: any) => {
          while (route.firstChild) {
            route = route.firstChild; // Conditional if needed in scenario
          }
          return route;
        })
      )
      .subscribe((elem: any) => {
        console.log('route event')
        this.curTab = elem.snapshot._routerState.url
      });
  }

  async getData() {
    const res = await this.commonHttpService.getUserInfo();
    this.userInfo = res?.data
    // this.hasPromise = this.userInfo.roles.length > 0;
    localStorage.setItem('loginuser', JSON.stringify(res?.data));
    // if (this.hasPromise === false) {
    //   this.router.navigate(['/home/flowform'], { queryParams: { flowtempname: 'itportal_userpermissionapplication' } }).then(() => { });
    // }
  }

  logout() {
    localStorage.clear();
    window?.parent?.location?.reload()
    window.location.reload()
  }

  shortName(name: string) {
    if (!name) return '';

    const names = name.split(' ');
    if (names.length >= 2) {
      return names[0].charAt(0) + names[1].charAt(0)
    } else {
      return name.charAt(0) + name.charAt(1)
    }

  }

  switchLang() {
    const lang = this.currLang === 'en' ? 'zh' : 'en'
    localStorage.setItem('lang', lang);
    this.translate.use(localStorage.getItem('lang') || 'en');
    location.reload();
  }

  switchMenu(url: string) {
    window.dispatchEvent(
      new CustomEvent('resetPage', {
        detail: { data: {} },
      })
    );

    this.curTab = url

    this.router.navigate([url], {
      queryParams: {},
    });
  }

  help() {
    window.open('./assets/files/数杮资产目录系统用户擝作手册.pdf', '_blank')
  }

  screenOperate() {
    this.isFull = !this.isFull
    this.isFull ? document.documentElement.requestFullscreen() : document.exitFullscreen()
  }

  search() {
    const path = this.location.path()
    this.router.navigate(['common/search'], {
      state: { keyword: this.keyword, path },
    });
  }

  hasPermission(roles: string[]) {
    let reslut = false
    this.userInfo.roles.forEach((item: any) => {
      if (roles.includes(item)) {
        reslut = true
      }
    });
    return reslut
  }

  goHome() {
    this.router.navigate(['/home/page'])
  }
}
