(function (global, factory) {
    typeof exports === 'object' && typeof module !== 'undefined' ? factory(exports, require('@angular/common'), require('@angular/core')) :
    typeof define === 'function' && define.amd ? define('@jabil/ui-ng', ['exports', '@angular/common', '@angular/core'], factory) :
    (global = typeof globalThis !== 'undefined' ? globalThis : global || self, factory((global.jabil = global.jabil || {}, global.jabil['ui-ng'] = {}), global.ng.common, global.ng.core));
}(this, (function (exports, common, i0) { 'use strict';

    var ImageOverlayComponent = /** @class */ (function () {
        function ImageOverlayComponent() {
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
        return ImageOverlayComponent;
    }());
    ImageOverlayComponent.decorators = [
        { type: i0.Component, args: [{
                    selector: 'ui-ng-image-overlay',
                    template: "<div\n\tclass=\"img-container\"\n\t[ngClass]=\"['img-' + imgAnimation, 'img-overlay-' + imgOverlayAnimation]\"\n>\n\t<img class=\"img-fluid img-item\" [src]=\"imgSrc\" [alt]=\"imgName\" />\n\t<div class=\"img-overlay\" [ngClass]=\"'img-overlay-' + imgOverlayBgColor\">\n\t\t<div class=\"img-overlay-content\">\n\t\t\t<ng-content></ng-content>\n\t\t</div>\n\t</div>\n</div>\n",
                    changeDetection: i0.ChangeDetectionStrategy.OnPush,
                    styles: [":host{display:block}.img-container{position:relative;z-index:1;display:inline-block;overflow:hidden}.img-container .img-item{transition:transform .25s ease-out;will-change:transform}.img-container .img-overlay{position:absolute;top:-.2rem;right:0;bottom:-.2rem;left:0;z-index:2;display:flex;align-items:center;justify-content:center;visibility:hidden;opacity:0;transition:all .3s ease-in;will-change:opacity,transform}.img-container .img-overlay-content{text-align:center}.img-container .img-overlay-black{background-color:rgba(0,0,0,.75)}.img-container .img-overlay-white{background-color:hsla(0,0%,100%,.9)}.img-container .img-overlay-primary-dark{background-color:rgba(62,74,89,.8)}.img-overlay-slide-top .img-overlay{transform:translateY(100%)}.img-overlay-slide-right .img-overlay{transform:translateX(-100%)}.img-overlay-slide-down .img-overlay{transform:translateY(-100%)}.img-overlay-slide-left .img-overlay{transform:translateX(100%)}.img-overlay-zoom-in .img-overlay{transform:scale(0)}.img-overlay-zoom-out .img-overlay{transform:scale(2)}.img-overlay-slide-top:hover .img-overlay{transform:translateY(0)}.img-overlay-slide-right:hover .img-overlay{transform:translateX(0)}.img-overlay-slide-down:hover .img-overlay{transform:translateY(0)}.img-overlay-slide-left:hover .img-overlay{transform:translateX(0)}.img-overlay-zoom-in:hover .img-overlay,.img-overlay-zoom-out:hover .img-overlay{transform:scale(1)}.img-zoom-in:hover .img-item{transform:scale(1.2)}.img-rotate-r:hover .img-item{transform:scale(1.4) rotate(8deg)}.img-rotate-l:hover .img-item{transform:scale(1.4) rotate(-8deg)}.img-container:hover .img-overlay{visibility:visible;opacity:1}.img-fluid{max-width:100%;height:auto}img{vertical-align:middle;border-style:none}"]
                },] }
    ];
    ImageOverlayComponent.propDecorators = {
        imgName: [{ type: i0.Input }],
        imgSrc: [{ type: i0.Input }],
        imgAnimation: [{ type: i0.Input }],
        imgOverlayAnimation: [{ type: i0.Input }],
        imgOverlayBgColor: [{ type: i0.Input }]
    };

    /**
     * Options for Right and Left containers in Layout
     */
    (function (LayoutPanelPosition) {
        LayoutPanelPosition["Default"] = "Default";
        LayoutPanelPosition["Fixed"] = "Fixed";
        LayoutPanelPosition["Hidden"] = "Hidden";
        LayoutPanelPosition["Hover"] = "Hover";
    })(exports.LayoutPanelPosition || (exports.LayoutPanelPosition = {}));

    /* istanbul ignore file */
    var WindowRef = /** @class */ (function () {
        function WindowRef() {
        }
        /**
         * Returns a reference to the window
         */
        WindowRef.prototype.getWindow = function () {
            return window;
        };
        return WindowRef;
    }());
    WindowRef.ɵprov = i0.ɵɵdefineInjectable({ factory: function WindowRef_Factory() { return new WindowRef(); }, token: WindowRef, providedIn: "root" });
    WindowRef.decorators = [
        { type: i0.Injectable, args: [{
                    providedIn: 'root',
                },] }
    ];

    var LayoutComponent = /** @class */ (function () {
        function LayoutComponent(changeDetectorRef, windowRef) {
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
        Object.defineProperty(LayoutComponent.prototype, "config", {
            /**
             * Gets component config
             */
            get: function () {
                return this._config;
            },
            /**
             * Sets component config
             * @param value - Component config
             */
            set: function (value) {
                this._config = Object.assign(Object.assign({}, LayoutComponent.DEFAULT_CONFIG), value);
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(LayoutComponent.prototype, "leftHoverMinimizedContent", {
            /**
             * Can minimize left container content when Hover collapsed
             */
            get: function () {
                return (this.config.leftContainerPosition === exports.LayoutPanelPosition.Hover &&
                    !!this.config.shiftLeftContentWithHoverMode &&
                    !this.leftContainerHovered &&
                    !(this.windowRef.getWindow().innerWidth <= 991.98));
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(LayoutComponent.prototype, "rightHoverMinimizedContent", {
            /**
             * Can minimize right container content when Hover collapsed
             */
            get: function () {
                return (this.config.rightContainerPosition === exports.LayoutPanelPosition.Hover &&
                    !!this.config.shiftRightContentWithHoverMode &&
                    !this.rightContainerHovered &&
                    !(this.windowRef.getWindow().innerWidth <= 991.98));
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(LayoutComponent.prototype, "layoutContainerClass", {
            /**
             * Layout component wrapper classNames
             */
            get: function () {
                return {
                    'switch-side-containers': !!(this.config.toggleLeftAndRightPosition &&
                        this.config.rightContainerPosition !==
                            exports.LayoutPanelPosition.Default &&
                        this.config.leftContainerPosition !==
                            exports.LayoutPanelPosition.Default) ||
                        !!(this.config.toggleLeftAndRightPosition &&
                            !this.leftTemplate &&
                            this.rightTemplate &&
                            this.config.rightContainerPosition !==
                                exports.LayoutPanelPosition.Default) ||
                        !!(this.config.toggleLeftAndRightPosition &&
                            !this.rightTemplate &&
                            this.leftTemplate &&
                            this.config.leftContainerPosition !==
                                exports.LayoutPanelPosition.Default),
                    'show-overlay': this.showOverlay,
                    'left-fixed': this.config.leftContainerPosition === exports.LayoutPanelPosition.Fixed,
                    'left-hidden': this.config.leftContainerPosition ===
                        exports.LayoutPanelPosition.Hidden,
                    'left-opened': this.config.leftContainerPosition ===
                        exports.LayoutPanelPosition.Hidden && this.isLeftOpened,
                    'left-opened-xs': this.isLeftXsOpened,
                    'left-hover': this.config.leftContainerPosition === exports.LayoutPanelPosition.Hover,
                    'left-shift-content': this.leftHoverMinimizedContent &&
                        (!this.config.toggleLeftAndRightPosition ||
                            (this.config.toggleLeftAndRightPosition &&
                                this.config.rightContainerPosition ===
                                    exports.LayoutPanelPosition.Default)),
                    'right-fixed': this.config.rightContainerPosition ===
                        exports.LayoutPanelPosition.Fixed,
                    'right-hidden': this.config.rightContainerPosition ===
                        exports.LayoutPanelPosition.Hidden,
                    'right-opened': this.config.rightContainerPosition ===
                        exports.LayoutPanelPosition.Hidden && this.isRightOpened,
                    'right-opened-xs': this.isRightXsOpened,
                    'right-hover': this.config.rightContainerPosition ===
                        exports.LayoutPanelPosition.Hover,
                    'right-shift-content': this.rightHoverMinimizedContent &&
                        (this.config.toggleLeftAndRightPosition ||
                            (!this.config.toggleLeftAndRightPosition &&
                                this.config.leftContainerPosition ===
                                    exports.LayoutPanelPosition.Default)),
                    'fixed-full-screen-on-small-resolution': !!this.config.fullWidthOnSmallScreen,
                };
            },
            enumerable: false,
            configurable: true
        });
        /**
         * Hide overlay
         */
        LayoutComponent.prototype.closeContainer = function () {
            this.showOverlay = false;
            this.isRightOpened = false;
            this.isLeftOpened = false;
            this.isRightXsOpened = false;
            this.isLeftXsOpened = false;
            this.changeDetectorRef.markForCheck();
        };
        /**
         * Show right container
         * @param withOverlay :boolean - show overlay
         */
        LayoutComponent.prototype.showRightContainer = function (withOverlay) {
            if (withOverlay === void 0) { withOverlay = false; }
            if (this.windowRef.getWindow().innerWidth <= 991.98 &&
                this.config.fullWidthOnSmallScreen &&
                this.config.rightContainerPosition !== exports.LayoutPanelPosition.Default) {
                this.showOverlay = withOverlay;
                this.isRightXsOpened = true;
                this.isRightOpened = true;
            }
            else if (this.config.rightContainerPosition === exports.LayoutPanelPosition.Hidden) {
                this.showOverlay = withOverlay;
                this.isRightOpened = true;
            }
            this.changeDetectorRef.markForCheck();
        };
        /**
         * Show left container
         * @param withOverlay :boolean - show overlay
         */
        LayoutComponent.prototype.showLeftContainer = function (withOverlay) {
            if (withOverlay === void 0) { withOverlay = false; }
            if (this.windowRef.getWindow().innerWidth <= 991.98 &&
                this.config.fullWidthOnSmallScreen &&
                this.config.leftContainerPosition !== exports.LayoutPanelPosition.Default) {
                this.showOverlay = withOverlay;
                this.isLeftXsOpened = true;
                this.isLeftOpened = true;
            }
            else if (this.config.leftContainerPosition === exports.LayoutPanelPosition.Hidden) {
                this.showOverlay = withOverlay;
                this.isLeftOpened = true;
            }
            this.changeDetectorRef.markForCheck();
        };
        /**
         * Left container mouse enter
         */
        LayoutComponent.prototype.leftContainerMouseEnter = function () {
            this.leftContainerHovered = true;
        };
        /**
         * Left Container mouse leave
         */
        LayoutComponent.prototype.leftContainerMouseLeave = function () {
            this.leftContainerHovered = false;
        };
        /**
         * Right container mouse enter
         */
        LayoutComponent.prototype.rightContainerMouseEnter = function () {
            this.rightContainerHovered = true;
        };
        /**
         * Right container mouse leave
         */
        LayoutComponent.prototype.rightContainerMouseLeave = function () {
            this.rightContainerHovered = false;
        };
        return LayoutComponent;
    }());
    LayoutComponent.DEFAULT_CONFIG = {
        leftContainerPosition: exports.LayoutPanelPosition.Default,
        rightContainerPosition: exports.LayoutPanelPosition.Default,
        toggleLeftAndRightPosition: false,
        fullWidthOnSmallScreen: false,
        shiftLeftContentWithHoverMode: false,
        shiftRightContentWithHoverMode: false,
    };
    LayoutComponent.decorators = [
        { type: i0.Component, args: [{
                    selector: 'ui-ng-layout',
                    template: "<div class=\"layout-page-wrap\" [ngClass]=\"layoutContainerClass\">\n\t<div\n\t\tclass=\"layout-left-container\"\n\t\t*ngIf=\"leftTemplate\"\n\t\t(mouseenter)=\"leftContainerMouseEnter()\"\n\t\t(mouseleave)=\"leftContainerMouseLeave()\"\n\t>\n\t\t<div class=\"layout-left-container-content\">\n\t\t\t<ng-container *ngTemplateOutlet=\"leftTemplate\"></ng-container>\n\t\t</div>\n\t</div>\n\t<div class=\"layout-central-container\">\n\t\t<ng-container *ngTemplateOutlet=\"centralContent\"></ng-container>\n\t</div>\n\t<div\n\t\tclass=\"layout-right-container\"\n\t\t*ngIf=\"rightTemplate\"\n\t\t(mouseenter)=\"rightContainerMouseEnter()\"\n\t\t(mouseleave)=\"rightContainerMouseLeave()\"\n\t>\n\t\t<div class=\"layout-right-container-content\">\n\t\t\t<ng-container *ngTemplateOutlet=\"rightTemplate\"></ng-container>\n\t\t</div>\n\t</div>\n\n\t<div\n\t\tclass=\"layout-page-overlay\"\n\t\t(click)=\"closeContainer()\"\n\t\t(keyup.enter)=\"closeContainer()\"\n\t></div>\n</div>\n\n<ng-template #centralContent>\n\t<div class=\"layout-top-container\" *ngIf=\"topTemplate\">\n\t\t<ng-container *ngTemplateOutlet=\"topTemplate\"></ng-container>\n\t</div>\n\t<div class=\"layout-main-container\">\n\t\t<ng-content></ng-content>\n\t</div>\n\t<div class=\"layout-bottom-container\" *ngIf=\"bottomTemplate\">\n\t\t<ng-container *ngTemplateOutlet=\"bottomTemplate\"></ng-container>\n\t</div>\n</ng-template>\n",
                    encapsulation: i0.ViewEncapsulation.None,
                    changeDetection: i0.ChangeDetectionStrategy.OnPush,
                    styles: [""]
                },] }
    ];
    LayoutComponent.ctorParameters = function () { return [
        { type: i0.ChangeDetectorRef },
        { type: WindowRef }
    ]; };
    LayoutComponent.propDecorators = {
        topTemplate: [{ type: i0.Input }],
        leftTemplate: [{ type: i0.Input }],
        rightTemplate: [{ type: i0.Input }],
        bottomTemplate: [{ type: i0.Input }],
        scrollBarOptions: [{ type: i0.Input }],
        config: [{ type: i0.Input }]
    };

    var NavMenuItemComponent = /** @class */ (function () {
        function NavMenuItemComponent() {
            /**
             * Sets top-level menu item id
             */
            this.setOpenId = new i0.EventEmitter();
            /**
             * Marks open menu items
             */
            this.isOpen = false;
        }
        /**
         * Updates menu item open state based on its id / owing of active item
         */
        NavMenuItemComponent.prototype.ngOnChanges = function (changes) {
            var _a;
            if (changes.item) {
                this.isOpen = ((_a = changes.item.currentValue.children) === null || _a === void 0 ? void 0 : _a.length) ? this.hasActiveChild(changes.item.currentValue)
                    : false;
            }
            else if (changes.openedId && this.id) {
                this.isOpen = this.id === changes.openedId.currentValue;
            }
        };
        /**
         * Toggles nav menu item state - open/close
         */
        NavMenuItemComponent.prototype.toggle = function () {
            this.isOpen = !this.isOpen;
            if (this.id) {
                this.setOpenId.emit(this.isOpen ? this.id : null);
            }
        };
        /**
         * Checks if among menu items children is an active one
         */
        NavMenuItemComponent.prototype.hasActiveChild = function (item) {
            var _this = this;
            var _a;
            if ((_a = item.children) === null || _a === void 0 ? void 0 : _a.length) {
                return item.children.some(function (child) { return _this.hasActiveChild(child); });
            }
            else {
                return !!item.isActive;
            }
        };
        return NavMenuItemComponent;
    }());
    NavMenuItemComponent.decorators = [
        { type: i0.Component, args: [{
                    selector: 'ui-ng-nav-menu-item',
                    template: "<ng-container *ngIf=\"!item.isHeading; else heading\">\n\t<ng-container *ngIf=\"item.children?.length; else finalItem\">\n\t\t<li class=\"nav-main-item\" [class.open]=\"isOpen || expandAll\">\n\t\t\t<button\n\t\t\t\tclass=\"btn btn-link nav-main-link nav-main-link-submenu\"\n\t\t\t\t(click)=\"toggle()\"\n\t\t\t>\n\t\t\t\t<ng-container *ngTemplateOutlet=\"content\"></ng-container>\n\t\t\t</button>\n\t\t\t<ul class=\"nav-main-submenu\" [hidden]=\"!isOpen && !expandAll\">\n\t\t\t\t<ng-container\n\t\t\t\t\t*ngFor=\"\n\t\t\t\t\t\tlet child of item.children\n\t\t\t\t\t\t\t| uiNgNavMenuFilter: filterValue\n\t\t\t\t\t\"\n\t\t\t\t>\n\t\t\t\t\t<ui-ng-nav-menu-item\n\t\t\t\t\t\t[item]=\"child\"\n\t\t\t\t\t\t[filterValue]=\"filterValue\"\n\t\t\t\t\t\t[expandAll]=\"expandAll\"\n\t\t\t\t\t></ui-ng-nav-menu-item>\n\t\t\t\t</ng-container>\n\t\t\t</ul>\n\t\t</li>\n\t</ng-container>\n\n\t<ng-template #finalItem>\n\t\t<li class=\"nav-main-item\">\n\t\t\t<a\n\t\t\t\tclass=\"btn btn-link nav-main-link\"\n\t\t\t\t[class.active]=\"item.isActive\"\n\t\t\t\t[href]=\"item.href\"\n\t\t\t>\n\t\t\t\t<ng-container *ngTemplateOutlet=\"content\"></ng-container>\n\t\t\t</a>\n\t\t</li>\n\t</ng-template>\n\n\t<ng-template #content>\n\t\t<i\n\t\t\t*ngIf=\"item.icon\"\n\t\t\tclass=\"nav-main-link-icon\"\n\t\t\t[ngClass]=\"item.icon\"\n\t\t></i>\n\t\t<span class=\"nav-main-link-name text-left\">{{ item.name }}</span>\n\t</ng-template>\n</ng-container>\n\n<ng-template #heading>\n\t<li class=\"nav-main-heading\">{{ item.name }}</li>\n</ng-template>\n",
                    styles: ["@charset \"UTF-8\";.nav-main-heading{padding:1.375rem 1.25rem .375rem;font-weight:600;font-size:.75rem;letter-spacing:.0625rem;text-transform:uppercase}.nav-main-item{display:flex;flex-direction:column}.nav-main-link{position:relative;display:flex;align-items:center;min-height:2.5rem;padding:.625rem 1.25rem;font-size:.875rem;line-height:1rem;text-decoration:none;border-radius:0;box-shadow:none;cursor:pointer}.nav-main-link .nav-main-link-icon{display:inline-block;flex:0 0 auto;width:1rem;margin-right:.625rem;text-align:center;transition:inherit}.nav-main-link .nav-main-link-name{display:inline-block;flex:1 1 auto;max-width:100%}.nav-main-link.nav-main-link-submenu{padding-right:2rem}.nav-main-link.nav-main-link-submenu:before{position:absolute;top:50%;right:.625rem;display:block;width:1rem;height:1rem;margin-top:-.5rem;font-weight:900;font-size:.75rem;font-family:Font Awesome\\ 5 Free,Font Awesome\\ 5 Pro,serif;line-height:1rem;text-align:center;opacity:.4;transition:opacity .25s ease-out,transform .25s ease-out;content:\"\uF104\"}.nav-main-submenu{height:0;padding-left:2.875rem;overflow:hidden;list-style:none}.nav-main-submenu .nav-main-item{transform:translateX(-.75rem);opacity:0;transition:opacity .25s ease-out,transform .25s ease-out}.nav-main-submenu .nav-main-heading{padding-top:1.25rem;padding-bottom:.25rem;padding-left:0}.nav-main-submenu .nav-main-link{min-height:2.125rem;margin:0;padding-top:.5rem;padding-bottom:.5rem;padding-left:0;font-size:.8125rem}.nav-main-submenu .nav-main-link .active,.nav-main-submenu .nav-main-link:focus,.nav-main-submenu .nav-main-link:hover{background-color:transparent}.nav-main-submenu .nav-main-submenu{padding-left:.75rem}.nav-main-submenu .nav-main-item.open .nav-main-link{background-color:transparent}.nav-main-item.open>.nav-main-link-submenu:before{transform:rotate(-90deg)}.nav-main-item.open>.nav-main-submenu{height:auto}.nav-main-item.open>.nav-main-submenu .nav-main-item{transform:translateX(0);opacity:1}"]
                },] }
    ];
    NavMenuItemComponent.propDecorators = {
        item: [{ type: i0.Input }],
        id: [{ type: i0.Input }],
        openedId: [{ type: i0.Input }],
        filterValue: [{ type: i0.Input }],
        expandAll: [{ type: i0.Input }],
        setOpenId: [{ type: i0.Output }]
    };

    /**
     * Main navigation component
     */
    var NavMenuComponent = /** @class */ (function () {
        function NavMenuComponent() {
            /**
             * Nav Munu Items
             */
            this.items = [];
            /**
             * Nav Menu Theme
             */
            this.theme = 'dark';
            /**
             * Defines whether custom or default scrollbar should be used
             */
            this.customScrollbar = true;
            /**
             * Nav Menu Mini Mode Enabled (if shown only)
             */
            this.miniMode = true;
            /**
             * Defines menu position - fixed or default
             */
            this.isFixed = false;
            /**
             * Define is collapse (mini mode) triggered on
             */
            this.isMiniOnMouseLeave = false;
            /**
             * Search text to filter nav item by name
             */
            this.filterValue = '';
            /**
             * Opened menu item index;
             */
            this.openedId = null;
            /**
             * Defines if all menu items are expanded at a time (if filter is applied)
             */
            this.expandAll = false;
            /**
             * Defines if nav menu should be shown
             */
            this.isShown = true;
        }
        /**
         * Resets openedId if items change
         */
        NavMenuComponent.prototype.ngOnChanges = function (changes) {
            if (changes.items) {
                this.openedId = null;
            }
            if (changes.filterValue) {
                this.expandAll = !!changes.filterValue.currentValue.trim();
            }
        };
        /**
         * Shows nav menu
         */
        NavMenuComponent.prototype.show = function () {
            this.isShown = true;
        };
        /**
         * Hides nav menu
         */
        NavMenuComponent.prototype.hide = function () {
            this.isShown = false;
        };
        /**
         * Toggles nav menu appearance
         */
        NavMenuComponent.prototype.toggle = function () {
            this.isShown = !this.isShown;
        };
        /**
         * Sets index of opened menu item
         */
        NavMenuComponent.prototype.setOpenedId = function (index) {
            this.openedId = index;
        };
        return NavMenuComponent;
    }());
    NavMenuComponent.decorators = [
        { type: i0.Component, args: [{
                    selector: 'ui-ng-nav-menu',
                    template: "<nav\n\tclass=\"sidebar sidebar-{{ theme }} custom-scrollbar-{{\n\t\tcustomScrollbar ? theme : ''\n\t}}\"\n\t[ngClass]=\"{\n\t\t'sidebar-mini': miniMode,\n\t\tshow: isShown,\n\t\t'sidebar-fixed': isFixed,\n\t\t'sidebar-collapsed': isMiniOnMouseLeave\n\t}\"\n>\n\t<div class=\"nav-menu-header\">\n\t\t<ng-content></ng-content>\n\t</div>\n\t<div class=\"nav-menu-content\">\n\t\t<ul class=\"nav-main\">\n\t\t\t<ng-container\n\t\t\t\t*ngFor=\"\n\t\t\t\t\tlet item of items | uiNgNavMenuFilter: filterValue;\n\t\t\t\t\tlet i = index\n\t\t\t\t\"\n\t\t\t>\n\t\t\t\t<ui-ng-nav-menu-item\n\t\t\t\t\t[item]=\"item\"\n\t\t\t\t\t[id]=\"i + 1\"\n\t\t\t\t\t[openedId]=\"openedId\"\n\t\t\t\t\t[filterValue]=\"filterValue\"\n\t\t\t\t\t[expandAll]=\"expandAll\"\n\t\t\t\t\t(setOpenId)=\"setOpenedId($event)\"\n\t\t\t\t></ui-ng-nav-menu-item>\n\t\t\t</ng-container>\n\t\t</ul>\n\t</div>\n</nav>\n",
                    changeDetection: i0.ChangeDetectionStrategy.OnPush,
                    styles: [":host{width:100%;height:100%}.sidebar{width:0;height:100%;overflow-y:auto;border:none;box-shadow:none;transition:width .28s ease-out;will-change:width;-ms-scroll-chaining:none;overscroll-behavior:contain}.sidebar.sidebar-fixed{position:fixed;top:0;bottom:0;left:0;z-index:1032}.nav-menu-content,.nav-menu-header{width:100%;margin:0 auto}.nav-menu-content{padding:0 1.25rem 1.25rem;overflow-x:hidden}.nav-main{margin:0 -1.25rem;padding:0;list-style:none}.sidebar-dark{color:#ebebeb;background-color:#002b49}.sidebar-dark ::ng-deep .nav-main-heading{color:hsla(0,0%,100%,.4)}.sidebar-dark ::ng-deep .nav-main-link{color:hsla(0,0%,100%,.5)}.sidebar-dark ::ng-deep .nav-main-link .nav-main-link-icon{color:hsla(0,0%,100%,.2)}.sidebar-dark ::ng-deep .nav-main-item.open>.nav-main-link-submenu,.sidebar-dark ::ng-deep .nav-main-link.active{color:#fff}.sidebar-light{color:#575757;background-color:#fff}.sidebar-light ::ng-deep .nav-main-heading{color:#979797}.sidebar-light ::ng-deep .nav-main-link{color:#575757}.sidebar-light ::ng-deep .nav-main-link .nav-main-link-icon{color:#b0b0b0}.sidebar-light ::ng-deep .nav-main-item.open>.nav-main-link-submenu,.sidebar-light ::ng-deep .nav-main-link.active{color:#000}.sidebar-dark ::ng-deep .nav-main-submenu{background-color:rgba(0,0,0,.15)}.sidebar-dark ::ng-deep .nav-main-submenu .nav-main-link{color:hsla(0,0%,100%,.4)}.sidebar-dark ::ng-deep .nav-main-submenu .nav-main-link.active{color:#fff}.sidebar-light ::ng-deep .nav-main-submenu{background-color:rgba(0,0,0,.02)}.sidebar-light ::ng-deep .nav-main-submenu .nav-main-link{color:#717171}.sidebar-light ::ng-deep .nav-main-submenu .nav-main-link.active{color:#000}.sidebar-dark ::ng-deep .nav-main-link.active .nav-main-link-icon{color:#fff}.sidebar-light ::ng-deep .nav-main-link.active .nav-main-link-icon{color:#000}.sidebar-dark ::ng-deep .nav-main-link:focus,.sidebar-dark ::ng-deep .nav-main-link:hover{color:#fff;background-color:rgba(0,0,0,.2)}.sidebar-dark ::ng-deep .nav-main-link:focus>.nav-main-link-icon,.sidebar-dark ::ng-deep .nav-main-link:hover>.nav-main-link-icon{color:#fff}.sidebar-light ::ng-deep .nav-main-link:focus,.sidebar-light ::ng-deep .nav-main-link:hover{color:#575757;background-color:#f9f9f9}.sidebar-light ::ng-deep .nav-main-link:focus>.nav-main-link-icon,.sidebar-light ::ng-deep .nav-main-link:hover>.nav-main-link-icon{color:#000}.sidebar-dark ::ng-deep .nav-main-submenu .nav-main-link:focus,.sidebar-dark ::ng-deep .nav-main-submenu .nav-main-link:hover{color:#fff}.sidebar-light ::ng-deep .nav-main-submenu .nav-main-link:focus,.sidebar-light ::ng-deep .nav-main-submenu .nav-main-link:hover{color:#000}.sidebar-dark ::ng-deep .nav-main-item.open>.nav-main-link-submenu .nav-main-link-icon{color:#fff}.sidebar-light ::ng-deep .nav-main-item.open>.nav-main-link-submenu .nav-main-link-icon{color:#000}.sidebar.show.sidebar-collapsed ::ng-deep .nav-main .nav-main-item>.nav-main-submenu,.sidebar.show.sidebar-collapsed ::ng-deep .remove-in-mini{display:none}.sidebar.show.sidebar-collapsed ::ng-deep .hide-in-mini,.sidebar.show.sidebar-collapsed ::ng-deep .nav-main .nav-main-heading,.sidebar.show.sidebar-collapsed ::ng-deep .nav-main .nav-main-link-name{opacity:0}@media (max-width:991.98px){.sidebar.show{width:100%}}@media (min-width:992px){.sidebar.show{width:14.375rem}.sidebar.show.sidebar-mini{width:3.75rem;overflow-x:hidden;transition:width .28s ease-out}.sidebar.show.sidebar-mini .nav-menu-content,.sidebar.show.sidebar-mini .nav-menu-header{width:14.375rem;transition:width .28s ease-out;will-change:width}.sidebar.show.sidebar-mini ::ng-deep .nav-main .nav-main-heading,.sidebar.show.sidebar-mini ::ng-deep .nav-main .nav-main-link-name{transition:opacity .28s ease-out}.sidebar.show.sidebar-mini:not(:hover) ::ng-deep .nav-main .nav-main-item>.nav-main-submenu,.sidebar.show.sidebar-mini:not(:hover) ::ng-deep .remove-in-mini{display:none}.sidebar.show.sidebar-mini:not(:hover) ::ng-deep .hide-in-mini,.sidebar.show.sidebar-mini:not(:hover) ::ng-deep .nav-main .nav-main-heading,.sidebar.show.sidebar-mini:not(:hover) ::ng-deep .nav-main .nav-main-link-name{opacity:0}.sidebar.show.sidebar-mini:hover{width:14.375rem}.sidebar.show.sidebar-mini:hover .nav-menu-content,.sidebar.show.sidebar-mini:hover .nav-menu-header{width:100%}}"]
                },] }
    ];
    NavMenuComponent.propDecorators = {
        items: [{ type: i0.Input }],
        theme: [{ type: i0.Input }],
        customScrollbar: [{ type: i0.Input }],
        miniMode: [{ type: i0.Input }],
        isFixed: [{ type: i0.Input }],
        isMiniOnMouseLeave: [{ type: i0.Input }],
        filterValue: [{ type: i0.Input }]
    };

    var NavMenuFilterPipe = /** @class */ (function () {
        function NavMenuFilterPipe() {
        }
        /**
         * Transform function returns array of items matching search term
         * @param value - menu item to be filtered
         * @param term - search term
         */
        NavMenuFilterPipe.prototype.transform = function (value, term) {
            var _this = this;
            if (value === void 0) { value = []; }
            if (term === void 0) { term = ''; }
            var searchText = term.trim();
            if (!searchText) {
                return value;
            }
            return value.filter(function (item) { return _this.containsFilterValue(item, searchText); });
        };
        NavMenuFilterPipe.prototype.containsFilterValue = function (menuItem, filterValue) {
            var _this = this;
            var _a;
            return !((_a = menuItem.children) === null || _a === void 0 ? void 0 : _a.length)
                ? menuItem.name.toLowerCase().includes(filterValue.toLowerCase())
                : menuItem.children.some(function (child) { return _this.containsFilterValue(child, filterValue); });
        };
        return NavMenuFilterPipe;
    }());
    NavMenuFilterPipe.decorators = [
        { type: i0.Pipe, args: [{
                    name: 'uiNgNavMenuFilter',
                },] }
    ];

    var UiNgModule = /** @class */ (function () {
        function UiNgModule() {
        }
        return UiNgModule;
    }());
    UiNgModule.decorators = [
        { type: i0.NgModule, args: [{
                    declarations: [
                        ImageOverlayComponent,
                        LayoutComponent,
                        NavMenuComponent,
                        NavMenuItemComponent,
                        NavMenuFilterPipe,
                    ],
                    imports: [common.CommonModule],
                    exports: [ImageOverlayComponent, LayoutComponent, NavMenuComponent],
                },] }
    ];

    /**
     * Generated bundle index. Do not edit.
     */

    exports.ImageOverlayComponent = ImageOverlayComponent;
    exports.LayoutComponent = LayoutComponent;
    exports.NavMenuComponent = NavMenuComponent;
    exports.UiNgModule = UiNgModule;
    exports.ɵa = WindowRef;
    exports.ɵb = NavMenuItemComponent;
    exports.ɵc = NavMenuFilterPipe;

    Object.defineProperty(exports, '__esModule', { value: true });

})));
//# sourceMappingURL=jabil-ui-ng.umd.js.map
