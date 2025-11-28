import { Injectable } from '@angular/core';
import * as i0 from "@angular/core";
import * as i1 from "../api/api.service";
export class JBService {
    constructor(http) {
        this.http = http;
    }
    setToken(url, data, fn) {
        this.http
            .request({
            url: url,
            method: 'post',
            data,
        })
            .then((res) => {
            fn && fn(res);
            if (res.code !== 0 || res.error) {
                console.error('set token error');
                return;
            }
            localStorage.setItem('jwt', res.data.token);
            localStorage.setItem('roles', res.data.roles);
        });
    }
}
JBService.ɵfac = function JBService_Factory(t) { return new (t || JBService)(i0.ɵɵinject(i1.HttpService)); };
JBService.ɵprov = /*@__PURE__*/ i0.ɵɵdefineInjectable({ token: JBService, factory: JBService.ɵfac });
(function () { (typeof ngDevMode === "undefined" || ngDevMode) && i0.ɵsetClassMetadata(JBService, [{
        type: Injectable
    }], function () { return [{ type: i1.HttpService }]; }, null); })();
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiamIuc2VydmljZS5qcyIsInNvdXJjZVJvb3QiOiIiLCJzb3VyY2VzIjpbIi4uLy4uLy4uLy4uLy4uLy4uL3NyYy9hcHAvc2VydmljZS9qYi1zZXJ2aWNlL2piLnNlcnZpY2UudHMiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IkFBQUEsT0FBTyxFQUFFLFVBQVUsRUFBRSxNQUFNLGVBQWUsQ0FBQzs7O0FBSTNDLE1BQU0sT0FBTyxTQUFTO0lBQ3BCLFlBQW9CLElBQWlCO1FBQWpCLFNBQUksR0FBSixJQUFJLENBQWE7SUFBRyxDQUFDO0lBRXpDLFFBQVEsQ0FBQyxHQUFXLEVBQUUsSUFBUyxFQUFFLEVBQWE7UUFDNUMsSUFBSSxDQUFDLElBQUk7YUFDTixPQUFPLENBQUM7WUFDUCxHQUFHLEVBQUUsR0FBRztZQUNSLE1BQU0sRUFBRSxNQUFNO1lBQ2QsSUFBSTtTQUNMLENBQUM7YUFDRCxJQUFJLENBQUMsQ0FBQyxHQUFRLEVBQUUsRUFBRTtZQUNqQixFQUFFLElBQUksRUFBRSxDQUFDLEdBQUcsQ0FBQyxDQUFDO1lBQ2QsSUFBSSxHQUFHLENBQUMsSUFBSSxLQUFLLENBQUMsSUFBSSxHQUFHLENBQUMsS0FBSyxFQUFFO2dCQUMvQixPQUFPLENBQUMsS0FBSyxDQUFDLGlCQUFpQixDQUFDLENBQUM7Z0JBQ2pDLE9BQU87YUFDUjtZQUNELFlBQVksQ0FBQyxPQUFPLENBQUMsS0FBSyxFQUFFLEdBQUcsQ0FBQyxJQUFJLENBQUMsS0FBSyxDQUFDLENBQUM7WUFDNUMsWUFBWSxDQUFDLE9BQU8sQ0FBQyxPQUFPLEVBQUUsR0FBRyxDQUFDLElBQUksQ0FBQyxLQUFLLENBQUMsQ0FBQztRQUNoRCxDQUFDLENBQUMsQ0FBQztJQUNQLENBQUM7O2tFQW5CVSxTQUFTOytEQUFULFNBQVMsV0FBVCxTQUFTO3VGQUFULFNBQVM7Y0FEckIsVUFBVSIsInNvdXJjZXNDb250ZW50IjpbImltcG9ydCB7IEluamVjdGFibGUgfSBmcm9tICdAYW5ndWxhci9jb3JlJztcclxuaW1wb3J0IHsgSHR0cFNlcnZpY2UgfSBmcm9tICcuLi9hcGkvYXBpLnNlcnZpY2UnO1xyXG5cclxuQEluamVjdGFibGUoKVxyXG5leHBvcnQgY2xhc3MgSkJTZXJ2aWNlIHtcclxuICBjb25zdHJ1Y3Rvcihwcml2YXRlIGh0dHA6IEh0dHBTZXJ2aWNlKSB7fVxyXG5cclxuICBzZXRUb2tlbih1cmw6IHN0cmluZywgZGF0YTogYW55LCBmbj86IEZ1bmN0aW9uKSB7XHJcbiAgICB0aGlzLmh0dHBcclxuICAgICAgLnJlcXVlc3Qoe1xyXG4gICAgICAgIHVybDogdXJsLFxyXG4gICAgICAgIG1ldGhvZDogJ3Bvc3QnLFxyXG4gICAgICAgIGRhdGEsXHJcbiAgICAgIH0pXHJcbiAgICAgIC50aGVuKChyZXM6IGFueSkgPT4ge1xyXG4gICAgICAgIGZuICYmIGZuKHJlcyk7XHJcbiAgICAgICAgaWYgKHJlcy5jb2RlICE9PSAwIHx8IHJlcy5lcnJvcikge1xyXG4gICAgICAgICAgY29uc29sZS5lcnJvcignc2V0IHRva2VuIGVycm9yJyk7XHJcbiAgICAgICAgICByZXR1cm47XHJcbiAgICAgICAgfVxyXG4gICAgICAgIGxvY2FsU3RvcmFnZS5zZXRJdGVtKCdqd3QnLCByZXMuZGF0YS50b2tlbik7XHJcbiAgICAgICAgbG9jYWxTdG9yYWdlLnNldEl0ZW0oJ3JvbGVzJywgcmVzLmRhdGEucm9sZXMpO1xyXG4gICAgICB9KTtcclxuICB9XHJcbn1cclxuIl19