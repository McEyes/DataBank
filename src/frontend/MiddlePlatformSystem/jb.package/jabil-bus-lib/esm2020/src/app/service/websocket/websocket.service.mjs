import { Injectable } from '@angular/core';
import * as i0 from "@angular/core";
let heartTime;
let reConnectTimeOut;
export class WebsocketService {
    constructor() { }
    init(url, params, resFunc, isReConnect) {
        if (!url) {
            return;
        }
        let webSocket;
        let noReConnect = false;
        webSocket = new WebSocket(url);
        webSocket.onmessage = async (res) => {
            const data = JSON.parse(res.data);
            resFunc(data);
            // this.buildData(data.MessageBody, params, resFunc);
        };
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
(function () { (typeof ngDevMode === "undefined" || ngDevMode) && i0.ɵsetClassMetadata(WebsocketService, [{
        type: Injectable
    }], function () { return []; }, null); })();
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoid2Vic29ja2V0LnNlcnZpY2UuanMiLCJzb3VyY2VSb290IjoiIiwic291cmNlcyI6WyIuLi8uLi8uLi8uLi8uLi8uLi9zcmMvYXBwL3NlcnZpY2Uvd2Vic29ja2V0L3dlYnNvY2tldC5zZXJ2aWNlLnRzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiJBQUFBLE9BQU8sRUFBRSxVQUFVLEVBQUUsTUFBTSxlQUFlLENBQUM7O0FBRTNDLElBQUksU0FBZSxDQUFDO0FBQ3BCLElBQUksZ0JBQXNCLENBQUM7QUFHM0IsTUFBTSxPQUFPLGdCQUFnQjtJQUMzQixnQkFBZSxDQUFDO0lBRWhCLElBQUksQ0FBQyxHQUFXLEVBQUUsTUFBVyxFQUFFLE9BQVksRUFBRSxXQUFxQjtRQUNoRSxJQUFJLENBQUMsR0FBRyxFQUFFO1lBQ1IsT0FBTztTQUNSO1FBQ0QsSUFBSSxTQUFlLENBQUM7UUFDcEIsSUFBSSxXQUFXLEdBQVksS0FBSyxDQUFDO1FBQ2pDLFNBQVMsR0FBRyxJQUFJLFNBQVMsQ0FBQyxHQUFHLENBQUMsQ0FBQztRQUMvQixTQUFTLENBQUMsU0FBUyxHQUFHLEtBQUssRUFBRSxHQUFRLEVBQUUsRUFBRTtZQUN2QyxNQUFNLElBQUksR0FBRyxJQUFJLENBQUMsS0FBSyxDQUFDLEdBQUcsQ0FBQyxJQUFJLENBQUMsQ0FBQztZQUNsQyxPQUFPLENBQUMsSUFBSSxDQUFDLENBQUM7WUFDZCxxREFBcUQ7UUFDdkQsQ0FBQyxDQUFDO1FBQ0YsU0FBUyxDQUFDLE1BQU0sR0FBRyxHQUFHLEVBQUU7WUFDdEIsT0FBTyxDQUFDLEdBQUcsQ0FBQyxxQkFBcUIsQ0FBQyxDQUFDO1lBQ25DLFNBQVMsR0FBRyxXQUFXLENBQUMsR0FBRyxFQUFFO2dCQUMzQixZQUFZO2dCQUNaLE9BQU8sQ0FBQyxHQUFHLENBQUMsT0FBTyxDQUFDLENBQUM7Z0JBQ3JCLFNBQVMsQ0FBQyxJQUFJLENBQUMsTUFBTSxDQUFDLENBQUM7WUFDekIsQ0FBQyxFQUFFLEtBQUssQ0FBQyxDQUFDO1lBQ1YsMENBQTBDO1FBQzVDLENBQUMsQ0FBQztRQUNGLFNBQVMsQ0FBQyxPQUFPLEdBQUcsR0FBRyxFQUFFO1lBQ3ZCLElBQUksV0FBVyxJQUFJLENBQUMsU0FBUyxDQUFDLFdBQVcsRUFBRTtnQkFDekMsSUFBSSxDQUFDLGVBQWUsQ0FBQyxHQUFHLEVBQUUsTUFBTSxFQUFFLE9BQU8sRUFBRSxXQUFXLENBQUMsQ0FBQzthQUN6RDtZQUNELFNBQVMsSUFBSSxhQUFhLENBQUMsU0FBUyxDQUFDLENBQUM7WUFDdEMsT0FBTyxDQUFDLEdBQUcsQ0FBQyxrQkFBa0IsQ0FBQyxDQUFDO1FBQ2xDLENBQUMsQ0FBQztRQUNGLFNBQVMsQ0FBQyxPQUFPLEdBQUcsQ0FBQyxHQUFRLEVBQUUsRUFBRTtZQUMvQixJQUFJLFdBQVcsSUFBSSxTQUFTLENBQUMsV0FBVyxFQUFFO2dCQUN4QyxJQUFJLENBQUMsZUFBZSxDQUFDLEdBQUcsRUFBRSxNQUFNLEVBQUUsT0FBTyxFQUFFLFdBQVcsQ0FBQyxDQUFDO2FBQ3pEO1lBQ0QsU0FBUyxJQUFJLGFBQWEsQ0FBQyxTQUFTLENBQUMsQ0FBQztZQUN0QyxPQUFPLENBQUMsS0FBSyxDQUFDLEdBQUcsQ0FBQyxDQUFDO1FBQ3JCLENBQUMsQ0FBQztRQUVGLE9BQU8sU0FBUyxDQUFDO0lBQ25CLENBQUM7SUFFTyxlQUFlLENBQUMsR0FBVyxFQUFFLE1BQVcsRUFBRSxPQUFZLEVBQUUsV0FBcUI7UUFDbkYsb0JBQW9CO1FBQ3BCLGdCQUFnQixJQUFJLFlBQVksQ0FBQyxnQkFBZ0IsQ0FBQyxDQUFDO1FBQ25ELGdCQUFnQixHQUFHLFVBQVUsQ0FBQyxHQUFHLEVBQUU7WUFDakMsSUFBSSxDQUFDLElBQUksQ0FBQyxHQUFHLEVBQUUsTUFBTSxFQUFFLE9BQU8sRUFBRSxXQUFXLENBQUMsQ0FBQztRQUMvQyxDQUFDLEVBQUUsSUFBSSxDQUFDLENBQUM7SUFDWCxDQUFDOztnRkFoRFUsZ0JBQWdCO3NFQUFoQixnQkFBZ0IsV0FBaEIsZ0JBQWdCO3VGQUFoQixnQkFBZ0I7Y0FENUIsVUFBVSIsInNvdXJjZXNDb250ZW50IjpbImltcG9ydCB7IEluamVjdGFibGUgfSBmcm9tICdAYW5ndWxhci9jb3JlJztcclxuXHJcbmxldCBoZWFydFRpbWUhOiBhbnk7XHJcbmxldCByZUNvbm5lY3RUaW1lT3V0ITogYW55O1xyXG5cclxuQEluamVjdGFibGUoKVxyXG5leHBvcnQgY2xhc3MgV2Vic29ja2V0U2VydmljZSB7XHJcbiAgY29uc3RydWN0b3IoKSB7fVxyXG5cclxuICBpbml0KHVybDogc3RyaW5nLCBwYXJhbXM6IGFueSwgcmVzRnVuYzogYW55LCBpc1JlQ29ubmVjdD86IGJvb2xlYW4pIHtcclxuICAgIGlmICghdXJsKSB7XHJcbiAgICAgIHJldHVybjtcclxuICAgIH1cclxuICAgIGxldCB3ZWJTb2NrZXQhOiBhbnk7XHJcbiAgICBsZXQgbm9SZUNvbm5lY3Q6IGJvb2xlYW4gPSBmYWxzZTtcclxuICAgIHdlYlNvY2tldCA9IG5ldyBXZWJTb2NrZXQodXJsKTtcclxuICAgIHdlYlNvY2tldC5vbm1lc3NhZ2UgPSBhc3luYyAocmVzOiBhbnkpID0+IHtcclxuICAgICAgY29uc3QgZGF0YSA9IEpTT04ucGFyc2UocmVzLmRhdGEpO1xyXG4gICAgICByZXNGdW5jKGRhdGEpO1xyXG4gICAgICAvLyB0aGlzLmJ1aWxkRGF0YShkYXRhLk1lc3NhZ2VCb2R5LCBwYXJhbXMsIHJlc0Z1bmMpO1xyXG4gICAgfTtcclxuICAgIHdlYlNvY2tldC5vbm9wZW4gPSAoKSA9PiB7XHJcbiAgICAgIGNvbnNvbGUubG9nKCd3ZWJTb2NrZXQgY29ubmVjdGVkJyk7XHJcbiAgICAgIGhlYXJ0VGltZSA9IHNldEludGVydmFsKCgpID0+IHtcclxuICAgICAgICAvLyBvbm1lc3NhZ2VcclxuICAgICAgICBjb25zb2xlLmxvZygncGluZyEnKTtcclxuICAgICAgICB3ZWJTb2NrZXQuc2VuZCgncGluZycpO1xyXG4gICAgICB9LCAyMDAwMCk7XHJcbiAgICAgIC8vIHdlYlNvY2tldC5zZW5kKEpTT04uc3RyaW5naWZ5KHBhcmFtcykpO1xyXG4gICAgfTtcclxuICAgIHdlYlNvY2tldC5vbmNsb3NlID0gKCkgPT4ge1xyXG4gICAgICBpZiAoaXNSZUNvbm5lY3QgJiYgIXdlYlNvY2tldC5ub1JlQ29ubmVjdCkge1xyXG4gICAgICAgIHRoaXMuc29ja2V0UmVDb25uZWN0KHVybCwgcGFyYW1zLCByZXNGdW5jLCBpc1JlQ29ubmVjdCk7XHJcbiAgICAgIH1cclxuICAgICAgaGVhcnRUaW1lICYmIGNsZWFySW50ZXJ2YWwoaGVhcnRUaW1lKTtcclxuICAgICAgY29uc29sZS5sb2coJ3dlYlNvY2tldCBjbG9zZWQnKTtcclxuICAgIH07XHJcbiAgICB3ZWJTb2NrZXQub25lcnJvciA9IChyZXM6IGFueSkgPT4ge1xyXG4gICAgICBpZiAoaXNSZUNvbm5lY3QgJiYgd2ViU29ja2V0Lm5vUmVDb25uZWN0KSB7XHJcbiAgICAgICAgdGhpcy5zb2NrZXRSZUNvbm5lY3QodXJsLCBwYXJhbXMsIHJlc0Z1bmMsIGlzUmVDb25uZWN0KTtcclxuICAgICAgfVxyXG4gICAgICBoZWFydFRpbWUgJiYgY2xlYXJJbnRlcnZhbChoZWFydFRpbWUpO1xyXG4gICAgICBjb25zb2xlLmVycm9yKHJlcyk7XHJcbiAgICB9O1xyXG5cclxuICAgIHJldHVybiB3ZWJTb2NrZXQ7XHJcbiAgfVxyXG5cclxuICBwcml2YXRlIHNvY2tldFJlQ29ubmVjdCh1cmw6IHN0cmluZywgcGFyYW1zOiBhbnksIHJlc0Z1bmM6IGFueSwgaXNSZUNvbm5lY3Q/OiBib29sZWFuKSB7XHJcbiAgICAvLyByZWNvbm5lY3QgdGltZSA1c1xyXG4gICAgcmVDb25uZWN0VGltZU91dCAmJiBjbGVhclRpbWVvdXQocmVDb25uZWN0VGltZU91dCk7XHJcbiAgICByZUNvbm5lY3RUaW1lT3V0ID0gc2V0VGltZW91dCgoKSA9PiB7XHJcbiAgICAgIHRoaXMuaW5pdCh1cmwsIHBhcmFtcywgcmVzRnVuYywgaXNSZUNvbm5lY3QpO1xyXG4gICAgfSwgNTAwMCk7XHJcbiAgfVxyXG59XHJcbiJdfQ==