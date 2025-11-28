import { HttpClient } from '@angular/common/http';
import * as i0 from "@angular/core";
export declare class HttpService {
    private http;
    self: any;
    constructor(http: HttpClient);
    /**
     * request
     * @param params
     * @param header
     * @returns {Promise<{success: boolean,msg: string}>|Promise<R>}
     */
    request(params: any, header?: any): any;
    /**
     * get
     * @param url
     * @param params
     * @param header
     * @returns {Promise<R>|Promise<U>}
     */
    private get;
    /**
     * post
     * @param url
     * @param params
     * @param header
     * @returns {Promise<R>|Promise<U>}
     */
    private post;
    /**
     * delete
     * @param url
     * @param params
     * @param header
     * @returns {Promise<R>|Promise<U>}
     */
    private delete;
    /**
     * put
     * @param url
     * @param params
     * @param header
     * @returns {Promise<R>|Promise<U>}
     */
    private put;
    /**
     * upload
     * @param url
     * @param params
     * @param header
     * @returns {Promise<R>|Promise<U>}
     */
    private upload;
    /**
  * downloadFile
  * @param url
  * @param params
  * @param type
  * @param fileName
  * @returns {Promise<R>|Promise<U>}
  */
    private downloadFile;
    /**
     * download
     * @param url
     * @param params
     * @param type
     * @returns {Promise<R>|Promise<U>}
     */
    private download;
    private static exportData;
    /**
     * success response
     * @param res
     * @returns {{data: (string|null|((node:any)=>any)
     */
    private static handleSuccess;
    /**
     * error
     * @param error
     * @returns {void|Promise<string>|Promise<T>|any}
     */
    private static handleError;
    private static logout;
    static ɵfac: i0.ɵɵFactoryDeclaration<HttpService, never>;
    static ɵprov: i0.ɵɵInjectableDeclaration<HttpService>;
}
