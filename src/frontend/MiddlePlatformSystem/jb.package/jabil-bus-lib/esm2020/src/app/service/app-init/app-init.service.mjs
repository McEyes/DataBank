import { Injectable } from '@angular/core';
import * as AppConstants from '../../common/data/app.constants';
import { registerLocaleData } from '@angular/common';
import * as i0 from "@angular/core";
import * as i1 from "../app-info/app-info.service";
import * as i2 from "@ngx-translate/core";
import * as i3 from "ngx-translate-cache";
export class AppInitService {
    constructor(appInfoService, translateService, translateCacheService) {
        this.appInfoService = appInfoService;
        this.translateService = translateService;
        this.translateCacheService = translateCacheService;
        this.localeInitializer = async (lang) => {
            //throw new Error("localeInitializer")
            const promise = new Promise((resolve, reject) => {
                // System.import()(`@angular/common/locales/${lang}.js`).then(
                // @/../node_modules/@angular/common/locales/extra/${id}
                import(`@/../node_modules/@angular/common/locales/${lang}.mjs`).then(module => {
                    registerLocaleData(module.default);
                    resolve();
                }, () => {
                    import(`@/../node_modules/@angular/common/locales/${lang.split('-')[0]}.mjs`).then(module => {
                        registerLocaleData(module.default);
                        resolve();
                    }, reject);
                });
            });
            return promise;
        };
        this.sleep = (milliseconds) => {
            return new Promise(resolve => setTimeout(resolve, milliseconds));
        };
    }
    async init() {
        // const initMsgElement = document.getElementById('init-message');
        // if (initMsgElement) initMsgElement.textContent = 'Loading...';
        //await this.sleep(2000);
        return new Promise((resolve, reject) => {
            // --------------romove lang http reuqest---------------------------; 
            // const app = localStorage.getItem('translateApp')
            // if(app) {
            //   this.initApp(JSON.parse(app), resolve, reject)
            //   return
            // }
            // this.appInfoService.getAppInfo().subscribe(
            //   async res => {
            //     const appsInfo: AppsInfo = res as AppsInfo;
            //     const app: Application | undefined = appsInfo.applications.find(
            //       a => a.application.toUpperCase() == AppConstants.APP_NAME.toUpperCase()
            //     );
            //     localStorage.setItem('translateApp', JSON.stringify(app))
            //     this.initApp(app, resolve, reject)
            //   },
            //   err => {
            //     // when i18n link error
            //     this.initApp({languages:[{code:'en'}, {code:'zh'}]}, resolve, reject)
            //     // if (initMsgElement) initMsgElement.textContent = 'Oops, something wrong.';
            //     console.error(err);
            //     // reject();
            //   },
            //   () => {
            //     // console.log("Setup application complete")
            //   }
            // );
            // --------------romove lang http reuqest---------------------------; 
            this.initApp({ languages: [{ code: 'en' }, { code: 'zh' }] }, resolve, reject);
        });
    }
    async initApp(app, resolve, reject) {
        this.appInfoService.setApp(app);
        const codes = app?.languages.map((l) => l.code);
        try {
            if (codes) {
                for (const code of codes) {
                    await this.localeInitializer(code);
                }
            }
            this.setupTranslation();
        }
        catch (err) {
            // if (initMsgElement) initMsgElement.textContent = 'Oops, something wrong.';
            console.error(err);
            reject();
        }
        resolve(true);
    }
    setupTranslation() {
        //throw new Error("setupTranslation")
        if (this.appInfoService.getApp()) {
            // @ts-ignore
            const supportedLangs = this.appInfoService.getApp().languages.map(l => l.code);
            this.translateService.addLangs(supportedLangs.reverse());
        }
        // const langToUse = this.translateCacheService.getCachedLanguage();
        const langToUse = localStorage.getItem('lang');
        if (!langToUse) {
            this.translateService.setDefaultLang(AppConstants.DEFAULT_LANGUAGE.code);
        }
        this.translateService.use(langToUse || AppConstants.DEFAULT_LANGUAGE.code);
        localStorage.setItem('lang', langToUse || AppConstants.DEFAULT_LANGUAGE.code);
        // this.translateCacheService.init();
    }
}
AppInitService.ɵfac = function AppInitService_Factory(t) { return new (t || AppInitService)(i0.ɵɵinject(i1.AppInfoService), i0.ɵɵinject(i2.TranslateService), i0.ɵɵinject(i3.TranslateCacheService)); };
AppInitService.ɵprov = /*@__PURE__*/ i0.ɵɵdefineInjectable({ token: AppInitService, factory: AppInitService.ɵfac, providedIn: 'root' });
(function () { (typeof ngDevMode === "undefined" || ngDevMode) && i0.ɵsetClassMetadata(AppInitService, [{
        type: Injectable,
        args: [{
                providedIn: 'root',
            }]
    }], function () { return [{ type: i1.AppInfoService }, { type: i2.TranslateService }, { type: i3.TranslateCacheService }]; }, null); })();
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiYXBwLWluaXQuc2VydmljZS5qcyIsInNvdXJjZVJvb3QiOiIiLCJzb3VyY2VzIjpbIi4uLy4uLy4uLy4uLy4uLy4uL3NyYy9hcHAvc2VydmljZS9hcHAtaW5pdC9hcHAtaW5pdC5zZXJ2aWNlLnRzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiJBQUFBLE9BQU8sRUFBRSxVQUFVLEVBQUUsTUFBTSxlQUFlLENBQUM7QUFHM0MsT0FBTyxLQUFLLFlBQVksTUFBTSxpQ0FBaUMsQ0FBQztBQUNoRSxPQUFPLEVBQUUsa0JBQWtCLEVBQUUsTUFBTSxpQkFBaUIsQ0FBQzs7Ozs7QUFPckQsTUFBTSxPQUFPLGNBQWM7SUFDekIsWUFDVSxjQUE4QixFQUM5QixnQkFBa0MsRUFDbEMscUJBQTRDO1FBRjVDLG1CQUFjLEdBQWQsY0FBYyxDQUFnQjtRQUM5QixxQkFBZ0IsR0FBaEIsZ0JBQWdCLENBQWtCO1FBQ2xDLDBCQUFxQixHQUFyQixxQkFBcUIsQ0FBdUI7UUE4RDlDLHNCQUFpQixHQUFHLEtBQUssRUFBRSxJQUFZLEVBQWdCLEVBQUU7WUFDL0Qsc0NBQXNDO1lBQ3RDLE1BQU0sT0FBTyxHQUFHLElBQUksT0FBTyxDQUFPLENBQUMsT0FBTyxFQUFFLE1BQU0sRUFBRSxFQUFFO2dCQUNwRCw4REFBOEQ7Z0JBQzlELHdEQUF3RDtnQkFDeEQsTUFBTSxDQUFDLDZDQUE2QyxJQUFJLE1BQU0sQ0FBQyxDQUFDLElBQUksQ0FDbEUsTUFBTSxDQUFDLEVBQUU7b0JBQ1Asa0JBQWtCLENBQUMsTUFBTSxDQUFDLE9BQU8sQ0FBQyxDQUFDO29CQUNuQyxPQUFPLEVBQUUsQ0FBQztnQkFDWixDQUFDLEVBQ0QsR0FBRyxFQUFFO29CQUNILE1BQU0sQ0FBQyw2Q0FBNkMsSUFBSSxDQUFDLEtBQUssQ0FBQyxHQUFHLENBQUMsQ0FBQyxDQUFDLENBQUMsTUFBTSxDQUFDLENBQUMsSUFBSSxDQUNoRixNQUFNLENBQUMsRUFBRTt3QkFDUCxrQkFBa0IsQ0FBQyxNQUFNLENBQUMsT0FBTyxDQUFD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Y0FBYyxDQUFDLFlBQVksQ0FBQyxnQkFBZ0IsQ0FBQyxJQUFJLENBQUMsQ0FBQTtTQUN6RTtRQUVELElBQUksQ0FBQyxnQkFBZ0IsQ0FBQyxHQUFHLENBQUMsU0FBUyxJQUFJLFlBQVksQ0FBQyxnQkFBZ0IsQ0FBQyxJQUFJLENBQUMsQ0FBQztRQUUzRSxZQUFZLENBQUMsT0FBTyxDQUFDLE1BQU0sRUFBRSxTQUFTLElBQUksWUFBWSxDQUFDLGdCQUFnQixDQUFDLElBQUksQ0FBQyxDQUFBO1FBQzdFLHFDQUFxQztJQUN2QyxDQUFDOzs0RUE5R1UsY0FBYztvRUFBZCxjQUFjLFdBQWQsY0FBYyxtQkFGYixNQUFNO3VGQUVQLGNBQWM7Y0FIMUIsVUFBVTtlQUFDO2dCQUNWLFVBQVUsRUFBRSxNQUFNO2FBQ25CIiwic291cmNlc0NvbnRlbnQiOlsiaW1wb3J0IHsgSW5qZWN0YWJsZSB9IGZyb20gJ0Bhbmd1bGFyL2NvcmUnO1xyXG5pbXBvcnQgeyBBcHBJbmZvU2VydmljZSB9IGZyb20gJy4uL2FwcC1pbmZvL2FwcC1pbmZvLnNlcnZpY2UnO1xyXG5pbXBvcnQgeyBBcHBzSW5mbywgQXBwbGljYXRpb24sIExhbmd1YWdlIH0gZnJvbSAnLi4vLi4vY29tbW9uL21vZGVsL2RhdGEubW9kZWwnO1xyXG5pbXBvcnQgKiBhcyBBcHBDb25zdGFudHMgZnJvbSAnLi4vLi4vY29tbW9uL2RhdGEvYXBwLmNvbnN0YW50cyc7XHJcbmltcG9ydCB7IHJlZ2lzdGVyTG9jYWxlRGF0YSB9IGZyb20gJ0Bhbmd1bGFyL2NvbW1vbic7XHJcbmltcG9ydCB7IFRyYW5zbGF0ZVNlcnZpY2UgfSBmcm9tICdAbmd4LXRyYW5zbGF0ZS9jb3JlJztcclxuaW1wb3J0IHsgVHJhbnNsYXRlQ2FjaGVTZXJ2aWNlIH0gZnJvbSAnbmd4LXRyYW5zbGF0ZS1jYWNoZSc7XHJcblxyXG5ASW5qZWN0YWJsZSh7XHJcbiAgcHJvdmlkZWRJbjogJ3Jvb3QnLFxyXG59KVxyXG5leHBvcnQgY2xhc3MgQXBwSW5pdFNlcnZpY2Uge1xyXG4gIGNvbnN0cnVjdG9yKFxyXG4gICAgcHJpdmF0ZSBhcHBJbmZvU2VydmljZTogQXBwSW5mb1NlcnZpY2UsXHJcbiAgICBwcml2YXRlIHRyYW5zbGF0ZVNlcnZpY2U6IFRyYW5zbGF0ZVNlcnZpY2UsXHJcbiAgICBwcml2YXRlIHRyYW5zbGF0ZUNhY2hlU2VydmljZTogVHJhbnNsYXRlQ2FjaGVTZXJ2aWNlXHJcbiAgKSB7fVxyXG5cclxuICBwdWJsaWMgYXN5bmMgaW5pdCgpIHtcclxuICAgIC8vIGNvbnN0IGluaXRNc2dFbGVtZW50ID0gZG9jdW1lbnQuZ2V0RWxlbWVudEJ5SWQoJ2luaXQtbWVzc2FnZScpO1xyXG4gICAgLy8gaWYgKGluaXRNc2dFbGVtZW50KSBpbml0TXNnRWxlbWVudC50ZXh0Q29udGVudCA9ICdMb2FkaW5nLi4uJztcclxuXHJcbiAgICAvL2F3YWl0IHRoaXMuc2xlZXAoMjAwMCk7XHJcbiAgICByZXR1cm4gbmV3IFByb21pc2UoKHJlc29sdmUsIHJlamVjdCkgPT4ge1xyXG5cclxuICAgICAgLy8gLS0tLS0tLS0tLS0tLS1yb21vdmUgbGFuZyBodHRwIHJldXFlc3QtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS07IFxyXG4gICAgICAvLyBjb25zdCBhcHAgPSBsb2NhbFN0b3JhZ2UuZ2V0SXRlbSgndHJhbnNsYXRlQXBwJylcclxuICAgICAgLy8gaWYoYXBwKSB7XHJcbiAgICAgIC8vICAgdGhpcy5pbml0QXBwKEpTT04ucGFyc2UoYXBwKSwgcmVzb2x2ZSwgcmVqZWN0KVxyXG4gICAgICAvLyAgIHJldHVyblxyXG4gICAgICAvLyB9XHJcblxyXG4gICAgICAvLyB0aGlzLmFwcEluZm9TZXJ2aWNlLmdldEFwcEluZm8oKS5zdWJzY3JpYmUoXHJcbiAgICAgIC8vICAgYXN5bmMgcmVzID0+IHtcclxuICAgICAgLy8gICAgIGNvbnN0IGFwcHNJbmZvOiBBcHBzSW5mbyA9IHJlcyBhcyBBcHBzSW5mbztcclxuICAgICAgLy8gICAgIGNvbnN0IGFwcDogQXBwbGljYXRpb24gfCB1bmRlZmluZWQgPSBhcHBzSW5mby5hcHBsaWNhdGlvbnMuZmluZChcclxuICAgICAgLy8gICAgICAgYSA9PiBhLmFwcGxpY2F0aW9uLnRvVXBwZXJDYXNlKCkgPT0gQXBwQ29uc3RhbnRzLkFQUF9OQU1FLnRvVXBwZXJDYXNlKClcclxuICAgICAgLy8gICAgICk7XHJcbiAgICAgIC8vICAgICBsb2NhbFN0b3JhZ2Uuc2V0SXRlbSgndHJhbnNsYXRlQXBwJywgSlNPTi5zdHJpbmdpZnkoYXBwKSlcclxuICAgICAgLy8gICAgIHRoaXMuaW5pdEFwcChhcHAsIHJlc29sdmUsIHJlamVjdClcclxuICAgICAgLy8gICB9LFxyXG4gICAgICAvLyAgIGVyciA9PiB7XHJcbiAgICAgIC8vICAgICAvLyB3aGVuIGkxOG4gbGluayBlcnJvclxyXG4gICAgICAvLyAgICAgdGhpcy5pbml0QXBwKHtsYW5ndWFnZXM6W3tjb2RlOidlbid9LCB7Y29kZTonemgnfV19LCByZXNvbHZlLCByZWplY3QpXHJcbiAgICAgIC8vICAgICAvLyBpZiAoaW5pdE1zZ0VsZW1lbnQpIGluaXRNc2dFbGVtZW50LnRleHRDb250ZW50ID0gJ09vcHMsIHNvbWV0aGluZyB3cm9uZy4nO1xyXG4gICAgICAvLyAgICAgY29uc29sZS5lcnJvcihlcnIpO1xyXG4gICAgICAvLyAgICAgLy8gcmVqZWN0KCk7XHJcbiAgICAgIC8vICAgfSxcclxuICAgICAgLy8gICAoKSA9PiB7XHJcbiAgICAgIC8vICAgICAvLyBjb25zb2xlLmxvZyhcIlNldHVwIGFwcGxpY2F0aW9uIGNvbXBsZXRlXCIpXHJcbiAgICAgIC8vICAgfVxyXG4gICAgICAvLyApO1xyXG4gICAgICAvLyAtLS0tLS0tLS0tLS0tLXJvbW92ZSBsYW5nIGh0dHAgcmV1cWVzdC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLTsgXHJcbiAgICAgIHRoaXMuaW5pdEFwcCh7bGFuZ3VhZ2VzOlt7Y29kZTonZW4nfSwge2NvZGU6J3poJ31dfSwgcmVzb2x2ZSwgcmVqZWN0KVxyXG4gICAgfSk7XHJcbiAgfVxyXG5cclxuICBhc3luYyBpbml0QXBwKGFwcDogYW55LCByZXNvbHZlOmFueSwgcmVqZWN0OmFueSkge1xyXG4gICAgdGhpcy5hcHBJbmZvU2VydmljZS5zZXRBcHAoYXBwKTtcclxuICAgIGNvbnN0IGNvZGVzOiBzdHJpbmdbXSB8IHVuZGVmaW5lZCA9IGFwcD8ubGFuZ3VhZ2VzLm1hcCgobDphbnkpID0+IGwuY29kZSk7XHJcblxyXG4gICAgdHJ5IHtcclxuICAgICAgaWYgKGNvZGVzKSB7XHJcbiAgICAgICAgZm9yIChjb25zdCBjb2RlIG9mIGNvZGVzKSB7XHJcbiAgICAgICAgICBhd2FpdCB0aGlzLmxvY2FsZUluaXRpYWxpemVyKGNvZGUpO1xyXG4gICAgICAgIH1cclxuICAgICAgfVxyXG4gICAgICB0aGlzLnNldHVwVHJhbnNsYXRpb24oKTtcclxuICAgIH0gY2F0Y2ggKGVycikge1xyXG4gICAgICAvLyBpZiAoaW5pdE1zZ0VsZW1lbnQpIGluaXRNc2dFbGVtZW50LnRleHRDb250ZW50ID0gJ09vcHMsIHNvbWV0aGluZyB3cm9uZy4nO1xyXG4gICAgICBjb25zb2xlLmVycm9yKGVycik7XHJcbiAgICAgIHJlamVjdCgpO1xyXG4gICAgfVxyXG5cclxuICAgIHJlc29sdmUodHJ1ZSk7XHJcbiAgfVxyXG5cclxuICBwcml2YXRlIGxvY2FsZUluaXRpYWxpemVyID0gYXN5bmMgKGxhbmc6IHN0cmluZyk6IFByb21pc2U8YW55PiA9PiB7XHJcbiAgICAvL3Rocm93IG5ldyBFcnJvcihcImxvY2FsZUluaXRpYWxpemVyXCIpXHJcbiAgICBjb25zdCBwcm9taXNlID0gbmV3IFByb21pc2U8dm9pZD4oKHJlc29sdmUsIHJlamVjdCkgPT4ge1xyXG4gICAgICAvLyBTeXN0ZW0uaW1wb3J0KCkoYEBhbmd1bGFyL2NvbW1vbi9sb2NhbGVzLyR7bGFuZ30uanNgKS50aGVuKFxyXG4gICAgICAvLyBALy4uL25vZGVfbW9kdWxlcy9AYW5ndWxhci9jb21tb24vbG9jYWxlcy9leHRyYS8ke2lkfVxyXG4gICAgICBpbXBvcnQoYEAvLi4vbm9kZV9tb2R1bGVzL0Bhbmd1bGFyL2NvbW1vbi9sb2NhbGVzLyR7bGFuZ30ubWpzYCkudGhlbihcclxuICAgICAgICBtb2R1bGUgPT4ge1xyXG4gICAgICAgICAgcmVnaXN0ZXJMb2NhbGVEYXRhKG1vZHVsZS5kZWZhdWx0KTtcclxuICAgICAgICAgIHJlc29sdmUoKTtcclxuICAgICAgICB9LFxyXG4gICAgICAgICgpID0+IHtcclxuICAgICAgICAgIGltcG9ydChgQC8uLi9ub2RlX21vZHVsZXMvQGFuZ3VsYXIvY29tbW9uL2xvY2FsZXMvJHtsYW5nLnNwbGl0KCctJylbMF19Lm1qc2ApLnRoZW4oXHJcbiAgICAgICAgICAgIG1vZHVsZSA9PiB7XHJcbiAgICAgICAgICAgICAgcmVnaXN0ZXJMb2NhbGVEYXRhKG1vZHVsZS5kZWZhdWx0KTtcclxuICAgICAgICAgICAgICByZXNvbHZlKCk7XHJcbiAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgIHJlamVjdFxyXG4gICAgICAgICAgKTtcclxuICAgICAgICB9XHJcbiAgICAgICk7XHJcbiAgICB9KTtcclxuXHJcbiAgICByZXR1cm4gcHJvbWlzZTtcclxuICB9O1xyXG5cclxuICBwcml2YXRlIHNldHVwVHJhbnNsYXRpb24oKSB7XHJcbiAgICAvL3Rocm93IG5ldyBFcnJvcihcInNldHVwVHJhbnNsYXRpb25cIilcclxuICAgIGlmICh0aGlzLmFwcEluZm9TZXJ2aWNlLmdldEFwcCgpKSB7XHJcbiAgICAgIC8vIEB0cy1pZ25vcmVcclxuICAgICAgY29uc3Qgc3VwcG9ydGVkTGFuZ3M6IHN0cmluZ1tdID0gdGhpcy5hcHBJbmZvU2VydmljZS5nZXRBcHAoKS5sYW5ndWFnZXMubWFwKGwgPT4gbC5jb2RlKTtcclxuICAgICAgdGhpcy50cmFuc2xhdGVTZXJ2aWNlLmFkZExhbmdzKHN1cHBvcnRlZExhbmdzLnJldmVyc2UoKSk7XHJcbiAgICB9XHJcblxyXG4gICAgLy8gY29uc3QgbGFuZ1RvVXNlID0gdGhpcy50cmFuc2xhdGVDYWNoZVNlcnZpY2UuZ2V0Q2FjaGVkTGFuZ3VhZ2UoKTtcclxuICAgIGNvbnN0IGxhbmdUb1VzZSA9IGxvY2FsU3RvcmFnZS5nZXRJdGVtKCdsYW5nJylcclxuXHJcbiAgICBpZiAoIWxhbmdUb1VzZSkge1xyXG4gICAgICB0aGlzLnRyYW5zbGF0ZVNlcnZpY2Uuc2V0RGVmYXVsdExhbmcoQXBwQ29uc3RhbnRzLkRFRkFVTFRfTEFOR1VBR0UuY29kZSlcclxuICAgIH1cclxuXHJcbiAgICB0aGlzLnRyYW5zbGF0ZVNlcnZpY2UudXNlKGxhbmdUb1VzZSB8fCBBcHBDb25zdGFudHMuREVGQVVMVF9MQU5HVUFHRS5jb2RlKTtcclxuXHJcbiAgICBsb2NhbFN0b3JhZ2Uuc2V0SXRlbSgnbGFuZycsIGxhbmdUb1VzZSB8fCBBcHBDb25zdGFudHMuREVGQVVMVF9MQU5HVUFHRS5jb2RlKVxyXG4gICAgLy8gdGhpcy50cmFuc2xhdGVDYWNoZVNlcnZpY2UuaW5pdCgpO1xyXG4gIH1cclxuXHJcbiAgcHJpdmF0ZSBzbGVlcCA9IChtaWxsaXNlY29uZHM6IG51bWJlcikgPT4ge1xyXG4gICAgcmV0dXJuIG5ldyBQcm9taXNlKHJlc29sdmUgPT4gc2V0VGltZW91dChyZXNvbHZlLCBtaWxsaXNlY29uZHMpKTtcclxuICB9O1xyXG59XHJcbiJdfQ==