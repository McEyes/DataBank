import { Injectable } from '@angular/core';
import * as i0 from "@angular/core";
import * as i1 from "../app-info/app-info.service";
export class MissingTranslationsService {
    constructor(appInfo) {
        this.appInfo = appInfo;
    }
    handle(params) {
        const namespaces = this.appInfo.getApp()?.namespaces;
        let key = `${params.key}`;
        if (namespaces) {
            for (let ns of namespaces) {
                if (key.indexOf(ns.namespace) !== -1) {
                    key = key.replace(`${ns.namespace}.`, '');
                    break;
                }
            }
        }
        return key;
    }
}
MissingTranslationsService.ɵfac = function MissingTranslationsService_Factory(t) { return new (t || MissingTranslationsService)(i0.ɵɵinject(i1.AppInfoService)); };
MissingTranslationsService.ɵprov = /*@__PURE__*/ i0.ɵɵdefineInjectable({ token: MissingTranslationsService, factory: MissingTranslationsService.ɵfac, providedIn: 'root' });
(function () { (typeof ngDevMode === "undefined" || ngDevMode) && i0.ɵsetClassMetadata(MissingTranslationsService, [{
        type: Injectable,
        args: [{
                providedIn: 'root',
            }]
    }], function () { return [{ type: i1.AppInfoService }]; }, null); })();
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoibWlzc2luZy10cmFuc2xhdGlvbnMuc2VydmljZS5qcyIsInNvdXJjZVJvb3QiOiIiLCJzb3VyY2VzIjpbIi4uLy4uLy4uLy4uLy4uLy4uL3NyYy9hcHAvc2VydmljZS90cmFuc2xhdGUvbWlzc2luZy10cmFuc2xhdGlvbnMuc2VydmljZS50cyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFBQSxPQUFPLEVBQUUsVUFBVSxFQUFFLE1BQU0sZUFBZSxDQUFDOzs7QUFPM0MsTUFBTSxPQUFPLDBCQUEwQjtJQUNyQyxZQUFvQixPQUF1QjtRQUF2QixZQUFPLEdBQVAsT0FBTyxDQUFnQjtJQUFHLENBQUM7SUFFL0MsTUFBTSxDQUFDLE1BQXVDO1FBQzVDLE1BQU0sVUFBVSxHQUFHLElBQUksQ0FBQyxPQUFPLENBQUMsTUFBTSxFQUFFLEVBQUUsVUFBVSxDQUFDO1FBQ3JELElBQUksR0FBRyxHQUFHLEdBQUcsTUFBTSxDQUFDLEdBQUcsRUFBRSxDQUFDO1FBQzFCLElBQUksVUFBVSxFQUFFO1lBQ2QsS0FBSyxJQUFJLEVBQUUsSUFBSSxVQUFVLEVBQUU7Z0JBQ3pCLElBQUksR0FBRyxDQUFDLE9BQU8sQ0FBQyxFQUFFLENBQUMsU0FBUyxDQUFDLEtBQUssQ0FBQyxDQUFDLEVBQUU7b0JBQ3BDLEdBQUcsR0FBRyxHQUFHLENBQUMsT0FBTyxDQUFDLEdBQUcsRUFBRSxDQUFDLFNBQVMsR0FBRyxFQUFFLEVBQUUsQ0FBQyxDQUFDO29CQUMxQyxNQUFNO2lCQUNQO2FBQ0Y7U0FDRjtRQUNELE9BQU8sR0FBRyxDQUFDO0lBQ2IsQ0FBQzs7b0dBZlUsMEJBQTBCO2dGQUExQiwwQkFBMEIsV0FBMUIsMEJBQTBCLG1CQUZ6QixNQUFNO3VGQUVQLDBCQUEwQjtjQUh0QyxVQUFVO2VBQUM7Z0JBQ1YsVUFBVSxFQUFFLE1BQU07YUFDbkIiLCJzb3VyY2VzQ29udGVudCI6WyJpbXBvcnQgeyBJbmplY3RhYmxlIH0gZnJvbSAnQGFuZ3VsYXIvY29yZSc7XHJcbmltcG9ydCB7IE1pc3NpbmdUcmFuc2xhdGlvbkhhbmRsZXIsIE1pc3NpbmdUcmFuc2xhdGlvbkhhbmRsZXJQYXJhbXMgfSBmcm9tICdAbmd4LXRyYW5zbGF0ZS9jb3JlJztcclxuaW1wb3J0IHsgQXBwSW5mb1NlcnZpY2UgfSBmcm9tICcuLi9hcHAtaW5mby9hcHAtaW5mby5zZXJ2aWNlJztcclxuXHJcbkBJbmplY3RhYmxlKHtcclxuICBwcm92aWRlZEluOiAncm9vdCcsXHJcbn0pXHJcbmV4cG9ydCBjbGFzcyBNaXNzaW5nVHJhbnNsYXRpb25zU2VydmljZSBpbXBsZW1lbnRzIE1pc3NpbmdUcmFuc2xhdGlvbkhhbmRsZXIge1xyXG4gIGNvbnN0cnVjdG9yKHByaXZhdGUgYXBwSW5mbzogQXBwSW5mb1NlcnZpY2UpIHt9XHJcblxyXG4gIGhhbmRsZShwYXJhbXM6IE1pc3NpbmdUcmFuc2xhdGlvbkhhbmRsZXJQYXJhbXMpIHtcclxuICAgIGNvbnN0IG5hbWVzcGFjZXMgPSB0aGlzLmFwcEluZm8uZ2V0QXBwKCk/Lm5hbWVzcGFjZXM7XHJcbiAgICBsZXQga2V5ID0gYCR7cGFyYW1zLmtleX1gO1xyXG4gICAgaWYgKG5hbWVzcGFjZXMpIHtcclxuICAgICAgZm9yIChsZXQgbnMgb2YgbmFtZXNwYWNlcykge1xyXG4gICAgICAgIGlmIChrZXkuaW5kZXhPZihucy5uYW1lc3BhY2UpICE9PSAtMSkge1xyXG4gICAgICAgICAga2V5ID0ga2V5LnJlcGxhY2UoYCR7bnMubmFtZXNwYWNlfS5gLCAnJyk7XHJcbiAgICAgICAgICBicmVhaztcclxuICAgICAgICB9XHJcbiAgICAgIH1cclxuICAgIH1cclxuICAgIHJldHVybiBrZXk7XHJcbiAgfVxyXG59XHJcbiJdfQ==