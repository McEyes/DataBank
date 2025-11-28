import { ChangeDetectionStrategy, ChangeDetectorRef, Component, Input, ViewEncapsulation, } from '@angular/core';
import { LayoutPanelPosition } from '../models';
import { WindowRef } from '../refs/window.ref';
export class LayoutComponent {
    constructor(changeDetectorRef, windowRef) {
        this.changeDetectorRef = changeDetectorRef;
        this.windowRef = windowRef;
        /**
         * Top template
         */
        this.topTemplate = null;
        /**
         * Left template
         */
        this.leftTemplate = null;
        /**
         * Right template
         */
        this.rightTemplate = null;
        /**
         * Bottom Template
         */
        this.bottomTemplate = null;
        /**
         * SimpleBar Options
         */
        this.scrollBarOptions = { autoHide: true, scrollbarMinSize: 100 };
        /**
         * open left container if fixed
         */
        this.isLeftOpened = false;
        /**
         * open right container if fixed
         */
        this.isRightOpened = false;
        /**
         * open right container if less than 991.98px
         */
        this.isRightXsOpened = false;
        /**
         * open left container if less than 991.98px
         */
        this.isLeftXsOpened = false;
        /**
         * Show Overlay: boolean
         */
        this.showOverlay = false;
        /**
         * Left container hovered
         */
        this.leftContainerHovered = false;
        /**
         * Right container hovered
         */
        this.rightContainerHovered = false;
        this._config = LayoutComponent.DEFAULT_CONFIG;
    }
    /**
     * Gets component config
     */
    get config() {
        return this._config;
    }
    /**
     * Sets component config
     * @param value - Component config
     */
    set config(value) {
        this._config = Object.assign(Object.assign({}, LayoutComponent.DEFAULT_CONFIG), value);
    }
    /**
     * Can minimize left container content when Hover collapsed
     */
    get leftHoverMinimizedContent() {
        return (this.config.leftContainerPosition === LayoutPanelPosition.Hover &&
            !!this.config.shiftLeftContentWithHoverMode &&
            !this.leftContainerHovered &&
            !(this.windowRef.getWindow().innerWidth <= 991.98));
    }
    /**
     * Can minimize right container content when Hover collapsed
     */
    get rightHoverMinimizedContent() {
        return (this.config.rightContainerPosition === LayoutPanelPosition.Hover &&
            !!this.config.shiftRightContentWithHoverMode &&
            !this.rightContainerHovered &&
            !(this.windowRef.getWindow().innerWidth <= 991.98));
    }
    /**
     * Layout component wrapper classNames
     */
    get layoutContainerClass() {
        return {
            'switch-side-containers': !!(this.config.toggleLeftAndRightPosition &&
                this.config.rightContainerPosition !==
                    LayoutPanelPosition.Default &&
                this.config.leftContainerPosition !==
                    LayoutPanelPosition.Default) ||
                !!(this.config.toggleLeftAndRightPosition &&
                    !this.leftTemplate &&
                    this.rightTemplate &&
                    this.config.rightContainerPosition !==
                        LayoutPanelPosition.Default) ||
                !!(this.config.toggleLeftAndRightPosition &&
                    !this.rightTemplate &&
                    this.leftTemplate &&
                    this.config.leftContainerPosition !==
                        LayoutPanelPosition.Default),
            'show-overlay': this.showOverlay,
            'left-fixed': this.config.leftContainerPosition === LayoutPanelPosition.Fixed,
            'left-hidden': this.config.leftContainerPosition ===
                LayoutPanelPosition.Hidden,
            'left-opened': this.config.leftContainerPosition ===
                LayoutPanelPosition.Hidden && this.isLeftOpened,
            'left-opened-xs': this.isLeftXsOpened,
            'left-hover': this.config.leftContainerPosition === LayoutPanelPosition.Hover,
            'left-shift-content': this.leftHoverMinimizedContent &&
                (!this.config.toggleLeftAndRightPosition ||
                    (this.config.toggleLeftAndRightPosition &&
                        this.config.rightContainerPosition ===
                            LayoutPanelPosition.Default)),
            'right-fixed': this.config.rightContainerPosition ===
                LayoutPanelPosition.Fixed,
            'right-hidden': this.config.rightContainerPosition ===
                LayoutPanelPosition.Hidden,
            'right-opened': this.config.rightContainerPosition ===
                LayoutPanelPosition.Hidden && this.isRightOpened,
            'right-opened-xs': this.isRightXsOpened,
            'right-hover': this.config.rightContainerPosition ===
                LayoutPanelPosition.Hover,
            'right-shift-content': this.rightHoverMinimizedContent &&
                (this.config.toggleLeftAndRightPosition ||
                    (!this.config.toggleLeftAndRightPosition &&
                        this.config.leftContainerPosition ===
                            LayoutPanelPosition.Default)),
            'fixed-full-screen-on-small-resolution': !!this.config.fullWidthOnSmallScreen,
        };
    }
    /**
     * Hide overlay
     */
    closeContainer() {
        this.showOverlay = false;
        this.isRightOpened = false;
        this.isLeftOpened = false;
        this.isRightXsOpened = false;
        this.isLeftXsOpened = false;
        this.changeDetectorRef.markForCheck();
    }
    /**
     * Show right container
     * @param withOverlay :boolean - show overlay
     */
    showRightContainer(withOverlay = false) {
        if (this.windowRef.getWindow().innerWidth <= 991.98 &&
            this.config.fullWidthOnSmallScreen &&
            this.config.rightContainerPosition !== LayoutPanelPosition.Default) {
            this.showOverlay = withOverlay;
            this.isRightXsOpened = true;
            this.isRightOpened = true;
        }
        else if (this.config.rightContainerPosition === LayoutPanelPosition.Hidden) {
            this.showOverlay = withOverlay;
            this.isRightOpened = true;
        }
        this.changeDetectorRef.markForCheck();
    }
    /**
     * Show left container
     * @param withOverlay :boolean - show overlay
     */
    showLeftContainer(withOverlay = false) {
        if (this.windowRef.getWindow().innerWidth <= 991.98 &&
            this.config.fullWidthOnSmallScreen &&
            this.config.leftContainerPosition !== LayoutPanelPosition.Default) {
            this.showOverlay = withOverlay;
            this.isLeftXsOpened = true;
            this.isLeftOpened = true;
        }
        else if (this.config.leftContainerPosition === LayoutPanelPosition.Hidden) {
            this.showOverlay = withOverlay;
            this.isLeftOpened = true;
        }
        this.changeDetectorRef.markForCheck();
    }
    /**
     * Left container mouse enter
     */
    leftContainerMouseEnter() {
        this.leftContainerHovered = true;
    }
    /**
     * Left Container mouse leave
     */
    leftContainerMouseLeave() {
        this.leftContainerHovered = false;
    }
    /**
     * Right container mouse enter
     */
    rightContainerMouseEnter() {
        this.rightContainerHovered = true;
    }
    /**
     * Right container mouse leave
     */
    rightContainerMouseLeave() {
        this.rightContainerHovered = false;
    }
}
LayoutComponent.DEFAULT_CONFIG = {
    leftContainerPosition: LayoutPanelPosition.Default,
    rightContainerPosition: LayoutPanelPosition.Default,
    toggleLeftAndRightPosition: false,
    fullWidthOnSmallScreen: false,
    shiftLeftContentWithHoverMode: false,
    shiftRightContentWithHoverMode: false,
};
LayoutComponent.decorators = [
    { type: Component, args: [{
                selector: 'ui-ng-layout',
                template: "<div class=\"layout-page-wrap\" [ngClass]=\"layoutContainerClass\">\n\t<div\n\t\tclass=\"layout-left-container\"\n\t\t*ngIf=\"leftTemplate\"\n\t\t(mouseenter)=\"leftContainerMouseEnter()\"\n\t\t(mouseleave)=\"leftContainerMouseLeave()\"\n\t>\n\t\t<div class=\"layout-left-container-content\">\n\t\t\t<ng-container *ngTemplateOutlet=\"leftTemplate\"></ng-container>\n\t\t</div>\n\t</div>\n\t<div class=\"layout-central-container\">\n\t\t<ng-container *ngTemplateOutlet=\"centralContent\"></ng-container>\n\t</div>\n\t<div\n\t\tclass=\"layout-right-container\"\n\t\t*ngIf=\"rightTemplate\"\n\t\t(mouseenter)=\"rightContainerMouseEnter()\"\n\t\t(mouseleave)=\"rightContainerMouseLeave()\"\n\t>\n\t\t<div class=\"layout-right-container-content\">\n\t\t\t<ng-container *ngTemplateOutlet=\"rightTemplate\"></ng-container>\n\t\t</div>\n\t</div>\n\n\t<div\n\t\tclass=\"layout-page-overlay\"\n\t\t(click)=\"closeContainer()\"\n\t\t(keyup.enter)=\"closeContainer()\"\n\t></div>\n</div>\n\n<ng-template #centralContent>\n\t<div class=\"layout-top-container\" *ngIf=\"topTemplate\">\n\t\t<ng-container *ngTemplateOutlet=\"topTemplate\"></ng-container>\n\t</div>\n\t<div class=\"layout-main-container\">\n\t\t<ng-content></ng-content>\n\t</div>\n\t<div class=\"layout-bottom-container\" *ngIf=\"bottomTemplate\">\n\t\t<ng-container *ngTemplateOutlet=\"bottomTemplate\"></ng-container>\n\t</div>\n</ng-template>\n",
                encapsulation: ViewEncapsulation.None,
                changeDetection: ChangeDetectionStrategy.OnPush,
                styles: [""]
            },] }
];
LayoutComponent.ctorParameters = () => [
    { type: ChangeDetectorRef },
    { type: WindowRef }
];
LayoutComponent.propDecorators = {
    topTemplate: [{ type: Input }],
    leftTemplate: [{ type: Input }],
    rightTemplate: [{ type: Input }],
    bottomTemplate: [{ type: Input }],
    scrollBarOptions: [{ type: Input }],
    config: [{ type: Input }]
};
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoibGF5b3V0LmNvbXBvbmVudC5qcyIsInNvdXJjZVJvb3QiOiIiLCJzb3VyY2VzIjpbIi4uLy4uLy4uLy4uLy4uLy4uLy4uL3NyYy9hcHAvbGliL2xheW91dC9jb21wb25lbnRzL2xheW91dC5jb21wb25lbnQudHMiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IkFBQUEsT0FBTyxFQUNOLHVCQUF1QixFQUN2QixpQkFBaUIsRUFDakIsU0FBUyxFQUNULEtBQUssRUFFTCxpQkFBaUIsR0FDakIsTUFBTSxlQUFlLENBQUM7QUFFdkIsT0FBTyxFQUFnQixtQkFBbUIsRUFBRSxNQUFNLFdBQVcsQ0FBQztBQUM5RCxPQUFPLEVBQUUsU0FBUyxFQUFFLE1BQU0sb0JBQW9CLENBQUM7QUFTL0MsTUFBTSxPQUFPLGVBQWU7SUF1RTNCLFlBQ1MsaUJBQW9DLEVBQ3BDLFNBQW9CO1FBRHBCLHNCQUFpQixHQUFqQixpQkFBaUIsQ0FBbUI7UUFDcEMsY0FBUyxHQUFULFNBQVMsQ0FBVztRQS9EN0I7O1dBRUc7UUFFSCxnQkFBVyxHQUE2QixJQUFJLENBQUM7UUFFN0M7O1dBRUc7UUFFSCxpQkFBWSxHQUE2QixJQUFJLENBQUM7UUFFOUM7O1dBRUc7UUFFSCxrQkFBYSxHQUE2QixJQUFJLENBQUM7UUFFL0M7O1dBRUc7UUFFSCxtQkFBYyxHQUE2QixJQUFJLENBQUM7UUFFaEQ7O1dBRUc7UUFFSCxxQkFBZ0IsR0FBRyxFQUFFLFFBQVEsRUFBRSxJQUFJLEVBQUUsZ0JBQWdCLEVBQUUsR0FBRyxFQUFFLENBQUM7UUFFN0Q7O1dBRUc7UUFDSCxpQkFBWSxHQUFHLEtBQUssQ0FBQztRQUNyQjs7V0FFRztRQUNILGtCQUFhLEdBQUcsS0FBSyxDQUFDO1FBQ3RCOztXQUVHO1FBQ0gsb0JBQWUsR0FBRyxLQUFLLENBQUM7UUFDeEI7O1dBRUc7UUFDSCxtQkFBYyxHQUFHLEtBQUssQ0FBQztRQUN2Qjs7V0FFRztRQUNILGdCQUFXLEdBQUcsS0FBSyxDQUFDO1FBQ3BCOztXQUVHO1FBQ0gseUJB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QUMsV0FBVyxHQUFHLFdBQVcsQ0FBQztZQUMvQixJQUFJLENBQUMsWUFBWSxHQUFHLElBQUksQ0FBQztTQUN6QjtRQUNELElBQUksQ0FBQyxpQkFBaUIsQ0FBQyxZQUFZLEVBQUUsQ0FBQztJQUN2QyxDQUFDO0lBRUQ7O09BRUc7SUFDSCx1QkFBdUI7UUFDdEIsSUFBSSxDQUFDLG9CQUFvQixHQUFHLElBQUksQ0FBQztJQUNsQyxDQUFDO0lBRUQ7O09BRUc7SUFDSCx1QkFBdUI7UUFDdEIsSUFBSSxDQUFDLG9CQUFvQixHQUFHLEtBQUssQ0FBQztJQUNuQyxDQUFDO0lBRUQ7O09BRUc7SUFDSCx3QkFBd0I7UUFDdkIsSUFBSSxDQUFDLHFCQUFxQixHQUFHLElBQUksQ0FBQztJQUNuQyxDQUFDO0lBRUQ7O09BRUc7SUFDSCx3QkFBd0I7UUFDdkIsSUFBSSxDQUFDLHFCQUFxQixHQUFHLEtBQUssQ0FBQztJQUNwQyxDQUFDOztBQTdRdUIsOEJBQWMsR0FBRztJQUN4QyxxQkFBcUIsRUFBRSxtQkFBbUIsQ0FBQyxPQUFPO0lBQ2xELHNCQUFzQixFQUFFLG1CQUFtQixDQUFDLE9BQU87SUFDbkQsMEJBQTBCLEVBQUUsS0FBSztJQUNqQyxzQkFBc0IsRUFBRSxLQUFLO0lBQzdCLDZCQUE2QixFQUFFLEtBQUs7SUFDcEMsOEJBQThCLEVBQUUsS0FBSztDQUNyQyxDQUFDOztZQWZGLFNBQVMsU0FBQztnQkFDVixRQUFRLEVBQUUsY0FBYztnQkFDeEIsbTRDQUFzQztnQkFFdEMsYUFBYSxFQUFFLGlCQUFpQixDQUFDLElBQUk7Z0JBQ3JDLGVBQWUsRUFBRSx1QkFBdUIsQ0FBQyxNQUFNOzthQUMvQzs7O1lBaEJBLGlCQUFpQjtZQVFULFNBQVM7OzswQkFzQmhCLEtBQUs7MkJBTUwsS0FBSzs0QkFNTCxLQUFLOzZCQU1MLEtBQUs7K0JBTUwsS0FBSztxQkFrREwsS0FBSyIsInNvdXJjZXNDb250ZW50IjpbImltcG9ydCB7XG5cdENoYW5nZURldGVjdGlvblN0cmF0ZWd5LFxuXHRDaGFuZ2VEZXRlY3RvclJlZixcblx0Q29tcG9uZW50LFxuXHRJbnB1dCxcblx0VGVtcGxhdGVSZWYsXG5cdFZpZXdFbmNhcHN1bGF0aW9uLFxufSBmcm9tICdAYW5ndWxhci9jb3JlJztcblxuaW1wb3J0IHsgTGF5b3V0Q29uZmlnLCBMYXlvdXRQYW5lbFBvc2l0aW9uIH0gZnJvbSAnLi4vbW9kZWxzJztcbmltcG9ydCB7IFdpbmRvd1JlZiB9IGZyb20gJy4uL3JlZnMvd2luZG93LnJlZic7XG5cbkBDb21wb25lbnQoe1xuXHRzZWxlY3RvcjogJ3VpLW5nLWxheW91dCcsXG5cdHRlbXBsYXRlVXJsOiAnLi9sYXlvdXQuY29tcG9uZW50Lmh0bWwnLFxuXHRzdHlsZVVybHM6IFsnLi9sYXlvdXQuY29tcG9uZW50LnNjc3MnXSxcblx0ZW5jYXBzdWxhdGlvbjogVmlld0VuY2Fwc3VsYXRpb24uTm9uZSxcblx0Y2hhbmdlRGV0ZWN0aW9uOiBDaGFuZ2VEZXRlY3Rpb25TdHJhdGVneS5PblB1c2gsXG59KVxuZXhwb3J0IGNsYXNzIExheW91dENvbXBvbmVudCB7XG5cdHByaXZhdGUgc3RhdGljIHJlYWRvbmx5IERFRkFVTFRfQ09ORklHID0ge1xuXHRcdGxlZnRDb250YWluZXJQb3NpdGlvbjogTGF5b3V0UGFuZWxQb3NpdGlvbi5EZWZhdWx0LFxuXHRcdHJpZ2h0Q29udGFpbmVyUG9zaXRpb246IExheW91dFBhbmVsUG9zaXRpb24uRGVmYXVsdCxcblx0XHR0b2dnbGVMZWZ0QW5kUmlnaHRQb3NpdGlvbjogZmFsc2UsXG5cdFx0ZnVsbFdpZHRoT25TbWFsbFNjcmVlbjogZmFsc2UsXG5cdFx0c2hpZnRMZWZ0Q29udGVudFdpdGhIb3Zlck1vZGU6IGZhbHNlLFxuXHRcdHNoaWZ0UmlnaHRDb250ZW50V2l0aEhvdmVyTW9kZTogZmFsc2UsXG5cdH07XG5cblx0LyoqXG5cdCAqIFRvcCB0ZW1wbGF0ZVxuXHQgKi9cblx0QElucHV0KClcblx0dG9wVGVtcGxhdGU6IFRlbXBsYXRlUmVmPHZvaWQ+IHwgbnVsbCA9IG51bGw7XG5cblx0LyoqXG5cdCAqIExlZnQgdGVtcGxhdGVcblx0ICovXG5cdEBJbnB1dCgpXG5cdGxlZnRUZW1wbGF0ZTogVGVtcGxhdGVSZWY8dm9pZD4gfCBudWxsID0gbnVsbDtcblxuXHQvKipcblx0ICogUmlnaHQgdGVtcGxhdGVcblx0ICovXG5cdEBJbnB1dCgpXG5cdHJpZ2h0VGVtcGxhdGU6IFRlbXBsYXRlUmVmPHZvaWQ+IHwgbnVsbCA9IG51bGw7XG5cblx0LyoqXG5cdCAqIEJvdHRvbSBUZW1wbGF0ZVxuXHQgKi9cblx0QElucHV0KClcblx0Ym90dG9tVGVtcGxhdGU6IFRlbXBsYXRlUmVmPHZvaWQ+IHwgbnVsbCA9IG51bGw7XG5cblx0LyoqXG5cdCAqIFNpbXBsZUJhciBPcHRpb25zXG5cdCAqL1xuXHRASW5wdXQoKVxuXHRzY3JvbGxCYXJPcHRpb25zID0geyBhdXRvSGlkZTogdHJ1ZSwgc2Nyb2xsYmFyTWluU2l6ZTogMTAwIH07XG5cblx0LyoqXG5cdCAqIG9wZW4gbGVmdCBjb250YWluZXIgaWYgZml4ZWRcblx0ICovXG5cdGlzTGVmdE9wZW5lZCA9IGZhbHNlO1xuXHQvKipcblx0ICogb3BlbiByaWdodCBjb250YWluZXIgaWYgZml4ZWRcblx0ICovXG5cdGlzUmlnaHRPcGVuZWQgPSBmYWxzZTtcblx0LyoqXG5cdCAqIG9wZW4gcmlnaHQgY29udGFpbmVyIGlmIGxlc3MgdGhhbiA5OTEuOThweFxuXHQgKi9cblx0aXNSaWdodFhzT3BlbmVkID0gZmFsc2U7XG5cdC8qKlxuXHQgKiBvcGVuIGxlZnQgY29udGFpbmVyIGlmIGxlc3MgdGhhbiA5OTEuOThweFxuXHQgKi9cblx0aXNMZWZ0WHNPcGVuZWQgPSBmYWxzZTtcblx0LyoqXG5cdCAqIFNob3cgT3ZlcmxheTogYm9vbGVhblxuXHQgKi9cblx0c2hvd092ZXJsYXkgPSBmYWxzZTtcblx0LyoqXG5cdCAqIExlZnQgY29udGFpbmVyIGhvdmVyZWRcblx0ICovXG5cdGxlZnRDb250YWluZXJIb3ZlcmVkID0gZmFsc2U7XG5cdC8qKlxuXHQgKiBSaWdodCBjb250YWluZXIgaG92ZXJlZFxuXHQgKi9cblx0cmlnaHRDb250YWluZXJIb3ZlcmVkID0gZmFsc2U7XG5cblx0cHJpdmF0ZSBfY29uZmlnOiBMYXlvdXRDb25maWcgPSBMYXlvdXRDb21wb25lbnQuREVGQVVMVF9DT05GSUc7XG5cblx0Y29uc3RydWN0b3IoXG5cdFx0cHJpdmF0ZSBjaGFuZ2VEZXRlY3RvclJlZjogQ2hhbmdlRGV0ZWN0b3JSZWYsXG5cdFx0cHJpdmF0ZSB3aW5kb3dSZWY6IFdpbmRvd1JlZixcblx0KSB7fVxuXG5cdC8qKlxuXHQgKiBHZXRzIGNvbXBvbmVudCBjb25maWdcblx0ICovXG5cdGdldCBjb25maWcoKTogTGF5b3V0Q29uZmlnIHtcblx0XHRyZXR1cm4gdGhpcy5fY29uZmlnO1xuXHR9XG5cblx0LyoqXG5cdCAqIFNldHMgY29tcG9uZW50IGNvbmZpZ1xuXHQgKiBAcGFyYW0gdmFsdWUgLSBDb21wb25lbnQgY29uZmlnXG5cdCAqL1xuXHRASW5wdXQoKVxuXHRzZXQgY29uZmlnKHZhbHVlOiBMYXlvdXRDb25maWcpIHtcblx0XHR0aGlzLl9jb25maWcgPSB7XG5cdFx0XHQuLi5MYXlvdXRDb21wb25lbnQuREVGQVVMVF9DT05GSUcsXG5cdFx0XHQuLi52YWx1ZSxcblx0XHR9O1xuXHR9XG5cblx0LyoqXG5cdCAqIENhbiBtaW5pbWl6ZSBsZWZ0IGNvbnRhaW5lciBjb250ZW50IHdoZW4gSG92ZXIgY29sbGFwc2VkXG5cdCAqL1xuXHRnZXQgbGVmdEhvdmVyTWluaW1pemVkQ29udGVudCgpOiBib29sZWFuIHtcblx0XHRyZXR1cm4gKFxuXHRcdFx0dGhpcy5jb25maWcubGVmdENvbnRhaW5lclBvc2l0aW9uID09PSBMYXlvdXRQYW5lbFBvc2l0aW9uLkhvdmVyICYmXG5cdFx0XHQhIXRoaXMuY29uZmlnLnNoaWZ0TGVmdENvbnRlbnRXaXRoSG92ZXJNb2RlICYmXG5cdFx0XHQhdGhpcy5sZWZ0Q29udGFpbmVySG92ZXJlZCAmJlxuXHRcdFx0ISh0aGlzLndpbmRvd1JlZi5nZXRXaW5kb3coKS5pbm5lcldpZHRoIDw9IDk5MS45OClcblx0XHQpO1xuXHR9XG5cblx0LyoqXG5cdCAqIENhbiBtaW5pbWl6ZSByaWdodCBjb250YWluZXIgY29udGVudCB3aGVuIEhvdmVyIGNvbGxhcHNlZFxuXHQgKi9cblx0Z2V0IHJpZ2h0SG92ZXJNaW5pbWl6ZWRDb250ZW50KCk6IGJvb2xlYW4ge1xuXHRcdHJldHVybiAoXG5cdFx0XHR0aGlzLmNvbmZpZy5yaWdodENvbnRhaW5lclBvc2l0aW9uID09PSBMYXlvdXRQYW5lbFBvc2l0aW9uLkhvdmVyICYmXG5cdFx0XHQhIXRoaXMuY29uZmlnLnNoaWZ0UmlnaHRDb250ZW50V2l0aEhvdmVyTW9kZSAmJlxuXHRcdFx0IXRoaXMucmlnaHRDb250YWluZXJIb3ZlcmVkICYmXG5cdFx0XHQhKHRoaXMud2luZG93UmVmLmdldFdpbmRvdygpLmlubmVyV2lkdGggPD0gOTkxLjk4KVxuXHRcdCk7XG5cdH1cblxuXHQvKipcblx0ICogTGF5b3V0IGNvbXBvbmVudCB3cmFwcGVyIGNsYXNzTmFtZXNcblx0ICovXG5cdGdldCBsYXlvdXRDb250YWluZXJDbGFzcygpOiB7IFtrZXk6IHN0cmluZ106IGJvb2xlYW4gfSB7XG5cdFx0cmV0dXJuIHtcblx0XHRcdCdzd2l0Y2gtc2lkZS1jb250YWluZXJzJzpcblx0XHRcdFx0ISEoXG5cdFx0XHRcdFx0dGhpcy5jb25maWcudG9nZ2xlTGVmdEFuZFJpZ2h0UG9zaXRpb24gJiZcblx0XHRcdFx0XHR0aGlzLmNvbmZpZy5yaWdodENvbnRhaW5lclBvc2l0aW9uICE9PVxuXHRcdFx0XHRcdFx0TGF5b3V0UGFuZWxQb3NpdGlvbi5EZWZhdWx0ICYmXG5cdFx0XHRcdFx0dGhpcy5jb25maWcubGVmdENvbnRhaW5lclBvc2l0aW9uICE9PVxuXHRcdFx0XHRcdFx0TGF5b3V0UGFuZWxQb3NpdGlvbi5EZWZhdWx0XG5cdFx0XHRcdCkgfHxcblx0XHRcdFx0ISEoXG5cdFx0XHRcdFx0dGhpcy5jb25maWcudG9nZ2xlTGVmdEFuZFJpZ2h0UG9zaXRpb24gJiZcblx0XHRcdFx0XHQhdGhpcy5sZWZ0VGVtcGxhdGUgJiZcblx0XHRcdFx0XHR0aGlzLnJpZ2h0VGVtcGxhdGUgJiZcblx0XHRcdFx0XHR0aGlzLmNvbmZpZy5yaWdodENvbnRhaW5lclBvc2l0aW9uICE9PVxuXHRcdFx0XHRcdFx0TGF5b3V0UGFuZWxQb3NpdGlvbi5EZWZhdWx0XG5cdFx0XHRcdCkgfHxcblx0XHRcdFx0ISEoXG5cdFx0XHRcdFx0dGhpcy5jb25maWcudG9nZ2xlTGVmdEFuZFJpZ2h0UG9zaXRpb24gJiZcblx0XHRcdFx0XHQhdGhpcy5yaWdodFRlbXBsYXRlICYmXG5cdFx0XHRcdFx0dGhpcy5sZWZ0VGVtcGxhdGUgJiZcblx0XHRcdFx0XHR0aGlzLmNvbmZpZy5sZWZ0Q29udGFpbmVyUG9zaXRpb24gIT09XG5cdFx0XHRcdFx0XHRMYXlvdXRQYW5lbFBvc2l0aW9uLkRlZmF1bHRcblx0XHRcdFx0KSxcblx0XHRcdCdzaG93LW92ZXJsYXknOiB0aGlzLnNob3dPdmVybGF5LFxuXHRcdFx0J2xlZnQtZml4ZWQnOlxuXHRcdFx0XHR0aGlzLmNvbmZpZy5sZWZ0Q29udGFpbmVyUG9zaXRpb24gPT09IExheW91dFBhbmVsUG9zaXRpb24uRml4ZWQsXG5cdFx0XHQnbGVmdC1oaWRkZW4nOlxuXHRcdFx0XHR0aGlzLmNvbmZpZy5sZWZ0Q29udGFpbmVyUG9zaXRpb24gPT09XG5cdFx0XHRcdExheW91dFBhbmVsUG9zaXRpb24uSGlkZGVuLFxuXHRcdFx0J2xlZnQtb3BlbmVkJzpcblx0XHRcdFx0dGhpcy5jb25maWcubGVmdENvbnRhaW5lclBvc2l0aW9uID09PVxuXHRcdFx0XHRcdExheW91dFBhbmVsUG9zaXRpb24uSGlkZGVuICYmIHRoaXMuaXNMZWZ0T3BlbmVkLFxuXHRcdFx0J2xlZnQtb3BlbmVkLXhzJzogdGhpcy5pc0xlZnRYc09wZW5lZCxcblx0XHRcdCdsZWZ0LWhvdmVyJzpcblx0XHRcdFx0dGhpcy5jb25maWcubGVmdENvbnRhaW5lclBvc2l0aW9uID09PSBMYXlvdXRQYW5lbFBvc2l0aW9uLkhvdmVyLFxuXHRcdFx0J2xlZnQtc2hpZnQtY29udGVudCc6XG5cdFx0XHRcdHRoaXMubGVmdEhvdmVyTWluaW1pemVkQ29udGVudCAmJlxuXHRcdFx0XHQoIXRoaXMuY29uZmlnLnRvZ2dsZUxlZnRBbmRSaWdodFBvc2l0aW9uIHx8XG5cdFx0XHRcdFx0KHRoaXMuY29uZmlnLnRvZ2dsZUxlZnRBbmRSaWdodFBvc2l0aW9uICYmXG5cdFx0XHRcdFx0XHR0aGlzLmNvbmZpZy5yaWdodENvbnRhaW5lclBvc2l0aW9uID09PVxuXHRcdFx0XHRcdFx0XHRMYXlvdXRQYW5lbFBvc2l0aW9uLkRlZmF1bHQpKSxcblx0XHRcdCdyaWdodC1maXhlZCc6XG5cdFx0XHRcdHRoaXMuY29uZmlnLnJpZ2h0Q29udGFpbmVyUG9zaXRpb24gPT09XG5cdFx0XHRcdExheW91dFBhbmVsUG9zaXRpb24uRml4ZWQsXG5cdFx0XHQncmlnaHQtaGlkZGVuJzpcblx0XHRcdFx0dGhpcy5jb25maWcucmlnaHRDb250YWluZXJQb3NpdGlvbiA9PT1cblx0XHRcdFx0TGF5b3V0UGFuZWxQb3NpdGlvbi5IaWRkZW4sXG5cdFx0XHQncmlnaHQtb3BlbmVkJzpcblx0XHRcdFx0dGhpcy5jb25maWcucmlnaHRDb250YWluZXJQb3NpdGlvbiA9PT1cblx0XHRcdFx0XHRMYXlvdXRQYW5lbFBvc2l0aW9uLkhpZGRlbiAmJiB0aGlzLmlzUmlnaHRPcGVuZWQsXG5cdFx0XHQncmlnaHQtb3BlbmVkLXhzJzogdGhpcy5pc1JpZ2h0WHNPcGVuZWQsXG5cdFx0XHQncmlnaHQtaG92ZXInOlxuXHRcdFx0XHR0aGlzLmNvbmZpZy5yaWdodENvbnRhaW5lclBvc2l0aW9uID09PVxuXHRcdFx0XHRMYXlvdXRQYW5lbFBvc2l0aW9uLkhvdmVyLFxuXHRcdFx0J3JpZ2h0LXNoaWZ0LWNvbnRlbnQnOlxuXHRcdFx0XHR0aGlzLnJpZ2h0SG92ZXJNaW5pbWl6ZWRDb250ZW50ICYmXG5cdFx0XHRcdCh0aGlzLmNvbmZpZy50b2dnbGVMZWZ0QW5kUmlnaHRQb3NpdGlvbiB8fFxuXHRcdFx0XHRcdCghdGhpcy5jb25maWcudG9nZ2xlTGVmdEFuZFJpZ2h0UG9zaXRpb24gJiZcblx0XHRcdFx0XHRcdHRoaXMuY29uZmlnLmxlZnRDb250YWluZXJQb3NpdGlvbiA9PT1cblx0XHRcdFx0XHRcdFx0TGF5b3V0UGFuZWxQb3NpdGlvbi5EZWZhdWx0KSksXG5cdFx0XHQnZml4ZWQtZnVsbC1zY3JlZW4tb24tc21hbGwtcmVzb2x1dGlvbic6XG5cdFx0XHRcdCEhdGhpcy5jb25maWcuZnVsbFdpZHRoT25TbWFsbFNjcmVlbixcblx0XHR9O1xuXHR9XG5cblx0LyoqXG5cdCAqIEhpZGUgb3ZlcmxheVxuXHQgKi9cblx0Y2xvc2VDb250YWluZXIoKTogdm9pZCB7XG5cdFx0dGhpcy5zaG93T3ZlcmxheSA9IGZhbHNlO1xuXHRcdHRoaXMuaXNSaWdodE9wZW5lZCA9IGZhbHNlO1xuXHRcdHRoaXMuaXNMZWZ0T3BlbmVkID0gZmFsc2U7XG5cdFx0dGhpcy5pc1JpZ2h0WHNPcGVuZWQgPSBmYWxzZTtcblx0XHR0aGlzLmlzTGVmdFhzT3BlbmVkID0gZmFsc2U7XG5cdFx0dGhpcy5jaGFuZ2VEZXRlY3RvclJlZi5tYXJrRm9yQ2hlY2soKTtcblx0fVxuXG5cdC8qKlxuXHQgKiBTaG93IHJpZ2h0IGNvbnRhaW5lclxuXHQgKiBAcGFyYW0gd2l0aE92ZXJsYXkgOmJvb2xlYW4gLSBzaG93IG92ZXJsYXlcblx0ICovXG5cdHNob3dSaWdodENvbnRhaW5lcih3aXRoT3ZlcmxheTogYm9vbGVhbiA9IGZhbHNlKTogdm9pZCB7XG5cdFx0aWYgKFxuXHRcdFx0dGhpcy53aW5kb3dSZWYuZ2V0V2luZG93KCkuaW5uZXJXaWR0aCA8PSA5OTEuOTggJiZcblx0XHRcdHRoaXMuY29uZmlnLmZ1bGxXaWR0aE9uU21hbGxTY3JlZW4gJiZcblx0XHRcdHRoaXMuY29uZmlnLnJpZ2h0Q29udGFpbmVyUG9zaXRpb24gIT09IExheW91dFBhbmVsUG9zaXRpb24uRGVmYXVsdFxuXHRcdCkge1xuXHRcdFx0dGhpcy5zaG93T3ZlcmxheSA9IHdpdGhPdmVybGF5O1xuXHRcdFx0dGhpcy5pc1JpZ2h0WHNPcGVuZWQgPSB0cnVlO1xuXHRcdFx0dGhpcy5pc1JpZ2h0T3BlbmVkID0gdHJ1ZTtcblx0XHR9IGVsc2UgaWYgKFxuXHRcdFx0dGhpcy5jb25maWcucmlnaHRDb250YWluZXJQb3NpdGlvbiA9PT0gTGF5b3V0UGFuZWxQb3NpdGlvbi5IaWRkZW5cblx0XHQpIHtcblx0XHRcdHRoaXMuc2hvd092ZXJsYXkgPSB3aXRoT3ZlcmxheTtcblx0XHRcdHRoaXMuaXNSaWdodE9wZW5lZCA9IHRydWU7XG5cdFx0fVxuXHRcdHRoaXMuY2hhbmdlRGV0ZWN0b3JSZWYubWFya0ZvckNoZWNrKCk7XG5cdH1cblxuXHQvKipcblx0ICogU2hvdyBsZWZ0IGNvbnRhaW5lclxuXHQgKiBAcGFyYW0gd2l0aE92ZXJsYXkgOmJvb2xlYW4gLSBzaG93IG92ZXJsYXlcblx0ICovXG5cdHNob3dMZWZ0Q29udGFpbmVyKHdpdGhPdmVybGF5OiBib29sZWFuID0gZmFsc2UpOiB2b2lkIHtcblx0XHRpZiAoXG5cdFx0XHR0aGlzLndpbmRvd1JlZi5nZXRXaW5kb3coKS5pbm5lcldpZHRoIDw9IDk5MS45OCAmJlxuXHRcdFx0dGhpcy5jb25maWcuZnVsbFdpZHRoT25TbWFsbFNjcmVlbiAmJlxuXHRcdFx0dGhpcy5jb25maWcubGVmdENvbnRhaW5lclBvc2l0aW9uICE9PSBMYXlvdXRQYW5lbFBvc2l0aW9uLkRlZmF1bHRcblx0XHQpIHtcblx0XHRcdHRoaXMuc2hvd092ZXJsYXkgPSB3aXRoT3ZlcmxheTtcblx0XHRcdHRoaXMuaXNMZWZ0WHNPcGVuZWQgPSB0cnVlO1xuXHRcdFx0dGhpcy5pc0xlZnRPcGVuZWQgPSB0cnVlO1xuXHRcdH0gZWxzZSBpZiAoXG5cdFx0XHR0aGlzLmNvbmZpZy5sZWZ0Q29udGFpbmVyUG9zaXRpb24gPT09IExheW91dFBhbmVsUG9zaXRpb24uSGlkZGVuXG5cdFx0KSB7XG5cdFx0XHR0aGlzLnNob3dPdmVybGF5ID0gd2l0aE92ZXJsYXk7XG5cdFx0XHR0aGlzLmlzTGVmdE9wZW5lZCA9IHRydWU7XG5cdFx0fVxuXHRcdHRoaXMuY2hhbmdlRGV0ZWN0b3JSZWYubWFya0ZvckNoZWNrKCk7XG5cdH1cblxuXHQvKipcblx0ICogTGVmdCBjb250YWluZXIgbW91c2UgZW50ZXJcblx0ICovXG5cdGxlZnRDb250YWluZXJNb3VzZUVudGVyKCk6IHZvaWQge1xuXHRcdHRoaXMubGVmdENvbnRhaW5lckhvdmVyZWQgPSB0cnVlO1xuXHR9XG5cblx0LyoqXG5cdCAqIExlZnQgQ29udGFpbmVyIG1vdXNlIGxlYXZlXG5cdCAqL1xuXHRsZWZ0Q29udGFpbmVyTW91c2VMZWF2ZSgpOiB2b2lkIHtcblx0XHR0aGlzLmxlZnRDb250YWluZXJIb3ZlcmVkID0gZmFsc2U7XG5cdH1cblxuXHQvKipcblx0ICogUmlnaHQgY29udGFpbmVyIG1vdXNlIGVudGVyXG5cdCAqL1xuXHRyaWdodENvbnRhaW5lck1vdXNlRW50ZXIoKTogdm9pZCB7XG5cdFx0dGhpcy5yaWdodENvbnRhaW5lckhvdmVyZWQgPSB0cnVlO1xuXHR9XG5cblx0LyoqXG5cdCAqIFJpZ2h0IGNvbnRhaW5lciBtb3VzZSBsZWF2ZVxuXHQgKi9cblx0cmlnaHRDb250YWluZXJNb3VzZUxlYXZlKCk6IHZvaWQge1xuXHRcdHRoaXMucmlnaHRDb250YWluZXJIb3ZlcmVkID0gZmFsc2U7XG5cdH1cbn1cbiJdfQ==