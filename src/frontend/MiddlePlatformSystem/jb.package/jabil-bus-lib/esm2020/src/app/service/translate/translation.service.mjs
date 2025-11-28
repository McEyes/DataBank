import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import en from './en.json';
import cn from './cn.json';
import * as i0 from "@angular/core";
import * as i1 from "../app-info/app-info.service";
export class TranslationApiService {
    constructor(appInfo) {
        this.appInfo = appInfo;
        this._domainUri = environment.i18nDomainUri;
        this.translateStatus = false;
    }
    getTranslationsFromApi(lang, translateJson) {
        // 获取本地翻译版本，判断是否更新
        const currLang = localStorage.getItem('lang') || '';
        const translateVersion = localStorage.getItem('translateVersion');
        if (currLang && localStorage.getItem(currLang)) {
            const translate = JSON.parse(localStorage.getItem(currLang) || '{}');
            if (translate && Number(translateVersion) < Number(translate.translateVersion)) {
                return new Observable(subscriber => {
                    subscriber.next(Object.assign(translate, translateJson) || {});
                    subscriber.complete();
                });
            }
        }
        return new Observable(subscriber => {
            const res = localStorage.getItem('lang') === 'zh'
                ? this.processTranslations(Object.assign(cn, translateJson))
                : this.processTranslations(Object.assign(en, translateJson));
            subscriber.next(res);
            subscriber.complete();
        });
        // --------------romove lang http reuqest---------------------------; 
        // 渲染本地数据
        // const appId: number | undefined = this.appInfo.getApp()?.id;
        // if(!appId || !lang){
        //   console.info('local lang resource')
        //   return new Observable(subscriber => {
        //     const res = localStorage.getItem('lang') === 'zh'
        //     ? this.processTranslations(Object.assign(cn, translateJson))
        //     : this.processTranslations(Object.assign( en, translateJson))
        //     subscriber.next(res)
        //     subscriber.complete()
        //   })
        // }
        // const langId: number | undefined = this.getLanguageByCode(lang)?.id;
        // let getTranslationsProxyUri =
        //   this._domainUri + '/api/applications/' + appId + '/languages/' + langId + '/translations';
        // const result = this.appInfo.getData(getTranslationsProxyUri, {});
        // return result.pipe(
        //   map((res: Object) =>
        //     localStorage.getItem('lang') === 'zh'
        //       ? this.processTranslations(Object.assign(res, cn, translateJson))
        //       : this.processTranslations(Object.assign(res, en, translateJson))
        //   )
        // );
        // --------------romove lang http reuqest---------------------------; 
    }
    processTranslations(translations) {
        const newTranslations = {};
        // if (TranslationApiService.isJsonString(translations)) {
        //   translations = JSON.parse(translations);
        // }
        for (const key in translations) {
            if (translations.hasOwnProperty(key)) {
                if (typeof translations[key] === 'object') {
                    newTranslations[key] = this.processTranslations(translations[key]);
                }
                else if (typeof translations[key] === 'string' && translations[key] === 'N/A') {
                    // Remove N/A from the translations response
                }
                else {
                    newTranslations[key] = translations[key];
                }
            }
        }
        this.translateStatus = true;
        const lang = localStorage.getItem('lang') || 'en';
        localStorage.setItem(lang, JSON.stringify(newTranslations));
        return newTranslations;
    }
    getLanguageByCode(code) {
        return this.appInfo.getApp()?.languages.find((l) => l.code == code);
    }
    static isJsonString(str) {
        try {
            const obj = JSON.parse(str);
            return !!(typeof obj == 'object' && obj);
        }
        catch (e) {
            console.log('error：' + str + '!!!' + e);
            return false;
        }
    }
    getTranslateStatus() {
        return this.translateStatus;
    }
}
TranslationApiService.ɵfac = function TranslationApiService_Factory(t) { return new (t || TranslationApiService)(i0.ɵɵinject(i1.AppInfoService)); };
TranslationApiService.ɵprov = /*@__PURE__*/ i0.ɵɵdefineInjectable({ token: TranslationApiService, factory: TranslationApiService.ɵfac, providedIn: 'root' });
(function () { (typeof ngDevMode === "undefined" || ngDevMode) && i0.ɵsetClassMetadata(TranslationApiService, [{
        type: Injectable,
        args: [{
                providedIn: 'root',
            }]
    }], function () { return [{ type: i1.AppInfoService }]; }, null); })();
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoidHJhbnNsYXRpb24uc2VydmljZS5qcyIsInNvdXJjZVJvb3QiOiIiLCJzb3VyY2VzIjpbIi4uLy4uLy4uLy4uLy4uLy4uL3NyYy9hcHAvc2VydmljZS90cmFuc2xhdGUvdHJhbnNsYXRpb24uc2VydmljZS50cyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFBQSxPQUFPLEVBQUUsVUFBVSxFQUFFLE1BQU0sZUFBZSxDQUFDO0FBQzNDLE9BQU8sRUFBRSxVQUFVLEVBQUUsTUFBTSxNQUFNLENBQUM7QUFDbEMsT0FBTyxFQUFFLFdBQVcsRUFBRSxNQUFNLG1DQUFtQyxDQUFDO0FBR2hFLE9BQU8sRUFBRSxNQUFNLFdBQVcsQ0FBQztBQUMzQixPQUFPLEVBQUUsTUFBTSxXQUFXLENBQUM7OztBQU0zQixNQUFNLE9BQU8scUJBQXFCO0lBSWhDLFlBQW9CLE9BQXVCO1FBQXZCLFlBQU8sR0FBUCxPQUFPLENBQWdCO1FBSG5DLGVBQVUsR0FBVyxXQUFXLENBQUMsYUFBYSxDQUFDO1FBQy9DLG9CQUFlLEdBQVksS0FBSyxDQUFDO0lBRUssQ0FBQztJQUV4QyxzQkFBc0IsQ0FDM0IsSUFBWSxFQUNaLGFBQXNCO1FBR3RCLGtCQUFrQjtRQUNsQixNQUFNLFFBQVEsR0FBVyxZQUFZLENBQUMsT0FBTyxDQUFDLE1BQU0sQ0FBQyxJQUFJLEVBQUUsQ0FBQTtRQUMzRCxNQUFNLGdCQUFnQixHQUFRLFlBQVksQ0FBQyxPQUFPLENBQUMsa0JBQWtCLENBQUMsQ0FBQTtRQUN0RSxJQUFHLFFBQVEsSUFBRSxZQUFZLENBQUMsT0FBTyxDQUFDLFFBQVEsQ0FBQyxFQUFFO1lBQzNDLE1BQU0sU0FBUyxHQUFRLElBQUksQ0FBQyxLQUFLLENBQUMsWUFBWSxDQUFDLE9BQU8sQ0FBQyxRQUFRLENBQUMsSUFBSSxJQUFJLENBQUMsQ0FBQTtZQUN6RSxJQUFHLFNBQVMsSUFBSSxNQUFNLENBQUMsZ0JBQWdCLENBQUMsR0FBRyxNQUFNLENBQUMsU0FBUyxDQUFDLGdCQUFnQixDQUFDLEVBQUU7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R0FBRyxHQUFHLElBQUksQ0FBQyxLQUFLLENBQUMsR0FBRyxDQUFDLENBQUM7WUFDNUIsT0FBTyxDQUFDLENBQUMsQ0FBQyxPQUFPLEdBQUcsSUFBSSxRQUFRLElBQUksR0FBRyxDQUFDLENBQUM7U0FDMUM7UUFBQyxPQUFPLENBQUMsRUFBRTtZQUNWLE9BQU8sQ0FBQyxHQUFHLENBQUMsUUFBUSxHQUFHLEdBQUcsR0FBRyxLQUFLLEdBQUcsQ0FBQyxDQUFDLENBQUM7WUFDeEMsT0FBTyxLQUFLLENBQUM7U0FDZDtJQUNILENBQUM7SUFFRCxrQkFBa0I7UUFDaEIsT0FBTyxJQUFJLENBQUMsZUFBZSxDQUFBO0lBQzdCLENBQUM7OzBGQXBHVSxxQkFBcUI7MkVBQXJCLHFCQUFxQixXQUFyQixxQkFBcUIsbUJBRnBCLE1BQU07dUZBRVAscUJBQXFCO2NBSGpDLFVBQVU7ZUFBQztnQkFDVixVQUFVLEVBQUUsTUFBTTthQUNuQiIsInNvdXJjZXNDb250ZW50IjpbImltcG9ydCB7IEluamVjdGFibGUgfSBmcm9tICdAYW5ndWxhci9jb3JlJztcclxuaW1wb3J0IHsgT2JzZXJ2YWJsZSB9IGZyb20gJ3J4anMnO1xyXG5pbXBvcnQgeyBlbnZpcm9ubWVudCB9IGZyb20gJy4uLy4uLy4uL2Vudmlyb25tZW50cy9lbnZpcm9ubWVudCc7XHJcbmltcG9ydCB7IExhbmd1YWdlIH0gZnJvbSAnLi4vLi4vY29tbW9uL21vZGVsL2RhdGEubW9kZWwnO1xyXG5pbXBvcnQgeyBBcHBJbmZvU2VydmljZSB9IGZyb20gJy4uL2FwcC1pbmZvL2FwcC1pbmZvLnNlcnZpY2UnO1xyXG5pbXBvcnQgZW4gZnJvbSAnLi9lbi5qc29uJztcclxuaW1wb3J0IGNuIGZyb20gJy4vY24uanNvbic7XHJcbmltcG9ydCB7IG1hcCB9IGZyb20gJ3J4anMvb3BlcmF0b3JzJztcclxuXHJcbkBJbmplY3RhYmxlKHtcclxuICBwcm92aWRlZEluOiAncm9vdCcsXHJcbn0pXHJcbmV4cG9ydCBjbGFzcyBUcmFuc2xhdGlvbkFwaVNlcnZpY2Uge1xyXG4gIHByaXZhdGUgX2RvbWFpblVyaTogc3RyaW5nID0gZW52aXJvbm1lbnQuaTE4bkRvbWFpblVyaTtcclxuICBwcml2YXRlIHRyYW5zbGF0ZVN0YXR1czogYm9vbGVhbiA9IGZhbHNlO1xyXG5cclxuICBjb25zdHJ1Y3Rvcihwcml2YXRlIGFwcEluZm86IEFwcEluZm9TZXJ2aWNlKSB7fVxyXG5cclxuICBwdWJsaWMgZ2V0VHJhbnNsYXRpb25zRnJvbUFwaShcclxuICAgIGxhbmc6IHN0cmluZyxcclxuICAgIHRyYW5zbGF0ZUpzb24/OiBvYmplY3QsXHJcbiAgKTogT2JzZXJ2YWJsZTxPYmplY3Q+IHtcclxuXHJcbiAgICAvLyDojrflj5bmnKzlnLDnv7vor5HniYjmnKzvvIzliKTmlq3mmK/lkKbmm7TmlrBcclxuICAgIGNvbnN0IGN1cnJMYW5nOiBzdHJpbmcgPSBsb2NhbFN0b3JhZ2UuZ2V0SXRlbSgnbGFuZycpIHx8ICcnXHJcbiAgICBjb25zdCB0cmFuc2xhdGVWZXJzaW9uOiBhbnkgPSBsb2NhbFN0b3JhZ2UuZ2V0SXRlbSgndHJhbnNsYXRlVmVyc2lvbicpXHJcbiAgICBpZihjdXJyTGFuZyYmbG9jYWxTdG9yYWdlLmdldEl0ZW0oY3VyckxhbmcpKSB7XHJcbiAgICAgIGNvbnN0IHRyYW5zbGF0ZTogYW55ID0gSlNPTi5wYXJzZShsb2NhbFN0b3JhZ2UuZ2V0SXRlbShjdXJyTGFuZykgfHwgJ3t9JylcclxuICAgICAgaWYodHJhbnNsYXRlICYmIE51bWJlcih0cmFuc2xhdGVWZXJzaW9uKSA8IE51bWJlcih0cmFuc2xhdGUudHJhbnNsYXRlVmVyc2lvbikpIHtcclxuICAgICAgICByZXR1cm4gbmV3IE9ic2VydmFibGUoc3Vic2NyaWJlciA9PiB7XHJcbiAgICAgICAgICBzdWJzY3JpYmVyLm5leHQoT2JqZWN0LmFzc2lnbih0cmFuc2xhdGUsIHRyYW5zbGF0ZUpzb24pIHx8IHt9KVxyXG4gICAgICAgICAgc3Vic2NyaWJlci5jb21wbGV0ZSgpXHJcbiAgICAgICAgfSlcclxuICAgICAgfVxyXG4gICAgfVxyXG5cclxuICAgIHJldHVybiBuZXcgT2JzZXJ2YWJsZShzdWJzY3JpYmVyID0+IHtcclxuICAgICAgY29uc3QgcmVzID0gbG9jYWxTdG9yYWdlLmdldEl0ZW0oJ2xhbmcnKSA9PT0gJ3poJ1xyXG4gICAgICA/IHRoaXMucHJvY2Vzc1RyYW5zbGF0aW9ucyhPYmplY3QuYXNzaWduKGNuLCB0cmFuc2xhdGVKc29uKSlcclxuICAgICAgOiB0aGlzLnByb2Nlc3NUcmFuc2xhdGlvbnMoT2JqZWN0LmFzc2lnbiggZW4sIHRyYW5zbGF0ZUpzb24pKVxyXG4gICAgICBzdWJzY3JpYmVyLm5leHQocmVzKVxyXG4gICAgICBzdWJzY3JpYmVyLmNvbXBsZXRlKClcclxuICAgIH0pXHJcblxyXG4gICAgICAgICAgLy8gLS0tLS0tLS0tLS0tLS1yb21vdmUgbGFuZyBodHRwIHJldXFlc3QtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS07IFxyXG4gICAgLy8g5riy5p+T5pys5Zyw5pWw5o2uXHJcbiAgICAvLyBjb25zdCBhcHBJZDogbnVtYmVyIHwgdW5kZWZpbmVkID0gdGhpcy5hcHBJbmZvLmdldEFwcCgpPy5pZDtcclxuICAgIC8vIGlmKCFhcHBJZCB8fCAhbGFuZyl7XHJcbiAgICAvLyAgIGNvbnNvbGUuaW5mbygnbG9jYWwgbGFuZyByZXNvdXJjZScpXHJcbiAgICAvLyAgIHJldHVybiBuZXcgT2JzZXJ2YWJsZShzdWJzY3JpYmVyID0+IHtcclxuICAgIC8vICAgICBjb25zdCByZXMgPSBsb2NhbFN0b3JhZ2UuZ2V0SXRlbSgnbGFuZycpID09PSAnemgnXHJcbiAgICAvLyAgICAgPyB0aGlzLnByb2Nlc3NUcmFuc2xhdGlvbnMoT2JqZWN0LmFzc2lnbihjbiwgdHJhbnNsYXRlSnNvbikpXHJcbiAgICAvLyAgICAgOiB0aGlzLnByb2Nlc3NUcmFuc2xhdGlvbnMoT2JqZWN0LmFzc2lnbiggZW4sIHRyYW5zbGF0ZUpzb24pKVxyXG4gICAgLy8gICAgIHN1YnNjcmliZXIubmV4dChyZXMpXHJcbiAgICAvLyAgICAgc3Vic2NyaWJlci5jb21wbGV0ZSgpXHJcbiAgICAvLyAgIH0pXHJcbiAgICAvLyB9XHJcblxyXG4gICAgLy8gY29uc3QgbGFuZ0lkOiBudW1iZXIgfCB1bmRlZmluZWQgPSB0aGlzLmdldExhbmd1YWdlQnlDb2RlKGxhbmcpPy5pZDtcclxuICAgIC8vIGxldCBnZXRUcmFuc2xhdGlvbnNQcm94eVVyaSA9XHJcbiAgICAvLyAgIHRoaXMuX2RvbWFpblVyaSArICcvYXBpL2FwcGxpY2F0aW9ucy8nICsgYXBwSWQgKyAnL2xhbmd1YWdlcy8nICsgbGFuZ0lkICsgJy90cmFuc2xhdGlvbnMnO1xyXG5cclxuICAgIC8vIGNvbnN0IHJlc3VsdCA9IHRoaXMuYXBwSW5mby5nZXREYXRhKGdldFRyYW5zbGF0aW9uc1Byb3h5VXJpLCB7fSk7XHJcbiAgICAvLyByZXR1cm4gcmVzdWx0LnBpcGUoXHJcbiAgICAvLyAgIG1hcCgocmVzOiBPYmplY3QpID0+XHJcbiAgICAvLyAgICAgbG9jYWxTdG9yYWdlLmdldEl0ZW0oJ2xhbmcnKSA9PT0gJ3poJ1xyXG4gICAgLy8gICAgICAgPyB0aGlzLnByb2Nlc3NUcmFuc2xhdGlvbnMoT2JqZWN0LmFzc2lnbihyZXMsIGNuLCB0cmFuc2xhdGVKc29uKSlcclxuICAgIC8vICAgICAgIDogdGhpcy5wcm9jZXNzVHJhbnNsYXRpb25zKE9iamVjdC5hc3NpZ24ocmVzLCBlbiwgdHJhbnNsYXRlSnNvbikpXHJcbiAgICAvLyAgIClcclxuICAgIC8vICk7XHJcbiAgICAgICAgICAvLyAtLS0tLS0tLS0tLS0tLXJvbW92ZSBsYW5nIGh0dHAgcmV1cWVzdC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLTsgXHJcbiAgfVxyXG5cclxuICBwcml2YXRlIHByb2Nlc3NUcmFuc2xhdGlvbnModHJhbnNsYXRpb25zOiBhbnkpIHtcclxuICAgIGNvbnN0IG5ld1RyYW5zbGF0aW9uczogYW55ID0ge307XHJcbiAgICAvLyBpZiAoVHJhbnNsYXRpb25BcGlTZXJ2aWNlLmlzSnNvblN0cmluZyh0cmFuc2xhdGlvbnMpKSB7XHJcbiAgICAvLyAgIHRyYW5zbGF0aW9ucyA9IEpTT04ucGFyc2UodHJhbnNsYXRpb25zKTtcclxuICAgIC8vIH1cclxuICAgIGZvciAoY29uc3Qga2V5IGluIHRyYW5zbGF0aW9ucykge1xyXG4gICAgICBpZiAodHJhbnNsYXRpb25zLmhhc093blByb3BlcnR5KGtleSkpIHtcclxuICAgICAgICBpZiAodHlwZW9mIHRyYW5zbGF0aW9uc1trZXldID09PSAnb2JqZWN0Jykge1xyXG4gICAgICAgICAgbmV3VHJhbnNsYXRpb25zW2tleV0gPSB0aGlzLnByb2Nlc3NUcmFuc2xhdGlvbnModHJhbnNsYXRpb25zW2tleV0pO1xyXG4gICAgICAgIH0gZWxzZSBpZiAodHlwZW9mIHRyYW5zbGF0aW9uc1trZXldID09PSAnc3RyaW5nJyAmJiB0cmFuc2xhdGlvbnNba2V5XSA9PT0gJ04vQScpIHtcclxuICAgICAgICAgIC8vIFJlbW92ZSBOL0EgZnJvbSB0aGUgdHJhbnNsYXRpb25zIHJlc3BvbnNlXHJcbiAgICAgICAgfSBlbHNlIHtcclxuICAgICAgICAgIG5ld1RyYW5zbGF0aW9uc1trZXldID0gdHJhbnNsYXRpb25zW2tleV07XHJcbiAgICAgICAgfVxyXG4gICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgdGhpcy50cmFuc2xhdGVTdGF0dXMgPSB0cnVlXHJcbiAgICBjb25zdCBsYW5nID0gbG9jYWxTdG9yYWdlLmdldEl0ZW0oJ2xhbmcnKSB8fCAnZW4nXHJcbiAgICBsb2NhbFN0b3JhZ2Uuc2V0SXRlbShsYW5nLEpTT04uc3RyaW5naWZ5KG5ld1RyYW5zbGF0aW9ucykpXHJcbiAgICByZXR1cm4gbmV3VHJhbnNsYXRpb25zO1xyXG4gIH1cclxuXHJcbiAgcHJpdmF0ZSBnZXRMYW5ndWFnZUJ5Q29kZShjb2RlOiBzdHJpbmcpOiBMYW5ndWFnZSB8IHVuZGVmaW5lZCB7XHJcbiAgICByZXR1cm4gdGhpcy5hcHBJbmZvLmdldEFwcCgpPy5sYW5ndWFnZXMuZmluZCgobDphbnkpID0+IGwuY29kZSA9PSBjb2RlKTtcclxuICB9XHJcblxyXG4gIHByaXZhdGUgc3RhdGljIGlzSnNvblN0cmluZyhzdHI6IHN0cmluZykge1xyXG4gICAgdHJ5IHtcclxuICAgICAgY29uc3Qgb2JqID0gSlNPTi5wYXJzZShzdHIpO1xyXG4gICAgICByZXR1cm4gISEodHlwZW9mIG9iaiA9PSAnb2JqZWN0JyAmJiBvYmopO1xyXG4gICAgfSBjYXRjaCAoZSkge1xyXG4gICAgICBjb25zb2xlLmxvZygnZXJyb3LvvJonICsgc3RyICsgJyEhIScgKyBlKTtcclxuICAgICAgcmV0dXJuIGZhbHNlO1xyXG4gICAgfVxyXG4gIH1cclxuXHJcbiAgZ2V0VHJhbnNsYXRlU3RhdHVzKCkge1xyXG4gICAgcmV0dXJuIHRoaXMudHJhbnNsYXRlU3RhdHVzXHJcbiAgfVxyXG59XHJcbiJdfQ==