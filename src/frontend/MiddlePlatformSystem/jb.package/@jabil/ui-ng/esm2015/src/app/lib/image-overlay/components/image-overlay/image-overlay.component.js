import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
export class ImageOverlayComponent {
    constructor() {
        /**
         * Image Name
         */
        this.imgName = 'image';
        /**
         * Image Animation Name
         */
        this.imgAnimation = null;
        /**
         * Image Overlay Animation Name
         */
        this.imgOverlayAnimation = null;
        /**
         * Image Overlay Background Color
         */
        this.imgOverlayBgColor = 'primary-dark';
    }
}
ImageOverlayComponent.decorators = [
    { type: Component, args: [{
                selector: 'ui-ng-image-overlay',
                template: "<div\n\tclass=\"img-container\"\n\t[ngClass]=\"['img-' + imgAnimation, 'img-overlay-' + imgOverlayAnimation]\"\n>\n\t<img class=\"img-fluid img-item\" [src]=\"imgSrc\" [alt]=\"imgName\" />\n\t<div class=\"img-overlay\" [ngClass]=\"'img-overlay-' + imgOverlayBgColor\">\n\t\t<div class=\"img-overlay-content\">\n\t\t\t<ng-content></ng-content>\n\t\t</div>\n\t</div>\n</div>\n",
                changeDetection: ChangeDetectionStrategy.OnPush,
                styles: [":host{display:block}.img-container{position:relative;z-index:1;display:inline-block;overflow:hidden}.img-container .img-item{transition:transform .25s ease-out;will-change:transform}.img-container .img-overlay{position:absolute;top:-.2rem;right:0;bottom:-.2rem;left:0;z-index:2;display:flex;align-items:center;justify-content:center;visibility:hidden;opacity:0;transition:all .3s ease-in;will-change:opacity,transform}.img-container .img-overlay-content{text-align:center}.img-container .img-overlay-black{background-color:rgba(0,0,0,.75)}.img-container .img-overlay-white{background-color:hsla(0,0%,100%,.9)}.img-container .img-overlay-primary-dark{background-color:rgba(62,74,89,.8)}.img-overlay-slide-top .img-overlay{transform:translateY(100%)}.img-overlay-slide-right .img-overlay{transform:translateX(-100%)}.img-overlay-slide-down .img-overlay{transform:translateY(-100%)}.img-overlay-slide-left .img-overlay{transform:translateX(100%)}.img-overlay-zoom-in .img-overlay{transform:scale(0)}.img-overlay-zoom-out .img-overlay{transform:scale(2)}.img-overlay-slide-top:hover .img-overlay{transform:translateY(0)}.img-overlay-slide-right:hover .img-overlay{transform:translateX(0)}.img-overlay-slide-down:hover .img-overlay{transform:translateY(0)}.img-overlay-slide-left:hover .img-overlay{transform:translateX(0)}.img-overlay-zoom-in:hover .img-overlay,.img-overlay-zoom-out:hover .img-overlay{transform:scale(1)}.img-zoom-in:hover .img-item{transform:scale(1.2)}.img-rotate-r:hover .img-item{transform:scale(1.4) rotate(8deg)}.img-rotate-l:hover .img-item{transform:scale(1.4) rotate(-8deg)}.img-container:hover .img-overlay{visibility:visible;opacity:1}.img-fluid{max-width:100%;height:auto}img{vertical-align:middle;border-style:none}"]
            },] }
];
ImageOverlayComponent.propDecorators = {
    imgName: [{ type: Input }],
    imgSrc: [{ type: Input }],
    imgAnimation: [{ type: Input }],
    imgOverlayAnimation: [{ type: Input }],
    imgOverlayBgColor: [{ type: Input }]
};
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiaW1hZ2Utb3ZlcmxheS5jb21wb25lbnQuanMiLCJzb3VyY2VSb290IjoiIiwic291cmNlcyI6WyIuLi8uLi8uLi8uLi8uLi8uLi8uLi8uLi9zcmMvYXBwL2xpYi9pbWFnZS1vdmVybGF5L2NvbXBvbmVudHMvaW1hZ2Utb3ZlcmxheS9pbWFnZS1vdmVybGF5LmNvbXBvbmVudC50cyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFBQSxPQUFPLEVBQUUsdUJBQXVCLEVBQUUsU0FBUyxFQUFFLEtBQUssRUFBRSxNQUFNLGVBQWUsQ0FBQztBQW1CMUUsTUFBTSxPQUFPLHFCQUFxQjtJQU5sQztRQU9DOztXQUVHO1FBRUgsWUFBTyxHQUFHLE9BQU8sQ0FBQztRQVFsQjs7V0FFRztRQUVILGlCQUFZLEdBQWlCLElBQUksQ0FBQztRQUVsQzs7V0FFRztRQUVILHdCQUFtQixHQUF3QixJQUFJLENBQUM7UUFFaEQ7O1dBRUc7UUFFSCxzQkFBaUIsR0FBc0IsY0FBYyxDQUFDO0lBQ3ZELENBQUM7OztZQXBDQSxTQUFTLFNBQUM7Z0JBQ1YsUUFBUSxFQUFFLHFCQUFxQjtnQkFDL0Isa1lBQTZDO2dCQUU3QyxlQUFlLEVBQUUsdUJBQXVCLENBQUMsTUFBTTs7YUFDL0M7OztzQkFLQyxLQUFLO3FCQU1MLEtBQUs7MkJBTUwsS0FBSztrQ0FNTCxLQUFLO2dDQU1MLEtBQUsiLCJzb3VyY2VzQ29udGVudCI6WyJpbXBvcnQgeyBDaGFuZ2VEZXRlY3Rpb25TdHJhdGVneSwgQ29tcG9uZW50LCBJbnB1dCB9IGZyb20gJ0Bhbmd1bGFyL2NvcmUnO1xuXG50eXBlIEltZ0FuaW1hdGlvbiA9ICd6b29tLWluJyB8ICdyb3RhdGUtbCcgfCAncm90YXRlLXInIHwgbnVsbDtcbnR5cGUgSW1nT3ZlcmxheUFuaW1hdGlvbiA9XG5cdHwgJ3NsaWRlLXRvcCdcblx0fCAnc2xpZGUtcmlnaHQnXG5cdHwgJ3NsaWRlLWRvd24nXG5cdHwgJ3NsaWRlLWxlZnQnXG5cdHwgJ3pvb20taW4nXG5cdHwgJ3pvb20tb3V0J1xuXHR8IG51bGw7XG50eXBlIEltZ092ZXJsYXlCZ0NvbG9yID0gJ2JsYWNrJyB8ICd3aGl0ZScgfCAncHJpbWFyeS1kYXJrJztcblxuQENvbXBvbmVudCh7XG5cdHNlbGVjdG9yOiAndWktbmctaW1hZ2Utb3ZlcmxheScsXG5cdHRlbXBsYXRlVXJsOiAnLi9pbWFnZS1vdmVybGF5LmNvbXBvbmVudC5odG1sJyxcblx0c3R5bGVVcmxzOiBbJy4vaW1hZ2Utb3ZlcmxheS5jb21wb25lbnQuc2NzcyddLFxuXHRjaGFuZ2VEZXRlY3Rpb246IENoYW5nZURldGVjdGlvblN0cmF0ZWd5Lk9uUHVzaCxcbn0pXG5leHBvcnQgY2xhc3MgSW1hZ2VPdmVybGF5Q29tcG9uZW50IHtcblx0LyoqXG5cdCAqIEltYWdlIE5hbWVcblx0ICovXG5cdEBJbnB1dCgpXG5cdGltZ05hbWUgPSAnaW1hZ2UnO1xuXG5cdC8qKlxuXHQgKiBJbWFnZSBTb3VyY2Vcblx0ICovXG5cdEBJbnB1dCgpXG5cdGltZ1NyYyE6IHN0cmluZztcblxuXHQvKipcblx0ICogSW1hZ2UgQW5pbWF0aW9uIE5hbWVcblx0ICovXG5cdEBJbnB1dCgpXG5cdGltZ0FuaW1hdGlvbjogSW1nQW5pbWF0aW9uID0gbnVsbDtcblxuXHQvKipcblx0ICogSW1hZ2UgT3ZlcmxheSBBbmltYXRpb24gTmFtZVxuXHQgKi9cblx0QElucHV0KClcblx0aW1nT3ZlcmxheUFuaW1hdGlvbjogSW1nT3ZlcmxheUFuaW1hdGlvbiA9IG51bGw7XG5cblx0LyoqXG5cdCAqIEltYWdlIE92ZXJsYXkgQmFja2dyb3VuZCBDb2xvclxuXHQgKi9cblx0QElucHV0KClcblx0aW1nT3ZlcmxheUJnQ29sb3I6IEltZ092ZXJsYXlCZ0NvbG9yID0gJ3ByaW1hcnktZGFyayc7XG59XG4iXX0=