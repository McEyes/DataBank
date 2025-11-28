import * as ɵngcc0 from '@angular/core';
declare type ImgAnimation = 'zoom-in' | 'rotate-l' | 'rotate-r' | null;
declare type ImgOverlayAnimation = 'slide-top' | 'slide-right' | 'slide-down' | 'slide-left' | 'zoom-in' | 'zoom-out' | null;
declare type ImgOverlayBgColor = 'black' | 'white' | 'primary-dark';
export declare class ImageOverlayComponent {
    /**
     * Image Name
     */
    imgName: string;
    /**
     * Image Source
     */
    imgSrc: string;
    /**
     * Image Animation Name
     */
    imgAnimation: ImgAnimation;
    /**
     * Image Overlay Animation Name
     */
    imgOverlayAnimation: ImgOverlayAnimation;
    /**
     * Image Overlay Background Color
     */
    imgOverlayBgColor: ImgOverlayBgColor;
    static ɵfac: ɵngcc0.ɵɵFactoryDeclaration<ImageOverlayComponent, never>;
    static ɵcmp: ɵngcc0.ɵɵComponentDeclaration<ImageOverlayComponent, "ui-ng-image-overlay", never, { "imgName": "imgName"; "imgAnimation": "imgAnimation"; "imgOverlayAnimation": "imgOverlayAnimation"; "imgOverlayBgColor": "imgOverlayBgColor"; "imgSrc": "imgSrc"; }, {}, never, ["*"]>;
}
export {};

//# sourceMappingURL=image-overlay.component.d.ts.map