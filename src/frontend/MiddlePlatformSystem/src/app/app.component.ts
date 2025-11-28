import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute,  } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { MessageService } from 'primeng/api';
import LoginHttpService from 'src/api/common/login'
import {
  CommonService,
  AppMenuService,
  LocalStorage,
  LoginService,
  TranslationApiService,
} from 'jabil-bus-lib';
import { debounceTime, fromEvent } from 'rxjs';
import { TranslateData } from './core/translate/translate-data';


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  providers: [
    LocalStorage,
    CommonService,
    AppMenuService,
    MessageService,
    LoginService,
    TranslationApiService,
    TranslateService,
    LoginHttpService,
  ],
})
export class AppComponent implements OnInit {
  @ViewChild('layout') layout: any | undefined;

  isLogin!: boolean;
  loading: boolean = false;
  // showPage: boolean = false;
  constructor(
    public route: ActivatedRoute,
    private commonService: CommonService,
    public messageService: MessageService,
    private translate: TranslateService,
  ) {
    commonService.translateData(TranslateData, translate);

  }

  public async ngOnInit() {
    // update translate cache data; keep same with jabil-bus-lib translateVersion
    localStorage.setItem('translateVersion', '1.6');
    this.init()

    setTimeout(() => {
      fromEvent(window, 'resize')
        .pipe(debounceTime(1000))
        .subscribe(res => {
          this.initScale()
        });
      this.initScale()
    }, 100)
  }

  async init() {
    let loginCheck = localStorage.getItem('loginCheck');
    this.isLogin = loginCheck === '1';
  }

  menuEvent(e: any) {
    // this.showPage = e !== '0'
  }

  initScale() {
    const SWidth = 1920
    const SHeight = 1080

    const width = Number(window.innerWidth)
    const height = Number(window.innerHeight)
    let scale = 1

    if (width / SWidth > height / SHeight) {
      scale = height / SHeight
    } else {
      scale = width / SWidth
    }

    if (scale < 0.75) {
      scale = 0.75
      // scale = Math.ceil(scale * 10) / 10
    }

    // if (width / SWidth < height / SHeight) {
    //   scale = height / SHeight
    // } else {
    //   scale = width / SWidth
    // }

    const scaleEle: any = document.getElementById('scale') || {}

    scaleEle.style.transform = `scale(${scale})`
    scaleEle.style.width = 100 / scale + '%'
    scaleEle.style.height = 100 / scale + '%'
    scaleEle.style.left = -(100 / scale - 100) / 2 + '%'
    scaleEle.style.top = -(100 / scale - 100) / 2 + '%'
  }
}
