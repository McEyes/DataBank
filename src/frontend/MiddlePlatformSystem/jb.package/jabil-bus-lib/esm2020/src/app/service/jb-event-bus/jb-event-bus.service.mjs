import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import * as i0 from "@angular/core";
export class JBEventBusService {
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
(function () { (typeof ngDevMode === "undefined" || ngDevMode) && i0.ɵsetClassMetadata(JBEventBusService, [{
        type: Injectable
    }], function () { return []; }, null); })();
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiamItZXZlbnQtYnVzLnNlcnZpY2UuanMiLCJzb3VyY2VSb290IjoiIiwic291cmNlcyI6WyIuLi8uLi8uLi8uLi8uLi8uLi9zcmMvYXBwL3NlcnZpY2UvamItZXZlbnQtYnVzL2piLWV2ZW50LWJ1cy5zZXJ2aWNlLnRzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiJBQUFBLE9BQU8sRUFBRSxVQUFVLEVBQUUsTUFBTSxlQUFlLENBQUM7QUFDM0MsT0FBTyxFQUFFLE9BQU8sRUFBYyxNQUFNLE1BQU0sQ0FBQzs7QUFHM0MsTUFBTSxPQUFPLGlCQUFpQjtJQUM1QjtRQUNFLGFBQWE7UUFDYixJQUFHLENBQUMsTUFBTSxDQUFDLE9BQU8sRUFBRTtZQUNsQixhQUFhO1lBQ2IsTUFBTSxDQUFDLE9BQU8sR0FBRyxJQUFJLE9BQU8sRUFBRSxDQUFDO1NBQ2hDO0lBQ0gsQ0FBQztJQUVNLElBQUksQ0FBQyxPQUFZO1FBQ3RCLGFBQWE7UUFDYixNQUFNLENBQUMsT0FBTyxDQUFDLElBQUksQ0FBQyxPQUFPLENBQUMsQ0FBQztJQUMvQixDQUFDO0lBRU0sR0FBRztRQUNSLGFBQWE7UUFDYixPQUFPLE1BQU0sQ0FBQyxPQUFPLENBQUMsWUFBWSxFQUFFLENBQUM7SUFDdkMsQ0FBQzs7a0ZBakJVLGlCQUFpQjt1RUFBakIsaUJBQWlCLFdBQWpCLGlCQUFpQjt1RkFBakIsaUJBQWlCO2NBRDdCLFVBQVUiLCJzb3VyY2VzQ29udGVudCI6WyJpbXBvcnQgeyBJbmplY3RhYmxlIH0gZnJvbSAnQGFuZ3VsYXIvY29yZSc7XHJcbmltcG9ydCB7IFN1YmplY3QsIE9ic2VydmFibGUgfSBmcm9tICdyeGpzJztcclxuXHJcbkBJbmplY3RhYmxlKClcclxuZXhwb3J0IGNsYXNzIEpCRXZlbnRCdXNTZXJ2aWNlIHtcclxuICBjb25zdHJ1Y3RvcigpIHtcclxuICAgIC8vIEB0cy1pZ25vcmVcclxuICAgIGlmKCF3aW5kb3cuc3ViamVjdCkge1xyXG4gICAgICAvLyBAdHMtaWdub3JlXHJcbiAgICAgIHdpbmRvdy5zdWJqZWN0ID0gbmV3IFN1YmplY3QoKTtcclxuICAgIH1cclxuICB9XHJcblxyXG4gIHB1YmxpYyBzZW5kKG1lc3NhZ2U6IGFueSkge1xyXG4gICAgLy8gQHRzLWlnbm9yZVxyXG4gICAgd2luZG93LnN1YmplY3QubmV4dChtZXNzYWdlKTtcclxuICB9XHJcblxyXG4gIHB1YmxpYyBnZXQoKTogT2JzZXJ2YWJsZTxhbnk+IHtcclxuICAgIC8vIEB0cy1pZ25vcmVcclxuICAgIHJldHVybiB3aW5kb3cuc3ViamVjdC5hc09ic2VydmFibGUoKTtcclxuICB9XHJcbn1cclxuXHJcblxyXG4iXX0=