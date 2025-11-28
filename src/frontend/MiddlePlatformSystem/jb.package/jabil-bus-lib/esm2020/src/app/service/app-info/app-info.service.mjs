import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { environment } from '../../../environments/environment';
import * as i0 from "@angular/core";
import * as i1 from "@angular/common/http";
export class AppInfoService {
    constructor(http) {
        this.http = http;
        this._domainUri = environment.i18nDomainUri;
        this._apiKey = environment.i18nApiKey;
    }
    getAppInfo() {
        const getAppInfoProxyUri = `${this._domainUri}/api/applications-info`;
        return this.getData(getAppInfoProxyUri, {});
    }
    getData(route, httpOptions) {
        let headers = httpOptions.headers;
        if (headers) {
            headers.append('x-api-key', this._apiKey);
        }
        else {
            httpOptions.headers = { 'x-api-key': this._apiKey };
        }
        return this.http.get(route, httpOptions);
    }
    getCacheData(key) {
        let langJson = localStorage.getItem(key);
        if (langJson)
            return new BehaviorSubject(langJson);
        return new BehaviorSubject('');
    }
    setApp(val) {
        this.application = val;
    }
    getApp() {
        return this.application;
    }
}
AppInfoService.ɵfac = function AppInfoService_Factory(t) { return new (t || AppInfoService)(i0.ɵɵinject(i1.HttpClient)); };
AppInfoService.ɵprov = /*@__PURE__*/ i0.ɵɵdefineInjectable({ token: AppInfoService, factory: AppInfoService.ɵfac, providedIn: 'root' });
(function () { (typeof ngDevMode === "undefined" || ngDevMode) && i0.ɵsetClassMetadata(AppInfoService, [{
        type: Injectable,
        args: [{
                providedIn: 'root',
            }]
    }], function () { return [{ type: i1.HttpClient }]; }, null); })();
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiYXBwLWluZm8uc2VydmljZS5qcyIsInNvdXJjZVJvb3QiOiIiLCJzb3VyY2VzIjpbIi4uLy4uLy4uLy4uLy4uLy4uL3NyYy9hcHAvc2VydmljZS9hcHAtaW5mby9hcHAtaW5mby5zZXJ2aWNlLnRzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiJBQUFBLE9BQU8sRUFBRSxVQUFVLEVBQUUsTUFBTSxlQUFlLENBQUM7QUFDM0MsT0FBTyxFQUFjLGVBQWUsRUFBRSxNQUFNLE1BQU0sQ0FBQztBQUNuRCxPQUFPLEVBQUUsV0FBVyxFQUFFLE1BQU0sbUNBQW1DLENBQUM7OztBQU1oRSxNQUFNLE9BQU8sY0FBYztJQUt6QixZQUFvQixJQUFnQjtRQUFoQixTQUFJLEdBQUosSUFBSSxDQUFZO1FBSjVCLGVBQVUsR0FBVyxXQUFXLENBQUMsYUFBYSxDQUFDO1FBQy9DLFlBQU8sR0FBVyxXQUFXLENBQUMsVUFBVSxDQUFDO0lBR1YsQ0FBQztJQUVqQyxVQUFVO1FBQ2YsTUFBTSxrQkFBa0IsR0FBRyxHQUFHLElBQUksQ0FBQyxVQUFVLHdCQUF3QixDQUFDO1FBQ3RFLE9BQU8sSUFBSSxDQUFDLE9BQU8sQ0FBQyxrQkFBa0IsRUFBRSxFQUFFLENBQXVCLENBQUM7SUFDcEUsQ0FBQztJQUVNLE9BQU8sQ0FBQyxLQUFhLEVBQUUsV0FBZ0I7UUFDNUMsSUFBSSxPQUFPLEdBQUcsV0FBVyxDQUFDLE9BQXNCLENBQUM7UUFDakQsSUFBSSxPQUFPLEVBQUU7WUFDWCxPQUFPLENBQUMsTUFBTSxDQUFDLFdBQVcsRUFBRSxJQUFJLENBQUMsT0FBTyxDQUFDLENBQUM7U0FDM0M7YUFBTTtZQUNMLFdBQVcsQ0FBQyxPQUFPLEdBQUcsRUFBRSxXQUFXLEVBQUUsSUFBSSxDQUFDLE9BQU8sRUFBRSxDQUFDO1NBQ3JEO1FBRUQsT0FBTyxJQUFJLENBQUMsSUFBSSxDQUFDLEdBQUcsQ0FBQyxLQUFLLEVBQUUsV0FBVyxDQUFDLENBQUM7SUFDM0MsQ0FBQztJQUVNLFlBQVksQ0FBQyxHQUFXO1FBQzdCLElBQUksUUFBUSxHQUFHLFlBQVksQ0FBQyxPQUFPLENBQUMsR0FBRyxDQUFDLENBQUM7UUFDekMsSUFBSSxRQUFRO1lBQUUsT0FBTyxJQUFJLGVBQWUsQ0FBQyxRQUFRLENBQUMsQ0FBQztRQUVuRCxPQUFPLElBQUksZUFBZSxDQUFDLEVBQUUsQ0FBQyxDQUFDO0lBQ2pDLENBQUM7SUFFRCxNQUFNLENBQUMsR0FBUTtRQUNiLElBQUksQ0FBQyxXQUFXLEdBQUcsR0FBRyxDQUFDO0lBQ3pCLENBQUM7SUFFRCxNQUFNO1FBQ0osT0FBTyxJQUFJLENBQUMsV0FBVyxDQUFDO0lBQzFCLENBQUM7OzRFQXBDVSxjQUFjO29FQUFkLGNBQWMsV0FBZCxjQUFjLG1CQUZiLE1BQU07dUZBRVAsY0FBYztjQUgxQixVQUFVO2VBQUM7Z0JBQ1YsVUFBVSxFQUFFLE1BQU07YUFDbkIiLCJzb3VyY2VzQ29udGVudCI6WyJpbXBvcnQgeyBJbmplY3RhYmxlIH0gZnJvbSAnQGFuZ3VsYXIvY29yZSc7XHJcbmltcG9ydCB7IE9ic2VydmFibGUsIEJlaGF2aW9yU3ViamVjdCB9IGZyb20gJ3J4anMnO1xyXG5pbXBvcnQgeyBlbnZpcm9ubWVudCB9IGZyb20gJy4uLy4uLy4uL2Vudmlyb25tZW50cy9lbnZpcm9ubWVudCc7XHJcbmltcG9ydCB7IEh0dHBDbGllbnQsIEh0dHBIZWFkZXJzIH0gZnJvbSAnQGFuZ3VsYXIvY29tbW9uL2h0dHAnO1xyXG5cclxuQEluamVjdGFibGUoe1xyXG4gIHByb3ZpZGVkSW46ICdyb290JyxcclxufSlcclxuZXhwb3J0IGNsYXNzIEFwcEluZm9TZXJ2aWNlIHtcclxuICBwcml2YXRlIF9kb21haW5Vcmk6IHN0cmluZyA9IGVudmlyb25tZW50LmkxOG5Eb21haW5Vcmk7XHJcbiAgcHJpdmF0ZSBfYXBpS2V5OiBzdHJpbmcgPSBlbnZpcm9ubWVudC5pMThuQXBpS2V5O1xyXG4gIHByaXZhdGUgYXBwbGljYXRpb246IGFueSB8IHVuZGVmaW5lZDtcclxuXHJcbiAgY29uc3RydWN0b3IocHJpdmF0ZSBodHRwOiBIdHRwQ2xpZW50KSB7fVxyXG5cclxuICBwdWJsaWMgZ2V0QXBwSW5mbygpOiBPYnNlcnZhYmxlPE9iamVjdD4ge1xyXG4gICAgY29uc3QgZ2V0QXBwSW5mb1Byb3h5VXJpID0gYCR7dGhpcy5fZG9tYWluVXJpfS9hcGkvYXBwbGljYXRpb25zLWluZm9gO1xyXG4gICAgcmV0dXJuIHRoaXMuZ2V0RGF0YShnZXRBcHBJbmZvUHJveHlVcmksIHt9KSBhcyBPYnNlcnZhYmxlPE9iamVjdD47XHJcbiAgfVxyXG5cclxuICBwdWJsaWMgZ2V0RGF0YShyb3V0ZTogc3RyaW5nLCBodHRwT3B0aW9uczogYW55KSB7XHJcbiAgICBsZXQgaGVhZGVycyA9IGh0dHBPcHRpb25zLmhlYWRlcnMgYXMgSHR0cEhlYWRlcnM7XHJcbiAgICBpZiAoaGVhZGVycykge1xyXG4gICAgICBoZWFkZXJzLmFwcGVuZCgneC1hcGkta2V5JywgdGhpcy5fYXBpS2V5KTtcclxuICAgIH0gZWxzZSB7XHJcbiAgICAgIGh0dHBPcHRpb25zLmhlYWRlcnMgPSB7ICd4LWFwaS1rZXknOiB0aGlzLl9hcGlLZXkgfTtcclxuICAgIH1cclxuXHJcbiAgICByZXR1cm4gdGhpcy5odHRwLmdldChyb3V0ZSwgaHR0cE9wdGlvbnMpO1xyXG4gIH1cclxuXHJcbiAgcHVibGljIGdldENhY2hlRGF0YShrZXk6IHN0cmluZykge1xyXG4gICAgbGV0IGxhbmdKc29uID0gbG9jYWxTdG9yYWdlLmdldEl0ZW0oa2V5KTtcclxuICAgIGlmIChsYW5nSnNvbikgcmV0dXJuIG5ldyBCZWhhdmlvclN1YmplY3QobGFuZ0pzb24pO1xyXG5cclxuICAgIHJldHVybiBuZXcgQmVoYXZpb3JTdWJqZWN0KCcnKTtcclxuICB9XHJcblxyXG4gIHNldEFwcCh2YWw6IGFueSkge1xyXG4gICAgdGhpcy5hcHBsaWNhdGlvbiA9IHZhbDtcclxuICB9XHJcblxyXG4gIGdldEFwcCgpIHtcclxuICAgIHJldHVybiB0aGlzLmFwcGxpY2F0aW9uO1xyXG4gIH1cclxufVxyXG4iXX0=