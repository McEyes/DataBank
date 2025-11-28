import { Injectable } from '@angular/core';
import * as i0 from "@angular/core";
let localLayout = null;
export class CommonService {
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
            if (localLayout) {
                localLayout.fullPage = !exitFullScreen;
            }
            window?.parent?.postMessage({
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
        agrs?.forEach((item) => {
            argsStr += '\n' + item;
        });
        return newParamsName + argsStr;
    }
    addRouteEvent(router) {
        if (!router?.navigate) {
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
            if (data?.messageBody) {
                console.log(data?.messageBody);
            }
        });
        const start = async () => {
            try {
                await connection.start();
                console.log('SignalR Connected.');
            }
            catch (err) {
                console.log(err);
                setTimeout(start, 5000);
            }
        };
        connection.onclose(async () => {
            await start();
        });
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
        var notification = new Notification($options?.title, $options);
        notification.onshow = (event) => {
            console.log("show : ", event);
        };
        notification.onclose = (event) => {
            console.log("close : ", event);
        };
        notification.onclick = (event) => {
            console.log("click : ", event);
            // 当点击事件触发，打开指定的url
            event?.target?.data && window.open(event?.target?.data);
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
(function () { (typeof ngDevMode === "undefined" || ngDevMode) && i0.ɵsetClassMetadata(CommonService, [{
        type: Injectable
    }], null, null); })();
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiY29tbW9uLnNlcnZpY2UuanMiLCJzb3VyY2VSb290IjoiIiwic291cmNlcyI6WyIuLi8uLi8uLi8uLi8uLi8uLi9zcmMvYXBwL3NlcnZpY2UvY29tbW9uL2NvbW1vbi5zZXJ2aWNlLnRzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiJBQUFBLE9BQU8sRUFBRSxVQUFVLEVBQUUsTUFBTSxlQUFlLENBQUM7O0FBQzNDLElBQUksV0FBVyxHQUFRLElBQUksQ0FBQztBQUU1QixNQUFNLE9BQU8sYUFBYTtJQUQxQjtRQUVTLGVBQVUsR0FBRyxVQUFVLEdBQVEsRUFBRSxJQUFTO1lBQy9DLElBQUksR0FBRyxDQUFDO1lBQ1IsTUFBTSxHQUFHLEdBQVE7Z0JBQ2YsSUFBSSxFQUFFLElBQUksQ0FBQyxXQUFXLEVBQUUsQ0FBQyxRQUFRLEVBQUU7Z0JBQ25DLElBQUksRUFBRSxDQUFDLElBQUksQ0FBQyxRQUFRLEVBQUUsR0FBRyxDQUFDLENBQUMsQ0FBQyxRQUFRLEVBQUU7Z0JBQ3RDLElBQUksRUFBRSxJQUFJLENBQUMsT0FBTyxFQUFFLENBQUMsUUFBUSxFQUFFO2dCQUMvQixJQUFJLEVBQUUsSUFBSSxDQUFDLFFBQVEsRUFBRSxDQUFDLFFBQVEsRUFBRTtnQkFDaEMsSUFBSSxFQUFFLElBQUksQ0FBQyxVQUFVLEVBQUUsQ0FBQyxRQUFRLEVBQUU7Z0JBQ2xDLElBQUksRUFBRSxJQUFJLENBQUMsVUFBVSxFQUFFLENBQUMsUUFBUSxFQUFFO2FBQ25DLENBQUM7WUFDRixLQUFLLElBQUksQ0FBQyxJQUFJLEdBQUcsRUFBRTtnQkFDakIsR0FBRyxHQUFHLElBQUksTUFBTSxDQUFDLEdBQUcsR0FBRyxDQUFDLEdBQUcsR0FBRyxDQUFDLENBQUMsSUFBSSxDQUFDLEdBQUcsQ0FBQyxDQUFDO2dCQUMxQyxJQUFJLEdBQUcsRUFBRTtvQkFDUCxHQUFHLEdBQUcsR0FBRyxDQUFDLE9BQU8sQ0FDZixHQUFHLENBQUMsQ0FBQyxDQUFDLEVBQ04sR0FBRyxDQUFDLENBQUMsQ0FBQyxD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QWIsYUFBYTt1RkFBYixhQUFhO2NBRHpCLFVBQVUiLCJzb3VyY2VzQ29udGVudCI6WyJpbXBvcnQgeyBJbmplY3RhYmxlIH0gZnJvbSAnQGFuZ3VsYXIvY29yZSc7XHJcbmxldCBsb2NhbExheW91dDogYW55ID0gbnVsbDtcclxuQEluamVjdGFibGUoKVxyXG5leHBvcnQgY2xhc3MgQ29tbW9uU2VydmljZSB7XHJcbiAgcHVibGljIGRhdGVGb3JtYXQgPSBmdW5jdGlvbiAoZm10OiBhbnksIGRhdGU6IGFueSkge1xyXG4gICAgbGV0IHJldDtcclxuICAgIGNvbnN0IG9wdDogYW55ID0ge1xyXG4gICAgICAnWSsnOiBkYXRlLmdldEZ1bGxZZWFyKCkudG9TdHJpbmcoKSxcclxuICAgICAgJ20rJzogKGRhdGUuZ2V0TW9udGgoKSArIDEpLnRvU3RyaW5nKCksXHJcbiAgICAgICdkKyc6IGRhdGUuZ2V0RGF0ZSgpLnRvU3RyaW5nKCksXHJcbiAgICAgICdIKyc6IGRhdGUuZ2V0SG91cnMoKS50b1N0cmluZygpLFxyXG4gICAgICAnTSsnOiBkYXRlLmdldE1pbnV0ZXMoKS50b1N0cmluZygpLFxyXG4gICAgICAnUysnOiBkYXRlLmdldFNlY29uZHMoKS50b1N0cmluZygpLFxyXG4gICAgfTtcclxuICAgIGZvciAobGV0IGsgaW4gb3B0KSB7XHJcbiAgICAgIHJldCA9IG5ldyBSZWdFeHAoJygnICsgayArICcpJykuZXhlYyhmbXQpO1xyXG4gICAgICBpZiAocmV0KSB7XHJcbiAgICAgICAgZm10ID0gZm10LnJlcGxhY2UoXHJcbiAgICAgICAgICByZXRbMV0sXHJcbiAgICAgICAgICByZXRbMV0ubGVuZ3RoID09IDEgPyBvcHRba10gOiBvcHRba10ucGFkU3RhcnQocmV0WzFdLmxlbmd0aCwgJzAnKVxyXG4gICAgICAgICk7XHJcbiAgICAgIH1cclxuICAgIH1cclxuICAgIHJldHVybiBmbXQ7XHJcbiAgfVxyXG5cclxuICBwdWJsaWMgZGF0ZUZvcm1hdE5ldyA9IGZ1bmN0aW9uICh0eXBlOiBhbnksIG5vdzogYW55KSB7XHJcbiAgICAvLyB2YXIgbm93ID0gbmV3IERhdGUoKSxcclxuICAgIGxldCB5ID0gbm93LmdldEZ1bGxZZWFyKCk7XHJcbiAgICBsZXQgbSA9ICgnMCcgKyAobm93LmdldE1vbnRoKCkgKyAxKSkuc2xpY2UoLTIpO1xyXG4gICAgbGV0IGQgPSAoJzAnICsgbm93LmdldERhdGUoKSkuc2xpY2UoLTIpO1xyXG4gICAgaWYgKHR5cGUgPT0gJ1ktbW0tZGQgMDA6MDA6MDAnKSB7XHJcbiAgICAgIGxldCB0ZW1wID0geSArICcvJyArIG0gKyAnLycgKyBkICsgJyAwMDowMDowMCc7XHJcbiAgICAgIHRlbXAgPSB0ZW1wLnJlcGxhY2UoLy0vZywgJy8nKTtcclxuICAgICAgcmV0dXJuIHRlbXA7XHJcbiAgICB9IGVsc2Uge1xyXG4gICAgICBsZXQgdGVtcCA9IHkgKyAnLycgKyBtICsgJy8nICsgZCArICcgMjM6NTk6NTknO1xyXG4gICAgICB0ZW1wID0gdGVtcC5yZXBsYWNlKC8tL2csICcvJyk7XHJcbiAgICAgIHJldHVybiB0ZW1wO1xyXG4gICAgfVxyXG4gIH1cclxuXHJcbiAgcHVibGljIGJ1aWxkRUNoYXJ0TGFiZWwocGFyYW1zOiBzdHJpbmcsIG4/OiBudW1iZXIsIGFncnM/OiBBcnJheTxhbnk+KSB7XHJcbiAgICBpZiAoIXBhcmFtcykge1xyXG4gICAgICByZXR1cm47XHJcbiAgICB9XHJcbiAgICBsZXQgbmV3UGFyYW1zTmFtZSA9ICcnOyAvLyDmnIDnu4jmi7zmjqXmiJDnmoTlrZfnrKbkuLJcclxuICAgIGxldCBwYXJhbXNOYW1lTnVtYmVyID0gcGFyYW1zLmxlbmd0aDsgLy8g5a6e6ZmF5qCH562+55qE5Liq5pWwXHJcbiAgICBjb25zdCBwcm92aWRlTnVtYmVyID0gbiB8fCA0OyAvLyDmr4/ooYzog73mmL7npLrnmoTlrZfnmoTkuKrmlbBcclxuICAgIGNvbnN0IHJvd051bWJlciA9IE1hdGguY2VpbChwYXJhbXNOYW1lTnVtYmVyIC8gcHJvdmlkZU51bWJlcik7IC8vIOaNouihjOeahOivne+8jOmcgOimgeaYvuekuuWHoOihjO+8jOWQkeS4iuWPluaVtFxyXG4gICAgLyoqXHJcbiAgICAgKiDliKTmlq3moIfnrb7nmoTkuKrmlbDmmK/lkKblpKfkuo7op4TlrprnmoTkuKrmlbDvvIwg5aaC5p6c5aSn5LqO77yM5YiZ6L+b6KGM5o2i6KGM5aSE55CGIOWmguaenOS4jeWkp+S6ju+8jOWNs+etieS6juaIluWwj+S6ju+8jOWwsei/lOWbnuWOn+agh+etvlxyXG4gICAgICovXHJcbiAgICAvLyDmnaHku7bnrYnlkIzkuo5yb3dOdW1iZXI+MVxyXG4gICAgaWYgKHBhcmFtc05hbWVOdW1iZXIgPiBwcm92aWRlTnVtYmVyKSB7XHJcbiAgICAgIC8qKiDlvqrnjq/mr4/kuIDooYwscOihqOekuuihjCAqL1xyXG4gICAgICBmb3IgKHZhciBwID0gMDsgcCA8IHJvd051bWJlcjsgcCsrKSB7XHJcbiAgICAgICAgdmFyIHRlbXBTdHIgPSAnJzsgLy8g6KGo56S65q+P5LiA5qyh5oiq5Y+W55qE5a2X56ym5LiyXHJcbiAgICAgICAgdmFyIHN0YXJ0ID0gcCAqIHByb3ZpZGVOdW1iZXI7IC8vIOW8gOWni+aIquWPlueahOS9jee9rlxyXG4gICAgICAgIHZhciBlbmQgPSBzdGFydCArIHByb3ZpZGVOdW1iZXI7IC8vIOe7k+adn+aIquWPlueahOS9jee9rlxyXG4gICAgICAgIC8vIOatpOWkhOeJueauiuWkhOeQhuacgOWQjuS4gOihjOeahOe0ouW8leWAvFxyXG4gICAgICAgIGlmIChwID09IHJvd051bWJlciAtIDEpIHtcclxuICAgICAgICAgIC8vIOacgOWQjuS4gOasoeS4jeaNouihjFxyXG4gICAgICAgICAgdGVtcFN0ciA9IHBhcmFtcy5zdWJzdHJpbmcoc3RhcnQsIHBhcmFtc05hbWVOdW1iZXIpO1xyXG4gICAgICAgIH0gZWxzZSB7XHJcbiAgICAgICAgICAvLyDmr4/kuIDmrKHmi7zmjqXlrZfnrKbkuLLlubbmjaLooYxcclxuICAgICAgICAgIHRlbXBTdHIgPSBwYXJhbXMuc3Vic3RyaW5nKHN0YXJ0LCBlbmQpICsgJ1xcbic7XHJcbiAgICAgICAgfVxyXG4gICAgICAgIG5ld1BhcmFtc05hbWUgKz0gdGVtcFN0cjsgLy8g5pyA57uI5ou85oiQ55qE5a2X56ym5LiyXHJcbiAgICAgIH1cclxuICAgIH0gZWxzZSB7XHJcbiAgICAgIC8vIOWwhuaXp+agh+etvueahOWAvOi1i+e7meaWsOagh+etvlxyXG4gICAgICBuZXdQYXJhbXNOYW1lID0gcGFyYW1zO1xyXG4gICAgfVxyXG4gICAgLy/lsIbmnIDnu4jnmoTlrZfnrKbkuLLov5Tlm55cclxuICAgIGxldCBhcmdzU3RyID0gJyc7XHJcbiAgICBhZ3JzPy5mb3JFYWNoKChpdGVtOiBzdHJpbmcpID0+IHtcclxuICAgICAgYXJnc1N0ciArPSAnXFxuJyArIGl0ZW07XHJcbiAgICB9KTtcclxuICAgIHJldHVybiBuZXdQYXJhbXNOYW1lICsgYXJnc1N0cjtcclxuICB9XHJcblxyXG4gIHB1YmxpYyBnZW5jbGFzcyA9IGZ1bmN0aW9uICh1cmw6IHN0cmluZykge1xyXG4gICAgaWYgKHVybCA9PSAnJyB8fCB1cmwgPT0gbnVsbCkge1xyXG4gICAgICByZXR1cm4gJyc7XHJcbiAgICB9IGVsc2Uge1xyXG4gICAgICBsZXQgdXJsYXJyID0gdXJsLnRvTG93ZXJDYXNlKCkuc3BsaXQoJy4nKTtcclxuICAgICAgbGV0IGV4dCA9IHVybGFyclt1cmxhcnIubGVuZ3RoIC0gMV07XHJcbiAgICAgIGxldCBsb2dvdHlwZSA9ICcnO1xyXG4gICAgICBzd2l0Y2ggKGV4dCkge1xyXG4gICAgICAgIGNhc2UgJ3hscyc6XHJcbiAgICAgICAgICBsb2dvdHlwZSA9ICdmYSBmYS1maWxlLWV4Y2VsIGZhLTR4IGdyZWVuJztcclxuICAgICAgICAgIGJyZWFrO1xyXG4gICAgICAgIGNhc2UgJ3hsc3gnOlxyXG4gICAgICAgICAgbG9nb3R5cGUgPSAnZmEgZmEtZmlsZS1leGNlbCBmYS00eCBncmVlbic7XHJcbiAgICAgICAgICBicmVhaztcclxuICAgICAgICBjYXNlICdkb2MnOlxyXG4gICAgICAgICAgbG9nb3R5cGUgPSAnZmEgZmEtZmlsZS13b3JkIGZhLTR4IGJsdWUnO1xyXG4gICAgICAgICAgYnJlYWs7XHJcbiAgICAgICAgY2FzZSAnZG9jeCc6XHJcbiAgICAgICAgICBsb2dvdHlwZSA9ICdmYSBmYS1maWxlLXdvcmQgZmEtNHggYmx1ZSc7XHJcbiAgICAgICAgICBicmVhaztcclxuICAgICAgICBjYXNlICdwcHQnOlxyXG4gICAgICAgICAgbG9nb3R5cGUgPSAnZmEgZmEtZmlsZS1wb3dlcnBvaW50IGZhLTR4IHJlZCc7XHJcbiAgICAgICAgICBicmVhaztcclxuICAgICAgICBjYXNlICdwcHR4JzpcclxuICAgICAgICAgIGxvZ290eXBlID0gJ2ZhIGZhLWZpbGUtcG93ZXJwb2ludCBmYS00eCByZWQnO1xyXG4gICAgICAgICAgYnJlYWs7XHJcbiAgICAgICAgY2FzZSAncHB0JzpcclxuICAgICAgICAgIGxvZ290eXBlID0gJ2ZhIGZhLWZpbGUtcGRmIGZhLTR4IHJlZCc7XHJcbiAgICAgICAgICBicmVhaztcclxuICAgICAgICBjYXNlICdwZGYnOlxyXG4gICAgICAgICAgbG9nb3R5cGUgPSAnZmEgZmEtZmlsZS1wZGYgZmEtNHggcmVkJztcclxuICAgICAgICAgIGJyZWFrO1xyXG4gICAgICAgIGRlZmF1bHQ6XHJcbiAgICAgICAgICBsb2dvdHlwZSA9ICdmYSBmYS1maWxlIGZhLTR4JztcclxuICAgICAgfVxyXG4gICAgICByZXR1cm4gbG9nb3R5cGU7XHJcbiAgICB9XHJcbiAgfVxyXG5cclxuICBwdWJsaWMgaXNJbnZhbGlkID0gZnVuY3Rpb24gKG9iajogYW55LCBsaXN0OiBBcnJheTxhbnk+KTogYm9vbGVhbiB7XHJcbiAgICBmb3IgKGxldCBpID0gMDsgaSA8IGxpc3QubGVuZ3RoOyBpKyspIHtcclxuICAgICAgaWYgKGxpc3RbaV0gaW4gb2JqICYmICFvYmpbbGlzdFtpXV0pIHtcclxuICAgICAgICByZXR1cm4gdHJ1ZTtcclxuICAgICAgfVxyXG4gICAgfVxyXG5cclxuICAgIHJldHVybiBmYWxzZTtcclxuICB9XHJcblxyXG4gIHB1YmxpYyBnZXROZXh0RGF0ZSA9IGZ1bmN0aW9uIChkYXRlOiBhbnksIGRheSA9IDEsIGZvcm1hdCA9ICd7eX0te219LXtkfScpIHtcclxuICAgIGlmIChkYXRlKSB7XHJcbiAgICAgIGNvbnN0IG5EYXRlID0gbmV3IERhdGUoZGF0ZSk7XHJcbiAgICAgIG5EYXRlLnNldERhdGUobkRhdGUuZ2V0RGF0ZSgpICsgZGF5KTtcclxuXHJcbiAgICAgIGNvbnN0IGZvcm1hdE9iajogYW55ID0ge1xyXG4gICAgICAgIHk6IG5EYXRlLmdldEZ1bGxZZWFyKCksXHJcbiAgICAgICAgbTogbkRhdGUuZ2V0TW9udGgoKSArIDEsXHJcbiAgICAgICAgZDogbkRhdGUuZ2V0RGF0ZSgpLFxyXG4gICAgICB9O1xyXG4gICAgICByZXR1cm4gZm9ybWF0LnJlcGxhY2UoL3soW3ltZF0pK30vZywgKHJlc3VsdCwga2V5KSA9PiB7XHJcbiAgICAgICAgY29uc3QgdmFsdWUgPSBmb3JtYXRPYmpba2V5XTtcclxuICAgICAgICByZXR1cm4gdmFsdWUudG9TdHJpbmcoKS5wYWRTdGFydCgyLCAnMCcpO1xyXG4gICAgICB9KTtcclxuICAgIH0gZWxzZSB7XHJcbiAgICAgIGNvbnNvbGUubG9nKCdkYXRlIGNhbiBub3QgYmUgbnVsbCcpO1xyXG4gICAgICByZXR1cm47XHJcbiAgICB9XHJcbiAgfVxyXG5cclxuICBwdWJsaWMgc2Vjb25kMkhvdXIgPSBmdW5jdGlvbiAodmFsdWU6IG51bWJlcikge1xyXG4gICAgLy8gIOenklxyXG4gICAgbGV0IHNlY29uZDogbnVtYmVyID0gdmFsdWU7XHJcbiAgICAvLyAg5YiGXHJcbiAgICBsZXQgbWludXRlID0gMDtcclxuICAgIC8vICDlsI/ml7ZcclxuICAgIGxldCBob3VyID0gMDtcclxuICAgIC8vICDlpKlcclxuICAgIC8vICBsZXQgZGF5ID0gMFxyXG4gICAgLy8gIOWmguaenOenkuaVsOWkp+S6jjYw77yM5bCG56eS5pWw6L2s5o2i5oiQ5pW05pWwXHJcbiAgICBpZiAoc2Vjb25kID4gNjApIHtcclxuICAgICAgLy8gIOiOt+WPluWIhumSn++8jOmZpOS7pTYw5Y+W5pW05pWw77yM5b6X5Yiw5pW05pWw5YiG6ZKfXHJcbiAgICAgIG1pbnV0ZSA9IHBhcnNlSW50KChzZWNvbmQgLyA2MCkudG9TdHJpbmcoKSk7XHJcbiAgICAgIC8vICDojrflj5bnp5LmlbDvvIznp5LmlbDlj5bkvZjvvIzlvpfliLDmlbTmlbDnp5LmlbBcclxuICAgICAgc2Vjb25kID0gcGFyc2VJbnQoKHNlY29uZCAlIDYwKS50b1N0cmluZygpKTtcclxuICAgICAgLy8gIOWmguaenOWIhumSn+Wkp+S6jjYw77yM5bCG5YiG6ZKf6L2s5o2i5oiQ5bCP5pe2XHJcbiAgICAgIGlmIChtaW51dGUgPiA2MCkge1xyXG4gICAgICAgIC8vICDojrflj5blsI/ml7bvvIzojrflj5bliIbpkp/pmaTku6U2MO+8jOW+l+WIsOaVtOaVsOWwj+aXtlxyXG4gICAgICAgIGhvdXIgPSBwYXJzZUludCgobWludXRlIC8gNjApLnRvU3RyaW5nKCkpO1xyXG4gICAgICAgIC8vICDojrflj5blsI/ml7blkI7lj5bkvZjnmoTliIbvvIzojrflj5bliIbpkp/pmaTku6U2MOWPluS9mOeahOWIhlxyXG4gICAgICAgIG1pbnV0ZSA9IHBhcnNlSW50KChtaW51dGUgJSA2MCkudG9TdHJpbmcoKSk7XHJcbiAgICAgICAgLy8gIOWmguaenOWwj+aXtuWkp+S6jjI077yM5bCG5bCP5pe26L2s5o2i5oiQ5aSpXHJcbiAgICAgICAgLy8gIGlmIChob3VyID4gMjMpIHtcclxuICAgICAgICAvLyAgICAvLyAg6I635Y+W5aSp5pWw77yM6I635Y+W5bCP5pe26Zmk5LulMjTvvIzlvpfliLDmlbTlpKnmlbBcclxuICAgICAgICAvLyAgICBkYXkgPSBwYXJzZUludChob3VyIC8gMjQpXHJcbiAgICAgICAgLy8gICAgLy8gIOiOt+WPluWkqeaVsOWQjuWPluS9meeahOWwj+aXtu+8jOiOt+WPluWwj+aXtumZpOS7pTI05Y+W5L2Z55qE5bCP5pe2XHJcbiAgICAgICAgLy8gICAgaG91ciA9IHBhcnNlSW50KGhvdXIgJSAyNClcclxuICAgICAgICAvLyAgfVxyXG4gICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgbGV0IHJlc3VsdCA9ICcnICsgcGFyc2VJbnQoc2Vjb25kLnRvU3RyaW5nKCkpICsgJ1MnO1xyXG4gICAgaWYgKG1pbnV0ZSA+IDApIHtcclxuICAgICAgcmVzdWx0ID0gJycgKyBwYXJzZUludChtaW51dGUudG9TdHJpbmcoKSkgKyAnTScgKyByZXN1bHQ7XHJcbiAgICB9XHJcbiAgICBpZiAoaG91ciA+IDApIHtcclxuICAgICAgcmVzdWx0ID0gJycgKyBwYXJzZUludChob3VyLnRvU3RyaW5nKCkpICsgJ0gnICsgcmVzdWx0O1xyXG4gICAgfVxyXG4gICAgLy8gIGlmIChkYXkgPiAwKSB7XHJcbiAgICAvLyAgICByZXN1bHQgPSAnJyArIHBhcnNlSW50KGRheSkgKyAn5aSpJyArIHJlc3VsdFxyXG4gICAgLy8gIH1cclxuICAgIHJldHVybiByZXN1bHQ7XHJcbiAgfVxyXG5cclxuICAvKipcclxuICAgKiBBcnJheSB0cmFuc2Zvcm0gdG8gdHJlZVxyXG4gICAqIEBwYXJhbSB7YXJyYXl9IGRhdGFcclxuICAgKiBAcGFyYW0ge3N0cmluZ30gaWRcclxuICAgKi9cclxuICAvLyDmlbDnu4TovazmjaLmiJB0cmVlXHJcbiAgcHVibGljIGFycmF5VG9UcmVlID0gZnVuY3Rpb24gKGRhdGE6IGFueSwgaWQ6IHN0cmluZykge1xyXG4gICAgY29uc3QgcmVzdWx0OiBhbnlbXSA9IFtdO1xyXG4gICAgaWYgKCFBcnJheS5pc0FycmF5KGRhdGEpKSB7XHJcbiAgICAgIHJldHVybiByZXN1bHQ7XHJcbiAgICB9XHJcbiAgICBjb25zdCBtYXAgPSB7fTtcclxuICAgIGRhdGEuZm9yRWFjaChpdGVtID0+IHtcclxuICAgICAgZGVsZXRlIGl0ZW0uY2hpbGRyZW47XHJcbiAgICAgIC8vIEB0cy1pZ25vcmVcclxuICAgICAgbWFwW2l0ZW1baWRdXSA9IGl0ZW07XHJcbiAgICB9KTtcclxuXHJcbiAgICBkYXRhLmZvckVhY2goaXRlbSA9PiB7XHJcbiAgICAgIC8vIEB0cy1pZ25vcmVcclxuICAgICAgY29uc3QgcGFyZW50ID0gbWFwW2l0ZW0ucGFyZW50SWRdIHx8IG1hcFtpdGVtLnBhcmVudENvZGVdO1xyXG5cclxuICAgICAgaWYgKHBhcmVudCkge1xyXG4gICAgICAgIChwYXJlbnQuY2hpbGRyZW4gfHwgKHBhcmVudC5jaGlsZHJlbiA9IFtdKSkucHVzaChpdGVtKTtcclxuICAgICAgfSBlbHNlIHtcclxuICAgICAgICByZXN1bHQucHVzaChpdGVtKTtcclxuICAgICAgfVxyXG4gICAgfSk7XHJcbiAgICByZXR1cm4gcmVzdWx0O1xyXG4gIH1cclxuXHJcbiAgLyoqXHJcbiAgICogVHJlZSB0cmFuc2Zvcm0gdG8gYXJyYXlcclxuICAgKiBAcGFyYW0gdHJlZVxyXG4gICAqL1xyXG4gIC8vIFRyZWUgdG8gYXJyYXlcclxuICBwdWJsaWMgdHJlZVRvQXJyYXkgPSBmdW5jdGlvbiAodHJlZTogYW55KSB7XHJcbiAgICBjb25zdCBhcnI6IGFueVtdID0gW107XHJcbiAgICBjb25zdCBleHBhbmRlZCA9IChkYXRhczogYW55KSA9PiB7XHJcbiAgICAgIGlmIChkYXRhcyAmJiBkYXRhcy5sZW5ndGggPiAwKSB7XHJcbiAgICAgICAgZGF0YXMuZm9yRWFjaCgoZTogYW55KSA9PiB7XHJcbiAgICAgICAgICBhcnIucHVzaChlKTtcclxuICAgICAgICAgIGV4cGFuZGVkKGUuY2hpbGRyZW4pO1xyXG4gICAgICAgIH0pO1xyXG4gICAgICB9XHJcbiAgICB9O1xyXG4gICAgZXhwYW5kZWQodHJlZSk7XHJcbiAgICByZXR1cm4gYXJyO1xyXG4gIH1cclxuXHJcbiAgcHVibGljIHByaW50TmV3V2luZG93ID0gZnVuY3Rpb24gKGltZ1NyYzogc3RyaW5nLCBvcHRpb24/OiBhbnkpIHtcclxuICAgIGxldCBvV2luOiBhbnkgPSB3aW5kb3cub3BlbihcclxuICAgICAgJycsXHJcbiAgICAgICdwcmluZ3dpbmRvdycsXHJcbiAgICAgICdtZW51YmFyPW5vLGxvY2F0aW9uPW5vLHJlc2l6YWJsZT15ZXMsc2Nyb2xsYmFycz1ubyxzdGF0dXM9bm8sd2lkdGg9JyArXHJcbiAgICAgIHdpbmRvdy5zY3JlZW4uYXZhaWxXaWR0aCArXHJcbiAgICAgICcsaGVpZ2h0PScgK1xyXG4gICAgICB3aW5kb3cuc2NyZWVuLmF2YWlsSGVpZ2h0XHJcbiAgICApO1xyXG4gICAgb1dpbi5kb2N1bWVudC5mbiA9IGZ1bmN0aW9uICgpIHtcclxuICAgICAgaWYgKG9XaW4pIHtcclxuICAgICAgICBvV2luLnByaW50KCk7XHJcbiAgICAgICAgb1dpbi5jbG9zZSgpO1xyXG4gICAgICB9XHJcbiAgICB9O1xyXG4gICAgbGV0IHRpdGxlID0gJyc7XHJcbiAgICBpZiAob3B0aW9uICYmIG9wdGlvbi50aXRsZSkge1xyXG4gICAgICB0aXRsZSA9XHJcbiAgICAgICAgJzxoMyBzdHlsZT1cIndpZHRoOiAxMDAlO3RleHQtYWxpZ246IGNlbnRlcjttYXJnaW46IDEwMHB4IDA7Zm9udC1zaXplOiAyNHB4O1wiPicgK1xyXG4gICAgICAgIG9wdGlvbi50aXRsZSArXHJcbiAgICAgICAgJzwvaDM+JztcclxuICAgIH1cclxuICAgIGxldCBodG1sID1cclxuICAgICAgJzxkaXYgc3R5bGU9XCJoZWlnaHQ6IDEwMCU7d2lkdGg6IDEwMCU7ZGlzcGxheTogZmxleDtmbGV4LWRpcmVjdGlvbjogY29sdW1uO2FsaWduLWl0ZW1zOiBjZW50ZXJcIj4nICtcclxuICAgICAgdGl0bGUgK1xyXG4gICAgICBgPGltZyBzcmM9XCIke2ltZ1NyY31cIiBvbmxvYWQ9XCJmbigpXCIgc3R5bGU9XCJ3aWR0aDogOTAlO1wiIC8+YCArXHJcbiAgICAgICc8L2Rpdj4nICtcclxuICAgICAgJzxzdHlsZSB0eXBlPVwidGV4dC9jc3NcIj4nICtcclxuICAgICAgJ0BwYWdlIHsgbWFyZ2luOiAwOyB9JyArXHJcbiAgICAgICc8L3N0eWxlPic7XHJcbiAgICBvV2luLmRvY3VtZW50Lm9wZW4oKTtcclxuICAgIG9XaW4uZG9jdW1lbnQud3JpdGUoaHRtbCk7XHJcbiAgICBvV2luLmRvY3VtZW50LmNsb3NlKCk7XHJcbiAgfVxyXG5cclxuICBnZXRIb3VyQnlNaW51dGUgPSBmdW5jdGlvbiAobWludXRlOiBudW1iZXIpIHtcclxuICAgIHJldHVybiBNYXRoLnRydW5jKG1pbnV0ZSAvIDYwKTtcclxuICB9XHJcblxyXG4gIHB1YmxpYyBmb3JtSW52YWxpZCA9IGZ1bmN0aW9uIChvYmo6IGFueSwgbGlzdDogQXJyYXk8YW55Pik6IGJvb2xlYW4ge1xyXG4gICAgZm9yIChsZXQgaSA9IDA7IGkgPCBsaXN0Lmxlbmd0aDsgaSsrKSB7XHJcbiAgICAgIGlmIChsaXN0W2ldIGluIG9iaiAmJiAhb2JqW2xpc3RbaV1dKSB7XHJcbiAgICAgICAgcmV0dXJuIGxpc3RbaV07XHJcbiAgICAgIH0gZWxzZSBpZiAoXHJcbiAgICAgICAgT2JqZWN0LnByb3RvdHlwZS50b1N0cmluZy5jYWxsKG9ialtsaXN0W2ldXSkgPT09ICdbb2JqZWN0IEFycmF5XScgJiZcclxuICAgICAgICBvYmpbbGlzdFtpXV0ubGVuZ3RoID09PSAwXHJcbiAgICAgICkge1xyXG4gICAgICAgIHJldHVybiBsaXN0W2ldO1xyXG4gICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgcmV0dXJuIGZhbHNlO1xyXG4gIH1cclxuXHJcbiAgcHVibGljIGdldEJhc2U2NCA9IChmaWxlOiBGaWxlKTogUHJvbWlzZTxzdHJpbmcgfCBBcnJheUJ1ZmZlciB8IG51bGw+ID0+XHJcbiAgICBuZXcgUHJvbWlzZSgocmVzb2x2ZSwgcmVqZWN0KSA9PiB7XHJcbiAgICAgIGNvbnN0IHJlYWRlciA9IG5ldyBGaWxlUmVhZGVyKCk7XHJcbiAgICAgIHJlYWRlci5yZWFkQXNEYXRhVVJMKGZpbGUpO1xyXG4gICAgICByZWFkZXIub25sb2FkID0gKCkgPT4gcmVzb2x2ZShyZWFkZXIucmVzdWx0KTtcclxuICAgICAgcmVhZGVyLm9uZXJyb3IgPSBlcnJvciA9PiByZWplY3QoZXJyb3IpO1xyXG4gICAgfSk7XHJcblxyXG4gIHB1YmxpYyBkb3dubG9hZEZpbGUgPSBmdW5jdGlvbiAodXJsU3RyOiBzdHJpbmcsIGZpbGVOYW1lOiBzdHJpbmcpIHtcclxuICAgIGNvbnN0IGxpbmsgPSBkb2N1bWVudC5jcmVhdGVFbGVtZW50KCdhJyk7XHJcbiAgICBjb25zdCB1cmwgPSAnL2dhdGV3YXkvbWZndHJhbm5pbmcvYXBpL21mZy9Db21tb24vZmlsZT9vYmplY3ROYW1lPScgKyB1cmxTdHI7XHJcbiAgICBsaW5rLnN0eWxlLmRpc3BsYXkgPSAnbm9uZSc7XHJcbiAgICBsaW5rLmhyZWYgPSB1cmw7XHJcbiAgICBsaW5rLnNldEF0dHJpYnV0ZSgnZG93bmxvYWQnLCBmaWxlTmFtZSk7XHJcbiAgICBkb2N1bWVudC5ib2R5LmFwcGVuZENoaWxkKGxpbmspO1xyXG4gICAgbGluay5jbGljaygpO1xyXG4gICAgZG9jdW1lbnQuYm9keS5yZW1vdmVDaGlsZChsaW5rKTtcclxuICB9XHJcblxyXG4gIHB1YmxpYyBvcHRpb25zVmFsaWQgPSBmdW5jdGlvbiAob3B0aW9uTGlzdDogQXJyYXk8YW55PiwgdHlwZTogYW55LCBtZXNzYWdlU2VydmljZTogYW55LCBUcmFuc2xhdGVEYXRhOiBhbnkpIHtcclxuICAgIGNvbnN0IG9wdGlvbkNvdW50ID0gb3B0aW9uTGlzdC5sZW5ndGg7XHJcbiAgICBsZXQgWUNvdW50ID0gMDtcclxuXHJcbiAgICBpZiAob3B0aW9uQ291bnQgPT09IDApIHtcclxuICAgICAgbWVzc2FnZVNlcnZpY2UuYWRkKHsgc2V2ZXJpdHk6ICd3YXJuJywgc3VtbWFyeTogVHJhbnNsYXRlRGF0YS5xdWVzdGlvbk9wdGlvbjFXYXJuIH0pO1xyXG5cclxuICAgICAgcmV0dXJuIGZhbHNlO1xyXG4gICAgfVxyXG5cclxuICAgIG9wdGlvbkxpc3QuZm9yRWFjaChpdGVtID0+IHtcclxuICAgICAgaWYgKGl0ZW0uYW5zd2VyKSB7XHJcbiAgICAgICAgWUNvdW50Kys7XHJcbiAgICAgIH1cclxuICAgIH0pO1xyXG5cclxuICAgIGlmICh0eXBlID09PSAnMScpIHtcclxuICAgICAgLy8gIOWIpOaWremimFxyXG4gICAgICBpZiAob3B0aW9uQ291bnQgIT09IDIpIHtcclxuICAgICAgICBtZXNzYWdlU2VydmljZS5hZGQoe1xyXG4gICAgICAgICAgc2V2ZXJpdHk6ICd3YXJuJyxcclxuICAgICAgICAgIHN1bW1hcnk6IFRyYW5zbGF0ZURhdGEucXVlc3Rpb25PcHRpb24yV2FybixcclxuICAgICAgICB9KTtcclxuICAgICAgICByZXR1cm4gZmFsc2U7XHJcbiAgICAgIH1cclxuXHJcbiAgICAgIGlmIChZQ291bnQgIT09IDEpIHtcclxuICAgICAgICBtZXNzYWdlU2VydmljZS5hZGQoe1xyXG4gICAgICAgICAgc2V2ZXJpdHk6ICd3YXJuJyxcclxuICAgICAgICAgIHN1bW1hcnk6IFRyYW5zbGF0ZURhdGEucXVlc3Rpb25PcHRpb24zV2FybixcclxuICAgICAgICB9KTtcclxuICAgICAgICByZXR1cm4gZmFsc2U7XHJcbiAgICAgIH1cclxuICAgIH0gZWxzZSBpZiAodHlwZSA9PT0gJzInKSB7XHJcbiAgICAgIGlmIChvcHRpb25Db3VudCA8IDMpIHtcclxuICAgICAgICBtZXNzYWdlU2VydmljZS5hZGQoe1xyXG4gICAgICAgICAgc2V2ZXJpdHk6ICd3YXJuJyxcclxuICAgICAgICAgIHN1bW1hcnk6IFRyYW5zbGF0ZURhdGEucXVlc3Rpb25PcHRpb240V2FybixcclxuICAgICAgICB9KTtcclxuICAgICAgICByZXR1cm4gZmFsc2U7XHJcbiAgICAgIH1cclxuXHJcbiAgICAgIGlmIChZQ291bnQgIT09IDEpIHtcclxuICAgICAgICBtZXNzYWdlU2VydmljZS5hZGQoe1xyXG4gICAgICAgICAgc2V2ZXJpdHk6ICd3YXJuJyxcclxuICAgICAgICAgIHN1bW1hcnk6IFRyYW5zbGF0ZURhdGEucXVlc3Rpb25PcHRpb241V2FybixcclxuICAgICAgICB9KTtcclxuICAgICAgICByZXR1cm4gZmFsc2U7XHJcbiAgICAgIH1cclxuICAgIH0gZWxzZSBpZiAodHlwZSA9PT0gJzMnKSB7XHJcbiAgICAgIC8vIOWkmumAiVxyXG4gICAgICBpZiAob3B0aW9uQ291bnQgPCAzKSB7XHJcbiAgICAgICAgbWVzc2FnZVNlcnZpY2UuYWRkKHtcclxuICAgICAgICAgIHNldmVyaXR5OiAnd2FybicsXHJcbiAgICAgICAgICBzdW1tYXJ5OiBUcmFuc2xhdGVEYXRhLnF1ZXN0aW9uT3B0aW9uNFdhcm4sXHJcbiAgICAgICAgfSk7XHJcbiAgICAgICAgcmV0dXJuIGZhbHNlO1xyXG4gICAgICB9XHJcblxyXG4gICAgICBpZiAoWUNvdW50IDw9IDEpIHtcclxuICAgICAgICBtZXNzYWdlU2VydmljZS5hZGQoe1xyXG4gICAgICAgICAgc2V2ZXJpdHk6ICd3YXJuJyxcclxuICAgICAgICAgIHN1bW1hcnk6IFRyYW5zbGF0ZURhdGEucXVlc3Rpb25PcHRpb242V2FybixcclxuICAgICAgICB9KTtcclxuICAgICAgICByZXR1cm4gZmFsc2U7XHJcbiAgICAgIH1cclxuICAgIH1cclxuICAgIHJldHVybiB0cnVlO1xyXG4gIH1cclxuXHJcbiAgcHVibGljIHRyYW5zbGF0ZURhdGEgPSBmdW5jdGlvbiAoT2JqOiBhbnksIHRyYW5zbGF0ZTogYW55KSB7XHJcbiAgICBmb3IgKGxldCBpdGVtIGluIE9iaikge1xyXG4gICAgICBpZiAoT2JqLmhhc093blByb3BlcnR5KGl0ZW0pKSB7XHJcbiAgICAgICAgdHJhbnNsYXRlLmdldChPYmpbaXRlbV0pLnN1YnNjcmliZSgoZGF0YTogc3RyaW5nKSA9PiB7XHJcbiAgICAgICAgICBPYmpbaXRlbV0gPSBkYXRhO1xyXG4gICAgICAgIH0pO1xyXG4gICAgICB9XHJcbiAgICB9XHJcbiAgfVxyXG5cclxuICBwdWJsaWMgaW1nVG9CYXNlNjQgPSBmdW5jdGlvbiAoaW1nU3JjOiBzdHJpbmcpIHtcclxuICAgIGNvbnN0IGNhbnZhczogYW55ID0gZG9jdW1lbnQuY3JlYXRlRWxlbWVudCgnQ0FOVkFTJyksXHJcbiAgICAgIGN0eCA9IGNhbnZhcy5nZXRDb250ZXh0KCcyZCcpLFxyXG4gICAgICBpbWcgPSBuZXcgSW1hZ2U7XHJcbiAgICBpbWcuY3Jvc3NPcmlnaW4gPSAnQW5vbnltb3VzJztcclxuICAgIGltZy5zcmMgPSBpbWdTcmM7XHJcbiAgICBpbWcub25sb2FkID0gZnVuY3Rpb24gKCkge1xyXG4gICAgICBjYW52YXMuaGVpZ2h0ID0gaW1nLmhlaWdodDtcclxuICAgICAgY2FudmFzLndpZHRoID0gaW1nLndpZHRoO1xyXG4gICAgICBjdHguZHJhd0ltYWdlKGltZywgMCwgMCk7XHJcbiAgICAgIGNvbnN0IGRhdGFVUkwgPSBjYW52YXMudG9EYXRhVVJMKCdpbWFnZS9wbmcnKTtcclxuICAgICAgLy8gbWUuZmxvb2RJbWFnZSA9IGJsb2IucmVwbGFjZSgnZGF0YTppbWFnZS9wbmc7YmFzZTY0LCcsICcnKTtcclxuICAgIH07XHJcbiAgfVxyXG5cclxuICBwdWJsaWMgc2V0RnVsbFNjcmVlbiA9IGZ1bmN0aW9uIChleGl0RnVsbFNjcmVlbj86IGJvb2xlYW4pIHtcclxuICAgIGlmIChsb2NhbExheW91dCkge1xyXG4gICAgICBsb2NhbExheW91dC5mdWxsUGFnZSA9ICFleGl0RnVsbFNjcmVlblxyXG4gICAgfVxyXG5cclxuICAgIHdpbmRvdz8ucGFyZW50Py5wb3N0TWVzc2FnZSh7XHJcbiAgICAgIHR5cGU6ICdqYWJpbC1idXMtZnVsbC1zY3JlZW4nLCBkYXRhOiB7IGV4aXRGdWxsU2NyZWVuOiBleGl0RnVsbFNjcmVlbiB9XHJcbiAgICB9LCAnKicpXHJcbiAgfVxyXG5cclxuICBwdWJsaWMgYWRkUm91dGVFdmVudChyb3V0ZXI6IGFueSkge1xyXG4gICAgaWYgKCFyb3V0ZXI/Lm5hdmlnYXRlKSB7XHJcbiAgICAgIGNvbnNvbGUuZXJyb3IoJ2phYmlsLWJ1cy1saWIgcGFyYW0gZXJyb3I6IHJvdXRlci5uYXZpZ2F0ZSBpcyB1bmRlZmluZWQnKVxyXG4gICAgICByZXR1cm5cclxuICAgIH1cclxuICAgIHdpbmRvdy5hZGRFdmVudExpc3RlbmVyKCd1cGRhdGVBcHBVcmwnLCAoZTogYW55KSA9PiB7XHJcbiAgICAgIHJvdXRlci5uYXZpZ2F0ZShbZS5kZXRhaWwucGF0aF0pO1xyXG4gICAgfSk7XHJcbiAgfVxyXG5cclxuICBwdWJsaWMgbG9nb2ZmID0gZnVuY3Rpb24gKCkge1xyXG4gICAgY29uc3QgcmVtb3ZlSXRlbXMgPSBbXHJcbiAgICAgICdsb2dpbkNoZWNrJyxcclxuICAgICAgJ2xvZ2luVHlwZScsXHJcbiAgICAgICdzZWxlY3RlZEluZGV4JyxcclxuICAgICAgJ2p3dCcsXHJcbiAgICAgICd0YWJzJyxcclxuICAgICAgJ3RhYk1lbnUnLFxyXG4gICAgICAncmVnaW9uJyxcclxuICAgICAgJ2N1cnJlbnRSZWdpb24nLFxyXG4gICAgICAndXNlcm5hbWUnLFxyXG4gICAgICAnZGVmYXVsdFVybCcsXHJcbiAgICAgICdyb2xlcycsXHJcbiAgICAgICd0cmFuc2xhdGVBcHAnLFxyXG4gICAgICAnemgnLFxyXG4gICAgICAnZW4nXHJcbiAgICBdO1xyXG4gICAgcmVtb3ZlSXRlbXMuZm9yRWFjaCgoaXRlbTogc3RyaW5nKSA9PiB7XHJcbiAgICAgIGxvY2FsU3RvcmFnZS5yZW1vdmVJdGVtKGl0ZW0pO1xyXG4gICAgfSk7XHJcblxyXG4gICAgY29uc3QgbGFuZ0NhY2hlS2V5ID0gbG9jYWxTdG9yYWdlLmdldEl0ZW0oJ2xhbmdDYWNoZUtleScpO1xyXG4gICAgbG9jYWxTdG9yYWdlLnJlbW92ZUl0ZW0oJycgKyBsYW5nQ2FjaGVLZXkpO1xyXG4gICAgd2luZG93LnBhcmVudC5sb2NhdGlvbi5yZWxvYWQoKVxyXG4gIH1cclxuXHJcbiAgcHVibGljIGdldFRva2VuID0gZnVuY3Rpb24gKCkge1xyXG4gICAgcmV0dXJuIGxvY2FsU3RvcmFnZS5nZXRJdGVtKCdqd3QnKVxyXG4gIH1cclxuXHJcbiAgc2F2ZUxheW91dChsYXlvdXQ6IGFueSkge1xyXG4gICAgbG9jYWxMYXlvdXQgPSBsYXlvdXRcclxuICB9XHJcblxyXG4gIGdldEd1aWQoKTogc3RyaW5nIHtcclxuICAgIHJldHVybiAoJycgKyBbMWU3XSArIC0xZTMgKyAtNGUzICsgLThlMyArIC0xZTExKS5yZXBsYWNlKC9bMDE4XS9nLCBjaCA9PiB7XHJcbiAgICAgIGxldCBjID0gTnVtYmVyKGNoKTtcclxuICAgICAgcmV0dXJuIChjIF4gY3J5cHRvLmdldFJhbmRvbVZhbHVlcyhuZXcgVWludDhBcnJheSgxKSlbMF0gJiAxNSA+PiBjIC8gNCkudG9TdHJpbmcoMTYpXHJcbiAgICB9XHJcbiAgICApXHJcbiAgfVxyXG5cclxuICBpbml0V2ViTm90aWNlKHNpZ25hbFI6IGFueSkge1xyXG4gICAgY29uc29sZS5sb2coc2lnbmFsUilcclxuXHJcbiAgICBjb25zdCBpc0h0dHBzID0gZG9jdW1lbnQubG9jYXRpb24ucHJvdG9jb2wgPT09ICdodHRwczonO1xyXG4gICAgY29uc3QgdGVzdFdTID0gJ2h0dHA6Ly9jbmh1YW0waXRzdGc4Mzo0NDAyMy9tb2RlbHN0YXR1cy1zdGF0aXN0aWNzLW1lc3NhZ2luZy1odWInO1xyXG4gICAgY29uc3QgcHJvZFdTID0gJ2h0dHBzOi8vamFiaWxidXMuamJsYXBwcy5jb20vd3Mvb2Njb21haW4vbW9kZWxzdGF0dXMtc3RhdGlzdGljcy1tZXNzYWdpbmctaHViJztcclxuICAgIGNvbnN0IHdzQmF5VXJsID0gaXNIdHRwcyA/IHByb2RXUyA6IHRlc3RXUztcclxuXHJcbiAgICBjb25zdCBjb25uZWN0aW9uID0gbmV3IHNpZ25hbFIuSHViQ29ubmVjdGlvbkJ1aWxkZXIoKVxyXG4gICAgICAud2l0aFVybCh3c0JheVVybClcclxuICAgICAgLmNvbmZpZ3VyZUxvZ2dpbmcoc2lnbmFsUi5Mb2dMZXZlbC5JbmZvcm1hdGlvbilcclxuICAgICAgLmJ1aWxkKCk7XHJcblxyXG4gICAgY29ubmVjdGlvbi5vbignUmVjZWl2ZU1lc3NhZ2VBc3luYycsIChkYXRhOiBhbnkpID0+IHtcclxuICAgICAgdGhpcy5yZWNpdmVXZWJOb3RpY2UoZGF0YSlcclxuICAgICAgaWYgKGRhdGE/Lm1lc3NhZ2VCb2R5KSB7XHJcbiAgICAgICAgY29uc29sZS5sb2coZGF0YT8ubWVzc2FnZUJvZHkpO1xyXG4gICAgICB9XHJcbiAgICB9KTtcclxuXHJcbiAgICBjb25zdCBzdGFydCA9IGFzeW5jICgpID0+IHtcclxuICAgICAgdHJ5IHtcclxuICAgICAgICBhd2FpdCBjb25uZWN0aW9uLnN0YXJ0KCk7XHJcbiAgICAgICAgY29uc29sZS5sb2coJ1NpZ25hbFIgQ29ubmVjdGVkLicpO1xyXG4gICAgICB9IGNhdGNoIChlcnIpIHtcclxuICAgICAgICBjb25zb2xlLmxvZyhlcnIpO1xyXG4gICAgICAgIHNldFRpbWVvdXQoc3RhcnQsIDUwMDApO1xyXG4gICAgICB9XHJcbiAgICB9O1xyXG5cclxuICAgIGNvbm5lY3Rpb24ub25jbG9zZShhc3luYyAoKSA9PiB7XHJcbiAgICAgIGF3YWl0IHN0YXJ0KCk7XHJcbiAgICB9KTtcclxuXHJcbiAgICAvLyBTdGFydCB0aGUgY29ubmVjdGlvbi5cclxuICAgIHN0YXJ0KCk7XHJcbiAgfVxyXG5cclxuICByZWNpdmVXZWJOb3RpY2UoZGF0YTogYW55KTogYW55IHtcclxuICAgIGxldCBvcHRpb25zOiBhbnkgPSBudWxsXHJcbiAgICB2YXIgUEVSTUlTU09OX0dSQU5URUQgPSBcImdyYW50ZWRcIjtcclxuICAgIHZhciBQRVJNSVNTT05fREVOSUVEID0gXCJkZW5pZWRcIjtcclxuICAgIHZhciBQRVJNSVNTT05fREVGQVVMVCA9IFwiZGVmYXVsdFwiO1xyXG5cclxuICAgIGNvbnNvbGUubG9nKGRvY3VtZW50LmxvY2F0aW9uKVxyXG4gICAgLy8gQHRzLWlnbm9yZVxyXG4gICAgd2luZG93Lm5vdGljZXMuZm9yRWFjaCgoaXRlbTogYW55KSA9PiB7XHJcbiAgICAgIC8vIOWIpOaWrWtleeaYr+WQpuebuOWQjFxyXG4gICAgICBpZiAoaXRlbSkge1xyXG4gICAgICAgIG9wdGlvbnMgPSBpdGVtXHJcbiAgICAgIH1cclxuICAgIH0pO1xyXG5cclxuICAgIGlmICghb3B0aW9ucykge1xyXG4gICAgICByZXR1cm5cclxuICAgIH1cclxuXHJcbiAgICAvLyDlpoLmnpznlKjmiLflt7Lnu4/lhYHorrjvvIznm7TmjqXmmL7npLrmtojmga/vvIzlpoLmnpzkuI3lhYHorrjliJnmj5DnpLrnlKjmiLfmjojmnYNcclxuICAgIGlmIChOb3RpZmljYXRpb24ucGVybWlzc2lvbiA9PT0gUEVSTUlTU09OX0dSQU5URUQpIHtcclxuICAgICAgdGhpcy5ub3RpZnkob3B0aW9ucyk7XHJcbiAgICB9IGVsc2Uge1xyXG4gICAgICBOb3RpZmljYXRpb24ucmVxdWVzdFBlcm1pc3Npb24oKHJlcykgPT4ge1xyXG4gICAgICAgIGlmIChyZXMgPT09IFBFUk1JU1NPTl9HUkFOVEVEKSB7XHJcbiAgICAgICAgICB0aGlzLm5vdGlmeShvcHRpb25zKTtcclxuICAgICAgICB9XHJcbiAgICAgIH0pO1xyXG4gICAgfVxyXG4gIH1cclxuXHJcbiAgcHJpdmF0ZSBub3RpZnkoJG9wdGlvbnM6IGFueSwgY2FsbGJhY2s/OiBGdW5jdGlvbikge1xyXG4gICAgdmFyIG5vdGlmaWNhdGlvbiA9IG5ldyBOb3RpZmljYXRpb24oJG9wdGlvbnM/LnRpdGxlLCAkb3B0aW9ucyk7XHJcblxyXG4gICAgbm90aWZpY2F0aW9uLm9uc2hvdyA9IChldmVudCkgPT4ge1xyXG4gICAgICBjb25zb2xlLmxvZyhcInNob3cgOiBcIiwgZXZlbnQpO1xyXG4gICAgfTtcclxuXHJcbiAgICBub3RpZmljYXRpb24ub25jbG9zZSA9IChldmVudCkgPT4ge1xyXG4gICAgICBjb25zb2xlLmxvZyhcImNsb3NlIDogXCIsIGV2ZW50KTtcclxuICAgIH07XHJcblxyXG4gICAgbm90aWZpY2F0aW9uLm9uY2xpY2sgPSAoZXZlbnQ6IGFueSkgPT4ge1xyXG4gICAgICBjb25zb2xlLmxvZyhcImNsaWNrIDogXCIsIGV2ZW50KTtcclxuICAgICAgLy8g5b2T54K55Ye75LqL5Lu26Kem5Y+R77yM5omT5byA5oyH5a6a55qEdXJsXHJcbiAgICAgIGV2ZW50Py50YXJnZXQ/LmRhdGEgJiYgd2luZG93Lm9wZW4oZXZlbnQ/LnRhcmdldD8uZGF0YSlcclxuICAgICAgbm90aWZpY2F0aW9uLmNsb3NlKCk7XHJcbiAgICB9O1xyXG4gIH1cclxuXHJcbiAgcmVnaXN0ZXJOb3RpY2Uob3B0aW9uczogYW55KSB7XHJcbiAgICAvLyBAdHMtaWdub3JlXHJcbiAgICB3aW5kb3cubm90aWNlcyA9IFtdXHJcbiAgICAvLyBAdHMtaWdub3JlXHJcbiAgICB3aW5kb3cubm90aWNlcy5wdXNoKG9wdGlvbnMpXHJcbiAgfVxyXG59XHJcbiJdfQ==