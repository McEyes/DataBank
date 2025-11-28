import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MessageService } from 'primeng/api';
import LoginHttpService from 'src/api/common/login';
import { TranslateData } from 'src/app/core/translate/translate-data';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
  providers: [MessageService, LoginHttpService],
})
export class LoginComponent implements OnInit {
  username!: string;
  password!: string;
  clearCookie: string;
  loading: boolean = false;
  rememberPwd = false;
  pwdType = 'password';
  language = [
    {
      label: 'English',
      // icon: 'usa',
      command: (event: any) => {
        localStorage.setItem('lang', 'en');
        window.location.reload();
      },
    },
    {
      label: '中文',
      // icon: 'cn',
      command: (event: any) => {
        localStorage.setItem('lang', 'zh');
        window.location.reload();
      },
    },
  ];
  constructor(
    public httpService: LoginHttpService,
    private router: Router,
    private messageService: MessageService,
  ) { }

  public async ngOnInit() {
    if (localStorage.getItem('rememberPwd') === '1') {
      this.rememberPwd = true;
      try {
        this.username = window.atob(localStorage.getItem('usernameText') || '');
        this.password = window.atob(localStorage.getItem('password') || '');
      } catch (e) {
        this.rememberPwd = false;
        this.username = '';
        this.password = '';
        this.savePwd();
      }
    }
  }

  winLogin(event: any) {
    if (event.keyCode === 13) {
      this.login();
    }
  }

  // async oktaLogin() {
  //   const { data } = await this.httpService.getOKTAUrl();

  //   if (data.url !== '') {
  //     window.location.href = data.url;
  //   }
  // }

  async login() {
    // if(this.clearCookie){
    //   localStorage.clear();
    // }
    if (this.username && this.password) {
      this.loading = true;
      let formData = {
        name: this.username,
        password: this.password,
      };
      let res: any = await this.httpService.login(formData);

      this.loading = false;
      if (!res.success) {
        this.messageService.add({
          severity: 'error',
          summary: res.msg,
        });
        return;
      }
      this.savePwd();
      this.handleLoginSuccess(res.data);
    } else {
      this.messageService.add({
        severity: 'warn',
        summary: TranslateData.psdNameInvalid,
      });
    }
  }

  async handleLoginSuccess(data: any) {
    localStorage.setItem('jwt', data.token);
    localStorage.setItem('loginCheck', '1');
    this.router.navigate(['/home/page']).then(() => {
      window.location.reload();
    });
  }

  savePwd() {
    if (this.rememberPwd) {
      localStorage.setItem('rememberPwd', '1');
      localStorage.setItem('usernameText', window.btoa(this.username));
      localStorage.setItem('password', window.btoa(this.password));
    } else {
      localStorage.setItem('rememberPwd', '0');
      localStorage.setItem('usernameText', '');
      localStorage.setItem('password', '');
    }
  }
}
