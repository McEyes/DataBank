/**
 * name:login service
 * describe: login common module
 */
import { Injectable } from '@angular/core';
import * as i0 from "@angular/core";
export class LoginService {
    constructor() { }
    /**
     * request
     * @param params
     * @returns {Promise<{success: boolean,msg: string}>|Promise<R>}
     */
    initLogin(params) {
        // this.loginParams = params
    }
    // public checkLogin() {
    //   if (window.location.toString().indexOf('/common/redirect?code=') >= 0) {
    //     localStorage.setItem('loginCheck', '0');
    //     this.isRedirect = true;
    //     this.checkOKTALogin();
    //     return;
    //   }
    //   let loginCheck = localStorage.getItem('loginCheck');
    //   this.isLogin = loginCheck === '1';
    // }
    //
    // private checkOKTALogin() {
    //   this.route.queryParams.subscribe(async (data: any) => {
    //     let code = data['code'];
    //     if (code) {
    //       const { data } = await this.http.getOKTAToken({ code });
    //       if (data) {
    //         this.handleLoginSuccess(data);
    //       } else {
    //         this.messageService.add({ severity: 'error', summary: 'OKTA Login Fail' });
    //         this.isRedirect = false;
    //       }
    //     }
    //   });
    // }
    //
    // async handleLoginSuccess(data: any) {
    //   localStorage.setItem('jwt', data.token);
    //   localStorage.setItem('roles', data.roles);
    //   localStorage.setItem('loginCheck', '1');
    //   localStorage.setItem('username', data.userName);
    //   const menu = await this.appMenuService.getMenuData(this.menuTreeKeys, this.menuTree);
    //   const url = menu[0]?.items ? menu[0].items[0].routerLink : menu[0].routerLink;
    //   localStorage.setItem('defaultUrl', url);
    //   this.isLogin = true;
    //   this.isRedirect = false;
    //   this.router.navigate([url]).then(() => {});
    // }
    static logout() {
        localStorage.removeItem('loginCheck');
        localStorage.removeItem('username');
        localStorage.removeItem('selectedIndex');
        localStorage.removeItem('jwt');
        localStorage.removeItem('tabs');
        localStorage.removeItem('tabMenu');
        localStorage.removeItem('region');
        localStorage.removeItem('currentRegion');
        localStorage.removeItem('translateApp');
        localStorage.removeItem('zh');
        localStorage.removeItem('en');
        window.location.reload();
    }
}
LoginService.ɵfac = function LoginService_Factory(t) { return new (t || LoginService)(); };
LoginService.ɵprov = /*@__PURE__*/ i0.ɵɵdefineInjectable({ token: LoginService, factory: LoginService.ɵfac });
(function () { (typeof ngDevMode === "undefined" || ngDevMode) && i0.ɵsetClassMetadata(LoginService, [{
        type: Injectable
    }], function () { return []; }, null); })();
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoibG9naW4uc2VydmljZS5qcyIsInNvdXJjZVJvb3QiOiIiLCJzb3VyY2VzIjpbIi4uLy4uLy4uLy4uLy4uLy4uL3NyYy9hcHAvc2VydmljZS9sb2dpbi9sb2dpbi5zZXJ2aWNlLnRzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiJBQUFBOzs7R0FHRztBQUNILE9BQU8sRUFBRSxVQUFVLEVBQUUsTUFBTSxlQUFlLENBQUM7O0FBRzNDLE1BQU0sT0FBTyxZQUFZO0lBRXZCLGdCQUFlLENBQUM7SUFFaEI7Ozs7T0FJRztJQUNJLFNBQVMsQ0FBQyxNQUFXO1FBQzFCLDRCQUE0QjtJQUM5QixDQUFDO0lBR0Qsd0JBQXdCO0lBQ3hCLDZFQUE2RTtJQUM3RSwrQ0FBK0M7SUFDL0MsOEJBQThCO0lBQzlCLDZCQUE2QjtJQUM3QixjQUFjO0lBQ2QsTUFBTTtJQUNOLHlEQUF5RDtJQUN6RCx1Q0FBdUM7SUFDdkMsSUFBSTtJQUNKLEVBQUU7SUFDRiw2QkFBNkI7SUFDN0IsNERBQTREO0lBQzVELCtCQUErQjtJQUMvQixrQkFBa0I7SUFDbEIsaUVBQWlFO0lBQ2pFLG9CQUFvQjtJQUNwQix5Q0FBeUM7SUFDekMsaUJBQWlCO0lBQ2pCLHNGQUFzRjtJQUN0RixtQ0FBbUM7SUFDbkMsVUFBVTtJQUNWLFFBQVE7SUFDUixRQUFRO0lBQ1IsSUFBSTtJQUNKLEVBQUU7SUFDRix3Q0FBd0M7SUFDeEMsNkNBQTZDO0lBQzdDLCtDQUErQztJQUMvQyw2Q0FBNkM7SUFDN0MscURBQXFEO0lBQ3JELDBGQUEwRjtJQUMxRixtRkFBbUY7SUFDbkYsNkNBQTZDO0lBQzdDLHlCQUF5QjtJQUN6Qiw2QkFBNkI7SUFDN0IsZ0RBQWdEO0lBQ2hELElBQUk7SUFFRyxNQUFNLENBQUMsTUFBTTtRQUNsQixZQUFZLENBQUMsVUFBVSxDQUFDLFlBQVksQ0FBQyxDQUFDO1FBQ3RDLFlBQVksQ0FBQyxVQUFVLENBQUMsVUFBVSxDQUFDLENBQUM7UUFDcEMsWUFBWSxDQUFDLFVBQVUsQ0FBQyxlQUFlLENBQUMsQ0FBQztRQUN6QyxZQUFZLENBQUMsVUFBVSxDQUFDLEtBQUssQ0FBQyxDQUFDO1FBQy9CLFlBQVksQ0FBQyxVQUFVLENBQUMsTUFBTSxDQUFDLENBQUM7UUFDaEMsWUFBWSxDQUFDLFVBQVUsQ0FBQyxTQUFTLENBQUMsQ0FBQztRQUNuQyxZQUFZLENBQUMsVUFBVSxDQUFDLFFBQVEsQ0FBQyxDQUFDO1FBQ2xDLFlBQVksQ0FBQyxVQUFVLENBQUMsZUFBZSxDQUFDLENBQUM7UUFDekMsWUFBWSxDQUFDLFVBQVUsQ0FBQyxjQUFjLENBQUMsQ0FBQztRQUN4QyxZQUFZLENBQUMsVUFBVSxDQUFDLElBQUksQ0FBQyxDQUFDO1FBQzlCLFlBQVksQ0FBQyxVQUFVLENBQUMsSUFBSSxDQUFDLENBQUM7UUFDOUIsTUFBTSxDQUFDLFFBQVEsQ0FBQyxNQUFNLEVBQUUsQ0FBQztJQUMzQixDQUFDOzt3RUFsRVUsWUFBWTtrRUFBWixZQUFZLFdBQVosWUFBWTt1RkFBWixZQUFZO2NBRHhCLFVBQVUiLCJzb3VyY2VzQ29udGVudCI6WyIvKipcclxuICogbmFtZTpsb2dpbiBzZXJ2aWNlXHJcbiAqIGRlc2NyaWJlOiBsb2dpbiBjb21tb24gbW9kdWxlXHJcbiAqL1xyXG5pbXBvcnQgeyBJbmplY3RhYmxlIH0gZnJvbSAnQGFuZ3VsYXIvY29yZSc7XHJcblxyXG5ASW5qZWN0YWJsZSgpXHJcbmV4cG9ydCBjbGFzcyBMb2dpblNlcnZpY2Uge1xyXG5cclxuICBjb25zdHJ1Y3RvcigpIHt9XHJcblxyXG4gIC8qKlxyXG4gICAqIHJlcXVlc3RcclxuICAgKiBAcGFyYW0gcGFyYW1zXHJcbiAgICogQHJldHVybnMge1Byb21pc2U8e3N1Y2Nlc3M6IGJvb2xlYW4sbXNnOiBzdHJpbmd9PnxQcm9taXNlPFI+fVxyXG4gICAqL1xyXG4gIHB1YmxpYyBpbml0TG9naW4ocGFyYW1zOiBhbnkpOiBhbnkge1xyXG4gICAgLy8gdGhpcy5sb2dpblBhcmFtcyA9IHBhcmFtc1xyXG4gIH1cclxuXHJcblxyXG4gIC8vIHB1YmxpYyBjaGVja0xvZ2luKCkge1xyXG4gIC8vICAgaWYgKHdpbmRvdy5sb2NhdGlvbi50b1N0cmluZygpLmluZGV4T2YoJy9jb21tb24vcmVkaXJlY3Q/Y29kZT0nKSA+PSAwKSB7XHJcbiAgLy8gICAgIGxvY2FsU3RvcmFnZS5zZXRJdGVtKCdsb2dpbkNoZWNrJywgJzAnKTtcclxuICAvLyAgICAgdGhpcy5pc1JlZGlyZWN0ID0gdHJ1ZTtcclxuICAvLyAgICAgdGhpcy5jaGVja09LVEFMb2dpbigpO1xyXG4gIC8vICAgICByZXR1cm47XHJcbiAgLy8gICB9XHJcbiAgLy8gICBsZXQgbG9naW5DaGVjayA9IGxvY2FsU3RvcmFnZS5nZXRJdGVtKCdsb2dpbkNoZWNrJyk7XHJcbiAgLy8gICB0aGlzLmlzTG9naW4gPSBsb2dpbkNoZWNrID09PSAnMSc7XHJcbiAgLy8gfVxyXG4gIC8vXHJcbiAgLy8gcHJpdmF0ZSBjaGVja09LVEFMb2dpbigpIHtcclxuICAvLyAgIHRoaXMucm91dGUucXVlcnlQYXJhbXMuc3Vic2NyaWJlKGFzeW5jIChkYXRhOiBhbnkpID0+IHtcclxuICAvLyAgICAgbGV0IGNvZGUgPSBkYXRhWydjb2RlJ107XHJcbiAgLy8gICAgIGlmIChjb2RlKSB7XHJcbiAgLy8gICAgICAgY29uc3QgeyBkYXRhIH0gPSBhd2FpdCB0aGlzLmh0dHAuZ2V0T0tUQVRva2VuKHsgY29kZSB9KTtcclxuICAvLyAgICAgICBpZiAoZGF0YSkge1xyXG4gIC8vICAgICAgICAgdGhpcy5oYW5kbGVMb2dpblN1Y2Nlc3MoZGF0YSk7XHJcbiAgLy8gICAgICAgfSBlbHNlIHtcclxuICAvLyAgICAgICAgIHRoaXMubWVzc2FnZVNlcnZpY2UuYWRkKHsgc2V2ZXJpdHk6ICdlcnJvcicsIHN1bW1hcnk6ICdPS1RBIExvZ2luIEZhaWwnIH0pO1xyXG4gIC8vICAgICAgICAgdGhpcy5pc1JlZGlyZWN0ID0gZmFsc2U7XHJcbiAgLy8gICAgICAgfVxyXG4gIC8vICAgICB9XHJcbiAgLy8gICB9KTtcclxuICAvLyB9XHJcbiAgLy9cclxuICAvLyBhc3luYyBoYW5kbGVMb2dpblN1Y2Nlc3MoZGF0YTogYW55KSB7XHJcbiAgLy8gICBsb2NhbFN0b3JhZ2Uuc2V0SXRlbSgnand0JywgZGF0YS50b2tlbik7XHJcbiAgLy8gICBsb2NhbFN0b3JhZ2Uuc2V0SXRlbSgncm9sZXMnLCBkYXRhLnJvbGVzKTtcclxuICAvLyAgIGxvY2FsU3RvcmFnZS5zZXRJdGVtKCdsb2dpbkNoZWNrJywgJzEnKTtcclxuICAvLyAgIGxvY2FsU3RvcmFnZS5zZXRJdGVtKCd1c2VybmFtZScsIGRhdGEudXNlck5hbWUpO1xyXG4gIC8vICAgY29uc3QgbWVudSA9IGF3YWl0IHRoaXMuYXBwTWVudVNlcnZpY2UuZ2V0TWVudURhdGEodGhpcy5tZW51VHJlZUtleXMsIHRoaXMubWVudVRyZWUpO1xyXG4gIC8vICAgY29uc3QgdXJsID0gbWVudVswXT8uaXRlbXMgPyBtZW51WzBdLml0ZW1zWzBdLnJvdXRlckxpbmsgOiBtZW51WzBdLnJvdXRlckxpbms7XHJcbiAgLy8gICBsb2NhbFN0b3JhZ2Uuc2V0SXRlbSgnZGVmYXVsdFVybCcsIHVybCk7XHJcbiAgLy8gICB0aGlzLmlzTG9naW4gPSB0cnVlO1xyXG4gIC8vICAgdGhpcy5pc1JlZGlyZWN0ID0gZmFsc2U7XHJcbiAgLy8gICB0aGlzLnJvdXRlci5uYXZpZ2F0ZShbdXJsXSkudGhlbigoKSA9PiB7fSk7XHJcbiAgLy8gfVxyXG5cclxuICBwdWJsaWMgc3RhdGljIGxvZ291dCgpIHtcclxuICAgIGxvY2FsU3RvcmFnZS5yZW1vdmVJdGVtKCdsb2dpbkNoZWNrJyk7XHJcbiAgICBsb2NhbFN0b3JhZ2UucmVtb3ZlSXRlbSgndXNlcm5hbWUnKTtcclxuICAgIGxvY2FsU3RvcmFnZS5yZW1vdmVJdGVtKCdzZWxlY3RlZEluZGV4Jyk7XHJcbiAgICBsb2NhbFN0b3JhZ2UucmVtb3ZlSXRlbSgnand0Jyk7XHJcbiAgICBsb2NhbFN0b3JhZ2UucmVtb3ZlSXRlbSgndGFicycpO1xyXG4gICAgbG9jYWxTdG9yYWdlLnJlbW92ZUl0ZW0oJ3RhYk1lbnUnKTtcclxuICAgIGxvY2FsU3RvcmFnZS5yZW1vdmVJdGVtKCdyZWdpb24nKTtcclxuICAgIGxvY2FsU3RvcmFnZS5yZW1vdmVJdGVtKCdjdXJyZW50UmVnaW9uJyk7XHJcbiAgICBsb2NhbFN0b3JhZ2UucmVtb3ZlSXRlbSgndHJhbnNsYXRlQXBwJyk7XHJcbiAgICBsb2NhbFN0b3JhZ2UucmVtb3ZlSXRlbSgnemgnKTtcclxuICAgIGxvY2FsU3RvcmFnZS5yZW1vdmVJdGVtKCdlbicpO1xyXG4gICAgd2luZG93LmxvY2F0aW9uLnJlbG9hZCgpO1xyXG4gIH1cclxufVxyXG4iXX0=