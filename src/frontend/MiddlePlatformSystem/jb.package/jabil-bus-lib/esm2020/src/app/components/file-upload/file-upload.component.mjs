import { Component, ViewChild } from '@angular/core';
import { HttpHeaders, } from '@angular/common/http';
import * as i0 from "@angular/core";
import * as i1 from "@angular/common/http";
import * as i2 from "ng-zorro-antd/icon";
import * as i3 from "ng-zorro-antd/button";
import * as i4 from "ng-zorro-antd/core/transition-patch";
import * as i5 from "ng-zorro-antd/core/wave";
import * as i6 from "ng-zorro-antd/progress";
const _c0 = ["fileImport"];
export class FileUploadComponent {
    constructor(httpClient) {
        this.httpClient = httpClient;
        this.progressShow = false;
        this.isCancel = false;
        this.fileProgress = 0;
        this.totalPieces = 0;
        this.firmwareFileList = [];
        this.examList = [];
        this.fileLast = null;
        this.fileObj = null;
        this.bytesPerPiece = null;
        this.uploadUrl = '/gateway/mfgtranning/api/mfg/Courseware/UploadCourseware';
    }
    async ngOnInit() { }
    /**/
    clickUpload() {
        // @ts-ignore
        this.fileImport?.nativeElement?.click();
        this.isCancel = false;
    }
    /**/
    inputFileChange(e) {
        this.firmwareFileList = [];
        this.fileLast = null;
        this.fileObj = null;
        this.examList = [];
        const files = e.target.files;
        if (files && files[0]) {
            const file = files[0];
            this.firmwareFileList = file;
            //
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
                this.loopSend(this.firmwareFileList, start, index); // 调用分片上传函数
            }, 100);
            e.target.value = '';
        }
    }
    loopSend(item, start, index) {
        if (this.isCancel) {
            // 取消上传
            return;
        }
        let that = this;
        const size = item.size;
        const finame = item.name;
        that.totalPieces = Math.ceil(size / that.bytesPerPiece); //计算文件切片总数
        const filekey = 'yxsource' + item.name + item.size + that.totalPieces;
        // const filekey1 = Md5.hashStr(filekey).toString();
        const filekey1 = filekey + Date.now();
        if (start >= size) {
            return;
        }
        let end = start + that.bytesPerPiece;
        if (end > size) {
            end = size;
        }
        var chunk = item.slice(start, end);
        var sliceIndex = index;
        var formData = new FormData();
        formData.append('file', chunk);
        formData.append('moduleName', 'bigFileUpload');
        formData.append('fileName', finame);
        formData.append('sliceIndex', sliceIndex.toString());
        formData.append('totalPieces', that.totalPieces.toString());
        formData.append('key', filekey1);
        let jwt = localStorage.getItem('jwt');
        this.httpClient
            .post(this.uploadUrl, formData, {
            responseType: 'json',
            headers: new HttpHeaders({
                Authorization: 'Bearer ' + jwt,
            }),
        })
            .subscribe(res => {
            // @ts-ignore
            // if (res.status === StatusEnum.SUCCESS) {
            if (res.status === 0) {
                // @ts-ignore
                let sIdx = res.datas.sliceIndex;
                if (Number.parseInt(sIdx) != -1) {
                    that.fileProgress = Math.floor((index / that.totalPieces) * 100); // 上传进度条进度
                    index = Number.parseInt(sIdx);
                    start = index * that.bytesPerPiece;
                    that.loopSend(item, start, index);
                }
                else {
                    that.fileProgress = 100;
                    this.progressShow = false;
                }
            }
            else {
                this.fileProgress = 0;
                this.progressShow = false;
                this.isCancel = true;
            }
        });
    }
    cancelUpload() {
        this.isCancel = true;
        this.fileProgress = 0;
        this.progressShow = false;
    }
}
FileUploadComponent.ɵfac = function FileUploadComponent_Factory(t) { return new (t || FileUploadComponent)(i0.ɵɵdirectiveInject(i1.HttpClient)); };
FileUploadComponent.ɵcmp = /*@__PURE__*/ i0.ɵɵdefineComponent({ type: FileUploadComponent, selectors: [["jabil-file-upload"]], viewQuery: function FileUploadComponent_Query(rf, ctx) { if (rf & 1) {
        i0.ɵɵviewQuery(_c0, 5);
    } if (rf & 2) {
        let _t;
        i0.ɵɵqueryRefresh(_t = i0.ɵɵloadQuery()) && (ctx.fileImport = _t.first);
    } }, decls: 12, vars: 4, consts: [[2, "display", "flex", "align-items", "center", "justify-content", "flex-start"], [2, "width", "30%"], ["nz-button", "", "nzType", "primary", 3, "disabled", "click"], ["nz-icon", "", "nzType", "plus", "nzTheme", "outline"], [1, "operaTao", 2, "margin-left", "15px", 3, "hidden", "click"], ["accept", "application/msword,application/vnd.openxmlformats-officedocument.wordprocessingml.document,\n              audio/mp4,video/mp4,application/pdf,video/x-msvideo", "type", "file", 2, "display", "none", 3, "change"], ["fileImport", ""], [2, "width", "40%", "position", "relative", "left", "-80px", 3, "hidden"], ["nzStatus", "active", 3, "nzPercent"]], template: function FileUploadComponent_Template(rf, ctx) { if (rf & 1) {
        i0.ɵɵelementStart(0, "div")(1, "div", 0)(2, "div", 1)(3, "button", 2);
        i0.ɵɵlistener("click", function FileUploadComponent_Template_button_click_3_listener() { return ctx.clickUpload(); });
        i0.ɵɵelement(4, "i", 3);
        i0.ɵɵtext(5, "\u4E0A\u4F20\u8BFE\u4EF6 ");
        i0.ɵɵelementEnd();
        i0.ɵɵelementStart(6, "span", 4);
        i0.ɵɵlistener("click", function FileUploadComponent_Template_span_click_6_listener() { return ctx.cancelUpload(); });
        i0.ɵɵtext(7, "\u53D6\u6D88");
        i0.ɵɵelementEnd();
        i0.ɵɵelementStart(8, "input", 5, 6);
        i0.ɵɵlistener("change", function FileUploadComponent_Template_input_change_8_listener($event) { return ctx.inputFileChange($event); });
        i0.ɵɵelementEnd()();
        i0.ɵɵelementStart(10, "div", 7);
        i0.ɵɵelement(11, "nz-progress", 8);
        i0.ɵɵelementEnd()()();
    } if (rf & 2) {
        i0.ɵɵadvance(3);
        i0.ɵɵproperty("disabled", ctx.progressShow);
        i0.ɵɵadvance(3);
        i0.ɵɵproperty("hidden", !ctx.progressShow);
        i0.ɵɵadvance(4);
        i0.ɵɵproperty("hidden", !ctx.progressShow);
        i0.ɵɵadvance(1);
        i0.ɵɵproperty("nzPercent", ctx.fileProgress);
    } }, dependencies: [i2.NzIconDirective, i3.NzButtonComponent, i4.ɵNzTransitionPatchDirective, i5.NzWaveDirective, i6.NzProgressComponent], styles: ["[_nghost-%COMP%]{width:100%;height:100%}"] });
(function () { (typeof ngDevMode === "undefined" || ngDevMode) && i0.ɵsetClassMetadata(FileUploadComponent, [{
        type: Component,
        args: [{ selector: 'jabil-file-upload', template: "<div>\r\n  <div style=\"display: flex;align-items: center;justify-content: flex-start\">\r\n    <div style=\"width: 30%\">\r\n      <button nz-button nzType=\"primary\" (click)=\"clickUpload()\" [disabled]=\"progressShow\">\r\n        <i nz-icon nzType=\"plus\" nzTheme=\"outline\"></i>\u4E0A\u4F20\u8BFE\u4EF6\r\n      </button>\r\n      <span class=\"operaTao\" style=\"margin-left: 15px\" (click)=\"cancelUpload()\" [hidden]=\"!progressShow\">\u53D6\u6D88</span>\r\n      <input #fileImport\r\n             accept=\"application/msword,application/vnd.openxmlformats-officedocument.wordprocessingml.document,\r\n              audio/mp4,video/mp4,application/pdf,video/x-msvideo\" (change)=\"inputFileChange($event)\"\r\n             type=\"file\" style=\"display: none\">\r\n    </div>\r\n    <div style=\"width: 40%;position: relative;left: -80px;\" [hidden]=\"!progressShow\">\r\n      <nz-progress [nzPercent]=\"fileProgress\" nzStatus=\"active\"></nz-progress>\r\n    </div>\r\n  </div>\r\n</div>\r\n", styles: [":host{width:100%;height:100%}\n"] }]
    }], function () { return [{ type: i1.HttpClient }]; }, { fileImport: [{
            type: ViewChild,
            args: ['fileImport']
        }] }); })();
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiZmlsZS11cGxvYWQuY29tcG9uZW50LmpzIiwic291cmNlUm9vdCI6IiIsInNvdXJjZXMiOlsiLi4vLi4vLi4vLi4vLi4vLi4vc3JjL2FwcC9jb21wb25lbnRzL2ZpbGUtdXBsb2FkL2ZpbGUtdXBsb2FkLmNvbXBvbmVudC50cyIsIi4uLy4uLy4uLy4uLy4uLy4uL3NyYy9hcHAvY29tcG9uZW50cy9maWxlLXVwbG9hZC9maWxlLXVwbG9hZC5jb21wb25lbnQuaHRtbCJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFBQSxPQUFPLEVBQUUsU0FBUyxFQUFzQixTQUFTLEVBQUUsTUFBTSxlQUFlLENBQUM7QUFDekUsT0FBTyxFQUVMLFdBQVcsR0FDWixNQUFNLHNCQUFzQixDQUFDOzs7Ozs7Ozs7QUFPOUIsTUFBTSxPQUFPLG1CQUFtQjtJQWU5QixZQUFvQixVQUFzQjtRQUF0QixlQUFVLEdBQVYsVUFBVSxDQUFZO1FBWDFDLGlCQUFZLEdBQVksS0FBSyxDQUFDO1FBQzlCLGFBQVEsR0FBWSxLQUFLLENBQUM7UUFDMUIsaUJBQVksR0FBVyxDQUFDLENBQUM7UUFDekIsZ0JBQVcsR0FBVyxDQUFDLENBQUM7UUFDeEIscUJBQWdCLEdBQWUsRUFBRSxDQUFDO1FBQ2xDLGFBQVEsR0FBZSxFQUFFLENBQUM7UUFDMUIsYUFBUSxHQUFRLElBQUksQ0FBQztRQUNyQixZQUFPLEdBQVEsSUFBSSxDQUFDO1FBQ3BCLGtCQUFhLEdBQVEsSUFBSSxDQUFDO1FBQzFCLGNBQVMsR0FBVywwREFBMEQsQ0FBQztJQUVsQyxDQUFDO0lBRXZDLEtBQUssQ0FBQyxRQUFRLEtBQUksQ0FBQztJQUUxQixJQUFJO0lBQ0osV0FBVztRQUNULGFBQWE7UUFDYixJQUFJLENBQUMsVUFBVSxFQUFFLGFBQWEsRUFBRSxLQUFLLEVBQUUsQ0FBQztRQUN4QyxJQUFJLENBQUMsUUFBUSxHQUFHLEtBQUssQ0FBQztJQUN4QixDQUFDO0lBRUQsSUFBSTtJQUNKLGVBQWUsQ0FBQyxDQUFNO1FBQ3BCLElBQUks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cm9tICdAYW5ndWxhci9jb3JlJztcclxuaW1wb3J0IHtcclxuICBIdHRwQ2xpZW50LFxyXG4gIEh0dHBIZWFkZXJzLFxyXG59IGZyb20gJ0Bhbmd1bGFyL2NvbW1vbi9odHRwJztcclxuXHJcbkBDb21wb25lbnQoe1xyXG4gIHNlbGVjdG9yOiAnamFiaWwtZmlsZS11cGxvYWQnLFxyXG4gIHRlbXBsYXRlVXJsOiAnLi9maWxlLXVwbG9hZC5jb21wb25lbnQuaHRtbCcsXHJcbiAgc3R5bGVVcmxzOiBbJy4vZmlsZS11cGxvYWQuY29tcG9uZW50LnNjc3MnXSxcclxufSlcclxuZXhwb3J0IGNsYXNzIEZpbGVVcGxvYWRDb21wb25lbnQgaW1wbGVtZW50cyBPbkluaXQge1xyXG4gIEBWaWV3Q2hpbGQoJ2ZpbGVJbXBvcnQnKSBmaWxlSW1wb3J0OiBFbGVtZW50UmVmIHwgdW5kZWZpbmVkO1xyXG5cclxuICBsb2FkaW5nITogYm9vbGVhbjtcclxuICBwcm9ncmVzc1Nob3c6IGJvb2xlYW4gPSBmYWxzZTtcclxuICBpc0NhbmNlbDogYm9vbGVhbiA9IGZhbHNlO1xyXG4gIGZpbGVQcm9ncmVzczogbnVtYmVyID0gMDtcclxuICB0b3RhbFBpZWNlczogbnVtYmVyID0gMDtcclxuICBmaXJtd2FyZUZpbGVMaXN0OiBBcnJheTxhbnk+ID0gW107XHJcbiAgZXhhbUxpc3Q6IEFycmF5PGFueT4gPSBbXTtcclxuICBmaWxlTGFzdDogYW55ID0gbnVsbDtcclxuICBmaWxlT2JqOiBhbnkgPSBudWxsO1xyXG4gIGJ5dGVzUGVyUGllY2U6IGFueSA9IG51bGw7XHJcbiAgdXBsb2FkVXJsOiBzdHJpbmcgPSAnL2dhdGV3YXkvbWZndHJhbm5pbmcvYXBpL21mZy9Db3Vyc2V3YXJlL1VwbG9hZENvdXJzZXdhcmUnO1xyXG5cclxuICBjb25zdHJ1Y3Rvcihwcml2YXRlIGh0dHBDbGllbnQ6IEh0dHBDbGllbnQpIHt9XHJcblxyXG4gIHB1YmxpYyBhc3luYyBuZ09uSW5pdCgpIHt9XHJcblxyXG4gIC8qKi9cclxuICBjbGlja1VwbG9hZCgpIHtcclxuICAgIC8vIEB0cy1pZ25vcmVcclxuICAgIHRoaXMuZmlsZUltcG9ydD8ubmF0aXZlRWxlbWVudD8uY2xpY2soKTtcclxuICAgIHRoaXMuaXNDYW5jZWwgPSBmYWxzZTtcclxuICB9XHJcblxyXG4gIC8qKi9cclxuICBpbnB1dEZpbGVDaGFuZ2UoZTogYW55KTogdm9pZCB7XHJcbiAgICB0aGlzLmZpcm13YXJlRmlsZUxpc3QgPSBbXTtcclxuICAgIHRoaXMuZmlsZUxhc3QgPSBudWxsO1xyXG4gICAgdGhpcy5maWxlT2JqID0gbnVsbDtcclxuICAgIHRoaXMuZXhhbUxpc3QgPSBbXTtcclxuICAgIGNvbnN0IGZpbGVzID0gZS50YXJnZXQuZmlsZXM7XHJcbiAgICBpZiAoZmlsZXMgJiYgZmlsZXNbMF0pIHtcclxuICAgICAgY29uc3QgZmlsZSA9IGZpbGVzWzBdO1xyXG4gICAgICB0aGlzLmZpcm13YXJlRmlsZUxpc3QgPSBmaWxlO1xyXG4gICAgICAvL1xyXG4gICAgICBpZiAoZmlsZS5zaXplID4gMTAyNCAqIDEwMjQgKiAxMDApIHtcclxuICAgICAgICB0aGlzLmJ5dGVzUGVyUGllY2UgPSBNYXRoLmZsb29yKGZpbGUuc2l6ZSAvIDEwMCk7XHJcbiAgICAgIH0gZWxzZSB7XHJcbiAgICAgICAgdGhpcy5ieXRlc1BlclBpZWNlID0gMTAyNCAqIDEwMjQgKiAxO1xyXG4gICAgICB9XHJcblxyXG4gICAgICB0aGlzLnByb2dyZXNzU2hvdyA9IHRydWU7XHJcbiAgICAgIHRoaXMuZmlsZVByb2dyZXNzID0gMDtcclxuICAgICAgbGV0IHN0YXJ0ID0gMDtcclxuICAgICAgbGV0IGluZGV4ID0gMDtcclxuICAgICAgc2V0VGltZW91dCgoKSA9PiB7XHJcbiAgICAgICAgdGhpcy5sb29wU2VuZCh0aGlzLmZpcm13YXJlRmlsZUxpc3QsIHN0YXJ0LCBpbmRleCk7IC8vIOiwg+eUqOWIhueJh+S4iuS8oOWHveaVsFxyXG4gICAgICB9LCAxMDApO1xyXG4gICAgICBlLnRhcmdldC52YWx1ZSA9ICcnO1xyXG4gICAgfVxyXG4gIH1cclxuXHJcbiAgbG9vcFNlbmQoaXRlbTogYW55LCBzdGFydDogbnVtYmVyLCBpbmRleDogbnVtYmVyKSB7XHJcbiAgICBpZiAodGhpcy5pc0NhbmNlbCkge1xyXG4gICAgICAvLyDlj5bmtojkuIrkvKBcclxuICAgICAgcmV0dXJuO1xyXG4gICAgfVxyXG5cclxuICAgIGxldCB0aGF0ID0gdGhpcztcclxuICAgIGNvbnN0IHNpemUgPSBpdGVtLnNpemU7XHJcbiAgICBjb25zdCBmaW5hbWUgPSBpdGVtLm5hbWU7XHJcblxyXG4gICAgdGhhdC50b3RhbFBpZWNlcyA9IE1hdGguY2VpbChzaXplIC8gdGhhdC5ieXRlc1BlclBpZWNlKTsgLy/orqHnrpfmlofku7bliIfniYfmgLvmlbBcclxuICAgIGNvbnN0IGZpbGVrZXkgPSAneXhzb3VyY2UnICsgaXRlbS5uYW1lICsgaXRlbS5zaXplICsgdGhhdC50b3RhbFBpZWNlcztcclxuICAgIC8vIGNvbnN0IGZpbGVrZXkxID0gTWQ1Lmhhc2hTdHIoZmlsZWtleSkudG9TdHJpbmcoKTtcclxuICAgIGNvbnN0IGZpbGVrZXkxID0gZmlsZWtleSArIERhdGUubm93KCk7XHJcblxyXG4gICAgaWYgKHN0YXJ0ID49IHNpemUpIHtcclxuICAgICAgcmV0dXJuO1xyXG4gICAgfVxyXG4gICAgbGV0IGVuZCA9IHN0YXJ0ICsgdGhhdC5ieXRlc1BlclBpZWNlO1xyXG4gICAgaWYgKGVuZCA+IHNpemUpIHtcclxuICAgICAgZW5kID0gc2l6ZTtcclxuICAgIH1cclxuICAgIHZhciBjaHVuayA9IGl0ZW0uc2xpY2Uoc3RhcnQsIGVuZCk7XHJcbiAgICB2YXIgc2xpY2VJbmRleCA9IGluZGV4O1xyXG5cclxuICAgIHZhciBmb3JtRGF0YSA9IG5ldyBGb3JtRGF0YSgpO1xyXG4gICAgZm9ybURhdGEuYXBwZW5kKCdmaWxlJywgY2h1bmspO1xyXG4gICAgZm9ybURhdGEuYXBwZW5kKCdtb2R1bGVOYW1lJywgJ2JpZ0ZpbGVVcGxvYWQnKTtcclxuICAgIGZvcm1EYXRhLmFwcGVuZCgnZmlsZU5hbWUnLCBmaW5hbWUpO1xyXG4gICAgZm9ybURhdGEuYXBwZW5kKCdzbGljZUluZGV4Jywgc2xpY2VJbmRleC50b1N0cmluZygpKTtcclxuICAgIGZvcm1EYXRhLmFwcGVuZCgndG90YWxQaWVjZXMnLCB0aGF0LnRvdGFsUGllY2VzLnRvU3RyaW5nKCkpO1xyXG4gICAgZm9ybURhdGEuYXBwZW5kKCdrZXknLCBmaWxla2V5MSk7XHJcbiAgICBsZXQgand0ID0gbG9jYWxTdG9yYWdlLmdldEl0ZW0oJ2p3dCcpO1xyXG5cclxuICAgIHRoaXMuaHR0cENsaWVudFxyXG4gICAgICAucG9zdCh0aGlzLnVwbG9hZFVybCwgZm9ybURhdGEsIHtcclxuICAgICAgICByZXNwb25zZVR5cGU6ICdqc29uJyxcclxuICAgICAgICBoZWFkZXJzOiBuZXcgSHR0cEhlYWRlcnMoe1xyXG4gICAgICAgICAgQXV0aG9yaXphdGlvbjogJ0JlYXJlciAnICsgand0LFxyXG4gICAgICAgIH0pLFxyXG4gICAgICB9KVxyXG4gICAgICAuc3Vic2NyaWJlKHJlcyA9PiB7XHJcbiAgICAgICAgLy8gQHRzLWlnbm9yZVxyXG4gICAgICAgIC8vIGlmIChyZXMuc3RhdHVzID09PSBTdGF0dXNFbnVtLlNVQ0NFU1MpIHtcclxuICAgICAgICBpZiAocmVzLnN0YXR1cyA9PT0gMCkge1xyXG4gICAgICAgICAgLy8gQHRzLWlnbm9yZVxyXG4gICAgICAgICAgbGV0IHNJZHggPSByZXMuZGF0YXMuc2xpY2VJbmRleDtcclxuICAgICAgICAgIGlmIChOdW1iZXIucGFyc2VJbnQoc0lkeCkgIT0gLTEpIHtcclxuICAgICAgICAgICAgdGhhdC5maWxlUHJvZ3Jlc3MgPSBNYXRoLmZsb29yKChpbmRleCAvIHRoYXQudG90YWxQaWVjZXMpICogMTAwKTsgLy8g5LiK5Lyg6L+b5bqm5p2h6L+b5bqmXHJcbiAgICAgICAgICAgIGluZGV4ID0gTnVtYmVyLnBhcnNlSW50KHNJZHgpO1xyXG4gICAgICAgICAgICBzdGFydCA9IGluZGV4ICogdGhhdC5ieXRlc1BlclBpZWNlO1xyXG5cclxuICAgICAgICAgICAgdGhhdC5sb29wU2VuZChpdGVtLCBzdGFydCwgaW5kZXgpO1xyXG4gICAgICAgICAgfSBlbHNlIHtcclxuICAgICAgICAgICAgdGhhdC5maWxlUHJvZ3Jlc3MgPSAxMDA7XHJcbiAgICAgICAgICAgIHRoaXMucHJvZ3Jlc3NTaG93ID0gZmFsc2U7XHJcbiAgICAgICAgICB9XHJcbiAgICAgICAgfSBlbHNlIHtcclxuICAgICAgICAgIHRoaXMuZmlsZVByb2dyZXNzID0gMDtcclxuICAgICAgICAgIHRoaXMucHJvZ3Jlc3NTaG93ID0gZmFsc2U7XHJcbiAgICAgICAgICB0aGlzLmlzQ2FuY2VsID0gdHJ1ZTtcclxuICAgICAgICB9XHJcbiAgICAgIH0pO1xyXG4gIH1cclxuXHJcbiAgY2FuY2VsVXBsb2FkKCkge1xyXG4gICAgdGhpcy5pc0NhbmNlbCA9IHRydWU7XHJcbiAgICB0aGlzLmZpbGVQcm9ncmVzcyA9IDA7XHJcbiAgICB0aGlzLnByb2dyZXNzU2hvdyA9IGZhbHNlO1xyXG4gIH1cclxufVxyXG4iLCI8ZGl2PlxyXG4gIDxkaXYgc3R5bGU9XCJkaXNwbGF5OiBmbGV4O2FsaWduLWl0ZW1zOiBjZW50ZXI7anVzdGlmeS1jb250ZW50OiBmbGV4LXN0YXJ0XCI+XHJcbiAgICA8ZGl2IHN0eWxlPVwid2lkdGg6IDMwJVwiPlxyXG4gICAgICA8YnV0dG9uIG56LWJ1dHRvbiBuelR5cGU9XCJwcmltYXJ5XCIgKGNsaWNrKT1cImNsaWNrVXBsb2FkKClcIiBbZGlzYWJsZWRdPVwicHJvZ3Jlc3NTaG93XCI+XHJcbiAgICAgICAgPGkgbnotaWNvbiBuelR5cGU9XCJwbHVzXCIgbnpUaGVtZT1cIm91dGxpbmVcIj48L2k+5LiK5Lyg6K++5Lu2XHJcbiAgICAgIDwvYnV0dG9uPlxyXG4gICAgICA8c3BhbiBjbGFzcz1cIm9wZXJhVGFvXCIgc3R5bGU9XCJtYXJnaW4tbGVmdDogMTVweFwiIChjbGljayk9XCJjYW5jZWxVcGxvYWQoKVwiIFtoaWRkZW5dPVwiIXByb2dyZXNzU2hvd1wiPuWPlua2iDwvc3Bhbj5cclxuICAgICAgPGlucHV0ICNmaWxlSW1wb3J0XHJcbiAgICAgICAgICAgICBhY2NlcHQ9XCJhcHBsaWNhdGlvbi9tc3dvcmQsYXBwbGljYXRpb24vdm5kLm9wZW54bWxmb3JtYXRzLW9mZmljZWRvY3VtZW50LndvcmRwcm9jZXNzaW5nbWwuZG9jdW1lbnQsXHJcbiAgICAgICAgICAgICAgYXVkaW8vbXA0LHZpZGVvL21wNCxhcHBsaWNhdGlvbi9wZGYsdmlkZW8veC1tc3ZpZGVvXCIgKGNoYW5nZSk9XCJpbnB1dEZpbGVDaGFuZ2UoJGV2ZW50KVwiXHJcbiAgICAgICAgICAgICB0eXBlPVwiZmlsZVwiIHN0eWxlPVwiZGlzcGxheTogbm9uZVwiPlxyXG4gICAgPC9kaXY+XHJcbiAgICA8ZGl2IHN0eWxlPVwid2lkdGg6IDQwJTtwb3NpdGlvbjogcmVsYXRpdmU7bGVmdDogLTgwcHg7XCIgW2hpZGRlbl09XCIhcHJvZ3Jlc3NTaG93XCI+XHJcbiAgICAgIDxuei1wcm9ncmVzcyBbbnpQZXJjZW50XT1cImZpbGVQcm9ncmVzc1wiIG56U3RhdHVzPVwiYWN0aXZlXCI+PC9uei1wcm9ncmVzcz5cclxuICAgIDwvZGl2PlxyXG4gIDwvZGl2PlxyXG48L2Rpdj5cclxuIl19