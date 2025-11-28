import * as i0 from '@angular/core';
import { Component, ViewChild, NgModule, Injectable, Input, EventEmitter, Output, Inject, Pipe } from '@angular/core';
import * as i5$1 from '@angular/common';
import { CommonModule, registerLocaleData, DOCUMENT } from '@angular/common';
import { __awaiter } from 'tslib';
import * as i1 from '@angular/common/http';
import { HttpHeaders, HttpRequest } from '@angular/common/http';
import * as i2 from 'ng-zorro-antd/icon';
import { NzIconModule } from 'ng-zorro-antd/icon';
import * as i3 from 'ng-zorro-antd/button';
import { NzButtonModule } from 'ng-zorro-antd/button';
import * as i4 from 'ng-zorro-antd/core/transition-patch';
import * as i5 from 'ng-zorro-antd/core/wave';
import * as i6 from 'ng-zorro-antd/progress';
import { NzProgressModule } from 'ng-zorro-antd/progress';
import * as QRCode from 'qrcode';
import * as i1$1 from '@angular/platform-browser';
import * as i2$1 from 'primeng/dialog';
import { DialogModule } from 'primeng/dialog';
import * as i3$1 from 'primeng/api';
import * as i2$2 from '@angular/router';
import { NavigationEnd } from '@angular/router';
import { filter as filter$2, map } from 'rxjs/operators';
import * as i1$2 from '@ngx-translate/core';
import * as i6$1 from 'primeng/panelmenu';
import { PanelMenuModule } from 'primeng/panelmenu';
import * as i7 from 'primeng/tieredmenu';
import { TieredMenuModule } from 'primeng/tieredmenu';
import * as i8 from 'primeng/contextmenu';
import { ContextMenuModule } from 'primeng/contextmenu';
import { ToastModule } from 'primeng/toast';
import { CheckboxModule } from 'primeng/checkbox';
import { ButtonModule } from 'primeng/button';
import { BehaviorSubject, Subject, Observable } from 'rxjs';
import * as i3$2 from 'ngx-translate-cache';
import { Md5 } from 'ts-md5/dist/cjs';
import * as FileSaver from 'file-saver';
import * as XLSX from 'xlsx';

const _c0$3 = ["fileImport"];
class FileUploadComponent {
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
    ngOnInit() {
        return __awaiter(this, void 0, void 0, function* () { });
    }
    /**/
    clickUpload() {
        var _a, _b;
        // @ts-ignore
        (_b = (_a = this.fileImport) === null || _a === void 0 ? void 0 : _a.nativeElement) === null || _b === void 0 ? void 0 : _b.click();
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
FileUploadComponent.ɵcmp = /*@__PURE__*/ i0.ɵɵdefineComponent({ type: FileUploadComponent, selectors: [["jabil-file-upload"]], viewQuery: function FileUploadComponent_Query(rf, ctx) {
        if (rf & 1) {
            i0.ɵɵviewQuery(_c0$3, 5);
        }
        if (rf & 2) {
            let _t;
            i0.ɵɵqueryRefresh(_t = i0.ɵɵloadQuery()) && (ctx.fileImport = _t.first);
        }
    }, decls: 12, vars: 4, consts: [[2, "display", "flex", "align-items", "center", "justify-content", "flex-start"], [2, "width", "30%"], ["nz-button", "", "nzType", "primary", 3, "disabled", "click"], ["nz-icon", "", "nzType", "plus", "nzTheme", "outline"], [1, "operaTao", 2, "margin-left", "15px", 3, "hidden", "click"], ["accept", "application/msword,application/vnd.openxmlformats-officedocument.wordprocessingml.document,\n              audio/mp4,video/mp4,application/pdf,video/x-msvideo", "type", "file", 2, "display", "none", 3, "change"], ["fileImport", ""], [2, "width", "40%", "position", "relative", "left", "-80px", 3, "hidden"], ["nzStatus", "active", 3, "nzPercent"]], template: function FileUploadComponent_Template(rf, ctx) {
        if (rf & 1) {
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
        }
        if (rf & 2) {
            i0.ɵɵadvance(3);
            i0.ɵɵproperty("disabled", ctx.progressShow);
            i0.ɵɵadvance(3);
            i0.ɵɵproperty("hidden", !ctx.progressShow);
            i0.ɵɵadvance(4);
            i0.ɵɵproperty("hidden", !ctx.progressShow);
            i0.ɵɵadvance(1);
            i0.ɵɵproperty("nzPercent", ctx.fileProgress);
        }
    }, dependencies: [i2.NzIconDirective, i3.NzButtonComponent, i4.ɵNzTransitionPatchDirective, i5.NzWaveDirective, i6.NzProgressComponent], styles: ["[_nghost-%COMP%]{width:100%;height:100%}"] });
(function () {
    (typeof ngDevMode === "undefined" || ngDevMode) && i0.ɵsetClassMetadata(FileUploadComponent, [{
            type: Component,
            args: [{ selector: 'jabil-file-upload', template: "<div>\r\n  <div style=\"display: flex;align-items: center;justify-content: flex-start\">\r\n    <div style=\"width: 30%\">\r\n      <button nz-button nzType=\"primary\" (click)=\"clickUpload()\" [disabled]=\"progressShow\">\r\n        <i nz-icon nzType=\"plus\" nzTheme=\"outline\"></i>\u4E0A\u4F20\u8BFE\u4EF6\r\n      </button>\r\n      <span class=\"operaTao\" style=\"margin-left: 15px\" (click)=\"cancelUpload()\" [hidden]=\"!progressShow\">\u53D6\u6D88</span>\r\n      <input #fileImport\r\n             accept=\"application/msword,application/vnd.openxmlformats-officedocument.wordprocessingml.document,\r\n              audio/mp4,video/mp4,application/pdf,video/x-msvideo\" (change)=\"inputFileChange($event)\"\r\n             type=\"file\" style=\"display: none\">\r\n    </div>\r\n    <div style=\"width: 40%;position: relative;left: -80px;\" [hidden]=\"!progressShow\">\r\n      <nz-progress [nzPercent]=\"fileProgress\" nzStatus=\"active\"></nz-progress>\r\n    </div>\r\n  </div>\r\n</div>\r\n", styles: [":host{width:100%;height:100%}\n"] }]
        }], function () { return [{ type: i1.HttpClient }]; }, { fileImport: [{
                type: ViewChild,
                args: ['fileImport']
            }] });
})();

class FileUploadModule {
}
FileUploadModule.ɵfac = function FileUploadModule_Factory(t) { return new (t || FileUploadModule)(); };
FileUploadModule.ɵmod = /*@__PURE__*/ i0.ɵɵdefineNgModule({ type: FileUploadModule });
FileUploadModule.ɵinj = /*@__PURE__*/ i0.ɵɵdefineInjector({ imports: [CommonModule,
        NzIconModule,
        NzButtonModule,
        NzProgressModule] });
(function () {
    (typeof ngDevMode === "undefined" || ngDevMode) && i0.ɵsetClassMetadata(FileUploadModule, [{
            type: NgModule,
            args: [{
                    declarations: [FileUploadComponent],
                    imports: [
                        CommonModule,
                        NzIconModule,
                        NzButtonModule,
                        NzProgressModule
                    ],
                    exports: [
                        FileUploadComponent
                    ]
                }]
        }], null, null);
})();
(function () {
    (typeof ngJitMode === "undefined" || ngJitMode) && i0.ɵɵsetNgModuleScope(FileUploadModule, { declarations: [FileUploadComponent], imports: [CommonModule,
            NzIconModule,
            NzButtonModule,
            NzProgressModule], exports: [FileUploadComponent] });
})();

class LoadingComponent {
    constructor() { }
    ngOnInit() {
        return __awaiter(this, void 0, void 0, function* () {
            // console.log('loading');
        });
    }
}
LoadingComponent.ɵfac = function LoadingComponent_Factory(t) { return new (t || LoadingComponent)(); };
LoadingComponent.ɵcmp = /*@__PURE__*/ i0.ɵɵdefineComponent({ type: LoadingComponent, selectors: [["jabil-loading"]], decls: 2, vars: 0, consts: [[1, "pi", "pi-spin", "pi-spinner", 2, "font-size", "2rem"]], template: function LoadingComponent_Template(rf, ctx) {
        if (rf & 1) {
            i0.ɵɵelementStart(0, "div");
            i0.ɵɵelement(1, "i", 0);
            i0.ɵɵelementEnd();
        }
    }, styles: ["[_nghost-%COMP%]{width:100%;height:100%;position:fixed;top:0;left:0;z-index:9999;color:#fff;font-size:1.4rem;display:flex;flex-direction:column;align-items:center;justify-content:center;background-color:#0006}[_nghost-%COMP%]   i[_ngcontent-%COMP%]{color:#fff;font-size:2.1rem}"] });
(function () {
    (typeof ngDevMode === "undefined" || ngDevMode) && i0.ɵsetClassMetadata(LoadingComponent, [{
            type: Component,
            args: [{ selector: 'jabil-loading', template: "<div>\r\n  <i class=\"pi pi-spin pi-spinner\" style=\"font-size: 2rem\"></i>\r\n</div>\r\n", styles: [":host{width:100%;height:100%;position:fixed;top:0;left:0;z-index:9999;color:#fff;font-size:1.4rem;display:flex;flex-direction:column;align-items:center;justify-content:center;background-color:#0006}:host i{color:#fff;font-size:2.1rem}\n"] }]
        }], function () { return []; }, null);
})();

class LoadingModule {
}
LoadingModule.ɵfac = function LoadingModule_Factory(t) { return new (t || LoadingModule)(); };
LoadingModule.ɵmod = /*@__PURE__*/ i0.ɵɵdefineNgModule({ type: LoadingModule });
LoadingModule.ɵinj = /*@__PURE__*/ i0.ɵɵdefineInjector({ imports: [CommonModule] });
(function () {
    (typeof ngDevMode === "undefined" || ngDevMode) && i0.ɵsetClassMetadata(LoadingModule, [{
            type: NgModule,
            args: [{
                    declarations: [
                        LoadingComponent
                    ],
                    imports: [
                        CommonModule
                    ],
                    exports: [
                        LoadingComponent
                    ]
                }]
        }], null, null);
})();
(function () { (typeof ngJitMode === "undefined" || ngJitMode) && i0.ɵɵsetNgModuleScope(LoadingModule, { declarations: [LoadingComponent], imports: [CommonModule], exports: [LoadingComponent] }); })();

let localLayout = null;
class CommonService {
    constructor() {
        this.dateFormat = function (fmt, date) {
            let ret;
            const opt = {
                'Y+': date.getFullYear().toString(),
                'm+': (date.getMonth() + 1).toString(),
                'd+': date.getDate().toString(),
                'H+': date.getHours().toString(),
                'M+': date.getMinutes().toString(),
                'S+': date.getSeconds().toString(),
            };
            for (let k in opt) {
                ret = new RegExp('(' + k + ')').exec(fmt);
                if (ret) {
                    fmt = fmt.replace(ret[1], ret[1].length == 1 ? opt[k] : opt[k].padStart(ret[1].length, '0'));
                }
            }
            return fmt;
        };
        this.dateFormatNew = function (type, now) {
            // var now = new Date(),
            let y = now.getFullYear();
            let m = ('0' + (now.getMonth() + 1)).slice(-2);
            let d = ('0' + now.getDate()).slice(-2);
            if (type == 'Y-mm-dd 00:00:00') {
                let temp = y + '/' + m + '/' + d + ' 00:00:00';
                temp = temp.replace(/-/g, '/');
                return temp;
            }
            else {
                let temp = y + '/' + m + '/' + d + ' 23:59:59';
                temp = temp.replace(/-/g, '/');
                return temp;
            }
        };
        this.genclass = function (url) {
            if (url == '' || url == null) {
                return '';
            }
            else {
                let urlarr = url.toLowerCase().split('.');
                let ext = urlarr[urlarr.length - 1];
                let logotype = '';
                switch (ext) {
                    case 'xls':
                        logotype = 'fa fa-file-excel fa-4x green';
                        break;
                    case 'xlsx':
                        logotype = 'fa fa-file-excel fa-4x green';
                        break;
                    case 'doc':
                        logotype = 'fa fa-file-word fa-4x blue';
                        break;
                    case 'docx':
                        logotype = 'fa fa-file-word fa-4x blue';
                        break;
                    case 'ppt':
                        logotype = 'fa fa-file-powerpoint fa-4x red';
                        break;
                    case 'pptx':
                        logotype = 'fa fa-file-powerpoint fa-4x red';
                        break;
                    case 'ppt':
                        logotype = 'fa fa-file-pdf fa-4x red';
                        break;
                    case 'pdf':
                        logotype = 'fa fa-file-pdf fa-4x red';
                        break;
                    default:
                        logotype = 'fa fa-file fa-4x';
                }
                return logotype;
            }
        };
        this.isInvalid = function (obj, list) {
            for (let i = 0; i < list.length; i++) {
                if (list[i] in obj && !obj[list[i]]) {
                    return true;
                }
            }
            return false;
        };
        this.getNextDate = function (date, day = 1, format = '{y}-{m}-{d}') {
            if (date) {
                const nDate = new Date(date);
                nDate.setDate(nDate.getDate() + day);
                const formatObj = {
                    y: nDate.getFullYear(),
                    m: nDate.getMonth() + 1,
                    d: nDate.getDate(),
                };
                return format.replace(/{([ymd])+}/g, (result, key) => {
                    const value = formatObj[key];
                    return value.toString().padStart(2, '0');
                });
            }
            else {
                console.log('date can not be null');
                return;
            }
        };
        this.second2Hour = function (value) {
            //  秒
            let second = value;
            //  分
            let minute = 0;
            //  小时
            let hour = 0;
            //  天
            //  let day = 0
            //  如果秒数大于60，将秒数转换成整数
            if (second > 60) {
                //  获取分钟，除以60取整数，得到整数分钟
                minute = parseInt((second / 60).toString());
                //  获取秒数，秒数取佘，得到整数秒数
                second = parseInt((second % 60).toString());
                //  如果分钟大于60，将分钟转换成小时
                if (minute > 60) {
                    //  获取小时，获取分钟除以60，得到整数小时
                    hour = parseInt((minute / 60).toString());
                    //  获取小时后取佘的分，获取分钟除以60取佘的分
                    minute = parseInt((minute % 60).toString());
                    //  如果小时大于24，将小时转换成天
                    //  if (hour > 23) {
                    //    //  获取天数，获取小时除以24，得到整天数
                    //    day = parseInt(hour / 24)
                    //    //  获取天数后取余的小时，获取小时除以24取余的小时
                    //    hour = parseInt(hour % 24)
                    //  }
                }
            }
            let result = '' + parseInt(second.toString()) + 'S';
            if (minute > 0) {
                result = '' + parseInt(minute.toString()) + 'M' + result;
            }
            if (hour > 0) {
                result = '' + parseInt(hour.toString()) + 'H' + result;
            }
            //  if (day > 0) {
            //    result = '' + parseInt(day) + '天' + result
            //  }
            return result;
        };
        /**
         * Array transform to tree
         * @param {array} data
         * @param {string} id
         */
        // 数组转换成tree
        this.arrayToTree = function (data, id) {
            const result = [];
            if (!Array.isArray(data)) {
                return result;
            }
            const map = {};
            data.forEach(item => {
                delete item.children;
                // @ts-ignore
                map[item[id]] = item;
            });
            data.forEach(item => {
                // @ts-ignore
                const parent = map[item.parentId] || map[item.parentCode];
                if (parent) {
                    (parent.children || (parent.children = [])).push(item);
                }
                else {
                    result.push(item);
                }
            });
            return result;
        };
        /**
         * Tree transform to array
         * @param tree
         */
        // Tree to array
        this.treeToArray = function (tree) {
            const arr = [];
            const expanded = (datas) => {
                if (datas && datas.length > 0) {
                    datas.forEach((e) => {
                        arr.push(e);
                        expanded(e.children);
                    });
                }
            };
            expanded(tree);
            return arr;
        };
        this.printNewWindow = function (imgSrc, option) {
            let oWin = window.open('', 'pringwindow', 'menubar=no,location=no,resizable=yes,scrollbars=no,status=no,width=' +
                window.screen.availWidth +
                ',height=' +
                window.screen.availHeight);
            oWin.document.fn = function () {
                if (oWin) {
                    oWin.print();
                    oWin.close();
                }
            };
            let title = '';
            if (option && option.title) {
                title =
                    '<h3 style="width: 100%;text-align: center;margin: 100px 0;font-size: 24px;">' +
                        option.title +
                        '</h3>';
            }
            let html = '<div style="height: 100%;width: 100%;display: flex;flex-direction: column;align-items: center">' +
                title +
                `<img src="${imgSrc}" onload="fn()" style="width: 90%;" />` +
                '</div>' +
                '<style type="text/css">' +
                '@page { margin: 0; }' +
                '</style>';
            oWin.document.open();
            oWin.document.write(html);
            oWin.document.close();
        };
        this.getHourByMinute = function (minute) {
            return Math.trunc(minute / 60);
        };
        this.formInvalid = function (obj, list) {
            for (let i = 0; i < list.length; i++) {
                if (list[i] in obj && !obj[list[i]]) {
                    return list[i];
                }
                else if (Object.prototype.toString.call(obj[list[i]]) === '[object Array]' &&
                    obj[list[i]].length === 0) {
                    return list[i];
                }
            }
            return false;
        };
        this.getBase64 = (file) => new Promise((resolve, reject) => {
            const reader = new FileReader();
            reader.readAsDataURL(file);
            reader.onload = () => resolve(reader.result);
            reader.onerror = error => reject(error);
        });
        this.downloadFile = function (urlStr, fileName) {
            const link = document.createElement('a');
            const url = '/gateway/mfgtranning/api/mfg/Common/file?objectName=' + urlStr;
            link.style.display = 'none';
            link.href = url;
            link.setAttribute('download', fileName);
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        };
        this.optionsValid = function (optionList, type, messageService, TranslateData) {
            const optionCount = optionList.length;
            let YCount = 0;
            if (optionCount === 0) {
                messageService.add({ severity: 'warn', summary: TranslateData.questionOption1Warn });
                return false;
            }
            optionList.forEach(item => {
                if (item.answer) {
                    YCount++;
                }
            });
            if (type === '1') {
                //  判断题
                if (optionCount !== 2) {
                    messageService.add({
                        severity: 'warn',
                        summary: TranslateData.questionOption2Warn,
                    });
                    return false;
                }
                if (YCount !== 1) {
                    messageService.add({
                        severity: 'warn',
                        summary: TranslateData.questionOption3Warn,
                    });
                    return false;
                }
            }
            else if (type === '2') {
                if (optionCount < 3) {
                    messageService.add({
                        severity: 'warn',
                        summary: TranslateData.questionOption4Warn,
                    });
                    return false;
                }
                if (YCount !== 1) {
                    messageService.add({
                        severity: 'warn',
                        summary: TranslateData.questionOption5Warn,
                    });
                    return false;
                }
            }
            else if (type === '3') {
                // 多选
                if (optionCount < 3) {
                    messageService.add({
                        severity: 'warn',
                        summary: TranslateData.questionOption4Warn,
                    });
                    return false;
                }
                if (YCount <= 1) {
                    messageService.add({
                        severity: 'warn',
                        summary: TranslateData.questionOption6Warn,
                    });
                    return false;
                }
            }
            return true;
        };
        this.translateData = function (Obj, translate) {
            for (let item in Obj) {
                if (Obj.hasOwnProperty(item)) {
                    translate.get(Obj[item]).subscribe((data) => {
                        Obj[item] = data;
                    });
                }
            }
        };
        this.imgToBase64 = function (imgSrc) {
            const canvas = document.createElement('CANVAS'), ctx = canvas.getContext('2d'), img = new Image;
            img.crossOrigin = 'Anonymous';
            img.src = imgSrc;
            img.onload = function () {
                canvas.height = img.height;
                canvas.width = img.width;
                ctx.drawImage(img, 0, 0);
                const dataURL = canvas.toDataURL('image/png');
                // me.floodImage = blob.replace('data:image/png;base64,', '');
            };
        };
        this.setFullScreen = function (exitFullScreen) {
            var _a;
            if (localLayout) {
                localLayout.fullPage = !exitFullScreen;
            }
            (_a = window === null || window === void 0 ? void 0 : window.parent) === null || _a === void 0 ? void 0 : _a.postMessage({
                type: 'jabil-bus-full-screen', data: { exitFullScreen: exitFullScreen }
            }, '*');
        };
        this.logoff = function () {
            const removeItems = [
                'loginCheck',
                'loginType',
                'selectedIndex',
                'jwt',
                'tabs',
                'tabMenu',
                'region',
                'currentRegion',
                'username',
                'defaultUrl',
                'roles',
                'translateApp',
                'zh',
                'en'
            ];
            removeItems.forEach((item) => {
                localStorage.removeItem(item);
            });
            const langCacheKey = localStorage.getItem('langCacheKey');
            localStorage.removeItem('' + langCacheKey);
            window.parent.location.reload();
        };
        this.getToken = function () {
            return localStorage.getItem('jwt');
        };
    }
    buildEChartLabel(params, n, agrs) {
        if (!params) {
            return;
        }
        let newParamsName = ''; // 最终拼接成的字符串
        let paramsNameNumber = params.length; // 实际标签的个数
        const provideNumber = n || 4; // 每行能显示的字的个数
        const rowNumber = Math.ceil(paramsNameNumber / provideNumber); // 换行的话，需要显示几行，向上取整
        /**
         * 判断标签的个数是否大于规定的个数， 如果大于，则进行换行处理 如果不大于，即等于或小于，就返回原标签
         */
        // 条件等同于rowNumber>1
        if (paramsNameNumber > provideNumber) {
            /** 循环每一行,p表示行 */
            for (var p = 0; p < rowNumber; p++) {
                var tempStr = ''; // 表示每一次截取的字符串
                var start = p * provideNumber; // 开始截取的位置
                var end = start + provideNumber; // 结束截取的位置
                // 此处特殊处理最后一行的索引值
                if (p == rowNumber - 1) {
                    // 最后一次不换行
                    tempStr = params.substring(start, paramsNameNumber);
                }
                else {
                    // 每一次拼接字符串并换行
                    tempStr = params.substring(start, end) + '\n';
                }
                newParamsName += tempStr; // 最终拼成的字符串
            }
        }
        else {
            // 将旧标签的值赋给新标签
            newParamsName = params;
        }
        //将最终的字符串返回
        let argsStr = '';
        agrs === null || agrs === void 0 ? void 0 : agrs.forEach((item) => {
            argsStr += '\n' + item;
        });
        return newParamsName + argsStr;
    }
    addRouteEvent(router) {
        if (!(router === null || router === void 0 ? void 0 : router.navigate)) {
            console.error('jabil-bus-lib param error: router.navigate is undefined');
            return;
        }
        window.addEventListener('updateAppUrl', (e) => {
            router.navigate([e.detail.path]);
        });
    }
    saveLayout(layout) {
        localLayout = layout;
    }
    getGuid() {
        return ('' + [1e7] + -1e3 + -4e3 + -8e3 + -1e11).replace(/[018]/g, ch => {
            let c = Number(ch);
            return (c ^ crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> c / 4).toString(16);
        });
    }
    initWebNotice(signalR) {
        console.log(signalR);
        const isHttps = document.location.protocol === 'https:';
        const testWS = 'http://cnhuam0itstg83:44023/modelstatus-statistics-messaging-hub';
        const prodWS = 'https://jabilbus.jblapps.com/ws/occomain/modelstatus-statistics-messaging-hub';
        const wsBayUrl = isHttps ? prodWS : testWS;
        const connection = new signalR.HubConnectionBuilder()
            .withUrl(wsBayUrl)
            .configureLogging(signalR.LogLevel.Information)
            .build();
        connection.on('ReceiveMessageAsync', (data) => {
            this.reciveWebNotice(data);
            if (data === null || data === void 0 ? void 0 : data.messageBody) {
                console.log(data === null || data === void 0 ? void 0 : data.messageBody);
            }
        });
        const start = () => __awaiter(this, void 0, void 0, function* () {
            try {
                yield connection.start();
                console.log('SignalR Connected.');
            }
            catch (err) {
                console.log(err);
                setTimeout(start, 5000);
            }
        });
        connection.onclose(() => __awaiter(this, void 0, void 0, function* () {
            yield start();
        }));
        // Start the connection.
        start();
    }
    reciveWebNotice(data) {
        let options = null;
        var PERMISSON_GRANTED = "granted";
        var PERMISSON_DENIED = "denied";
        var PERMISSON_DEFAULT = "default";
        console.log(document.location);
        // @ts-ignore
        window.notices.forEach((item) => {
            // 判断key是否相同
            if (item) {
                options = item;
            }
        });
        if (!options) {
            return;
        }
        // 如果用户已经允许，直接显示消息，如果不允许则提示用户授权
        if (Notification.permission === PERMISSON_GRANTED) {
            this.notify(options);
        }
        else {
            Notification.requestPermission((res) => {
                if (res === PERMISSON_GRANTED) {
                    this.notify(options);
                }
            });
        }
    }
    notify($options, callback) {
        var notification = new Notification($options === null || $options === void 0 ? void 0 : $options.title, $options);
        notification.onshow = (event) => {
            console.log("show : ", event);
        };
        notification.onclose = (event) => {
            console.log("close : ", event);
        };
        notification.onclick = (event) => {
            var _a, _b;
            console.log("click : ", event);
            // 当点击事件触发，打开指定的url
            ((_a = event === null || event === void 0 ? void 0 : event.target) === null || _a === void 0 ? void 0 : _a.data) && window.open((_b = event === null || event === void 0 ? void 0 : event.target) === null || _b === void 0 ? void 0 : _b.data);
            notification.close();
        };
    }
    registerNotice(options) {
        // @ts-ignore
        window.notices = [];
        // @ts-ignore
        window.notices.push(options);
    }
}
CommonService.ɵfac = function CommonService_Factory(t) { return new (t || CommonService)(); };
CommonService.ɵprov = /*@__PURE__*/ i0.ɵɵdefineInjectable({ token: CommonService, factory: CommonService.ɵfac });
(function () {
    (typeof ngDevMode === "undefined" || ngDevMode) && i0.ɵsetClassMetadata(CommonService, [{
            type: Injectable
        }], null, null);
})();

const _c0$2 = function (a0, a1) { return { width: a0, height: a1 }; };
class QRComponent {
    constructor() {
        this.width = '200px';
        this.height = '200px';
        this.QRUrl = '';
        // console.log(QRCode)
    }
    ngOnInit() {
        return __awaiter(this, void 0, void 0, function* () {
            this.getQR();
        });
    }
    getQR() {
        var _a;
        QRCode.toDataURL((_a = this.field) === null || _a === void 0 ? void 0 : _a.id)
            .then((url) => {
            this.QRUrl = url;
        })
            .catch((err) => {
            console.error(err);
        });
    }
    printQR() {
        const commonService = new CommonService();
        commonService.printNewWindow(this.QRUrl, this.field);
    }
}
QRComponent.ɵfac = function QRComponent_Factory(t) { return new (t || QRComponent)(); };
QRComponent.ɵcmp = /*@__PURE__*/ i0.ɵɵdefineComponent({ type: QRComponent, selectors: [["jabil-qr"]], inputs: { field: "field", width: "width", height: "height" }, decls: 3, vars: 6, consts: [[1, "print-content"], [3, "src"]], template: function QRComponent_Template(rf, ctx) {
        if (rf & 1) {
            i0.ɵɵelementStart(0, "div")(1, "div", 0);
            i0.ɵɵelement(2, "img", 1);
            i0.ɵɵelementEnd()();
        }
        if (rf & 2) {
            i0.ɵɵadvance(2);
            i0.ɵɵstyleMap(i0.ɵɵpureFunction2(3, _c0$2, ctx.width, ctx.height));
            i0.ɵɵproperty("src", ctx.QRUrl, i0.ɵɵsanitizeUrl);
        }
    }, styles: ["[_nghost-%COMP%]{justify-content:center;display:flex}"] });
(function () {
    (typeof ngDevMode === "undefined" || ngDevMode) && i0.ɵsetClassMetadata(QRComponent, [{
            type: Component,
            args: [{ selector: 'jabil-qr', template: "<div>\r\n  <div class=\"print-content\">\r\n    <img [style]=\"{width: width,height: height}\" [src]=\"QRUrl\" />\r\n  </div>\r\n</div>\r\n", styles: [":host{justify-content:center;display:flex}\n"] }]
        }], function () { return []; }, { field: [{
                type: Input
            }], width: [{
                type: Input
            }], height: [{
                type: Input
            }] });
})();

class QRModule {
}
QRModule.ɵfac = function QRModule_Factory(t) { return new (t || QRModule)(); };
QRModule.ɵmod = /*@__PURE__*/ i0.ɵɵdefineNgModule({ type: QRModule });
QRModule.ɵinj = /*@__PURE__*/ i0.ɵɵdefineInjector({ imports: [CommonModule] });
(function () {
    (typeof ngDevMode === "undefined" || ngDevMode) && i0.ɵsetClassMetadata(QRModule, [{
            type: NgModule,
            args: [{
                    declarations: [
                        QRComponent
                    ],
                    imports: [
                        CommonModule
                    ],
                    exports: [
                        QRComponent
                    ]
                }]
        }], null, null);
})();
(function () { (typeof ngJitMode === "undefined" || ngJitMode) && i0.ɵɵsetNgModuleScope(QRModule, { declarations: [QRComponent], imports: [CommonModule], exports: [QRComponent] }); })();

function VideoDialogComponent_ng_template_2_Template(rf, ctx) {
    if (rf & 1) {
        i0.ɵɵelement(0, "h3", 9);
    }
}
const _c0$1 = function () { return { width: "100%", height: "100%" }; };
class VideoDialogComponent {
    constructor(sanitizer) {
        this.sanitizer = sanitizer;
        this.closeDialog = new EventEmitter();
        this.dialogDisplay = true;
        this.currentLanguage = 'en';
        this.srcUrl = '';
    }
    ngOnInit() {
        return __awaiter(this, void 0, void 0, function* () {
            this.srcUrl = this.sanitizer.bypassSecurityTrustResourceUrl(this.field);
        });
    }
    getSrcUrl() {
        return this.sanitizer.bypassSecurityTrustResourceUrl(this.field);
    }
    closeEditDialog() {
        this.dialogDisplay = false;
        this.closeDialog.emit();
        this.dialogDisplay = true;
    }
}
VideoDialogComponent.ɵfac = function VideoDialogComponent_Factory(t) { return new (t || VideoDialogComponent)(i0.ɵɵdirectiveInject(i1$1.DomSanitizer)); };
VideoDialogComponent.ɵcmp = /*@__PURE__*/ i0.ɵɵdefineComponent({ type: VideoDialogComponent, selectors: [["jabil-video-dialog"]], inputs: { field: "field" }, outputs: { closeDialog: "closeDialog" }, features: [i0.ɵɵProvidersFeature([])], decls: 9, vars: 11, consts: [["styleClass", "p-fluid", 3, "visible", "modal", "draggable", "resizable", "visibleChange", "onHide"], ["dialog", ""], ["pTemplate", "header"], ["width", "100%", "height", "100%", "controls", ""], ["type", "video/mp4", 3, "src"], ["type", "video/ogg", 3, "src"], ["type", "video/webm", 3, "src"], ["data", i0.ɵɵtrustConstantResourceUrl `movie.mp4`, "width", "104", "height", "104"], ["width", "100%", "height", "100%", 3, "src"], [1, "text-center", "w-100"]], template: function VideoDialogComponent_Template(rf, ctx) {
        if (rf & 1) {
            i0.ɵɵelementStart(0, "p-dialog", 0, 1);
            i0.ɵɵlistener("visibleChange", function VideoDialogComponent_Template_p_dialog_visibleChange_0_listener($event) { return ctx.dialogDisplay = $event; })("onHide", function VideoDialogComponent_Template_p_dialog_onHide_0_listener() { return ctx.closeEditDialog(); });
            i0.ɵɵtemplate(2, VideoDialogComponent_ng_template_2_Template, 1, 0, "ng-template", 2);
            i0.ɵɵelementStart(3, "video", 3);
            i0.ɵɵelement(4, "source", 4)(5, "source", 5)(6, "source", 6);
            i0.ɵɵelementStart(7, "object", 7);
            i0.ɵɵelement(8, "embed", 8);
            i0.ɵɵelementEnd()()();
        }
        if (rf & 2) {
            i0.ɵɵstyleMap(i0.ɵɵpureFunction0(10, _c0$1));
            i0.ɵɵproperty("visible", ctx.dialogDisplay)("modal", true)("draggable", false)("resizable", false);
            i0.ɵɵadvance(4);
            i0.ɵɵproperty("src", ctx.getSrcUrl(), i0.ɵɵsanitizeUrl);
            i0.ɵɵadvance(1);
            i0.ɵɵproperty("src", ctx.getSrcUrl(), i0.ɵɵsanitizeUrl);
            i0.ɵɵadvance(1);
            i0.ɵɵproperty("src", ctx.getSrcUrl(), i0.ɵɵsanitizeUrl);
            i0.ɵɵadvance(2);
            i0.ɵɵproperty("src", ctx.getSrcUrl(), i0.ɵɵsanitizeResourceUrl);
        }
    }, dependencies: [i2$1.Dialog, i3$1.PrimeTemplate], styles: ["[_nghost-%COMP%]{height:100%;display:block}[_nghost-%COMP%]   .card[_ngcontent-%COMP%]{height:100%;background:#fff;padding:24px;border-radius:12px}[_nghost-%COMP%]   .card[_ngcontent-%COMP%]   label[_ngcontent-%COMP%]{width:100%}[_nghost-%COMP%]     .p-dialog{max-height:100%}[_nghost-%COMP%]     .p-dialog .p-dialog-header, [_nghost-%COMP%]     .p-dialog .p-dialog-content{background:rgba(0,0,0,.9)}[_nghost-%COMP%]     .p-dialog .p-dialog-header-close-icon{color:#f8f9fa}"] });
(function () {
    (typeof ngDevMode === "undefined" || ngDevMode) && i0.ɵsetClassMetadata(VideoDialogComponent, [{
            type: Component,
            args: [{ selector: 'jabil-video-dialog', providers: [], template: "<p-dialog\r\n  #dialog\r\n  [(visible)]=\"dialogDisplay\"\r\n  [style]=\"{ width: '100%',height: '100%' }\"\r\n  [modal]=\"true\"\r\n  [draggable]=\"false\"\r\n  [resizable]=\"false\"\r\n  (onHide)=\"closeEditDialog()\"\r\n  styleClass=\"p-fluid\">\r\n  <ng-template pTemplate=\"header\">\r\n    <h3 class=\"text-center w-100\"></h3>\r\n  </ng-template>\r\n\r\n  <video width=\"100%\" height=\"100%\" controls>\r\n    <source\r\n      [src]=\"getSrcUrl()\"\r\n      type=\"video/mp4\" />\r\n    <source\r\n      [src]=\"getSrcUrl()\"\r\n      type=\"video/ogg\" />\r\n    <source\r\n      [src]=\"getSrcUrl()\"\r\n      type=\"video/webm\" />\r\n    <object data=\"movie.mp4\" width=\"104\" height=\"104\">\r\n      <embed\r\n        [src]=\"getSrcUrl()\"\r\n        width=\"100%\"\r\n        height=\"100%\" />\r\n    </object>\r\n  </video>\r\n</p-dialog>\r\n", styles: [":host{height:100%;display:block}:host .card{height:100%;background:#fff;padding:24px;border-radius:12px}:host .card label{width:100%}:host ::ng-deep .p-dialog{max-height:100%}:host ::ng-deep .p-dialog .p-dialog-header,:host ::ng-deep .p-dialog .p-dialog-content{background:rgba(0,0,0,.9)}:host ::ng-deep .p-dialog .p-dialog-header-close-icon{color:#f8f9fa}\n"] }]
        }], function () { return [{ type: i1$1.DomSanitizer }]; }, { field: [{
                type: Input
            }], closeDialog: [{
                type: Output
            }] });
})();

class VideoDialogModule {
}
VideoDialogModule.ɵfac = function VideoDialogModule_Factory(t) { return new (t || VideoDialogModule)(); };
VideoDialogModule.ɵmod = /*@__PURE__*/ i0.ɵɵdefineNgModule({ type: VideoDialogModule });
VideoDialogModule.ɵinj = /*@__PURE__*/ i0.ɵɵdefineInjector({ imports: [CommonModule,
        DialogModule] });
(function () {
    (typeof ngDevMode === "undefined" || ngDevMode) && i0.ɵsetClassMetadata(VideoDialogModule, [{
            type: NgModule,
            args: [{
                    declarations: [VideoDialogComponent],
                    imports: [
                        CommonModule,
                        DialogModule
                    ],
                    exports: [
                        VideoDialogComponent
                    ]
                }]
        }], null, null);
})();
(function () {
    (typeof ngJitMode === "undefined" || ngJitMode) && i0.ɵɵsetNgModuleScope(VideoDialogModule, { declarations: [VideoDialogComponent], imports: [CommonModule,
            DialogModule], exports: [VideoDialogComponent] });
})();

class SimpleReuseStrategy {
    shouldReuseRoute(future, curr) {
        return future.routeConfig === curr.routeConfig;
    }
    shouldDetach(route) {
        return false;
        // console.log(route.data['keep'])
        //   return Boolean(route.data['keep']);
    }
    store(route, handle) {
        const url = this.getFullRouteUrl(route);
        const data = this.getRouteData(route);
        if (SimpleReuseStrategy.waitDelete && SimpleReuseStrategy.waitDelete === url) {
            SimpleReuseStrategy.waitDelete = null;
            return;
        }
        else {
            if (SimpleReuseStrategy.currentDelete && SimpleReuseStrategy.currentDelete === url) {
                SimpleReuseStrategy.currentDelete = null;
                return;
            }
            else {
                SimpleReuseStrategy.routeCache.set(url, { handle, data });
                this.addRedirectsRecursively(route);
            }
        }
    }
    shouldAttach(route) {
        const url = this.getFullRouteUrl(route);
        return SimpleReuseStrategy.routeCache.has(url);
    }
    retrieve(route) {
        var _a;
        const url = this.getFullRouteUrl(route);
        const data = this.getRouteData(route);
        return data && SimpleReuseStrategy.routeCache.has(url)
            ? (_a = SimpleReuseStrategy.routeCache.get(url)) === null || _a === void 0 ? void 0 : _a.handle
            : null;
    }
    addRedirectsRecursively(route) {
        const config = route.routeConfig;
        if (config) {
            if (!config.loadChildren) {
                const routeFirstChild = route.firstChild;
                const routeFirstChildUrl = routeFirstChild
                    ? this.getRouteUrlPaths(routeFirstChild).join('/')
                    : '';
                const childConfigs = config.children;
                if (childConfigs) {
                    const childConfigWithRedirect = childConfigs.find(c => c.path === '' && !!c.redirectTo);
                    if (childConfigWithRedirect) {
                        childConfigWithRedirect.redirectTo = routeFirstChildUrl;
                    }
                }
            }
            route.children.forEach(childRoute => this.addRedirectsRecursively(childRoute));
        }
    }
    getFullRouteUrl(route) {
        return this.getFullRouteUrlPaths(route).filter(Boolean).join('/').replace('/', '_');
    }
    getFullRouteUrlPaths(route) {
        const paths = this.getRouteUrlPaths(route);
        return route.parent ? [...this.getFullRouteUrlPaths(route.parent), ...paths] : paths;
    }
    getRouteUrlPaths(route) {
        return route.url.map(urlSegment => urlSegment.path);
    }
    getRouteData(route) {
        return route.routeConfig && route.routeConfig.data;
    }
    /** delete the route snapshot*/
    static deleteRouteSnapshot(url) {
        if (url[0] === '/') {
            url = url.substring(1);
        }
        url = url.replace('/', '_');
        if (SimpleReuseStrategy.routeCache.has(url)) {
            SimpleReuseStrategy.routeCache.delete(url);
            SimpleReuseStrategy.currentDelete = url;
        }
        else {
            SimpleReuseStrategy.waitDelete = url;
        }
    }
}
SimpleReuseStrategy.routeCache = new Map();
SimpleReuseStrategy.ɵfac = function SimpleReuseStrategy_Factory(t) { return new (t || SimpleReuseStrategy)(); };
SimpleReuseStrategy.ɵprov = /*@__PURE__*/ i0.ɵɵdefineInjectable({ token: SimpleReuseStrategy, factory: SimpleReuseStrategy.ɵfac });
(function () {
    (typeof ngDevMode === "undefined" || ngDevMode) && i0.ɵsetClassMetadata(SimpleReuseStrategy, [{
            type: Injectable
        }], null, null);
})();

var translateVersion$1 = "1.6";
var login$1 = "Login";
var logoff$1 = "Logout";
var username$1 = "Username";
var password$1 = "Password";
var reset$1 = "Reset";
var search$1 = "Search";
var status$1 = "Status";
var operation$1 = "Operation";
var input$1 = "Input";
var alert$1 = "Alert";
var cancel$1 = "Cancel";
var all$1 = "All";
var normal$1 = "Normal";
var warning$1 = "Warning";
var filter$1 = "Filter";
var submit$1 = "Submit";
var date$1 = "Date";
var data$1 = "Data";
var customerBtn$1 = "Customer";
var day$1 = "Day";
var No$1 = "No";
var add$1 = "Add";
var modify$1 = "Modify";
var save$1 = "Save";
var disabled$1 = "Disabled";
var enabled$1 = "Enabled";
var nodata$1 = "No Data";
var time$1 = "Time";
var close$1 = "Close";
var sort$1 = "Sort";
var s$1 = "S";
var m$1 = "M";
var h$1 = "H";
var category$1 = "Category";
var yes$1 = "Yes";
var no$1 = "No";
var online$1 = "Online";
var offline$1 = "Offline";
var permission$1 = "Permission";
var confirm$1 = "Confirm";
var hour$1 = "Hour";
var list$1 = "List";
var unresolved$1 = "Unresolved";
var processing$1 = "In Process";
var processed$1 = "Completed ";
var reviewed$1 = "UnReviewed";
var upgraded$1 = "Upgraded";
var min$1 = "Min";
var owners$1 = "Owners";
var description$1 = "Description";
var severity$1 = "Severity";
var station$1 = "Station";
var detail$1 = "Detail";
var pic$1 = "PIC";
var message$1 = "Message";
var task$1 = "Task";
var unread$1 = "Unread";
var view$1 = "View";
var technician$1 = "Technician";
var engineer$1 = "Engineer";
var shift$1 = "Shift";
var tag$1 = "Tag";
var author$1 = "Author";
var reference$1 = "Reference";
var name$1 = "Name";
var upload$1 = "Upload";
var solution$1 = "Solution";
var action$1 = "Action";
var process$1 = "Process";
var model$1 = "Model";
var summary$1 = "Summary";
var approve$1 = "Approve";
var workflow$1 = "Workflow";
var print$1 = "Print";
var NTID$1 = "NTID";
var proposal$1 = "Proposal";
var satisfaction$1 = "Satisfaction";
var sector$1 = "Sector";
var attachment$1 = "Attachment";
var download$1 = "Download";
var back$1 = "Back";
var total$1 = "Total";
var items$1 = "Items";
var icon$1 = "Icon";
var order$1 = "Order";
var comments$1 = "Comments";
var release$1 = "Release";
var edit$1 = "Edit";
var email$1 = "Email";
var invalid$1 = "Invalid";
var reminder$1 = "Reminder";
var user$1 = "User";
var phone$1 = "Phone";
var roles$1 = "Roles";
var code$1 = "Code";
var title$1 = "Title";
var comment$1 = "Comment";
var read$1 = "Read";
var type$1 = "Type";
var unit$1 = "unit";
var remark$1 = "Remark";
var copy$1 = "Copy";
var plant$1 = "Plant";
var host$1 = "Host";
var port$1 = "Port";
var kind$1 = "Kind";
var region$1 = "Region";
var stream$1 = "Stream";
var endPoint$1 = "End Point";
var broker$1 = "Broker";
var queue$1 = "Queue";
var equipment$1 = "Equipment";
var db$1 = "DB";
var run$1 = "Run";
var check$1 = "Check";
var left$1 = "Left";
var top$1 = "Top";
var bay$1 = "Bay";
var menu$1 = "Menu";
var api$1 = "API";
var service$1 = "Service";
var project$1 = "Project";
var cycle$1 = "Cycle";
var reassign$1 = "Reassign";
var topic$1 = "Topic";
var satisfied$1 = "Satisfied";
var ordinary$1 = "Ordinary";
var dissatisfied$1 = "Dissatisfied";
var reason$1 = "Reason";
var approver$1 = "Approver";
var opinion$1 = "Opinion";
var agree$1 = "Agree";
var reject$1 = "Reject";
var application$1 = "Application";
var benefit$1 = "Benefit";
var confirmed$1 = "Confirmed";
var unconfirmed$1 = "Unconfirmed";
var approving$1 = "Approving";
var note$1 = "Note";
var company$1 = "Company";
var refresh$1 = "Refresh";
var Upgraded$1 = "Upgraded";
var NotProcessed$1 = "Pending";
var Processing$1 = "In Progress ";
var Processed$1 = "Completed";
var Approving$1 = "Approving";
var Closed$1 = "Close";
var Low$1 = "Low";
var Normal$1 = "Normal";
var High$1 = "High";
var Urgent$1 = "Urgent";
var Immediately$1 = "Immediately";
var All$1 = "All";
var Close$1 = "Close";
var Running$1 = "Running";
var RUSH_TO_REPAIR$1 = "Rush To Repair";
var ADD_PERSON$1 = "Add Person";
var NPI$1 = "NPI";
var DOWNTIME$1 = "Downtime";
var POWER_CUT$1 = "Power Cut";
var RAINSTORM$1 = "Rainstorm";
var UNDERMANNED$1 = "Undermanned";
var WaitConfirm$1 = "Wait Confirm";
var HasConfirm$1 = "No Need Interview";
var WaitInterview$1 = "Wait Interview";
var HasInterview$1 = "Finish Interview";
var TestValidationManagement$1 = "E-Ticket";
var TemperatureManagement$1 = "Temperature Management";
var RemoteControl$1 = "Remote Control";
var TeslaCTTrackingManagement$1 = "Tesla CT Tracking";
var TeslaAutoBuildPlan$1 = "Auto Build Plan";
var TeslaOrder$1 = "Order";
var TeslaPlan$1 = "Plan";
var TeslaPlanMonitoring$1 = "Plan Monitoring";
var MFGPeopleManagement$1 = "MFG People Management";
var MFGDashboard$1 = "Dashboard";
var MFGTranning$1 = "College of Manufacturing";
var MFGEmployeeCare$1 = "Employee Care";
var MFGEBuddy$1 = "E-Buddy";
var EPromotion$1 = "E-Promotion";
var MFGPerformance$1 = "Performance";
var WIKIHome$1 = "WIKI Home";
var WIKIBackground$1 = "Management";
var WIKIDashboard$1 = "Dashboard";
var WIKITopic$1 = "Topic";
var WIKIMyTopic$1 = "My Topic";
var WIKIComment$1 = "Comment";
var WIKIMyComment$1 = "My Comment";
var System$1 = "System";
var Users$1 = "Users";
var Roles$1 = "Roles";
var BatchRoles$1 = "Batch Roles";
var Menus$1 = "Menus";
var AuditLog$1 = "AuditLog";
var ES$1 = "ES";
var DataDictionary$1 = "DataDictionary";
var SystemAnnouncement$1 = "Announcement";
var ESCSManagement$1 = "Digital Twins";
var Home$1 = "Home";
var MesStreamConfig$1 = "Mes Config";
var MesStreamWip$1 = "Wip Search";
var WorkflowConfig$1 = "Workflow Config";
var WorkflowTeslaConfig$1 = "Tesla Config";
var WorkflowPermissionConfig$1 = "Permission Config";
var Sediot$1 = "SolarEdge IOT";
var SediotCycleTime$1 = "Machine Cycle Time";
var SediotEmailConfig$1 = "Machine Process Report Mail Config";
var SediotProcessReport$1 = "Machine Process Report";
var SediotUtilizationReport$1 = "Machine Utilization Report";
var SediotPMAlert$1 = "Machine PM";
var SediotLessonLearn$1 = "Machine Lesson Learn";
var SediotLayout$1 = "Machine Layout";
var OC$1 = "Smart Change Over";
var OCConfig$1 = "Data Config";
var OCSummary$1 = "Change Over";
var RealTime$1 = "Real Time";
var DataTraceability$1 = "Data Traceability";
var DataTraceabilityHomePage$1 = "Home Page";
var DataTraceabilityPortal$1 = "Portal";
var HOSAppWidget$1 = "App Widget";
var HOSRole$1 = "Role";
var HOSTemplate$1 = "Template";
var HOSRoleTemplateConfig$1 = "Role Template Config";
var HMCWebsite$1 = "HMC Website";
var ReportCenter$1 = "Report Center";
var wiki$1 = {
    forum: "Forum",
    "forum.name": "Forum Name",
    "subsidiary.forum": "Subsidiary Forum",
    "topic.video.max.size": "Video Max Size 10M"
};
var draft$1 = "Draft";
var reply$1 = "Reply";
var pcn$1 = {
    stage: "Stage",
    "parameter.name": "Parameter Name",
    "stage.name": "Stage Name",
    site: "Site",
    "material.group": "Material Group",
    "external.material.group": "External Material Group",
    part: "Part",
    "supplier.part": "Supplier Part",
    type: "Type",
    "type.desc": "Part",
    "make.buy": "Make/Buy",
    description: "Description",
    "customer.name": "Customer Name",
    "affected.models": "Affected Models",
    coordinator: "PCN Coordinator",
    "change.description": "Change Description",
    "check.man.count": "Check Man Count",
    "model.id": "Model Id",
    "machine.id": "Machine Id",
    "code.type": "Code Type"
};
var primeng$1 = {
    startsWith: "Starts with",
    contains: "Contains",
    notContains: "Not contains",
    endsWith: "Ends with",
    equals: "Equals",
    notEquals: "Not equals",
    noFilter: "No Filter",
    lt: "Less than",
    lte: "Less than or equal to",
    gt: "Greater than",
    gte: "Greater than or equal to",
    is: "Is",
    isNot: "Is not",
    before: "Before",
    after: "After",
    dateIs: "Date is",
    dateIsNot: "Date is not",
    dateBefore: "Date is before",
    dateAfter: "Date is after",
    clear: "Clear",
    apply: "Apply",
    matchAll: "Match All",
    matchAny: "Match Any",
    addRule: "Add Rule",
    removeRule: "Remove Rule",
    accept: "Yes",
    reject: "No",
    choose: "Choose",
    upload: "Upload",
    cancel: "Cancel",
    dayNames: [
        "Sunday",
        "Monday",
        "Tuesday",
        "Wednesday",
        "Thursday",
        "Friday",
        "Saturday"
    ],
    dayNamesShort: [
        "Sun",
        "Mon",
        "Tue",
        "Wed",
        "Thu",
        "Fri",
        "Sat"
    ],
    dayNamesMin: [
        "Su",
        "Mo",
        "Tu",
        "We",
        "Th",
        "Fr",
        "Sa"
    ],
    monthNames: [
        "January",
        "February",
        "March",
        "April",
        "May",
        "June",
        "July",
        "August",
        "September",
        "October",
        "November",
        "December"
    ],
    monthNamesShort: [
        "Jan",
        "Feb",
        "Mar",
        "Apr",
        "May",
        "Jun",
        "Jul",
        "Aug",
        "Sep",
        "Oct",
        "Nov",
        "Dec"
    ],
    dateFormat: "mm/dd/yy",
    firstDayOfWeek: 0,
    today: "Today",
    weekHeader: "Wk",
    weak: "Weak",
    medium: "Medium",
    strong: "Strong",
    passwordPrompt: "Enter a password",
    emptyMessage: "No results found",
    emptyFilterMessage: "No results found"
};
var PCNConfig$1 = "Code Config";
var PCNCodeParameter$1 = "Code Parameter";
var PCNCodeStage$1 = "Code Stage";
var PCNCodeTemplate$1 = "Code Template";
var PCNMasterTemplate$1 = "Master Template";
var PCNCodeBuild$1 = "Code Build";
var PCNMachine$1 = "Machine Control";
var PCNMachineInfo$1 = "Machine Info";
var PCNWorkflow$1 = "Workflow";
var PCNMachineModel$1 = "Machine Model";
var PCNMachineChange$1 = "Machine Change";
var PCNWorkcellCodeConfig$1 = "Workcell Code Config";
var PCNDefaultApprover$1 = "Default Approver";
var PCNNPI$1 = "NPI";
var PCNSOP$1 = "SOP";
var PCNApplyActionConfig$1 = "Application Config";
var PCNEscalation$1 = "Escalation";
var config$1 = "Config";
var preview$1 = "Preview";
var value$1 = "Value";
var synchronize$1 = "Synchronize";
var handle$1 = "Handle";
var initiator$1 = "Initiator";
var applicant$1 = "Applicant";
var originator$1 = "Originator";
var MasterDataManagement$1 = "Master Data Management";
var MasterDataHomepage$1 = "Home Page";
var MasterDataBay$1 = "Bay";
var MasterDataEquipment$1 = "Equipment";
var MasterDataEmployee$1 = "Employee";
var MasterDataHUAIDCreate$1 = "HUA ID Create";
var MasterDataEmployeeQuery$1 = "Employee Query";
var MasterDataOperationConfiguration$1 = "Operation Configuration";
var MasterDataFunction$1 = "Function";
var MasterDataOrganization$1 = "Organization";
var MasterDataLocation$1 = "Location";
var MasterDataProductionArea$1 = "Production Area";
var DataAssetMap$1 = "Data Asset Catalog";
var DataAssetHomepage$1 = "Home Page";
var DataAssetSQLSearch$1 = "SQL Query";
var DataAssetMasterDataManagement$1 = "Master Data Management";
var DataAssetTopicDomainDefinition$1 = "Topic Domain Definition";
var DataAssetMetadataManagement$1 = "Metadata Management";
var DataAssetDataApi$1 = "API";
var DataAssetDataAuthorization$1 = "Data Authorization";
var DataAssetDataSource$1 = "Data Source";
var DataAssetDataTable$1 = "Data Table";
var DataAssetDataAccessLog$1 = "Data Access Log";
var DataAssetDataAssetQuery$1 = "Data Asset Query";
var DataAssetDataAssetCatalog$1 = "Data Asset Catalog";
var DataAssetPermissionApplicationRecord$1 = "Permission Application Record";
var DataAssetUserPermissionApplicationRecord$1 = "User Permission Application Record";
var DataAssetAuthorizedData$1 = "Authorized Data";
var ThingsLinkerBroker$1 = "Broker";
var ThingsLinkerEquipment$1 = "Equipment";
var ThingsLinkerBooking$1 = "Booking";
var ThingsLinkerDispatchOrder$1 = "Dispatch Order";
var ThingsLinkerConfig$1 = "Config";
var ThingsLinker$1 = "Things Linker";
var JIT = "JIT";
var JiTBuildPlan$1 = "Build Plan";
var JITDeliveryBooking$1 = "Delivery Booking";
var JITPullListKanban$1 = "Pull List Kanban";
var JITSIgnFor$1 = "SIgn For";
var JITBuildPlanManagement$1 = "Build Plan Management";
var JITSubmitBuildPlan$1 = "Submit Build Plan";
var JITShortageList$1 = "Shortage List";
var JITPullListApply$1 = "Pull List Apply";
var JITDemandReport$1 = "Demand Report";
var JITRemainingInventory$1 = "Remaining Inventory";
var JITBaseData$1 = "Base Data";
var JITDummyBOM$1 = "Dummy BOM";
var JITUPH$1 = "UPH";
var JITModelFrequency$1 = "Model Frequency";
var JITAGVLineInfo$1 = "AGV Line Info";
var JITSPQ$1 = "SPQ";
var JITDeliveryLeadTime$1 = "Delivery Lead Time";
var JITStorageLocation$1 = "Storage Location";
var JITDeliveryTaskControl$1 = "Delivery Task Control";
var JITBuyerNameByPart$1 = "Buyer Name By Part";
var JITWarehouseManagement$1 = "Warehouse Management";
var JITWarehouseDelivery$1 = "Warehouse Delivery";
var JITInventoryManagement$1 = "Inventory Management";
var JITInventoryBin$1 = "Inventory Bin";
var JITInventoryReport$1 = "Inventory Report";
var JITInventoryStoreConfig$1 = "Inventory Store Config";
var JITInventory$1 = "Inventory";
var JITVendorAssignRate$1 = "Vendor Assign Rate";
var JITLockOpenPO$1 = "Lock Open PO";
var JITPDSRemindConfig$1 = "PDS Remind Config";
var JITPDSSearch$1 = "PDS Search";
var HuaDataLake$1 = "Hua Data Lake";
var HUAJabilBus$1 = "HUA Jabil Bus";
var HUAIOTPlatform$1 = "HUA IOT Platform";
var jit$1 = {
    "form.status": "Form Status",
    "part.number": "Part Number",
    "sap.model": "SAP Model",
    customer: "Customer",
    vendor: "Vendor",
    "material.request.time": "Material Request Time",
    "material.request.time.from": "Material Request Time From",
    "material.request.time.to": "Material Request Time To",
    "build.plan": "Build Plan",
    "pull.list.no": "Pull List No.",
    "delivery.booking": "Delivery Booking",
    "delivery.time": "Delivery Time",
    "delivery.type": "Delivery Type",
    "bin.name": "Bin Name",
    "cage.code": "Cage Code",
    "material.req.time": "Material Req Time",
    "storage.area": "Storage Area",
    "pull.list.pds.no": "Pull List/PDS No.",
    "pull.list.pds.no.status": "Pull List/PDS No. Status",
    "delivery.model": "Delivery Model",
    qty: "Qty",
    "received.time.from": "Received Time From",
    "received.time.to": "Received Time To",
    "received.by": "Received By",
    "submit.time.from": "Submit Time From",
    "submit.time.to": "Submit Time To",
    "submit.by": "Submit By",
    "received.from.wh.by": "Received From WH By",
    "stock.in.time.from": "Stock In Time From",
    "stock.in.time.to": "Stock In Time to",
    "stock.in.by": "Stock In By",
    "stock.out.time.from": "Stock Out Time From",
    "stock.out.time.to": "Stock Out Time to",
    "stock.out.by": "Stock Out By",
    "delivery.time.from": "Delivery Time From",
    "delivery.time.to": "Delivery Time To",
    "delivery.by": "Delivery By",
    "received.in.line.time.from": "Received In Line Time From",
    "received.in.line.time.to": "Received In Line Time To",
    "received.in.line.by": "Received In Line By",
    submitted: "Submitted",
    "await.receive": "Await Receive",
    "to.be.confirmed": "To Be Confirmed",
    received: "Received",
    caged: "Caged",
    "stocked.in": "Stocked In",
    "stocked.out": "Stocked Out",
    distributing: "Distributing",
    closed: "Closed",
    cancelled: "Cancelled",
    "received.from.wh.time.from": "Received From WH Time From",
    "received.from.wh.time.to": "Received From WH Time To",
    "material.request.time.invalid": "Material request time 2 hours greater than the current time，can not book"
};
var AOS$1 = "AOS";
var AOSCheckList$1 = "Check List";
var AOSAuditPlan$1 = "Audit Plan";
var AOSTaskList$1 = "Task List";
var AOSApproval = "Approval";
var AOSNCList$1 = "NC List";
var AOSConfig$1 = "Configuration";
var AOSAuditor$1 = "Auditor";
var AOSAuditorTest$1 = "Auditor";
var BMWIOT$1 = "BMWIOT";
var JMJagentMonitoring$1 = "Jagent Monitoring";
var JMMonitoring$1 = "Monitoring";
var JMWarningLog$1 = "Warning Log";
var JMConfig$1 = "Config";
var SMJagentMonitoring$1 = "Server Monitoring";
var SMMonitoring$1 = "Monitoring";
var SMWarningLog$1 = "Warning Log";
var SMConfig$1 = "Config";
var en = {
    translateVersion: translateVersion$1,
    "is.load.translate": "true",
    "export": "Export",
    "import": "Import",
    login: login$1,
    logoff: logoff$1,
    username: username$1,
    password: password$1,
    "remember.me": "Remember Me",
    "forgot.login.details": "Forgot Login Details",
    reset: reset$1,
    search: search$1,
    status: status$1,
    operation: operation$1,
    input: input$1,
    alert: alert$1,
    "return": "Return",
    cancel: cancel$1,
    all: all$1,
    normal: normal$1,
    warning: warning$1,
    filter: filter$1,
    submit: submit$1,
    date: date$1,
    data: data$1,
    customerBtn: customerBtn$1,
    day: day$1,
    No: No$1,
    add: add$1,
    modify: modify$1,
    save: save$1,
    "delete": "Delete",
    disabled: disabled$1,
    enabled: enabled$1,
    "start.time": "Start Time",
    "select.start.time": "Select Start Time",
    "end.time": "End Time",
    "select.end.time": "Select End Time",
    nodata: nodata$1,
    time: time$1,
    close: close$1,
    "select.time": "Select Time",
    "range.date": "Range Date",
    "range.time": "Range Time",
    "select.range.date": "Select start date and end date",
    "basic.information": "Basic information",
    sort: sort$1,
    s: s$1,
    m: m$1,
    h: h$1,
    "release.time": "Release Time",
    "upload.time": "Upload Time",
    "update.time": "Update Time",
    "category.name": "Category Name",
    category: category$1,
    yes: yes$1,
    no: no$1,
    online: online$1,
    offline: offline$1,
    permission: permission$1,
    "upload.file": "Upload File",
    "select.file": "Click or drag file to this area to upload",
    "save.success": "Save success!",
    "save.fail": "Save fail!",
    "delete.success": "Delete success!",
    "delete.fail": "Delete fail!",
    "is.delete": "Are you sure you want to delete the data?",
    "form.is.not.valid": "Form is not valid!",
    "webSocket.is.closed": "WebSocket is closed!",
    confirm: confirm$1,
    "file.upload.success": "File uploads success!",
    "file.upload.fail": "File upload fail!",
    hour: hour$1,
    "create.time": "Creat Time",
    "create.date": "Creat Date",
    "create.person": "Creat Person",
    "work.cell": "Work Cell",
    "bay.no": "Bay NO",
    list: list$1,
    "this.week": "This Week",
    "this.month": "This Month",
    "problem.type": "Problem Type",
    "average.duration": "Average duration",
    "week.on.month.ratio": "Week on month ratio",
    unresolved: unresolved$1,
    processing: processing$1,
    processed: processed$1,
    reviewed: reviewed$1,
    upgraded: upgraded$1,
    min: min$1,
    owners: owners$1,
    description: description$1,
    "occurred.time": "Occurred Time",
    severity: severity$1,
    station: station$1,
    "root.course": "Root Course",
    detail: detail$1,
    pic: pic$1,
    message: message$1,
    task: task$1,
    unread: unread$1,
    "set.all.read": "Set All Read",
    view: view$1,
    technician: technician$1,
    engineer: engineer$1,
    shift: shift$1,
    tag: tag$1,
    author: author$1,
    "publish.date": "Publish Date",
    "view.count": "View Count",
    reference: reference$1,
    "batch.delete": "Batch Delete",
    name: name$1,
    "parent.catalog": "Parent Catalog",
    "processing.information": "Processing Information",
    "issue.type": "Issue Type",
    upload: upload$1,
    solution: solution$1,
    action: action$1,
    "workflow.info": "Workflow Info",
    "complete.time": "Complete Time",
    "handle.time": "Handle Time",
    "approval.comment": "Approval Comments",
    "link.name": "Link Name",
    process: process$1,
    model: model$1,
    "problem.description": "Problem Description",
    "process.name": "Process Name",
    "station.name": "Station Name",
    "operation.success": "Operation Success",
    "operation.error": "Operation Error",
    summary: summary$1,
    "return.approve": "Return Approve",
    approve: approve$1,
    workflow: workflow$1,
    print: print$1,
    NTID: NTID$1,
    "feedback.time": "Feedback Time",
    proposal: proposal$1,
    satisfaction: satisfaction$1,
    sector: sector$1,
    "record.time": "Record Time",
    attachment: attachment$1,
    download: download$1,
    "file.name": "File Name",
    "shift.name": "Shift Name",
    back: back$1,
    total: total$1,
    items: items$1,
    "guangzhou.huangpu": "GuangZhou（HuangPu Factory）",
    "no.menu.permission": "No Menu Permission",
    "login.fail": "Login Fail",
    "form.required": "is required",
    "download.template": "Download Template",
    "add.parent.node": "Add Parent Node",
    "add.sub.node": "Add Sub Node",
    icon: icon$1,
    order: order$1,
    comments: comments$1,
    "items.selected": "Items Selected",
    release: release$1,
    "please.input": "Please Input.....",
    edit: edit$1,
    "no.results.found": "No results found",
    "name.is.repeat": "Name Is Repeat",
    "can.not.delete.referenced.data": "Can Not Delete Referenced Data",
    "can.not.edit.referenced.data": "Can Not Edit Referenced Data",
    "email.download": "Email Download",
    "user.ntid": "User NTID",
    email: email$1,
    invalid: invalid$1,
    reminder: reminder$1,
    "home.reminder": "You do not have access to other modules. You can contact the administrator to open.",
    "user.info": "User Info",
    "user.name": "User Name",
    user: user$1,
    phone: phone$1,
    roles: roles$1,
    "upload.person": "Upload Person",
    code: code$1,
    "parent.name": "Parent Name",
    "jpg.png": "You can only upload JPG/PNG image",
    "template.invalid": "Template Invalid",
    "enable.or.not": "Enable Or Not",
    title: title$1,
    "thumbs.up": "Thumbs Up",
    comment: comment$1,
    read: read$1,
    "reply.time": "Reply Time",
    "reply.content": "Reply Content",
    type: type$1,
    "min.value": "Min Value",
    "max.value": "Max Value",
    unit: unit$1,
    "preview.value": "Preview Value",
    remark: remark$1,
    copy: copy$1,
    "template.name": "Template Name",
    "config.name": "Config Name",
    "code.NO": "Code NO",
    "parameter.config": "Parameter Config",
    "parameter.count": "Parameter Count",
    plant: plant$1,
    "user.guide": "User Guide",
    "login.name": "Login Name",
    "psd.name.invalid": "Password or Login Name Can Not Be Null",
    host: host$1,
    port: port$1,
    kind: kind$1,
    region: region$1,
    stream: stream$1,
    endPoint: endPoint$1,
    broker: broker$1,
    queue: queue$1,
    equipment: equipment$1,
    db: db$1,
    run: run$1,
    check: check$1,
    "select.no.data": "No Data Selected",
    "psd.name.incorrect": "Username Or Password Is Incorrect",
    left: left$1,
    top: top$1,
    bay: bay$1,
    "equipment.type": "Equipment Type",
    "background.picture": "Background Picture",
    menu: menu$1,
    api: api$1,
    "common.approval.processes": "Common Approval Processes",
    "permission.application": "Permission Application",
    "secondary.development.application": "Secondary Development Application",
    "release.application": "Release Application",
    "installation.application": "Installation Application",
    "home.announcement": "Announcement",
    "type.number": "Number",
    "type.string": "String",
    "is.show": "Show",
    "is.show.track": "Show Track",
    "check.man.count": "Check Man Count",
    "no.upload.file": "Please Upload The File",
    "mobile.app.url": "Mobile App Url",
    "mobile.app.name": "Mobile App Name",
    "mobile.app.zh.name": "Mobile App Chinese Name",
    "mobile.app.bg": "Mobile App Cover",
    "mobile.app.brief": "Mobile App Brief",
    "mobile.app": "Mobile App",
    "mobile.end": "Phone",
    "machine.direction": "Direction",
    "machine.direction.left": "Right To Left",
    "machine.direction.right": "Left To Right",
    "user.img": "User Image",
    "old.password": "Old Password",
    "web.app.path": "Web App Path",
    "web.app.Router": "Web App Router",
    "web.app.iframe": "Iframe App",
    service: service$1,
    "email.or.ntid": "Email/NTID",
    project: project$1,
    cycle: cycle$1,
    reassign: reassign$1,
    "is.drafted": "Is Drafted",
    "is.public": "Is Publish",
    "comment.time": "Comment Time",
    "comment.content": "Comment Content",
    topic: topic$1,
    "please.login.and.reload.page": "Please login and reload page",
    "work.cell.is.repeat": "WorkCell Is Repeat",
    "very.satisfied": "Very Satisfied",
    satisfied: satisfied$1,
    ordinary: ordinary$1,
    dissatisfied: dissatisfied$1,
    "very.dissatisfied": "Very Dissatisfied",
    "workflow.no": "Workflow No.",
    "workflow.type": "Workflow Type",
    reason: reason$1,
    approver: approver$1,
    "approved.time": "Approved Time",
    opinion: opinion$1,
    agree: agree$1,
    reject: reject$1,
    "function.workcell": "Function/Workcell",
    "apply.date": "Apply Date",
    "form.no.opinion": "Please Input Opinion",
    "sme.invalid": "SME Is Invalid",
    application: application$1,
    "application.description": "Application Description",
    benefit: benefit$1,
    confirmed: confirmed$1,
    unconfirmed: unconfirmed$1,
    approving: approving$1,
    "git.address": "Git Address",
    "release.date": "Release Date",
    "version.no": "Version NO",
    "release.content": "Release Content",
    "home.web": "Web",
    "home.phone": "Phone",
    "install.purpose": "Install Purpose",
    "home.jb.permission": "Jabil Bus Permission Application",
    "home.jb.second.dev": "Jabil Bus Secondary Development Application",
    "home.jb.release": "Jabil Bus Release Application",
    "home.ja.install": "JAgent Installation Application",
    "home.jb.service": "Jabil Bus Service Application",
    "home.tesla.visit.permission": "Tesla Sector Visiting Pre-Approval System",
    "home.workflow.status.processing": "Processing",
    "home.workflow.status.processed": "Processed",
    "home.workflow.status.finish": "Finish",
    "no.mobile": "Please browse the website on a computer",
    note: note$1,
    "tesla.workflow.note": "The request must be submitted at least 4 hours ahead the visit.",
    "submitter.information": "Submitter Information",
    "reception.information": "Reception Information",
    "approver.information": "Approver Information",
    "visitor.information": "Visitor Information",
    "visitor.type": "Visitor Type",
    "visiting.time": "Visiting Time",
    company: company$1,
    "home.tesla.workflow.title": "Title",
    "home.tesla.workflow.purpose": "Purpose",
    refresh: refresh$1,
    Upgraded: Upgraded$1,
    NotProcessed: NotProcessed$1,
    Processing: Processing$1,
    Processed: Processed$1,
    Approving: Approving$1,
    Closed: Closed$1,
    Low: Low$1,
    Normal: Normal$1,
    High: High$1,
    Urgent: Urgent$1,
    Immediately: Immediately$1,
    All: All$1,
    Close: Close$1,
    Running: Running$1,
    RUSH_TO_REPAIR: RUSH_TO_REPAIR$1,
    ADD_PERSON: ADD_PERSON$1,
    NPI: NPI$1,
    DOWNTIME: DOWNTIME$1,
    POWER_CUT: POWER_CUT$1,
    RAINSTORM: RAINSTORM$1,
    UNDERMANNED: UNDERMANNED$1,
    WaitConfirm: WaitConfirm$1,
    HasConfirm: HasConfirm$1,
    WaitInterview: WaitInterview$1,
    HasInterview: HasInterview$1,
    "tesla.ct.monitoring": "Monitoring",
    "tesla.ct.shift": "Shift",
    "tesla.ct.last.shift": "Last Shift Situation",
    "tesla.ct.customer": "Customer",
    "tesla.ct.select.customer": "Select Customer",
    "tesla.ct.bay": "Bay",
    "tesla.ct.select.bay": "Select Bay",
    "tesla.ct.model": "Model",
    "tesla.ct.select.model": "Select Model",
    "tesla.ct.product": "Product",
    "tesla.ct.product.family": "Product Family",
    "tesla.ct.model.No": "ModelNo",
    "tesla.ct.bay.No": "BayNo",
    "tesla.ct.route.text": "RouteText",
    "tesla.ct.date&time": "Date&Time",
    "tesla.ct.accumulate.today.output": "Accumulate Today Output",
    "tesla.ct.IE.UPH": "IE UPH",
    "tesla.ct.theoretical.UPH": "Theoretical UPH",
    "tesla.ct.actual.UPH": "Actual UPH",
    "tesla.ct.fail.qty": "Fail Qty",
    "tesla.ct.root.course": "Root Course",
    "tesla.ct.action": "Action",
    "tesla.ct.occurred.time": "Occurred Time",
    "tesla.ct.PIC": "PIC",
    "tesla.ct.history": "History",
    "tesla.ct.progress": "Progress",
    "tesla.ct.downtime": "Downtime",
    "tesla.ct.what.happened": "What Happened",
    "tesla.ct.effective.output": "Effective Output",
    "tesla.ct.has.a.serious.delay.please.check.it": "Has a serious delay. please check it",
    "tesla.ct.sending.message": "Send Message",
    "tesla.ct.input.root.course": "Input RootCourse，Press Enter Confirm",
    "tesla.ct.input.action": "Input Action，Press Enter Confirm",
    "tesla.ct.select.PIC": "Select PIC",
    "tesla.ct.delay": "Delay",
    "tesla.ct.info": "Info",
    "tesla.ct.last.repair.records": "The Last 10 repair records",
    "tesla.ct.report": "Report",
    "tesla.ct.actual.output": "Actual Output",
    "tesla.ct.theoretical.output": "Theoretical Output",
    "tesla.ct.list": "List",
    "tesla.ct.graph": "Graph",
    "tesla.ct.utilization.rate": "Utilization Rate",
    "tesla.ct.theoretical.CT": "Theoretical CT",
    "tesla.ct.actual.CT": "Actual CT",
    "tesla.ct.save.as.image": "Save as image",
    "tesla.ct.configuration": "Configuration",
    "tesla.ct.alert.configuration": "Alert Configuration",
    "tesla.ct.condition": "Condition",
    "tesla.ct.percentage.of.errors": "Percentage Of Alert",
    "tesla.ct.alert.to": "Alert To",
    "tesla.ct.select.alert.to": "Select Alert To",
    "tesla.ct.error.when": "Alert When",
    "tesla.ct.warning.when": "Warning When",
    "tesla.ct.information": "Information",
    "tesla.ct.shift.name": "Shift Name",
    "tesla.ct.input.shift.name": "Input Shift Name",
    "tesla.ct.input.sort": "Input Sort",
    "tesla.ct.input.IE.UPH": "Input IE UPH",
    "tesla.ct.select.sn": "Name",
    "tesla.ct.record": "Record",
    "tesla.ct.message.send": "Message send!",
    "tesla.ct.message.send.error": "Message send fail!",
    "tesla.ct.select.station": "Station",
    "tesla.ct.effective.time.setting": "Effective Time Setting",
    "tesla.ct.effective.time": "Effective Time",
    "tesla.ct.effective.date": "Effective date",
    "tesla.ct.effective.start.date": "Effective Start Date",
    "tesla.ct.effective.end.date": "Effective End Date",
    "tesla.ct.effective.working.hours": "Day effective hours",
    "tesla.ct.shift.time": "Shift Time",
    "tesla.ct.delay.SN": "Delay SN",
    "tesla.ct.SN.status": "SN Status",
    "menu.temperature.collection": "Temperature Collection",
    "menu.temperature.monitoring": "Monitoring",
    "menu.temperature.analysis": "Analysis",
    "menu.temperature.configuration": "Configuration",
    "menu.remote.service": "Remote Service",
    "menu.remote.sector": "Sector",
    "menu.remote.machine": "Machine",
    "menu.remote.user": "User",
    "menu.remote.status": "Status",
    "menu.remote.control": "Control",
    "menu.tesla.ct.tracking": "Tesla Output&CT Tracking",
    "menu.tesla.ct.monitoring": "Monitoring",
    "menu.tesla.ct.report": "Report",
    "menu.tesla.ct.configuration": "Configuration",
    "menu.tesla.ct.information": "Information",
    "menu.MFG.test": "MFG",
    "menu.MFG.test.announcement": "Multimedia Interactive Platform",
    "menu.MFG.test.course.ware": "Course Ware",
    "menu.MFG.test.learning": "Learning",
    "menu.MFG.test.question.bank": "Question Bank",
    "menu.MFG.test.test": "Test",
    "menu.system": "System",
    "menu.system.user": "User",
    "menu.system.role": "Role",
    "menu.system.organization": "Organization",
    "menu.system.menu": "Menu",
    "menu.system.audit.log": "Audit Log",
    "menu.system.ES.log": "ES Log",
    "menu.system.data.dictionary": "Data Dictionary",
    "menu.common.home": "Home",
    "mfg.training.announce.title": "Announce Title",
    "mfg.training.announce.category": "Announce Category",
    "mfg.training.announce.content": "Announce Content",
    "mfg.training.course.ware.name": "Course Ware Name",
    "mfg.training.course.ware.category": "Course Ware Category",
    "mfg.training.course.ware.category.manage": "Manage Course Ware Category",
    "mfg.training.course.ware.open": "IsOpen",
    "mfg.training.course.ware.select": "Select Course Ware",
    "mfg.training.course.ware.support.file": "Support for PDF or MP4",
    "mfg.training.course.ware.parent.category": "Parent Category",
    "mfg.training.learning.title": "Learning Title",
    "mfg.training.learning.people.number": "Need Learning People Number",
    "mfg.training.learning.associated.test": "Associated Test",
    "mfg.training.learning.time": "Learning Time",
    "mfg.training.learning.online.offline": "Online/Offline",
    "mfg.training.learning.test": "Need Test",
    "mfg.training.learning.select.test": "Select Test",
    "mfg.training.learning.test.count": "Test Count",
    "mfg.training.learning.qr": "Qr Code Check",
    "mfg.training.learning.introduce": "Introduce",
    "mfg.training.learning.upload.ntid": "Upload Employee NTID",
    "mfg.training.test.title": "Test Title",
    "mfg.training.test.people": "Test People Count",
    "mfg.training.test.category": "Test Category",
    "mfg.training.test.permission": "Test Permission",
    "mfg.training.test.select.question.bank": "Select Question Bank",
    "mfg.training.test.question.out.order": "Question Out Order",
    "mfg.training.test.option.out.order": "Option Out Order",
    "mfg.training.test.multi.option": "Multi Option",
    "mfg.training.test.all.score": "All Score",
    "mfg.training.test.part.score": "Part Score",
    "mfg.training.test.pass.score": "Pass Score",
    "mfg.training.test.retest.count": "Retest Count",
    "mfg.training.test.time": "Test Time",
    "mfg.training.test.duration": "Test Duration",
    "mfg.training.test.show.result": "Show Result",
    "mfg.training.test.score": "Score",
    "mfg.training.test.rank": "Rank",
    "mfg.training.test.over.time": "Over Time Auto Commit",
    "mfg.training.test.introduce": "Test Introduce",
    "mfg.training.test.single.num": "single Choice Question Num",
    "mfg.training.test.multi.num": "Multi Choice Question Num",
    "mfg.training.test.true.false": "True Or False Num",
    "mfg.training.test.multi.all.score": "Multi All Score",
    "mfg.training.test.multi.part.score": "Multi Part Score",
    "mfg.training.test.single.score": "Single Score",
    "mfg.training.test.true.false.score": "Judge Score",
    "mfg.training.question.bank.title": "Question Bank Title",
    "mfg.training.question.bank.category": "Question Bank Category",
    "mfg.training.question.bank.type": "Question Bank Type",
    "mfg.training.question.bank.test": "Test Question Bank",
    "mfg.training.question.bank.practice": "Practice Question Bank",
    "mfg.training.question.bank.permission": "Question Bank Permission",
    "mfg.training.question.bank.question": "View Question",
    "mfg.training.question.bank.out.order": "Out Order Question",
    "mfg.training.question.bank.upload": "Upload Question Bank",
    "mfg.training.question.title": "Question Title",
    "mfg.training.question.type": "Question Type",
    "mfg.training.question.type.judge": "Judge",
    "mfg.training.question.type.single": "Single",
    "mfg.training.question.type.multi": "Multi",
    "mfg.training.question.content": "Question Content",
    "mfg.training.question.option.manage": "Option Manage",
    "mfg.training.question.new.option": "Edit Option",
    "mfg.training.question.answer": "Answer",
    "mfg.training.question.option": "Option",
    "mfg.training.question.explain": "Question Explain",
    "mfg.training.add.category": "Add Category",
    "mfg.training.category.manage": "Category Manage",
    "mfg.training.announce.picture": "Announce Picture",
    "mfg.training.prevent.cheat": "Prevent Cheat",
    "mfg.training.option.warning": "Option is not valid!",
    "mfg.training.question.option1": "Please Add Option",
    "mfg.training.question.option2": "Two Options Are Required",
    "mfg.training.question.option3": "Required One True Option",
    "mfg.training.question.option4": "Required More Than Three Options",
    "mfg.training.question.option5": "Required One True Option",
    "mfg.training.question.option6": "Required More Than One True Option",
    "mfg.training.course.ware.learn.duration": "Course Ware Learn Duration",
    "mfg.training.course.ware.learn.people": "Course Ware Learn People",
    "mfg.training.learn.course.statistics": "Learning Course Statistics",
    "mfg.training.test.course.statistics": "Examination Course Statistics",
    "mfg.training.test.detail": "Test Detail",
    "mfg.care.care.title": "Care Title",
    "mfg.care.care.category": "Care Category",
    "mfg.care.care.time": "Active Time",
    "mfg.care.sign.employee": "Sign Employee",
    "mfg.care.sign.qr": "Sign QR",
    "mfg.care.sign.time": "Sign Time",
    "mfg.care.employee.feedback": "Feedback",
    "mfg.care.organizer": "Organizer",
    "mfg.care.content": "Content",
    "mfg.care.interview.title": "Interview Title",
    "mfg.care.interview.category": "Interview Category",
    "mfg.care.interview.supervisor": "Interview Supervisor/Manager",
    "mfg.care.interview.employee": "Interview Employee",
    "mfg.care.interview.time": "Interview Time",
    "mfg.care.site.employee": "Site Employee",
    "mfg.care.abnormal.feedback": "Abnormal Feedback",
    "mfg.care.interview.content": "Interview Content:",
    "mfg.care.feedback.content": "Feedback Content",
    "mfg.care.feedback.title": "Feedback Title",
    "mfg.care.interview.status": "Interview Status",
    "mfg.care.feedback.status": "Feedback Status",
    "mfg.care.normal.feedback": "Normal Feedback",
    "mfg.care.finish.interview": "Finish Interview",
    "mfg.video.max.size": "Video can upload max size 2g",
    "mfg.email.template": "Email Upload Template",
    "mfg.question.bank.template": "Question Bank Upload Template",
    "mfg.file.20mb": "Image must smaller than 20MB",
    "mfg.active.name": "name",
    "mfg.participated.in.activities": "Participated In Activities",
    "mfg.activity.initiator": "Activity Initiator",
    "mfg.satisfaction.assessment": "Satisfaction Assessment",
    "mfg.activity.time": "Activity Time",
    "mfg.activity.type": "Activity Type",
    "mfg.activity.summary": "Activity Summary",
    "mfg.employee": "Employee",
    "mfg.teml": "Teml",
    "mfg.satisfaction": "Satisfaction",
    "mfg.month": "Month",
    "mfg.activity.count": "Activity Count",
    "mfg.accumulated.participate.person": "Accumulated Participate Person",
    "mfg.care.pic": "PIC",
    "mfg.learn.detail": "Learn Detail",
    "mfg.guide.content": "Guide Content",
    "mfg.guide.type": "Guide Type",
    "mfg.release.person": "Release Person",
    "mfg.to.do": "To Do",
    "mfg.save.to.do": "Save To Do",
    "mfg.to.do.list": "To Do List",
    "mfg.content": "Content",
    "mfg.guide.language": "Guide Language",
    "mfg.save.guide.language.template": "Save Guide Language Template",
    "mfg.on.job.days": "On Job Days",
    "mfg.show.title": "Show Title",
    "mfg.keyword": "Keyword",
    "mfg.attr": "Attribute",
    "mfg.question.title": "Question Title",
    "mfg.question.category": "Question Category",
    "mfg.resolvent": "Resolvent",
    "mfg.hot.search": "Hot Search",
    "mfg.topping": "Topping",
    "mfg.feedback": "Feedback",
    "mfg.consultation.content": "Consultation Content",
    "mfg.consultation.person": "Consultation Person",
    "mfg.consultation.time": "Consultation Time",
    "mfg.recovery.person": "Recovery Person",
    "mfg.recovery.time": "Recovery Time",
    "mfg.rating": "Rating",
    "mfg.publish.consultation.content": "Published Consultation Content",
    "mfg.interview.number": "Number of Interview",
    "mfg.active": "Active",
    "mfg.active.detail": "Active Detail",
    "mfg.active.summary": "Active Summary",
    "mfg.no.permission": "No Permission",
    "test.valid.not.resolved": "Current not resolved case",
    "test.valid.cumulative.upgrade": "Cumulative upgrade case",
    "test.valid.resolved.today": "Resolved today case",
    "test.valid.generated.today": "Generated today case",
    "test.valid.in.settlement": "In settlement case",
    "test.valid.problem.trend": "Problem Trend",
    "test.valid.accumulated.problems": "Accumulated problems (half a year)",
    "test.valid.problems.this.week": "Problems arise this week",
    "test.valid.problems.this.month": "Problems arise this month",
    "test.valid.problems.distribution": "Distribution of problems (half a year)",
    "test.valid.solving.duration": "Problem solving duration (week)",
    "test.valid.my.case": "My Case",
    "test.valid.max.processing.time": "Longest Time This Week",
    "test.valid.average.processing.time": "Average Time This Week",
    "test.valid.shortest.processing.time": "Shortest Time This Week",
    "test.valid.number.completed": "Completed Number this Week",
    "test.valid.my.case.list": "My Case list",
    "test.valid.event": "Event",
    "test.valid.assign.to": "Assign to",
    "test.valid.occurrence time": "Occurrence time",
    "test.valid.speed": "Speed",
    "test.valid.processing.time": "Processing Time",
    "test.valid.bay.data": "Bay Data",
    "test.valid.station.data": "Station Data",
    "test.valid.staff.data": "Staff Data",
    "test.valid.shift.data": "Shift Data",
    "test.valid.add.bay": "Add Bay",
    "test.valid.edit.bay": "Edit Bay",
    "test.valid.add.station": "Add Station",
    "test.valid.edit.station": "Edit Station",
    "test.valid.add.shift": "Add Shift",
    "test.valid.edit.shift": "Edit Shift",
    "test.valid.add.technician": "Add Technician",
    "test.valid.edit.technician": "Edit Technician",
    "test.valid.subject": "Subject",
    "test.valid.show.catalog": "Show In Catalog",
    "test.valid.cook.book.tag.edit": "Cookbook Tag Edit",
    "test.valid.cook.book.tag.add": "Cookbook Tag Add",
    "test.valid.test.station.problem": "Test Station Problem",
    "test.valid.upload.type": "jpg/png/MP4 file，size not more than 50M",
    "test.valid.isSync.cookbook": "IsSync Cookbook",
    "test.valid.tag.num": "Tag:Max 5 Tags",
    "test.valid.case.file.size": "File must smaller than 50MB!",
    "test.valid.case.file.count": "Max three images and one video!",
    "test.valid.processing.files": "Processing Files",
    "test.valid.solution.files": "Solution Files",
    "test.valid.tag": "Tag",
    "test.valid.test.station": "Test Station",
    "test.valid.case.no": "Case No",
    "test.valid.host.name": "Host Name",
    "system.menu.route.name": "Route Name",
    "system.menu.route.path": "Route Path",
    "system.menu.is.hidden": "Is Hidden",
    "system.menu.parent.id": "Parent Id",
    "system.menu.file.path": "File Path",
    "system.menu.display.name": "Display Name",
    "system.menu.keep.alive": "Keep Alive",
    "system.menu.close.tab": "Close Tab",
    "system.menu.parameter.type": "Parameter Type",
    "system.menu.parameter.key": "Parameter Key",
    "system.menu.parameter.value": "Parameter Value",
    "system.menu.button.name": "Button Name",
    "escs.qos": "QOS",
    "escs.code.execution.results": "Code Execution Results",
    "escs.machine.execution.results": "Machine Execution Results",
    "escs.execution.code": "Execution Code",
    "escs.site": "Site",
    "escs.branch": "Branch",
    TestValidationManagement: TestValidationManagement$1,
    "TestValidationManagement.Dashboard": "Dashboard",
    "TestValidationManagement.MyCase": "My Case",
    "TestValidationManagement.CaseCenter": "Case Center",
    "TestValidationManagement.MessageCenter": "Message Center",
    "TestValidationManagement.MainData": "Main Data",
    "TestValidationManagement.CookbookList": "Cookbook List",
    "TestValidationManagement.CookbookLabel": "Cookbook Label",
    TemperatureManagement: TemperatureManagement$1,
    "TemperatureManagement.Monitoring": "Monitoring",
    "TemperatureManagement.Analysis": "Analysis",
    "TemperatureManagement.Configuration": "Configuration",
    RemoteControl: RemoteControl$1,
    "RemoteControl.Sector": "Sector",
    "RemoteControl.Machine": "Machine",
    "RemoteControl.User": "User",
    "RemoteControl.Status": "Status",
    "RemoteControl.MachineControl": "Machine Control",
    TeslaCTTrackingManagement: TeslaCTTrackingManagement$1,
    "TeslaCTTrackingManagement.Monitor": "Monitoring",
    "TeslaCTTrackingManagement.Report": "Report",
    "TeslaCTTrackingManagement.Configuration": "Configuration",
    "TeslaCTTrackingManagement.Information": "Information",
    TeslaAutoBuildPlan: TeslaAutoBuildPlan$1,
    TeslaOrder: TeslaOrder$1,
    TeslaPlan: TeslaPlan$1,
    TeslaPlanMonitoring: TeslaPlanMonitoring$1,
    MFGPeopleManagement: MFGPeopleManagement$1,
    MFGDashboard: MFGDashboard$1,
    MFGTranning: MFGTranning$1,
    "MFGTranning.Announcement": "Multimedia Interactive Platform",
    "MFGTranning.CourseWare": "Course Ware",
    "MFGTranning.Learning": "Learning",
    "MFGTranning.QuestionBank": "Question Bank",
    "MFGTranning.Test": "Exam",
    "MFGTranning.Statistics": "Statistics",
    MFGEmployeeCare: MFGEmployeeCare$1,
    "MFGEmployeeCare.CareRecord": "Care Record",
    "MFGEmployeeCare.Interview": "Interview",
    "MFGEmployeeCare.InterviewReport": "Report",
    "MFGEmployeeCare.Feedback": "Feedback",
    "MFGEmployeeCare.employee.abnormal": "Proactive Care",
    "MFGEmployeeCare.active.summary": "Active Summary",
    "MFGEmployeeCare.active.detail": "Active Detail",
    MFGEBuddy: MFGEBuddy$1,
    "MFGEBuddy.new.guide": "New Guidance",
    "MFGEBuddy.frequently.question": "Frequently Question",
    "MFGEBuddy.consultation": "Consultation",
    "MFGConfig.config": "Configuration",
    "MFGConfig.permission": "Permission",
    "MFGConfig.employee": "Employee",
    EPromotion: EPromotion$1,
    "ePromotion.hc": "Hc Vacancy",
    "ePromotion.skill": "Skill Vacancy",
    MFGPerformance: MFGPerformance$1,
    "MFGPerformance.PWT.calculation": "PWT Bonus",
    "MFGPerformance.Leader.PWT.calculation": "Line Leader Bonus",
    "MFGPerformance.PWT.calculation.result": "Bonus Calculation Result",
    "MFGPerformance.PWT.team.manage": "DPE Team Management",
    "MFGPerformance.PWT.super.DL.bonus": "Super DL Bonus",
    "MFGPerformance.PWT.super.LL.bonus": "Super LL Bonus",
    WIKIHome: WIKIHome$1,
    WIKIBackground: WIKIBackground$1,
    WIKIDashboard: WIKIDashboard$1,
    WIKITopic: WIKITopic$1,
    WIKIMyTopic: WIKIMyTopic$1,
    WIKIComment: WIKIComment$1,
    WIKIMyComment: WIKIMyComment$1,
    System: System$1,
    Users: Users$1,
    Roles: Roles$1,
    BatchRoles: BatchRoles$1,
    Menus: Menus$1,
    AuditLog: AuditLog$1,
    ES: ES$1,
    DataDictionary: DataDictionary$1,
    SystemAnnouncement: SystemAnnouncement$1,
    ESCSManagement: ESCSManagement$1,
    "ESCSManagement.Index": "Digital Twins",
    Home: Home$1,
    MesStreamConfig: MesStreamConfig$1,
    MesStreamWip: MesStreamWip$1,
    WorkflowConfig: WorkflowConfig$1,
    WorkflowTeslaConfig: WorkflowTeslaConfig$1,
    WorkflowPermissionConfig: WorkflowPermissionConfig$1,
    Sediot: Sediot$1,
    SediotCycleTime: SediotCycleTime$1,
    SediotEmailConfig: SediotEmailConfig$1,
    SediotProcessReport: SediotProcessReport$1,
    SediotUtilizationReport: SediotUtilizationReport$1,
    SediotPMAlert: SediotPMAlert$1,
    SediotLessonLearn: SediotLessonLearn$1,
    SediotLayout: SediotLayout$1,
    OC: OC$1,
    OCConfig: OCConfig$1,
    OCSummary: OCSummary$1,
    RealTime: RealTime$1,
    DataTraceability: DataTraceability$1,
    DataTraceabilityHomePage: DataTraceabilityHomePage$1,
    DataTraceabilityPortal: DataTraceabilityPortal$1,
    HOSAppWidget: HOSAppWidget$1,
    HOSRole: HOSRole$1,
    HOSTemplate: HOSTemplate$1,
    HOSRoleTemplateConfig: HOSRoleTemplateConfig$1,
    HMCWebsite: HMCWebsite$1,
    ReportCenter: ReportCenter$1,
    "user.operate": "User Operate",
    "password.confirm": "password(confirm)",
    "password.is.inconsistent": "Password Is Inconsistent",
    "role.name": "Role Name",
    "role.operate": "Role Operate",
    "is.default": "Is Default",
    "name.code": "Name/Code",
    "data.dictionary.operate": "Data Dictionary Operate",
    wiki: wiki$1,
    "release.topic": "Release Topic",
    "topic.comment.trend": "Topic/Comment Trend",
    "view.reply": "View Reply",
    "is.visible": "Visible",
    "topic.title": "Topic Title",
    "topic.content": "Topic Content",
    "add.forum": "Add Forum",
    draft: draft$1,
    "half.year": "Half Year",
    "today.topic.comment": "Today Topic/Comment",
    "count.comments": "Comments",
    "forum.manage": "Forum Manage",
    "category.manage": "Category Manage",
    "add.category": "Add Category",
    "reply.to": "Reply To",
    reply: reply$1,
    pcn: pcn$1,
    primeng: primeng$1,
    PCNConfig: PCNConfig$1,
    PCNCodeParameter: PCNCodeParameter$1,
    PCNCodeStage: PCNCodeStage$1,
    PCNCodeTemplate: PCNCodeTemplate$1,
    PCNMasterTemplate: PCNMasterTemplate$1,
    PCNCodeBuild: PCNCodeBuild$1,
    PCNMachine: PCNMachine$1,
    PCNMachineInfo: PCNMachineInfo$1,
    PCNWorkflow: PCNWorkflow$1,
    PCNMachineModel: PCNMachineModel$1,
    PCNMachineChange: PCNMachineChange$1,
    PCNWorkcellCodeConfig: PCNWorkcellCodeConfig$1,
    PCNDefaultApprover: PCNDefaultApprover$1,
    PCNNPI: PCNNPI$1,
    PCNSOP: PCNSOP$1,
    PCNApplyActionConfig: PCNApplyActionConfig$1,
    PCNEscalation: PCNEscalation$1,
    "sort.unique": "Sort Is Not Unique",
    config: config$1,
    "machine.config": "Machine Config",
    "insert.param": "Insert Param",
    preview: preview$1,
    "add.machine.config": "Add Machine Config",
    "manage.machine.config": "Manage Machine Config",
    "machine.name": "Machine Name",
    "stage.name": "Stage Name",
    "master.template.name": "Master Template Name",
    "master.template": "Master Template",
    "sub.stage.name": "Sub Stage Name",
    "param.name": "Param Name",
    value: value$1,
    "copy.template": "Copy Template",
    synchronize: synchronize$1,
    handle: handle$1,
    initiator: initiator$1,
    "affected.models": "Affected Models",
    "effective.date": "Effective Date",
    "actual.implementation.date": "Actual Implementation Date",
    "synchronize.data.success": "Synchronize Data Success",
    "synchronize.data.fail": "Synchronize Data Fail",
    applicant: applicant$1,
    "base.info": "Base Info",
    "approval.process": "Approval Process",
    "complete.review": "Complete Review",
    "process.log": "Process Log",
    "approved.by": "Approved By",
    "approval.time": "Approval Time",
    "approval.opinions": "Approval Opinions",
    "workflow.reject.pcn": "Reject",
    "workflow.agree.pcn": "Agree",
    originator: originator$1,
    MasterDataManagement: MasterDataManagement$1,
    MasterDataHomepage: MasterDataHomepage$1,
    MasterDataBay: MasterDataBay$1,
    MasterDataEquipment: MasterDataEquipment$1,
    MasterDataEmployee: MasterDataEmployee$1,
    MasterDataHUAIDCreate: MasterDataHUAIDCreate$1,
    MasterDataEmployeeQuery: MasterDataEmployeeQuery$1,
    MasterDataOperationConfiguration: MasterDataOperationConfiguration$1,
    MasterDataFunction: MasterDataFunction$1,
    MasterDataOrganization: MasterDataOrganization$1,
    MasterDataLocation: MasterDataLocation$1,
    MasterDataProductionArea: MasterDataProductionArea$1,
    "MasterData.UpdateRecordQuery": "Update Record Query",
    "MasterData.CostCenter": "Cost Center",
    DataAssetMap: DataAssetMap$1,
    DataAssetHomepage: DataAssetHomepage$1,
    DataAssetSQLSearch: DataAssetSQLSearch$1,
    DataAssetMasterDataManagement: DataAssetMasterDataManagement$1,
    DataAssetTopicDomainDefinition: DataAssetTopicDomainDefinition$1,
    DataAssetMetadataManagement: DataAssetMetadataManagement$1,
    DataAssetDataApi: DataAssetDataApi$1,
    DataAssetDataAuthorization: DataAssetDataAuthorization$1,
    DataAssetDataSource: DataAssetDataSource$1,
    DataAssetDataTable: DataAssetDataTable$1,
    DataAssetDataAccessLog: DataAssetDataAccessLog$1,
    DataAssetDataAssetQuery: DataAssetDataAssetQuery$1,
    DataAssetDataAssetCatalog: DataAssetDataAssetCatalog$1,
    DataAssetPermissionApplicationRecord: DataAssetPermissionApplicationRecord$1,
    DataAssetUserPermissionApplicationRecord: DataAssetUserPermissionApplicationRecord$1,
    DataAssetAuthorizedData: DataAssetAuthorizedData$1,
    ThingsLinkerBroker: ThingsLinkerBroker$1,
    ThingsLinkerEquipment: ThingsLinkerEquipment$1,
    ThingsLinkerBooking: ThingsLinkerBooking$1,
    ThingsLinkerDispatchOrder: ThingsLinkerDispatchOrder$1,
    ThingsLinkerConfig: ThingsLinkerConfig$1,
    ThingsLinker: ThingsLinker$1,
    JIT: JIT,
    JiTBuildPlan: JiTBuildPlan$1,
    JITDeliveryBooking: JITDeliveryBooking$1,
    JITPullListKanban: JITPullListKanban$1,
    JITSIgnFor: JITSIgnFor$1,
    JITBuildPlanManagement: JITBuildPlanManagement$1,
    JITSubmitBuildPlan: JITSubmitBuildPlan$1,
    JITShortageList: JITShortageList$1,
    JITPullListApply: JITPullListApply$1,
    JITDemandReport: JITDemandReport$1,
    JITRemainingInventory: JITRemainingInventory$1,
    JITBaseData: JITBaseData$1,
    JITDummyBOM: JITDummyBOM$1,
    JITUPH: JITUPH$1,
    JITModelFrequency: JITModelFrequency$1,
    JITAGVLineInfo: JITAGVLineInfo$1,
    JITSPQ: JITSPQ$1,
    JITDeliveryLeadTime: JITDeliveryLeadTime$1,
    JITStorageLocation: JITStorageLocation$1,
    JITDeliveryTaskControl: JITDeliveryTaskControl$1,
    JITBuyerNameByPart: JITBuyerNameByPart$1,
    JITWarehouseManagement: JITWarehouseManagement$1,
    JITWarehouseDelivery: JITWarehouseDelivery$1,
    JITInventoryManagement: JITInventoryManagement$1,
    JITInventoryBin: JITInventoryBin$1,
    JITInventoryReport: JITInventoryReport$1,
    JITInventoryStoreConfig: JITInventoryStoreConfig$1,
    JITInventory: JITInventory$1,
    JITVendorAssignRate: JITVendorAssignRate$1,
    JITLockOpenPO: JITLockOpenPO$1,
    JITPDSRemindConfig: JITPDSRemindConfig$1,
    JITPDSSearch: JITPDSSearch$1,
    HuaDataLake: HuaDataLake$1,
    HUAJabilBus: HUAJabilBus$1,
    HUAIOTPlatform: HUAIOTPlatform$1,
    jit: jit$1,
    AOS: AOS$1,
    AOSCheckList: AOSCheckList$1,
    AOSAuditPlan: AOSAuditPlan$1,
    AOSTaskList: AOSTaskList$1,
    AOSApproval: AOSApproval,
    AOSNCList: AOSNCList$1,
    AOSConfig: AOSConfig$1,
    AOSAuditor: AOSAuditor$1,
    AOSAuditorTest: AOSAuditorTest$1,
    BMWIOT: BMWIOT$1,
    "BMWIOT.Dashboard": "Dashboard",
    "BMWIOT.Production": "ProductionAndTest",
    "BMWIOT.EquipmentAlarm": "EquipmentAlarm",
    "BMWIOT.ChangeInformation": "ChangeInformation",
    JMJagentMonitoring: JMJagentMonitoring$1,
    JMMonitoring: JMMonitoring$1,
    JMWarningLog: JMWarningLog$1,
    JMConfig: JMConfig$1,
    SMJagentMonitoring: SMJagentMonitoring$1,
    SMMonitoring: SMMonitoring$1,
    SMWarningLog: SMWarningLog$1,
    SMConfig: SMConfig$1
};

var translateVersion = "1.6";
var login = "登录";
var logoff = "退出";
var username = "用户名";
var password = "密码";
var reset = "重置";
var search = "查询";
var status = "状态";
var operation = "操作";
var input = "输入";
var alert = "报警";
var cancel = "取消";
var all = "全部";
var normal = "正常";
var warning = "报警";
var filter = "过滤";
var submit = "提交";
var date = "日期";
var data = "数据";
var customerBtn = "自定义";
var day = "日";
var No = "序号";
var add = "新增";
var modify = "修改";
var save = "保存";
var disabled = "无效";
var enabled = "有效";
var nodata = "暂无数据";
var time = "时间";
var close = "关闭";
var sort = "排序";
var s = "秒";
var m = "分";
var h = "时";
var category = "分类";
var yes = "是";
var no = "否";
var online = "线上";
var offline = "线下";
var permission = "权限";
var confirm = "确认信息";
var hour = "小时";
var list = "列表";
var unresolved = "未解决";
var processing = "处理中";
var processed = "已处理";
var reviewed = "待审核";
var upgraded = "已升级";
var min = "分";
var owners = "归属";
var description = "描述";
var severity = "严重度";
var station = "站点";
var detail = "详情";
var pic = "PIC";
var message = "信息";
var task = "任务";
var unread = "未读";
var view = "查看";
var technician = "技术员";
var engineer = "工程师";
var shift = "班次";
var tag = "标签";
var author = "作者";
var reference = "引用";
var name = "名称";
var upload = "上传";
var solution = "解决";
var action = "措施";
var process = "处理流程";
var model = "型号";
var summary = "概要";
var approve = "审批";
var workflow = "处理流程";
var print = "打印";
var NTID = "NTID";
var proposal = "建议";
var satisfaction = "满意度";
var sector = "部门";
var attachment = "附件";
var download = "下载";
var back = "返回";
var total = "共";
var items = "条";
var icon = "图标";
var order = "排序";
var comments = "备注";
var release = "发布";
var edit = "编辑";
var email = "邮箱";
var invalid = "无效";
var reminder = "温馨提示";
var user = "用户";
var phone = "电话";
var roles = "角色";
var code = "编码";
var title = "标题";
var comment = "评论";
var read = "阅读";
var type = "类型";
var unit = "单位";
var remark = "备注";
var copy = "复制";
var plant = "工厂";
var host = "主机";
var port = "端口";
var kind = "种类";
var region = "区域";
var stream = "流";
var endPoint = "终点";
var broker = "代理";
var queue = "队列";
var equipment = "设备";
var db = "数据库";
var run = "运行";
var check = "检测";
var left = "位置左";
var top = "位置高";
var bay = "产线";
var menu = "菜单";
var api = "接口";
var service = "服务";
var project = "项目";
var cycle = "周期";
var reassign = "指定他人回复";
var topic = "话题";
var reason = "原因";
var approver = "审批人";
var opinion = "意见";
var agree = "同意";
var reject = "拒绝";
var application = "应用系统";
var benefit = "利益";
var confirmed = "已确认";
var unconfirmed = "未确认";
var approving = "审批中";
var satisfied = "满意";
var ordinary = "一般";
var dissatisfied = "不满意";
var note = "提示";
var company = "公司";
var refresh = "刷新";
var Upgraded = "已升级";
var NotProcessed = "未解决";
var Processing = "处理中";
var Processed = "已处理";
var Approving = "待审核";
var Closed = "已关闭";
var Low = "低";
var Normal = "正常";
var High = "高";
var Urgent = "急";
var Immediately = "紧急";
var All = "全部";
var Close = "关闭";
var Running = "运行";
var RUSH_TO_REPAIR = "抢修";
var ADD_PERSON = "增加人手";
var NPI = "NPI";
var DOWNTIME = "故障";
var POWER_CUT = "断电";
var RAINSTORM = "暴雨";
var UNDERMANNED = "人员不足";
var WaitConfirm = "等待确认";
var HasConfirm = "无需面谈";
var WaitInterview = "等待面谈";
var HasInterview = "完成面谈";
var TestValidationManagement = "E-Ticket";
var TemperatureManagement = "温度管理";
var RemoteControl = "远程控制";
var TeslaCTTrackingManagement = "特斯拉产线监测";
var TeslaAutoBuildPlan = "自动生成计划";
var TeslaOrder = "订货";
var TeslaPlan = "计划";
var TeslaPlanMonitoring = "计划监测";
var MFGPeopleManagement = "MFG员工管理";
var MFGDashboard = "仪表盘";
var MFGTranning = "制造学院";
var MFGEmployeeCare = "员工关爱";
var MFGEBuddy = "E-Buddy";
var EPromotion = "人员晋升";
var MFGPerformance = "绩效";
var System = "系统";
var Users = "用户";
var Roles = "角色";
var BatchRoles = "批量角色";
var Menus = "菜单";
var AuditLog = "Audit日志";
var ES = "ES日志";
var DataDictionary = "数据字典";
var SystemAnnouncement = "公告管理";
var ESCSManagement = "数字孪生";
var Home = "首页";
var MesStreamConfig = "Mes配置";
var MesStreamWip = "Wip查询";
var WorkflowConfig = "流程配置";
var WorkflowTeslaConfig = "特斯拉配置";
var WorkflowPermissionConfig = "权限配置";
var WIKIHome = "WIKI首页";
var WIKIBackground = "后台管理";
var WIKIDashboard = "仪表板";
var WIKITopic = "话题管理";
var WIKIMyTopic = "我的话题";
var WIKIComment = "评论管理";
var WIKIMyComment = "我的评论";
var Sediot = "SolarEdge IOT";
var SediotCycleTime = "机器周期时间";
var SediotEmailConfig = "机器进程报告邮件配置";
var SediotProcessReport = "机器进程报告";
var SediotUtilizationReport = "机器利用率报告";
var SediotPMAlert = "机器保养";
var SediotLessonLearn = "机器学习";
var SediotLayout = "机器布局";
var OC = "一键转拉";
var OCConfig = "数据配置";
var OCSummary = "转拉概览";
var RealTime = "Real Time";
var DataTraceability = "数据溯源";
var DataTraceabilityHomePage = "主页面";
var DataTraceabilityPortal = "系统入口";
var HOSAppWidget = "App小组件";
var HOSRole = "角色管理";
var HOSTemplate = "模板管理";
var HOSRoleTemplateConfig = "角色模板配置";
var HMCWebsite = "HMC网站";
var ReportCenter = "报表中心";
var wiki = {
    forum: "板块",
    "forum.name": "板块名称",
    "subsidiary.forum": "所属板块",
    "topic.video.max.size": "上传视频最大10M"
};
var draft = "草稿";
var reply = "回复";
var pcn = {
    stage: "阶段",
    "parameter.name": "参数名称",
    "stage.name": "阶段名称",
    site: "地点",
    "material.group": "材料组",
    "external.material.group": "外部材料组",
    part: "部分",
    "supplier.part": "供应商部分",
    type: "类型",
    "type.desc": "类型描述",
    "make.buy": "制作/购买",
    "customer.name": "客户名称",
    "affected.models": "影响的型号",
    coordinator: "PCN协调员",
    "change.description": "改变描述",
    "check.man.count": "检查人数",
    "model.id": "模型ID",
    "machine.id": "机器ID",
    "code.type": "代码类型"
};
var primeng = {
    startsWith: "Starts with",
    contains: "Contains",
    notContains: "Not contains",
    endsWith: "Ends with",
    equals: "Equals",
    notEquals: "Not equals",
    noFilter: "No Filter",
    lt: "Less than",
    lte: "Less than or equal to",
    gt: "Greater than",
    gte: "Greater than or equal to",
    is: "Is",
    isNot: "Is not",
    before: "Before",
    after: "After",
    dateIs: "Date is",
    dateIsNot: "Date is not",
    dateBefore: "Date is before",
    dateAfter: "Date is after",
    clear: "清空",
    apply: "Apply",
    matchAll: "Match All",
    matchAny: "Match Any",
    addRule: "Add Rule",
    removeRule: "Remove Rule",
    accept: "Yes",
    reject: "No",
    choose: "Choose",
    upload: "Upload",
    cancel: "Cancel",
    dayNames: [
        "周日",
        "周一",
        "周二",
        "周三",
        "周四",
        "周五",
        "周六"
    ],
    dayNamesShort: [
        "周日",
        "周一",
        "周二",
        "周三",
        "周四",
        "周五",
        "周六"
    ],
    dayNamesMin: [
        "周日",
        "周一",
        "周二",
        "周三",
        "周四",
        "周五",
        "周六"
    ],
    monthNames: [
        "一月",
        "二月",
        "三月",
        "四月",
        "五月",
        "六月",
        "七月",
        "八月",
        "九月",
        "十月",
        "十一月",
        "十二月"
    ],
    monthNamesShort: [
        "一月",
        "二月",
        "三月",
        "四月",
        "五月",
        "六月",
        "七月",
        "八月",
        "九月",
        "十月",
        "十一月",
        "十二月"
    ],
    dateFormat: "mm/dd/yy",
    firstDayOfWeek: 0,
    today: "今日",
    weekHeader: "Wk",
    weak: "Weak",
    medium: "Medium",
    strong: "Strong",
    passwordPrompt: "Enter a password",
    emptyMessage: "暂无数据",
    emptyFilterMessage: "暂无数据"
};
var PCNConfig = "代码配置";
var PCNCodeParameter = "代码参数设置";
var PCNCodeStage = "代码阶段设置";
var PCNCodeTemplate = "代码模版设置";
var PCNMasterTemplate = "代码母版设置";
var PCNCodeBuild = "代码生成";
var PCNMachine = "机器控制";
var PCNMachineInfo = "机器信息设置";
var PCNWorkflow = "审批";
var PCNMachineModel = "机器Model信息";
var PCNMachineChange = "机器变更信息";
var PCNDefaultApprover = "默认审批人设置";
var PCNWorkcellCodeConfig = "Workcell代码配置";
var PCNNPI = "NPI";
var PCNSOP = "SOP";
var PCNApplyActionConfig = "应用配置";
var PCNEscalation = "升级";
var config = "配置";
var preview = "预览";
var value = "值";
var synchronize = "同步";
var handle = "处理";
var initiator = "发起人";
var applicant = "申请人";
var originator = "发起人";
var MasterDataManagement = "主数据管理系统";
var MasterDataHomepage = "主页";
var MasterDataBay = "产线";
var MasterDataEquipment = "设备";
var MasterDataEmployee = "人员";
var MasterDataHUAIDCreate = "HUA ID创建";
var MasterDataEmployeeQuery = "员工信息查询";
var MasterDataOperationConfiguration = "操作配置";
var MasterDataFunction = "职能部门";
var MasterDataOrganization = "工作坊组织架构";
var MasterDataLocation = "位置";
var MasterDataProductionArea = "段位";
var DataAssetMap = "数据资产目录";
var DataAssetHomepage = "主页";
var DataAssetSQLSearch = "SQL 查询";
var DataAssetMasterDataManagement = "主数据管理";
var DataAssetTopicDomainDefinition = "主题域定义";
var DataAssetMetadataManagement = "元数据管理";
var DataAssetDataApi = "数据 API";
var DataAssetDataAuthorization = "数据授权";
var DataAssetDataSource = "数据源";
var DataAssetDataTable = "数据表";
var DataAssetDataAccessLog = "数据访问日志";
var DataAssetDataAssetQuery = "数据资产查询";
var DataAssetDataAssetCatalog = "数据资产目录";
var DataAssetPermissionApplicationRecord = "权限申请记录";
var DataAssetUserPermissionApplicationRecord = "用户权限申请记录";
var DataAssetAuthorizedData = "已授权数据";
var ThingsLinkerBroker = "代理";
var ThingsLinkerEquipment = "设备";
var ThingsLinkerBooking = "预订";
var ThingsLinkerDispatchOrder = "发送指令";
var ThingsLinkerConfig = "配置";
var ThingsLinker = "事物联机";
var JiTBuildPlan = "构建计划";
var JITDeliveryBooking = "交货预订";
var JITPullListKanban = "拉取列表看板";
var JITSIgnFor = "签收";
var JITBuildPlanManagement = "计划管理";
var JITSubmitBuildPlan = "提交计划";
var JITShortageList = "短缺清单";
var JITDemandReport = "需求报告";
var JITRemainingInventory = "剩余库存";
var JITBaseData = "基础数据";
var JITDummyBOM = "虚拟物料清单";
var JITUPH = "UPH";
var JITModelFrequency = "型号频率";
var JITAGVLineInfo = "AGV线信息";
var JITSPQ = "SPQ";
var JITDeliveryLeadTime = "交货时间";
var JITStorageLocation = "存储位置";
var JITPullListApply = "拉取列表申请";
var JITDeliveryTaskControl = "交付任务控制";
var JITBuyerNameByPart = "采购姓名";
var JITWarehouseManagement = "仓库管理";
var JITWarehouseDelivery = "仓库发货";
var JITInventoryManagement = "库存管理";
var JITInventoryBin = "库存仓";
var JITInventoryReport = "库存报告";
var JITInventoryStoreConfig = "库存商配置";
var JITInventory = "库存";
var JITVendorAssignRate = "供应商分配率";
var JITLockOpenPO = "锁定的Open PO";
var JITPDSRemindConfig = "PDS提醒配置";
var JITPDSSearch = "PD查询";
var HuaDataLake = "数据湖主页";
var HUAJabilBus = "Jabil Bus主页";
var HUAIOTPlatform = "IOT平台主页";
var jit = {
    "form.status": "表单状态",
    "part.number": "零件号",
    "sap.model": "SAP模型",
    customer: "客户",
    vendor: "厂商",
    "material.request.time": "材料申请时间",
    "material.request.time.from": "材料申请起始时间",
    "material.request.time.to": "材料申请结束时间",
    "build.plan": "构建计划",
    "pull.list.no": "拉取列表编号",
    "delivery.booking": "交货预订",
    "delivery.time": "交货时间",
    "delivery.type": "交货方式",
    "bin.name": "站点名称",
    "cage.code": "载具编号",
    "storage.area": "存储区",
    "pull.list.pds.no": "拉取列表/PDS编号",
    "pull.list.pds.no.status": "拉取列表/PDS编号状态",
    "delivery.model": "传送模式",
    qty: "数量",
    "received.time.from": "接收起始时间",
    "received.time.to": "接收结束时间",
    "received.by": "接收人",
    "submit.time.from": "提交起始时间",
    "submit.time.to": "提交结束时间",
    "submit.by": "提交人",
    "received.from.wh.by": "仓库接收人",
    "stock.in.time.from": "入库起始时间",
    "stock.in.time.to": "入库结束时间",
    "stock.in.by": "入库人",
    "stock.out.time.from": "出库起始时间",
    "stock.out.time.to": "出库结束时间",
    "stock.out.by": "出库人",
    "delivery.time.from": "交货起始时间",
    "delivery.time.to": "交货结束时间",
    "delivery.by": "交货人",
    "received.in.line.time.from": "在线接收起始时间",
    "received.in.line.time.to": "在线接收结束时间",
    "received.in.line.by": "在线接收人",
    submitted: "已提交",
    "await.receive": "待接收",
    "to.be.confirmed": "待确认",
    received: "已接收",
    caged: "锁定",
    "stocked.in": "已入库",
    "stocked.out": "已出库",
    distributing: "分发中",
    closed: "已关闭",
    cancelled: "已取消",
    "received.from.wh.time.from": "仓库接收起始时间",
    "received.from.wh.time.to": "仓库接收结束时间",
    "material.request.time.invalid": "物料请求时间大于大于当前时间2小时，不能预定"
};
var AOS = "AOS";
var AOSCheckList = "检查清单";
var AOSAuditPlan = "审核计划";
var AOSTaskList = "任务列表";
var AOSApprove = "审批";
var AOSNCList = "NC列表";
var AOSConfig = "配置";
var AOSAuditor = "审计员";
var AOSAuditorTest = "审计员";
var BMWIOT = "BMWIOT";
var JMJagentMonitoring = "Jagent监测";
var JMMonitoring = "监测";
var JMWarningLog = "报警日志";
var JMConfig = "配置";
var SMJagentMonitoring = "服务器监测";
var SMMonitoring = "监测";
var SMWarningLog = "报警日志";
var SMConfig = "配置";
var cn = {
    translateVersion: translateVersion,
    "is.load.translate": "true",
    "export": "导出",
    "import": "导入",
    login: login,
    logoff: logoff,
    username: username,
    password: password,
    "remember.me": "记住密码",
    "forgot.login.details": "忘记密码",
    reset: reset,
    search: search,
    status: status,
    operation: operation,
    input: input,
    alert: alert,
    "return": "返回",
    cancel: cancel,
    all: all,
    normal: normal,
    warning: warning,
    filter: filter,
    submit: submit,
    date: date,
    data: data,
    customerBtn: customerBtn,
    day: day,
    No: No,
    add: add,
    modify: modify,
    save: save,
    "delete": "删除",
    disabled: disabled,
    enabled: enabled,
    "start.time": "开始时间",
    "select.start.time": "选择开始时间",
    "end.time": "结束时间",
    "select.end.time": "选择结束时间",
    nodata: nodata,
    time: time,
    close: close,
    "select.time": "选择时间",
    "range.date": "日期范围",
    "range.time": "时间范围",
    "select.range.date": "选择开始日期和结束日期",
    "basic.information": "基础信息",
    sort: sort,
    s: s,
    m: m,
    h: h,
    "release.time": "发布时间",
    "upload.time": "上传时间",
    "update.time": "更新时间",
    "category.name": "分类名称",
    category: category,
    yes: yes,
    no: no,
    online: online,
    offline: offline,
    permission: permission,
    "upload.file": "上传文件",
    "select.file": "点击或拖拽文件到此处上传",
    "save.success": "保存成功!",
    "save.fail": "保存失败!",
    "delete.success": "删除成功!",
    "delete.fail": "删除失败!",
    "is.delete": "确定删除此数据?",
    "form.is.not.valid": "表单无效!",
    "webSocket.is.closed": "连接关闭!",
    confirm: confirm,
    "file.upload.success": "文件上传成功！",
    "file.upload.fail": "文件上传失败！",
    hour: hour,
    "create.time": "创建时间",
    "create.date": "创建日期",
    "create.person": "创建人",
    "work.cell": "Work Cell",
    "bay.no": "产线",
    list: list,
    "this.week": "本周",
    "this.month": "本月",
    "problem.type": "问题类型",
    "average.duration": "平均时长",
    "week.on.month.ratio": "周环比",
    unresolved: unresolved,
    processing: processing,
    processed: processed,
    reviewed: reviewed,
    upgraded: upgraded,
    min: min,
    owners: owners,
    description: description,
    "occurred.time": "发生时间",
    severity: severity,
    station: station,
    "root.course": "处理报警",
    detail: detail,
    pic: pic,
    message: message,
    task: task,
    unread: unread,
    "set.all.read": "全部标为已读",
    view: view,
    technician: technician,
    engineer: engineer,
    shift: shift,
    tag: tag,
    author: author,
    "publish.date": "发表日期",
    "view.count": "浏览次数",
    reference: reference,
    "batch.delete": "批量删除",
    name: name,
    "parent.catalog": "父目录",
    "processing.information": "处理过程信息",
    "issue.type": "问题类型",
    upload: upload,
    solution: solution,
    action: action,
    "workflow.info": "流转信息",
    "complete.time": "完成时间",
    "handle.time": "处理时长",
    "approval.comment": "审批意见",
    "link.name": "Link Name",
    process: process,
    model: model,
    "problem.description": "问题描述",
    "process.name": "处理名称",
    "station.name": "站点名称",
    "operation.success": "操作成功",
    "operation.error": "操作失败",
    summary: summary,
    "return.approve": "退回流程",
    approve: approve,
    workflow: workflow,
    print: print,
    NTID: NTID,
    "feedback.time": "反馈时间",
    proposal: proposal,
    satisfaction: satisfaction,
    sector: sector,
    "record.time": "记录时间",
    attachment: attachment,
    download: download,
    "file.name": "文件名称",
    "shift.name": "班次名称",
    back: back,
    total: total,
    items: items,
    "guangzhou.huangpu": "广州（黄埔厂）",
    "no.menu.permission": "无菜单权限",
    "login.fail": "登录失败",
    "form.required": "字段必填",
    "download.template": "下载模版",
    "add.parent.node": "父节点新增",
    "add.sub.node": "子节点新增",
    icon: icon,
    order: order,
    comments: comments,
    "items.selected": "个选项选中",
    release: release,
    "please.input": "请输入.....",
    edit: edit,
    "no.results.found": "暂无数据",
    "name.is.repeat": "名称重复",
    "can.not.delete.referenced.data": "不能删除被引用数据",
    "can.not.edit.referenced.data": "不能编辑被引用数据",
    "email.download": "邮件文件下载",
    "user.ntid": "用户NTID",
    email: email,
    invalid: invalid,
    reminder: reminder,
    "home.reminder": "您暂无其它模块访问权限，可以联系管理员开通",
    "user.info": "用户信息",
    "user.name": "用户名",
    user: user,
    phone: phone,
    roles: roles,
    "upload.person": "上传人",
    code: code,
    "parent.name": "父级名称",
    "jpg.png": "只支持JPG/PNG格式文件上传",
    "template.invalid": "模版无效",
    "enable.or.not": "是否启用",
    title: title,
    "thumbs.up": "点赞",
    comment: comment,
    read: read,
    "reply.time": "回复时间",
    "reply.content": "回复内容",
    type: type,
    "min.value": "最小值",
    "max.value": "最大值",
    unit: unit,
    "preview.value": "预览值",
    remark: remark,
    copy: copy,
    "template.name": "模版名称",
    "config.name": "配置名称",
    "code.NO": "代码编码",
    "parameter.config": "参数配置",
    "parameter.count": "参数数量",
    plant: plant,
    "user.guide": "用户指南",
    "login.name": "登录名称",
    "psd.name.invalid": "密码或登录名不能为空",
    host: host,
    port: port,
    kind: kind,
    region: region,
    stream: stream,
    endPoint: endPoint,
    broker: broker,
    queue: queue,
    equipment: equipment,
    db: db,
    run: run,
    check: check,
    "select.no.data": "请选择数据",
    "psd.name.incorrect": "账号或密码错误",
    left: left,
    top: top,
    bay: bay,
    "equipment.type": "设备类型",
    "background.picture": "背景图",
    menu: menu,
    api: api,
    "common.approval.processes": "常用审批流程",
    "permission.application": "权限申请",
    "secondary.development.application": "二次开发申请",
    "release.application": "发布申请",
    "installation.application": "安装申请",
    "home.announcement": "公告栏",
    "type.number": "数值",
    "type.string": "字符",
    "is.show": "显示",
    "is.show.track": "显示轨道",
    "check.man.count": "审核人数",
    "no.upload.file": "请上传文件",
    "mobile.app.url": "移动端应用路径",
    "mobile.app.name": "移动端应用名称",
    "mobile.app.zh.name": "移动端应用中文名称",
    "mobile.app.bg": "移动端应用背景",
    "mobile.app.brief": "移动端应用简介",
    "mobile.app": "是否移动端应用",
    "mobile.end": "移动端",
    "machine.direction": "流向",
    "machine.direction.left": "从右往左",
    "machine.direction.right": "从左往右",
    "user.img": "用户头像",
    "old.password": "旧密码",
    "web.app.path": "web应用地址",
    "web.app.Router": "Web应用路由",
    "web.app.iframe": "是否Iframe应用",
    service: service,
    "email.or.ntid": "邮箱/NTID",
    project: project,
    cycle: cycle,
    reassign: reassign,
    "is.drafted": "是否草稿",
    "is.public": "是否已发表",
    "comment.time": "评论时间",
    "comment.content": "评论内容",
    topic: topic,
    "please.login.and.reload.page": "请登录后重新加载页面",
    "workflow.no": "流程编号",
    "workflow.type": "流程类型",
    reason: reason,
    approver: approver,
    "approved.time": "审批时间",
    opinion: opinion,
    agree: agree,
    reject: reject,
    "function.workcell": "Function/Workcell",
    "apply.date": "申请时间",
    "form.no.opinion": "请填写意见",
    "sme.invalid": "SME无效",
    application: application,
    "application.description": "应用系统描述",
    benefit: benefit,
    confirmed: confirmed,
    unconfirmed: unconfirmed,
    approving: approving,
    "git.address": "Git地址",
    "release.date": "发布日期",
    "version.no": "版本号",
    "release.content": "需发布内容",
    "home.web": "网站",
    "home.phone": "手机",
    "install.purpose": "安装用途",
    "home.jb.permission": "Jabil Bus 权限申请",
    "home.jb.second.dev": "Jabil Bus 二次开发申请",
    "home.jb.release": "Jabil Bus 发布申请",
    "home.ja.install": "JAgent 安装申请",
    "home.jb.service": "Jabil Bus 服务申请",
    "home.tesla.visit.permission": "特斯拉访问预先申请",
    "home.workflow.status.processing": "审核中",
    "home.workflow.status.processed": "已审核",
    "home.workflow.status.finish": "已完成",
    "work.cell.is.repeat": "WorkCell重复",
    "very.satisfied": "非常满意",
    satisfied: satisfied,
    ordinary: ordinary,
    dissatisfied: dissatisfied,
    "very.dissatisfied": "非常不满意",
    "no.mobile": "请在电脑端浏览，谢谢",
    note: note,
    "tesla.workflow.note": "申请必须在访问前至少4小时提交",
    "submitter.information": "提交人信息",
    "reception.information": "接待人信息",
    "approver.information": "审批人信息",
    "visitor.information": "访客人信息",
    "visitor.type": "访客类型",
    "visiting.time": "访问时间",
    company: company,
    "home.tesla.workflow.title": "职称",
    "home.tesla.workflow.purpose": "目的",
    refresh: refresh,
    Upgraded: Upgraded,
    NotProcessed: NotProcessed,
    Processing: Processing,
    Processed: Processed,
    Approving: Approving,
    Closed: Closed,
    Low: Low,
    Normal: Normal,
    High: High,
    Urgent: Urgent,
    Immediately: Immediately,
    All: All,
    Close: Close,
    Running: Running,
    RUSH_TO_REPAIR: RUSH_TO_REPAIR,
    ADD_PERSON: ADD_PERSON,
    NPI: NPI,
    DOWNTIME: DOWNTIME,
    POWER_CUT: POWER_CUT,
    RAINSTORM: RAINSTORM,
    UNDERMANNED: UNDERMANNED,
    WaitConfirm: WaitConfirm,
    HasConfirm: HasConfirm,
    WaitInterview: WaitInterview,
    HasInterview: HasInterview,
    "tesla.ct.monitoring": "监测",
    "tesla.ct.shift": "班次",
    "tesla.ct.last.shift": "上一班次情况",
    "tesla.ct.customer": "客户",
    "tesla.ct.select.customer": "选择客户",
    "tesla.ct.bay": "产线",
    "tesla.ct.select.bay": "选择产线",
    "tesla.ct.model": "型号",
    "tesla.ct.select.model": "选择型号",
    "tesla.ct.product": "产品",
    "tesla.ct.product.family": "产品系列",
    "tesla.ct.model.No": "模型编号",
    "tesla.ct.bay.No": "产线编号",
    "tesla.ct.route.text": "路线描述",
    "tesla.ct.date&time": "时间",
    "tesla.ct.accumulate.today.output": "今日产出",
    "tesla.ct.IE.UPH": "IE UPH",
    "tesla.ct.theoretical.UPH": "理论UPH",
    "tesla.ct.actual.UPH": "实际UPH",
    "tesla.ct.fail.qty": "次品",
    "tesla.ct.root.course": "处理报警",
    "tesla.ct.action": "措施",
    "tesla.ct.occurred.time": "发生时间",
    "tesla.ct.PIC": "PIC",
    "tesla.ct.history": "处理历史",
    "tesla.ct.progress": "进度",
    "tesla.ct.downtime": "停工期",
    "tesla.ct.what.happened": "发生问题",
    "tesla.ct.effective.output": "有效产量",
    "tesla.ct.has.a.serious.delay.please.check.it": "有一个严重的延误，请检查",
    "tesla.ct.sending.message": "发送信息",
    "tesla.ct.input.root.course": "输入处理方法，回车确认",
    "tesla.ct.input.action": "输入措施，回车确认",
    "tesla.ct.select.PIC": "Select PIC",
    "tesla.ct.delay": "延误",
    "tesla.ct.info": "信息",
    "tesla.ct.last.repair.records": "最近10条处理记录",
    "tesla.ct.report": "报告",
    "tesla.ct.actual.output": "实际产出",
    "tesla.ct.theoretical.output": "理论产出",
    "tesla.ct.list": "列表",
    "tesla.ct.graph": "图表",
    "tesla.ct.utilization.rate": "利用率",
    "tesla.ct.theoretical.CT": "理论CT",
    "tesla.ct.actual.CT": "实际CT",
    "tesla.ct.save.as.image": "保存图片",
    "tesla.ct.configuration": "配置",
    "tesla.ct.alert.configuration": "报警配置",
    "tesla.ct.condition": "条件",
    "tesla.ct.percentage.of.errors": "报警占比",
    "tesla.ct.alert.to": "接收人",
    "tesla.ct.select.alert.to": "选择接收人",
    "tesla.ct.error.when": "报警条件",
    "tesla.ct.warning.when": "警告条件",
    "tesla.ct.information": "信息",
    "tesla.ct.shift.name": "班次名称",
    "tesla.ct.input.shift.name": "输入班次名称",
    "tesla.ct.input.sort": "输入排序",
    "tesla.ct.input.IE.UPH": "输入IE UPH",
    "tesla.ct.select.sn": "名字",
    "tesla.ct.record": "维修记录",
    "tesla.ct.message.send": "信息已发送!",
    "tesla.ct.message.send.error": "信息发送失败!",
    "tesla.ct.select.station": "工位",
    "tesla.ct.effective.time.setting": "有效时长配置",
    "tesla.ct.effective.time": "有效时长",
    "tesla.ct.effective.date": "有效日期",
    "tesla.ct.effective.start.date": "有效开始日期",
    "tesla.ct.effective.end.date": "有效结束日期",
    "tesla.ct.effective.working.hours": "一天有效工作时间为",
    "tesla.ct.shift.time": "班次时长",
    "tesla.ct.delay.SN": "延迟 SN",
    "tesla.ct.SN.status": "SN 状态",
    "menu.temperature.collection": "温度收集",
    "menu.temperature.monitoring": "监测管理",
    "menu.temperature.analysis": "数据分析",
    "menu.temperature.configuration": "配置管理",
    "menu.remote.service": "远程服务",
    "menu.remote.sector": "部门管理",
    "menu.remote.machine": "机器管理",
    "menu.remote.user": "用户管理",
    "menu.remote.status": "状态管理",
    "menu.remote.control": "控制管理",
    "menu.tesla.ct.tracking": "特斯拉产线",
    "menu.tesla.ct.monitoring": "产线监测",
    "menu.tesla.ct.report": "分析报告",
    "menu.tesla.ct.configuration": "配置管理",
    "menu.tesla.ct.information": "信息管理",
    "menu.MFG.test": "考试管理",
    "menu.MFG.test.announcement": "MFG公告",
    "menu.MFG.test.course.ware": "课件管理",
    "menu.MFG.test.learning": "学习管理",
    "menu.MFG.test.question.bank": "题库管理",
    "menu.MFG.test.test": "考试管理",
    "menu.system": "系统管理",
    "menu.system.user": "用户管理",
    "menu.system.role": "角色管理",
    "menu.system.organization": "组织管理",
    "menu.system.menu": "菜单管理",
    "menu.system.audit.log": "审计日志",
    "menu.system.ES.log": "ES日志",
    "menu.system.data.dictionary": "数据字典",
    "menu.common.home": "首页",
    "mfg.training.announce.title": "公告标题",
    "mfg.training.announce.category": "公告类别",
    "mfg.training.announce.content": "公告内容",
    "mfg.training.course.ware.name": "课件名称",
    "mfg.training.course.ware.category": "课件分类",
    "mfg.training.course.ware.category.manage": "管理课件分类",
    "mfg.training.course.ware.open": "公开课件",
    "mfg.training.course.ware.select": "选择课件",
    "mfg.training.course.ware.support.file": "支持PDF或MP4",
    "mfg.training.course.ware.parent.category": "上级分类",
    "mfg.training.learning.title": "学习标题",
    "mfg.training.learning.people.number": "学习人数",
    "mfg.training.learning.associated.test": "关联考试",
    "mfg.training.learning.time": "学习时间",
    "mfg.training.learning.online.offline": "线上/线下",
    "mfg.training.learning.test": "是否考试",
    "mfg.training.learning.select.test": "选择考试",
    "mfg.training.learning.test.count": "考试次数",
    "mfg.training.learning.qr": "二维码签到",
    "mfg.training.learning.introduce": "学习简介",
    "mfg.training.learning.upload.ntid": "上传员工NTID",
    "mfg.training.test.title": "考试名称",
    "mfg.training.test.people": "考试人数",
    "mfg.training.test.category": "考试分类",
    "mfg.training.test.permission": "考试权限",
    "mfg.training.test.select.question.bank": "选择题库",
    "mfg.training.test.question.out.order": "试题乱序",
    "mfg.training.test.option.out.order": "选项乱序",
    "mfg.training.test.multi.option": "多选设置",
    "mfg.training.test.all.score": "全对得分",
    "mfg.training.test.part.score": "部分得分",
    "mfg.training.test.pass.score": "及格分数",
    "mfg.training.test.retest.count": "重考次数",
    "mfg.training.test.time": "考试时间",
    "mfg.training.test.duration": "考试时长",
    "mfg.training.test.show.result": "显示成绩",
    "mfg.training.test.score": "分数",
    "mfg.training.test.rank": "排行榜",
    "mfg.training.test.over.time": "超时提交",
    "mfg.training.test.introduce": "考试说明",
    "mfg.training.test.single.num": "单选题数量",
    "mfg.training.test.multi.num": "多选题数量",
    "mfg.training.test.true.false": "判断题数量",
    "mfg.training.test.multi.all.score": "多选全对得分",
    "mfg.training.test.multi.part.score": "多选部分得分",
    "mfg.training.test.single.score": "单选分数",
    "mfg.training.test.true.false.score": "判断分数",
    "mfg.training.question.bank.title": "题库名称",
    "mfg.training.question.bank.category": "题库分类",
    "mfg.training.question.bank.type": "题库类型",
    "mfg.training.question.bank.test": "考试题库",
    "mfg.training.question.bank.practice": "练习题库",
    "mfg.training.question.bank.permission": "题库权限",
    "mfg.training.question.bank.question": "查看题目",
    "mfg.training.question.bank.out.order": "乱序出题",
    "mfg.training.question.bank.upload": "上传题库",
    "mfg.training.question.option1": "请添加选项",
    "mfg.training.question.option2": "判断题需要为2个选项",
    "mfg.training.question.option3": "判断题有一个正确选项",
    "mfg.training.question.option4": "选择题至少3个选项",
    "mfg.training.question.option5": "单选题有一个正确选项",
    "mfg.training.question.option6": "多选题需要多个正确选项",
    "mfg.training.question.title": "题目名称",
    "mfg.training.question.type": "题目类型",
    "mfg.training.question.type.judge": "对错选择",
    "mfg.training.question.type.single": "单项选择",
    "mfg.training.question.type.multi": "多项选择",
    "mfg.training.question.content": "考题内容",
    "mfg.training.question.option.manage": "选项管理",
    "mfg.training.question.new.option": "编辑选项",
    "mfg.training.question.answer": "答案",
    "mfg.training.question.option": "选项",
    "mfg.training.question.explain": "考题解析",
    "mfg.training.add.category": "新增分类",
    "mfg.training.category.manage": "管理分类",
    "mfg.training.announce.picture": "宣传图片",
    "mfg.training.prevent.cheat": "防止作弊",
    "mfg.training.option.warning": "选项无效!",
    "mfg.training.course.ware.learn.duration": "课件学习时长",
    "mfg.training.course.ware.learn.people": "课件学习人数",
    "mfg.training.learn.course.statistics": "学习课程统计",
    "mfg.training.test.course.statistics": "考试课程统计",
    "mfg.training.test.detail": "考试详情",
    "mfg.care.care.title": "关爱活动标题",
    "mfg.care.care.category": "关爱活动分类",
    "mfg.care.care.time": "活动时间",
    "mfg.care.sign.employee": "签到人员",
    "mfg.care.sign.qr": "签到二维码",
    "mfg.care.sign.time": "签到时间",
    "mfg.care.employee.feedback": "员工反馈",
    "mfg.care.organizer": "举办方",
    "mfg.care.content": "活动内容",
    "mfg.care.interview.title": "员工面谈标题",
    "mfg.care.interview.category": "面谈主题类型",
    "mfg.care.interview.supervisor": "面谈主管/经理",
    "mfg.care.interview.employee": "面谈员工",
    "mfg.care.interview.time": "面谈时间",
    "mfg.care.site.employee": "现场员工",
    "mfg.care.abnormal.feedback": "异常反馈",
    "mfg.care.interview.content": "面谈内容",
    "mfg.care.feedback.content": "反馈内容",
    "mfg.active.name": "姓名",
    "mfg.participated.in.activities": "参加活动记录",
    "mfg.activity.initiator": "活动发起人",
    "mfg.satisfaction.assessment": "满意度评估",
    "mfg.activity.time": "活动时间",
    "mfg.activity.type": "活动类型",
    "mfg.activity.summary": "活动汇总",
    "mfg.employee": "员工",
    "mfg.teml": "序号",
    "mfg.satisfaction": "满意度",
    "mfg.month": "月份",
    "mfg.activity.count": "活动场次",
    "mfg.accumulated.participate.person": "累计参与人次",
    "mfg.care.pic": "负责人",
    "mfg.care.feedback.title": "反馈标题",
    "mfg.care.interview.status": "面谈状态",
    "mfg.care.feedback.status": "反馈状态",
    "mfg.care.normal.feedback": "正常反馈",
    "mfg.care.finish.interview": "完成面谈",
    "mfg.video.max.size": "视频最大可上传2g文件",
    "mfg.email.template": "邮箱上传模版",
    "mfg.question.bank.template": "题库上传模版",
    "mfg.file.20mb": "最大可上传20MB文件",
    "mfg.learn.detail": "学习详情",
    "mfg.guide.content": "指引内容",
    "mfg.guide.type": "指引类型",
    "mfg.release.person": "发布人",
    "mfg.to.do": "待办事项",
    "mfg.save.to.do": "保存待办事项",
    "mfg.to.do.list": "待办清单",
    "mfg.content": "事项内容",
    "mfg.guide.language": "引导语",
    "mfg.save.guide.language.template": "保存引导语模板",
    "mfg.on.job.days": "入职天数",
    "mfg.show.title": "显示标题",
    "mfg.keyword": "关键字",
    "mfg.attr": "属性",
    "mfg.question.title": "问题标题",
    "mfg.question.category": "问题分类",
    "mfg.resolvent": "解决方法",
    "mfg.hot.search": "热搜",
    "mfg.topping": "置顶",
    "mfg.feedback": "用户反馈",
    "mfg.consultation.content": "咨询内容",
    "mfg.consultation.person": "咨询人",
    "mfg.consultation.time": "咨询时间",
    "mfg.recovery.person": "回复人",
    "mfg.recovery.time": "回复时间",
    "mfg.rating": "用户评分",
    "mfg.publish.consultation.content": "发布咨询内容",
    "mfg.interview.number": "面谈人数",
    "mfg.active": "活动",
    "mfg.active.detail": "活动详情",
    "mfg.active.summary": "活动摘要",
    "mfg.no.permission": "无权限",
    "test.valid.not.resolved": "当前未解决Case",
    "test.valid.cumulative.upgrade": "累计升级case",
    "test.valid.resolved.today": "今日已解决case",
    "test.valid.generated.today": "今日产生Case",
    "test.valid.in.settlement": "解决中Case",
    "test.valid.problem.trend": "问题趋势",
    "test.valid.accumulated.problems": "累计产生问题（半年内）",
    "test.valid.problems.this.week": "本周产生问题",
    "test.valid.problems.this.month": "本月产生问题",
    "test.valid.problems.distribution": "问题分布情况（累计半年内）",
    "test.valid.solving.duration": "问题解决时长（周）",
    "test.valid.my.case": "我的case",
    "test.valid.max.processing.time": "本周case处理最长时间",
    "test.valid.average.processing.time": "本周case平均处理时间",
    "test.valid.shortest.processing.time": "本周case最短时间",
    "test.valid.number.completed": "本周完成case数",
    "test.valid.my.case.list": "我的case列表",
    "test.valid.event": "事件",
    "test.valid.assign.to": "指定人",
    "test.valid.occurrence time": "发生时间",
    "test.valid.speed": "已完成",
    "test.valid.processing.time": "处理时长",
    "test.valid.bay.data": "产线数据",
    "test.valid.station.data": "站点数据",
    "test.valid.staff.data": "员工数据",
    "test.valid.shift.data": "班次数据",
    "test.valid.add.bay": "添加产线",
    "test.valid.add.station": "添加站点",
    "test.valid.add.shift": "添加班次",
    "test.valid.edit.shift": "编辑班次",
    "test.valid.add.technician": "添加技术员",
    "test.valid.edit.technician": "编辑技术员",
    "test.valid.subject": "主题",
    "test.valid.show.catalog": "是否显示在目录",
    "test.valid.cook.book.tag.edit": "知识标签编辑",
    "test.valid.cook.book.tag.add": "知识标签添加",
    "test.valid.test.station.problem": "测试工站问题",
    "test.valid.upload.type": "仅支持jpg/png/MP4格式文件，单个文件不超过50M",
    "test.valid.isSync.cookbook": "是否一键同步到知识",
    "test.valid.tag.num": "标签:最多可选择5个标签",
    "test.valid.case.file.size": "文件不可超过50M!",
    "test.valid.case.file.count": "最多只能上传3张图片1个视频!",
    "test.valid.processing.files": "处理过程文件",
    "test.valid.solution.files": "解决文件",
    "test.valid.tag": "标签",
    "test.valid.test.station": "测试站点",
    "test.valid.case.no": "案例编号",
    "test.valid.host.name": "主机映射",
    "system.menu.route.name": "路由名称",
    "system.menu.route.path": "路由路径",
    "system.menu.is.hidden": "是否隐藏",
    "system.menu.parent.id": "父级id",
    "system.menu.file.path": "文件路径",
    "system.menu.display.name": "显示名称",
    "system.menu.keep.alive": "Keep Alive",
    "system.menu.close.tab": "关闭标签",
    "system.menu.parameter.type": "参数类型",
    "system.menu.parameter.key": "参数 Key",
    "system.menu.parameter.value": "参数值",
    "system.menu.button.name": "按钮名称",
    "escs.qos": "QOS",
    "escs.code.execution.results": "代码执行结果",
    "escs.machine.execution.results": "机器执行结果",
    "escs.execution.code": "执行代码",
    "escs.site": "厂",
    "escs.branch": "子厂",
    TestValidationManagement: TestValidationManagement,
    "TestValidationManagement.Dashboard": "仪表盘",
    "TestValidationManagement.MyCase": "我的案例",
    "TestValidationManagement.CaseCenter": "案例中心",
    "TestValidationManagement.MessageCenter": "信息中心",
    "TestValidationManagement.MainData": "主数据",
    "TestValidationManagement.CookbookList": "知识列表",
    "TestValidationManagement.CookbookLabel": "知识标签",
    TemperatureManagement: TemperatureManagement,
    "TemperatureManagement.Monitoring": "温度监测",
    "TemperatureManagement.Analysis": "温度分析",
    "TemperatureManagement.Configuration": "温度配置",
    RemoteControl: RemoteControl,
    "RemoteControl.Sector": "部门",
    "RemoteControl.Machine": "机器",
    "RemoteControl.User": "用户",
    "RemoteControl.Status": "状态",
    "RemoteControl.MachineControl": "控制机器",
    TeslaCTTrackingManagement: TeslaCTTrackingManagement,
    "TeslaCTTrackingManagement.Monitor": "产线监测",
    "TeslaCTTrackingManagement.Report": "分析报告",
    "TeslaCTTrackingManagement.Configuration": "配置管理",
    "TeslaCTTrackingManagement.Information": "信息管理",
    TeslaAutoBuildPlan: TeslaAutoBuildPlan,
    TeslaOrder: TeslaOrder,
    TeslaPlan: TeslaPlan,
    TeslaPlanMonitoring: TeslaPlanMonitoring,
    MFGPeopleManagement: MFGPeopleManagement,
    MFGDashboard: MFGDashboard,
    MFGTranning: MFGTranning,
    "MFGTranning.Announcement": "MFG公告",
    "MFGTranning.CourseWare": "课件管理",
    "MFGTranning.Learning": "学习管理",
    "MFGTranning.QuestionBank": "题库管理",
    "MFGTranning.Test": "考试管理",
    "MFGTranning.Statistics": "统计",
    MFGEmployeeCare: MFGEmployeeCare,
    "MFGEmployeeCare.CareRecord": "关爱记录",
    "MFGEmployeeCare.Interview": "访谈",
    "MFGEmployeeCare.InterviewReport": "报表",
    "MFGEmployeeCare.Feedback": "反馈",
    "MFGEmployeeCare.active.summary": "活动摘要",
    "MFGEmployeeCare.active.detail": "活动详情",
    "MFGEmployeeCare.employee.abnormal": "主动关爱",
    MFGEBuddy: MFGEBuddy,
    "MFGEBuddy.new.guide": "新人指引",
    "MFGEBuddy.frequently.question": "常见问题",
    "MFGEBuddy.consultation": "咨询管理",
    "MFGConfig.config": "配置管理",
    "MFGConfig.permission": "权限配置",
    "MFGConfig.employee": "员工",
    EPromotion: EPromotion,
    "ePromotion.hc": "人员空位",
    "ePromotion.skill": "技能空位",
    MFGPerformance: MFGPerformance,
    "MFGPerformance.PWT.calculation": "PWT奖金",
    "MFGPerformance.Leader.PWT.calculation": "Line Leader奖金",
    "MFGPerformance.PWT.calculation.result": "奖金计算结果",
    "MFGPerformance.PWT.team.manage": "DPE团队管理",
    "MFGPerformance.PWT.super.DL.bonus": "超级员工奖金",
    "MFGPerformance.PWT.super.LL.bonus": "超级助理奖金",
    System: System,
    Users: Users,
    Roles: Roles,
    BatchRoles: BatchRoles,
    Menus: Menus,
    AuditLog: AuditLog,
    ES: ES,
    DataDictionary: DataDictionary,
    SystemAnnouncement: SystemAnnouncement,
    ESCSManagement: ESCSManagement,
    "ESCSManagement.Index": "数字孪生",
    Home: Home,
    MesStreamConfig: MesStreamConfig,
    MesStreamWip: MesStreamWip,
    WorkflowConfig: WorkflowConfig,
    WorkflowTeslaConfig: WorkflowTeslaConfig,
    WorkflowPermissionConfig: WorkflowPermissionConfig,
    WIKIHome: WIKIHome,
    WIKIBackground: WIKIBackground,
    WIKIDashboard: WIKIDashboard,
    WIKITopic: WIKITopic,
    WIKIMyTopic: WIKIMyTopic,
    WIKIComment: WIKIComment,
    WIKIMyComment: WIKIMyComment,
    Sediot: Sediot,
    SediotCycleTime: SediotCycleTime,
    SediotEmailConfig: SediotEmailConfig,
    SediotProcessReport: SediotProcessReport,
    SediotUtilizationReport: SediotUtilizationReport,
    SediotPMAlert: SediotPMAlert,
    SediotLessonLearn: SediotLessonLearn,
    SediotLayout: SediotLayout,
    OC: OC,
    OCConfig: OCConfig,
    OCSummary: OCSummary,
    RealTime: RealTime,
    DataTraceability: DataTraceability,
    DataTraceabilityHomePage: DataTraceabilityHomePage,
    DataTraceabilityPortal: DataTraceabilityPortal,
    HOSAppWidget: HOSAppWidget,
    HOSRole: HOSRole,
    HOSTemplate: HOSTemplate,
    HOSRoleTemplateConfig: HOSRoleTemplateConfig,
    HMCWebsite: HMCWebsite,
    ReportCenter: ReportCenter,
    "user.operate": "用户操作",
    "password.confirm": "密码(再次输入)",
    "password.is.inconsistent": "密码不一致",
    "role.name": "角色名称",
    "role.operate": "角色操作",
    "is.default": "是否默认",
    "name.code": "名字/编码",
    "data.dictionary.operate": "数据字典操作",
    wiki: wiki,
    "release.topic": "发布话题",
    "topic.comment.trend": "话题/评论走势",
    "view.reply": "查看回复",
    "is.visible": "是否可见",
    "topic.title": "话题标题",
    "topic.content": "话题内容",
    "add.forum": "新增板块",
    draft: draft,
    "half.year": "半年",
    "today.topic.comment": "今日发帖/评论数",
    "count.comments": "条评论",
    "forum.manage": "板块管理",
    "category.manage": "分类管理",
    "add.category": "新增分类",
    "reply.to": "回复",
    reply: reply,
    pcn: pcn,
    primeng: primeng,
    PCNConfig: PCNConfig,
    PCNCodeParameter: PCNCodeParameter,
    PCNCodeStage: PCNCodeStage,
    PCNCodeTemplate: PCNCodeTemplate,
    PCNMasterTemplate: PCNMasterTemplate,
    PCNCodeBuild: PCNCodeBuild,
    PCNMachine: PCNMachine,
    PCNMachineInfo: PCNMachineInfo,
    PCNWorkflow: PCNWorkflow,
    PCNMachineModel: PCNMachineModel,
    PCNMachineChange: PCNMachineChange,
    PCNDefaultApprover: PCNDefaultApprover,
    PCNWorkcellCodeConfig: PCNWorkcellCodeConfig,
    PCNNPI: PCNNPI,
    PCNSOP: PCNSOP,
    PCNApplyActionConfig: PCNApplyActionConfig,
    PCNEscalation: PCNEscalation,
    "sort.unique": "排序字段不唯一",
    config: config,
    "machine.config": "机器配置",
    "insert.param": "插入参数",
    preview: preview,
    "add.machine.config": "添加机器配置",
    "manage.machine.config": "管理机器配置",
    "machine.name": "机器名称",
    "stage.name": "阶段名称",
    "master.template.name": "母版名称",
    "master.template": "母版",
    "sub.stage.name": "子阶段",
    "param.name": "参数名",
    value: value,
    "copy.template": "复制模版",
    synchronize: synchronize,
    handle: handle,
    initiator: initiator,
    "affected.models": "影响型号",
    "effective.date": "生效日期",
    "actual.implementation.date": "实际实施日期",
    "synchronize.data.success": "同步数据成功",
    "synchronize.data.fail": "同步数据失败",
    applicant: applicant,
    "base.info": "基础信息",
    "approval.process": "审批流程",
    "complete.review": "完成审核",
    "process.log": "审批日志",
    "approved.by": "审批人",
    "approval.time": "审批时间",
    "approval.opinions": "审批意见",
    "pcn.reject": "否决",
    "pcn.agree": "同意",
    originator: originator,
    MasterDataManagement: MasterDataManagement,
    MasterDataHomepage: MasterDataHomepage,
    MasterDataBay: MasterDataBay,
    MasterDataEquipment: MasterDataEquipment,
    MasterDataEmployee: MasterDataEmployee,
    MasterDataHUAIDCreate: MasterDataHUAIDCreate,
    MasterDataEmployeeQuery: MasterDataEmployeeQuery,
    MasterDataOperationConfiguration: MasterDataOperationConfiguration,
    MasterDataFunction: MasterDataFunction,
    MasterDataOrganization: MasterDataOrganization,
    MasterDataLocation: MasterDataLocation,
    MasterDataProductionArea: MasterDataProductionArea,
    "MasterData.UpdateRecordQuery": "更新记录查询",
    "MasterData.CostCenter": "成本中心",
    DataAssetMap: DataAssetMap,
    DataAssetHomepage: DataAssetHomepage,
    DataAssetSQLSearch: DataAssetSQLSearch,
    DataAssetMasterDataManagement: DataAssetMasterDataManagement,
    DataAssetTopicDomainDefinition: DataAssetTopicDomainDefinition,
    DataAssetMetadataManagement: DataAssetMetadataManagement,
    DataAssetDataApi: DataAssetDataApi,
    DataAssetDataAuthorization: DataAssetDataAuthorization,
    DataAssetDataSource: DataAssetDataSource,
    DataAssetDataTable: DataAssetDataTable,
    DataAssetDataAccessLog: DataAssetDataAccessLog,
    DataAssetDataAssetQuery: DataAssetDataAssetQuery,
    DataAssetDataAssetCatalog: DataAssetDataAssetCatalog,
    DataAssetPermissionApplicationRecord: DataAssetPermissionApplicationRecord,
    DataAssetUserPermissionApplicationRecord: DataAssetUserPermissionApplicationRecord,
    DataAssetAuthorizedData: DataAssetAuthorizedData,
    ThingsLinkerBroker: ThingsLinkerBroker,
    ThingsLinkerEquipment: ThingsLinkerEquipment,
    ThingsLinkerBooking: ThingsLinkerBooking,
    ThingsLinkerDispatchOrder: ThingsLinkerDispatchOrder,
    ThingsLinkerConfig: ThingsLinkerConfig,
    ThingsLinker: ThingsLinker,
    JiTBuildPlan: JiTBuildPlan,
    JITDeliveryBooking: JITDeliveryBooking,
    JITPullListKanban: JITPullListKanban,
    JITSIgnFor: JITSIgnFor,
    JITBuildPlanManagement: JITBuildPlanManagement,
    JITSubmitBuildPlan: JITSubmitBuildPlan,
    JITShortageList: JITShortageList,
    JITDemandReport: JITDemandReport,
    JITRemainingInventory: JITRemainingInventory,
    JITBaseData: JITBaseData,
    JITDummyBOM: JITDummyBOM,
    JITUPH: JITUPH,
    JITModelFrequency: JITModelFrequency,
    JITAGVLineInfo: JITAGVLineInfo,
    JITSPQ: JITSPQ,
    JITDeliveryLeadTime: JITDeliveryLeadTime,
    JITStorageLocation: JITStorageLocation,
    JITPullListApply: JITPullListApply,
    JITDeliveryTaskControl: JITDeliveryTaskControl,
    JITBuyerNameByPart: JITBuyerNameByPart,
    JITWarehouseManagement: JITWarehouseManagement,
    JITWarehouseDelivery: JITWarehouseDelivery,
    JITInventoryManagement: JITInventoryManagement,
    JITInventoryBin: JITInventoryBin,
    JITInventoryReport: JITInventoryReport,
    JITInventoryStoreConfig: JITInventoryStoreConfig,
    JITInventory: JITInventory,
    JITVendorAssignRate: JITVendorAssignRate,
    JITLockOpenPO: JITLockOpenPO,
    JITPDSRemindConfig: JITPDSRemindConfig,
    JITPDSSearch: JITPDSSearch,
    HuaDataLake: HuaDataLake,
    HUAJabilBus: HUAJabilBus,
    HUAIOTPlatform: HUAIOTPlatform,
    jit: jit,
    AOS: AOS,
    AOSCheckList: AOSCheckList,
    AOSAuditPlan: AOSAuditPlan,
    AOSTaskList: AOSTaskList,
    AOSApprove: AOSApprove,
    AOSNCList: AOSNCList,
    AOSConfig: AOSConfig,
    AOSAuditor: AOSAuditor,
    AOSAuditorTest: AOSAuditorTest,
    BMWIOT: BMWIOT,
    "BMWIOT.Dashboard": "仪表盘",
    "BMWIOT.Production": "产品生产与测试数据",
    "BMWIOT.EquipmentAlarm": "设备报警与信息",
    "BMWIOT.ChangeInformation": "变更信息",
    JMJagentMonitoring: JMJagentMonitoring,
    JMMonitoring: JMMonitoring,
    JMWarningLog: JMWarningLog,
    JMConfig: JMConfig,
    SMJagentMonitoring: SMJagentMonitoring,
    SMMonitoring: SMMonitoring,
    SMWarningLog: SMWarningLog,
    SMConfig: SMConfig
};

let self$1 = {};
class AppMenuService {
    constructor() {
        this.defaultCheckedKeys = [];
        this.translateObj = {};
        self$1 = this;
        const currentLanguage = localStorage.getItem('lang') || 'en';
        this.translateObj = currentLanguage === 'en' ? en : cn;
    }
    getMenuData(menuTreeKeys, menuTree) {
        return __awaiter(this, void 0, void 0, function* () {
            this.defaultCheckedKeys = [];
            menuTreeKeys.data.forEach((item) => {
                this.defaultCheckedKeys.push(item.menuInfoId);
            });
            this.buildTableTreeData(menuTree.data);
            if (menuTree.data.length === 0) {
                return [
                    {
                        label: this.translateObj['Home'],
                        routerLink: '/common/home',
                        key: 'DefaultPage',
                        icon: '',
                        url: undefined,
                        items: undefined,
                    },
                ];
            }
            menuTree.data.sort((a, b) => {
                return a.sort - b.sort;
            });
            this.sortMenu(menuTree.data);
            return menuTree.data.length === 1 && menuTree.data[0].items ? menuTree.data[0].items : menuTree.data;
        });
    }
    sortMenu(data) {
        var _a;
        for (let i = 0; i < data.length; i++) {
            (_a = data[i].items) === null || _a === void 0 ? void 0 : _a.sort((a, b) => {
                return a.sort - b.sort;
            });
            if (data[i].items) {
                this.sortMenu(data[i].items);
            }
        }
    }
    buildTableTreeData(data, pName, greatPName) {
        var _a, _b, _c, _d, _e, _f;
        const isHttps = document.location.protocol === 'https:';
        for (let index = 0; index < data.length; index++) {
            if (!this.defaultCheckedKeys.includes(data[index].menuInfo.id) ||
                data[index].menuInfo.isHidden || (data[index].menuInfo.isDisableNet && isHttps)) {
                data.splice(index, 1);
                index--;
                // data[index].visible = false;
                continue;
            }
            data[index].label =
                this.translateObj[data[index].menuInfo.title] || data[index].menuInfo.title;
            data[index].key = data[index].menuInfo.id;
            data[index].routerLink = data[index].menuInfo.path === '/' ? '' : data[index].menuInfo.path;
            data[index].routerPath = data[index].menuInfo.path === '/' ? '' : data[index].menuInfo.path;
            data[index].icon = data[index].menuInfo.icon;
            data[index].sort = data[index].menuInfo.sort;
            data[index].webAppPath = this.getWebApp((_a = data[index].menuInfo) === null || _a === void 0 ? void 0 : _a.webAppPath) || '';
            data[index].webAppRoute = ((_b = data[index].menuInfo) === null || _b === void 0 ? void 0 : _b.webAppRoute) || '';
            data[index].isWebApp = !!((_c = data[index].menuInfo) === null || _c === void 0 ? void 0 : _c.isWebApp);
            data[index].routerLink = ((_d = data[index].menuInfo) === null || _d === void 0 ? void 0 : _d.isWebApp) ? data[index].webAppRoute : data[index].routerLink;
            data[index].routerLinkActiveOptions = { exact: !!((_e = data[index].menuInfo) === null || _e === void 0 ? void 0 : _e.isWebApp) };
            data[index].url = undefined;
            data[index].pName = pName;
            data[index].greatPName = greatPName;
            data[index].items =
                data[index].menuChildren.length > 0 ? data[index].menuChildren : undefined;
            if (data[index].menuChildren && data[index].menuChildren.length > 0) {
                this.buildTableTreeData(data[index].menuChildren, data[index].label, (_f = data[index]) === null || _f === void 0 ? void 0 : _f.pName);
            }
        }
    }
    getWebApp(str) {
        if (!str || !(str === null || str === void 0 ? void 0 : str.includes('modules'))) {
            return str;
        }
        const urls = str.split('modules');
        if (urls.length > 0) {
            return location.origin + '/modules' + (urls[1] || '');
        }
        else {
            return str;
        }
    }
    isExternalNetwork() {
    }
    getMenuInfo(menuTreeKeys, menuTree, key) {
        let menu = null;
        menuTree.forEach((item) => {
            if (item.menuInfo.id === key) {
                menu = item;
            }
        });
        this.buildMenuData([menu], menuTreeKeys);
        return menu;
    }
    buildMenuData(data, menuTreeKeys) {
        for (let i = 0; i < data.length; i++) {
            const index = menuTreeKeys.findIndex((item) => {
                return data[i].menuInfo.id === item.menuInfoId;
            });
            if (index < 0) {
                data.splice(i, 1);
                i--;
                continue;
            }
            if (data[i].menuChildren && data[i].menuChildren.length > 0) {
                this.buildMenuData(data[i].menuChildren, menuTreeKeys);
            }
        }
    }
}
AppMenuService.ɵfac = function AppMenuService_Factory(t) { return new (t || AppMenuService)(); };
AppMenuService.ɵprov = /*@__PURE__*/ i0.ɵɵdefineInjectable({ token: AppMenuService, factory: AppMenuService.ɵfac });
(function () {
    (typeof ngDevMode === "undefined" || ngDevMode) && i0.ɵsetClassMetadata(AppMenuService, [{
            type: Injectable
        }], function () { return []; }, null);
})();

var layout = {
    "layout-logo": "data:image/png;base64,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",
    "layout-logo-collapsed": "data:image/png;base64,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",
    user: "data:image/jpeg;base64,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",
    "sidebar-icon": "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAACgAAAAoCAYAAACM/rhtAAAAAXNSR0IArs4c6QAAAbRJREFUWEft1q9Pw0AUB/D3OjOBReD2L/QcjgUSIEFgEEgcfwIJS0DAH4DEISdJwJAgISFp722CLOAqJ3As2QTrkZKyNM3a+7V1FTfbu75Pv6/3VoSa/7DmPnBA2w65BF2CtgnY7nfvYC0SZIztAkAXEZ8550dlKMbYOSJ2AOCMc34jewDrFic4RLwHgCYAfHHO14uKMsYuEPEyuS6E6BLR8VKBWZwQYhTH8V6/33+dVzSLA4APz/PaQRAMlwasApfgjVqca+tkOp3uLDq5/2S1gXmcEOKQiJ4W2dbsvbSAVeO0WrwKnDLQ9/1tz/Me01EykbQ1mXNX6SiJGo3GpsppLTrNSi1mjD0g4kFa9JSIbktm3TcirgHADyL6YRi+y0ZJ2XUlYC7BISJuhWH4WXAwZgkCwMt4PN4fDAYjU6QSMLl57h2UIWf/GLZIZeCqkFrAVSC1gVUjjYBVIo2BVSGtgPOQnPMNxe/BOyI6kY0fa2AWGcfxW6/Xa0u+qP9GkBCiQ0TXlQCTIq1WqxlF0URWUHftQhJUQZmucUDT5Iy/qG0L6u53LdZNLL/eJegStE3Adv8v1UglOOgmLHQAAAAASUVORK5CYII=",
    "fullscreen-expand": "data:image/png;base64,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",
    "fullscreen-shrink": "data:image/png;base64,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"
};
var imageData = {
    layout: layout
};

/**
 * name:http service
 * describe: http common module
 */
let self = {};
let isDownload = false;
let selfName = '';
class HttpService {
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
        var _a;
        const blob = new Blob([res.body], {
            type: res.body.type,
        });
        const filename = selfName ||
            window.decodeURI((_a = res.headers.get('content-disposition')) === null || _a === void 0 ? void 0 : _a.split('=')[1]);
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
        var _a, _b, _c, _d, _e;
        // download
        if (((_a = res === null || res === void 0 ? void 0 : res.url) === null || _a === void 0 ? void 0 : _a.includes('export')) || ((_b = res === null || res === void 0 ? void 0 : res.url) === null || _b === void 0 ? void 0 : _b.includes('Export')) ||
            ((_c = res === null || res === void 0 ? void 0 : res.url) === null || _c === void 0 ? void 0 : _c.includes('excel')) || ((_d = res === null || res === void 0 ? void 0 : res.url) === null || _d === void 0 ? void 0 : _d.includes('Excel')) ||
            isDownload) {
            const blob = new Blob([res.body], {
                type: res.body.type,
            });
            const filename = selfName ||
                window.decodeURI((_e = res.headers.get('content-disposition')) === null || _e === void 0 ? void 0 : _e.split('=')[1]);
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
(function () {
    (typeof ngDevMode === "undefined" || ngDevMode) && i0.ɵsetClassMetadata(HttpService, [{
            type: Injectable
        }], function () { return [{ type: i1.HttpClient }]; }, null);
})();

function LayoutComponent_div_1_img_2_Template(rf, ctx) {
    if (rf & 1) {
        i0.ɵɵelement(0, "img", 15);
    }
    if (rf & 2) {
        const ctx_r6 = i0.ɵɵnextContext(2);
        i0.ɵɵproperty("src", ctx_r6.layoutLogo, i0.ɵɵsanitizeUrl);
    }
}
function LayoutComponent_div_1_img_3_Template(rf, ctx) {
    if (rf & 1) {
        i0.ɵɵelement(0, "img", 15);
    }
    if (rf & 2) {
        const ctx_r7 = i0.ɵɵnextContext(2);
        i0.ɵɵproperty("src", ctx_r7.layoutLogoCollapsed, i0.ɵɵsanitizeUrl);
    }
}
const _c0 = function () { return { width: "100%" }; };
function LayoutComponent_div_1_p_panelMenu_4_Template(rf, ctx) {
    if (rf & 1) {
        i0.ɵɵelement(0, "p-panelMenu", 16);
    }
    if (rf & 2) {
        const ctx_r8 = i0.ɵɵnextContext(2);
        i0.ɵɵstyleMap(i0.ɵɵpureFunction0(3, _c0));
        i0.ɵɵproperty("model", ctx_r8.menuList);
    }
}
function LayoutComponent_div_1_p_tieredMenu_5_Template(rf, ctx) {
    if (rf & 1) {
        i0.ɵɵelement(0, "p-tieredMenu", 17);
    }
    if (rf & 2) {
        const ctx_r9 = i0.ɵɵnextContext(2);
        i0.ɵɵstyleMap(i0.ɵɵpureFunction0(4, _c0));
        i0.ɵɵproperty("autoDisplay", true)("model", ctx_r9.menuList);
    }
}
function LayoutComponent_div_1_Template(rf, ctx) {
    if (rf & 1) {
        i0.ɵɵelementStart(0, "div")(1, "div", 11);
        i0.ɵɵtemplate(2, LayoutComponent_div_1_img_2_Template, 1, 1, "img", 12);
        i0.ɵɵtemplate(3, LayoutComponent_div_1_img_3_Template, 1, 1, "img", 12);
        i0.ɵɵelementEnd();
        i0.ɵɵtemplate(4, LayoutComponent_div_1_p_panelMenu_4_Template, 1, 4, "p-panelMenu", 13);
        i0.ɵɵtemplate(5, LayoutComponent_div_1_p_tieredMenu_5_Template, 1, 5, "p-tieredMenu", 14);
        i0.ɵɵelementEnd();
    }
    if (rf & 2) {
        const ctx_r0 = i0.ɵɵnextContext();
        i0.ɵɵclassMap(ctx_r0.isCollapsed ? "collapsed siderbar" : "siderbar");
        i0.ɵɵadvance(2);
        i0.ɵɵproperty("ngIf", !ctx_r0.isCollapsed);
        i0.ɵɵadvance(1);
        i0.ɵɵproperty("ngIf", ctx_r0.isCollapsed);
        i0.ɵɵadvance(1);
        i0.ɵɵproperty("ngIf", !ctx_r0.isCollapsed);
        i0.ɵɵadvance(1);
        i0.ɵɵproperty("ngIf", ctx_r0.isCollapsed);
    }
}
function LayoutComponent_div_4_span_5_Template(rf, ctx) {
    if (rf & 1) {
        i0.ɵɵelementStart(0, "span");
        i0.ɵɵtext(1, ">>");
        i0.ɵɵelementEnd();
    }
}
function LayoutComponent_div_4_span_7_Template(rf, ctx) {
    if (rf & 1) {
        i0.ɵɵelementStart(0, "span");
        i0.ɵɵtext(1, ">>");
        i0.ɵɵelementEnd();
    }
}
function LayoutComponent_div_4_img_11_Template(rf, ctx) {
    if (rf & 1) {
        const _r17 = i0.ɵɵgetCurrentView();
        i0.ɵɵelementStart(0, "img", 20);
        i0.ɵɵlistener("click", function LayoutComponent_div_4_img_11_Template_img_click_0_listener() { i0.ɵɵrestoreView(_r17); const ctx_r16 = i0.ɵɵnextContext(2); return i0.ɵɵresetView(ctx_r16.exitFullScreen()); });
        i0.ɵɵelementEnd();
    }
    if (rf & 2) {
        const ctx_r12 = i0.ɵɵnextContext(2);
        i0.ɵɵproperty("src", ctx_r12.fullscreenShrink, i0.ɵɵsanitizeUrl);
    }
}
function LayoutComponent_div_4_img_12_Template(rf, ctx) {
    if (rf & 1) {
        const _r19 = i0.ɵɵgetCurrentView();
        i0.ɵɵelementStart(0, "img", 20);
        i0.ɵɵlistener("click", function LayoutComponent_div_4_img_12_Template_img_click_0_listener() { i0.ɵɵrestoreView(_r19); const ctx_r18 = i0.ɵɵnextContext(2); return i0.ɵɵresetView(ctx_r18.fullScreen()); });
        i0.ɵɵelementEnd();
    }
    if (rf & 2) {
        const ctx_r13 = i0.ɵɵnextContext(2);
        i0.ɵɵproperty("src", ctx_r13.fullscreenExpand, i0.ɵɵsanitizeUrl);
    }
}
function LayoutComponent_div_4_Template(rf, ctx) {
    if (rf & 1) {
        const _r21 = i0.ɵɵgetCurrentView();
        i0.ɵɵelementStart(0, "div", 18)(1, "div", 19)(2, "img", 20);
        i0.ɵɵlistener("click", function LayoutComponent_div_4_Template_img_click_2_listener() { i0.ɵɵrestoreView(_r21); const ctx_r20 = i0.ɵɵnextContext(); return i0.ɵɵresetView(ctx_r20.isCollapsed = !ctx_r20.isCollapsed); });
        i0.ɵɵelementEnd();
        i0.ɵɵelementStart(3, "span", 21);
        i0.ɵɵtext(4);
        i0.ɵɵtemplate(5, LayoutComponent_div_4_span_5_Template, 2, 0, "span", 7);
        i0.ɵɵtext(6);
        i0.ɵɵtemplate(7, LayoutComponent_div_4_span_7_Template, 2, 0, "span", 7);
        i0.ɵɵtext(8);
        i0.ɵɵelementEnd()();
        i0.ɵɵelementStart(9, "div", 22)(10, "span");
        i0.ɵɵtemplate(11, LayoutComponent_div_4_img_11_Template, 1, 1, "img", 23);
        i0.ɵɵtemplate(12, LayoutComponent_div_4_img_12_Template, 1, 1, "img", 23);
        i0.ɵɵelementEnd();
        i0.ɵɵelementStart(13, "span", 24);
        i0.ɵɵlistener("click", function LayoutComponent_div_4_Template_span_click_13_listener() { i0.ɵɵrestoreView(_r21); const ctx_r22 = i0.ɵɵnextContext(); return i0.ɵɵresetView(ctx_r22.editUserEvent()); });
        i0.ɵɵelement(14, "img", 25);
        i0.ɵɵtext(15);
        i0.ɵɵelementEnd();
        i0.ɵɵelementStart(16, "span", 26);
        i0.ɵɵlistener("click", function LayoutComponent_div_4_Template_span_click_16_listener($event) { i0.ɵɵrestoreView(_r21); const _r14 = i0.ɵɵreference(20); return i0.ɵɵresetView(_r14.toggle($event)); });
        i0.ɵɵelement(17, "i", 27);
        i0.ɵɵtext(18);
        i0.ɵɵelementEnd();
        i0.ɵɵelement(19, "p-tieredMenu", 28, 29);
        i0.ɵɵelementStart(21, "div", 30);
        i0.ɵɵlistener("click", function LayoutComponent_div_4_Template_div_click_21_listener() { i0.ɵɵrestoreView(_r21); const ctx_r24 = i0.ɵɵnextContext(); return i0.ɵɵresetView(ctx_r24.logoff()); });
        i0.ɵɵelement(22, "i", 31);
        i0.ɵɵtext(23);
        i0.ɵɵelementEnd();
        i0.ɵɵelement(24, "p-tieredMenu", 32, 33);
        i0.ɵɵtext(26, "\u00A0\u00A0 ");
        i0.ɵɵelementEnd()();
    }
    if (rf & 2) {
        const ctx_r1 = i0.ɵɵnextContext();
        i0.ɵɵadvance(2);
        i0.ɵɵproperty("src", ctx_r1.sidebarIcon, i0.ɵɵsanitizeUrl);
        i0.ɵɵadvance(2);
        i0.ɵɵtextInterpolate1(" ", ctx_r1.tabMenu[ctx_r1.selectedIndex || 0] && ctx_r1.tabMenu[ctx_r1.selectedIndex || 0].greatPName, " ");
        i0.ɵɵadvance(1);
        i0.ɵɵproperty("ngIf", ctx_r1.tabMenu[ctx_r1.selectedIndex || 0] && ctx_r1.tabMenu[ctx_r1.selectedIndex || 0].greatPName);
        i0.ɵɵadvance(1);
        i0.ɵɵtextInterpolate1(" ", ctx_r1.tabMenu[ctx_r1.selectedIndex || 0] && ctx_r1.tabMenu[ctx_r1.selectedIndex || 0].pName, " ");
        i0.ɵɵadvance(1);
        i0.ɵɵproperty("ngIf", ctx_r1.tabMenu[ctx_r1.selectedIndex || 0] && ctx_r1.tabMenu[ctx_r1.selectedIndex || 0].pName);
        i0.ɵɵadvance(1);
        i0.ɵɵtextInterpolate1(" ", ctx_r1.tabMenu[ctx_r1.selectedIndex || 0] && ctx_r1.tabMenu[ctx_r1.selectedIndex || 0].label, " ");
        i0.ɵɵadvance(3);
        i0.ɵɵproperty("ngIf", ctx_r1.isFullScreen);
        i0.ɵɵadvance(1);
        i0.ɵɵproperty("ngIf", !ctx_r1.isFullScreen);
        i0.ɵɵadvance(2);
        i0.ɵɵproperty("src", ctx_r1.user, i0.ɵɵsanitizeUrl);
        i0.ɵɵadvance(1);
        i0.ɵɵtextInterpolate1(" ", ctx_r1.username, " ");
        i0.ɵɵadvance(3);
        i0.ɵɵtextInterpolate1("\u00A0", ctx_r1.langDesc, " ");
        i0.ɵɵadvance(1);
        i0.ɵɵproperty("model", ctx_r1.language)("popup", true);
        i0.ɵɵadvance(4);
        i0.ɵɵtextInterpolate1("\u00A0", ctx_r1.logoffObj[ctx_r1.currentLanguage], " ");
        i0.ɵɵadvance(1);
        i0.ɵɵproperty("popup", true);
    }
}
function LayoutComponent_div_7_Template(rf, ctx) {
    if (rf & 1) {
        const _r26 = i0.ɵɵgetCurrentView();
        i0.ɵɵelementStart(0, "div", 34);
        i0.ɵɵlistener("click", function LayoutComponent_div_7_Template_div_click_0_listener() { i0.ɵɵrestoreView(_r26); const ctx_r25 = i0.ɵɵnextContext(); return i0.ɵɵresetView(ctx_r25.scroll("left")); });
        i0.ɵɵelement(1, "i", 35);
        i0.ɵɵelementEnd();
    }
}
function LayoutComponent_ng_container_8_li_3_a_1_a_1_Template(rf, ctx) {
    if (rf & 1) {
        const _r37 = i0.ɵɵgetCurrentView();
        i0.ɵɵelementStart(0, "a", 45);
        i0.ɵɵlistener("click", function LayoutComponent_ng_container_8_li_3_a_1_a_1_Template_a_click_0_listener() { i0.ɵɵrestoreView(_r37); const i_r31 = i0.ɵɵnextContext(2).index; const ctx_r35 = i0.ɵɵnextContext(2); return i0.ɵɵresetView(ctx_r35.chooseTab(i_r31)); });
        i0.ɵɵtext(1);
        i0.ɵɵelementEnd();
    }
    if (rf & 2) {
        const tab_r30 = i0.ɵɵnextContext(2).$implicit;
        i0.ɵɵproperty("routerLink", tab_r30.routerLink);
        i0.ɵɵadvance(1);
        i0.ɵɵtextInterpolate1(" ", tab_r30.label, " ");
    }
}
function LayoutComponent_ng_container_8_li_3_a_1_i_3_Template(rf, ctx) {
    if (rf & 1) {
        const _r41 = i0.ɵɵgetCurrentView();
        i0.ɵɵelementStart(0, "i", 46);
        i0.ɵɵlistener("click", function LayoutComponent_ng_container_8_li_3_a_1_i_3_Template_i_click_0_listener() { i0.ɵɵrestoreView(_r41); const i_r31 = i0.ɵɵnextContext(2).index; const ctx_r39 = i0.ɵɵnextContext(2); return i0.ɵɵresetView(ctx_r39.closeTab(i_r31)); });
        i0.ɵɵelementEnd();
    }
}
function LayoutComponent_ng_container_8_li_3_a_1_Template(rf, ctx) {
    if (rf & 1) {
        i0.ɵɵelementStart(0, "a", 42);
        i0.ɵɵtemplate(1, LayoutComponent_ng_container_8_li_3_a_1_a_1_Template, 2, 2, "a", 43);
        i0.ɵɵtext(2, "\u00A0\u00A0 ");
        i0.ɵɵtemplate(3, LayoutComponent_ng_container_8_li_3_a_1_i_3_Template, 1, 0, "i", 44);
        i0.ɵɵelementEnd();
    }
    if (rf & 2) {
        const ctx_r42 = i0.ɵɵnextContext();
        const i_r31 = ctx_r42.index;
        const tab_r30 = ctx_r42.$implicit;
        const ctx_r32 = i0.ɵɵnextContext(2);
        i0.ɵɵclassMap(ctx_r32.selectedIndex === i_r31 ? "active" : "");
        i0.ɵɵadvance(1);
        i0.ɵɵproperty("ngIf", tab_r30 == null ? null : tab_r30.routerLink);
        i0.ɵɵadvance(2);
        i0.ɵɵproperty("ngIf", i_r31 > 0);
    }
}
function LayoutComponent_ng_container_8_li_3_Template(rf, ctx) {
    if (rf & 1) {
        const _r44 = i0.ɵɵgetCurrentView();
        i0.ɵɵelementStart(0, "li", 40);
        i0.ɵɵlistener("contextmenu", function LayoutComponent_ng_container_8_li_3_Template_li_contextmenu_0_listener() { const restoredCtx = i0.ɵɵrestoreView(_r44); const tab_r30 = restoredCtx.$implicit; const i_r31 = restoredCtx.index; const ctx_r43 = i0.ɵɵnextContext(2); return i0.ɵɵresetView(ctx_r43.contextMenu(tab_r30, i_r31)); });
        i0.ɵɵtemplate(1, LayoutComponent_ng_container_8_li_3_a_1_Template, 4, 4, "a", 41);
        i0.ɵɵelementEnd();
    }
    if (rf & 2) {
        const tab_r30 = ctx.$implicit;
        i0.ɵɵadvance(1);
        i0.ɵɵproperty("ngIf", (tab_r30 == null ? null : tab_r30.label) && (tab_r30 == null ? null : tab_r30.routerLink));
    }
}
function LayoutComponent_ng_container_8_p_contextMenu_4_Template(rf, ctx) {
    if (rf & 1) {
        const _r46 = i0.ɵɵgetCurrentView();
        i0.ɵɵelementStart(0, "p-contextMenu", 47);
        i0.ɵɵlistener("onShow", function LayoutComponent_ng_container_8_p_contextMenu_4_Template_p_contextMenu_onShow_0_listener() { i0.ɵɵrestoreView(_r46); const ctx_r45 = i0.ɵɵnextContext(2); return i0.ɵɵresetView(ctx_r45.showContextMenu = true); })("onHide", function LayoutComponent_ng_container_8_p_contextMenu_4_Template_p_contextMenu_onHide_0_listener() { i0.ɵɵrestoreView(_r46); const ctx_r47 = i0.ɵɵnextContext(2); return i0.ɵɵresetView(ctx_r47.showContextMenu = false); });
        i0.ɵɵelementEnd();
    }
    if (rf & 2) {
        i0.ɵɵnextContext();
        const _r27 = i0.ɵɵreference(2);
        const ctx_r29 = i0.ɵɵnextContext();
        i0.ɵɵproperty("target", _r27)("model", ctx_r29.items);
    }
}
const _c1 = function (a0) { return { "padding": a0 }; };
function LayoutComponent_ng_container_8_Template(rf, ctx) {
    if (rf & 1) {
        i0.ɵɵelementContainerStart(0);
        i0.ɵɵelementStart(1, "ul", 36, 37);
        i0.ɵɵtemplate(3, LayoutComponent_ng_container_8_li_3_Template, 2, 1, "li", 38);
        i0.ɵɵelementEnd();
        i0.ɵɵtemplate(4, LayoutComponent_ng_container_8_p_contextMenu_4_Template, 1, 2, "p-contextMenu", 39);
        i0.ɵɵelementContainerEnd();
    }
    if (rf & 2) {
        const ctx_r3 = i0.ɵɵnextContext();
        i0.ɵɵadvance(1);
        i0.ɵɵstyleMap(i0.ɵɵpureFunction1(4, _c1, ctx_r3.hasScroll ? "0 30px" : ""));
        i0.ɵɵadvance(2);
        i0.ɵɵproperty("ngForOf", ctx_r3.tabMenu);
        i0.ɵɵadvance(1);
        i0.ɵɵproperty("ngIf", !ctx_r3.fullPage);
    }
}
function LayoutComponent_div_9_Template(rf, ctx) {
    if (rf & 1) {
        const _r49 = i0.ɵɵgetCurrentView();
        i0.ɵɵelementStart(0, "div", 48);
        i0.ɵɵlistener("click", function LayoutComponent_div_9_Template_div_click_0_listener() { i0.ɵɵrestoreView(_r49); const ctx_r48 = i0.ɵɵnextContext(); return i0.ɵɵresetView(ctx_r48.scroll("right")); });
        i0.ɵɵelement(1, "i", 49);
        i0.ɵɵelementEnd();
    }
}
function LayoutComponent_div_11_Template(rf, ctx) {
    if (rf & 1) {
        i0.ɵɵelement(0, "div", 50);
    }
}
class LayoutComponent {
    constructor(translate, router, route, ref, appMenuService, http, location) {
        this.translate = translate;
        this.router = router;
        this.route = route;
        this.ref = ref;
        this.appMenuService = appMenuService;
        this.http = http;
        this.location = location;
        this.editUser = new EventEmitter();
        this.userDisplay = false;
        this.formData = {};
        this.logoffObj = {
            en: 'Logout',
            zh: '退出',
        };
        // @ts-ignore
        this.layoutLogo = imageData.layout['layout-logo'];
        // @ts-ignore
        this.layoutLogoCollapsed = imageData.layout['layout-logo-collapsed'];
        // @ts-ignore
        this.user = localStorage.getItem('avatar')
            ? '/gateway/basic/file/preview?fileName=' + localStorage.getItem('avatar')
            : imageData.layout['user'];
        // @ts-ignore
        this.sidebarIcon = imageData.layout['sidebar-icon'];
        // @ts-ignore
        this.fullscreenExpand = imageData.layout['fullscreen-expand'];
        // @ts-ignore
        this.fullscreenShrink = imageData.layout['fullscreen-shrink'];
        this.tabMenu = [];
        this.tabs = [];
        this.selectedIndex = 0;
        this.isCollapsed = false;
        this.fullPage = true; // router
        this.canGetData = false;
        this.hasScroll = false;
        this.isFullScreen = false; // button
        this.menuList = [];
        this.currRouter = {};
        this.username = localStorage.getItem('username') || '';
        this.currTabObj = {};
        this.contextMenuTab = {};
        this.contextMenuIndex = 0;
        this.firstJoin = true;
        this.showContextMenu = false;
        this.items = [];
    }
    setCommand(menuList) {
        for (let index = 0; index < menuList.length; index++) {
            menuList[index].command = (event) => {
                this.tabClick(menuList[index]);
            };
            if (menuList[index].items && menuList[index].items.length > 0) {
                this.setCommand(menuList[index].items);
            }
        }
    }
    ngOnInit() {
        var _a, _b, _c, _d;
        return __awaiter(this, void 0, void 0, function* () {
            this.currentLanguage = localStorage.getItem('lang');
            this.translate.use(this.currentLanguage);
            this.items = [
                {
                    label: this.currentLanguage === 'en' ? 'Close All' : '关闭全部',
                    // icon: 'pi pi-fw pi-trash',
                    command: (e) => {
                        this.closeAllTab(e);
                    }
                },
                {
                    separator: true
                },
                {
                    label: this.currentLanguage === 'en' ? 'Close Others' : '关闭其它',
                    // icon: "pi pi-fw pi-power-off",
                    command: (e) => {
                        this.closeOtherTab(e);
                    }
                }
            ];
            // @ts-ignore
            // const menuTreeKeys = await this.roleHttpService.getLoginRoleMenuGrant();
            // const menuTree = await this.menuHttpService.getMenuTree();
            this.menuList = yield this.appMenuService.getMenuData(this.menuTreeKeys, this.menuTree);
            this.setCommand(this.menuList);
            this.router.events
                .pipe(filter$2((event) => event instanceof NavigationEnd), map(() => this.route), map((route) => {
                while (route.firstChild) {
                    route = route.firstChild; // Conditional if needed in scenario
                }
                return route;
            }))
                .subscribe((elem) => {
                console.log('route event');
                const path = this.location.path();
                this.getItemByPath(this.menuList, path);
                this.canGetData = true;
                this.fullPage = !!elem.snapshot.data.fullScreen;
                if (path.includes('jabil-bus-screen')) {
                    this.fullPage = true;
                }
            });
            if (!this.canGetData) {
                const path = this.location.path();
                // @ts-ignore
                this.fullPage = ((_d = (_c = (_b = (_a = this.route.snapshot) === null || _a === void 0 ? void 0 : _a.children[0]) === null || _b === void 0 ? void 0 : _b.firstChild) === null || _c === void 0 ? void 0 : _c.data) === null || _d === void 0 ? void 0 : _d.fullScreen) || false;
                if (path.includes('jabil-bus-screen')) {
                    this.fullPage = true;
                }
            }
            this.tempInit();
        });
    }
    // just correct head tab text,do not navigate
    getItemByPath(menu, path) {
        const menuList = menu || this.menuList || [];
        const realPath = path.split('?')[0];
        for (let index = 0; index < menuList.length; index++) {
            if (menuList[index].routerLink === realPath) {
                this.tabClick(menuList[index], true);
                return;
            }
            if (menuList[index].items && menuList[index].items.length > 0) {
                this.getItemByPath(menuList[index].items, path);
            }
        }
    }
    getTabObjByPath(path, list) {
        if (this.currTabObj) {
            return;
        }
        let result = null;
        for (let index = 0; index < list.length; index++) {
            if (list[index].routerLink === path) {
                this.currTabObj = list[index];
            }
            if (list[index].items && list[index].items.length > 0) {
                this.getTabObjByPath(path, list[index].items);
            }
        }
        return result;
    }
    buildSelectedIndex() {
        // 00000000
        const location = this.location.path().split('?')[0];
        let hasTab = false;
        this.selectedIndex = 0;
        this.tabMenu.forEach((item, i) => {
            if ((item === null || item === void 0 ? void 0 : item.routerLink) === location || '/' + (item === null || item === void 0 ? void 0 : item.routerLink) === location) {
                hasTab = true;
                this.selectedIndex = i;
            }
        });
        localStorage.setItem('selectedIndex', JSON.stringify(this.selectedIndex));
        if (this.tabMenu.length > 0 && this.tabs.length > 0 && !hasTab) {
            this.currTabObj = null;
            this.getTabObjByPath(location, this.menuList);
            // modify
            this.tabs.push(this.currTabObj.label + '$' + this.currTabObj.routerLink);
            this.tabMenu.push(this.currTabObj);
            this.selectedIndex = this.tabs.length - 1;
            localStorage.setItem('tabs', JSON.stringify(this.tabs));
            localStorage.setItem('tabMenu', JSON.stringify(this.tabMenu));
        }
    }
    tempInit() {
        this.countries = [
            { name: 'English', code: 'en' },
            { name: '简体中文', code: 'zh' },
        ];
        this.countries.forEach(ele => {
            if (ele.code === this.currentLanguage) {
                this.langDesc = ele.name;
            }
        });
        let tabStr = localStorage.getItem('tabs');
        let tabMenuStr = localStorage.getItem('tabMenu');
        if (tabStr && tabMenuStr) {
            this.tabs = JSON.parse(tabStr);
            this.tabMenu = JSON.parse(tabMenuStr);
        }
        this.buildSelectedIndex();
        // this.selectedIndex = Number(localStorage.getItem('selectedIndex'));
        this.language = [
            {
                label: 'English',
                key: 'en',
                icon: 'fa fa-globe',
                command: (event) => {
                    this.initChangeLang(event.item.key);
                },
            },
            {
                label: '简体中文',
                key: 'zh',
                icon: 'fa fa-globe',
                command: (event) => {
                    this.initChangeLang(event.item.key);
                },
            },
        ];
        if (this.tabMenu.length === 0 && this.tabs.length === 0) {
            this.setTab();
        }
        else {
            this.broadcastIframeUpdate();
        }
        this.initTabScroll();
    }
    setTab() {
        var _a, _b, _c, _d;
        if (((_a = this === null || this === void 0 ? void 0 : this.menuList[0]) === null || _a === void 0 ? void 0 : _a.items) && ((_c = (_b = this === null || this === void 0 ? void 0 : this.menuList[0]) === null || _b === void 0 ? void 0 : _b.items[0]) === null || _c === void 0 ? void 0 : _c.items)) {
            this.tabMenu = [this.menuList[0].items[0].items[0]];
            this.tabs = [this.menuList[0].items[0].items[0].label + '$' + this.menuList[0].items[0].items[0].routerLink];
        }
        else if ((_d = this === null || this === void 0 ? void 0 : this.menuList[0]) === null || _d === void 0 ? void 0 : _d.items) {
            this.tabMenu = [this.menuList[0].items[0]];
            this.tabs = [this.menuList[0].items[0].label + '$' + this.menuList[0].items[0].routerLink];
        }
        else {
            this.tabMenu = [this.menuList[0]];
            this.tabs = [this.menuList[0].label + '$' + this.menuList[0].routerLink];
        }
        localStorage.setItem('tabs', JSON.stringify(this.tabs));
        localStorage.setItem('tabMenu', JSON.stringify(this.tabMenu));
        this.currRouter = this.tabMenu[this.selectedIndex];
    }
    initChangeLang(key) {
        localStorage.setItem('lang', key);
        localStorage.setItem('selectedIndex', '0');
        localStorage.removeItem('tabs');
        localStorage.removeItem('tabMenu');
        const defaultUrl = localStorage.getItem('defaultUrl');
        if (defaultUrl) {
            this.router.navigate([defaultUrl]).then(() => {
                window.location.reload();
            });
        }
        else {
            window.location.reload();
        }
    }
    closeTab(index) {
        const currentTab = this.tabMenu[index].routerLink;
        SimpleReuseStrategy.deleteRouteSnapshot(currentTab);
        this.tabMenu.splice(index, 1);
        this.tabs.splice(index, 1);
        localStorage.setItem('tabs', JSON.stringify(this.tabs));
        localStorage.setItem('tabMenu', JSON.stringify(this.tabMenu));
        if (this.tabs.length === 1) {
            this.selectedIndex = 0;
            this.router.navigate([this.tabMenu[this.selectedIndex].routerLink]);
        }
        else if (this.selectedIndex > this.tabs.length - 1) {
            this.selectedIndex--;
            this.router.navigate([this.tabMenu[this.selectedIndex].routerLink]);
        }
        else if (this.selectedIndex === index) {
            this.selectedIndex = 0;
            this.router.navigate([this.tabMenu[0].routerLink]);
        }
        localStorage.setItem('selectedIndex', this.selectedIndex.toString());
        // this.broadcastIframeUpdate();
        // this.initTabScroll();
    }
    chooseTab(index) {
        this.selectedIndex = index;
        localStorage.setItem('selectedIndex', this.selectedIndex.toString());
        this.broadcastIframeUpdate();
    }
    tabClick(item, noNavigate) {
        var _a;
        // outlink
        const web = ['Doris', 'HUE', 'Dinky', 'Flink', 'Kafka', 'Ketel', 'HDFS', 'Dolphin Scheduler', 'Atlas', 'HMC Website', 'HMC网站'];
        if (web.includes(item.label)) {
            window.open(item.routerLink, '_blank');
            return;
        }
        if (((_a = item === null || item === void 0 ? void 0 : item.items) === null || _a === void 0 ? void 0 : _a.length) > 0) {
            return;
        }
        this.currRouter = item;
        let tabName = item.label;
        // this.tabs.indexOf(tabName) < 0 || 
        if (!this.isTabOpen(item.routerLink, tabName)) {
            this.tabs.push(tabName + '$' + item.routerLink);
            localStorage.setItem('tabs', JSON.stringify(this.tabs));
            this.tabMenu.push(item);
            localStorage.setItem('tabMenu', JSON.stringify(this.tabMenu));
            this.selectedIndex = this.tabs.length - 1;
            localStorage.setItem('selectedIndex', this.selectedIndex.toString());
        }
        else {
            let that = this;
            this.tabs.forEach((ele, index) => {
                let link = (ele === null || ele === void 0 ? void 0 : ele.split('$')[1]) || '';
                if (link === item.routerLink || ele === tabName) {
                    that.selectedIndex = index;
                    localStorage.setItem('selectedIndex', that.selectedIndex.toString());
                }
            });
        }
        this.broadcastIframeUpdate();
        this.initTabScroll();
    }
    isTabOpen(routerLink, tabName) {
        let isOpen = false;
        this.tabs.forEach((ele, index) => {
            let link = (ele === null || ele === void 0 ? void 0 : ele.split('$')[1]) || '';
            if (link === routerLink || ele === tabName) {
                isOpen = true;
            }
        });
        return isOpen;
    }
    logoff() {
        var _a, _b, _c;
        const iframe = document.getElementsByTagName('iframe');
        (iframe === null || iframe === void 0 ? void 0 : iframe.length) > 0 && ((_c = (_b = (_a = iframe[0]) === null || _a === void 0 ? void 0 : _a.contentWindow) === null || _b === void 0 ? void 0 : _b.location) === null || _c === void 0 ? void 0 : _c.reload());
        const removeItems = [
            'loginCheck',
            'loginType',
            'selectedIndex',
            'jwt',
            'tabs',
            'tabMenu',
            'region',
            'currentRegion',
            'username',
            'defaultUrl',
            'roles',
            'translateApp',
            'zh',
            'en'
        ];
        removeItems.forEach((item) => {
            localStorage.removeItem(item);
        });
        const langCacheKey = localStorage.getItem('langCacheKey');
        localStorage.removeItem('' + langCacheKey);
        window.location.href = '/';
    }
    fullScreen() {
        this.isFullScreen = true;
        const el = document.body;
        el.requestFullscreen();
    }
    exitFullScreen() {
        this.isFullScreen = false;
        document.exitFullscreen();
    }
    editUserEvent() {
        this.editUser.emit();
    }
    broadcastIframeUpdate() {
        var _a, _b;
        const item = this.tabMenu[this.selectedIndex];
        // @ts-ignore
        if ((item === null || item === void 0 ? void 0 : item.isWebApp) && (window === null || window === void 0 ? void 0 : window.router)) {
            if (this.location.path().includes('/jabil-bus-screen')) {
                // @ts-ignore
                (_a = window === null || window === void 0 ? void 0 : window.router) === null || _a === void 0 ? void 0 : _a.navigate([item.routerLink]);
                return;
            }
            let url = '';
            if (this.firstJoin) {
                const params = this.location.path().split('?')[1];
                // url = params? item.routerLink + encodeURIComponent('?' + params) : item.routerLink
                url = params ? item.routerLink + '?' + params : item.routerLink;
                this.firstJoin = false;
            }
            else {
                url = item.routerLink;
            }
            // @ts-ignore
            (_b = window === null || window === void 0 ? void 0 : window.router) === null || _b === void 0 ? void 0 : _b.navigate([url]).then(() => {
                window.dispatchEvent(new CustomEvent('updateAppUrl', {
                    detail: { path: item.routerPath },
                }));
                // @ts-ignore
                // const childWindow: any = document.getElementById('jabil-bus').contentWindow;
                // if (childWindow && !this.location.path().includes('/jabil-bus-screen')) {
                // document.domain = 'localhost';
                // childWindow.dispatchEvent(
                //   new CustomEvent('updateAppUrl', {
                //     detail: { path: item.routerPath },
                //   })
                // );
                //   window.dispatchEvent(
                //     new CustomEvent('updateAppUrl', {
                //       detail: { path: item.routerPath },
                //     })
                //   );
                // }
            });
        }
    }
    initTabScroll() {
        setTimeout(() => {
            const myTab = document.getElementById('myTab');
            myTab === null || myTab === void 0 ? void 0 : myTab.scrollTo(1, 0);
            // @ts-ignore
            this.hasScroll = (myTab === null || myTab === void 0 ? void 0 : myTab.scrollLeft) > 0;
            myTab === null || myTab === void 0 ? void 0 : myTab.scrollTo(0, 0);
        }, 500);
    }
    scroll(type) {
        const element = document.getElementById('myTab');
        // @ts-ignore
        const step = type === 'left' ? (element === null || element === void 0 ? void 0 : element.scrollLeft) - 500 : (element === null || element === void 0 ? void 0 : element.scrollLeft) + 500;
        element === null || element === void 0 ? void 0 : element.scrollTo(step, 0);
    }
    closeAllTab(e) {
        return __awaiter(this, void 0, void 0, function* () {
            const tabsStr = localStorage.getItem('tabs');
            const tabMenuStr = localStorage.getItem('tabMenu');
            if (!tabsStr || !tabMenuStr) {
                return;
            }
            const homeTab = JSON.parse(tabsStr)[0];
            const homeTabMenu = JSON.parse(tabMenuStr)[0];
            localStorage.setItem('tabs', JSON.stringify([homeTab]));
            localStorage.setItem('tabMenu', JSON.stringify([homeTabMenu]));
            localStorage.setItem('selectedIndex', '0');
            this.tabMenu = [homeTabMenu];
            this.tabs = [homeTab];
            this.selectedIndex = 0;
            this.router.navigate([homeTabMenu.routerLink]);
            this.broadcastIframeUpdate();
            this.initTabScroll();
        });
    }
    closeOtherTab(e) {
        return __awaiter(this, void 0, void 0, function* () {
            if (this.contextMenuIndex === 0) {
                this.closeAllTab(e);
                return;
            }
            const tabsStr = localStorage.getItem('tabs');
            const tabMenuStr = localStorage.getItem('tabMenu');
            if (!tabsStr || !tabMenuStr) {
                return;
            }
            const homeTab = JSON.parse(tabsStr)[0];
            const homeTabMenu = JSON.parse(tabMenuStr)[0];
            const tab = JSON.parse(tabsStr)[this.contextMenuIndex];
            const tabMenu = JSON.parse(tabMenuStr)[this.contextMenuIndex];
            localStorage.setItem('tabs', JSON.stringify([homeTab, tab]));
            localStorage.setItem('tabMenu', JSON.stringify([homeTabMenu, tabMenu]));
            localStorage.setItem('selectedIndex', '1');
            this.tabMenu = [homeTabMenu, tabMenu];
            this.tabs = [homeTab, tab];
            this.selectedIndex = 1;
            this.router.navigate([tabMenu.routerLink]);
            this.broadcastIframeUpdate();
            this.initTabScroll();
        });
    }
    contextMenu(tab, index) {
        this.contextMenuTab = tab;
        this.contextMenuIndex = index;
    }
}
LayoutComponent.ɵfac = function LayoutComponent_Factory(t) { return new (t || LayoutComponent)(i0.ɵɵdirectiveInject(i1$2.TranslateService), i0.ɵɵdirectiveInject(i2$2.Router), i0.ɵɵdirectiveInject(i2$2.ActivatedRoute), i0.ɵɵdirectiveInject(i0.ChangeDetectorRef), i0.ɵɵdirectiveInject(AppMenuService), i0.ɵɵdirectiveInject(HttpService), i0.ɵɵdirectiveInject(i5$1.Location)); };
LayoutComponent.ɵcmp = /*@__PURE__*/ i0.ɵɵdefineComponent({ type: LayoutComponent, selectors: [["app-layout"]], inputs: { menuTreeKeys: "menuTreeKeys", menuTree: "menuTree" }, outputs: { editUser: "editUser" }, features: [i0.ɵɵProvidersFeature([AppMenuService, HttpService, AppMenuService, SimpleReuseStrategy])], decls: 13, vars: 10, consts: [[1, "layout", 2, "height", "100%", "width", "100%"], [3, "class", 4, "ngIf"], [1, "w-100", "h-100"], ["class", "titlebar", 4, "ngIf"], [1, "inner-content"], ["id", "headerTab", 1, "flex", "flex-row", 2, "position", "relative"], ["style", "position: absolute; left: 0; height: 34px", "class", "arrow cursor-pointer", 3, "click", 4, "ngIf"], [4, "ngIf"], ["style", "position: absolute; right: 0; height: 34px", "class", "arrow cursor-pointer", 3, "click", 4, "ngIf"], [1, "main-content"], ["style", "position: absolute;width: 100%;height: 100%;", 4, "ngIf"], [1, "logo"], ["alt", "", 3, "src", 4, "ngIf"], [3, "model", "style", 4, "ngIf"], ["class", "position-fixed", "styleClass", "jb-menu", "appendTo", "body", 3, "autoDisplay", "model", "style", 4, "ngIf"], ["alt", "", 3, "src"], [3, "model"], ["styleClass", "jb-menu", "appendTo", "body", 1, "position-fixed", 3, "autoDisplay", "model"], [1, "titlebar"], [1, "flexrow", "alc", "jsfs"], ["alt", "", 3, "src", "click"], [1, "currTab"], [1, "flexrow", "alc", "jste", 2, "padding-right", "0.5em"], ["alt", "", 3, "src", "click", 4, "ngIf"], [2, "margin-right", "30px", "cursor", "pointer", 3, "click"], ["alt", "", 2, "margin-right", "2px", "width", "32px", "height", "32px", "border-radius", "50%", "transform", "rotate(0deg)", 3, "src"], [2, "cursor", "pointer", "margin-right", "24px", 3, "click"], [1, "fa", "fa-globe"], ["styleClass", "language", "appendTo", "body", 3, "model", "popup"], ["lang", ""], [1, "actionitem", 2, "cursor", "pointer", 3, "click"], ["aria-hidden", "true", 1, "fa", "fa-power-off"], ["styleClass", "language", 3, "popup"], ["menu", ""], [1, "arrow", "cursor-pointer", 2, "position", "absolute", "left", "0", "height", "34px", 3, "click"], [1, "pi", "pi-chevron-left"], ["id", "myTab", "role", "tablist", 1, "nav", "nav-tabs", "flex-nowrap", "white-space-nowrap", "overflow-y-auto"], ["headerMenu", ""], ["class", "nav-item", 3, "contextmenu", 4, "ngFor", "ngForOf"], ["appendTo", "body", 3, "target", "model", "onShow", "onHide", 4, "ngIf"], [1, "nav-item", 3, "contextmenu"], ["class", "nav-link", "data-toggle", "tab", "role", "tab", "aria-selected", "true", 3, "class", 4, "ngIf"], ["data-toggle", "tab", "role", "tab", "aria-selected", "true", 1, "nav-link"], [3, "routerLink", "click", 4, "ngIf"], ["class", "fa fa-times", "style", "cursor: pointer", "aria-hidden", "true", 3, "click", 4, "ngIf"], [3, "routerLink", "click"], ["aria-hidden", "true", 1, "fa", "fa-times", 2, "cursor", "pointer", 3, "click"], ["appendTo", "body", 3, "target", "model", "onShow", "onHide"], [1, "arrow", "cursor-pointer", 2, "position", "absolute", "right", "0", "height", "34px", 3, "click"], [1, "pi", "pi-chevron-right"], [2, "position", "absolute", "width", "100%", "height", "100%"]], template: function LayoutComponent_Template(rf, ctx) {
        if (rf & 1) {
            i0.ɵɵelementStart(0, "div", 0);
            i0.ɵɵtemplate(1, LayoutComponent_div_1_Template, 6, 6, "div", 1);
            i0.ɵɵelementStart(2, "div", 2)(3, "div");
            i0.ɵɵtemplate(4, LayoutComponent_div_4_Template, 27, 15, "div", 3);
            i0.ɵɵelementStart(5, "div", 4)(6, "div", 5);
            i0.ɵɵtemplate(7, LayoutComponent_div_7_Template, 2, 0, "div", 6);
            i0.ɵɵtemplate(8, LayoutComponent_ng_container_8_Template, 5, 6, "ng-container", 7);
            i0.ɵɵtemplate(9, LayoutComponent_div_9_Template, 2, 0, "div", 8);
            i0.ɵɵelementEnd();
            i0.ɵɵelementStart(10, "div", 9);
            i0.ɵɵtemplate(11, LayoutComponent_div_11_Template, 1, 0, "div", 10);
            i0.ɵɵelement(12, "router-outlet");
            i0.ɵɵelementEnd()()()()();
        }
        if (rf & 2) {
            i0.ɵɵadvance(1);
            i0.ɵɵproperty("ngIf", !ctx.fullPage);
            i0.ɵɵadvance(1);
            i0.ɵɵclassMap(ctx.fullPage ? "no-style" : "");
            i0.ɵɵadvance(1);
            i0.ɵɵclassMap(ctx.isCollapsed ? "collapsed mainscontent" : "mainscontent");
            i0.ɵɵadvance(1);
            i0.ɵɵproperty("ngIf", !ctx.fullPage);
            i0.ɵɵadvance(3);
            i0.ɵɵproperty("ngIf", ctx.hasScroll);
            i0.ɵɵadvance(1);
            i0.ɵɵproperty("ngIf", !ctx.fullPage);
            i0.ɵɵadvance(1);
            i0.ɵɵproperty("ngIf", ctx.hasScroll);
            i0.ɵɵadvance(2);
            i0.ɵɵproperty("ngIf", !ctx.fullPage && ctx.showContextMenu);
        }
    }, dependencies: [i5$1.NgForOf, i5$1.NgIf, i6$1.PanelMenu, i2$2.RouterOutlet, i2$2.RouterLinkWithHref, i7.TieredMenu, i8.ContextMenu], styles: ["[_nghost-%COMP%]{overflow:hidden}[_nghost-%COMP%]   .actionitem[_ngcontent-%COMP%]{display:flex;align-items:center;justify-content:center;font-size:1rem;font-weight:500;width:auto;padding-left:.5em;padding-right:.5em}[_nghost-%COMP%]   .mainscontent[_ngcontent-%COMP%]{display:flex;flex-direction:column;height:100%;width:100%;padding-left:240px}[_nghost-%COMP%]   .nav-tabs[_ngcontent-%COMP%]   .nav-link.active[_ngcontent-%COMP%]{background:#147ad9}[_nghost-%COMP%]   .nav-tabs[_ngcontent-%COMP%]   .nav-link.active[_ngcontent-%COMP%]   a[_ngcontent-%COMP%], [_nghost-%COMP%]   .nav-tabs[_ngcontent-%COMP%]   .nav-link.active[_ngcontent-%COMP%]   i[_ngcontent-%COMP%]{color:#fff}[_nghost-%COMP%]   .nav-link[_ngcontent-%COMP%]{font-weight:400}[_nghost-%COMP%]   .active[_ngcontent-%COMP%]{font-weight:700}[_nghost-%COMP%]   .inner-content[_ngcontent-%COMP%]{height:100%;background:#f0f2f5}[_nghost-%COMP%]   .main-content[_ngcontent-%COMP%]{height:calc(100% - 35px);padding:5px 24px 24px}[_nghost-%COMP%]   .siderbar[_ngcontent-%COMP%]{width:240px;background:#fff;position:fixed;top:0;left:0;z-index:999;overflow-y:auto;height:100%}[_nghost-%COMP%]   .siderbar[_ngcontent-%COMP%]   .logo[_ngcontent-%COMP%]{height:110px;width:208px;display:flex;justify-content:center;align-items:center;border-bottom:1px solid #f0f0f0;margin:0 auto 32px}[_nghost-%COMP%]   .siderbar[_ngcontent-%COMP%]   .logo[_ngcontent-%COMP%]   img[_ngcontent-%COMP%]{width:176px;height:44px}[_nghost-%COMP%]   .titlebar[_ngcontent-%COMP%]{height:50px;width:100%;display:flex;flex-direction:row;align-items:center;justify-content:space-between}[_nghost-%COMP%]   .titlebar[_ngcontent-%COMP%]   img[_ngcontent-%COMP%]{width:20px;height:20px;margin-left:24px;margin-right:16px;cursor:pointer}[_nghost-%COMP%]   .titlebar[_ngcontent-%COMP%]   .currTab[_ngcontent-%COMP%]{font-size:16px;font-weight:500}[_nghost-%COMP%]   .collapsed.mainscontent[_ngcontent-%COMP%]{padding-left:50px}[_nghost-%COMP%]   .collapsed.mainscontent[_ngcontent-%COMP%]   .titlebar[_ngcontent-%COMP%]   img[_ngcontent-%COMP%]{transform:rotate(180deg)}[_nghost-%COMP%]   .collapsed.siderbar[_ngcontent-%COMP%]{width:50px}[_nghost-%COMP%]   .collapsed.siderbar[_ngcontent-%COMP%]   .logo[_ngcontent-%COMP%]{width:50px;margin-bottom:10px}[_nghost-%COMP%]   .collapsed.siderbar[_ngcontent-%COMP%]   .logo[_ngcontent-%COMP%]   img[_ngcontent-%COMP%]{width:48px;height:48px}[_nghost-%COMP%]   .collapsed.siderbar[_ngcontent-%COMP%]     .p-panelmenu .p-panelmenu-header>a{width:100%;height:56px;border-radius:unset!important;display:flex;justify-content:center;align-items:center}[_nghost-%COMP%]   .collapsed.siderbar[_ngcontent-%COMP%]     .p-panelmenu .p-panelmenu-header>a span{position:relative;right:20px}[_nghost-%COMP%]   .collapsed.siderbar[_ngcontent-%COMP%]     .p-panelmenu .p-panelmenu-header>a .p-menuitem-text{display:none}[_nghost-%COMP%]   .collapsed[_ngcontent-%COMP%]     .p-panelmenu-panel .p-submenu-expanded a{width:65px;justify-content:center;margin-left:0}[_nghost-%COMP%]   .collapsed[_ngcontent-%COMP%]     .p-panelmenu-panel a .pi-chevron-right, [_nghost-%COMP%]   .collapsed[_ngcontent-%COMP%]     .p-panelmenu-panel a .pi-chevron-down, [_nghost-%COMP%]   .collapsed[_ngcontent-%COMP%]     .p-panelmenu-panel a .p-menuitem-text{display:none}[_nghost-%COMP%]   .collapsed[_ngcontent-%COMP%]     .p-panelmenu-panel a .p-menu-icon{position:unset!important}[_nghost-%COMP%]   .dialogview[_ngcontent-%COMP%]{display:flex;flex-direction:row;width:60vw;flex-wrap:wrap}[_nghost-%COMP%]   .dialog-item[_ngcontent-%COMP%]{width:50%;padding-bottom:2rem}[_nghost-%COMP%]   .dialog-item-name[_ngcontent-%COMP%]{font-size:1.5rem;font-weight:700;text-decoration:underline}[_nghost-%COMP%]   .dialog-item-email[_ngcontent-%COMP%]{word-break:break-all}[_nghost-%COMP%]   p-tieredmenusub[_ngcontent-%COMP%]{position:fixed}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-content{border:none!important}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-header-link .p-menuitem-icon{right:0!important}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu .p-panelmenu-header>a{background:transparent;border:none!important;font-size:16px!important;font-weight:700!important;border-radius:8px!important;width:208px;color:#999!important;margin:0 auto}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu .p-panelmenu-header>a .p-menuitem-icon{display:inline-block;width:24px;height:24px;background-size:contain}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-submenu-list .p-menuitem{padding-left:8px}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-submenu-list .p-menuitem a.p-menuitem-link{width:198px}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel .p-submenu-expanded{padding-left:10px}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link{background:transparent!important;border:none!important;font-size:16px!important;font-weight:700!important;border-radius:8px!important;width:208px;margin:0 auto 16px}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link .p-menuitem-text{color:#999!important}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link .p-menuitem-link-active .p-menuitem-text{color:#fff!important}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a{margin-bottom:16px}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon{display:inline-block;width:24px;height:24px;min-width:24px;background-size:100% 100%;background-repeat:no-repeat}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon[ng-reflect-ng-class=\"fa fa\"]{background-image:url(../../../assets/images/teslaOutputTracking/menu-monitor.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.system{background-image:url(../../../assets/images/menu/system/system.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.systemUser{background-image:url(../../../assets/images/menu/system/system-user.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.systemMenu{background-image:url(../../../assets/images/menu/tesla/tesla-report.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.systemRole{background-image:url(../../../assets/images/menu/system/system-role.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.systemDataDictionary{background-image:url(../../../assets/images/menu/system/system-dictionary.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.systemAnnouncement{background-image:url(../../../assets/images/menu/mfg/mfg-announce.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.tesla{background-image:url(../../../assets/images/menu/tesla/tesla.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.teslaMonitor{background-image:url(../../../assets/images/menu/tesla/tesla-monitor.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.teslaReport{background-image:url(../../../assets/images/menu/tesla/tesla-report.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.teslaConfig{background-image:url(../../../assets/images/menu/tesla/tesla-config.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.teslaInfor{background-image:url(../../../assets/images/menu/tesla/tesla-info.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.teslaAutoBuildPlan{background-image:url(../../../assets/images/menu/tesla/tesla-auto-build-plan.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.teslaOrder{background-image:url(../../../assets/images/menu/tesla/tesla-order.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.teslaPlan{background-image:url(../../../assets/images/menu/tesla/tesla-plan.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.teslaPlanMonitoring{background-image:url(../../../assets/images/menu/tesla/tesla-plan-monitoring.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.test{background-image:url(../../../assets/images/menu/testValidation/test.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.testDashboard{background-image:url(../../../assets/images/menu/testValidation/test-dashboard.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.testMyCase{background-image:url(../../../assets/images/menu/testValidation/test-my-case.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.testCaseCenter{background-image:url(../../../assets/images/menu/testValidation/test-case-center.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.testMessage{background-image:url(../../../assets/images/menu/testValidation/test-message.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.testMainData{background-image:url(../../../assets/images/menu/testValidation/test-main-data.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.testCookbook{background-image:url(../../../assets/images/menu/testValidation/test-cook-book.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.testCookbookLabel{background-image:url(../../../assets/images/menu/testValidation/test-cook-book-label.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.escs{background-image:url(../../../assets/images/menu/escs/escs.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.escsHome{background-image:url(../../../assets/images/menu/escs/escs-home.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.mfg{background-image:url(../../../assets/images/menu/mfg/mfg.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.mfgDashboard{background-image:url(../../../assets/images/menu/mfg/mfg-dashboard.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.mfgTraining{background-image:url(../../../assets/images/menu/mfg/mfg-training.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.mfgAnnounce{background-image:url(../../../assets/images/menu/mfg/mfg-announce.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.CourseWare{background-image:url(../../../assets/images/menu/mfg/CourseWare.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.ProactiveCare{background-image:url(../../../assets/images/menu/mfg/ProactiveCare.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.PWTBonus{background-image:url(../../../assets/images/menu/mfg/PWTBonus.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.LineLeaderBonus{background-image:url(../../../assets/images/menu/mfg/LineLeaderBonus.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.SuperDLBonus{background-image:url(../../../assets/images/menu/mfg/LineLeaderBonus.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.SuperLLBonus{background-image:url(../../../assets/images/menu/mfg/SuperLLBonus.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.DLPerformance{background-image:url(../../../assets/images/menu/mfg/DL-Performance.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.DLPWTSource{background-image:url(../../../assets/images/menu/mfg/DLPWTSource.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.DLPWTReport{background-image:url(../../../assets/images/menu/mfg/DLPWTReport.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.NewSystemAnnouncement{background-image:url(../../../assets/images/menu/mfg/SystemAnnouncement.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.mfgCourseWare{background-image:url(../../../assets/images/menu/mfg/mfg-course-ware.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.mfgLearning{background-image:url(../../../assets/images/menu/mfg/mfg-learning.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.mfgQuestionBank{background-image:url(../../../assets/images/menu/mfg/mfg-question-bank.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.mfgTest{background-image:url(../../../assets/images/menu/mfg/mfg-test.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.mfgStatistics{background-image:url(../../../assets/images/menu/mfg/mfg-report.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.mfgEmployee{background-image:url(../../../assets/images/menu/mfg/mfg-employee.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.mfgCareRecord{background-image:url(../../../assets/images/menu/mfg/mfg-care-record.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.mfgInterview{background-image:url(../../../assets/images/menu/mfg/mfg-interview.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.mfgInterviewReport{background-image:url(../../../assets/images/menu/mfg/mfg-interview-report.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.mfgEmployeeAbnormal{background-image:url(../../../assets/images/menu/mfg/mfg-employee-abnormal.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.mfgFeedback{background-image:url(../../../assets/images/menu/mfg/mfg-feedback.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.mfgActiveSummary{background-image:url(../../../assets/images/menu/mfg/mfg-active-summary.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.mfgActiveDetail{background-image:url(../../../assets/images/menu/mfg/mfg-active-detail.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.mfgEBuddy{background-image:url(../../../assets/images/menu/mfg/mfg-e-buddy.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.mfgEBuddyNewGuide{background-image:url(../../../assets/images/menu/mfg/mfg-new-guidance.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.mfgEBuddyFrequentlyQuestion{background-image:url(../../../assets/images/menu/mfg/mfg-frequently-question.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.mfgEBuddyConsultation{background-image:url(../../../assets/images/menu/mfg/mfg-consultation.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.mfgPerformance{background-image:url(../../../assets/images/menu/mfg/mfg-performance.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.mfgPerformancePWT{background-image:url(../../../assets/images/menu/mfg/mfg-PWT-calculation.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.mfgPerformancePWTResult{background-image:url(../../../assets/images/menu/mfg/mfg-PWT-calculation-result.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.mfgPWTTeamManage{background-image:url(../../../assets/images/menu/mfg/mfg-PWT-team-manage.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.ThingsLinkerBroker{background-image:url(../../../assets/images/menu/thingsLinker/broker.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.ThingsLinkerEquipment{background-image:url(../../../assets/images/menu/thingsLinker/equipment.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.ThingsLinkerBooking{background-image:url(../../../assets/images/menu/thingsLinker/booking.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.ThingsLinkerDispatchOrder{background-image:url(../../../assets/images/menu/thingsLinker/order.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.ThingsLinkerConfig{background-image:url(../../../assets/images/menu/tesla/tesla-config.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.ThingsLinker{background-image:url(../../../assets/images/menu/thingsLinker/things-linker.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.PCNConfig{background-image:url(../../../assets/images/menu/pcn/code-config.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.PCNCodeParameter{background-image:url(../../../assets/images/menu/pcn/code-parameter.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.PCNCodeStage{background-image:url(../../../assets/images/menu/pcn/code-stage.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.PCNMasterTemplate{background-image:url(../../../assets/images/menu/pcn/master-template.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.PCNCodeTemplate{background-image:url(../../../assets/images/menu/pcn/code-template.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.PCNCodeBuild{background-image:url(../../../assets/images/menu/pcn/code-build.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.PCNMachine{background-image:url(../../../assets/images/menu/pcn/machine-control.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.PCNMachineInfo{background-image:url(../../../assets/images/menu/pcn/machine-info.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.PCNWorkflow{background-image:url(../../../assets/images/menu/pcn/workflow.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.PCNMachineModel{background-image:url(../../../assets/images/menu/pcn/machine-model.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.PCNMachineChange{background-image:url(../../../assets/images/menu/pcn/machine-change.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.PCNWorkcellCodeConfig{background-image:url(../../../assets/images/menu/pcn/work-cell-config.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.PCNDefaultApprover{background-image:url(../../../assets/images/menu/pcn/default-approver.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.PCNNPI{background-image:url(../../../assets/images/menu/pcn/npi.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.PCNSOP{background-image:url(../../../assets/images/menu/pcn/sop.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.PCN{background-image:url(../../../assets/images/menu/pcn/pcn.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.Home{background-image:url(../../../assets/images/menu/home/home.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.Demo{background-image:url(../../../assets/images/demo/menu/demo.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.WIKIHome{background-image:url(../../../assets/images/menu/wiki/wiki-home.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.WIKIBackground{background-image:url(../../../assets/images/menu/wiki/wiki-config.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.WIKIDashboard{background-image:url(../../../assets/images/menu/wiki/wiki-dashboard.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.WIKITopic{background-image:url(../../../assets/images/menu/wiki/wiki-topic.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.WIKIMyTopic{background-image:url(../../../assets/images/menu/wiki/wiki-my-topic.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.WIKIComment{background-image:url(../../../assets/images/menu/wiki/wiki-comment.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.WIKIMyComment{background-image:url(../../../assets/images/menu/wiki/wiki-my-comment.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.WIKI{background-image:url(../../../assets/images/menu/wiki/wiki.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.JiTBuildPlan{background-image:url(../../../assets/images/menu/jit/jit-build-plan.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.JITDeliveryBooking{background-image:url(../../../assets/images/menu/jit/jit-delivery-booking.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.JITPullListKanban{background-image:url(../../../assets/images/menu/jit/jit-pull-list-kanban.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.JITSIgnFor{background-image:url(../../../assets/images/menu/jit/jit-sign-for.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.JITBuildPlanManagement{background-image:url(../../../assets/images/menu/jit/jit-build-plan-manage.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.JITSubmitBuildPlan{background-image:url(../../../assets/images/menu/jit/jit-submit-build-plan.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.JITShortageList{background-image:url(../../../assets/images/menu/jit/jit-shortage-list.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.JITDemandReport{background-image:url(../../../assets/images/menu/jit/jit-demand-report.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.JITRemainingInventory{background-image:url(../../../assets/images/menu/jit/jit-remaining-inventory.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.JITBaseData{background-image:url(../../../assets/images/menu/jit/jit-base-data.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.JITDummyBOM{background-image:url(../../../assets/images/menu/jit/jit-dummy-bom.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.JITUPH{background-image:url(../../../assets/images/menu/jit/jit-uph.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.JITModelFrequency{background-image:url(../../../assets/images/menu/jit/jit-model-frequency.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.JITAGVLineInfo{background-image:url(../../../assets/images/menu/jit/jit-agv-line-info.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.JITSPQ{background-image:url(../../../assets/images/menu/jit/jit-spq.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.JITDeliveryLeadTime{background-image:url(../../../assets/images/menu/jit/jit-delivery-lead-time.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.JITStorageLocation{background-image:url(../../../assets/images/menu/jit/jit-storage-location.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.JITDeliveryTaskControl{background-image:url(../../../assets/images/menu/jit/jit-delivery-task-control.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.JITBuyerNameByPart{background-image:url(../../../assets/images/menu/jit/jit-buyer-name-by-part.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.JITWarehouseManagement{background-image:url(../../../assets/images/menu/jit/jit-warehouse-management.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.JITWarehouseDelivery{background-image:url(../../../assets/images/menu/jit/jit-warehouse-delivery.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.JITInventoryManagement{background-image:url(../../../assets/images/menu/jit/jit-inventory-management.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.JITInventoryBin{background-image:url(../../../assets/images/menu/jit/jit-inventory-bin.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.MesStream{background-image:url(../../../assets/images/menu/mesStream/mes-stream.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.MesStreamConfig{background-image:url(../../../assets/images/menu/mesStream/mes-stream-config.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.MesStreamWip{background-image:url(../../../assets/images/menu/mesStream/mes-stream-wip.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.FlowChart{background-image:url(../../../assets/images/menu/flowchart/flowchart.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.MFGGOGS{background-image:url(../../../assets/images/menu/mfggogs/mfggogs.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.QA{background-image:url(../../../assets/images/menu/qa/qa.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.ESCSExtendedSystem{background-image:url(../../../assets/images/menu/escs/escsextendedsystem.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.Quotation{background-image:url(../../../assets/images/menu/quotation/quotation.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.Machine{background-image:url(../../../assets/images/menu/machine/machine.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.Setup{background-image:url(../../../assets/images/menu/setup/setup.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.JMJagentMonitoring{background-image:url(../../../assets/images/menu/jmjagentmonitoring/jmjagentmonitoring.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.WorkflowConfig{background-image:url(../../../assets/images/menu/workflowConfig/workflow-config.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.WorkflowTeslaConfig{background-image:url(../../../assets/images/menu/workflowConfig/workflow-tesla-config.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.Sediot{background-image:url(../../../assets/images/menu/sediot/sediot.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.SediotCycleTime{background-image:url(../../../assets/images/menu/sediot/sediot-cycle-time.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.SediotProcessReport{background-image:url(../../../assets/images/menu/sediot/sediot-process-report.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.SediotEmailConfig{background-image:url(../../../assets/images/menu/sediot/sediot-email-config.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.SediotUtilizationReport{background-image:url(../../../assets/images/menu/sediot/sediot-utilization-report.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.SediotPMAlert{background-image:url(../../../assets/images/menu/sediot/sediot-pm-alert.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.SediotLessonLearn{background-image:url(../../../assets/images/menu/sediot/sediot-lesson-learn.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.SediotLayout{background-image:url(../../../assets/images/menu/sediot/sediot-layout.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.OC{background-image:url(../../../assets/images/menu/oc/oc.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.RealTime{background-image:url(../../../assets/images/menu/escs/realtime.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.ProcessAnalysis{background-image:url(../../../assets/images/menu/escs/ProcessAnalysis.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.DataLake{background-image:url(../../../assets/images/menu/dataLake/data-lake.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.Atlas{background-image:url(../../../assets/images/menu/dataLake/Atlas.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.Dinky{background-image:url(../../../assets/images/menu/dataLake/Dinky.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.DolphinScheduler{background-image:url(../../../assets/images/menu/dataLake/DolphinScheduler.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.Doris{background-image:url(../../../assets/images/menu/dataLake/Doris.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.Flink{background-image:url(../../../assets/images/menu/dataLake/Flink.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.HDFS{background-image:url(../../../assets/images/menu/dataLake/HDFS.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.HUE{background-image:url(../../../assets/images/menu/dataLake/HUE.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.Kafka{background-image:url(../../../assets/images/menu/dataLake/Kafka.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.Ketel{background-image:url(../../../assets/images/menu/dataLake/Ketel.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.Portal{background-image:url(../../../assets/images/menu/dataLake/Portal.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.DataTransport{background-image:url(../../../assets/images/menu/dataLake/data-transport.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.DataTraceabilityPortal{background-image:url(../../../assets/images/menu/dataLake/data-lake-portal.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.DataTraceabilityOther{background-image:url(../../../assets/images/menu/dataLake/data-lake-other.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.AOS{background-image:url(../../../assets/images/menu/aos/aos.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.AOSCheckList{background-image:url(../../../assets/images/menu/aos/aos-check-list.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.AOSAuditPlan{background-image:url(../../../assets/images/menu/aos/aos-audit-plan.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.AOSTaskList{background-image:url(../../../assets/images/menu/aos/aos-task-list.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.BMWIOT{background-image:url(../../../assets/images/menu/bmwiot/bmwiot.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.BMWIOTDashboard{background-image:url(../../../assets/images/menu/bmwiot/Dashboard.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.BMWIOTCCUProductionKanban{background-image:url(../../../assets/images/menu/bmwiot/CCUProductionKanban.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.BMWIOTProductionAndTest{background-image:url(../../../assets/images/menu/bmwiot/ProductionAndTest.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.BMWIOTEquipmentAlarm{background-image:url(../../../assets/images/menu/bmwiot/EquipmentAlarm.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.BMWIOTChangeInformation{background-image:url(../../../assets/images/menu/bmwiot/ChangeInformation.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.BMWIOTEquipmentData{background-image:url(../../../assets/images/menu/bmwiot/EquipmentData.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.MasterDataLogo{background-image:url(../../../assets/images/menu/masterdata/masterData-logo.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.MasterDataHomePage{background-image:url(../../../assets/images/menu/masterdata/masterData-homepage.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.MasterDataUpdateRecordQuery{background-image:url(../../../assets/images/menu/masterdata/masterData-updaterecordquery.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.MasterDataProdictionarea{background-image:url(../../../assets/images/menu/masterdata/masterData-prodictionarea.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.MasterDataBay{background-image:url(../../../assets/images/menu/masterdata/masterData-bay.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.MasterDataEmployeen{background-image:url(../../../assets/images/menu/masterdata/masterData-employeen.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.MasterDataEmployeenQuery{background-image:url(../../../assets/images/menu/masterdata/masterData-employeenquery.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.MasterDataOperationConfiguration{background-image:url(../../../assets/images/menu/masterdata/masterData-OperationConfiguration.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.MasterDataHUAID{background-image:url(../../../assets/images/menu/masterdata/masterData-HUAID.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.MasterDataEquipment{background-image:url(../../../assets/images/menu/masterdata/masterData-equipment.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.MaterDataFunction{background-image:url(../../../assets/images/menu/masterdata/masterData-function.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.MasterDataLocation{background-image:url(../../../assets/images/menu/masterdata/masterData-location.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.MaterDataOrganization{background-image:url(../../../assets/images/menu/masterdata/masterData-organization.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.MasterDataManagement{background-image:url(../../../assets/images/menu/masterdata/masterdata.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.CostCenter{background-image:url(../../../assets/images/menu/masterdata/masterData-costcenter.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.dataassetLogo{background-image:url(../../../assets/images/menu/dataasset/dataasset.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.dataassetHome{background-image:url(../../../assets/images/menu/dataasset/dataassetHome.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.dataassetAssetquery{background-image:url(../../../assets/images/menu/dataasset/assetquery.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.dataassetDataAuthorized{background-image:url(../../../assets/images/menu/dataasset/dataAuthorized.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.dataassetDatasource{background-image:url(../../../assets/images/menu/dataasset/datasource.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.dataassetDatatable{background-image:url(../../../assets/images/menu/dataasset/datatable.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.dataasseTopiccategory{background-image:url(../../../assets/images/menu/dataasset/topiccategory.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.dataassetDataapi{background-image:url(../../../assets/images/menu/dataasset/dataapi.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.dataassetDataapilog{background-image:url(../../../assets/images/menu/dataasset/dataapilog.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.dataassetSqlquery{background-image:url(../../../assets/images/menu/dataasset/sqlquery.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.dataassetDataauthority{background-image:url(../../../assets/images/menu/dataasset/dataauthority.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.dataassetDataauthorityquery{background-image:url(../../../assets/images/menu/dataasset/dataauthorityquery.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.dataassetDataUserauthorityquery{background-image:url(../../../assets/images/menu/dataasset/dataUserauthorityquery.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.SMServerMonitoring{background-image:url(../../../assets/images/menu/serverMonitoring/server-monitoring.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.HOSAppWidget{background-image:url(../../../assets/images/menu/hos/hos-app-widget.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.HOSTemplate{background-image:url(../../../assets/images/menu/hos/hos-template.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.HOSRoleTemplateConfig{background-image:url(../../../assets/images/menu/hos/hos-role-template.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.EPromotion{background-image:url(../../../assets/images/menu/mfg/EPromotion.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.EPromotionHc{background-image:url(../../../assets/images/menu/mfg/EPromotionHc.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.EPromotionSkill{background-image:url(../../../assets/images/menu/mfg/EPromotionSkill.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.HMCWebsite{background-image:url(../../../assets/images/menu/hmc/HMCWebsite.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.ReportCenter{background-image:url(../../../assets/images/menu/reportCenter/reportCenter.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.HuaDataLake{background-image:url(../../../assets/images/menu/basic/HuaDataLake.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.JabilBus{background-image:url(../../../assets/images/menu/basic/JabilBus.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menuitem-icon.IOTPlatform{background-image:url(../../../assets/images/menu/basic/IOTPlatform.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a .p-menu-icon{font-size:24px}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active{background:#147ad9!important;color:#fff!important;font-size:16px!important;box-shadow:0 8px 10px #147ad926!important;border:none!important}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-text{color:#fff!important}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menu-icon{color:#fff!important}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon[ng-reflect-ng-class=\"fa fa\"]{background-image:url(../../../assets/images/teslaOutputTracking/menu-monitor-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.system{background-image:url(../../../assets/images/menu/system/system-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.systemUser{background-image:url(../../../assets/images/menu/system/system-user-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.systemMenu{background-image:url(../../../assets/images/menu/tesla/tesla-report-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.systemRole{background-image:url(../../../assets/images/menu/system/system-role-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.systemDataDictionary{background-image:url(../../../assets/images/menu/system/system-dictionary-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.systemAnnouncement{background-image:url(../../../assets/images/menu/mfg/mfg-announce-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.tesla{background-image:url(../../../assets/images/menu/tesla/tesla-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.teslaMonitor{background-image:url(../../../assets/images/menu/tesla/tesla-monitor-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.teslaReport{background-image:url(../../../assets/images/menu/tesla/tesla-report-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.teslaConfig{background-image:url(../../../assets/images/menu/tesla/tesla-config-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.teslaInfor{background-image:url(../../../assets/images/menu/tesla/tesla-info-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.teslaAutoBuildPlan{background-image:url(../../../assets/images/menu/tesla/tesla-auto-build-plan-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.teslaOrder{background-image:url(../../../assets/images/menu/tesla/tesla-order-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.teslaPlan{background-image:url(../../../assets/images/menu/tesla/tesla-plan-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.teslaPlanMonitoring{background-image:url(../../../assets/images/menu/tesla/tesla-plan-monitoring-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.test{background-image:url(../../../assets/images/menu/testValidation/test-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.testDashboard{background-image:url(../../../assets/images/menu/testValidation/test-dashboard-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.testMyCase{background-image:url(../../../assets/images/menu/testValidation/test-my-case-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.testCaseCenter{background-image:url(../../../assets/images/menu/testValidation/test-case-center-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.testMessage{background-image:url(../../../assets/images/menu/testValidation/test-message-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.testMainData{background-image:url(../../../assets/images/menu/testValidation/test-main-data-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.testCookbook{background-image:url(../../../assets/images/menu/testValidation/test-cook-book-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.testCookbookLabel{background-image:url(../../../assets/images/menu/testValidation/test-cook-book-label-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.escs{background-image:url(../../../assets/images/menu/escs/escs-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.escsHome{background-image:url(../../../assets/images/menu/escs/escs-home-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.mfg{background-image:url(../../../assets/images/menu/mfg/mfg-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.mfgDashboard{background-image:url(../../../assets/images/menu/mfg/mfg-dashboard-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.mfgTraining{background-image:url(../../../assets/images/menu/mfg/mfg-training-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.mfgAnnounce{background-image:url(../../../assets/images/menu/mfg/mfg-announce-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.CourseWare{background-image:url(../../../assets/images/menu/mfg/CourseWare-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.ProactiveCare{background-image:url(../../../assets/images/menu/mfg/ProactiveCare-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.PWTBonus{background-image:url(../../../assets/images/menu/mfg/PWTBonus-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.LineLeaderBonus{background-image:url(../../../assets/images/menu/mfg/LineLeaderBonus-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.SuperDLBonus{background-image:url(../../../assets/images/menu/mfg/LineLeaderBonus-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.SuperLLBonus{background-image:url(../../../assets/images/menu/mfg/SuperLLBonus-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.DLPerformance{background-image:url(../../../assets/images/menu/mfg/DL-Performance-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.DLPWTSource{background-image:url(../../../assets/images/menu/mfg/DLPWTSource-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.DLPWTReport{background-image:url(../../../assets/images/menu/mfg/DLPWTReport-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.NewSystemAnnouncement{background-image:url(../../../assets/images/menu/mfg/SystemAnnouncement-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.mfgCourseWare{background-image:url(../../../assets/images/menu/mfg/mfg-course-ware-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.mfgLearning{background-image:url(../../../assets/images/menu/mfg/mfg-learning-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.mfgQuestionBank{background-image:url(../../../assets/images/menu/mfg/mfg-question-bank-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.mfgTest{background-image:url(../../../assets/images/menu/mfg/mfg-test-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.mfgStatistics{background-image:url(../../../assets/images/menu/mfg/mfg-report-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.mfgEmployee{background-image:url(../../../assets/images/menu/mfg/mfg-employee-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.mfgCareRecord{background-image:url(../../../assets/images/menu/mfg/mfg-care-record-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.mfgInterview{background-image:url(../../../assets/images/menu/mfg/mfg-interview-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.mfgInterviewReport{background-image:url(../../../assets/images/menu/mfg/mfg-interview-report-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.mfgEmployeeAbnormal{background-image:url(../../../assets/images/menu/mfg/mfg-employee-abnormal-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.mfgFeedback{background-image:url(../../../assets/images/menu/mfg/mfg-feedback-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.mfgActiveSummary{background-image:url(../../../assets/images/menu/mfg/mfg-active-summary-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.mfgActiveDetail{background-image:url(../../../assets/images/menu/mfg/mfg-active-detail-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.mfgEBuddy{background-image:url(../../../assets/images/menu/mfg/mfg-e-buddy-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.mfgEBuddyNewGuide{background-image:url(../../../assets/images/menu/mfg/mfg-new-guidance-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.mfgEBuddyFrequentlyQuestion{background-image:url(../../../assets/images/menu/mfg/mfg-frequently-question-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.mfgEBuddyConsultation{background-image:url(../../../assets/images/menu/mfg/mfg-consultation-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.mfgPerformance{background-image:url(../../../assets/images/menu/mfg/mfg-performance-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.mfgPerformancePWT{background-image:url(../../../assets/images/menu/mfg/mfg-PWT-calculation-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.mfgPerformancePWTResult{background-image:url(../../../assets/images/menu/mfg/mfg-PWT-calculation-result-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.mfgPWTTeamManage{background-image:url(../../../assets/images/menu/mfg/mfg-PWT-team-manage-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.ThingsLinkerBroker{background-image:url(../../../assets/images/menu/thingsLinker/broker-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.ThingsLinkerEquipment{background-image:url(../../../assets/images/menu/thingsLinker/equipment-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.ThingsLinkerBooking{background-image:url(../../../assets/images/menu/thingsLinker/booking-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.ThingsLinkerDispatchOrder{background-image:url(../../../assets/images/menu/thingsLinker/order-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.ThingsLinkerConfig{background-image:url(../../../assets/images/menu/tesla/tesla-config-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.ThingsLinker{background-image:url(../../../assets/images/menu/thingsLinker/things-linker-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.PCNConfig{background-image:url(../../../assets/images/menu/pcn/code-config-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.PCNCodeParameter{background-image:url(../../../assets/images/menu/pcn/code-parameter-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.PCNCodeStage{background-image:url(../../../assets/images/menu/pcn/code-stage-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.PCNMasterTemplate{background-image:url(../../../assets/images/menu/pcn/master-template-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.PCNCodeTemplate{background-image:url(../../../assets/images/menu/pcn/code-template-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.PCNCodeBuild{background-image:url(../../../assets/images/menu/pcn/code-build-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.PCNMachine{background-image:url(../../../assets/images/menu/pcn/machine-control-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.PCNMachineInfo{background-image:url(../../../assets/images/menu/pcn/machine-info-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.PCNWorkflow{background-image:url(../../../assets/images/menu/pcn/workflow-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.PCNMachineModel{background-image:url(../../../assets/images/menu/pcn/machine-model-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.PCNMachineChange{background-image:url(../../../assets/images/menu/pcn/machine-change-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.PCNWorkcellCodeConfig{background-image:url(../../../assets/images/menu/pcn/work-cell-config-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.PCNDefaultApprover{background-image:url(../../../assets/images/menu/pcn/default-approver-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.PCNNPI{background-image:url(../../../assets/images/menu/pcn/npi-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.PCNSOP{background-image:url(../../../assets/images/menu/pcn/sop-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.PCN{background-image:url(../../../assets/images/menu/pcn/pcn-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.Home{background-image:url(../../../assets/images/menu/home/home-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.Demo{background-image:url(../../../assets/images/demo/menu/demo-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.WIKIHome{background-image:url(../../../assets/images/menu/wiki/wiki-home-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.WIKIBackground{background-image:url(../../../assets/images/menu/wiki/wiki-config-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.WIKIDashboard{background-image:url(../../../assets/images/menu/wiki/wiki-dashboard-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.WIKITopic{background-image:url(../../../assets/images/menu/wiki/wiki-topic-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.WIKIMyTopic{background-image:url(../../../assets/images/menu/wiki/wiki-my-topic-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.WIKIComment{background-image:url(../../../assets/images/menu/wiki/wiki-comment-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.WIKIMyComment{background-image:url(../../../assets/images/menu/wiki/wiki-my-comment-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.WIKI{background-image:url(../../../assets/images/menu/wiki/wiki-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.JiTBuildPlan{background-image:url(../../../assets/images/menu/jit/jit-build-plan-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.JITDeliveryBooking{background-image:url(../../../assets/images/menu/jit/jit-delivery-booking-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.JITPullListKanban{background-image:url(../../../assets/images/menu/jit/jit-pull-list-kanban-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.JITSIgnFor{background-image:url(../../../assets/images/menu/jit/jit-sign-for-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.JITBuildPlanManagement{background-image:url(../../../assets/images/menu/jit/jit-build-plan-manage-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.JITSubmitBuildPlan{background-image:url(../../../assets/images/menu/jit/jit-submit-build-plan-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.JITShortageList{background-image:url(../../../assets/images/menu/jit/jit-shortage-list-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.JITDemandReport{background-image:url(../../../assets/images/menu/jit/jit-demand-report-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.JITRemainingInventory{background-image:url(../../../assets/images/menu/jit/jit-remaining-inventory-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.JITBaseData{background-image:url(../../../assets/images/menu/jit/jit-base-data-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.JITDummyBOM{background-image:url(../../../assets/images/menu/jit/jit-dummy-bom-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.JITUPH{background-image:url(../../../assets/images/menu/jit/jit-uph-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.JITModelFrequency{background-image:url(../../../assets/images/menu/jit/jit-model-frequency-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.JITAGVLineInfo{background-image:url(../../../assets/images/menu/jit/jit-agv-line-info-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.JITSPQ{background-image:url(../../../assets/images/menu/jit/jit-spq-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.JITDeliveryLeadTime{background-image:url(../../../assets/images/menu/jit/jit-delivery-lead-time-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.JITStorageLocation{background-image:url(../../../assets/images/menu/jit/jit-storage-location-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.JITDeliveryTaskControl{background-image:url(../../../assets/images/menu/jit/jit-delivery-task-control-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.JITBuyerNameByPart{background-image:url(../../../assets/images/menu/jit/jit-buyer-name-by-part-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.JITWarehouseManagement{background-image:url(../../../assets/images/menu/jit/jit-warehouse-management-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.JITWarehouseDelivery{background-image:url(../../../assets/images/menu/jit/jit-warehouse-delivery-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.JITInventoryManagement{background-image:url(../../../assets/images/menu/jit/jit-inventory-management-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.JITInventoryBin{background-image:url(../../../assets/images/menu/jit/jit-inventory-bin-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.MesStream{background-image:url(../../../assets/images/menu/mesStream/mes-stream-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.MesStreamConfig{background-image:url(../../../assets/images/menu/mesStream/mes-stream-config-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.MesStreamWip{background-image:url(../../../assets/images/menu/mesStream/mes-stream-wip-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.FlowChart{background-image:url(../../../assets/images/menu/flowchart/flowchart-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.MFGGOGS{background-image:url(../../../assets/images/menu/mfggogs/mfggogs-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.QA{background-image:url(../../../assets/images/menu/qa/qa-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.ESCSExtendedSystem{background-image:url(../../../assets/images/menu/escs/escsextendedsystem-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.Quotation{background-image:url(../../../assets/images/menu/quotation/quotation-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.Machine{background-image:url(../../../assets/images/menu/machine/machine-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.Setup{background-image:url(../../../assets/images/menu/setup/setup-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.JMJagentMonitoring{background-image:url(../../../assets/images/menu/jmjagentmonitoring/jmjagentmonitoring-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.WorkflowConfig{background-image:url(../../../assets/images/menu/workflowConfig/workflow-config-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.WorkflowTeslaConfig{background-image:url(../../../assets/images/menu/workflowConfig/workflow-tesla-config-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.Sediot{background-image:url(../../../assets/images/menu/sediot/sediot-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.SediotCycleTime{background-image:url(../../../assets/images/menu/sediot/sediot-cycle-time-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.SediotProcessReport{background-image:url(../../../assets/images/menu/sediot/sediot-process-report-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.SediotEmailConfig{background-image:url(../../../assets/images/menu/sediot/sediot-email-config-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.SediotUtilizationReport{background-image:url(../../../assets/images/menu/sediot/sediot-utilization-report-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.SediotPMAlert{background-image:url(../../../assets/images/menu/sediot/sediot-pm-alert-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.SediotLessonLearn{background-image:url(../../../assets/images/menu/sediot/sediot-lesson-learn-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.SediotLayout{background-image:url(../../../assets/images/menu/sediot/sediot-layout-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.OC{background-image:url(../../../assets/images/menu/oc/oc-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.RealTime{background-image:url(../../../assets/images/menu/escs/realtime-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.ProcessAnalysis{background-image:url(../../../assets/images/menu/escs/ProcessAnalysis-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.DataLake{background-image:url(../../../assets/images/menu/dataLake/data-lake-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.Atlas{background-image:url(../../../assets/images/menu/dataLake/Atlas-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.Dinky{background-image:url(../../../assets/images/menu/dataLake/Dinky-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.DolphinScheduler{background-image:url(../../../assets/images/menu/dataLake/DolphinScheduler-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.Doris{background-image:url(../../../assets/images/menu/dataLake/Doris-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.Flink{background-image:url(../../../assets/images/menu/dataLake/Flink-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.HDFS{background-image:url(../../../assets/images/menu/dataLake/HDFS-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.HUE{background-image:url(../../../assets/images/menu/dataLake/HUE-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.Kafka{background-image:url(../../../assets/images/menu/dataLake/Kafka-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.Ketel{background-image:url(../../../assets/images/menu/dataLake/Ketel-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.Portal{background-image:url(../../../assets/images/menu/dataLake/Portal-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.DataTransport{background-image:url(../../../assets/images/menu/dataLake/data-transport-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.DataTraceabilityPortal{background-image:url(../../../assets/images/menu/dataLake/data-lake-portal-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.DataTraceabilityOther{background-image:url(../../../assets/images/menu/dataLake/data-lake-other-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.AOS{background-image:url(../../../assets/images/menu/aos/aos-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.AOSCheckList{background-image:url(../../../assets/images/menu/aos/aos-check-list-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.AOSAuditPlan{background-image:url(../../../assets/images/menu/aos/aos-audit-plan-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.AOSTaskList{background-image:url(../../../assets/images/menu/aos/aos-task-list-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.BMWIOT{background-image:url(../../../assets/images/menu/bmwiot/bmwiot-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.BMWIOTDashboard{background-image:url(../../../assets/images/menu/bmwiot/Dashboard-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.BMWIOTCCUProductionKanban{background-image:url(../../../assets/images/menu/bmwiot/CCUProductionKanban-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.BMWIOTProductionAndTest{background-image:url(../../../assets/images/menu/bmwiot/ProductionAndTest-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.BMWIOTEquipmentAlarm{background-image:url(../../../assets/images/menu/bmwiot/EquipmentAlarm-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.BMWIOTChangeInformation{background-image:url(../../../assets/images/menu/bmwiot/ChangeInformation-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.BMWIOTEquipmentData{background-image:url(../../../assets/images/menu/bmwiot/EquipmentData-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.MasterDataLogo{background-image:url(../../../assets/images/menu/masterdata/masterData-logo-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.MasterDataHomePage{background-image:url(../../../assets/images/menu/masterdata/masterData-homepage-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.MasterDataUpdateRecordQuery{background-image:url(../../../assets/images/menu/masterdata/masterData-updaterecordquery-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.MasterDataProdictionarea{background-image:url(../../../assets/images/menu/masterdata/masterData-prodictionarea-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.MasterDataBay{background-image:url(../../../assets/images/menu/masterdata/masterData-bay-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.MasterDataEmployeen{background-image:url(../../../assets/images/menu/masterdata/masterData-employeen-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.MasterDataEmployeenQuery{background-image:url(../../../assets/images/menu/masterdata/masterData-employeenquery-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.MasterDataOperationConfiguration{background-image:url(../../../assets/images/menu/masterdata/masterData-OperationConfiguration-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.MasterDataHUAID{background-image:url(../../../assets/images/menu/masterdata/masterData-HUAID-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.MasterDataEquipment{background-image:url(../../../assets/images/menu/masterdata/masterData-equipment-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.MaterDataFunction{background-image:url(../../../assets/images/menu/masterdata/masterData-function-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.MasterDataLocation{background-image:url(../../../assets/images/menu/masterdata/masterData-location-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.MaterDataOrganization{background-image:url(../../../assets/images/menu/masterdata/masterData-organization-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.MasterDataManagement{background-image:url(../../../assets/images/menu/masterdata/masterdata-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.CostCenter{background-image:url(../../../assets/images/menu/masterdata/masterData-costcenter-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.dataassetLogo{background-image:url(../../../assets/images/menu/dataasset/dataasset-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.dataassetHome{background-image:url(../../../assets/images/menu/dataasset/dataassetHome-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.dataassetAssetquery{background-image:url(../../../assets/images/menu/dataasset/assetquery-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.dataassetDataAuthorized{background-image:url(../../../assets/images/menu/dataasset/dataAuthorized-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.dataassetDatasource{background-image:url(../../../assets/images/menu/dataasset/datasource-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.dataassetDatatable{background-image:url(../../../assets/images/menu/dataasset/datatable-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.dataasseTopiccategory{background-image:url(../../../assets/images/menu/dataasset/topiccategory-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.dataassetDataapi{background-image:url(../../../assets/images/menu/dataasset/dataapi-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.dataassetDataapilog{background-image:url(../../../assets/images/menu/dataasset/dataapilog-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.dataassetSqlquery{background-image:url(../../../assets/images/menu/dataasset/sqlquery-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.dataassetDataauthority{background-image:url(../../../assets/images/menu/dataasset/dataauthority-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.dataassetDataauthorityquery{background-image:url(../../../assets/images/menu/dataasset/dataauthorityquery-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.dataassetDataUserauthorityquery{background-image:url(../../../assets/images/menu/dataasset/dataUserauthorityquery-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.SMServerMonitoring{background-image:url(../../../assets/images/menu/serverMonitoring/server-monitoring-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.HOSAppWidget{background-image:url(../../../assets/images/menu/hos/hos-app-widget-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.HOSTemplate{background-image:url(../../../assets/images/menu/hos/hos-template-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.HOSRoleTemplateConfig{background-image:url(../../../assets/images/menu/hos/hos-role-template-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.EPromotion{background-image:url(../../../assets/images/menu/mfg/EPromotion-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.EPromotionHc{background-image:url(../../../assets/images/menu/mfg/EPromotionHc-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.EPromotionSkill{background-image:url(../../../assets/images/menu/mfg/EPromotionSkill-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.HMCWebsite{background-image:url(../../../assets/images/menu/hmc/HMCWebsite-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.ReportCenter{background-image:url(../../../assets/images/menu/reportCenter/reportCenter-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.HuaDataLake{background-image:url(../../../assets/images/menu/basic/HuaDataLake-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.JabilBus{background-image:url(../../../assets/images/menu/basic/JabilBus-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     a.p-menuitem-link-active .p-menuitem-icon.IOTPlatform{background-image:url(../../../assets/images/menu/basic/IOTPlatform-active.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover{background:rgba(20,122,217,.1)!important;color:#147ad9!important;font-size:16px!important;box-shadow:0 8px 10px #147ad926!important;border:none!important}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menu-icon, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menu-icon{color:#147ad9!important}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon[ng-reflect-ng-class=\"fa fa\"], [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon[ng-reflect-ng-class=\"fa fa\"]{background-image:url(../../../assets/images/teslaOutputTracking/menu-monitor-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.system, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.system{background-image:url(../../../assets/images/menu/system/system-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.systemUser, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.systemUser{background-image:url(../../../assets/images/menu/system/system-user-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.systemMenu, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.systemMenu{background-image:url(../../../assets/images/menu/tesla/tesla-report-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.systemRole, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.systemRole{background-image:url(../../../assets/images/menu/system/system-role-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.systemDataDictionary, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.systemDataDictionary{background-image:url(../../../assets/images/menu/system/system-dictionary-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.systemAnnouncement, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.systemAnnouncement{background-image:url(../../../assets/images/menu/mfg/mfg-announce-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.tesla, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.tesla{background-image:url(../../../assets/images/menu/tesla/tesla-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.teslaMonitor, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.teslaMonitor{background-image:url(../../../assets/images/menu/tesla/tesla-monitor-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.teslaReport, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.teslaReport{background-image:url(../../../assets/images/menu/tesla/tesla-report-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.teslaConfig, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.teslaConfig{background-image:url(../../../assets/images/menu/tesla/tesla-config-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.teslaInfor, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.teslaInfor{background-image:url(../../../assets/images/menu/tesla/tesla-info-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.teslaAutoBuildPlan, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.teslaAutoBuildPlan{background-image:url(../../../assets/images/menu/tesla/tesla-auto-build-plan-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.teslaOrder, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.teslaOrder{background-image:url(../../../assets/images/menu/tesla/tesla-order-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.teslaPlan, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.teslaPlan{background-image:url(../../../assets/images/menu/tesla/tesla-plan-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.teslaPlanMonitoring, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.teslaPlanMonitoring{background-image:url(../../../assets/images/menu/tesla/tesla-plan-monitoring-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.test, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.test{background-image:url(../../../assets/images/menu/testValidation/test-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.testDashboard, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.testDashboard{background-image:url(../../../assets/images/menu/testValidation/test-dashboard-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.testMyCase, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.testMyCase{background-image:url(../../../assets/images/menu/testValidation/test-my-case-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.testCaseCenter, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.testCaseCenter{background-image:url(../../../assets/images/menu/testValidation/test-case-center-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.testMessage, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.testMessage{background-image:url(../../../assets/images/menu/testValidation/test-message-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.testMainData, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.testMainData{background-image:url(../../../assets/images/menu/testValidation/test-main-data-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.testCookbook, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.testCookbook{background-image:url(../../../assets/images/menu/testValidation/test-cook-book-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.testCookbookLabel, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.testCookbookLabel{background-image:url(../../../assets/images/menu/testValidation/test-cook-book-label-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.escs, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.escs{background-image:url(../../../assets/images/menu/escs/escs-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.escsHome, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.escsHome{background-image:url(../../../assets/images/menu/escs/escs-home-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.mfg, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.mfg{background-image:url(../../../assets/images/menu/mfg/mfg-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.mfgDashboard, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.mfgDashboard{background-image:url(../../../assets/images/menu/mfg/mfg-dashboard-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.mfgTraining, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.mfgTraining{background-image:url(../../../assets/images/menu/mfg/mfg-training-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.mfgAnnounce, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.mfgAnnounce{background-image:url(../../../assets/images/menu/mfg/mfg-announce-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.CourseWare, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.CourseWare{background-image:url(../../../assets/images/menu/mfg/CourseWare-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.ProactiveCare, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.ProactiveCare{background-image:url(../../../assets/images/menu/mfg/ProactiveCare-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.PWTBonus, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.PWTBonus{background-image:url(../../../assets/images/menu/mfg/PWTBonus-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.LineLeaderBonus, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.LineLeaderBonus{background-image:url(../../../assets/images/menu/mfg/LineLeaderBonus-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.SuperDLBonus, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.SuperDLBonus{background-image:url(../../../assets/images/menu/mfg/LineLeaderBonus-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.SuperLLBonus, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.SuperLLBonus{background-image:url(../../../assets/images/menu/mfg/SuperLLBonus-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.DLPerformance, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.DLPerformance{background-image:url(../../../assets/images/menu/mfg/DL-Performance-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.DLPWTSource, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.DLPWTSource{background-image:url(../../../assets/images/menu/mfg/DLPWTSource-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.DLPWTReport, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.DLPWTReport{background-image:url(../../../assets/images/menu/mfg/DLPWTReport-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.NewSystemAnnouncement, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.NewSystemAnnouncement{background-image:url(../../../assets/images/menu/mfg/SystemAnnouncement-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.mfgCourseWare, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.mfgCourseWare{background-image:url(../../../assets/images/menu/mfg/mfg-course-ware-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.mfgLearning, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.mfgLearning{background-image:url(../../../assets/images/menu/mfg/mfg-learning-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.mfgQuestionBank, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.mfgQuestionBank{background-image:url(../../../assets/images/menu/mfg/mfg-question-bank-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.mfgTest, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.mfgTest{background-image:url(../../../assets/images/menu/mfg/mfg-test-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.mfgStatistics, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.mfgStatistics{background-image:url(../../../assets/images/menu/mfg/mfg-report-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.mfgEmployee, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.mfgEmployee{background-image:url(../../../assets/images/menu/mfg/mfg-employee-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.mfgCareRecord, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.mfgCareRecord{background-image:url(../../../assets/images/menu/mfg/mfg-care-record-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.mfgInterview, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.mfgInterview{background-image:url(../../../assets/images/menu/mfg/mfg-interview-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.mfgEmployeeAbnormal, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.mfgEmployeeAbnormal{background-image:url(../../../assets/images/menu/mfg/mfg-employee-abnormal-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.mfgInterviewReport, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.mfgInterviewReport{background-image:url(../../../assets/images/menu/mfg/mfg-interview-report-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.mfgFeedback, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.mfgFeedback{background-image:url(../../../assets/images/menu/mfg/mfg-feedback-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.mfgActiveSummary, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.mfgActiveSummary{background-image:url(../../../assets/images/menu/mfg/mfg-active-summary-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.mfgActiveDetail, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.mfgActiveDetail{background-image:url(../../../assets/images/menu/mfg/mfg-active-detail-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.mfgEBuddy, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.mfgEBuddy{background-image:url(../../../assets/images/menu/mfg/mfg-e-buddy-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.mfgEBuddyNewGuide, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.mfgEBuddyNewGuide{background-image:url(../../../assets/images/menu/mfg/mfg-new-guidance-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.mfgEBuddyFrequentlyQuestion, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.mfgEBuddyFrequentlyQuestion{background-image:url(../../../assets/images/menu/mfg/mfg-frequently-question-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.mfgEBuddyConsultation, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.mfgEBuddyConsultation{background-image:url(../../../assets/images/menu/mfg/mfg-consultation-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.mfgPerformance, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.mfgPerformance{background-image:url(../../../assets/images/menu/mfg/mfg-performance-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.mfgPerformancePWT, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.mfgPerformancePWT{background-image:url(../../../assets/images/menu/mfg/mfg-PWT-calculation-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.mfgPerformancePWTResult, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.mfgPerformancePWTResult{background-image:url(../../../assets/images/menu/mfg/mfg-PWT-calculation-result-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.mfgPWTTeamManage, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.mfgPWTTeamManage{background-image:url(../../../assets/images/menu/mfg/mfg-PWT-team-manage-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.ThingsLinkerBroker, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.ThingsLinkerBroker{background-image:url(../../../assets/images/menu/thingsLinker/broker-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.ThingsLinkerEquipment, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.ThingsLinkerEquipment{background-image:url(../../../assets/images/menu/thingsLinker/equipment-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.ThingsLinkerBooking, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.ThingsLinkerBooking{background-image:url(../../../assets/images/menu/thingsLinker/booking-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.ThingsLinkerDispatchOrder, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.ThingsLinkerDispatchOrder{background-image:url(../../../assets/images/menu/thingsLinker/order-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.ThingsLinkerConfig, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.ThingsLinkerConfig{background-image:url(../../../assets/images/menu/tesla/tesla-config-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.ThingsLinker, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.ThingsLinker{background-image:url(../../../assets/images/menu/thingsLinker/things-linker-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.PCNConfig, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.PCNConfig{background-image:url(../../../assets/images/menu/pcn/code-config-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.PCNCodeParameter, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.PCNCodeParameter{background-image:url(../../../assets/images/menu/pcn/code-parameter-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.PCNCodeStage, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.PCNCodeStage{background-image:url(../../../assets/images/menu/pcn/code-stage-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.PCNMasterTemplate, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.PCNMasterTemplate{background-image:url(../../../assets/images/menu/pcn/master-template-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.PCNCodeTemplate, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.PCNCodeTemplate{background-image:url(../../../assets/images/menu/pcn/code-template-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.PCNCodeBuild, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.PCNCodeBuild{background-image:url(../../../assets/images/menu/pcn/code-build-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.PCNMachine, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.PCNMachine{background-image:url(../../../assets/images/menu/pcn/machine-control-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.PCNMachineInfo, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.PCNMachineInfo{background-image:url(../../../assets/images/menu/pcn/machine-info-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.PCNWorkflow, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.PCNWorkflow{background-image:url(../../../assets/images/menu/pcn/workflow-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.PCNMachineModel, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.PCNMachineModel{background-image:url(../../../assets/images/menu/pcn/machine-model-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.PCNMachineChange, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.PCNMachineChange{background-image:url(../../../assets/images/menu/pcn/machine-change-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.PCNWorkcellCodeConfig, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.PCNWorkcellCodeConfig{background-image:url(../../../assets/images/menu/pcn/work-cell-config-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.PCNDefaultApprover, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.PCNDefaultApprover{background-image:url(../../../assets/images/menu/pcn/default-approver-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.PCNNPI, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.PCNNPI{background-image:url(../../../assets/images/menu/pcn/npi-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.PCNSOP, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.PCNSOP{background-image:url(../../../assets/images/menu/pcn/sop-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.PCN, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.PCN{background-image:url(../../../assets/images/menu/pcn/pcn-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.Home, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.Home{background-image:url(../../../assets/images/menu/home/home-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.Demo, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.Demo{background-image:url(../../../assets/images/demo/menu/demo-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.WIKIHome, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.WIKIHome{background-image:url(../../../assets/images/menu/wiki/wiki-home-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.WIKIBackground, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.WIKIBackground{background-image:url(../../../assets/images/menu/wiki/wiki-config-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.WIKIDashboard, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.WIKIDashboard{background-image:url(../../../assets/images/menu/wiki/wiki-dashboard-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.WIKITopic, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.WIKITopic{background-image:url(../../../assets/images/menu/wiki/wiki-topic-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.WIKIMyTopic, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.WIKIMyTopic{background-image:url(../../../assets/images/menu/wiki/wiki-my-topic-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.WIKIComment, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.WIKIComment{background-image:url(../../../assets/images/menu/wiki/wiki-comment-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.WIKIMyComment, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.WIKIMyComment{background-image:url(../../../assets/images/menu/wiki/wiki-my-comment-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.WIKI, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.WIKI{background-image:url(../../../assets/images/menu/wiki/wiki-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.JiTBuildPlan, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.JiTBuildPlan{background-image:url(../../../assets/images/menu/jit/jit-build-plan-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.JITDeliveryBooking, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.JITDeliveryBooking{background-image:url(../../../assets/images/menu/jit/jit-delivery-booking-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.JITPullListKanban, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.JITPullListKanban{background-image:url(../../../assets/images/menu/jit/jit-pull-list-kanban-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.JITSIgnFor, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.JITSIgnFor{background-image:url(../../../assets/images/menu/jit/jit-sign-for-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.JITBuildPlanManagement, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.JITBuildPlanManagement{background-image:url(../../../assets/images/menu/jit/jit-build-plan-manage-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.JITSubmitBuildPlan, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.JITSubmitBuildPlan{background-image:url(../../../assets/images/menu/jit/jit-submit-build-plan-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.JITShortageList, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.JITShortageList{background-image:url(../../../assets/images/menu/jit/jit-shortage-list-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.JITDemandReport, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.JITDemandReport{background-image:url(../../../assets/images/menu/jit/jit-demand-report-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.JITRemainingInventory, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.JITRemainingInventory{background-image:url(../../../assets/images/menu/jit/jit-remaining-inventory-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.JITBaseData, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.JITBaseData{background-image:url(../../../assets/images/menu/jit/jit-base-data-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.JITDummyBOM, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.JITDummyBOM{background-image:url(../../../assets/images/menu/jit/jit-dummy-bom-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.JITUPH, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.JITUPH{background-image:url(../../../assets/images/menu/jit/jit-uph-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.JITModelFrequency, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.JITModelFrequency{background-image:url(../../../assets/images/menu/jit/jit-model-frequency-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.JITAGVLineInfo, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.JITAGVLineInfo{background-image:url(../../../assets/images/menu/jit/jit-agv-line-info-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.JITSPQ, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.JITSPQ{background-image:url(../../../assets/images/menu/jit/jit-spq-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.JITDeliveryLeadTime, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.JITDeliveryLeadTime{background-image:url(../../../assets/images/menu/jit/jit-delivery-lead-time-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.JITStorageLocation, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.JITStorageLocation{background-image:url(../../../assets/images/menu/jit/jit-storage-location-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.JITDeliveryTaskControl, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.JITDeliveryTaskControl{background-image:url(../../../assets/images/menu/jit/jit-delivery-task-control-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.JITBuyerNameByPart, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.JITBuyerNameByPart{background-image:url(../../../assets/images/menu/jit/jit-buyer-name-by-part-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.JITWarehouseManagement, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.JITWarehouseManagement{background-image:url(../../../assets/images/menu/jit/jit-warehouse-management-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.JITWarehouseDelivery, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.JITWarehouseDelivery{background-image:url(../../../assets/images/menu/jit/jit-warehouse-delivery-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.JITInventoryManagement, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.JITInventoryManagement{background-image:url(../../../assets/images/menu/jit/jit-inventory-management-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.JITInventoryBin, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.JITInventoryBin{background-image:url(../../../assets/images/menu/jit/jit-inventory-bin-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.MesStream, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.MesStream{background-image:url(../../../assets/images/menu/mesStream/mes-stream-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.MesStreamConfig, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.MesStreamConfig{background-image:url(../../../assets/images/menu/mesStream/mes-stream-config-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.MesStreamWip, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.MesStreamWip{background-image:url(../../../assets/images/menu/mesStream/mes-stream-wip-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.FlowChart, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.FlowChart{background-image:url(../../../assets/images/menu/flowchart/flowchart-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.QA, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.QA{background-image:url(../../../assets/images/menu/qa/qa-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.ESCSExtendedSystem, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.ESCSExtendedSystem{background-image:url(../../../assets/images/menu/escs/escsextendedsystem-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.Quotation, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.Quotation{background-image:url(../../../assets/images/menu/quotation/quotation-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.Machine, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.Machine{background-image:url(../../../assets/images/menu/machine/machine-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.Setup, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.Setup{background-image:url(../../../assets/images/menu/setup/setup-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.JMJagentMonitoring, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.JMJagentMonitoring{background-image:url(../../../assets/images/menu/jmjagentmonitoring/jmjagentmonitoring-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.MFGGOGS, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.MFGGOGS{background-image:url(../../../assets/images/menu/mfggogs/mfggogs-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.WorkflowConfig, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.WorkflowConfig{background-image:url(../../../assets/images/menu/workflowConfig/workflow-config-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.WorkflowTeslaConfig, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.WorkflowTeslaConfig{background-image:url(../../../assets/images/menu/workflowConfig/workflow-tesla-config-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.Sediot, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.Sediot{background-image:url(../../../assets/images/menu/sediot/sediot-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.SediotCycleTime, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.SediotCycleTime{background-image:url(../../../assets/images/menu/sediot/sediot-cycle-time-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.SediotProcessReport, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.SediotProcessReport{background-image:url(../../../assets/images/menu/sediot/sediot-process-report-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.SediotEmailConfig, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.SediotEmailConfig{background-image:url(../../../assets/images/menu/sediot/sediot-email-config-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.SediotUtilizationReport, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.SediotUtilizationReport{background-image:url(../../../assets/images/menu/sediot/sediot-utilization-report-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.SediotPMAlert, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.SediotPMAlert{background-image:url(../../../assets/images/menu/sediot/sediot-pm-alert-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.SediotLessonLearn, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.SediotLessonLearn{background-image:url(../../../assets/images/menu/sediot/sediot-lesson-learn-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.SediotLayout, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.SediotLayout{background-image:url(../../../assets/images/menu/sediot/sediot-layout-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.OC, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.OC{background-image:url(../../../assets/images/menu/oc/oc-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.RealTime, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.RealTime{background-image:url(../../../assets/images/menu/escs/realtime-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.ProcessAnalysis, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.ProcessAnalysis{background-image:url(../../../assets/images/menu/escs/ProcessAnalysis-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.DataLake, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.DataLake{background-image:url(../../../assets/images/menu/dataLake/data-lake-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.Atlas, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.Atlas{background-image:url(../../../assets/images/menu/dataLake/Atlas-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.Dinky, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.Dinky{background-image:url(../../../assets/images/menu/dataLake/Dinky-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.DolphinScheduler, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.DolphinScheduler{background-image:url(../../../assets/images/menu/dataLake/DolphinScheduler-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.Doris, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.Doris{background-image:url(../../../assets/images/menu/dataLake/Doris-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.Flink, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.Flink{background-image:url(../../../assets/images/menu/dataLake/Flink-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.HDFS, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.HDFS{background-image:url(../../../assets/images/menu/dataLake/HDFS-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.HUE, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.HUE{background-image:url(../../../assets/images/menu/dataLake/HUE-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.Kafka, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.Kafka{background-image:url(../../../assets/images/menu/dataLake/Kafka-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.Ketel, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.Ketel{background-image:url(../../../assets/images/menu/dataLake/Ketel-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.Portal, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.Portal{background-image:url(../../../assets/images/menu/dataLake/Portal-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.DataTransport, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.DataTransport{background-image:url(../../../assets/images/menu/dataLake/data-transport-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.DataTraceabilityPortal, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.DataTraceabilityPortal{background-image:url(../../../assets/images/menu/dataLake/data-lake-portal-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.DataTraceabilityOther, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.DataTraceabilityOther{background-image:url(../../../assets/images/menu/dataLake/data-lake-other-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.AOS, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.AOS{background-image:url(../../../assets/images/menu/aos/aos-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.AOSCheckList, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.AOSCheckList{background-image:url(../../../assets/images/menu/aos/aos-check-list-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.AOSAuditPlan, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.AOSAuditPlan{background-image:url(../../../assets/images/menu/aos/aos-audit-plan-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.AOSTaskList, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.AOSTaskList{background-image:url(../../../assets/images/menu/aos/aos-task-list-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.BMWIOT, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.BMWIOT{background-image:url(../../../assets/images/menu/bmwiot/bmwiot-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.BMWIOTDashboard, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.BMWIOTDashboard{background-image:url(../../../assets/images/menu/bmwiot/Dashboard-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.BMWIOTCCUProductionKanban, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.BMWIOTCCUProductionKanban{background-image:url(../../../assets/images/menu/bmwiot/CCUProductionKanban-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.BMWIOTProductionAndTest, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.BMWIOTProductionAndTest{background-image:url(../../../assets/images/menu/bmwiot/ProductionAndTest-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.BMWIOTEquipmentAlarm, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.BMWIOTEquipmentAlarm{background-image:url(../../../assets/images/menu/bmwiot/EquipmentAlarm-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.BMWIOTChangeInformation, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.BMWIOTChangeInformation{background-image:url(../../../assets/images/menu/bmwiot/ChangeInformation-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.BMWIOTEquipmentData, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.BMWIOTEquipmentData{background-image:url(../../../assets/images/menu/bmwiot/EquipmentData-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.MasterDataLogo, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.MasterDataLogo{background-image:url(../../../assets/images/menu/masterdata/masterData-logo-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.MasterDataHomePage, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.MasterDataHomePage{background-image:url(../../../assets/images/menu/masterdata/masterData-homepage-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.MasterDataUpdateRecordQuery, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.MasterDataUpdateRecordQuery{background-image:url(../../../assets/images/menu/masterdata/masterData-updaterecordquery-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.MasterDataProdictionarea, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.MasterDataProdictionarea{background-image:url(../../../assets/images/menu/masterdata/masterData-prodictionarea-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.MasterDataBay, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.MasterDataBay{background-image:url(../../../assets/images/menu/masterdata/masterData-bay-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.MasterDataEmployeen, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.MasterDataEmployeen{background-image:url(../../../assets/images/menu/masterdata/masterData-employeen-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.MasterDataEmployeenQuery, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.MasterDataEmployeenQuery{background-image:url(../../../assets/images/menu/masterdata/masterData-employeenquery-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.MasterDataOperationConfiguration, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.MasterDataOperationConfiguration{background-image:url(../../../assets/images/menu/masterdata/masterData-OperationConfiguration-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.MasterDataHUAID, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.MasterDataHUAID{background-image:url(../../../assets/images/menu/masterdata/masterData-HUAID-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.MasterDataEquipment, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.MasterDataEquipment{background-image:url(../../../assets/images/menu/masterdata/masterData-equipment-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.MaterDataFunction, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.MaterDataFunction{background-image:url(../../../assets/images/menu/masterdata/masterData-function-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.MasterDataLocation, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.MasterDataLocation{background-image:url(../../../assets/images/menu/masterdata/masterData-location-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.MaterDataOrganization, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.MaterDataOrganization{background-image:url(../../../assets/images/menu/masterdata/masterData-organization-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.MasterDataManagement, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.MasterDataManagement{background-image:url(../../../assets/images/menu/masterdata/masterdata-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.CostCenter, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.CostCenter{background-image:url(../../../assets/images/menu/masterdata/masterData-costcenter-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.dataassetLogo, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.dataassetLogo{background-image:url(../../../assets/images/menu/dataasset/dataasset-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.dataassetHome, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.dataassetHome{background-image:url(../../../assets/images/menu/dataasset/dataassetHome-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.dataassetAssetquery, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.dataassetAssetquery{background-image:url(../../../assets/images/menu/dataasset/assetquery-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.dataassetDataAuthorized, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.dataassetDataAuthorized{background-image:url(../../../assets/images/menu/dataasset/dataAuthorized-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.dataassetDatasource, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.dataassetDatasource{background-image:url(../../../assets/images/menu/dataasset/datasource-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.dataassetDatatable, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.dataassetDatatable{background-image:url(../../../assets/images/menu/dataasset/datatable-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.dataasseTopiccategory, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.dataasseTopiccategory{background-image:url(../../../assets/images/menu/dataasset/topiccategory-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.dataassetDataapi, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.dataassetDataapi{background-image:url(../../../assets/images/menu/dataasset/dataapi-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.dataassetDataapilog, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.dataassetDataapilog{background-image:url(../../../assets/images/menu/dataasset/dataapilog-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.dataassetSqlquery, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.dataassetSqlquery{background-image:url(../../../assets/images/menu/dataasset/sqlquery-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.dataassetDataauthority, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.dataassetDataauthority{background-image:url(../../../assets/images/menu/dataasset/dataauthority-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.dataassetDataauthorityquery, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.dataassetDataauthorityquery{background-image:url(../../../assets/images/menu/dataasset/dataauthorityquery-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.dataassetDataUserauthorityquery, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.dataassetDataUserauthorityquery{background-image:url(../../../assets/images/menu/dataasset/dataUserauthorityquery-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.SMServerMonitoring, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.SMServerMonitoring{background-image:url(../../../assets/images/menu/serverMonitoring/server-monitoring-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.HOSAppWidget, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.HOSAppWidget{background-image:url(../../../assets/images/menu/hos/hos-app-widget-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.HOSTemplate, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.HOSTemplate{background-image:url(../../../assets/images/menu/hos/hos-template-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.HOSRoleTemplateConfig, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.HOSRoleTemplateConfig{background-image:url(../../../assets/images/menu/hos/hos-role-template-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.EPromotion, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.EPromotion{background-image:url(../../../assets/images/menu/mfg/EPromotion-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.EPromotionHc, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.EPromotionHc{background-image:url(../../../assets/images/menu/mfg/EPromotionHc-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.EPromotionSkill, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.EPromotionSkill{background-image:url(../../../assets/images/menu/mfg/EPromotionSkill-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.HMCWebsite, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.HMCWebsite{background-image:url(../../../assets/images/menu/hmc/HMCWebsite-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.ReportCenter, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.ReportCenter{background-image:url(../../../assets/images/menu/reportCenter/reportCenter-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.JabilBus, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.JabilBus{background-image:url(../../../assets/images/menu/basic/JabilBus-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-icon.IOTPlatform, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.IOTPlatform{background-image:url(../../../assets/images/menu/basic/IOTPlatform-hover.png)}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-panelmenu-panel a:hover .p-menuitem-text, [_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .p-tieredmenu.jb-menu a:hover .p-menuitem-text{color:#147ad9!important}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .ng-trigger-rootItem{width:208px;margin:0 auto}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     div.p-tieredmenu.jb-menu{border:none;width:40px}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     div.p-tieredmenu.jb-menu p-tieredmenusub[root=root] ul{width:40px}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     div.p-tieredmenu.jb-menu p-tieredmenusub[root=root] ul .p-submenu-list{padding-top:15px}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     div.p-tieredmenu.jb-menu p-tieredmenusub[root=root] ul li ul{width:unset}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     div.p-tieredmenu.jb-menu p-tieredmenusub[root=root] ul li ul a{min-width:200px;width:100%}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     div.p-tieredmenu.jb-menu p-tieredmenusub[root=root] ul li ul a .p-menuitem-text{display:unset}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     div.p-tieredmenu.jb-menu p-tieredmenusub[root=root] ul li a{width:50px}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     div.p-tieredmenu.jb-menu p-tieredmenusub[root=root] ul li a .p-menuitem-text{display:none}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .language{width:unset!important}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .language ul{width:auto!important}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .language ul a{width:auto!important;margin:0!important}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .language ul a:hover{border-radius:0!important}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .language ul a .p-menuitem-icon{position:relative;top:5px}[_nghost-%COMP%]   .layout[_ngcontent-%COMP%]     .language ul a .p-menuitem-text{font-size:12px!important;display:unset!important}[_nghost-%COMP%]   .no-style[_ngcontent-%COMP%]{height:100%;width:100%}[_nghost-%COMP%]   .no-style[_ngcontent-%COMP%]   .collapsed[_ngcontent-%COMP%]{padding:0}[_nghost-%COMP%]   .no-style[_ngcontent-%COMP%]   .mainscontent[_ngcontent-%COMP%]{padding:0}[_nghost-%COMP%]   .no-style[_ngcontent-%COMP%]   .inner-content[_ngcontent-%COMP%]   .main-content[_ngcontent-%COMP%]{padding:0;height:100%}[_nghost-%COMP%]   .arrow[_ngcontent-%COMP%]{width:30px;background:#fff;display:flex;justify-content:center;align-items:center}[_nghost-%COMP%]   [_ngcontent-%COMP%]::-webkit-scrollbar{width:0;height:0}"] });
(function () {
    (typeof ngDevMode === "undefined" || ngDevMode) && i0.ɵsetClassMetadata(LayoutComponent, [{
            type: Component,
            args: [{ selector: 'app-layout', providers: [AppMenuService, HttpService, AppMenuService, SimpleReuseStrategy], template: "<div class=\"layout\" style=\"height: 100%; width: 100%\">\r\n  <div *ngIf=\"!fullPage\" [class]=\"isCollapsed?'collapsed siderbar':'siderbar'\">\r\n    <div class=\"logo\">\r\n      <img *ngIf=\"!isCollapsed\" [src]=\"layoutLogo\" alt=\"\" />\r\n      <img *ngIf=\"isCollapsed\" [src]=\"layoutLogoCollapsed\" alt=\"\" />\r\n    </div>\r\n    <p-panelMenu *ngIf=\"!isCollapsed\" [model]=\"menuList\" [style]=\"{ width: '100%' }\"> </p-panelMenu>\r\n    <p-tieredMenu class=\"position-fixed\" styleClass=\"jb-menu\" appendTo=\"body\" [autoDisplay]=\"true\" *ngIf=\"isCollapsed\"\r\n      [model]=\"menuList\" [style]=\"{ width: '100%' }\"></p-tieredMenu>\r\n  </div>\r\n  <div class=\"w-100 h-100\" [class]=\"fullPage ? 'no-style' : ''\">\r\n    <div [class]=\"isCollapsed?'collapsed mainscontent':'mainscontent'\">\r\n      <div *ngIf=\"!fullPage\" class=\"titlebar\">\r\n        <div class=\"flexrow alc jsfs\">\r\n          <img [src]=\"sidebarIcon\" alt=\"\" (click)=\"isCollapsed = !isCollapsed\" />\r\n          <span class=\"currTab\">\r\n            {{tabMenu[selectedIndex||0]&&tabMenu[selectedIndex||0].greatPName}}\r\n            <span *ngIf=\"tabMenu[selectedIndex||0]&&tabMenu[selectedIndex||0].greatPName\">>></span>\r\n            {{tabMenu[selectedIndex||0]&&tabMenu[selectedIndex||0].pName}}\r\n            <span *ngIf=\"tabMenu[selectedIndex||0]&&tabMenu[selectedIndex||0].pName\">>></span>\r\n            {{tabMenu[selectedIndex||0]&&tabMenu[selectedIndex||0].label}}\r\n          </span>\r\n        </div>\r\n\r\n        <div class=\"flexrow alc jste\" style=\"padding-right: 0.5em\">\r\n          <span>\r\n            <img *ngIf=\"isFullScreen\" [src]=\"fullscreenShrink\" alt=\"\" (click)=\"exitFullScreen()\" />\r\n            <img *ngIf=\"!isFullScreen\" [src]=\"fullscreenExpand\" alt=\"\" (click)=\"fullScreen()\" />\r\n          </span>\r\n\r\n          <span (click)=\"editUserEvent()\" style=\"margin-right: 30px; cursor: pointer\">\r\n            <img style=\"\r\n              margin-right: 2px;\r\n              width: 32px;\r\n              height: 32px;\r\n              border-radius: 50%;\r\n              transform: rotate(0deg);\r\n            \" [src]=\"user\" alt=\"\" />\r\n            {{username}}\r\n          </span>\r\n          <span style=\"cursor: pointer; margin-right: 24px\" (click)=\"lang.toggle($event)\">\r\n            <i class=\"fa fa-globe\"></i>&nbsp;{{ langDesc }}\r\n          </span>\r\n          <p-tieredMenu styleClass=\"language\" appendTo=\"body\" #lang [model]=\"language\" [popup]=\"true\"></p-tieredMenu>\r\n          <div class=\"actionitem\" style=\"cursor: pointer\" (click)=\"logoff()\">\r\n            <i class=\"fa fa-power-off\" aria-hidden=\"true\"></i>&nbsp;{{ logoffObj[currentLanguage] }}\r\n          </div>\r\n          <p-tieredMenu styleClass=\"language\" #menu [popup]=\"true\"></p-tieredMenu>&nbsp;&nbsp;\r\n        </div>\r\n      </div>\r\n\r\n      <div class=\"inner-content\">\r\n        <div id=\"headerTab\" class=\"flex flex-row\" style=\"position: relative\">\r\n          <div *ngIf=\"hasScroll\" (click)=\"scroll('left')\" style=\"position: absolute; left: 0; height: 34px\"\r\n            class=\"arrow cursor-pointer\">\r\n            <i class=\"pi pi-chevron-left\"></i>\r\n          </div>\r\n          <ng-container *ngIf=\"!fullPage\">\r\n            <ul #headerMenu [style]=\"{'padding': hasScroll ? '0 30px' : '' }\"\r\n              class=\"nav nav-tabs flex-nowrap white-space-nowrap overflow-y-auto\" id=\"myTab\" role=\"tablist\">\r\n              <li class=\"nav-item\" *ngFor=\"let tab of tabMenu; index as i\" (contextmenu)=\"contextMenu(tab, i)\">\r\n                <a *ngIf=\"tab?.label && tab?.routerLink\" class=\"nav-link\" [class]=\"selectedIndex === i ? 'active' : ''\"\r\n                  data-toggle=\"tab\" role=\"tab\" aria-selected=\"true\">\r\n                  <a *ngIf=\"tab?.routerLink\" [routerLink]=\"tab.routerLink\" (click)=\"chooseTab(i)\">\r\n                    {{ tab.label }}\r\n                  </a>&nbsp;&nbsp;\r\n                  <i class=\"fa fa-times\" style=\"cursor: pointer\" aria-hidden=\"true\" *ngIf=\"i > 0\"\r\n                    (click)=\"closeTab(i)\"></i>\r\n                </a>\r\n              </li>\r\n            </ul>\r\n            <p-contextMenu *ngIf=\"!fullPage\" appendTo=\"body\" [target]=\"headerMenu\" \r\n            [model]=\"items\" (onShow)=\"showContextMenu=true\" (onHide)=\"showContextMenu=false\"></p-contextMenu>\r\n          </ng-container>\r\n\r\n          <div *ngIf=\"hasScroll\" (click)=\"scroll('right')\" style=\"position: absolute; right: 0; height: 34px\"\r\n            class=\"arrow cursor-pointer\">\r\n            <i class=\"pi pi-chevron-right\"></i>\r\n          </div>\r\n        </div>\r\n        <div class=\"main-content\">\r\n          <div *ngIf=\"!fullPage && showContextMenu\" style=\"position: absolute;width: 100%;height: 100%;\"></div>\r\n          <router-outlet> </router-outlet>\r\n        </div>\r\n      </div>\r\n\r\n      <!--      <div class=\"inner-content\">-->\r\n      <!--        <ul *ngIf=\"!fullPage\" class=\"nav nav-tabs\" id=\"myTab\" role=\"tablist\">-->\r\n      <!--          <li class=\"nav-item\" *ngFor=\"let tab of tabMenu; index as i\">-->\r\n      <!--            <a-->\r\n      <!--              class=\"nav-link\"-->\r\n      <!--              [class]=\"selectedIndex === i ? 'active' : ''\"-->\r\n      <!--              data-toggle=\"tab\"-->\r\n      <!--              role=\"tab\"-->\r\n      <!--              aria-selected=\"true\">-->\r\n      <!--              <a [routerLink]=\"tab?.routerLink\" (click)=\"chooseTab(i)\">{{ tab?.label }}</a>&nbsp;&nbsp;-->\r\n      <!--              <i-->\r\n      <!--                class=\"fa fa-times\"-->\r\n      <!--                style=\"cursor: pointer\"-->\r\n      <!--                aria-hidden=\"true\"-->\r\n      <!--                *ngIf=\"i > 0\"-->\r\n      <!--                (click)=\"closeTab(i)\"></i>-->\r\n      <!--            </a>-->\r\n      <!--          </li>-->\r\n      <!--        </ul>-->\r\n\r\n      <!--        <div class=\"main-content\">-->\r\n      <!--          <router-outlet> </router-outlet>-->\r\n      <!--        </div>-->\r\n      <!--      </div>-->\r\n    </div>\r\n  </div>\r\n\r\n  <!--  <router-outlet *ngIf=\"fullPage\"> </router-outlet>-->\r\n</div>", styles: [":host{overflow:hidden}:host .actionitem{display:flex;align-items:center;justify-content:center;font-size:1rem;font-weight:500;width:auto;padding-left:.5em;padding-right:.5em}:host .mainscontent{display:flex;flex-direction:column;height:100%;width:100%;padding-left:240px}:host .nav-tabs .nav-link.active{background:#147ad9}:host .nav-tabs .nav-link.active a,:host .nav-tabs .nav-link.active i{color:#fff}:host .nav-link{font-weight:400}:host .active{font-weight:700}:host .inner-content{height:100%;background:#f0f2f5}:host .main-content{height:calc(100% - 35px);padding:5px 24px 24px}:host .siderbar{width:240px;background:#fff;position:fixed;top:0;left:0;z-index:999;overflow-y:auto;height:100%}:host .siderbar .logo{height:110px;width:208px;display:flex;justify-content:center;align-items:center;border-bottom:1px solid #f0f0f0;margin:0 auto 32px}:host .siderbar .logo img{width:176px;height:44px}:host .titlebar{height:50px;width:100%;display:flex;flex-direction:row;align-items:center;justify-content:space-between}:host .titlebar img{width:20px;height:20px;margin-left:24px;margin-right:16px;cursor:pointer}:host .titlebar .currTab{font-size:16px;font-weight:500}:host .collapsed.mainscontent{padding-left:50px}:host .collapsed.mainscontent .titlebar img{transform:rotate(180deg)}:host .collapsed.siderbar{width:50px}:host .collapsed.siderbar .logo{width:50px;margin-bottom:10px}:host .collapsed.siderbar .logo img{width:48px;height:48px}:host .collapsed.siderbar ::ng-deep .p-panelmenu .p-panelmenu-header>a{width:100%;height:56px;border-radius:unset!important;display:flex;justify-content:center;align-items:center}:host .collapsed.siderbar ::ng-deep .p-panelmenu .p-panelmenu-header>a span{position:relative;right:20px}:host .collapsed.siderbar ::ng-deep .p-panelmenu .p-panelmenu-header>a .p-menuitem-text{display:none}:host .collapsed ::ng-deep .p-panelmenu-panel .p-submenu-expanded a{width:65px;justify-content:center;margin-left:0}:host .collapsed ::ng-deep .p-panelmenu-panel a .pi-chevron-right,:host .collapsed ::ng-deep .p-panelmenu-panel a .pi-chevron-down,:host .collapsed ::ng-deep .p-panelmenu-panel a .p-menuitem-text{display:none}:host .collapsed ::ng-deep .p-panelmenu-panel a .p-menu-icon{position:unset!important}:host .dialogview{display:flex;flex-direction:row;width:60vw;flex-wrap:wrap}:host .dialog-item{width:50%;padding-bottom:2rem}:host .dialog-item-name{font-size:1.5rem;font-weight:700;text-decoration:underline}:host .dialog-item-email{word-break:break-all}:host p-tieredmenusub{position:fixed}:host .layout ::ng-deep .p-panelmenu-content{border:none!important}:host .layout ::ng-deep .p-panelmenu-header-link .p-menuitem-icon{right:0!important}:host .layout ::ng-deep .p-panelmenu .p-panelmenu-header>a{background:transparent;border:none!important;font-size:16px!important;font-weight:700!important;border-radius:8px!important;width:208px;color:#999!important;margin:0 auto}:host .layout ::ng-deep .p-panelmenu .p-panelmenu-header>a .p-menuitem-icon{display:inline-block;width:24px;height:24px;background-size:contain}:host .layout ::ng-deep .p-submenu-list .p-menuitem{padding-left:8px}:host .layout ::ng-deep .p-submenu-list .p-menuitem a.p-menuitem-link{width:198px}:host .layout ::ng-deep .p-panelmenu-panel .p-submenu-expanded{padding-left:10px}:host .layout ::ng-deep a.p-menuitem-link{background:transparent!important;border:none!important;font-size:16px!important;font-weight:700!important;border-radius:8px!important;width:208px;margin:0 auto 16px}:host .layout ::ng-deep a.p-menuitem-link .p-menuitem-text{color:#999!important}:host .layout ::ng-deep a.p-menuitem-link .p-menuitem-link-active .p-menuitem-text{color:#fff!important}:host .layout ::ng-deep a{margin-bottom:16px}:host .layout ::ng-deep a .p-menuitem-icon{display:inline-block;width:24px;height:24px;min-width:24px;background-size:100% 100%;background-repeat:no-repeat}:host .layout ::ng-deep a .p-menuitem-icon[ng-reflect-ng-class=\"fa fa\"]{background-image:url(../../../assets/images/teslaOutputTracking/menu-monitor.png)}:host .layout ::ng-deep a .p-menuitem-icon.system{background-image:url(../../../assets/images/menu/system/system.png)}:host .layout ::ng-deep a .p-menuitem-icon.systemUser{background-image:url(../../../assets/images/menu/system/system-user.png)}:host .layout ::ng-deep a .p-menuitem-icon.systemMenu{background-image:url(../../../assets/images/menu/tesla/tesla-report.png)}:host .layout ::ng-deep a .p-menuitem-icon.systemRole{background-image:url(../../../assets/images/menu/system/system-role.png)}:host .layout ::ng-deep a .p-menuitem-icon.systemDataDictionary{background-image:url(../../../assets/images/menu/system/system-dictionary.png)}:host .layout ::ng-deep a .p-menuitem-icon.systemAnnouncement{background-image:url(../../../assets/images/menu/mfg/mfg-announce.png)}:host .layout ::ng-deep a .p-menuitem-icon.tesla{background-image:url(../../../assets/images/menu/tesla/tesla.png)}:host .layout ::ng-deep a .p-menuitem-icon.teslaMonitor{background-image:url(../../../assets/images/menu/tesla/tesla-monitor.png)}:host .layout ::ng-deep a .p-menuitem-icon.teslaReport{background-image:url(../../../assets/images/menu/tesla/tesla-report.png)}:host .layout ::ng-deep a .p-menuitem-icon.teslaConfig{background-image:url(../../../assets/images/menu/tesla/tesla-config.png)}:host .layout ::ng-deep a .p-menuitem-icon.teslaInfor{background-image:url(../../../assets/images/menu/tesla/tesla-info.png)}:host .layout ::ng-deep a .p-menuitem-icon.teslaAutoBuildPlan{background-image:url(../../../assets/images/menu/tesla/tesla-auto-build-plan.png)}:host .layout ::ng-deep a .p-menuitem-icon.teslaOrder{background-image:url(../../../assets/images/menu/tesla/tesla-order.png)}:host .layout ::ng-deep a .p-menuitem-icon.teslaPlan{background-image:url(../../../assets/images/menu/tesla/tesla-plan.png)}:host .layout ::ng-deep a .p-menuitem-icon.teslaPlanMonitoring{background-image:url(../../../assets/images/menu/tesla/tesla-plan-monitoring.png)}:host .layout ::ng-deep a .p-menuitem-icon.test{background-image:url(../../../assets/images/menu/testValidation/test.png)}:host .layout ::ng-deep a .p-menuitem-icon.testDashboard{background-image:url(../../../assets/images/menu/testValidation/test-dashboard.png)}:host .layout ::ng-deep a .p-menuitem-icon.testMyCase{background-image:url(../../../assets/images/menu/testValidation/test-my-case.png)}:host .layout ::ng-deep a .p-menuitem-icon.testCaseCenter{background-image:url(../../../assets/images/menu/testValidation/test-case-center.png)}:host .layout ::ng-deep a .p-menuitem-icon.testMessage{background-image:url(../../../assets/images/menu/testValidation/test-message.png)}:host .layout ::ng-deep a .p-menuitem-icon.testMainData{background-image:url(../../../assets/images/menu/testValidation/test-main-data.png)}:host .layout ::ng-deep a .p-menuitem-icon.testCookbook{background-image:url(../../../assets/images/menu/testValidation/test-cook-book.png)}:host .layout ::ng-deep a .p-menuitem-icon.testCookbookLabel{background-image:url(../../../assets/images/menu/testValidation/test-cook-book-label.png)}:host .layout ::ng-deep a .p-menuitem-icon.escs{background-image:url(../../../assets/images/menu/escs/escs.png)}:host .layout ::ng-deep a .p-menuitem-icon.escsHome{background-image:url(../../../assets/images/menu/escs/escs-home.png)}:host .layout ::ng-deep a .p-menuitem-icon.mfg{background-image:url(../../../assets/images/menu/mfg/mfg.png)}:host .layout ::ng-deep a .p-menuitem-icon.mfgDashboard{background-image:url(../../../assets/images/menu/mfg/mfg-dashboard.png)}:host .layout ::ng-deep a .p-menuitem-icon.mfgTraining{background-image:url(../../../assets/images/menu/mfg/mfg-training.png)}:host .layout ::ng-deep a .p-menuitem-icon.mfgAnnounce{background-image:url(../../../assets/images/menu/mfg/mfg-announce.png)}:host .layout ::ng-deep a .p-menuitem-icon.CourseWare{background-image:url(../../../assets/images/menu/mfg/CourseWare.png)}:host .layout ::ng-deep a .p-menuitem-icon.ProactiveCare{background-image:url(../../../assets/images/menu/mfg/ProactiveCare.png)}:host .layout ::ng-deep a .p-menuitem-icon.PWTBonus{background-image:url(../../../assets/images/menu/mfg/PWTBonus.png)}:host .layout ::ng-deep a .p-menuitem-icon.LineLeaderBonus{background-image:url(../../../assets/images/menu/mfg/LineLeaderBonus.png)}:host .layout ::ng-deep a .p-menuitem-icon.SuperDLBonus{background-image:url(../../../assets/images/menu/mfg/LineLeaderBonus.png)}:host .layout ::ng-deep a .p-menuitem-icon.SuperLLBonus{background-image:url(../../../assets/images/menu/mfg/SuperLLBonus.png)}:host .layout ::ng-deep a .p-menuitem-icon.DLPerformance{background-image:url(../../../assets/images/menu/mfg/DL-Performance.png)}:host .layout ::ng-deep a .p-menuitem-icon.DLPWTSource{background-image:url(../../../assets/images/menu/mfg/DLPWTSource.png)}:host .layout ::ng-deep a .p-menuitem-icon.DLPWTReport{background-image:url(../../../assets/images/menu/mfg/DLPWTReport.png)}:host .layout ::ng-deep a .p-menuitem-icon.NewSystemAnnouncement{background-image:url(../../../assets/images/menu/mfg/SystemAnnouncement.png)}:host .layout ::ng-deep a .p-menuitem-icon.mfgCourseWare{background-image:url(../../../assets/images/menu/mfg/mfg-course-ware.png)}:host .layout ::ng-deep a .p-menuitem-icon.mfgLearning{background-image:url(../../../assets/images/menu/mfg/mfg-learning.png)}:host .layout ::ng-deep a .p-menuitem-icon.mfgQuestionBank{background-image:url(../../../assets/images/menu/mfg/mfg-question-bank.png)}:host .layout ::ng-deep a .p-menuitem-icon.mfgTest{background-image:url(../../../assets/images/menu/mfg/mfg-test.png)}:host .layout ::ng-deep a .p-menuitem-icon.mfgStatistics{background-image:url(../../../assets/images/menu/mfg/mfg-report.png)}:host .layout ::ng-deep a .p-menuitem-icon.mfgEmployee{background-image:url(../../../assets/images/menu/mfg/mfg-employee.png)}:host .layout ::ng-deep a .p-menuitem-icon.mfgCareRecord{background-image:url(../../../assets/images/menu/mfg/mfg-care-record.png)}:host .layout ::ng-deep a .p-menuitem-icon.mfgInterview{background-image:url(../../../assets/images/menu/mfg/mfg-interview.png)}:host .layout ::ng-deep a .p-menuitem-icon.mfgInterviewReport{background-image:url(../../../assets/images/menu/mfg/mfg-interview-report.png)}:host .layout ::ng-deep a .p-menuitem-icon.mfgEmployeeAbnormal{background-image:url(../../../assets/images/menu/mfg/mfg-employee-abnormal.png)}:host .layout ::ng-deep a .p-menuitem-icon.mfgFeedback{background-image:url(../../../assets/images/menu/mfg/mfg-feedback.png)}:host .layout ::ng-deep a .p-menuitem-icon.mfgActiveSummary{background-image:url(../../../assets/images/menu/mfg/mfg-active-summary.png)}:host .layout ::ng-deep a .p-menuitem-icon.mfgActiveDetail{background-image:url(../../../assets/images/menu/mfg/mfg-active-detail.png)}:host .layout ::ng-deep a .p-menuitem-icon.mfgEBuddy{background-image:url(../../../assets/images/menu/mfg/mfg-e-buddy.png)}:host .layout ::ng-deep a .p-menuitem-icon.mfgEBuddyNewGuide{background-image:url(../../../assets/images/menu/mfg/mfg-new-guidance.png)}:host .layout ::ng-deep a .p-menuitem-icon.mfgEBuddyFrequentlyQuestion{background-image:url(../../../assets/images/menu/mfg/mfg-frequently-question.png)}:host .layout ::ng-deep a .p-menuitem-icon.mfgEBuddyConsultation{background-image:url(../../../assets/images/menu/mfg/mfg-consultation.png)}:host .layout ::ng-deep a .p-menuitem-icon.mfgPerformance{background-image:url(../../../assets/images/menu/mfg/mfg-performance.png)}:host .layout ::ng-deep a .p-menuitem-icon.mfgPerformancePWT{background-image:url(../../../assets/images/menu/mfg/mfg-PWT-calculation.png)}:host .layout ::ng-deep a .p-menuitem-icon.mfgPerformancePWTResult{background-image:url(../../../assets/images/menu/mfg/mfg-PWT-calculation-result.png)}:host .layout ::ng-deep a .p-menuitem-icon.mfgPWTTeamManage{background-image:url(../../../assets/images/menu/mfg/mfg-PWT-team-manage.png)}:host .layout ::ng-deep a .p-menuitem-icon.ThingsLinkerBroker{background-image:url(../../../assets/images/menu/thingsLinker/broker.png)}:host .layout ::ng-deep a .p-menuitem-icon.ThingsLinkerEquipment{background-image:url(../../../assets/images/menu/thingsLinker/equipment.png)}:host .layout ::ng-deep a .p-menuitem-icon.ThingsLinkerBooking{background-image:url(../../../assets/images/menu/thingsLinker/booking.png)}:host .layout ::ng-deep a .p-menuitem-icon.ThingsLinkerDispatchOrder{background-image:url(../../../assets/images/menu/thingsLinker/order.png)}:host .layout ::ng-deep a .p-menuitem-icon.ThingsLinkerConfig{background-image:url(../../../assets/images/menu/tesla/tesla-config.png)}:host .layout ::ng-deep a .p-menuitem-icon.ThingsLinker{background-image:url(../../../assets/images/menu/thingsLinker/things-linker.png)}:host .layout ::ng-deep a .p-menuitem-icon.PCNConfig{background-image:url(../../../assets/images/menu/pcn/code-config.png)}:host .layout ::ng-deep a .p-menuitem-icon.PCNCodeParameter{background-image:url(../../../assets/images/menu/pcn/code-parameter.png)}:host .layout ::ng-deep a .p-menuitem-icon.PCNCodeStage{background-image:url(../../../assets/images/menu/pcn/code-stage.png)}:host .layout ::ng-deep a .p-menuitem-icon.PCNMasterTemplate{background-image:url(../../../assets/images/menu/pcn/master-template.png)}:host .layout ::ng-deep a .p-menuitem-icon.PCNCodeTemplate{background-image:url(../../../assets/images/menu/pcn/code-template.png)}:host .layout ::ng-deep a .p-menuitem-icon.PCNCodeBuild{background-image:url(../../../assets/images/menu/pcn/code-build.png)}:host .layout ::ng-deep a .p-menuitem-icon.PCNMachine{background-image:url(../../../assets/images/menu/pcn/machine-control.png)}:host .layout ::ng-deep a .p-menuitem-icon.PCNMachineInfo{background-image:url(../../../assets/images/menu/pcn/machine-info.png)}:host .layout ::ng-deep a .p-menuitem-icon.PCNWorkflow{background-image:url(../../../assets/images/menu/pcn/workflow.png)}:host .layout ::ng-deep a .p-menuitem-icon.PCNMachineModel{background-image:url(../../../assets/images/menu/pcn/machine-model.png)}:host .layout ::ng-deep a .p-menuitem-icon.PCNMachineChange{background-image:url(../../../assets/images/menu/pcn/machine-change.png)}:host .layout ::ng-deep a .p-menuitem-icon.PCNWorkcellCodeConfig{background-image:url(../../../assets/images/menu/pcn/work-cell-config.png)}:host .layout ::ng-deep a .p-menuitem-icon.PCNDefaultApprover{background-image:url(../../../assets/images/menu/pcn/default-approver.png)}:host .layout ::ng-deep a .p-menuitem-icon.PCNNPI{background-image:url(../../../assets/images/menu/pcn/npi.png)}:host .layout ::ng-deep a .p-menuitem-icon.PCNSOP{background-image:url(../../../assets/images/menu/pcn/sop.png)}:host .layout ::ng-deep a .p-menuitem-icon.PCN{background-image:url(../../../assets/images/menu/pcn/pcn.png)}:host .layout ::ng-deep a .p-menuitem-icon.Home{background-image:url(../../../assets/images/menu/home/home.png)}:host .layout ::ng-deep a .p-menuitem-icon.Demo{background-image:url(../../../assets/images/demo/menu/demo.png)}:host .layout ::ng-deep a .p-menuitem-icon.WIKIHome{background-image:url(../../../assets/images/menu/wiki/wiki-home.png)}:host .layout ::ng-deep a .p-menuitem-icon.WIKIBackground{background-image:url(../../../assets/images/menu/wiki/wiki-config.png)}:host .layout ::ng-deep a .p-menuitem-icon.WIKIDashboard{background-image:url(../../../assets/images/menu/wiki/wiki-dashboard.png)}:host .layout ::ng-deep a .p-menuitem-icon.WIKITopic{background-image:url(../../../assets/images/menu/wiki/wiki-topic.png)}:host .layout ::ng-deep a .p-menuitem-icon.WIKIMyTopic{background-image:url(../../../assets/images/menu/wiki/wiki-my-topic.png)}:host .layout ::ng-deep a .p-menuitem-icon.WIKIComment{background-image:url(../../../assets/images/menu/wiki/wiki-comment.png)}:host .layout ::ng-deep a .p-menuitem-icon.WIKIMyComment{background-image:url(../../../assets/images/menu/wiki/wiki-my-comment.png)}:host .layout ::ng-deep a .p-menuitem-icon.WIKI{background-image:url(../../../assets/images/menu/wiki/wiki.png)}:host .layout ::ng-deep a .p-menuitem-icon.JiTBuildPlan{background-image:url(../../../assets/images/menu/jit/jit-build-plan.png)}:host .layout ::ng-deep a .p-menuitem-icon.JITDeliveryBooking{background-image:url(../../../assets/images/menu/jit/jit-delivery-booking.png)}:host .layout ::ng-deep a .p-menuitem-icon.JITPullListKanban{background-image:url(../../../assets/images/menu/jit/jit-pull-list-kanban.png)}:host .layout ::ng-deep a .p-menuitem-icon.JITSIgnFor{background-image:url(../../../assets/images/menu/jit/jit-sign-for.png)}:host .layout ::ng-deep a .p-menuitem-icon.JITBuildPlanManagement{background-image:url(../../../assets/images/menu/jit/jit-build-plan-manage.png)}:host .layout ::ng-deep a .p-menuitem-icon.JITSubmitBuildPlan{background-image:url(../../../assets/images/menu/jit/jit-submit-build-plan.png)}:host .layout ::ng-deep a .p-menuitem-icon.JITShortageList{background-image:url(../../../assets/images/menu/jit/jit-shortage-list.png)}:host .layout ::ng-deep a .p-menuitem-icon.JITDemandReport{background-image:url(../../../assets/images/menu/jit/jit-demand-report.png)}:host .layout ::ng-deep a .p-menuitem-icon.JITRemainingInventory{background-image:url(../../../assets/images/menu/jit/jit-remaining-inventory.png)}:host .layout ::ng-deep a .p-menuitem-icon.JITBaseData{background-image:url(../../../assets/images/menu/jit/jit-base-data.png)}:host .layout ::ng-deep a .p-menuitem-icon.JITDummyBOM{background-image:url(../../../assets/images/menu/jit/jit-dummy-bom.png)}:host .layout ::ng-deep a .p-menuitem-icon.JITUPH{background-image:url(../../../assets/images/menu/jit/jit-uph.png)}:host .layout ::ng-deep a .p-menuitem-icon.JITModelFrequency{background-image:url(../../../assets/images/menu/jit/jit-model-frequency.png)}:host .layout ::ng-deep a .p-menuitem-icon.JITAGVLineInfo{background-image:url(../../../assets/images/menu/jit/jit-agv-line-info.png)}:host .layout ::ng-deep a .p-menuitem-icon.JITSPQ{background-image:url(../../../assets/images/menu/jit/jit-spq.png)}:host .layout ::ng-deep a .p-menuitem-icon.JITDeliveryLeadTime{background-image:url(../../../assets/images/menu/jit/jit-delivery-lead-time.png)}:host .layout ::ng-deep a .p-menuitem-icon.JITStorageLocation{background-image:url(../../../assets/images/menu/jit/jit-storage-location.png)}:host .layout ::ng-deep a .p-menuitem-icon.JITDeliveryTaskControl{background-image:url(../../../assets/images/menu/jit/jit-delivery-task-control.png)}:host .layout ::ng-deep a .p-menuitem-icon.JITBuyerNameByPart{background-image:url(../../../assets/images/menu/jit/jit-buyer-name-by-part.png)}:host .layout ::ng-deep a .p-menuitem-icon.JITWarehouseManagement{background-image:url(../../../assets/images/menu/jit/jit-warehouse-management.png)}:host .layout ::ng-deep a .p-menuitem-icon.JITWarehouseDelivery{background-image:url(../../../assets/images/menu/jit/jit-warehouse-delivery.png)}:host .layout ::ng-deep a .p-menuitem-icon.JITInventoryManagement{background-image:url(../../../assets/images/menu/jit/jit-inventory-management.png)}:host .layout ::ng-deep a .p-menuitem-icon.JITInventoryBin{background-image:url(../../../assets/images/menu/jit/jit-inventory-bin.png)}:host .layout ::ng-deep a .p-menuitem-icon.MesStream{background-image:url(../../../assets/images/menu/mesStream/mes-stream.png)}:host .layout ::ng-deep a .p-menuitem-icon.MesStreamConfig{background-image:url(../../../assets/images/menu/mesStream/mes-stream-config.png)}:host .layout ::ng-deep a .p-menuitem-icon.MesStreamWip{background-image:url(../../../assets/images/menu/mesStream/mes-stream-wip.png)}:host .layout ::ng-deep a .p-menuitem-icon.FlowChart{background-image:url(../../../assets/images/menu/flowchart/flowchart.png)}:host .layout ::ng-deep a .p-menuitem-icon.MFGGOGS{background-image:url(../../../assets/images/menu/mfggogs/mfggogs.png)}:host .layout ::ng-deep a .p-menuitem-icon.QA{background-image:url(../../../assets/images/menu/qa/qa.png)}:host .layout ::ng-deep a .p-menuitem-icon.ESCSExtendedSystem{background-image:url(../../../assets/images/menu/escs/escsextendedsystem.png)}:host .layout ::ng-deep a .p-menuitem-icon.Quotation{background-image:url(../../../assets/images/menu/quotation/quotation.png)}:host .layout ::ng-deep a .p-menuitem-icon.Machine{background-image:url(../../../assets/images/menu/machine/machine.png)}:host .layout ::ng-deep a .p-menuitem-icon.Setup{background-image:url(../../../assets/images/menu/setup/setup.png)}:host .layout ::ng-deep a .p-menuitem-icon.JMJagentMonitoring{background-image:url(../../../assets/images/menu/jmjagentmonitoring/jmjagentmonitoring.png)}:host .layout ::ng-deep a .p-menuitem-icon.WorkflowConfig{background-image:url(../../../assets/images/menu/workflowConfig/workflow-config.png)}:host .layout ::ng-deep a .p-menuitem-icon.WorkflowTeslaConfig{background-image:url(../../../assets/images/menu/workflowConfig/workflow-tesla-config.png)}:host .layout ::ng-deep a .p-menuitem-icon.Sediot{background-image:url(../../../assets/images/menu/sediot/sediot.png)}:host .layout ::ng-deep a .p-menuitem-icon.SediotCycleTime{background-image:url(../../../assets/images/menu/sediot/sediot-cycle-time.png)}:host .layout ::ng-deep a .p-menuitem-icon.SediotProcessReport{background-image:url(../../../assets/images/menu/sediot/sediot-process-report.png)}:host .layout ::ng-deep a .p-menuitem-icon.SediotEmailConfig{background-image:url(../../../assets/images/menu/sediot/sediot-email-config.png)}:host .layout ::ng-deep a .p-menuitem-icon.SediotUtilizationReport{background-image:url(../../../assets/images/menu/sediot/sediot-utilization-report.png)}:host .layout ::ng-deep a .p-menuitem-icon.SediotPMAlert{background-image:url(../../../assets/images/menu/sediot/sediot-pm-alert.png)}:host .layout ::ng-deep a .p-menuitem-icon.SediotLessonLearn{background-image:url(../../../assets/images/menu/sediot/sediot-lesson-learn.png)}:host .layout ::ng-deep a .p-menuitem-icon.SediotLayout{background-image:url(../../../assets/images/menu/sediot/sediot-layout.png)}:host .layout ::ng-deep a .p-menuitem-icon.OC{background-image:url(../../../assets/images/menu/oc/oc.png)}:host .layout ::ng-deep a .p-menuitem-icon.RealTime{background-image:url(../../../assets/images/menu/escs/realtime.png)}:host .layout ::ng-deep a .p-menuitem-icon.ProcessAnalysis{background-image:url(../../../assets/images/menu/escs/ProcessAnalysis.png)}:host .layout ::ng-deep a .p-menuitem-icon.DataLake{background-image:url(../../../assets/images/menu/dataLake/data-lake.png)}:host .layout ::ng-deep a .p-menuitem-icon.Atlas{background-image:url(../../../assets/images/menu/dataLake/Atlas.png)}:host .layout ::ng-deep a .p-menuitem-icon.Dinky{background-image:url(../../../assets/images/menu/dataLake/Dinky.png)}:host .layout ::ng-deep a .p-menuitem-icon.DolphinScheduler{background-image:url(../../../assets/images/menu/dataLake/DolphinScheduler.png)}:host .layout ::ng-deep a .p-menuitem-icon.Doris{background-image:url(../../../assets/images/menu/dataLake/Doris.png)}:host .layout ::ng-deep a .p-menuitem-icon.Flink{background-image:url(../../../assets/images/menu/dataLake/Flink.png)}:host .layout ::ng-deep a .p-menuitem-icon.HDFS{background-image:url(../../../assets/images/menu/dataLake/HDFS.png)}:host .layout ::ng-deep a .p-menuitem-icon.HUE{background-image:url(../../../assets/images/menu/dataLake/HUE.png)}:host .layout ::ng-deep a .p-menuitem-icon.Kafka{background-image:url(../../../assets/images/menu/dataLake/Kafka.png)}:host .layout ::ng-deep a .p-menuitem-icon.Ketel{background-image:url(../../../assets/images/menu/dataLake/Ketel.png)}:host .layout ::ng-deep a .p-menuitem-icon.Portal{background-image:url(../../../assets/images/menu/dataLake/Portal.png)}:host .layout ::ng-deep a .p-menuitem-icon.DataTransport{background-image:url(../../../assets/images/menu/dataLake/data-transport.png)}:host .layout ::ng-deep a .p-menuitem-icon.DataTraceabilityPortal{background-image:url(../../../assets/images/menu/dataLake/data-lake-portal.png)}:host .layout ::ng-deep a .p-menuitem-icon.DataTraceabilityOther{background-image:url(../../../assets/images/menu/dataLake/data-lake-other.png)}:host .layout ::ng-deep a .p-menuitem-icon.AOS{background-image:url(../../../assets/images/menu/aos/aos.png)}:host .layout ::ng-deep a .p-menuitem-icon.AOSCheckList{background-image:url(../../../assets/images/menu/aos/aos-check-list.png)}:host .layout ::ng-deep a .p-menuitem-icon.AOSAuditPlan{background-image:url(../../../assets/images/menu/aos/aos-audit-plan.png)}:host .layout ::ng-deep a .p-menuitem-icon.AOSTaskList{background-image:url(../../../assets/images/menu/aos/aos-task-list.png)}:host .layout ::ng-deep a .p-menuitem-icon.BMWIOT{background-image:url(../../../assets/images/menu/bmwiot/bmwiot.png)}:host .layout ::ng-deep a .p-menuitem-icon.BMWIOTDashboard{background-image:url(../../../assets/images/menu/bmwiot/Dashboard.png)}:host .layout ::ng-deep a .p-menuitem-icon.BMWIOTCCUProductionKanban{background-image:url(../../../assets/images/menu/bmwiot/CCUProductionKanban.png)}:host .layout ::ng-deep a .p-menuitem-icon.BMWIOTProductionAndTest{background-image:url(../../../assets/images/menu/bmwiot/ProductionAndTest.png)}:host .layout ::ng-deep a .p-menuitem-icon.BMWIOTEquipmentAlarm{background-image:url(../../../assets/images/menu/bmwiot/EquipmentAlarm.png)}:host .layout ::ng-deep a .p-menuitem-icon.BMWIOTChangeInformation{background-image:url(../../../assets/images/menu/bmwiot/ChangeInformation.png)}:host .layout ::ng-deep a .p-menuitem-icon.BMWIOTEquipmentData{background-image:url(../../../assets/images/menu/bmwiot/EquipmentData.png)}:host .layout ::ng-deep a .p-menuitem-icon.MasterDataLogo{background-image:url(../../../assets/images/menu/masterdata/masterData-logo.png)}:host .layout ::ng-deep a .p-menuitem-icon.MasterDataHomePage{background-image:url(../../../assets/images/menu/masterdata/masterData-homepage.png)}:host .layout ::ng-deep a .p-menuitem-icon.MasterDataUpdateRecordQuery{background-image:url(../../../assets/images/menu/masterdata/masterData-updaterecordquery.png)}:host .layout ::ng-deep a .p-menuitem-icon.MasterDataProdictionarea{background-image:url(../../../assets/images/menu/masterdata/masterData-prodictionarea.png)}:host .layout ::ng-deep a .p-menuitem-icon.MasterDataBay{background-image:url(../../../assets/images/menu/masterdata/masterData-bay.png)}:host .layout ::ng-deep a .p-menuitem-icon.MasterDataEmployeen{background-image:url(../../../assets/images/menu/masterdata/masterData-employeen.png)}:host .layout ::ng-deep a .p-menuitem-icon.MasterDataEmployeenQuery{background-image:url(../../../assets/images/menu/masterdata/masterData-employeenquery.png)}:host .layout ::ng-deep a .p-menuitem-icon.MasterDataOperationConfiguration{background-image:url(../../../assets/images/menu/masterdata/masterData-OperationConfiguration.png)}:host .layout ::ng-deep a .p-menuitem-icon.MasterDataHUAID{background-image:url(../../../assets/images/menu/masterdata/masterData-HUAID.png)}:host .layout ::ng-deep a .p-menuitem-icon.MasterDataEquipment{background-image:url(../../../assets/images/menu/masterdata/masterData-equipment.png)}:host .layout ::ng-deep a .p-menuitem-icon.MaterDataFunction{background-image:url(../../../assets/images/menu/masterdata/masterData-function.png)}:host .layout ::ng-deep a .p-menuitem-icon.MasterDataLocation{background-image:url(../../../assets/images/menu/masterdata/masterData-location.png)}:host .layout ::ng-deep a .p-menuitem-icon.MaterDataOrganization{background-image:url(../../../assets/images/menu/masterdata/masterData-organization.png)}:host .layout ::ng-deep a .p-menuitem-icon.MasterDataManagement{background-image:url(../../../assets/images/menu/masterdata/masterdata.png)}:host .layout ::ng-deep a .p-menuitem-icon.CostCenter{background-image:url(../../../assets/images/menu/masterdata/masterData-costcenter.png)}:host .layout ::ng-deep a .p-menuitem-icon.dataassetLogo{background-image:url(../../../assets/images/menu/dataasset/dataasset.png)}:host .layout ::ng-deep a .p-menuitem-icon.dataassetHome{background-image:url(../../../assets/images/menu/dataasset/dataassetHome.png)}:host .layout ::ng-deep a .p-menuitem-icon.dataassetAssetquery{background-image:url(../../../assets/images/menu/dataasset/assetquery.png)}:host .layout ::ng-deep a .p-menuitem-icon.dataassetDataAuthorized{background-image:url(../../../assets/images/menu/dataasset/dataAuthorized.png)}:host .layout ::ng-deep a .p-menuitem-icon.dataassetDatasource{background-image:url(../../../assets/images/menu/dataasset/datasource.png)}:host .layout ::ng-deep a .p-menuitem-icon.dataassetDatatable{background-image:url(../../../assets/images/menu/dataasset/datatable.png)}:host .layout ::ng-deep a .p-menuitem-icon.dataasseTopiccategory{background-image:url(../../../assets/images/menu/dataasset/topiccategory.png)}:host .layout ::ng-deep a .p-menuitem-icon.dataassetDataapi{background-image:url(../../../assets/images/menu/dataasset/dataapi.png)}:host .layout ::ng-deep a .p-menuitem-icon.dataassetDataapilog{background-image:url(../../../assets/images/menu/dataasset/dataapilog.png)}:host .layout ::ng-deep a .p-menuitem-icon.dataassetSqlquery{background-image:url(../../../assets/images/menu/dataasset/sqlquery.png)}:host .layout ::ng-deep a .p-menuitem-icon.dataassetDataauthority{background-image:url(../../../assets/images/menu/dataasset/dataauthority.png)}:host .layout ::ng-deep a .p-menuitem-icon.dataassetDataauthorityquery{background-image:url(../../../assets/images/menu/dataasset/dataauthorityquery.png)}:host .layout ::ng-deep a .p-menuitem-icon.dataassetDataUserauthorityquery{background-image:url(../../../assets/images/menu/dataasset/dataUserauthorityquery.png)}:host .layout ::ng-deep a .p-menuitem-icon.SMServerMonitoring{background-image:url(../../../assets/images/menu/serverMonitoring/server-monitoring.png)}:host .layout ::ng-deep a .p-menuitem-icon.HOSAppWidget{background-image:url(../../../assets/images/menu/hos/hos-app-widget.png)}:host .layout ::ng-deep a .p-menuitem-icon.HOSTemplate{background-image:url(../../../assets/images/menu/hos/hos-template.png)}:host .layout ::ng-deep a .p-menuitem-icon.HOSRoleTemplateConfig{background-image:url(../../../assets/images/menu/hos/hos-role-template.png)}:host .layout ::ng-deep a .p-menuitem-icon.EPromotion{background-image:url(../../../assets/images/menu/mfg/EPromotion.png)}:host .layout ::ng-deep a .p-menuitem-icon.EPromotionHc{background-image:url(../../../assets/images/menu/mfg/EPromotionHc.png)}:host .layout ::ng-deep a .p-menuitem-icon.EPromotionSkill{background-image:url(../../../assets/images/menu/mfg/EPromotionSkill.png)}:host .layout ::ng-deep a .p-menuitem-icon.HMCWebsite{background-image:url(../../../assets/images/menu/hmc/HMCWebsite.png)}:host .layout ::ng-deep a .p-menuitem-icon.ReportCenter{background-image:url(../../../assets/images/menu/reportCenter/reportCenter.png)}:host .layout ::ng-deep a .p-menuitem-icon.HuaDataLake{background-image:url(../../../assets/images/menu/basic/HuaDataLake.png)}:host .layout ::ng-deep a .p-menuitem-icon.JabilBus{background-image:url(../../../assets/images/menu/basic/JabilBus.png)}:host .layout ::ng-deep a .p-menuitem-icon.IOTPlatform{background-image:url(../../../assets/images/menu/basic/IOTPlatform.png)}:host .layout ::ng-deep a .p-menu-icon{font-size:24px}:host .layout ::ng-deep a.p-menuitem-link-active{background:#147ad9!important;color:#fff!important;font-size:16px!important;box-shadow:0 8px 10px #147ad926!important;border:none!important}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-text{color:#fff!important}:host .layout ::ng-deep a.p-menuitem-link-active .p-menu-icon{color:#fff!important}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon[ng-reflect-ng-class=\"fa fa\"]{background-image:url(../../../assets/images/teslaOutputTracking/menu-monitor-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.system{background-image:url(../../../assets/images/menu/system/system-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.systemUser{background-image:url(../../../assets/images/menu/system/system-user-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.systemMenu{background-image:url(../../../assets/images/menu/tesla/tesla-report-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.systemRole{background-image:url(../../../assets/images/menu/system/system-role-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.systemDataDictionary{background-image:url(../../../assets/images/menu/system/system-dictionary-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.systemAnnouncement{background-image:url(../../../assets/images/menu/mfg/mfg-announce-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.tesla{background-image:url(../../../assets/images/menu/tesla/tesla-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.teslaMonitor{background-image:url(../../../assets/images/menu/tesla/tesla-monitor-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.teslaReport{background-image:url(../../../assets/images/menu/tesla/tesla-report-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.teslaConfig{background-image:url(../../../assets/images/menu/tesla/tesla-config-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.teslaInfor{background-image:url(../../../assets/images/menu/tesla/tesla-info-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.teslaAutoBuildPlan{background-image:url(../../../assets/images/menu/tesla/tesla-auto-build-plan-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.teslaOrder{background-image:url(../../../assets/images/menu/tesla/tesla-order-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.teslaPlan{background-image:url(../../../assets/images/menu/tesla/tesla-plan-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.teslaPlanMonitoring{background-image:url(../../../assets/images/menu/tesla/tesla-plan-monitoring-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.test{background-image:url(../../../assets/images/menu/testValidation/test-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.testDashboard{background-image:url(../../../assets/images/menu/testValidation/test-dashboard-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.testMyCase{background-image:url(../../../assets/images/menu/testValidation/test-my-case-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.testCaseCenter{background-image:url(../../../assets/images/menu/testValidation/test-case-center-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.testMessage{background-image:url(../../../assets/images/menu/testValidation/test-message-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.testMainData{background-image:url(../../../assets/images/menu/testValidation/test-main-data-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.testCookbook{background-image:url(../../../assets/images/menu/testValidation/test-cook-book-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.testCookbookLabel{background-image:url(../../../assets/images/menu/testValidation/test-cook-book-label-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.escs{background-image:url(../../../assets/images/menu/escs/escs-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.escsHome{background-image:url(../../../assets/images/menu/escs/escs-home-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.mfg{background-image:url(../../../assets/images/menu/mfg/mfg-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.mfgDashboard{background-image:url(../../../assets/images/menu/mfg/mfg-dashboard-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.mfgTraining{background-image:url(../../../assets/images/menu/mfg/mfg-training-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.mfgAnnounce{background-image:url(../../../assets/images/menu/mfg/mfg-announce-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.CourseWare{background-image:url(../../../assets/images/menu/mfg/CourseWare-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.ProactiveCare{background-image:url(../../../assets/images/menu/mfg/ProactiveCare-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.PWTBonus{background-image:url(../../../assets/images/menu/mfg/PWTBonus-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.LineLeaderBonus{background-image:url(../../../assets/images/menu/mfg/LineLeaderBonus-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.SuperDLBonus{background-image:url(../../../assets/images/menu/mfg/LineLeaderBonus-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.SuperLLBonus{background-image:url(../../../assets/images/menu/mfg/SuperLLBonus-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.DLPerformance{background-image:url(../../../assets/images/menu/mfg/DL-Performance-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.DLPWTSource{background-image:url(../../../assets/images/menu/mfg/DLPWTSource-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.DLPWTReport{background-image:url(../../../assets/images/menu/mfg/DLPWTReport-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.NewSystemAnnouncement{background-image:url(../../../assets/images/menu/mfg/SystemAnnouncement-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.mfgCourseWare{background-image:url(../../../assets/images/menu/mfg/mfg-course-ware-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.mfgLearning{background-image:url(../../../assets/images/menu/mfg/mfg-learning-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.mfgQuestionBank{background-image:url(../../../assets/images/menu/mfg/mfg-question-bank-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.mfgTest{background-image:url(../../../assets/images/menu/mfg/mfg-test-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.mfgStatistics{background-image:url(../../../assets/images/menu/mfg/mfg-report-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.mfgEmployee{background-image:url(../../../assets/images/menu/mfg/mfg-employee-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.mfgCareRecord{background-image:url(../../../assets/images/menu/mfg/mfg-care-record-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.mfgInterview{background-image:url(../../../assets/images/menu/mfg/mfg-interview-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.mfgInterviewReport{background-image:url(../../../assets/images/menu/mfg/mfg-interview-report-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.mfgEmployeeAbnormal{background-image:url(../../../assets/images/menu/mfg/mfg-employee-abnormal-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.mfgFeedback{background-image:url(../../../assets/images/menu/mfg/mfg-feedback-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.mfgActiveSummary{background-image:url(../../../assets/images/menu/mfg/mfg-active-summary-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.mfgActiveDetail{background-image:url(../../../assets/images/menu/mfg/mfg-active-detail-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.mfgEBuddy{background-image:url(../../../assets/images/menu/mfg/mfg-e-buddy-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.mfgEBuddyNewGuide{background-image:url(../../../assets/images/menu/mfg/mfg-new-guidance-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.mfgEBuddyFrequentlyQuestion{background-image:url(../../../assets/images/menu/mfg/mfg-frequently-question-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.mfgEBuddyConsultation{background-image:url(../../../assets/images/menu/mfg/mfg-consultation-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.mfgPerformance{background-image:url(../../../assets/images/menu/mfg/mfg-performance-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.mfgPerformancePWT{background-image:url(../../../assets/images/menu/mfg/mfg-PWT-calculation-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.mfgPerformancePWTResult{background-image:url(../../../assets/images/menu/mfg/mfg-PWT-calculation-result-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.mfgPWTTeamManage{background-image:url(../../../assets/images/menu/mfg/mfg-PWT-team-manage-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.ThingsLinkerBroker{background-image:url(../../../assets/images/menu/thingsLinker/broker-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.ThingsLinkerEquipment{background-image:url(../../../assets/images/menu/thingsLinker/equipment-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.ThingsLinkerBooking{background-image:url(../../../assets/images/menu/thingsLinker/booking-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.ThingsLinkerDispatchOrder{background-image:url(../../../assets/images/menu/thingsLinker/order-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.ThingsLinkerConfig{background-image:url(../../../assets/images/menu/tesla/tesla-config-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.ThingsLinker{background-image:url(../../../assets/images/menu/thingsLinker/things-linker-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.PCNConfig{background-image:url(../../../assets/images/menu/pcn/code-config-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.PCNCodeParameter{background-image:url(../../../assets/images/menu/pcn/code-parameter-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.PCNCodeStage{background-image:url(../../../assets/images/menu/pcn/code-stage-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.PCNMasterTemplate{background-image:url(../../../assets/images/menu/pcn/master-template-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.PCNCodeTemplate{background-image:url(../../../assets/images/menu/pcn/code-template-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.PCNCodeBuild{background-image:url(../../../assets/images/menu/pcn/code-build-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.PCNMachine{background-image:url(../../../assets/images/menu/pcn/machine-control-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.PCNMachineInfo{background-image:url(../../../assets/images/menu/pcn/machine-info-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.PCNWorkflow{background-image:url(../../../assets/images/menu/pcn/workflow-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.PCNMachineModel{background-image:url(../../../assets/images/menu/pcn/machine-model-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.PCNMachineChange{background-image:url(../../../assets/images/menu/pcn/machine-change-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.PCNWorkcellCodeConfig{background-image:url(../../../assets/images/menu/pcn/work-cell-config-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.PCNDefaultApprover{background-image:url(../../../assets/images/menu/pcn/default-approver-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.PCNNPI{background-image:url(../../../assets/images/menu/pcn/npi-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.PCNSOP{background-image:url(../../../assets/images/menu/pcn/sop-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.PCN{background-image:url(../../../assets/images/menu/pcn/pcn-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.Home{background-image:url(../../../assets/images/menu/home/home-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.Demo{background-image:url(../../../assets/images/demo/menu/demo-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.WIKIHome{background-image:url(../../../assets/images/menu/wiki/wiki-home-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.WIKIBackground{background-image:url(../../../assets/images/menu/wiki/wiki-config-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.WIKIDashboard{background-image:url(../../../assets/images/menu/wiki/wiki-dashboard-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.WIKITopic{background-image:url(../../../assets/images/menu/wiki/wiki-topic-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.WIKIMyTopic{background-image:url(../../../assets/images/menu/wiki/wiki-my-topic-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.WIKIComment{background-image:url(../../../assets/images/menu/wiki/wiki-comment-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.WIKIMyComment{background-image:url(../../../assets/images/menu/wiki/wiki-my-comment-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.WIKI{background-image:url(../../../assets/images/menu/wiki/wiki-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.JiTBuildPlan{background-image:url(../../../assets/images/menu/jit/jit-build-plan-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.JITDeliveryBooking{background-image:url(../../../assets/images/menu/jit/jit-delivery-booking-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.JITPullListKanban{background-image:url(../../../assets/images/menu/jit/jit-pull-list-kanban-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.JITSIgnFor{background-image:url(../../../assets/images/menu/jit/jit-sign-for-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.JITBuildPlanManagement{background-image:url(../../../assets/images/menu/jit/jit-build-plan-manage-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.JITSubmitBuildPlan{background-image:url(../../../assets/images/menu/jit/jit-submit-build-plan-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.JITShortageList{background-image:url(../../../assets/images/menu/jit/jit-shortage-list-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.JITDemandReport{background-image:url(../../../assets/images/menu/jit/jit-demand-report-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.JITRemainingInventory{background-image:url(../../../assets/images/menu/jit/jit-remaining-inventory-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.JITBaseData{background-image:url(../../../assets/images/menu/jit/jit-base-data-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.JITDummyBOM{background-image:url(../../../assets/images/menu/jit/jit-dummy-bom-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.JITUPH{background-image:url(../../../assets/images/menu/jit/jit-uph-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.JITModelFrequency{background-image:url(../../../assets/images/menu/jit/jit-model-frequency-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.JITAGVLineInfo{background-image:url(../../../assets/images/menu/jit/jit-agv-line-info-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.JITSPQ{background-image:url(../../../assets/images/menu/jit/jit-spq-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.JITDeliveryLeadTime{background-image:url(../../../assets/images/menu/jit/jit-delivery-lead-time-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.JITStorageLocation{background-image:url(../../../assets/images/menu/jit/jit-storage-location-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.JITDeliveryTaskControl{background-image:url(../../../assets/images/menu/jit/jit-delivery-task-control-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.JITBuyerNameByPart{background-image:url(../../../assets/images/menu/jit/jit-buyer-name-by-part-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.JITWarehouseManagement{background-image:url(../../../assets/images/menu/jit/jit-warehouse-management-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.JITWarehouseDelivery{background-image:url(../../../assets/images/menu/jit/jit-warehouse-delivery-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.JITInventoryManagement{background-image:url(../../../assets/images/menu/jit/jit-inventory-management-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.JITInventoryBin{background-image:url(../../../assets/images/menu/jit/jit-inventory-bin-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.MesStream{background-image:url(../../../assets/images/menu/mesStream/mes-stream-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.MesStreamConfig{background-image:url(../../../assets/images/menu/mesStream/mes-stream-config-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.MesStreamWip{background-image:url(../../../assets/images/menu/mesStream/mes-stream-wip-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.FlowChart{background-image:url(../../../assets/images/menu/flowchart/flowchart-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.MFGGOGS{background-image:url(../../../assets/images/menu/mfggogs/mfggogs-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.QA{background-image:url(../../../assets/images/menu/qa/qa-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.ESCSExtendedSystem{background-image:url(../../../assets/images/menu/escs/escsextendedsystem-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.Quotation{background-image:url(../../../assets/images/menu/quotation/quotation-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.Machine{background-image:url(../../../assets/images/menu/machine/machine-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.Setup{background-image:url(../../../assets/images/menu/setup/setup-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.JMJagentMonitoring{background-image:url(../../../assets/images/menu/jmjagentmonitoring/jmjagentmonitoring-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.WorkflowConfig{background-image:url(../../../assets/images/menu/workflowConfig/workflow-config-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.WorkflowTeslaConfig{background-image:url(../../../assets/images/menu/workflowConfig/workflow-tesla-config-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.Sediot{background-image:url(../../../assets/images/menu/sediot/sediot-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.SediotCycleTime{background-image:url(../../../assets/images/menu/sediot/sediot-cycle-time-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.SediotProcessReport{background-image:url(../../../assets/images/menu/sediot/sediot-process-report-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.SediotEmailConfig{background-image:url(../../../assets/images/menu/sediot/sediot-email-config-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.SediotUtilizationReport{background-image:url(../../../assets/images/menu/sediot/sediot-utilization-report-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.SediotPMAlert{background-image:url(../../../assets/images/menu/sediot/sediot-pm-alert-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.SediotLessonLearn{background-image:url(../../../assets/images/menu/sediot/sediot-lesson-learn-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.SediotLayout{background-image:url(../../../assets/images/menu/sediot/sediot-layout-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.OC{background-image:url(../../../assets/images/menu/oc/oc-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.RealTime{background-image:url(../../../assets/images/menu/escs/realtime-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.ProcessAnalysis{background-image:url(../../../assets/images/menu/escs/ProcessAnalysis-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.DataLake{background-image:url(../../../assets/images/menu/dataLake/data-lake-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.Atlas{background-image:url(../../../assets/images/menu/dataLake/Atlas-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.Dinky{background-image:url(../../../assets/images/menu/dataLake/Dinky-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.DolphinScheduler{background-image:url(../../../assets/images/menu/dataLake/DolphinScheduler-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.Doris{background-image:url(../../../assets/images/menu/dataLake/Doris-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.Flink{background-image:url(../../../assets/images/menu/dataLake/Flink-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.HDFS{background-image:url(../../../assets/images/menu/dataLake/HDFS-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.HUE{background-image:url(../../../assets/images/menu/dataLake/HUE-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.Kafka{background-image:url(../../../assets/images/menu/dataLake/Kafka-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.Ketel{background-image:url(../../../assets/images/menu/dataLake/Ketel-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.Portal{background-image:url(../../../assets/images/menu/dataLake/Portal-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.DataTransport{background-image:url(../../../assets/images/menu/dataLake/data-transport-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.DataTraceabilityPortal{background-image:url(../../../assets/images/menu/dataLake/data-lake-portal-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.DataTraceabilityOther{background-image:url(../../../assets/images/menu/dataLake/data-lake-other-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.AOS{background-image:url(../../../assets/images/menu/aos/aos-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.AOSCheckList{background-image:url(../../../assets/images/menu/aos/aos-check-list-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.AOSAuditPlan{background-image:url(../../../assets/images/menu/aos/aos-audit-plan-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.AOSTaskList{background-image:url(../../../assets/images/menu/aos/aos-task-list-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.BMWIOT{background-image:url(../../../assets/images/menu/bmwiot/bmwiot-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.BMWIOTDashboard{background-image:url(../../../assets/images/menu/bmwiot/Dashboard-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.BMWIOTCCUProductionKanban{background-image:url(../../../assets/images/menu/bmwiot/CCUProductionKanban-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.BMWIOTProductionAndTest{background-image:url(../../../assets/images/menu/bmwiot/ProductionAndTest-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.BMWIOTEquipmentAlarm{background-image:url(../../../assets/images/menu/bmwiot/EquipmentAlarm-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.BMWIOTChangeInformation{background-image:url(../../../assets/images/menu/bmwiot/ChangeInformation-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.BMWIOTEquipmentData{background-image:url(../../../assets/images/menu/bmwiot/EquipmentData-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.MasterDataLogo{background-image:url(../../../assets/images/menu/masterdata/masterData-logo-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.MasterDataHomePage{background-image:url(../../../assets/images/menu/masterdata/masterData-homepage-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.MasterDataUpdateRecordQuery{background-image:url(../../../assets/images/menu/masterdata/masterData-updaterecordquery-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.MasterDataProdictionarea{background-image:url(../../../assets/images/menu/masterdata/masterData-prodictionarea-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.MasterDataBay{background-image:url(../../../assets/images/menu/masterdata/masterData-bay-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.MasterDataEmployeen{background-image:url(../../../assets/images/menu/masterdata/masterData-employeen-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.MasterDataEmployeenQuery{background-image:url(../../../assets/images/menu/masterdata/masterData-employeenquery-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.MasterDataOperationConfiguration{background-image:url(../../../assets/images/menu/masterdata/masterData-OperationConfiguration-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.MasterDataHUAID{background-image:url(../../../assets/images/menu/masterdata/masterData-HUAID-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.MasterDataEquipment{background-image:url(../../../assets/images/menu/masterdata/masterData-equipment-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.MaterDataFunction{background-image:url(../../../assets/images/menu/masterdata/masterData-function-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.MasterDataLocation{background-image:url(../../../assets/images/menu/masterdata/masterData-location-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.MaterDataOrganization{background-image:url(../../../assets/images/menu/masterdata/masterData-organization-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.MasterDataManagement{background-image:url(../../../assets/images/menu/masterdata/masterdata-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.CostCenter{background-image:url(../../../assets/images/menu/masterdata/masterData-costcenter-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.dataassetLogo{background-image:url(../../../assets/images/menu/dataasset/dataasset-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.dataassetHome{background-image:url(../../../assets/images/menu/dataasset/dataassetHome-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.dataassetAssetquery{background-image:url(../../../assets/images/menu/dataasset/assetquery-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.dataassetDataAuthorized{background-image:url(../../../assets/images/menu/dataasset/dataAuthorized-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.dataassetDatasource{background-image:url(../../../assets/images/menu/dataasset/datasource-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.dataassetDatatable{background-image:url(../../../assets/images/menu/dataasset/datatable-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.dataasseTopiccategory{background-image:url(../../../assets/images/menu/dataasset/topiccategory-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.dataassetDataapi{background-image:url(../../../assets/images/menu/dataasset/dataapi-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.dataassetDataapilog{background-image:url(../../../assets/images/menu/dataasset/dataapilog-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.dataassetSqlquery{background-image:url(../../../assets/images/menu/dataasset/sqlquery-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.dataassetDataauthority{background-image:url(../../../assets/images/menu/dataasset/dataauthority-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.dataassetDataauthorityquery{background-image:url(../../../assets/images/menu/dataasset/dataauthorityquery-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.dataassetDataUserauthorityquery{background-image:url(../../../assets/images/menu/dataasset/dataUserauthorityquery-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.SMServerMonitoring{background-image:url(../../../assets/images/menu/serverMonitoring/server-monitoring-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.HOSAppWidget{background-image:url(../../../assets/images/menu/hos/hos-app-widget-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.HOSTemplate{background-image:url(../../../assets/images/menu/hos/hos-template-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.HOSRoleTemplateConfig{background-image:url(../../../assets/images/menu/hos/hos-role-template-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.EPromotion{background-image:url(../../../assets/images/menu/mfg/EPromotion-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.EPromotionHc{background-image:url(../../../assets/images/menu/mfg/EPromotionHc-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.EPromotionSkill{background-image:url(../../../assets/images/menu/mfg/EPromotionSkill-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.HMCWebsite{background-image:url(../../../assets/images/menu/hmc/HMCWebsite-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.ReportCenter{background-image:url(../../../assets/images/menu/reportCenter/reportCenter-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.HuaDataLake{background-image:url(../../../assets/images/menu/basic/HuaDataLake-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.JabilBus{background-image:url(../../../assets/images/menu/basic/JabilBus-active.png)}:host .layout ::ng-deep a.p-menuitem-link-active .p-menuitem-icon.IOTPlatform{background-image:url(../../../assets/images/menu/basic/IOTPlatform-active.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover{background:rgba(20,122,217,.1)!important;color:#147ad9!important;font-size:16px!important;box-shadow:0 8px 10px #147ad926!important;border:none!important}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menu-icon,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menu-icon{color:#147ad9!important}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon[ng-reflect-ng-class=\"fa fa\"],:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon[ng-reflect-ng-class=\"fa fa\"]{background-image:url(../../../assets/images/teslaOutputTracking/menu-monitor-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.system,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.system{background-image:url(../../../assets/images/menu/system/system-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.systemUser,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.systemUser{background-image:url(../../../assets/images/menu/system/system-user-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.systemMenu,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.systemMenu{background-image:url(../../../assets/images/menu/tesla/tesla-report-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.systemRole,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.systemRole{background-image:url(../../../assets/images/menu/system/system-role-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.systemDataDictionary,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.systemDataDictionary{background-image:url(../../../assets/images/menu/system/system-dictionary-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.systemAnnouncement,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.systemAnnouncement{background-image:url(../../../assets/images/menu/mfg/mfg-announce-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.tesla,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.tesla{background-image:url(../../../assets/images/menu/tesla/tesla-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.teslaMonitor,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.teslaMonitor{background-image:url(../../../assets/images/menu/tesla/tesla-monitor-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.teslaReport,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.teslaReport{background-image:url(../../../assets/images/menu/tesla/tesla-report-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.teslaConfig,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.teslaConfig{background-image:url(../../../assets/images/menu/tesla/tesla-config-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.teslaInfor,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.teslaInfor{background-image:url(../../../assets/images/menu/tesla/tesla-info-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.teslaAutoBuildPlan,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.teslaAutoBuildPlan{background-image:url(../../../assets/images/menu/tesla/tesla-auto-build-plan-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.teslaOrder,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.teslaOrder{background-image:url(../../../assets/images/menu/tesla/tesla-order-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.teslaPlan,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.teslaPlan{background-image:url(../../../assets/images/menu/tesla/tesla-plan-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.teslaPlanMonitoring,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.teslaPlanMonitoring{background-image:url(../../../assets/images/menu/tesla/tesla-plan-monitoring-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.test,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.test{background-image:url(../../../assets/images/menu/testValidation/test-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.testDashboard,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.testDashboard{background-image:url(../../../assets/images/menu/testValidation/test-dashboard-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.testMyCase,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.testMyCase{background-image:url(../../../assets/images/menu/testValidation/test-my-case-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.testCaseCenter,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.testCaseCenter{background-image:url(../../../assets/images/menu/testValidation/test-case-center-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.testMessage,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.testMessage{background-image:url(../../../assets/images/menu/testValidation/test-message-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.testMainData,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.testMainData{background-image:url(../../../assets/images/menu/testValidation/test-main-data-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.testCookbook,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.testCookbook{background-image:url(../../../assets/images/menu/testValidation/test-cook-book-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.testCookbookLabel,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.testCookbookLabel{background-image:url(../../../assets/images/menu/testValidation/test-cook-book-label-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.escs,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.escs{background-image:url(../../../assets/images/menu/escs/escs-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.escsHome,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.escsHome{background-image:url(../../../assets/images/menu/escs/escs-home-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.mfg,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.mfg{background-image:url(../../../assets/images/menu/mfg/mfg-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.mfgDashboard,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.mfgDashboard{background-image:url(../../../assets/images/menu/mfg/mfg-dashboard-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.mfgTraining,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.mfgTraining{background-image:url(../../../assets/images/menu/mfg/mfg-training-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.mfgAnnounce,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.mfgAnnounce{background-image:url(../../../assets/images/menu/mfg/mfg-announce-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.CourseWare,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.CourseWare{background-image:url(../../../assets/images/menu/mfg/CourseWare-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.ProactiveCare,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.ProactiveCare{background-image:url(../../../assets/images/menu/mfg/ProactiveCare-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.PWTBonus,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.PWTBonus{background-image:url(../../../assets/images/menu/mfg/PWTBonus-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.LineLeaderBonus,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.LineLeaderBonus{background-image:url(../../../assets/images/menu/mfg/LineLeaderBonus-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.SuperDLBonus,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.SuperDLBonus{background-image:url(../../../assets/images/menu/mfg/LineLeaderBonus-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.SuperLLBonus,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.SuperLLBonus{background-image:url(../../../assets/images/menu/mfg/SuperLLBonus-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.DLPerformance,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.DLPerformance{background-image:url(../../../assets/images/menu/mfg/DL-Performance-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.DLPWTSource,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.DLPWTSource{background-image:url(../../../assets/images/menu/mfg/DLPWTSource-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.DLPWTReport,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.DLPWTReport{background-image:url(../../../assets/images/menu/mfg/DLPWTReport-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.NewSystemAnnouncement,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.NewSystemAnnouncement{background-image:url(../../../assets/images/menu/mfg/SystemAnnouncement-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.mfgCourseWare,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.mfgCourseWare{background-image:url(../../../assets/images/menu/mfg/mfg-course-ware-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.mfgLearning,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.mfgLearning{background-image:url(../../../assets/images/menu/mfg/mfg-learning-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.mfgQuestionBank,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.mfgQuestionBank{background-image:url(../../../assets/images/menu/mfg/mfg-question-bank-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.mfgTest,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.mfgTest{background-image:url(../../../assets/images/menu/mfg/mfg-test-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.mfgStatistics,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.mfgStatistics{background-image:url(../../../assets/images/menu/mfg/mfg-report-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.mfgEmployee,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.mfgEmployee{background-image:url(../../../assets/images/menu/mfg/mfg-employee-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.mfgCareRecord,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.mfgCareRecord{background-image:url(../../../assets/images/menu/mfg/mfg-care-record-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.mfgInterview,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.mfgInterview{background-image:url(../../../assets/images/menu/mfg/mfg-interview-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.mfgEmployeeAbnormal,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.mfgEmployeeAbnormal{background-image:url(../../../assets/images/menu/mfg/mfg-employee-abnormal-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.mfgInterviewReport,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.mfgInterviewReport{background-image:url(../../../assets/images/menu/mfg/mfg-interview-report-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.mfgFeedback,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.mfgFeedback{background-image:url(../../../assets/images/menu/mfg/mfg-feedback-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.mfgActiveSummary,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.mfgActiveSummary{background-image:url(../../../assets/images/menu/mfg/mfg-active-summary-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.mfgActiveDetail,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.mfgActiveDetail{background-image:url(../../../assets/images/menu/mfg/mfg-active-detail-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.mfgEBuddy,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.mfgEBuddy{background-image:url(../../../assets/images/menu/mfg/mfg-e-buddy-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.mfgEBuddyNewGuide,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.mfgEBuddyNewGuide{background-image:url(../../../assets/images/menu/mfg/mfg-new-guidance-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.mfgEBuddyFrequentlyQuestion,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.mfgEBuddyFrequentlyQuestion{background-image:url(../../../assets/images/menu/mfg/mfg-frequently-question-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.mfgEBuddyConsultation,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.mfgEBuddyConsultation{background-image:url(../../../assets/images/menu/mfg/mfg-consultation-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.mfgPerformance,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.mfgPerformance{background-image:url(../../../assets/images/menu/mfg/mfg-performance-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.mfgPerformancePWT,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.mfgPerformancePWT{background-image:url(../../../assets/images/menu/mfg/mfg-PWT-calculation-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.mfgPerformancePWTResult,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.mfgPerformancePWTResult{background-image:url(../../../assets/images/menu/mfg/mfg-PWT-calculation-result-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.mfgPWTTeamManage,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.mfgPWTTeamManage{background-image:url(../../../assets/images/menu/mfg/mfg-PWT-team-manage-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.ThingsLinkerBroker,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.ThingsLinkerBroker{background-image:url(../../../assets/images/menu/thingsLinker/broker-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.ThingsLinkerEquipment,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.ThingsLinkerEquipment{background-image:url(../../../assets/images/menu/thingsLinker/equipment-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.ThingsLinkerBooking,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.ThingsLinkerBooking{background-image:url(../../../assets/images/menu/thingsLinker/booking-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.ThingsLinkerDispatchOrder,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.ThingsLinkerDispatchOrder{background-image:url(../../../assets/images/menu/thingsLinker/order-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.ThingsLinkerConfig,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.ThingsLinkerConfig{background-image:url(../../../assets/images/menu/tesla/tesla-config-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.ThingsLinker,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.ThingsLinker{background-image:url(../../../assets/images/menu/thingsLinker/things-linker-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.PCNConfig,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.PCNConfig{background-image:url(../../../assets/images/menu/pcn/code-config-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.PCNCodeParameter,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.PCNCodeParameter{background-image:url(../../../assets/images/menu/pcn/code-parameter-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.PCNCodeStage,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.PCNCodeStage{background-image:url(../../../assets/images/menu/pcn/code-stage-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.PCNMasterTemplate,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.PCNMasterTemplate{background-image:url(../../../assets/images/menu/pcn/master-template-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.PCNCodeTemplate,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.PCNCodeTemplate{background-image:url(../../../assets/images/menu/pcn/code-template-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.PCNCodeBuild,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.PCNCodeBuild{background-image:url(../../../assets/images/menu/pcn/code-build-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.PCNMachine,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.PCNMachine{background-image:url(../../../assets/images/menu/pcn/machine-control-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.PCNMachineInfo,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.PCNMachineInfo{background-image:url(../../../assets/images/menu/pcn/machine-info-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.PCNWorkflow,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.PCNWorkflow{background-image:url(../../../assets/images/menu/pcn/workflow-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.PCNMachineModel,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.PCNMachineModel{background-image:url(../../../assets/images/menu/pcn/machine-model-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.PCNMachineChange,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.PCNMachineChange{background-image:url(../../../assets/images/menu/pcn/machine-change-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.PCNWorkcellCodeConfig,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.PCNWorkcellCodeConfig{background-image:url(../../../assets/images/menu/pcn/work-cell-config-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.PCNDefaultApprover,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.PCNDefaultApprover{background-image:url(../../../assets/images/menu/pcn/default-approver-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.PCNNPI,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.PCNNPI{background-image:url(../../../assets/images/menu/pcn/npi-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.PCNSOP,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.PCNSOP{background-image:url(../../../assets/images/menu/pcn/sop-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.PCN,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.PCN{background-image:url(../../../assets/images/menu/pcn/pcn-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.Home,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.Home{background-image:url(../../../assets/images/menu/home/home-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.Demo,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.Demo{background-image:url(../../../assets/images/demo/menu/demo-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.WIKIHome,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.WIKIHome{background-image:url(../../../assets/images/menu/wiki/wiki-home-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.WIKIBackground,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.WIKIBackground{background-image:url(../../../assets/images/menu/wiki/wiki-config-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.WIKIDashboard,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.WIKIDashboard{background-image:url(../../../assets/images/menu/wiki/wiki-dashboard-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.WIKITopic,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.WIKITopic{background-image:url(../../../assets/images/menu/wiki/wiki-topic-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.WIKIMyTopic,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.WIKIMyTopic{background-image:url(../../../assets/images/menu/wiki/wiki-my-topic-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.WIKIComment,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.WIKIComment{background-image:url(../../../assets/images/menu/wiki/wiki-comment-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.WIKIMyComment,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.WIKIMyComment{background-image:url(../../../assets/images/menu/wiki/wiki-my-comment-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.WIKI,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.WIKI{background-image:url(../../../assets/images/menu/wiki/wiki-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.JiTBuildPlan,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.JiTBuildPlan{background-image:url(../../../assets/images/menu/jit/jit-build-plan-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.JITDeliveryBooking,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.JITDeliveryBooking{background-image:url(../../../assets/images/menu/jit/jit-delivery-booking-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.JITPullListKanban,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.JITPullListKanban{background-image:url(../../../assets/images/menu/jit/jit-pull-list-kanban-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.JITSIgnFor,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.JITSIgnFor{background-image:url(../../../assets/images/menu/jit/jit-sign-for-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.JITBuildPlanManagement,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.JITBuildPlanManagement{background-image:url(../../../assets/images/menu/jit/jit-build-plan-manage-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.JITSubmitBuildPlan,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.JITSubmitBuildPlan{background-image:url(../../../assets/images/menu/jit/jit-submit-build-plan-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.JITShortageList,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.JITShortageList{background-image:url(../../../assets/images/menu/jit/jit-shortage-list-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.JITDemandReport,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.JITDemandReport{background-image:url(../../../assets/images/menu/jit/jit-demand-report-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.JITRemainingInventory,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.JITRemainingInventory{background-image:url(../../../assets/images/menu/jit/jit-remaining-inventory-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.JITBaseData,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.JITBaseData{background-image:url(../../../assets/images/menu/jit/jit-base-data-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.JITDummyBOM,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.JITDummyBOM{background-image:url(../../../assets/images/menu/jit/jit-dummy-bom-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.JITUPH,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.JITUPH{background-image:url(../../../assets/images/menu/jit/jit-uph-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.JITModelFrequency,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.JITModelFrequency{background-image:url(../../../assets/images/menu/jit/jit-model-frequency-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.JITAGVLineInfo,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.JITAGVLineInfo{background-image:url(../../../assets/images/menu/jit/jit-agv-line-info-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.JITSPQ,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.JITSPQ{background-image:url(../../../assets/images/menu/jit/jit-spq-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.JITDeliveryLeadTime,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.JITDeliveryLeadTime{background-image:url(../../../assets/images/menu/jit/jit-delivery-lead-time-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.JITStorageLocation,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.JITStorageLocation{background-image:url(../../../assets/images/menu/jit/jit-storage-location-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.JITDeliveryTaskControl,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.JITDeliveryTaskControl{background-image:url(../../../assets/images/menu/jit/jit-delivery-task-control-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.JITBuyerNameByPart,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.JITBuyerNameByPart{background-image:url(../../../assets/images/menu/jit/jit-buyer-name-by-part-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.JITWarehouseManagement,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.JITWarehouseManagement{background-image:url(../../../assets/images/menu/jit/jit-warehouse-management-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.JITWarehouseDelivery,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.JITWarehouseDelivery{background-image:url(../../../assets/images/menu/jit/jit-warehouse-delivery-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.JITInventoryManagement,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.JITInventoryManagement{background-image:url(../../../assets/images/menu/jit/jit-inventory-management-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.JITInventoryBin,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.JITInventoryBin{background-image:url(../../../assets/images/menu/jit/jit-inventory-bin-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.MesStream,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.MesStream{background-image:url(../../../assets/images/menu/mesStream/mes-stream-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.MesStreamConfig,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.MesStreamConfig{background-image:url(../../../assets/images/menu/mesStream/mes-stream-config-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.MesStreamWip,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.MesStreamWip{background-image:url(../../../assets/images/menu/mesStream/mes-stream-wip-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.FlowChart,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.FlowChart{background-image:url(../../../assets/images/menu/flowchart/flowchart-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.QA,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.QA{background-image:url(../../../assets/images/menu/qa/qa-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.ESCSExtendedSystem,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.ESCSExtendedSystem{background-image:url(../../../assets/images/menu/escs/escsextendedsystem-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.Quotation,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.Quotation{background-image:url(../../../assets/images/menu/quotation/quotation-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.Machine,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.Machine{background-image:url(../../../assets/images/menu/machine/machine-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.Setup,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.Setup{background-image:url(../../../assets/images/menu/setup/setup-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.JMJagentMonitoring,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.JMJagentMonitoring{background-image:url(../../../assets/images/menu/jmjagentmonitoring/jmjagentmonitoring-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.MFGGOGS,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.MFGGOGS{background-image:url(../../../assets/images/menu/mfggogs/mfggogs-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.WorkflowConfig,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.WorkflowConfig{background-image:url(../../../assets/images/menu/workflowConfig/workflow-config-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.WorkflowTeslaConfig,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.WorkflowTeslaConfig{background-image:url(../../../assets/images/menu/workflowConfig/workflow-tesla-config-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.Sediot,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.Sediot{background-image:url(../../../assets/images/menu/sediot/sediot-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.SediotCycleTime,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.SediotCycleTime{background-image:url(../../../assets/images/menu/sediot/sediot-cycle-time-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.SediotProcessReport,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.SediotProcessReport{background-image:url(../../../assets/images/menu/sediot/sediot-process-report-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.SediotEmailConfig,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.SediotEmailConfig{background-image:url(../../../assets/images/menu/sediot/sediot-email-config-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.SediotUtilizationReport,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.SediotUtilizationReport{background-image:url(../../../assets/images/menu/sediot/sediot-utilization-report-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.SediotPMAlert,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.SediotPMAlert{background-image:url(../../../assets/images/menu/sediot/sediot-pm-alert-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.SediotLessonLearn,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.SediotLessonLearn{background-image:url(../../../assets/images/menu/sediot/sediot-lesson-learn-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.SediotLayout,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.SediotLayout{background-image:url(../../../assets/images/menu/sediot/sediot-layout-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.OC,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.OC{background-image:url(../../../assets/images/menu/oc/oc-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.RealTime,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.RealTime{background-image:url(../../../assets/images/menu/escs/realtime-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.ProcessAnalysis,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.ProcessAnalysis{background-image:url(../../../assets/images/menu/escs/ProcessAnalysis-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.DataLake,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.DataLake{background-image:url(../../../assets/images/menu/dataLake/data-lake-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.Atlas,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.Atlas{background-image:url(../../../assets/images/menu/dataLake/Atlas-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.Dinky,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.Dinky{background-image:url(../../../assets/images/menu/dataLake/Dinky-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.DolphinScheduler,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.DolphinScheduler{background-image:url(../../../assets/images/menu/dataLake/DolphinScheduler-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.Doris,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.Doris{background-image:url(../../../assets/images/menu/dataLake/Doris-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.Flink,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.Flink{background-image:url(../../../assets/images/menu/dataLake/Flink-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.HDFS,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.HDFS{background-image:url(../../../assets/images/menu/dataLake/HDFS-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.HUE,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.HUE{background-image:url(../../../assets/images/menu/dataLake/HUE-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.Kafka,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.Kafka{background-image:url(../../../assets/images/menu/dataLake/Kafka-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.Ketel,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.Ketel{background-image:url(../../../assets/images/menu/dataLake/Ketel-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.Portal,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.Portal{background-image:url(../../../assets/images/menu/dataLake/Portal-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.DataTransport,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.DataTransport{background-image:url(../../../assets/images/menu/dataLake/data-transport-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.DataTraceabilityPortal,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.DataTraceabilityPortal{background-image:url(../../../assets/images/menu/dataLake/data-lake-portal-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.DataTraceabilityOther,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.DataTraceabilityOther{background-image:url(../../../assets/images/menu/dataLake/data-lake-other-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.AOS,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.AOS{background-image:url(../../../assets/images/menu/aos/aos-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.AOSCheckList,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.AOSCheckList{background-image:url(../../../assets/images/menu/aos/aos-check-list-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.AOSAuditPlan,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.AOSAuditPlan{background-image:url(../../../assets/images/menu/aos/aos-audit-plan-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.AOSTaskList,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.AOSTaskList{background-image:url(../../../assets/images/menu/aos/aos-task-list-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.BMWIOT,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.BMWIOT{background-image:url(../../../assets/images/menu/bmwiot/bmwiot-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.BMWIOTDashboard,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.BMWIOTDashboard{background-image:url(../../../assets/images/menu/bmwiot/Dashboard-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.BMWIOTCCUProductionKanban,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.BMWIOTCCUProductionKanban{background-image:url(../../../assets/images/menu/bmwiot/CCUProductionKanban-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.BMWIOTProductionAndTest,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.BMWIOTProductionAndTest{background-image:url(../../../assets/images/menu/bmwiot/ProductionAndTest-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.BMWIOTEquipmentAlarm,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.BMWIOTEquipmentAlarm{background-image:url(../../../assets/images/menu/bmwiot/EquipmentAlarm-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.BMWIOTChangeInformation,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.BMWIOTChangeInformation{background-image:url(../../../assets/images/menu/bmwiot/ChangeInformation-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.BMWIOTEquipmentData,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.BMWIOTEquipmentData{background-image:url(../../../assets/images/menu/bmwiot/EquipmentData-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.MasterDataLogo,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.MasterDataLogo{background-image:url(../../../assets/images/menu/masterdata/masterData-logo-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.MasterDataHomePage,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.MasterDataHomePage{background-image:url(../../../assets/images/menu/masterdata/masterData-homepage-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.MasterDataUpdateRecordQuery,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.MasterDataUpdateRecordQuery{background-image:url(../../../assets/images/menu/masterdata/masterData-updaterecordquery-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.MasterDataProdictionarea,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.MasterDataProdictionarea{background-image:url(../../../assets/images/menu/masterdata/masterData-prodictionarea-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.MasterDataBay,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.MasterDataBay{background-image:url(../../../assets/images/menu/masterdata/masterData-bay-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.MasterDataEmployeen,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.MasterDataEmployeen{background-image:url(../../../assets/images/menu/masterdata/masterData-employeen-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.MasterDataEmployeenQuery,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.MasterDataEmployeenQuery{background-image:url(../../../assets/images/menu/masterdata/masterData-employeenquery-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.MasterDataOperationConfiguration,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.MasterDataOperationConfiguration{background-image:url(../../../assets/images/menu/masterdata/masterData-OperationConfiguration-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.MasterDataHUAID,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.MasterDataHUAID{background-image:url(../../../assets/images/menu/masterdata/masterData-HUAID-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.MasterDataEquipment,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.MasterDataEquipment{background-image:url(../../../assets/images/menu/masterdata/masterData-equipment-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.MaterDataFunction,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.MaterDataFunction{background-image:url(../../../assets/images/menu/masterdata/masterData-function-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.MasterDataLocation,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.MasterDataLocation{background-image:url(../../../assets/images/menu/masterdata/masterData-location-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.MaterDataOrganization,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.MaterDataOrganization{background-image:url(../../../assets/images/menu/masterdata/masterData-organization-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.MasterDataManagement,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.MasterDataManagement{background-image:url(../../../assets/images/menu/masterdata/masterdata-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.CostCenter,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.CostCenter{background-image:url(../../../assets/images/menu/masterdata/masterData-costcenter-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.dataassetLogo,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.dataassetLogo{background-image:url(../../../assets/images/menu/dataasset/dataasset-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.dataassetHome,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.dataassetHome{background-image:url(../../../assets/images/menu/dataasset/dataassetHome-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.dataassetAssetquery,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.dataassetAssetquery{background-image:url(../../../assets/images/menu/dataasset/assetquery-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.dataassetDataAuthorized,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.dataassetDataAuthorized{background-image:url(../../../assets/images/menu/dataasset/dataAuthorized-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.dataassetDatasource,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.dataassetDatasource{background-image:url(../../../assets/images/menu/dataasset/datasource-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.dataassetDatatable,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.dataassetDatatable{background-image:url(../../../assets/images/menu/dataasset/datatable-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.dataasseTopiccategory,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.dataasseTopiccategory{background-image:url(../../../assets/images/menu/dataasset/topiccategory-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.dataassetDataapi,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.dataassetDataapi{background-image:url(../../../assets/images/menu/dataasset/dataapi-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.dataassetDataapilog,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.dataassetDataapilog{background-image:url(../../../assets/images/menu/dataasset/dataapilog-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.dataassetSqlquery,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.dataassetSqlquery{background-image:url(../../../assets/images/menu/dataasset/sqlquery-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.dataassetDataauthority,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.dataassetDataauthority{background-image:url(../../../assets/images/menu/dataasset/dataauthority-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.dataassetDataauthorityquery,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.dataassetDataauthorityquery{background-image:url(../../../assets/images/menu/dataasset/dataauthorityquery-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.dataassetDataUserauthorityquery,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.dataassetDataUserauthorityquery{background-image:url(../../../assets/images/menu/dataasset/dataUserauthorityquery-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.SMServerMonitoring,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.SMServerMonitoring{background-image:url(../../../assets/images/menu/serverMonitoring/server-monitoring-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.HOSAppWidget,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.HOSAppWidget{background-image:url(../../../assets/images/menu/hos/hos-app-widget-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.HOSTemplate,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.HOSTemplate{background-image:url(../../../assets/images/menu/hos/hos-template-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.HOSRoleTemplateConfig,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.HOSRoleTemplateConfig{background-image:url(../../../assets/images/menu/hos/hos-role-template-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.EPromotion,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.EPromotion{background-image:url(../../../assets/images/menu/mfg/EPromotion-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.EPromotionHc,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.EPromotionHc{background-image:url(../../../assets/images/menu/mfg/EPromotionHc-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.EPromotionSkill,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.EPromotionSkill{background-image:url(../../../assets/images/menu/mfg/EPromotionSkill-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.HMCWebsite,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.HMCWebsite{background-image:url(../../../assets/images/menu/hmc/HMCWebsite-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.ReportCenter,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.ReportCenter{background-image:url(../../../assets/images/menu/reportCenter/reportCenter-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.JabilBus,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.JabilBus{background-image:url(../../../assets/images/menu/basic/JabilBus-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-icon.IOTPlatform,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-icon.IOTPlatform{background-image:url(../../../assets/images/menu/basic/IOTPlatform-hover.png)}:host .layout ::ng-deep .p-panelmenu-panel a:hover .p-menuitem-text,:host .layout ::ng-deep .p-tieredmenu.jb-menu a:hover .p-menuitem-text{color:#147ad9!important}:host .layout ::ng-deep .ng-trigger-rootItem{width:208px;margin:0 auto}:host .layout ::ng-deep div.p-tieredmenu.jb-menu{border:none;width:40px}:host .layout ::ng-deep div.p-tieredmenu.jb-menu p-tieredmenusub[root=root] ul{width:40px}:host .layout ::ng-deep div.p-tieredmenu.jb-menu p-tieredmenusub[root=root] ul .p-submenu-list{padding-top:15px}:host .layout ::ng-deep div.p-tieredmenu.jb-menu p-tieredmenusub[root=root] ul li ul{width:unset}:host .layout ::ng-deep div.p-tieredmenu.jb-menu p-tieredmenusub[root=root] ul li ul a{min-width:200px;width:100%}:host .layout ::ng-deep div.p-tieredmenu.jb-menu p-tieredmenusub[root=root] ul li ul a .p-menuitem-text{display:unset}:host .layout ::ng-deep div.p-tieredmenu.jb-menu p-tieredmenusub[root=root] ul li a{width:50px}:host .layout ::ng-deep div.p-tieredmenu.jb-menu p-tieredmenusub[root=root] ul li a .p-menuitem-text{display:none}:host .layout ::ng-deep .language{width:unset!important}:host .layout ::ng-deep .language ul{width:auto!important}:host .layout ::ng-deep .language ul a{width:auto!important;margin:0!important}:host .layout ::ng-deep .language ul a:hover{border-radius:0!important}:host .layout ::ng-deep .language ul a .p-menuitem-icon{position:relative;top:5px}:host .layout ::ng-deep .language ul a .p-menuitem-text{font-size:12px!important;display:unset!important}:host .no-style{height:100%;width:100%}:host .no-style .collapsed{padding:0}:host .no-style .mainscontent{padding:0}:host .no-style .inner-content .main-content{padding:0;height:100%}:host .arrow{width:30px;background:#fff;display:flex;justify-content:center;align-items:center}:host ::-webkit-scrollbar{width:0;height:0}\n"] }]
        }], function () { return [{ type: i1$2.TranslateService }, { type: i2$2.Router }, { type: i2$2.ActivatedRoute }, { type: i0.ChangeDetectorRef }, { type: AppMenuService }, { type: HttpService }, { type: i5$1.Location }]; }, { menuTreeKeys: [{
                type: Input
            }], menuTree: [{
                type: Input
            }], editUser: [{
                type: Output
            }] });
})();

class LayoutModule {
}
LayoutModule.ɵfac = function LayoutModule_Factory(t) { return new (t || LayoutModule)(); };
LayoutModule.ɵmod = /*@__PURE__*/ i0.ɵɵdefineNgModule({ type: LayoutModule });
LayoutModule.ɵinj = /*@__PURE__*/ i0.ɵɵdefineInjector({ imports: [CommonModule,
        PanelMenuModule,
        ToastModule,
        TieredMenuModule,
        CheckboxModule,
        ButtonModule,
        ContextMenuModule] });
(function () {
    (typeof ngDevMode === "undefined" || ngDevMode) && i0.ɵsetClassMetadata(LayoutModule, [{
            type: NgModule,
            args: [{
                    declarations: [
                        LayoutComponent
                    ],
                    imports: [
                        CommonModule,
                        PanelMenuModule,
                        ToastModule,
                        TieredMenuModule,
                        CheckboxModule,
                        ButtonModule,
                        ContextMenuModule,
                    ],
                    exports: [
                        LayoutComponent
                    ]
                }]
        }], null, null);
})();
(function () {
    (typeof ngJitMode === "undefined" || ngJitMode) && i0.ɵɵsetNgModuleScope(LayoutModule, { declarations: [LayoutComponent], imports: [CommonModule,
            PanelMenuModule,
            ToastModule,
            TieredMenuModule,
            CheckboxModule,
            ButtonModule,
            ContextMenuModule], exports: [LayoutComponent] });
})();

// This file can be replaced during build by using the `fileReplacements` array.
// `ng build` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.
const environment = {
    production: false,
    i18nDomainUri: 'https://jabilbus.jblapps.com/language',
    i18nApiKey: 'ZzLWTI0r9R78G0H5ZoXIKaGdYIBwPAcNaFUGkGSF',
};
/*
 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/plugins/zone-error';  // Included with Angular CLI.

class AppInfoService {
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
(function () {
    (typeof ngDevMode === "undefined" || ngDevMode) && i0.ɵsetClassMetadata(AppInfoService, [{
            type: Injectable,
            args: [{
                    providedIn: 'root',
                }]
        }], function () { return [{ type: i1.HttpClient }]; }, null);
})();

const APP_NAME = 'jabilService'; // export const APP_NAME: string = '';
const DEFAULT_LANGUAGE = {
    id: 0,
    language: 'English',
    code: 'en',
};

class AppInitService {
    constructor(appInfoService, translateService, translateCacheService) {
        this.appInfoService = appInfoService;
        this.translateService = translateService;
        this.translateCacheService = translateCacheService;
        this.localeInitializer = (lang) => __awaiter(this, void 0, void 0, function* () {
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
        });
        this.sleep = (milliseconds) => {
            return new Promise(resolve => setTimeout(resolve, milliseconds));
        };
    }
    init() {
        return __awaiter(this, void 0, void 0, function* () {
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
        });
    }
    initApp(app, resolve, reject) {
        return __awaiter(this, void 0, void 0, function* () {
            this.appInfoService.setApp(app);
            const codes = app === null || app === void 0 ? void 0 : app.languages.map((l) => l.code);
            try {
                if (codes) {
                    for (const code of codes) {
                        yield this.localeInitializer(code);
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
        });
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
            this.translateService.setDefaultLang(DEFAULT_LANGUAGE.code);
        }
        this.translateService.use(langToUse || DEFAULT_LANGUAGE.code);
        localStorage.setItem('lang', langToUse || DEFAULT_LANGUAGE.code);
        // this.translateCacheService.init();
    }
}
AppInitService.ɵfac = function AppInitService_Factory(t) { return new (t || AppInitService)(i0.ɵɵinject(AppInfoService), i0.ɵɵinject(i1$2.TranslateService), i0.ɵɵinject(i3$2.TranslateCacheService)); };
AppInitService.ɵprov = /*@__PURE__*/ i0.ɵɵdefineInjectable({ token: AppInitService, factory: AppInitService.ɵfac, providedIn: 'root' });
(function () {
    (typeof ngDevMode === "undefined" || ngDevMode) && i0.ɵsetClassMetadata(AppInitService, [{
            type: Injectable,
            args: [{
                    providedIn: 'root',
                }]
        }], function () { return [{ type: AppInfoService }, { type: i1$2.TranslateService }, { type: i3$2.TranslateCacheService }]; }, null);
})();

class BigFileUploadService {
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
(function () {
    (typeof ngDevMode === "undefined" || ngDevMode) && i0.ɵsetClassMetadata(BigFileUploadService, [{
            type: Injectable
        }], function () { return [{ type: i1.HttpClient }]; }, null);
})();

class DefaultRouteGuardService {
    constructor(router, document) {
        this.router = router;
        this.document = document;
    }
    canActivate(route, state) {
        return __awaiter(this, void 0, void 0, function* () {
            const defaultUrl = localStorage.getItem('defaultUrl');
            const loginCheck = localStorage.getItem('loginCheck');
            if (defaultUrl === '/common/home' || defaultUrl === '/common/redirect') {
                return true;
            }
            if (defaultUrl && loginCheck === '1') {
                // if (defaultUrl?.includes('/common/jabil-bus') && state.url.includes('/common/jabil-bus')) {
                //   return true;
                // }
                if (state.url.includes('/common/jabil-bus')) {
                    return true;
                }
                this.router.navigate([defaultUrl]);
            }
            return true;
        });
    }
}
DefaultRouteGuardService.ɵfac = function DefaultRouteGuardService_Factory(t) { return new (t || DefaultRouteGuardService)(i0.ɵɵinject(i2$2.Router), i0.ɵɵinject(DOCUMENT)); };
DefaultRouteGuardService.ɵprov = /*@__PURE__*/ i0.ɵɵdefineInjectable({ token: DefaultRouteGuardService, factory: DefaultRouteGuardService.ɵfac });
(function () {
    (typeof ngDevMode === "undefined" || ngDevMode) && i0.ɵsetClassMetadata(DefaultRouteGuardService, [{
            type: Injectable
        }], function () {
        return [{ type: i2$2.Router }, { type: undefined, decorators: [{
                        type: Inject,
                        args: [DOCUMENT]
                    }] }];
    }, null);
})();

class ExcelExportService {
    constructor() {
        this.infoList = [];
        this.fileName = 'download';
    }
    exp(data, exportColumns, fileName) {
        this.fileName = fileName || 'download';
        let worksheet = {};
        if (data.length !== 0) {
            this.downloadData(data, worksheet, exportColumns);
        }
        else {
            console.log('no data');
        }
    }
    downloadData(scoredata, worksheet, exportColumns) {
        //set excel cell width
        var long = {
            '001': [
                { wch: 20 },
                { wch: 15 },
                { wch: 30 },
                { wch: 20 },
                { wch: 20 },
                { wch: 20 },
                { wch: 20 },
            ],
        };
        //excel set first row height
        var row = {
            '001': [{ hpt: 30 }],
        };
        // console.log('export', exportColumns);
        let result = [];
        if (scoredata.length) {
            //handle data
            if (exportColumns.length && (scoredata.length === exportColumns.length)) {
                scoredata.forEach((element, i) => {
                    let obj = {};
                    for (var key in element) {
                        obj[exportColumns[i]] = element[key];
                        // let has = false;
                        // for (var item of exportColumns) {
                        //   if (item.index == key) {
                        //     has = true;
                        //   }
                        // }
                    }
                    result.push(obj);
                });
                scoredata = result;
            }
            worksheet = XLSX.utils.json_to_sheet(scoredata);
            let header = exportColumns;
            //excel header en translate to zh
            // for (let i = Number('A'.charCodeAt(0)); i <= Number('V'.charCodeAt(0)); i++) {
            //   header.push(String.fromCharCode(i) + '1');
            // }
            //set excel cell height and width
            worksheet['!cols'] = long['001'];
            worksheet['!rows'] = row['001'];
            const workbook = { Sheets: { data: worksheet }, SheetNames: ['data'] };
            const excelBuffer = XLSX.write(workbook, { bookType: 'xlsx', type: 'array' });
            //save excel
            this.saveAsExcelFile(excelBuffer, this.fileName);
        }
        else {
            console.log('no data');
        }
    }
    saveAsExcelFile(buffer, fileName) {
        const data = new Blob([buffer], {
            type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8',
        });
        FileSaver.saveAs(data, fileName + '.xlsx');
    }
}
ExcelExportService.ɵfac = function ExcelExportService_Factory(t) { return new (t || ExcelExportService)(); };
ExcelExportService.ɵprov = /*@__PURE__*/ i0.ɵɵdefineInjectable({ token: ExcelExportService, factory: ExcelExportService.ɵfac, providedIn: 'root' });
(function () {
    (typeof ngDevMode === "undefined" || ngDevMode) && i0.ɵsetClassMetadata(ExcelExportService, [{
            type: Injectable,
            args: [{
                    providedIn: 'root',
                }]
        }], null, null);
})();

class LocalStorage {
    constructor() {
        if (!localStorage) {
            throw new Error('Current browser does not support Local Storage');
        }
        this.localStorage = localStorage;
    }
    set(key, value) {
        this.localStorage[key] = value;
    }
    get(key) {
        return this.localStorage[key] || false;
    }
    setObject(key, value) {
        this.localStorage[key] = JSON.stringify(value);
    }
    getObject(key) {
        return JSON.parse(this.localStorage[key] || '{}');
    }
    remove(key) {
        this.localStorage.removeItem(key);
    }
}

class MessageSender {
    constructor() {
        this.emitChangeSource = new Subject();
    }
}
MessageSender.ɵfac = function MessageSender_Factory(t) { return new (t || MessageSender)(); };
MessageSender.ɵprov = /*@__PURE__*/ i0.ɵɵdefineInjectable({ token: MessageSender, factory: MessageSender.ɵfac, providedIn: 'root' });
(function () {
    (typeof ngDevMode === "undefined" || ngDevMode) && i0.ɵsetClassMetadata(MessageSender, [{
            type: Injectable,
            args: [{
                    providedIn: 'root',
                }]
        }], null, null);
})();

let heartTime;
let reConnectTimeOut;
class WebsocketService {
    constructor() { }
    init(url, params, resFunc, isReConnect) {
        if (!url) {
            return;
        }
        let webSocket;
        let noReConnect = false;
        webSocket = new WebSocket(url);
        webSocket.onmessage = (res) => __awaiter(this, void 0, void 0, function* () {
            const data = JSON.parse(res.data);
            resFunc(data);
            // this.buildData(data.MessageBody, params, resFunc);
        });
        webSocket.onopen = () => {
            console.log('webSocket connected');
            heartTime = setInterval(() => {
                // onmessage
                console.log('ping!');
                webSocket.send('ping');
            }, 20000);
            // webSocket.send(JSON.stringify(params));
        };
        webSocket.onclose = () => {
            if (isReConnect && !webSocket.noReConnect) {
                this.socketReConnect(url, params, resFunc, isReConnect);
            }
            heartTime && clearInterval(heartTime);
            console.log('webSocket closed');
        };
        webSocket.onerror = (res) => {
            if (isReConnect && webSocket.noReConnect) {
                this.socketReConnect(url, params, resFunc, isReConnect);
            }
            heartTime && clearInterval(heartTime);
            console.error(res);
        };
        return webSocket;
    }
    socketReConnect(url, params, resFunc, isReConnect) {
        // reconnect time 5s
        reConnectTimeOut && clearTimeout(reConnectTimeOut);
        reConnectTimeOut = setTimeout(() => {
            this.init(url, params, resFunc, isReConnect);
        }, 5000);
    }
}
WebsocketService.ɵfac = function WebsocketService_Factory(t) { return new (t || WebsocketService)(); };
WebsocketService.ɵprov = /*@__PURE__*/ i0.ɵɵdefineInjectable({ token: WebsocketService, factory: WebsocketService.ɵfac });
(function () {
    (typeof ngDevMode === "undefined" || ngDevMode) && i0.ɵsetClassMetadata(WebsocketService, [{
            type: Injectable
        }], function () { return []; }, null);
})();

/**
 * name:login service
 * describe: login common module
 */
class LoginService {
    constructor() { }
    /**
     * request
     * @param params
     * @returns {Promise<{success: boolean,msg: string}>|Promise<R>}
     */
    initLogin(params) {
        // this.loginParams = params
    }
    // public checkLogin() {
    //   if (window.location.toString().indexOf('/common/redirect?code=') >= 0) {
    //     localStorage.setItem('loginCheck', '0');
    //     this.isRedirect = true;
    //     this.checkOKTALogin();
    //     return;
    //   }
    //   let loginCheck = localStorage.getItem('loginCheck');
    //   this.isLogin = loginCheck === '1';
    // }
    //
    // private checkOKTALogin() {
    //   this.route.queryParams.subscribe(async (data: any) => {
    //     let code = data['code'];
    //     if (code) {
    //       const { data } = await this.http.getOKTAToken({ code });
    //       if (data) {
    //         this.handleLoginSuccess(data);
    //       } else {
    //         this.messageService.add({ severity: 'error', summary: 'OKTA Login Fail' });
    //         this.isRedirect = false;
    //       }
    //     }
    //   });
    // }
    //
    // async handleLoginSuccess(data: any) {
    //   localStorage.setItem('jwt', data.token);
    //   localStorage.setItem('roles', data.roles);
    //   localStorage.setItem('loginCheck', '1');
    //   localStorage.setItem('username', data.userName);
    //   const menu = await this.appMenuService.getMenuData(this.menuTreeKeys, this.menuTree);
    //   const url = menu[0]?.items ? menu[0].items[0].routerLink : menu[0].routerLink;
    //   localStorage.setItem('defaultUrl', url);
    //   this.isLogin = true;
    //   this.isRedirect = false;
    //   this.router.navigate([url]).then(() => {});
    // }
    static logout() {
        localStorage.removeItem('loginCheck');
        localStorage.removeItem('username');
        localStorage.removeItem('selectedIndex');
        localStorage.removeItem('jwt');
        localStorage.removeItem('tabs');
        localStorage.removeItem('tabMenu');
        localStorage.removeItem('region');
        localStorage.removeItem('currentRegion');
        localStorage.removeItem('translateApp');
        localStorage.removeItem('zh');
        localStorage.removeItem('en');
        window.location.reload();
    }
}
LoginService.ɵfac = function LoginService_Factory(t) { return new (t || LoginService)(); };
LoginService.ɵprov = /*@__PURE__*/ i0.ɵɵdefineInjectable({ token: LoginService, factory: LoginService.ɵfac });
(function () {
    (typeof ngDevMode === "undefined" || ngDevMode) && i0.ɵsetClassMetadata(LoginService, [{
            type: Injectable
        }], function () { return []; }, null);
})();

class JBService {
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
JBService.ɵfac = function JBService_Factory(t) { return new (t || JBService)(i0.ɵɵinject(HttpService)); };
JBService.ɵprov = /*@__PURE__*/ i0.ɵɵdefineInjectable({ token: JBService, factory: JBService.ɵfac });
(function () {
    (typeof ngDevMode === "undefined" || ngDevMode) && i0.ɵsetClassMetadata(JBService, [{
            type: Injectable
        }], function () { return [{ type: HttpService }]; }, null);
})();

class JBEventBusService {
    constructor() {
        // @ts-ignore
        if (!window.subject) {
            // @ts-ignore
            window.subject = new Subject();
        }
    }
    send(message) {
        // @ts-ignore
        window.subject.next(message);
    }
    get() {
        // @ts-ignore
        return window.subject.asObservable();
    }
}
JBEventBusService.ɵfac = function JBEventBusService_Factory(t) { return new (t || JBEventBusService)(); };
JBEventBusService.ɵprov = /*@__PURE__*/ i0.ɵɵdefineInjectable({ token: JBEventBusService, factory: JBEventBusService.ɵfac });
(function () {
    (typeof ngDevMode === "undefined" || ngDevMode) && i0.ɵsetClassMetadata(JBEventBusService, [{
            type: Injectable
        }], function () { return []; }, null);
})();

function sendEvent(eventName) {
    window.dispatchEvent(new CustomEvent(eventName));
}
function onListener(eventName, callback) {
    window.addEventListener(eventName, () => {
        callback();
    });
}

function setRouter(router) {
    // @ts-ignore
    window.router = router;
}
function getRouter() {
    // @ts-ignore
    return window.router;
}

class MissingTranslationsService {
    constructor(appInfo) {
        this.appInfo = appInfo;
    }
    handle(params) {
        var _a;
        const namespaces = (_a = this.appInfo.getApp()) === null || _a === void 0 ? void 0 : _a.namespaces;
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
MissingTranslationsService.ɵfac = function MissingTranslationsService_Factory(t) { return new (t || MissingTranslationsService)(i0.ɵɵinject(AppInfoService)); };
MissingTranslationsService.ɵprov = /*@__PURE__*/ i0.ɵɵdefineInjectable({ token: MissingTranslationsService, factory: MissingTranslationsService.ɵfac, providedIn: 'root' });
(function () {
    (typeof ngDevMode === "undefined" || ngDevMode) && i0.ɵsetClassMetadata(MissingTranslationsService, [{
            type: Injectable,
            args: [{
                    providedIn: 'root',
                }]
        }], function () { return [{ type: AppInfoService }]; }, null);
})();

class TranslationApiService {
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
        var _a;
        return (_a = this.appInfo.getApp()) === null || _a === void 0 ? void 0 : _a.languages.find((l) => l.code == code);
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
TranslationApiService.ɵfac = function TranslationApiService_Factory(t) { return new (t || TranslationApiService)(i0.ɵɵinject(AppInfoService)); };
TranslationApiService.ɵprov = /*@__PURE__*/ i0.ɵɵdefineInjectable({ token: TranslationApiService, factory: TranslationApiService.ɵfac, providedIn: 'root' });
(function () {
    (typeof ngDevMode === "undefined" || ngDevMode) && i0.ɵsetClassMetadata(TranslationApiService, [{
            type: Injectable,
            args: [{
                    providedIn: 'root',
                }]
        }], function () { return [{ type: AppInfoService }]; }, null);
})();

class TranslationApiLoader {
    constructor(api, translateJson) {
        this.api = api;
        this.translateJson = translateJson;
    }
    getTranslation(lang) {
        return this.api.getTranslationsFromApi(lang, this.translateJson);
    }
}

// import distanceInWordsToNow from 'date-fns/distance_in_words_to_now';
// import dateFns from 'date-fns';
class FormatTime {
    /**
     * transform datetime.
     */
    transform(value, ...args) {
        if (!value) {
            return '';
        }
        let result = new Date(value);
        // 24: getDay(): 获取传入的日期是星期几
        // 25: getMonth(): 返回传入时间的月份
        // 26: getMinutes(): 返回传入时间的分钟数
        // 27:getHours():返回传入时间的小时数
        const month = result.getMonth() + 1 < 10 ? '0' + (result.getMonth() + 1) : result.getMonth() + 1;
        const day = result.getDate() < 10 ? '0' + result.getDate() : result.getDate();
        const hour = result.getHours() < 10 ? '0' + result.getHours() : result.getHours();
        const minute = result.getMinutes() < 10 ? '0' + result.getMinutes() : result.getMinutes();
        return result.getFullYear() + '-' + month + '-' + day + ' ' + hour + ':' + minute;
        // return distanceInWordsToNow(new Date(value), { addSuffix: true });
    }
}
FormatTime.ɵfac = function FormatTime_Factory(t) { return new (t || FormatTime)(); };
FormatTime.ɵpipe = /*@__PURE__*/ i0.ɵɵdefinePipe({ name: "FormatTime", type: FormatTime, pure: true });
(function () {
    (typeof ngDevMode === "undefined" || ngDevMode) && i0.ɵsetClassMetadata(FormatTime, [{
            type: Pipe,
            args: [{
                    name: 'FormatTime',
                }]
        }], null, null);
})();

class FormatTimeModule {
}
FormatTimeModule.ɵfac = function FormatTimeModule_Factory(t) { return new (t || FormatTimeModule)(); };
FormatTimeModule.ɵmod = /*@__PURE__*/ i0.ɵɵdefineNgModule({ type: FormatTimeModule });
FormatTimeModule.ɵinj = /*@__PURE__*/ i0.ɵɵdefineInjector({ imports: [CommonModule] });
(function () {
    (typeof ngDevMode === "undefined" || ngDevMode) && i0.ɵsetClassMetadata(FormatTimeModule, [{
            type: NgModule,
            args: [{
                    declarations: [FormatTime],
                    imports: [
                        CommonModule,
                    ],
                    exports: [
                        FormatTime
                    ]
                }]
        }], null, null);
})();
(function () { (typeof ngJitMode === "undefined" || ngJitMode) && i0.ɵɵsetNgModuleScope(FormatTimeModule, { declarations: [FormatTime], imports: [CommonModule], exports: [FormatTime] }); })();

// import distanceInWordsToNow from 'date-fns/distance_in_words_to_now';
// import dateFns from 'date-fns';
class SecondToHour {
    /**
     * Takes a value and makes it lowercase.
     */
    transform(value, ...args) {
        if (!value) {
            return '';
        }
        //  秒
        let second = value;
        //  分
        let minute = 0;
        //  小时
        let hour = 0;
        //  天
        //  let day = 0
        //  如果秒数大于60，将秒数转换成整数
        if (second > 60) {
            //  获取分钟，除以60取整数，得到整数分钟
            minute = parseInt((second / 60).toString());
            //  获取秒数，秒数取佘，得到整数秒数
            second = parseInt((second % 60).toString());
            //  如果分钟大于60，将分钟转换成小时
            if (minute > 60) {
                //  获取小时，获取分钟除以60，得到整数小时
                hour = parseInt((minute / 60).toString());
                //  获取小时后取佘的分，获取分钟除以60取佘的分
                minute = parseInt((minute % 60).toString());
                //  如果小时大于24，将小时转换成天
                //  if (hour > 23) {
                //    //  获取天数，获取小时除以24，得到整天数
                //    day = parseInt(hour / 24)
                //    //  获取天数后取余的小时，获取小时除以24取余的小时
                //    hour = parseInt(hour % 24)
                //  }
            }
        }
        let result = '' + parseInt(second.toString()) + 'S';
        if (minute > 0) {
            result = '' + parseInt(minute.toString()) + 'M' + result;
        }
        if (hour > 0) {
            result = '' + parseInt(hour.toString()) + 'H' + result;
        }
        //  if (day > 0) {
        //    result = '' + parseInt(day) + '天' + result
        //  }
        return result;
    }
}
SecondToHour.ɵfac = function SecondToHour_Factory(t) { return new (t || SecondToHour)(); };
SecondToHour.ɵpipe = /*@__PURE__*/ i0.ɵɵdefinePipe({ name: "SecondToHour", type: SecondToHour, pure: true });
(function () {
    (typeof ngDevMode === "undefined" || ngDevMode) && i0.ɵsetClassMetadata(SecondToHour, [{
            type: Pipe,
            args: [{
                    name: 'SecondToHour',
                }]
        }], null, null);
})();

class SecondToHourModule {
}
SecondToHourModule.ɵfac = function SecondToHourModule_Factory(t) { return new (t || SecondToHourModule)(); };
SecondToHourModule.ɵmod = /*@__PURE__*/ i0.ɵɵdefineNgModule({ type: SecondToHourModule });
SecondToHourModule.ɵinj = /*@__PURE__*/ i0.ɵɵdefineInjector({ imports: [CommonModule] });
(function () {
    (typeof ngDevMode === "undefined" || ngDevMode) && i0.ɵsetClassMetadata(SecondToHourModule, [{
            type: NgModule,
            args: [{
                    declarations: [SecondToHour],
                    imports: [
                        CommonModule,
                    ],
                    exports: [
                        SecondToHour
                    ]
                }]
        }], null, null);
})();
(function () { (typeof ngJitMode === "undefined" || ngJitMode) && i0.ɵɵsetNgModuleScope(SecondToHourModule, { declarations: [SecondToHour], imports: [CommonModule], exports: [SecondToHour] }); })();

// components

/**
 * Generated bundle index. Do not edit.
 */

export { AppInfoService, AppInitService, AppMenuService, BigFileUploadService, CommonService, DefaultRouteGuardService, ExcelExportService, FileUploadComponent, FileUploadModule, FormatTime, FormatTimeModule, HttpService, JBEventBusService, JBService, LayoutComponent, LayoutModule, LoadingComponent, LoadingModule, LocalStorage, LoginService, MessageSender, MissingTranslationsService, QRComponent, QRModule, SecondToHour, SecondToHourModule, SimpleReuseStrategy, TranslationApiLoader, TranslationApiService, VideoDialogComponent, VideoDialogModule, WebsocketService, getRouter, onListener, sendEvent, setRouter };
//# sourceMappingURL=jabil-bus-lib-project.mjs.map
