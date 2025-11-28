import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import * as i0 from "@angular/core";
export class MessageSender {
    constructor() {
        this.emitChangeSource = new Subject();
    }
}
MessageSender.ɵfac = function MessageSender_Factory(t) { return new (t || MessageSender)(); };
MessageSender.ɵprov = /*@__PURE__*/ i0.ɵɵdefineInjectable({ token: MessageSender, factory: MessageSender.ɵfac, providedIn: 'root' });
(function () { (typeof ngDevMode === "undefined" || ngDevMode) && i0.ɵsetClassMetadata(MessageSender, [{
        type: Injectable,
        args: [{
                providedIn: 'root',
            }]
    }], null, null); })();
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoibWVzc2FnZS1zZW5kZXIuc2VydmljZS5qcyIsInNvdXJjZVJvb3QiOiIiLCJzb3VyY2VzIjpbIi4uLy4uLy4uLy4uLy4uLy4uL3NyYy9hcHAvc2VydmljZS9tZXNzYWdlLXNlbmRlci9tZXNzYWdlLXNlbmRlci5zZXJ2aWNlLnRzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiJBQUFBLE9BQU8sRUFBRSxVQUFVLEVBQUUsTUFBTSxlQUFlLENBQUM7QUFDM0MsT0FBTyxFQUFFLE9BQU8sRUFBRSxNQUFNLE1BQU0sQ0FBQzs7QUFLL0IsTUFBTSxPQUFPLGFBQWE7SUFIMUI7UUFJRSxxQkFBZ0IsR0FBRyxJQUFJLE9BQU8sRUFBTyxDQUFDO0tBVXZDOzswRUFYWSxhQUFhO21FQUFiLGFBQWEsV0FBYixhQUFhLG1CQUZaLE1BQU07dUZBRVAsYUFBYTtjQUh6QixVQUFVO2VBQUM7Z0JBQ1YsVUFBVSxFQUFFLE1BQU07YUFDbkIiLCJzb3VyY2VzQ29udGVudCI6WyJpbXBvcnQgeyBJbmplY3RhYmxlIH0gZnJvbSAnQGFuZ3VsYXIvY29yZSc7XHJcbmltcG9ydCB7IFN1YmplY3QgfSBmcm9tICdyeGpzJztcclxuXHJcbkBJbmplY3RhYmxlKHtcclxuICBwcm92aWRlZEluOiAncm9vdCcsXHJcbn0pXHJcbmV4cG9ydCBjbGFzcyBNZXNzYWdlU2VuZGVyIHtcclxuICBlbWl0Q2hhbmdlU291cmNlID0gbmV3IFN1YmplY3Q8YW55PigpO1xyXG4gIC8vIGNoYW5nZUVtaXR0ZWQkID0gdGhpcy5lbWl0Q2hhbmdlU291cmNlKCk7XHJcbiAgLy8gY29uc3RydWN0b3IoKSB7fVxyXG4gIC8vIGVtaXRDaGFuZ2UoY2hhbmdlOiBhbnkpIHtcclxuICAvLyAgIGFsZXJ0KGNoYW5nZSlcclxuICAvLyAgIHRoaXMuZW1pdENoYW5nZVNvdXJjZS5uZXh0KGNoYW5nZSk7XHJcbiAgLy8gfVxyXG4gIC8vIGdldCgpIHtcclxuICAvLyAgIHJldHVybiB0aGlzLmVtaXRDaGFuZ2VTb3VyY2UoKTtcclxuICAvLyB9XHJcbn1cclxuIl19