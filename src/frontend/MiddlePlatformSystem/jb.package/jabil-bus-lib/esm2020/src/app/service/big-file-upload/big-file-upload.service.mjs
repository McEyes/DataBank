import { Injectable } from '@angular/core';
import { HttpHeaders } from '@angular/common/http';
import { Md5 } from 'ts-md5/dist/cjs';
import * as i0 from "@angular/core";
import * as i1 from "@angular/common/http";
export class BigFileUploadService {
    constructor(httpClient) {
        this.httpClient = httpClient;
        this.progressShow = false;
        this.isCancel = false;
        this.fileProgress = 0;
        this.totalPieces = 0;
        this.firmwareFileList = [];
        this.bytesPerPiece = null;
        this.baseUrl = '';
    }
    inputFileChange(e, successFn, failFn, progressFn) {
        this.firmwareFileList = [];
        this.baseUrl = e.url;
        const file = e.file;
        if (file) {
            this.firmwareFileList = file;
            // slice file
            if (file.size > 1024 * 1024 * 100) {
                this.bytesPerPiece = Math.floor(file.size / 100);
            }
            else {
                this.bytesPerPiece = 1024 * 1024 * 1;
            }
            this.progressShow = true;
            this.fileProgress = 0;
            let start = 0;
            let index = 0;
            setTimeout(() => {
                this.loopSend(this.firmwareFileList, start, index, successFn, failFn, progressFn);
            }, 100);
            // e.target.value = '';
        }
    }
    // upload file
    loopSend(item, start, index, successFn, failFn, progressFn) {
        if (this.isCancel) {
            return;
        }
        let that = this;
        const size = item.size || 1;
        const fileName = item.name;
        that.totalPieces = Math.ceil(size / that.bytesPerPiece) || 1; //slice count
        const filekey = Md5.hashStr(item.name + item.size + that.totalPieces).toString();
        if (start >= size) {
            return;
        }
        let end = start + that.bytesPerPiece;
        if (end > size) {
            end = size;
        }
        var chunk = item.slice(start, end); // file
        var sliceIndex = index; // file index
        var formData = new FormData();
        formData.append('file', chunk);
        let jwt = localStorage.getItem('jwt');
        const url = this.baseUrl +
            `FileName=${fileName}&SliceIndex=${sliceIndex.toString()}&TotalPieces=${that.totalPieces.toString()}&Key=${filekey}`;
        this.httpClient
            .post(url, formData, {
            responseType: 'json',
            headers: new HttpHeaders({
                Authorization: 'Bearer ' + jwt,
            }),
        })
            .subscribe(res => {
            // @ts-ignore
            if (res.code === 0 && res.data === 'good') {
                // @ts-ignore
                let sIdx = (++sliceIndex).toString(); // create new index
                if (Number(sIdx) == that.totalPieces) {
                    sIdx = '-1';
                }
                if (Number.parseInt(sIdx) != -1) {
                    that.fileProgress = Math.floor((index / that.totalPieces) * 100); // progress
                    index = Number.parseInt(sIdx);
                    start = index * that.bytesPerPiece;
                    progressFn && progressFn(that.fileProgress);
                    that.loopSend(item, start, index, successFn, failFn, progressFn);
                }
                else {
                    // complete
                    // that.fileProgress = 100;
                    // this.progressShow = false;
                    successFn({
                        fileName: fileName,
                        key: filekey,
                        moduleName: 'courseWare',
                        sliceIndex: sliceIndex,
                        totalPieces: that.totalPieces,
                    });
                }
            }
            else {
                failFn({});
                // this.fileProgress = 0;
                // this.progressShow = false;
                // this.isCancel = true;
            }
        });
    }
    cancelUpload() {
        this.isCancel = true;
        this.fileProgress = 0;
        this.progressShow = false;
    }
}
BigFileUploadService.ɵfac = function BigFileUploadService_Factory(t) { return new (t || BigFileUploadService)(i0.ɵɵinject(i1.HttpClient)); };
BigFileUploadService.ɵprov = /*@__PURE__*/ i0.ɵɵdefineInjectable({ token: BigFileUploadService, factory: BigFileUploadService.ɵfac });
(function () { (typeof ngDevMode === "undefined" || ngDevMode) && i0.ɵsetClassMetadata(BigFileUploadService, [{
        type: Injectable
    }], function () { return [{ type: i1.HttpClient }]; }, null); })();
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiYmlnLWZpbGUtdXBsb2FkLnNlcnZpY2UuanMiLCJzb3VyY2VSb290IjoiIiwic291cmNlcyI6WyIuLi8uLi8uLi8uLi8uLi8uLi9zcmMvYXBwL3NlcnZpY2UvYmlnLWZpbGUtdXBsb2FkL2JpZy1maWxlLXVwbG9hZC5zZXJ2aWNlLnRzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiJBQUFBLE9BQU8sRUFBRSxVQUFVLEVBQUUsTUFBTSxlQUFlLENBQUM7QUFDM0MsT0FBTyxFQUFFLFdBQVcsRUFBYyxNQUFNLHNCQUFzQixDQUFDO0FBQy9ELE9BQU8sRUFBRSxHQUFHLEVBQUUsTUFBTSxpQkFBaUIsQ0FBQzs7O0FBR3RDLE1BQU0sT0FBTyxvQkFBb0I7SUFTL0IsWUFBb0IsVUFBc0I7UUFBdEIsZUFBVSxHQUFWLFVBQVUsQ0FBWTtRQVAxQyxpQkFBWSxHQUFZLEtBQUssQ0FBQztRQUM5QixhQUFRLEdBQVksS0FBSyxDQUFDO1FBQzFCLGlCQUFZLEdBQVcsQ0FBQyxDQUFDO1FBQ3pCLGdCQUFXLEdBQVcsQ0FBQyxDQUFDO1FBQ3hCLHFCQUFnQixHQUFlLEVBQUUsQ0FBQztRQUNsQyxrQkFBYSxHQUFRLElBQUksQ0FBQztRQUMxQixZQUFPLEdBQVcsRUFBRSxDQUFDO0lBQ3dCLENBQUM7SUFFOUMsZUFBZSxDQUFDLENBQU0sRUFBRSxTQUFtQixFQUFFLE1BQWdCLEVBQUUsVUFBcUI7UUFDbEYsSUFBSSxDQUFDLGdCQUFnQixHQUFHLEVBQUUsQ0FBQztRQUMzQixJQUFJLENBQUMsT0FBTyxHQUFHLENBQUMsQ0FBQyxHQUFHLENBQUM7UUFDckIsTUFBTSxJQUFJLEdBQUcsQ0FBQyxDQUFDLElBQUksQ0FBQztRQUNwQixJQUFJLElBQUksRUFBRTtZQUNSLElBQUksQ0FBQyxnQkFBZ0IsR0FBRyxJQUFJLENBQUM7WUFDN0IsYUFBYTtZQUNiLElBQUksSUFBSSxDQUFDLElBQUksR0FBRyxJQUFJLEdBQUcsSUFBSSxHQUFHLEdBQUcsRUFBRTtnQkFDakMsSUFBSSxDQUFDLGFB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QyxRQUFRLEdBQUcsSUFBSSxDQUFDO1FBQ3JCLElBQUksQ0FBQyxZQUFZLEdBQUcsQ0FBQyxDQUFDO1FBQ3RCLElBQUksQ0FBQyxZQUFZLEdBQUcsS0FBSyxDQUFDO0lBQzVCLENBQUM7O3dGQXpIVSxvQkFBb0I7MEVBQXBCLG9CQUFvQixXQUFwQixvQkFBb0I7dUZBQXBCLG9CQUFvQjtjQURoQyxVQUFVIiwic291cmNlc0NvbnRlbnQiOlsiaW1wb3J0IHsgSW5qZWN0YWJsZSB9IGZyb20gJ0Bhbmd1bGFyL2NvcmUnO1xyXG5pbXBvcnQgeyBIdHRwSGVhZGVycywgSHR0cENsaWVudCB9IGZyb20gJ0Bhbmd1bGFyL2NvbW1vbi9odHRwJztcclxuaW1wb3J0IHsgTWQ1IH0gZnJvbSAndHMtbWQ1L2Rpc3QvY2pzJztcclxuXHJcbkBJbmplY3RhYmxlKClcclxuZXhwb3J0IGNsYXNzIEJpZ0ZpbGVVcGxvYWRTZXJ2aWNlIHtcclxuICBsb2FkaW5nITogYm9vbGVhbjtcclxuICBwcm9ncmVzc1Nob3c6IGJvb2xlYW4gPSBmYWxzZTtcclxuICBpc0NhbmNlbDogYm9vbGVhbiA9IGZhbHNlO1xyXG4gIGZpbGVQcm9ncmVzczogbnVtYmVyID0gMDtcclxuICB0b3RhbFBpZWNlczogbnVtYmVyID0gMDtcclxuICBmaXJtd2FyZUZpbGVMaXN0OiBBcnJheTxhbnk+ID0gW107XHJcbiAgYnl0ZXNQZXJQaWVjZTogYW55ID0gbnVsbDtcclxuICBiYXNlVXJsOiBzdHJpbmcgPSAnJztcclxuICBjb25zdHJ1Y3Rvcihwcml2YXRlIGh0dHBDbGllbnQ6IEh0dHBDbGllbnQpIHt9XHJcblxyXG4gIGlucHV0RmlsZUNoYW5nZShlOiBhbnksIHN1Y2Nlc3NGbjogRnVuY3Rpb24sIGZhaWxGbjogRnVuY3Rpb24sIHByb2dyZXNzRm4/OiBGdW5jdGlvbik6IHZvaWQge1xyXG4gICAgdGhpcy5maXJtd2FyZUZpbGVMaXN0ID0gW107XHJcbiAgICB0aGlzLmJhc2VVcmwgPSBlLnVybDtcclxuICAgIGNvbnN0IGZpbGUgPSBlLmZpbGU7XHJcbiAgICBpZiAoZmlsZSkge1xyXG4gICAgICB0aGlzLmZpcm13YXJlRmlsZUxpc3QgPSBmaWxlO1xyXG4gICAgICAvLyBzbGljZSBmaWxlXHJcbiAgICAgIGlmIChmaWxlLnNpemUgPiAxMDI0ICogMTAyNCAqIDEwMCkge1xyXG4gICAgICAgIHRoaXMuYnl0ZXNQZXJQaWVjZSA9IE1hdGguZmxvb3IoZmlsZS5zaXplIC8gMTAwKTtcclxuICAgICAgfSBlbHNlIHtcclxuICAgICAgICB0aGlzLmJ5dGVzUGVyUGllY2UgPSAxMDI0ICogMTAyNCAqIDE7XHJcbiAgICAgIH1cclxuXHJcbiAgICAgIHRoaXMucHJvZ3Jlc3NTaG93ID0gdHJ1ZTtcclxuICAgICAgdGhpcy5maWxlUHJvZ3Jlc3MgPSAwO1xyXG4gICAgICBsZXQgc3RhcnQgPSAwO1xyXG4gICAgICBsZXQgaW5kZXggPSAwO1xyXG4gICAgICBzZXRUaW1lb3V0KCgpID0+IHtcclxuICAgICAgICB0aGlzLmxvb3BTZW5kKHRoaXMuZmlybXdhcmVGaWxlTGlzdCwgc3RhcnQsIGluZGV4LCBzdWNjZXNzRm4sIGZhaWxGbiwgcHJvZ3Jlc3NGbik7XHJcbiAgICAgIH0sIDEwMCk7XHJcbiAgICAgIC8vIGUudGFyZ2V0LnZhbHVlID0gJyc7XHJcbiAgICB9XHJcbiAgfVxyXG5cclxuICAvLyB1cGxvYWQgZmlsZVxyXG4gIGxvb3BTZW5kKFxyXG4gICAgaXRlbTogYW55LFxyXG4gICAgc3RhcnQ6IG51bWJlcixcclxuICAgIGluZGV4OiBudW1iZXIsXHJcbiAgICBzdWNjZXNzRm46IEZ1bmN0aW9uLFxyXG4gICAgZmFpbEZuOiBGdW5jdGlvbixcclxuICAgIHByb2dyZXNzRm4/OiBGdW5jdGlvblxyXG4gICkge1xyXG4gICAgaWYgKHRoaXMuaXNDYW5jZWwpIHtcclxuICAgICAgcmV0dXJuO1xyXG4gICAgfVxyXG5cclxuICAgIGxldCB0aGF0ID0gdGhpcztcclxuICAgIGNvbnN0IHNpemUgPSBpdGVtLnNpemUgfHwgMTtcclxuICAgIGNvbnN0IGZpbGVOYW1lID0gaXRlbS5uYW1lO1xyXG5cclxuICAgIHRoYXQudG90YWxQaWVjZXMgPSBNYXRoLmNlaWwoc2l6ZSAvIHRoYXQuYnl0ZXNQZXJQaWVjZSkgfHwgMTsgLy9zbGljZSBjb3VudFxyXG4gICAgY29uc3QgZmlsZWtleSA9IE1kNS5oYXNoU3RyKGl0ZW0ubmFtZSArIGl0ZW0uc2l6ZSArIHRoYXQudG90YWxQaWVjZXMpLnRvU3RyaW5nKCk7XHJcblxyXG4gICAgaWYgKHN0YXJ0ID49IHNpemUpIHtcclxuICAgICAgcmV0dXJuO1xyXG4gICAgfVxyXG5cclxuICAgIGxldCBlbmQgPSBzdGFydCArIHRoYXQuYnl0ZXNQZXJQaWVjZTtcclxuICAgIGlmIChlbmQgPiBzaXplKSB7XHJcbiAgICAgIGVuZCA9IHNpemU7XHJcbiAgICB9XHJcbiAgICB2YXIgY2h1bmsgPSBpdGVtLnNsaWNlKHN0YXJ0LCBlbmQpOyAvLyBmaWxlXHJcbiAgICB2YXIgc2xpY2VJbmRleCA9IGluZGV4OyAvLyBmaWxlIGluZGV4XHJcblxyXG4gICAgdmFyIGZvcm1EYXRhID0gbmV3IEZvcm1EYXRhKCk7XHJcbiAgICBmb3JtRGF0YS5hcHBlbmQoJ2ZpbGUnLCBjaHVuayk7XHJcbiAgICBsZXQgand0ID0gbG9jYWxTdG9yYWdlLmdldEl0ZW0oJ2p3dCcpO1xyXG5cclxuICAgIGNvbnN0IHVybCA9XHJcbiAgICAgIHRoaXMuYmFzZVVybCArXHJcbiAgICAgIGBGaWxlTmFtZT0ke2ZpbGVOYW1lfSZTbGljZUluZGV4PSR7c2xpY2VJbmRleC50b1N0cmluZygpfSZUb3RhbFBpZWNlcz0ke3RoYXQudG90YWxQaWVjZXMudG9TdHJpbmcoKX0mS2V5PSR7ZmlsZWtleX1gO1xyXG5cclxuICAgIHRoaXMuaHR0cENsaWVudFxyXG4gICAgICAucG9zdCh1cmwsIGZvcm1EYXRhLCB7XHJcbiAgICAgICAgcmVzcG9uc2VUeXBlOiAnanNvbicsXHJcbiAgICAgICAgaGVhZGVyczogbmV3IEh0dHBIZWFkZXJzKHtcclxuICAgICAgICAgIEF1dGhvcml6YXRpb246ICdCZWFyZXIgJyArIGp3dCxcclxuICAgICAgICB9KSxcclxuICAgICAgfSlcclxuICAgICAgLnN1YnNjcmliZShyZXMgPT4ge1xyXG4gICAgICAgIC8vIEB0cy1pZ25vcmVcclxuICAgICAgICBpZiAocmVzLmNvZGUgPT09IDAgJiYgcmVzLmRhdGEgPT09ICdnb29kJykge1xyXG4gICAgICAgICAgLy8gQHRzLWlnbm9yZVxyXG4gICAgICAgICAgbGV0IHNJZHggPSAoKytzbGljZUluZGV4KS50b1N0cmluZygpOyAvLyBjcmVhdGUgbmV3IGluZGV4XHJcbiAgICAgICAgICBpZiAoTnVtYmVyKHNJZHgpID09IHRoYXQudG90YWxQaWVjZXMpIHtcclxuICAgICAgICAgICAgc0lkeCA9ICctMSc7XHJcbiAgICAgICAgICB9XHJcbiAgICAgICAgICBpZiAoTnVtYmVyLnBhcnNlSW50KHNJZHgpICE9IC0xKSB7XHJcbiAgICAgICAgICAgIHRoYXQuZmlsZVByb2dyZXNzID0gTWF0aC5mbG9vcigoaW5kZXggLyB0aGF0LnRvdGFsUGllY2VzKSAqIDEwMCk7IC8vIHByb2dyZXNzXHJcbiAgICAgICAgICAgIGluZGV4ID0gTnVtYmVyLnBhcnNlSW50KHNJZHgpO1xyXG4gICAgICAgICAgICBzdGFydCA9IGluZGV4ICogdGhhdC5ieXRlc1BlclBpZWNlO1xyXG5cclxuICAgICAgICAgICAgcHJvZ3Jlc3NGbiAmJiBwcm9ncmVzc0ZuKHRoYXQuZmlsZVByb2dyZXNzKTtcclxuICAgICAgICAgICAgdGhhdC5sb29wU2VuZChpdGVtLCBzdGFydCwgaW5kZXgsIHN1Y2Nlc3NGbiwgZmFpbEZuLCBwcm9ncmVzc0ZuKTtcclxuICAgICAgICAgIH0gZWxzZSB7XHJcbiAgICAgICAgICAgIC8vIGNvbXBsZXRlXHJcbiAgICAgICAgICAgIC8vIHRoYXQuZmlsZVByb2dyZXNzID0gMTAwO1xyXG4gICAgICAgICAgICAvLyB0aGlzLnByb2dyZXNzU2hvdyA9IGZhbHNlO1xyXG4gICAgICAgICAgICBzdWNjZXNzRm4oe1xyXG4gICAgICAgICAgICAgIGZpbGVOYW1lOiBmaWxlTmFtZSxcclxuICAgICAgICAgICAgICBrZXk6IGZpbGVrZXksXHJcbiAgICAgICAgICAgICAgbW9kdWxlTmFtZTogJ2NvdXJzZVdhcmUnLFxyXG4gICAgICAgICAgICAgIHNsaWNlSW5kZXg6IHNsaWNlSW5kZXgsXHJcbiAgICAgICAgICAgICAgdG90YWxQaWVjZXM6IHRoYXQudG90YWxQaWVjZXMsXHJcbiAgICAgICAgICAgIH0pO1xyXG4gICAgICAgICAgfVxyXG4gICAgICAgIH0gZWxzZSB7XHJcbiAgICAgICAgICBmYWlsRm4oe30pO1xyXG4gICAgICAgICAgLy8gdGhpcy5maWxlUHJvZ3Jlc3MgPSAwO1xyXG4gICAgICAgICAgLy8gdGhpcy5wcm9ncmVzc1Nob3cgPSBmYWxzZTtcclxuICAgICAgICAgIC8vIHRoaXMuaXNDYW5jZWwgPSB0cnVlO1xyXG4gICAgICAgIH1cclxuICAgICAgfSk7XHJcbiAgfVxyXG5cclxuICBjYW5jZWxVcGxvYWQoKSB7XHJcbiAgICB0aGlzLmlzQ2FuY2VsID0gdHJ1ZTtcclxuICAgIHRoaXMuZmlsZVByb2dyZXNzID0gMDtcclxuICAgIHRoaXMucHJvZ3Jlc3NTaG93ID0gZmFsc2U7XHJcbiAgfVxyXG59XHJcbiJdfQ==