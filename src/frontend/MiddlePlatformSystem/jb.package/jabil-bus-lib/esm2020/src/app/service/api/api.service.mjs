/**
 * name:http service
 * describe: http common module
 */
import { Injectable } from '@angular/core';
import { HttpRequest, HttpHeaders } from '@angular/common/http';
import * as i0 from "@angular/core";
import * as i1 from "@angular/common/http";
let self = {};
let isDownload = false;
let selfName = '';
export class HttpService {
    constructor(http) {
        this.http = http;
        this.self = {};
        self = this;
    }
    /**
     * request
     * @param params
     * @param header
     * @returns {Promise<{success: boolean,msg: string}>|Promise<R>}
     */
    request(params, header) {
        isDownload = false;
        const method = params['method'].toLowerCase();
        if (method === 'post') {
            return this.post(params['url'], params['data'], header);
        }
        else if (method === 'delete') {
            return this.delete(params['url'], params['data'], header);
        }
        else if (method === 'put') {
            return this.put(params['url'], params['data'], header);
        }
        else if (method === 'download') {
            return this.download(params['url'], params['data'], header);
        }
        else if (method === 'downloadfile') {
            return this.downloadFile(params['url'], params['data'], header, params['fileName']);
        }
        else if (method === 'upload') {
            return this.upload(params['url'], params['data'], header);
        }
        else {
            return this.get(params['url'], params['data'], header);
        }
    }
    /**
     * get
     * @param url
     * @param params
     * @param header
     * @returns {Promise<R>|Promise<U>}
     */
    get(url, params, header) {
        let jwt = localStorage.getItem('jwt');
        let headers = { 'Content-Type': 'application/json-patch+json' };
        if (jwt) {
            headers = {
                'Content-Type': 'application/json-patch+json',
                Authorization: 'Bearer ' + jwt,
            };
        }
        if (header) {
            headers = Object.assign(headers, header);
        }
        let options = {
            params,
            headers,
        };
        return this.http
            .get(url, options)
            .toPromise()
            .then(HttpService.handleSuccess)
            .catch(res => HttpService.handleError(res));
    }
    /**
     * post
     * @param url
     * @param params
     * @param header
     * @returns {Promise<R>|Promise<U>}
     */
    post(url, params, header) {
        let jwt = localStorage.getItem('jwt');
        let lang = localStorage.getItem('lang');
        let headers = { 'Content-Type': 'application/json-patch+json' };
        if (jwt) {
            headers = {
                'Content-Type': 'application/json-patch+json',
                'Accept-Language': lang === 'en' ? 'en' : 'zh-Hans',
                Authorization: 'Bearer ' + jwt,
            };
        }
        if (header) {
            headers = Object.assign(headers, header);
        }
        let options = {
            headers,
        };
        if (headers['Content-Type'] && headers['Content-Type'].includes('form-data')) {
            const formData = params;
            return this.http
                .post(url, formData, options)
                .toPromise()
                .then(HttpService.handleSuccess)
                .catch(res => HttpService.handleError(res));
        }
        return this.http
            .post(url, params, options)
            .toPromise()
            .then(HttpService.handleSuccess)
            .catch(res => HttpService.handleError(res));
    }
    /**
     * delete
     * @param url
     * @param params
     * @param header
     * @returns {Promise<R>|Promise<U>}
     */
    delete(url, params, header) {
        let jwt = localStorage.getItem('jwt');
        let headers = { 'Content-Type': 'application/json-patch+json' };
        if (jwt) {
            headers = {
                'Content-Type': 'application/json-patch+json',
                Authorization: 'Bearer ' + jwt,
            };
        }
        if (header) {
            headers = Object.assign(headers, header);
        }
        let options = {
            body: params,
            headers,
        };
        return this.http
            .delete(url, options)
            .toPromise()
            .then(HttpService.handleSuccess)
            .catch(res => HttpService.handleError(res));
    }
    /**
     * put
     * @param url
     * @param params
     * @param header
     * @returns {Promise<R>|Promise<U>}
     */
    put(url, params, header) {
        let jwt = localStorage.getItem('jwt');
        let headers = { 'Content-Type': 'application/json-patch+json' };
        if (jwt) {
            headers = {
                'Content-Type': 'application/json-patch+json',
                Authorization: 'Bearer ' + jwt,
            };
        }
        if (header) {
            headers = Object.assign(headers, header);
        }
        let options = {
            headers,
        };
        return this.http
            .put(url, params, options)
            .toPromise()
            .then(HttpService.handleSuccess)
            .catch(res => HttpService.handleError(res));
    }
    /**
     * upload
     * @param url
     * @param params
     * @param header
     * @returns {Promise<R>|Promise<U>}
     */
    upload(url, params, header) {
        let jwt = localStorage.getItem('jwt');
        let headers = new HttpHeaders();
        headers.append('Content-Type', 'multipart/form-data');
        if (jwt) {
            headers.append('Authorization', 'Bearer ' + jwt);
        }
        let options = {
            headers,
        };
        if (header) {
            headers = Object.assign(headers, header);
        }
        const req = new HttpRequest('POST', url, params, {
            headers: headers,
            reportProgress: true,
        });
        return this.http
            .request(req)
            .toPromise()
            .then(HttpService.handleSuccess)
            .catch(res => HttpService.handleError(res));
    }
    /**
  * downloadFile
  * @param url
  * @param params
  * @param type
  * @param fileName
  * @returns {Promise<R>|Promise<U>}
  */
    downloadFile(url, params, type, fileName) {
        selfName = fileName || '';
        isDownload = true;
        let jwt = localStorage.getItem('jwt');
        let headers = new HttpHeaders({ Authorization: 'Bearer ' + jwt });
        const req = new HttpRequest(type || 'POST', url, params, {
            headers: headers,
            reportProgress: true,
            responseType: 'blob',
        });
        return this.http
            .request(req)
            .toPromise()
            .then(HttpService.handleSuccess)
            .catch(res => HttpService.handleError(res));
    }
    /**
     * download
     * @param url
     * @param params
     * @param type
     * @returns {Promise<R>|Promise<U>}
     */
    download(url, params, type) {
        selfName = '';
        isDownload = true;
        let jwt = localStorage.getItem('jwt');
        let headers = new HttpHeaders({ Authorization: 'Bearer ' + jwt });
        const req = new HttpRequest(type || 'POST', url, params, {
            headers: headers,
            reportProgress: true,
            responseType: 'blob',
        });
        return this.http
            .request(req)
            .toPromise()
            .then(HttpService.exportData)
            .catch(res => HttpService.handleError(res));
    }
    static exportData(res) {
        const blob = new Blob([res.body], {
            type: res.body.type,
        });
        const filename = selfName ||
            window.decodeURI(res.headers.get('content-disposition')?.split('=')[1]);
        const link = document.createElement('a');
        const url = URL.createObjectURL(blob);
        link.style.display = 'none';
        link.href = url;
        link.setAttribute('download', filename);
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        URL.revokeObjectURL(url); // revoke blob object
    }
    /**
     * success response
     * @param res
     * @returns {{data: (string|null|((node:any)=>any)
     */
    static handleSuccess(res) {
        // download
        if (res?.url?.includes('export') || res?.url?.includes('Export') ||
            res?.url?.includes('excel') || res?.url?.includes('Excel') ||
            isDownload) {
            const blob = new Blob([res.body], {
                type: res.body.type,
            });
            const filename = selfName ||
                window.decodeURI(res.headers.get('content-disposition')?.split('=')[1]);
            const link = document.createElement('a');
            const url = URL.createObjectURL(blob);
            link.style.display = 'none';
            link.href = url;
            link.setAttribute('download', filename);
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
            URL.revokeObjectURL(url); // revoke blob object
            return;
        }
        return res;
    }
    /**
     * error
     * @param error
     * @returns {void|Promise<string>|Promise<T>|any}
     */
    static handleError(error) {
        let msg = 'request error';
        if (error.status == 400) {
            console.log('please check parameter');
        }
        if (error.status == 401) {
            HttpService.logout();
        }
        if (error.status == 403) {
            console.log('No Permission');
            window.dispatchEvent(new CustomEvent('api-403'));
            // const messageService = new MessageService()
            // messageService.add({
            //   severity: 'warn',
            //   summary: 'No Permission',
            // });
        }
        if (error.status == 404) {
            console.error('please check link');
        }
        if (error.status == 500) {
            console.error('please check server');
        }
        return { code: -1, data: null, msg: error, };
    }
    static logout() {
        localStorage.removeItem('loginCheck');
        localStorage.removeItem('username');
        localStorage.removeItem('selectedIndex');
        localStorage.removeItem('jwt');
        localStorage.removeItem('tabs');
        localStorage.removeItem('tabMenu');
        localStorage.removeItem('region');
        localStorage.removeItem('translateApp');
        localStorage.removeItem('zh');
        localStorage.removeItem('en');
        window.location.reload();
        window.parent.location.reload();
    }
}
HttpService.ɵfac = function HttpService_Factory(t) { return new (t || HttpService)(i0.ɵɵinject(i1.HttpClient)); };
HttpService.ɵprov = /*@__PURE__*/ i0.ɵɵdefineInjectable({ token: HttpService, factory: HttpService.ɵfac });
(function () { (typeof ngDevMode === "undefined" || ngDevMode) && i0.ɵsetClassMetadata(HttpService, [{
        type: Injectable
    }], function () { return [{ type: i1.HttpClient }]; }, null); })();
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiYXBpLnNlcnZpY2UuanMiLCJzb3VyY2VSb290IjoiIiwic291cmNlcyI6WyIuLi8uLi8uLi8uLi8uLi8uLi9zcmMvYXBwL3NlcnZpY2UvYXBpL2FwaS5zZXJ2aWNlLnRzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiJBQUFBOzs7R0FHRztBQUNILE9BQU8sRUFBRSxVQUFVLEVBQUUsTUFBTSxlQUFlLENBQUM7QUFDM0MsT0FBTyxFQUFjLFdBQVcsRUFBRSxXQUFXLEVBQUUsTUFBTSxzQkFBc0IsQ0FBQzs7O0FBSTVFLElBQUksSUFBSSxHQUFRLEVBQUUsQ0FBQztBQUNuQixJQUFJLFVBQVUsR0FBWSxLQUFLLENBQUM7QUFDaEMsSUFBSSxRQUFRLEdBQVcsRUFBRSxDQUFDO0FBRzFCLE1BQU0sT0FBTyxXQUFXO0lBR3RCLFlBQW9CLElBQWdCO1FBQWhCLFNBQUksR0FBSixJQUFJLENBQVk7UUFGcEMsU0FBSSxHQUFRLEVBQUUsQ0FBQztRQUdiLElBQUksR0FBRyxJQUFJLENBQUM7SUFDZCxDQUFDO0lBRUQ7Ozs7O09BS0c7SUFDSSxPQUFPLENBQUMsTUFBVyxFQUFFLE1BQVk7UUFDdEMsVUFBVSxHQUFHLEtBQUssQ0FBQztRQUNuQixNQUFNLE1BQU0sR0FBRyxNQUFNLENBQUMsUUFBUSxDQUFDLENBQUMsV0FBVyxFQUFFLENBQUM7UUFDOUMsSUFBSSxNQUFNLEtBQUssTUFBTSxFQUFFO1lBQ3JCLE9BQU8sSUFBSSxDQUFDLElBQUksQ0FBQyxNQUFNLENBQUMsS0FBSyxDQUFDLEVBQUUsTUFBTSxDQUFDLE1BQU0sQ0FBQyxFQUFFLE1BQU0sQ0FBQyxDQUFDO1NBQ3pEO2FBQU0sSUFBSSxNQUFNLEtBQUssUUFBUSxFQUFFO1lBQzlCLE9BQU8sSUFBSSxDQUFDLE1BQU0sQ0FBQyxNQUFNLENBQUMsS0FBSyxDQUFDLEVBQUUsTUFBTSxDQUFDLE1BQU0sQ0FBQyxFQUFFLE1BQU0sQ0FBQyxDQUFDO1NBQzNEO2FBQU0sSUFBSSxNQUFNLEtBQUssS0FBSyxFQUFFO1lBQzNC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QUMsVUFBVSxDQUFDLE1BQU0sQ0FBQyxDQUFDO1FBQ2hDLFlBQVksQ0FBQyxVQUFVLENBQUMsU0FBUyxDQUFDLENBQUM7UUFDbkMsWUFBWSxDQUFDLFVBQVUsQ0FBQyxRQUFRLENBQUMsQ0FBQztRQUNsQyxZQUFZLENBQUMsVUFBVSxDQUFDLGNBQWMsQ0FBQyxDQUFDO1FBQ3hDLFlBQVksQ0FBQyxVQUFVLENBQUMsSUFBSSxDQUFDLENBQUM7UUFDOUIsWUFBWSxDQUFDLFVBQVUsQ0FBQyxJQUFJLENBQUMsQ0FBQztRQUM5QixNQUFNLENBQUMsUUFBUSxDQUFDLE1BQU0sRUFBRSxDQUFDO1FBQ3pCLE1BQU0sQ0FBQyxNQUFNLENBQUMsUUFBUSxDQUFDLE1BQU0sRUFBRSxDQUFDO0lBQ2xDLENBQUM7O3NFQTNWVSxXQUFXO2lFQUFYLFdBQVcsV0FBWCxXQUFXO3VGQUFYLFdBQVc7Y0FEdkIsVUFBVSIsInNvdXJjZXNDb250ZW50IjpbIi8qKlxyXG4gKiBuYW1lOmh0dHAgc2VydmljZVxyXG4gKiBkZXNjcmliZTogaHR0cCBjb21tb24gbW9kdWxlXHJcbiAqL1xyXG5pbXBvcnQgeyBJbmplY3RhYmxlIH0gZnJvbSAnQGFuZ3VsYXIvY29yZSc7XHJcbmltcG9ydCB7IEh0dHBDbGllbnQsIEh0dHBSZXF1ZXN0LCBIdHRwSGVhZGVycyB9IGZyb20gJ0Bhbmd1bGFyL2NvbW1vbi9odHRwJztcclxuaW1wb3J0IHsgTWVzc2FnZVNlcnZpY2UgfSBmcm9tICdwcmltZW5nL2FwaSc7XHJcbmltcG9ydCB7IExvYWRpbmdDb21wb25lbnQgfSBmcm9tICdzcmMvYXBwL2NvbXBvbmVudHMvbG9hZGluZy9sb2FkaW5nLmNvbXBvbmVudCc7XHJcblxyXG5sZXQgc2VsZjogYW55ID0ge307XHJcbmxldCBpc0Rvd25sb2FkOiBib29sZWFuID0gZmFsc2U7XHJcbmxldCBzZWxmTmFtZTogc3RyaW5nID0gJyc7XHJcblxyXG5ASW5qZWN0YWJsZSgpXHJcbmV4cG9ydCBjbGFzcyBIdHRwU2VydmljZSB7XHJcbiAgc2VsZjogYW55ID0ge307XHJcblxyXG4gIGNvbnN0cnVjdG9yKHByaXZhdGUgaHR0cDogSHR0cENsaWVudCkge1xyXG4gICAgc2VsZiA9IHRoaXM7XHJcbiAgfVxyXG5cclxuICAvKipcclxuICAgKiByZXF1ZXN0XHJcbiAgICogQHBhcmFtIHBhcmFtc1xyXG4gICAqIEBwYXJhbSBoZWFkZXJcclxuICAgKiBAcmV0dXJucyB7UHJvbWlzZTx7c3VjY2VzczogYm9vbGVhbixtc2c6IHN0cmluZ30+fFByb21pc2U8Uj59XHJcbiAgICovXHJcbiAgcHVibGljIHJlcXVlc3QocGFyYW1zOiBhbnksIGhlYWRlcj86IGFueSk6IGFueSB7XHJcbiAgICBpc0Rvd25sb2FkID0gZmFsc2U7XHJcbiAgICBjb25zdCBtZXRob2QgPSBwYXJhbXNbJ21ldGhvZCddLnRvTG93ZXJDYXNlKCk7XHJcbiAgICBpZiAobWV0aG9kID09PSAncG9zdCcpIHtcclxuICAgICAgcmV0dXJuIHRoaXMucG9zdChwYXJhbXNbJ3VybCddLCBwYXJhbXNbJ2RhdGEnXSwgaGVhZGVyKTtcclxuICAgIH0gZWxzZSBpZiAobWV0aG9kID09PSAnZGVsZXRlJykge1xyXG4gICAgICByZXR1cm4gdGhpcy5kZWxldGUocGFyYW1zWyd1cmwnXSwgcGFyYW1zWydkYXRhJ10sIGhlYWRlcik7XHJcbiAgICB9IGVsc2UgaWYgKG1ldGhvZCA9PT0gJ3B1dCcpIHtcclxuICAgICAgcmV0dXJuIHRoaXMucHV0KHBhcmFtc1sndXJsJ10sIHBhcmFtc1snZGF0YSddLCBoZWFkZXIpO1xyXG4gICAgfSBlbHNlIGlmIChtZXRob2QgPT09ICdkb3dubG9hZCcpIHtcclxuICAgICAgcmV0dXJuIHRoaXMuZG93bmxvYWQocGFyYW1zWyd1cmwnXSwgcGFyYW1zWydkYXRhJ10sIGhlYWRlcik7XHJcbiAgICB9IGVsc2UgaWYgKG1ldGhvZCA9PT0gJ2Rvd25sb2FkZmlsZScpIHtcclxuICAgICAgcmV0dXJuIHRoaXMuZG93bmxvYWRGaWxlKHBhcmFtc1sndXJsJ10sIHBhcmFtc1snZGF0YSddLCBoZWFkZXIsIHBhcmFtc1snZmlsZU5hbWUnXSk7XHJcbiAgICB9IGVsc2UgaWYgKG1ldGhvZCA9PT0gJ3VwbG9hZCcpIHtcclxuICAgICAgcmV0dXJuIHRoaXMudXBsb2FkKHBhcmFtc1sndXJsJ10sIHBhcmFtc1snZGF0YSddLCBoZWFkZXIpO1xyXG4gICAgfSBlbHNlIHtcclxuICAgICAgcmV0dXJuIHRoaXMuZ2V0KHBhcmFtc1sndXJsJ10sIHBhcmFtc1snZGF0YSddLCBoZWFkZXIpO1xyXG4gICAgfVxyXG4gIH1cclxuXHJcbiAgLyoqXHJcbiAgICogZ2V0XHJcbiAgICogQHBhcmFtIHVybFxyXG4gICAqIEBwYXJhbSBwYXJhbXNcclxuICAgKiBAcGFyYW0gaGVhZGVyXHJcbiAgICogQHJldHVybnMge1Byb21pc2U8Uj58UHJvbWlzZTxVPn1cclxuICAgKi9cclxuICBwcml2YXRlIGdldCh1cmw6IHN0cmluZywgcGFyYW1zOiBhbnksIGhlYWRlcj86IGFueSk6IGFueSB7XHJcbiAgICBsZXQgand0ID0gbG9jYWxTdG9yYWdlLmdldEl0ZW0oJ2p3dCcpO1xyXG4gICAgbGV0IGhlYWRlcnM6IGFueSA9IHsgJ0NvbnRlbnQtVHlwZSc6ICdhcHBsaWNhdGlvbi9qc29uLXBhdGNoK2pzb24nIH07XHJcbiAgICBpZiAoand0KSB7XHJcbiAgICAgIGhlYWRlcnMgPSB7XHJcbiAgICAgICAgJ0NvbnRlbnQtVHlwZSc6ICdhcHBsaWNhdGlvbi9qc29uLXBhdGNoK2pzb24nLFxyXG4gICAgICAgIEF1dGhvcml6YXRpb246ICdCZWFyZXIgJyArIGp3dCxcclxuICAgICAgfTtcclxuICAgIH1cclxuICAgIGlmIChoZWFkZXIpIHtcclxuICAgICAgaGVhZGVycyA9IE9iamVjdC5hc3NpZ24oaGVhZGVycywgaGVhZGVyKVxyXG4gICAgfVxyXG5cclxuICAgIGxldCBvcHRpb25zID0ge1xyXG4gICAgICBwYXJhbXMsXHJcbiAgICAgIGhlYWRlcnMsXHJcbiAgICB9O1xyXG5cclxuICAgIHJldHVybiB0aGlzLmh0dHBcclxuICAgICAgLmdldCh1cmwsIG9wdGlvbnMpXHJcbiAgICAgIC50b1Byb21pc2UoKVxyXG4gICAgICAudGhlbihIdHRwU2VydmljZS5oYW5kbGVTdWNjZXNzKVxyXG4gICAgICAuY2F0Y2gocmVzID0+IEh0dHBTZXJ2aWNlLmhhbmRsZUVycm9yKHJlcykpO1xyXG4gIH1cclxuXHJcbiAgLyoqXHJcbiAgICogcG9zdFxyXG4gICAqIEBwYXJhbSB1cmxcclxuICAgKiBAcGFyYW0gcGFyYW1zXHJcbiAgICogQHBhcmFtIGhlYWRlclxyXG4gICAqIEByZXR1cm5zIHtQcm9taXNlPFI+fFByb21pc2U8VT59XHJcbiAgICovXHJcbiAgcHJpdmF0ZSBwb3N0KHVybDogc3RyaW5nLCBwYXJhbXM6IGFueSwgaGVhZGVyPzogYW55KSB7XHJcbiAgICBsZXQgand0ID0gbG9jYWxTdG9yYWdlLmdldEl0ZW0oJ2p3dCcpO1xyXG4gICAgbGV0IGxhbmcgPSBsb2NhbFN0b3JhZ2UuZ2V0SXRlbSgnbGFuZycpO1xyXG4gICAgbGV0IGhlYWRlcnM6IGFueSA9IHsgJ0NvbnRlbnQtVHlwZSc6ICdhcHBsaWNhdGlvbi9qc29uLXBhdGNoK2pzb24nIH07XHJcbiAgICBpZiAoand0KSB7XHJcbiAgICAgIGhlYWRlcnMgPSB7XHJcbiAgICAgICAgJ0NvbnRlbnQtVHlwZSc6ICdhcHBsaWNhdGlvbi9qc29uLXBhdGNoK2pzb24nLFxyXG4gICAgICAgICdBY2NlcHQtTGFuZ3VhZ2UnOiBsYW5nID09PSAnZW4nID8gJ2VuJyA6ICd6aC1IYW5zJyxcclxuICAgICAgICBBdXRob3JpemF0aW9uOiAnQmVhcmVyICcgKyBqd3QsXHJcbiAgICAgIH07XHJcbiAgICB9XHJcblxyXG4gICAgaWYgKGhlYWRlcikge1xyXG4gICAgICBoZWFkZXJzID0gT2JqZWN0LmFzc2lnbihoZWFkZXJzLCBoZWFkZXIpXHJcbiAgICB9XHJcblxyXG4gICAgbGV0IG9wdGlvbnMgPSB7XHJcbiAgICAgIGhlYWRlcnMsXHJcbiAgICB9O1xyXG5cclxuICAgIGlmIChoZWFkZXJzWydDb250ZW50LVR5cGUnXSAmJiBoZWFkZXJzWydDb250ZW50LVR5cGUnXS5pbmNsdWRlcygnZm9ybS1kYXRhJykpIHtcclxuICAgICAgY29uc3QgZm9ybURhdGEgPSBwYXJhbXNcclxuICAgICAgcmV0dXJuIHRoaXMuaHR0cFxyXG4gICAgICAgIC5wb3N0KHVybCwgZm9ybURhdGEsIG9wdGlvbnMpXHJcbiAgICAgICAgLnRvUHJvbWlzZSgpXHJcbiAgICAgICAgLnRoZW4oSHR0cFNlcnZpY2UuaGFuZGxlU3VjY2VzcylcclxuICAgICAgICAuY2F0Y2gocmVzID0+IEh0dHBTZXJ2aWNlLmhhbmRsZUVycm9yKHJlcykpO1xyXG4gICAgfVxyXG5cclxuICAgIHJldHVybiB0aGlzLmh0dHBcclxuICAgICAgLnBvc3QodXJsLCBwYXJhbXMsIG9wdGlvbnMpXHJcbiAgICAgIC50b1Byb21pc2UoKVxyXG4gICAgICAudGhlbihIdHRwU2VydmljZS5oYW5kbGVTdWNjZXNzKVxyXG4gICAgICAuY2F0Y2gocmVzID0+IEh0dHBTZXJ2aWNlLmhhbmRsZUVycm9yKHJlcykpO1xyXG4gIH1cclxuXHJcbiAgLyoqXHJcbiAgICogZGVsZXRlXHJcbiAgICogQHBhcmFtIHVybFxyXG4gICAqIEBwYXJhbSBwYXJhbXNcclxuICAgKiBAcGFyYW0gaGVhZGVyXHJcbiAgICogQHJldHVybnMge1Byb21pc2U8Uj58UHJvbWlzZTxVPn1cclxuICAgKi9cclxuICBwcml2YXRlIGRlbGV0ZSh1cmw6IHN0cmluZywgcGFyYW1zOiBhbnksIGhlYWRlcj86IGFueSkge1xyXG4gICAgbGV0IGp3dCA9IGxvY2FsU3RvcmFnZS5nZXRJdGVtKCdqd3QnKTtcclxuICAgIGxldCBoZWFkZXJzOiBhbnkgPSB7ICdDb250ZW50LVR5cGUnOiAnYXBwbGljYXRpb24vanNvbi1wYXRjaCtqc29uJyB9O1xyXG4gICAgaWYgKGp3dCkge1xyXG4gICAgICBoZWFkZXJzID0ge1xyXG4gICAgICAgICdDb250ZW50LVR5cGUnOiAnYXBwbGljYXRpb24vanNvbi1wYXRjaCtqc29uJyxcclxuICAgICAgICBBdXRob3JpemF0aW9uOiAnQmVhcmVyICcgKyBqd3QsXHJcbiAgICAgIH07XHJcbiAgICB9XHJcblxyXG4gICAgaWYgKGhlYWRlcikge1xyXG4gICAgICBoZWFkZXJzID0gT2JqZWN0LmFzc2lnbihoZWFkZXJzLCBoZWFkZXIpXHJcbiAgICB9XHJcbiAgICBsZXQgb3B0aW9ucyA9IHtcclxuICAgICAgYm9keTogcGFyYW1zLFxyXG4gICAgICBoZWFkZXJzLFxyXG4gICAgfTtcclxuICAgIHJldHVybiB0aGlzLmh0dHBcclxuICAgICAgLmRlbGV0ZSh1cmwsIG9wdGlvbnMpXHJcbiAgICAgIC50b1Byb21pc2UoKVxyXG4gICAgICAudGhlbihIdHRwU2VydmljZS5oYW5kbGVTdWNjZXNzKVxyXG4gICAgICAuY2F0Y2gocmVzID0+IEh0dHBTZXJ2aWNlLmhhbmRsZUVycm9yKHJlcykpO1xyXG4gIH1cclxuXHJcbiAgLyoqXHJcbiAgICogcHV0XHJcbiAgICogQHBhcmFtIHVybFxyXG4gICAqIEBwYXJhbSBwYXJhbXNcclxuICAgKiBAcGFyYW0gaGVhZGVyXHJcbiAgICogQHJldHVybnMge1Byb21pc2U8Uj58UHJvbWlzZTxVPn1cclxuICAgKi9cclxuICBwcml2YXRlIHB1dCh1cmw6IHN0cmluZywgcGFyYW1zOiBhbnksIGhlYWRlcj86IGFueSkge1xyXG4gICAgbGV0IGp3dCA9IGxvY2FsU3RvcmFnZS5nZXRJdGVtKCdqd3QnKTtcclxuICAgIGxldCBoZWFkZXJzOiBhbnkgPSB7ICdDb250ZW50LVR5cGUnOiAnYXBwbGljYXRpb24vanNvbi1wYXRjaCtqc29uJyB9O1xyXG4gICAgaWYgKGp3dCkge1xyXG4gICAgICBoZWFkZXJzID0ge1xyXG4gICAgICAgICdDb250ZW50LVR5cGUnOiAnYXBwbGljYXRpb24vanNvbi1wYXRjaCtqc29uJyxcclxuICAgICAgICBBdXRob3JpemF0aW9uOiAnQmVhcmVyICcgKyBqd3QsXHJcbiAgICAgIH07XHJcbiAgICB9XHJcblxyXG4gICAgaWYgKGhlYWRlcikge1xyXG4gICAgICBoZWFkZXJzID0gT2JqZWN0LmFzc2lnbihoZWFkZXJzLCBoZWFkZXIpXHJcbiAgICB9XHJcblxyXG4gICAgbGV0IG9wdGlvbnMgPSB7XHJcbiAgICAgIGhlYWRlcnMsXHJcbiAgICB9O1xyXG4gICAgcmV0dXJuIHRoaXMuaHR0cFxyXG4gICAgICAucHV0KHVybCwgcGFyYW1zLCBvcHRpb25zKVxyXG4gICAgICAudG9Qcm9taXNlKClcclxuICAgICAgLnRoZW4oSHR0cFNlcnZpY2UuaGFuZGxlU3VjY2VzcylcclxuICAgICAgLmNhdGNoKHJlcyA9PiBIdHRwU2VydmljZS5oYW5kbGVFcnJvcihyZXMpKTtcclxuICB9XHJcblxyXG4gIC8qKlxyXG4gICAqIHVwbG9hZFxyXG4gICAqIEBwYXJhbSB1cmxcclxuICAgKiBAcGFyYW0gcGFyYW1zXHJcbiAgICogQHBhcmFtIGhlYWRlclxyXG4gICAqIEByZXR1cm5zIHtQcm9taXNlPFI+fFByb21pc2U8VT59XHJcbiAgICovXHJcbiAgcHJpdmF0ZSB1cGxvYWQodXJsOiBzdHJpbmcsIHBhcmFtczogYW55LCBoZWFkZXI/OiBhbnkpIHtcclxuICAgIGxldCBqd3QgPSBsb2NhbFN0b3JhZ2UuZ2V0SXRlbSgnand0Jyk7XHJcbiAgICBsZXQgaGVhZGVyczogSHR0cEhlYWRlcnMgPSBuZXcgSHR0cEhlYWRlcnMoKTtcclxuICAgIGhlYWRlcnMuYXBwZW5kKCdDb250ZW50LVR5cGUnLCAnbXVsdGlwYXJ0L2Zvcm0tZGF0YScpO1xyXG4gICAgaWYgKGp3dCkge1xyXG4gICAgICBoZWFkZXJzLmFwcGVuZCgnQXV0aG9yaXphdGlvbicsICdCZWFyZXIgJyArIGp3dCk7XHJcbiAgICB9XHJcbiAgICBsZXQgb3B0aW9ucyA9IHtcclxuICAgICAgaGVhZGVycyxcclxuICAgIH07XHJcblxyXG4gICAgaWYgKGhlYWRlcikge1xyXG4gICAgICBoZWFkZXJzID0gT2JqZWN0LmFzc2lnbihoZWFkZXJzLCBoZWFkZXIpXHJcbiAgICB9XHJcblxyXG4gICAgY29uc3QgcmVxID0gbmV3IEh0dHBSZXF1ZXN0KCdQT1NUJywgdXJsLCBwYXJhbXMsIHtcclxuICAgICAgaGVhZGVyczogaGVhZGVycyxcclxuICAgICAgcmVwb3J0UHJvZ3Jlc3M6IHRydWUsXHJcbiAgICB9KTtcclxuXHJcbiAgICByZXR1cm4gdGhpcy5odHRwXHJcbiAgICAgIC5yZXF1ZXN0KHJlcSlcclxuICAgICAgLnRvUHJvbWlzZSgpXHJcbiAgICAgIC50aGVuKEh0dHBTZXJ2aWNlLmhhbmRsZVN1Y2Nlc3MpXHJcbiAgICAgIC5jYXRjaChyZXMgPT4gSHR0cFNlcnZpY2UuaGFuZGxlRXJyb3IocmVzKSk7XHJcbiAgfVxyXG5cclxuICAvKipcclxuKiBkb3dubG9hZEZpbGVcclxuKiBAcGFyYW0gdXJsXHJcbiogQHBhcmFtIHBhcmFtc1xyXG4qIEBwYXJhbSB0eXBlXHJcbiogQHBhcmFtIGZpbGVOYW1lXHJcbiogQHJldHVybnMge1Byb21pc2U8Uj58UHJvbWlzZTxVPn1cclxuKi9cclxuICBwcml2YXRlIGRvd25sb2FkRmlsZSh1cmw6IHN0cmluZywgcGFyYW1zOiBhbnksIHR5cGU/OiBzdHJpbmcsIGZpbGVOYW1lPzogc3RyaW5nKSB7XHJcbiAgICBzZWxmTmFtZSA9IGZpbGVOYW1lIHx8ICcnXHJcbiAgICBpc0Rvd25sb2FkID0gdHJ1ZTtcclxuICAgIGxldCBqd3QgPSBsb2NhbFN0b3JhZ2UuZ2V0SXRlbSgnand0Jyk7XHJcbiAgICBsZXQgaGVhZGVyczogSHR0cEhlYWRlcnMgPSBuZXcgSHR0cEhlYWRlcnMoeyBBdXRob3JpemF0aW9uOiAnQmVhcmVyICcgKyBqd3QgfSk7XHJcblxyXG4gICAgY29uc3QgcmVxID0gbmV3IEh0dHBSZXF1ZXN0KHR5cGUgfHwgJ1BPU1QnLCB1cmwsIHBhcmFtcywge1xyXG4gICAgICBoZWFkZXJzOiBoZWFkZXJzLFxyXG4gICAgICByZXBvcnRQcm9ncmVzczogdHJ1ZSxcclxuICAgICAgcmVzcG9uc2VUeXBlOiAnYmxvYicsXHJcbiAgICB9KTtcclxuXHJcbiAgICByZXR1cm4gdGhpcy5odHRwXHJcbiAgICAgIC5yZXF1ZXN0KHJlcSlcclxuICAgICAgLnRvUHJvbWlzZSgpXHJcbiAgICAgIC50aGVuKEh0dHBTZXJ2aWNlLmhhbmRsZVN1Y2Nlc3MpXHJcbiAgICAgIC5jYXRjaChyZXMgPT4gSHR0cFNlcnZpY2UuaGFuZGxlRXJyb3IocmVzKSk7XHJcbiAgfVxyXG5cclxuICAvKipcclxuICAgKiBkb3dubG9hZFxyXG4gICAqIEBwYXJhbSB1cmxcclxuICAgKiBAcGFyYW0gcGFyYW1zXHJcbiAgICogQHBhcmFtIHR5cGVcclxuICAgKiBAcmV0dXJucyB7UHJvbWlzZTxSPnxQcm9taXNlPFU+fVxyXG4gICAqL1xyXG4gIHByaXZhdGUgZG93bmxvYWQodXJsOiBzdHJpbmcsIHBhcmFtczogYW55LCB0eXBlPzogc3RyaW5nKSB7XHJcbiAgICBzZWxmTmFtZSA9ICcnXHJcbiAgICBpc0Rvd25sb2FkID0gdHJ1ZTtcclxuICAgIGxldCBqd3QgPSBsb2NhbFN0b3JhZ2UuZ2V0SXRlbSgnand0Jyk7XHJcbiAgICBsZXQgaGVhZGVyczogSHR0cEhlYWRlcnMgPSBuZXcgSHR0cEhlYWRlcnMoeyBBdXRob3JpemF0aW9uOiAnQmVhcmVyICcgKyBqd3QgfSk7XHJcblxyXG4gICAgY29uc3QgcmVxID0gbmV3IEh0dHBSZXF1ZXN0KHR5cGUgfHwgJ1BPU1QnLCB1cmwsIHBhcmFtcywge1xyXG4gICAgICBoZWFkZXJzOiBoZWFkZXJzLFxyXG4gICAgICByZXBvcnRQcm9ncmVzczogdHJ1ZSxcclxuICAgICAgcmVzcG9uc2VUeXBlOiAnYmxvYicsXHJcbiAgICB9KTtcclxuXHJcbiAgICByZXR1cm4gdGhpcy5odHRwXHJcbiAgICAgIC5yZXF1ZXN0KHJlcSlcclxuICAgICAgLnRvUHJvbWlzZSgpXHJcbiAgICAgIC50aGVuKEh0dHBTZXJ2aWNlLmV4cG9ydERhdGEpXHJcbiAgICAgIC5jYXRjaChyZXMgPT4gSHR0cFNlcnZpY2UuaGFuZGxlRXJyb3IocmVzKSk7XHJcbiAgfVxyXG5cclxuICBwcml2YXRlIHN0YXRpYyBleHBvcnREYXRhKHJlczogYW55KSB7XHJcbiAgICBjb25zdCBibG9iID0gbmV3IEJsb2IoW3Jlcy5ib2R5XSwge1xyXG4gICAgICB0eXBlOiByZXMuYm9keS50eXBlLFxyXG4gICAgfSk7XHJcbiAgICBjb25zdCBmaWxlbmFtZSA9IHNlbGZOYW1lIHx8XHJcbiAgICAgIHdpbmRvdy5kZWNvZGVVUkkocmVzLmhlYWRlcnMuZ2V0KCdjb250ZW50LWRpc3Bvc2l0aW9uJyk/LnNwbGl0KCc9JylbMV0pO1xyXG4gICAgY29uc3QgbGluayA9IGRvY3VtZW50LmNyZWF0ZUVsZW1lbnQoJ2EnKTtcclxuICAgIGNvbnN0IHVybCA9IFVSTC5jcmVhdGVPYmplY3RVUkwoYmxvYik7XHJcbiAgICBsaW5rLnN0eWxlLmRpc3BsYXkgPSAnbm9uZSc7XHJcbiAgICBsaW5rLmhyZWYgPSB1cmw7XHJcbiAgICBsaW5rLnNldEF0dHJpYnV0ZSgnZG93bmxvYWQnLCBmaWxlbmFtZSk7XHJcbiAgICBkb2N1bWVudC5ib2R5LmFwcGVuZENoaWxkKGxpbmspO1xyXG4gICAgbGluay5jbGljaygpO1xyXG4gICAgZG9jdW1lbnQuYm9keS5yZW1vdmVDaGlsZChsaW5rKTtcclxuICAgIFVSTC5yZXZva2VPYmplY3RVUkwodXJsKTsgLy8gcmV2b2tlIGJsb2Igb2JqZWN0XHJcbiAgfVxyXG5cclxuICAvKipcclxuICAgKiBzdWNjZXNzIHJlc3BvbnNlXHJcbiAgICogQHBhcmFtIHJlc1xyXG4gICAqIEByZXR1cm5zIHt7ZGF0YTogKHN0cmluZ3xudWxsfCgobm9kZTphbnkpPT5hbnkpXHJcbiAgICovXHJcbiAgcHJpdmF0ZSBzdGF0aWMgaGFuZGxlU3VjY2VzcyhyZXM6IGFueSkge1xyXG4gICAgLy8gZG93bmxvYWRcclxuICAgIGlmIChyZXM/LnVybD8uaW5jbHVkZXMoJ2V4cG9ydCcpIHx8IHJlcz8udXJsPy5pbmNsdWRlcygnRXhwb3J0JykgfHxcclxuICAgICAgcmVzPy51cmw/LmluY2x1ZGVzKCdleGNlbCcpIHx8IHJlcz8udXJsPy5pbmNsdWRlcygnRXhjZWwnKSB8fFxyXG4gICAgICBpc0Rvd25sb2FkKSB7XHJcbiAgICAgIGNvbnN0IGJsb2IgPSBuZXcgQmxvYihbcmVzLmJvZHldLCB7XHJcbiAgICAgICAgdHlwZTogcmVzLmJvZHkudHlwZSxcclxuICAgICAgfSk7XHJcbiAgICAgIGNvbnN0IGZpbGVuYW1lID0gc2VsZk5hbWUgfHxcclxuICAgICAgICB3aW5kb3cuZGVjb2RlVVJJKHJlcy5oZWFkZXJzLmdldCgnY29udGVudC1kaXNwb3NpdGlvbicpPy5zcGxpdCgnPScpWzFdKTtcclxuICAgICAgY29uc3QgbGluayA9IGRvY3VtZW50LmNyZWF0ZUVsZW1lbnQoJ2EnKTtcclxuICAgICAgY29uc3QgdXJsID0gVVJMLmNyZWF0ZU9iamVjdFVSTChibG9iKTtcclxuICAgICAgbGluay5zdHlsZS5kaXNwbGF5ID0gJ25vbmUnO1xyXG4gICAgICBsaW5rLmhyZWYgPSB1cmw7XHJcbiAgICAgIGxpbmsuc2V0QXR0cmlidXRlKCdkb3dubG9hZCcsIGZpbGVuYW1lKTtcclxuICAgICAgZG9jdW1lbnQuYm9keS5hcHBlbmRDaGlsZChsaW5rKTtcclxuICAgICAgbGluay5jbGljaygpO1xyXG4gICAgICBkb2N1bWVudC5ib2R5LnJlbW92ZUNoaWxkKGxpbmspO1xyXG4gICAgICBVUkwucmV2b2tlT2JqZWN0VVJMKHVybCk7IC8vIHJldm9rZSBibG9iIG9iamVjdFxyXG4gICAgICByZXR1cm47XHJcbiAgICB9XHJcbiAgICByZXR1cm4gcmVzO1xyXG4gIH1cclxuXHJcbiAgLyoqXHJcbiAgICogZXJyb3JcclxuICAgKiBAcGFyYW0gZXJyb3JcclxuICAgKiBAcmV0dXJucyB7dm9pZHxQcm9taXNlPHN0cmluZz58UHJvbWlzZTxUPnxhbnl9XHJcbiAgICovXHJcbiAgcHJpdmF0ZSBzdGF0aWMgaGFuZGxlRXJyb3IoZXJyb3I6IGFueSkge1xyXG4gICAgbGV0IG1zZyA9ICdyZXF1ZXN0IGVycm9yJztcclxuICAgIGlmIChlcnJvci5zdGF0dXMgPT0gNDAwKSB7XHJcbiAgICAgIGNvbnNvbGUubG9nKCdwbGVhc2UgY2hlY2sgcGFyYW1ldGVyJyk7XHJcbiAgICB9XHJcbiAgICBpZiAoZXJyb3Iuc3RhdHVzID09IDQwMSkge1xyXG4gICAgICBIdHRwU2VydmljZS5sb2dvdXQoKTtcclxuICAgIH1cclxuICAgIGlmIChlcnJvci5zdGF0dXMgPT0gNDAzKSB7XHJcbiAgICAgIGNvbnNvbGUubG9nKCdObyBQZXJtaXNzaW9uJyk7XHJcbiAgICAgIHdpbmRvdy5kaXNwYXRjaEV2ZW50KG5ldyBDdXN0b21FdmVudCgnYXBpLTQwMycpKTtcclxuICAgICAgLy8gY29uc3QgbWVzc2FnZVNlcnZpY2UgPSBuZXcgTWVzc2FnZVNlcnZpY2UoKVxyXG4gICAgICAvLyBtZXNzYWdlU2VydmljZS5hZGQoe1xyXG4gICAgICAvLyAgIHNldmVyaXR5OiAnd2FybicsXHJcbiAgICAgIC8vICAgc3VtbWFyeTogJ05vIFBlcm1pc3Npb24nLFxyXG4gICAgICAvLyB9KTtcclxuICAgIH1cclxuICAgIGlmIChlcnJvci5zdGF0dXMgPT0gNDA0KSB7XHJcbiAgICAgIGNvbnNvbGUuZXJyb3IoJ3BsZWFzZSBjaGVjayBsaW5rJyk7XHJcbiAgICB9XHJcbiAgICBpZiAoZXJyb3Iuc3RhdHVzID09IDUwMCkge1xyXG4gICAgICBjb25zb2xlLmVycm9yKCdwbGVhc2UgY2hlY2sgc2VydmVyJyk7XHJcbiAgICB9XHJcbiAgICByZXR1cm4geyBjb2RlOiAtMSwgZGF0YTogbnVsbCwgbXNnOiBlcnJvciwgfTtcclxuICB9XHJcblxyXG4gIHByaXZhdGUgc3RhdGljIGxvZ291dCgpIHtcclxuICAgIGxvY2FsU3RvcmFnZS5yZW1vdmVJdGVtKCdsb2dpbkNoZWNrJyk7XHJcbiAgICBsb2NhbFN0b3JhZ2UucmVtb3ZlSXRlbSgndXNlcm5hbWUnKTtcclxuICAgIGxvY2FsU3RvcmFnZS5yZW1vdmVJdGVtKCdzZWxlY3RlZEluZGV4Jyk7XHJcbiAgICBsb2NhbFN0b3JhZ2UucmVtb3ZlSXRlbSgnand0Jyk7XHJcbiAgICBsb2NhbFN0b3JhZ2UucmVtb3ZlSXRlbSgndGFicycpO1xyXG4gICAgbG9jYWxTdG9yYWdlLnJlbW92ZUl0ZW0oJ3RhYk1lbnUnKTtcclxuICAgIGxvY2FsU3RvcmFnZS5yZW1vdmVJdGVtKCdyZWdpb24nKTtcclxuICAgIGxvY2FsU3RvcmFnZS5yZW1vdmVJdGVtKCd0cmFuc2xhdGVBcHAnKTtcclxuICAgIGxvY2FsU3RvcmFnZS5yZW1vdmVJdGVtKCd6aCcpO1xyXG4gICAgbG9jYWxTdG9yYWdlLnJlbW92ZUl0ZW0oJ2VuJyk7XHJcbiAgICB3aW5kb3cubG9jYXRpb24ucmVsb2FkKCk7XHJcbiAgICB3aW5kb3cucGFyZW50LmxvY2F0aW9uLnJlbG9hZCgpO1xyXG4gIH1cclxufVxyXG5cclxuIl19