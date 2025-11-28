import { Component, Input, Output, EventEmitter } from '@angular/core';
import * as i0 from "@angular/core";
import * as i1 from "@angular/platform-browser";
import * as i2 from "primeng/dialog";
import * as i3 from "primeng/api";
function VideoDialogComponent_ng_template_2_Template(rf, ctx) { if (rf & 1) {
    i0.ɵɵelement(0, "h3", 9);
} }
const _c0 = function () { return { width: "100%", height: "100%" }; };
export class VideoDialogComponent {
    constructor(sanitizer) {
        this.sanitizer = sanitizer;
        this.closeDialog = new EventEmitter();
        this.dialogDisplay = true;
        this.currentLanguage = 'en';
        this.srcUrl = '';
    }
    async ngOnInit() {
        this.srcUrl = this.sanitizer.bypassSecurityTrustResourceUrl(this.field);
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
VideoDialogComponent.ɵfac = function VideoDialogComponent_Factory(t) { return new (t || VideoDialogComponent)(i0.ɵɵdirectiveInject(i1.DomSanitizer)); };
VideoDialogComponent.ɵcmp = /*@__PURE__*/ i0.ɵɵdefineComponent({ type: VideoDialogComponent, selectors: [["jabil-video-dialog"]], inputs: { field: "field" }, outputs: { closeDialog: "closeDialog" }, features: [i0.ɵɵProvidersFeature([])], decls: 9, vars: 11, consts: [["styleClass", "p-fluid", 3, "visible", "modal", "draggable", "resizable", "visibleChange", "onHide"], ["dialog", ""], ["pTemplate", "header"], ["width", "100%", "height", "100%", "controls", ""], ["type", "video/mp4", 3, "src"], ["type", "video/ogg", 3, "src"], ["type", "video/webm", 3, "src"], ["data", i0.ɵɵtrustConstantResourceUrl `movie.mp4`, "width", "104", "height", "104"], ["width", "100%", "height", "100%", 3, "src"], [1, "text-center", "w-100"]], template: function VideoDialogComponent_Template(rf, ctx) { if (rf & 1) {
        i0.ɵɵelementStart(0, "p-dialog", 0, 1);
        i0.ɵɵlistener("visibleChange", function VideoDialogComponent_Template_p_dialog_visibleChange_0_listener($event) { return ctx.dialogDisplay = $event; })("onHide", function VideoDialogComponent_Template_p_dialog_onHide_0_listener() { return ctx.closeEditDialog(); });
        i0.ɵɵtemplate(2, VideoDialogComponent_ng_template_2_Template, 1, 0, "ng-template", 2);
        i0.ɵɵelementStart(3, "video", 3);
        i0.ɵɵelement(4, "source", 4)(5, "source", 5)(6, "source", 6);
        i0.ɵɵelementStart(7, "object", 7);
        i0.ɵɵelement(8, "embed", 8);
        i0.ɵɵelementEnd()()();
    } if (rf & 2) {
        i0.ɵɵstyleMap(i0.ɵɵpureFunction0(10, _c0));
        i0.ɵɵproperty("visible", ctx.dialogDisplay)("modal", true)("draggable", false)("resizable", false);
        i0.ɵɵadvance(4);
        i0.ɵɵproperty("src", ctx.getSrcUrl(), i0.ɵɵsanitizeUrl);
        i0.ɵɵadvance(1);
        i0.ɵɵproperty("src", ctx.getSrcUrl(), i0.ɵɵsanitizeUrl);
        i0.ɵɵadvance(1);
        i0.ɵɵproperty("src", ctx.getSrcUrl(), i0.ɵɵsanitizeUrl);
        i0.ɵɵadvance(2);
        i0.ɵɵproperty("src", ctx.getSrcUrl(), i0.ɵɵsanitizeResourceUrl);
    } }, dependencies: [i2.Dialog, i3.PrimeTemplate], styles: ["[_nghost-%COMP%]{height:100%;display:block}[_nghost-%COMP%]   .card[_ngcontent-%COMP%]{height:100%;background:#fff;padding:24px;border-radius:12px}[_nghost-%COMP%]   .card[_ngcontent-%COMP%]   label[_ngcontent-%COMP%]{width:100%}[_nghost-%COMP%]     .p-dialog{max-height:100%}[_nghost-%COMP%]     .p-dialog .p-dialog-header, [_nghost-%COMP%]     .p-dialog .p-dialog-content{background:rgba(0,0,0,.9)}[_nghost-%COMP%]     .p-dialog .p-dialog-header-close-icon{color:#f8f9fa}"] });
(function () { (typeof ngDevMode === "undefined" || ngDevMode) && i0.ɵsetClassMetadata(VideoDialogComponent, [{
        type: Component,
        args: [{ selector: 'jabil-video-dialog', providers: [], template: "<p-dialog\r\n  #dialog\r\n  [(visible)]=\"dialogDisplay\"\r\n  [style]=\"{ width: '100%',height: '100%' }\"\r\n  [modal]=\"true\"\r\n  [draggable]=\"false\"\r\n  [resizable]=\"false\"\r\n  (onHide)=\"closeEditDialog()\"\r\n  styleClass=\"p-fluid\">\r\n  <ng-template pTemplate=\"header\">\r\n    <h3 class=\"text-center w-100\"></h3>\r\n  </ng-template>\r\n\r\n  <video width=\"100%\" height=\"100%\" controls>\r\n    <source\r\n      [src]=\"getSrcUrl()\"\r\n      type=\"video/mp4\" />\r\n    <source\r\n      [src]=\"getSrcUrl()\"\r\n      type=\"video/ogg\" />\r\n    <source\r\n      [src]=\"getSrcUrl()\"\r\n      type=\"video/webm\" />\r\n    <object data=\"movie.mp4\" width=\"104\" height=\"104\">\r\n      <embed\r\n        [src]=\"getSrcUrl()\"\r\n        width=\"100%\"\r\n        height=\"100%\" />\r\n    </object>\r\n  </video>\r\n</p-dialog>\r\n", styles: [":host{height:100%;display:block}:host .card{height:100%;background:#fff;padding:24px;border-radius:12px}:host .card label{width:100%}:host ::ng-deep .p-dialog{max-height:100%}:host ::ng-deep .p-dialog .p-dialog-header,:host ::ng-deep .p-dialog .p-dialog-content{background:rgba(0,0,0,.9)}:host ::ng-deep .p-dialog .p-dialog-header-close-icon{color:#f8f9fa}\n"] }]
    }], function () { return [{ type: i1.DomSanitizer }]; }, { field: [{
            type: Input
        }], closeDialog: [{
            type: Output
        }] }); })();
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoidmlkZW8tZGlhbG9nLmNvbXBvbmVudC5qcyIsInNvdXJjZVJvb3QiOiIiLCJzb3VyY2VzIjpbIi4uLy4uLy4uLy4uLy4uLy4uL3NyYy9hcHAvY29tcG9uZW50cy92aWRlby1kaWFsb2cvdmlkZW8tZGlhbG9nLmNvbXBvbmVudC50cyIsIi4uLy4uLy4uLy4uLy4uLy4uL3NyYy9hcHAvY29tcG9uZW50cy92aWRlby1kaWFsb2cvdmlkZW8tZGlhbG9nLmNvbXBvbmVudC5odG1sIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiJBQUFBLE9BQU8sRUFBRSxTQUFTLEVBQUUsS0FBSyxFQUFVLE1BQU0sRUFBRSxZQUFZLEVBQUUsTUFBTSxlQUFlLENBQUM7Ozs7OztJQ1UzRSx3QkFBbUM7OztBRER2QyxNQUFNLE9BQU8sb0JBQW9CO0lBTy9CLFlBQW9CLFNBQXVCO1FBQXZCLGNBQVMsR0FBVCxTQUFTLENBQWM7UUFMakMsZ0JBQVcsR0FBRyxJQUFJLFlBQVksRUFBVSxDQUFDO1FBRW5ELGtCQUFhLEdBQVksSUFBSSxDQUFDO1FBQzlCLG9CQUFlLEdBQVEsSUFBSSxDQUFDO1FBQzVCLFdBQU0sR0FBUSxFQUFFLENBQUM7SUFDNkIsQ0FBQztJQUV4QyxLQUFLLENBQUMsUUFBUTtRQUNuQixJQUFJLENBQUMsTUFBTSxHQUFHLElBQUksQ0FBQyxTQUFTLENBQUMsOEJBQThCLENBQUMsSUFBSSxDQUFDLEtBQUssQ0FBQyxDQUFDO0lBQzFFLENBQUM7SUFFRCxTQUFTO1FBQ1AsT0FBTyxJQUFJLENBQUMsU0FBUyxDQUFDLDhCQUE4QixDQUFDLElBQUksQ0FBQyxLQUFLLENBQUMsQ0FBQztJQUNuRSxDQUFDO0lBRUQsZUFBZTtRQUNiLElBQUksQ0FBQyxhQUFhLEdBQUcsS0FBSyxDQUFDO1FBQzNCLElBQUksQ0FBQyxXQUFXLENBQUMsSUFBSSxFQUFFLENBQUM7UUFDeEIsSUFBSSxDQUFDLGFBQWEsR0FBRyxJQUFJLENBQUM7SUFDNUIsQ0FBQzs7d0ZBckJVLG9CQUFvQjt1RUFBcEIsb0JBQW9CLDZJQUZwQixFQUFFLG1WQ2dCRyw4QkFBQSxXQUFTO1FBdkIzQixzQ0FRdUI7UUFOckIsdUpBQTJCLHdGQUtqQixxQkFBaUIsSUFMQTtRQU8zQixxRkFFYztRQUVkLGdDQUEyQztRQUN6Qyw0QkFFcUIsZ0JBQUEsZ0JBQUE7UUFPckIsaUNBQWtEO1FBQ2hELDJCQUdrQjtRQUNwQixpQkFBUyxFQUFBLEVBQUE7O1FBekJYLDBDQUEwQztRQUQxQywyQ0FBMkIsZUFBQSxvQkFBQSxvQkFBQTtRQWF2QixlQUFtQjtRQUFuQix1REFBbUI7UUFHbkIsZUFBbUI7UUFBbkIsdURBQW1CO1FBR25CLGVBQW1CO1FBQW5CLHVEQUFtQjtRQUlqQixlQUFtQjtRQUFuQiwrREFBbUI7O3VGRGhCZCxvQkFBb0I7Y0FOaEMsU0FBUzsyQkFDRSxvQkFBb0IsYUFHbkIsRUFBRTsrREFHSixLQUFLO2tCQUFiLEtBQUs7WUFDSSxXQUFXO2tCQUFwQixNQUFNIiwic291cmNlc0NvbnRlbnQiOlsiaW1wb3J0IHsgQ29tcG9uZW50LCBJbnB1dCwgT25Jbml0LCBPdXRwdXQsIEV2ZW50RW1pdHRlciB9IGZyb20gJ0Bhbmd1bGFyL2NvcmUnO1xyXG5pbXBvcnQgeyBEb21TYW5pdGl6ZXIgfSBmcm9tICdAYW5ndWxhci9wbGF0Zm9ybS1icm93c2VyJztcclxuXHJcbkBDb21wb25lbnQoe1xyXG4gIHNlbGVjdG9yOiAnamFiaWwtdmlkZW8tZGlhbG9nJyxcclxuICB0ZW1wbGF0ZVVybDogJy4vdmlkZW8tZGlhbG9nLmNvbXBvbmVudC5odG1sJyxcclxuICBzdHlsZVVybHM6IFsnLi92aWRlby1kaWFsb2cuY29tcG9uZW50LnNjc3MnXSxcclxuICBwcm92aWRlcnM6IFtdLFxyXG59KVxyXG5leHBvcnQgY2xhc3MgVmlkZW9EaWFsb2dDb21wb25lbnQgaW1wbGVtZW50cyBPbkluaXQge1xyXG4gIEBJbnB1dCgpIGZpZWxkOiBhbnk7XHJcbiAgQE91dHB1dCgpIGNsb3NlRGlhbG9nID0gbmV3IEV2ZW50RW1pdHRlcjxzdHJpbmc+KCk7XHJcblxyXG4gIGRpYWxvZ0Rpc3BsYXk6IGJvb2xlYW4gPSB0cnVlO1xyXG4gIGN1cnJlbnRMYW5ndWFnZTogYW55ID0gJ2VuJztcclxuICBzcmNVcmw6IGFueSA9ICcnO1xyXG4gIGNvbnN0cnVjdG9yKHByaXZhdGUgc2FuaXRpemVyOiBEb21TYW5pdGl6ZXIpIHt9XHJcblxyXG4gIHB1YmxpYyBhc3luYyBuZ09uSW5pdCgpIHtcclxuICAgIHRoaXMuc3JjVXJsID0gdGhpcy5zYW5pdGl6ZXIuYnlwYXNzU2VjdXJpdHlUcnVzdFJlc291cmNlVXJsKHRoaXMuZmllbGQpO1xyXG4gIH1cclxuXHJcbiAgZ2V0U3JjVXJsKCkge1xyXG4gICAgcmV0dXJuIHRoaXMuc2FuaXRpemVyLmJ5cGFzc1NlY3VyaXR5VHJ1c3RSZXNvdXJjZVVybCh0aGlzLmZpZWxkKTtcclxuICB9XHJcblxyXG4gIGNsb3NlRWRpdERpYWxvZygpIHtcclxuICAgIHRoaXMuZGlhbG9nRGlzcGxheSA9IGZhbHNlO1xyXG4gICAgdGhpcy5jbG9zZURpYWxvZy5lbWl0KCk7XHJcbiAgICB0aGlzLmRpYWxvZ0Rpc3BsYXkgPSB0cnVlO1xyXG4gIH1cclxufVxyXG4iLCI8cC1kaWFsb2dcclxuICAjZGlhbG9nXHJcbiAgWyh2aXNpYmxlKV09XCJkaWFsb2dEaXNwbGF5XCJcclxuICBbc3R5bGVdPVwieyB3aWR0aDogJzEwMCUnLGhlaWdodDogJzEwMCUnIH1cIlxyXG4gIFttb2RhbF09XCJ0cnVlXCJcclxuICBbZHJhZ2dhYmxlXT1cImZhbHNlXCJcclxuICBbcmVzaXphYmxlXT1cImZhbHNlXCJcclxuICAob25IaWRlKT1cImNsb3NlRWRpdERpYWxvZygpXCJcclxuICBzdHlsZUNsYXNzPVwicC1mbHVpZFwiPlxyXG4gIDxuZy10ZW1wbGF0ZSBwVGVtcGxhdGU9XCJoZWFkZXJcIj5cclxuICAgIDxoMyBjbGFzcz1cInRleHQtY2VudGVyIHctMTAwXCI+PC9oMz5cclxuICA8L25nLXRlbXBsYXRlPlxyXG5cclxuICA8dmlkZW8gd2lkdGg9XCIxMDAlXCIgaGVpZ2h0PVwiMTAwJVwiIGNvbnRyb2xzPlxyXG4gICAgPHNvdXJjZVxyXG4gICAgICBbc3JjXT1cImdldFNyY1VybCgpXCJcclxuICAgICAgdHlwZT1cInZpZGVvL21wNFwiIC8+XHJcbiAgICA8c291cmNlXHJcbiAgICAgIFtzcmNdPVwiZ2V0U3JjVXJsKClcIlxyXG4gICAgICB0eXBlPVwidmlkZW8vb2dnXCIgLz5cclxuICAgIDxzb3VyY2VcclxuICAgICAgW3NyY109XCJnZXRTcmNVcmwoKVwiXHJcbiAgICAgIHR5cGU9XCJ2aWRlby93ZWJtXCIgLz5cclxuICAgIDxvYmplY3QgZGF0YT1cIm1vdmllLm1wNFwiIHdpZHRoPVwiMTA0XCIgaGVpZ2h0PVwiMTA0XCI+XHJcbiAgICAgIDxlbWJlZFxyXG4gICAgICAgIFtzcmNdPVwiZ2V0U3JjVXJsKClcIlxyXG4gICAgICAgIHdpZHRoPVwiMTAwJVwiXHJcbiAgICAgICAgaGVpZ2h0PVwiMTAwJVwiIC8+XHJcbiAgICA8L29iamVjdD5cclxuICA8L3ZpZGVvPlxyXG48L3AtZGlhbG9nPlxyXG4iXX0=