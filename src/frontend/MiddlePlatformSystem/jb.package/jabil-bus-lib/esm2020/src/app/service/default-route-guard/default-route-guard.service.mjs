import { Injectable, Inject } from '@angular/core';
import { DOCUMENT } from '@angular/common';
import * as i0 from "@angular/core";
import * as i1 from "@angular/router";
export class DefaultRouteGuardService {
    constructor(router, document) {
        this.router = router;
        this.document = document;
    }
    async canActivate(route, state) {
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
    }
}
DefaultRouteGuardService.ɵfac = function DefaultRouteGuardService_Factory(t) { return new (t || DefaultRouteGuardService)(i0.ɵɵinject(i1.Router), i0.ɵɵinject(DOCUMENT)); };
DefaultRouteGuardService.ɵprov = /*@__PURE__*/ i0.ɵɵdefineInjectable({ token: DefaultRouteGuardService, factory: DefaultRouteGuardService.ɵfac });
(function () { (typeof ngDevMode === "undefined" || ngDevMode) && i0.ɵsetClassMetadata(DefaultRouteGuardService, [{
        type: Injectable
    }], function () { return [{ type: i1.Router }, { type: undefined, decorators: [{
                type: Inject,
                args: [DOCUMENT]
            }] }]; }, null); })();
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiZGVmYXVsdC1yb3V0ZS1ndWFyZC5zZXJ2aWNlLmpzIiwic291cmNlUm9vdCI6IiIsInNvdXJjZXMiOlsiLi4vLi4vLi4vLi4vLi4vLi4vc3JjL2FwcC9zZXJ2aWNlL2RlZmF1bHQtcm91dGUtZ3VhcmQvZGVmYXVsdC1yb3V0ZS1ndWFyZC5zZXJ2aWNlLnRzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiJBQUFBLE9BQU8sRUFBRSxVQUFVLEVBQUUsTUFBTSxFQUFFLE1BQU0sZUFBZSxDQUFDO0FBQ25ELE9BQU8sRUFBRSxRQUFRLEVBQUUsTUFBTSxpQkFBaUIsQ0FBQzs7O0FBVTNDLE1BQU0sT0FBTyx3QkFBd0I7SUFDbkMsWUFDVSxNQUFjLEVBQ0ksUUFBYTtRQUQvQixXQUFNLEdBQU4sTUFBTSxDQUFRO1FBQ0ksYUFBUSxHQUFSLFFBQVEsQ0FBSztJQUN0QyxDQUFDO0lBRUosS0FBSyxDQUFDLFdBQVcsQ0FBQyxLQUE2QixFQUFFLEtBQTBCO1FBQ3pFLE1BQU0sVUFBVSxHQUFHLFlBQVksQ0FBQyxPQUFPLENBQUMsWUFBWSxDQUFDLENBQUM7UUFDdEQsTUFBTSxVQUFVLEdBQUcsWUFBWSxDQUFDLE9BQU8sQ0FBQyxZQUFZLENBQUMsQ0FBQztRQUN0RCxJQUFJLFVBQVUsS0FBSyxjQUFjLElBQUksVUFBVSxLQUFLLGtCQUFrQixFQUFFO1lBQ3RFLE9BQU8sSUFBSSxDQUFDO1NBQ2I7UUFDRCxJQUFJLFVBQVUsSUFBSSxVQUFVLEtBQUssR0FBRyxFQUFFO1lBQ3BDLDhGQUE4RjtZQUM5RixpQkFBaUI7WUFDakIsSUFBSTtZQUNKLElBQUcsS0FBSyxDQUFDLEdBQUcsQ0FBQyxRQUFRLENBQUMsbUJBQW1CLENBQUMsRUFBQztnQkFDekMsT0FBTyxJQUFJLENBQUM7YUFDYjtZQUNELElBQUksQ0FBQyxNQUFNLENBQUMsUUFBUSxDQUFDLENBQUMsVUFBVSxDQUFDLENBQUMsQ0FBQztTQUNwQztRQUNELE9BQU8sSUFBSSxDQUFDO0lBQ2QsQ0FBQzs7Z0dBdEJVLHdCQUF3QixzQ0FHekIsUUFBUTs4RUFIUCx3QkFBd0IsV0FBeEIsd0JBQXdCO3VGQUF4Qix3QkFBd0I7Y0FEcEMsVUFBVTs7c0JBSU4sTUFBTTt1QkFBQyxRQUFRIiwic291cmNlc0NvbnRlbnQiOlsiaW1wb3J0IHsgSW5qZWN0YWJsZSwgSW5qZWN0IH0gZnJvbSAnQGFuZ3VsYXIvY29yZSc7XHJcbmltcG9ydCB7IERPQ1VNRU5UIH0gZnJvbSAnQGFuZ3VsYXIvY29tbW9uJztcclxuaW1wb3J0IHtcclxuICBDYW5BY3RpdmF0ZSxcclxuICBBY3RpdmF0ZWRSb3V0ZVNuYXBzaG90LFxyXG4gIFJvdXRlclN0YXRlU25hcHNob3QsXHJcbiAgUm91dGVyLFxyXG4gIE5hdmlnYXRpb25TdGFydCxcclxufSBmcm9tICdAYW5ndWxhci9yb3V0ZXInO1xyXG5cclxuQEluamVjdGFibGUoKVxyXG5leHBvcnQgY2xhc3MgRGVmYXVsdFJvdXRlR3VhcmRTZXJ2aWNlIGltcGxlbWVudHMgQ2FuQWN0aXZhdGUge1xyXG4gIGNvbnN0cnVjdG9yKFxyXG4gICAgcHJpdmF0ZSByb3V0ZXI6IFJvdXRlcixcclxuICAgIEBJbmplY3QoRE9DVU1FTlQpIHByaXZhdGUgZG9jdW1lbnQ6IGFueSxcclxuICApIHt9XHJcblxyXG4gIGFzeW5jIGNhbkFjdGl2YXRlKHJvdXRlOiBBY3RpdmF0ZWRSb3V0ZVNuYXBzaG90LCBzdGF0ZTogUm91dGVyU3RhdGVTbmFwc2hvdCk6IFByb21pc2U8Ym9vbGVhbj4ge1xyXG4gICAgY29uc3QgZGVmYXVsdFVybCA9IGxvY2FsU3RvcmFnZS5nZXRJdGVtKCdkZWZhdWx0VXJsJyk7XHJcbiAgICBjb25zdCBsb2dpbkNoZWNrID0gbG9jYWxTdG9yYWdlLmdldEl0ZW0oJ2xvZ2luQ2hlY2snKTtcclxuICAgIGlmIChkZWZhdWx0VXJsID09PSAnL2NvbW1vbi9ob21lJyB8fCBkZWZhdWx0VXJsID09PSAnL2NvbW1vbi9yZWRpcmVjdCcpIHtcclxuICAgICAgcmV0dXJuIHRydWU7XHJcbiAgICB9XHJcbiAgICBpZiAoZGVmYXVsdFVybCAmJiBsb2dpbkNoZWNrID09PSAnMScpIHtcclxuICAgICAgLy8gaWYgKGRlZmF1bHRVcmw/LmluY2x1ZGVzKCcvY29tbW9uL2phYmlsLWJ1cycpICYmIHN0YXRlLnVybC5pbmNsdWRlcygnL2NvbW1vbi9qYWJpbC1idXMnKSkge1xyXG4gICAgICAvLyAgIHJldHVybiB0cnVlO1xyXG4gICAgICAvLyB9XHJcbiAgICAgIGlmKHN0YXRlLnVybC5pbmNsdWRlcygnL2NvbW1vbi9qYWJpbC1idXMnKSl7XHJcbiAgICAgICAgcmV0dXJuIHRydWU7XHJcbiAgICAgIH1cclxuICAgICAgdGhpcy5yb3V0ZXIubmF2aWdhdGUoW2RlZmF1bHRVcmxdKTtcclxuICAgIH1cclxuICAgIHJldHVybiB0cnVlO1xyXG4gIH1cclxufVxyXG4iXX0=