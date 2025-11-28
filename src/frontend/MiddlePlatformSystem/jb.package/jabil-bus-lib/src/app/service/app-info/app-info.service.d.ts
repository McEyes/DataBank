import { Observable, BehaviorSubject } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import * as i0 from "@angular/core";
export declare class AppInfoService {
    private http;
    private _domainUri;
    private _apiKey;
    private application;
    constructor(http: HttpClient);
    getAppInfo(): Observable<Object>;
    getData(route: string, httpOptions: any): Observable<ArrayBuffer>;
    getCacheData(key: string): BehaviorSubject<string>;
    setApp(val: any): void;
    getApp(): any;
    static ɵfac: i0.ɵɵFactoryDeclaration<AppInfoService, never>;
    static ɵprov: i0.ɵɵInjectableDeclaration<AppInfoService>;
}
