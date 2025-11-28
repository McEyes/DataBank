import { Component, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-redirect',
  templateUrl: './index.html',
  styleUrls: ['./index.scss'],
})
export class RedirectComponent implements OnInit {
  currentLanguage: any = 'en';
  loading: boolean = false;
  userInfo: any = {};

  constructor(private translate: TranslateService) {}

  public async ngOnInit() {
    this.currentLanguage = localStorage.getItem('lang');
    this.translate.use(this.currentLanguage);
  }
}
