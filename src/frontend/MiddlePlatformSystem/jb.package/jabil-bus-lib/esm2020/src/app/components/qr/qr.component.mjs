import { Component, Input } from '@angular/core';
import { CommonService } from '../../service/common/common.service';
import * as QRCode from 'qrcode';
import * as i0 from "@angular/core";
const _c0 = function (a0, a1) { return { width: a0, height: a1 }; };
export class QRComponent {
    constructor() {
        this.width = '200px';
        this.height = '200px';
        this.QRUrl = '';
        // console.log(QRCode)
    }
    async ngOnInit() {
        this.getQR();
    }
    getQR() {
        QRCode.toDataURL(this.field?.id)
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
QRComponent.ɵcmp = /*@__PURE__*/ i0.ɵɵdefineComponent({ type: QRComponent, selectors: [["jabil-qr"]], inputs: { field: "field", width: "width", height: "height" }, decls: 3, vars: 6, consts: [[1, "print-content"], [3, "src"]], template: function QRComponent_Template(rf, ctx) { if (rf & 1) {
        i0.ɵɵelementStart(0, "div")(1, "div", 0);
        i0.ɵɵelement(2, "img", 1);
        i0.ɵɵelementEnd()();
    } if (rf & 2) {
        i0.ɵɵadvance(2);
        i0.ɵɵstyleMap(i0.ɵɵpureFunction2(3, _c0, ctx.width, ctx.height));
        i0.ɵɵproperty("src", ctx.QRUrl, i0.ɵɵsanitizeUrl);
    } }, styles: ["[_nghost-%COMP%]{justify-content:center;display:flex}"] });
(function () { (typeof ngDevMode === "undefined" || ngDevMode) && i0.ɵsetClassMetadata(QRComponent, [{
        type: Component,
        args: [{ selector: 'jabil-qr', template: "<div>\r\n  <div class=\"print-content\">\r\n    <img [style]=\"{width: width,height: height}\" [src]=\"QRUrl\" />\r\n  </div>\r\n</div>\r\n", styles: [":host{justify-content:center;display:flex}\n"] }]
    }], function () { return []; }, { field: [{
            type: Input
        }], width: [{
            type: Input
        }], height: [{
            type: Input
        }] }); })();
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoicXIuY29tcG9uZW50LmpzIiwic291cmNlUm9vdCI6IiIsInNvdXJjZXMiOlsiLi4vLi4vLi4vLi4vLi4vLi4vc3JjL2FwcC9jb21wb25lbnRzL3FyL3FyLmNvbXBvbmVudC50cyIsIi4uLy4uLy4uLy4uLy4uLy4uL3NyYy9hcHAvY29tcG9uZW50cy9xci9xci5jb21wb25lbnQuaHRtbCJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFBQSxPQUFPLEVBQUUsU0FBUyxFQUFFLEtBQUssRUFBVSxNQUFNLGVBQWUsQ0FBQztBQUN6RCxPQUFPLEVBQUUsYUFBYSxFQUFFLE1BQU0scUNBQXFDLENBQUM7QUFDcEUsT0FBTyxLQUFLLE1BQU0sTUFBTSxRQUFRLENBQUM7OztBQVFqQyxNQUFNLE9BQU8sV0FBVztJQVF0QjtRQU5TLFVBQUssR0FBVyxPQUFPLENBQUM7UUFDeEIsV0FBTSxHQUFXLE9BQU8sQ0FBQztRQUdsQyxVQUFLLEdBQVcsRUFBRSxDQUFDO1FBR2pCLHNCQUFzQjtJQUN4QixDQUFDO0lBRU0sS0FBSyxDQUFDLFFBQVE7UUFDbkIsSUFBSSxDQUFDLEtBQUssRUFBRSxDQUFDO0lBQ2YsQ0FBQztJQUVELEtBQUs7UUFDSCxNQUFNLENBQUMsU0FBUyxDQUFDLElBQUksQ0FBQyxLQUFLLEVBQUUsRUFBRSxDQUFDO2FBQzdCLElBQUksQ0FBQyxDQUFDLEdBQVEsRUFBRSxFQUFFO1lBQ2pCLElBQUksQ0FBQyxLQUFLLEdBQUcsR0FBRyxDQUFDO1FBQ25CLENBQUMsQ0FBQzthQUNELEtBQUssQ0FBQyxDQUFDLEdBQVEsRUFBRSxFQUFFO1lBQ2xCLE9BQU8sQ0FBQyxLQUFLLENBQUMsR0FBRyxDQUFDLENBQUM7UUFDckIsQ0FBQyxDQUFDLENBQUM7SUFDUCxDQUFDO0lBRUQsT0FBTztRQUNMLE1BQU0sYUFBYSxHQUFHLElBQUksYUFBYSxFQUFFLENBQUE7UUFDekMsYUFBYSxDQUFDLGNBQWMsQ0FBQyxJQUFJLENBQUMsS0FBSyxFQUFFLElBQUksQ0FBQyxLQUFLLENBQUMsQ0FBQztJQUN2RCxDQUFDOztzRUE3QlUsV0FBVzs4REFBWCxXQUFXO1FDVnhCLDJCQUFLLGFBQUE7UUFFRCx5QkFBNkQ7UUFDL0QsaUJBQU0sRUFBQTs7UUFEQyxlQUF1QztRQUF2QyxnRUFBdUM7UUFBQyxpREFBYTs7dUZEUWpELFdBQVc7Y0FOdkIsU0FBUzsyQkFDRSxVQUFVO3NDQU1YLEtBQUs7a0JBQWIsS0FBSztZQUNHLEtBQUs7a0JBQWIsS0FBSztZQUNHLE1BQU07a0JBQWQsS0FBSyIsInNvdXJjZXNDb250ZW50IjpbImltcG9ydCB7IENvbXBvbmVudCwgSW5wdXQsIE9uSW5pdCB9IGZyb20gJ0Bhbmd1bGFyL2NvcmUnO1xyXG5pbXBvcnQgeyBDb21tb25TZXJ2aWNlIH0gZnJvbSAnLi4vLi4vc2VydmljZS9jb21tb24vY29tbW9uLnNlcnZpY2UnO1xyXG5pbXBvcnQgKiBhcyBRUkNvZGUgZnJvbSAncXJjb2RlJztcclxuXHJcbkBDb21wb25lbnQoe1xyXG4gIHNlbGVjdG9yOiAnamFiaWwtcXInLFxyXG4gIHRlbXBsYXRlVXJsOiAnLi9xci5jb21wb25lbnQuaHRtbCcsXHJcbiAgc3R5bGVVcmxzOiBbJy4vcXIuY29tcG9uZW50LnNjc3MnXSxcclxuICAvLyBwcm92aWRlcnM6IFtDb21tb25GdW5jdGlvbl0sXHJcbn0pXHJcbmV4cG9ydCBjbGFzcyBRUkNvbXBvbmVudCBpbXBsZW1lbnRzIE9uSW5pdCB7XHJcbiAgQElucHV0KCkgZmllbGQ6IGFueTtcclxuICBASW5wdXQoKSB3aWR0aDogc3RyaW5nID0gJzIwMHB4JztcclxuICBASW5wdXQoKSBoZWlnaHQ6IHN0cmluZyA9ICcyMDBweCc7XHJcblxyXG4gIGxvYWRpbmchOiBib29sZWFuO1xyXG4gIFFSVXJsOiBzdHJpbmcgPSAnJztcclxuXHJcbiAgY29uc3RydWN0b3IoKSB7XHJcbiAgICAvLyBjb25zb2xlLmxvZyhRUkNvZGUpXHJcbiAgfVxyXG5cclxuICBwdWJsaWMgYXN5bmMgbmdPbkluaXQoKSB7XHJcbiAgICB0aGlzLmdldFFSKCk7XHJcbiAgfVxyXG5cclxuICBnZXRRUigpIHtcclxuICAgIFFSQ29kZS50b0RhdGFVUkwodGhpcy5maWVsZD8uaWQpXHJcbiAgICAgIC50aGVuKCh1cmw6IGFueSkgPT4ge1xyXG4gICAgICAgIHRoaXMuUVJVcmwgPSB1cmw7XHJcbiAgICAgIH0pXHJcbiAgICAgIC5jYXRjaCgoZXJyOiBhbnkpID0+IHtcclxuICAgICAgICBjb25zb2xlLmVycm9yKGVycik7XHJcbiAgICAgIH0pO1xyXG4gIH1cclxuXHJcbiAgcHJpbnRRUigpIHtcclxuICAgIGNvbnN0IGNvbW1vblNlcnZpY2UgPSBuZXcgQ29tbW9uU2VydmljZSgpXHJcbiAgICBjb21tb25TZXJ2aWNlLnByaW50TmV3V2luZG93KHRoaXMuUVJVcmwsIHRoaXMuZmllbGQpO1xyXG4gIH1cclxufVxyXG4iLCI8ZGl2PlxyXG4gIDxkaXYgY2xhc3M9XCJwcmludC1jb250ZW50XCI+XHJcbiAgICA8aW1nIFtzdHlsZV09XCJ7d2lkdGg6IHdpZHRoLGhlaWdodDogaGVpZ2h0fVwiIFtzcmNdPVwiUVJVcmxcIiAvPlxyXG4gIDwvZGl2PlxyXG48L2Rpdj5cclxuIl19