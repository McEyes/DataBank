import { ChangeDetectionStrategy, Component, Input, } from '@angular/core';
/**
 * Main navigation component
 */
export class NavMenuComponent {
    constructor() {
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
    ngOnChanges(changes) {
        if (changes.items) {
            this.openedId = null;
        }
        if (changes.filterValue) {
            this.expandAll = !!changes.filterValue.currentValue.trim();
        }
    }
    /**
     * Shows nav menu
     */
    show() {
        this.isShown = true;
    }
    /**
     * Hides nav menu
     */
    hide() {
        this.isShown = false;
    }
    /**
     * Toggles nav menu appearance
     */
    toggle() {
        this.isShown = !this.isShown;
    }
    /**
     * Sets index of opened menu item
     */
    setOpenedId(index) {
        this.openedId = index;
    }
}
NavMenuComponent.decorators = [
    { type: Component, args: [{
                selector: 'ui-ng-nav-menu',
                template: "<nav\n\tclass=\"sidebar sidebar-{{ theme }} custom-scrollbar-{{\n\t\tcustomScrollbar ? theme : ''\n\t}}\"\n\t[ngClass]=\"{\n\t\t'sidebar-mini': miniMode,\n\t\tshow: isShown,\n\t\t'sidebar-fixed': isFixed,\n\t\t'sidebar-collapsed': isMiniOnMouseLeave\n\t}\"\n>\n\t<div class=\"nav-menu-header\">\n\t\t<ng-content></ng-content>\n\t</div>\n\t<div class=\"nav-menu-content\">\n\t\t<ul class=\"nav-main\">\n\t\t\t<ng-container\n\t\t\t\t*ngFor=\"\n\t\t\t\t\tlet item of items | uiNgNavMenuFilter: filterValue;\n\t\t\t\t\tlet i = index\n\t\t\t\t\"\n\t\t\t>\n\t\t\t\t<ui-ng-nav-menu-item\n\t\t\t\t\t[item]=\"item\"\n\t\t\t\t\t[id]=\"i + 1\"\n\t\t\t\t\t[openedId]=\"openedId\"\n\t\t\t\t\t[filterValue]=\"filterValue\"\n\t\t\t\t\t[expandAll]=\"expandAll\"\n\t\t\t\t\t(setOpenId)=\"setOpenedId($event)\"\n\t\t\t\t></ui-ng-nav-menu-item>\n\t\t\t</ng-container>\n\t\t</ul>\n\t</div>\n</nav>\n",
                changeDetection: ChangeDetectionStrategy.OnPush,
                styles: [":host{width:100%;height:100%}.sidebar{width:0;height:100%;overflow-y:auto;border:none;box-shadow:none;transition:width .28s ease-out;will-change:width;-ms-scroll-chaining:none;overscroll-behavior:contain}.sidebar.sidebar-fixed{position:fixed;top:0;bottom:0;left:0;z-index:1032}.nav-menu-content,.nav-menu-header{width:100%;margin:0 auto}.nav-menu-content{padding:0 1.25rem 1.25rem;overflow-x:hidden}.nav-main{margin:0 -1.25rem;padding:0;list-style:none}.sidebar-dark{color:#ebebeb;background-color:#002b49}.sidebar-dark ::ng-deep .nav-main-heading{color:hsla(0,0%,100%,.4)}.sidebar-dark ::ng-deep .nav-main-link{color:hsla(0,0%,100%,.5)}.sidebar-dark ::ng-deep .nav-main-link .nav-main-link-icon{color:hsla(0,0%,100%,.2)}.sidebar-dark ::ng-deep .nav-main-item.open>.nav-main-link-submenu,.sidebar-dark ::ng-deep .nav-main-link.active{color:#fff}.sidebar-light{color:#575757;background-color:#fff}.sidebar-light ::ng-deep .nav-main-heading{color:#979797}.sidebar-light ::ng-deep .nav-main-link{color:#575757}.sidebar-light ::ng-deep .nav-main-link .nav-main-link-icon{color:#b0b0b0}.sidebar-light ::ng-deep .nav-main-item.open>.nav-main-link-submenu,.sidebar-light ::ng-deep .nav-main-link.active{color:#000}.sidebar-dark ::ng-deep .nav-main-submenu{background-color:rgba(0,0,0,.15)}.sidebar-dark ::ng-deep .nav-main-submenu .nav-main-link{color:hsla(0,0%,100%,.4)}.sidebar-dark ::ng-deep .nav-main-submenu .nav-main-link.active{color:#fff}.sidebar-light ::ng-deep .nav-main-submenu{background-color:rgba(0,0,0,.02)}.sidebar-light ::ng-deep .nav-main-submenu .nav-main-link{color:#717171}.sidebar-light ::ng-deep .nav-main-submenu .nav-main-link.active{color:#000}.sidebar-dark ::ng-deep .nav-main-link.active .nav-main-link-icon{color:#fff}.sidebar-light ::ng-deep .nav-main-link.active .nav-main-link-icon{color:#000}.sidebar-dark ::ng-deep .nav-main-link:focus,.sidebar-dark ::ng-deep .nav-main-link:hover{color:#fff;background-color:rgba(0,0,0,.2)}.sidebar-dark ::ng-deep .nav-main-link:focus>.nav-main-link-icon,.sidebar-dark ::ng-deep .nav-main-link:hover>.nav-main-link-icon{color:#fff}.sidebar-light ::ng-deep .nav-main-link:focus,.sidebar-light ::ng-deep .nav-main-link:hover{color:#575757;background-color:#f9f9f9}.sidebar-light ::ng-deep .nav-main-link:focus>.nav-main-link-icon,.sidebar-light ::ng-deep .nav-main-link:hover>.nav-main-link-icon{color:#000}.sidebar-dark ::ng-deep .nav-main-submenu .nav-main-link:focus,.sidebar-dark ::ng-deep .nav-main-submenu .nav-main-link:hover{color:#fff}.sidebar-light ::ng-deep .nav-main-submenu .nav-main-link:focus,.sidebar-light ::ng-deep .nav-main-submenu .nav-main-link:hover{color:#000}.sidebar-dark ::ng-deep .nav-main-item.open>.nav-main-link-submenu .nav-main-link-icon{color:#fff}.sidebar-light ::ng-deep .nav-main-item.open>.nav-main-link-submenu .nav-main-link-icon{color:#000}.sidebar.show.sidebar-collapsed ::ng-deep .nav-main .nav-main-item>.nav-main-submenu,.sidebar.show.sidebar-collapsed ::ng-deep .remove-in-mini{display:none}.sidebar.show.sidebar-collapsed ::ng-deep .hide-in-mini,.sidebar.show.sidebar-collapsed ::ng-deep .nav-main .nav-main-heading,.sidebar.show.sidebar-collapsed ::ng-deep .nav-main .nav-main-link-name{opacity:0}@media (max-width:991.98px){.sidebar.show{width:100%}}@media (min-width:992px){.sidebar.show{width:14.375rem}.sidebar.show.sidebar-mini{width:3.75rem;overflow-x:hidden;transition:width .28s ease-out}.sidebar.show.sidebar-mini .nav-menu-content,.sidebar.show.sidebar-mini .nav-menu-header{width:14.375rem;transition:width .28s ease-out;will-change:width}.sidebar.show.sidebar-mini ::ng-deep .nav-main .nav-main-heading,.sidebar.show.sidebar-mini ::ng-deep .nav-main .nav-main-link-name{transition:opacity .28s ease-out}.sidebar.show.sidebar-mini:not(:hover) ::ng-deep .nav-main .nav-main-item>.nav-main-submenu,.sidebar.show.sidebar-mini:not(:hover) ::ng-deep .remove-in-mini{display:none}.sidebar.show.sidebar-mini:not(:hover) ::ng-deep .hide-in-mini,.sidebar.show.sidebar-mini:not(:hover) ::ng-deep .nav-main .nav-main-heading,.sidebar.show.sidebar-mini:not(:hover) ::ng-deep .nav-main .nav-main-link-name{opacity:0}.sidebar.show.sidebar-mini:hover{width:14.375rem}.sidebar.show.sidebar-mini:hover .nav-menu-content,.sidebar.show.sidebar-mini:hover .nav-menu-header{width:100%}}"]
            },] }
];
NavMenuComponent.propDecorators = {
    items: [{ type: Input }],
    theme: [{ type: Input }],
    customScrollbar: [{ type: Input }],
    miniMode: [{ type: Input }],
    isFixed: [{ type: Input }],
    isMiniOnMouseLeave: [{ type: Input }],
    filterValue: [{ type: Input }]
};
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoibmF2LW1lbnUuY29tcG9uZW50LmpzIiwic291cmNlUm9vdCI6IiIsInNvdXJjZXMiOlsiLi4vLi4vLi4vLi4vLi4vLi4vLi4vLi4vc3JjL2FwcC9saWIvbmF2LW1lbnUvY29tcG9uZW50cy9uYXYtbWVudS9uYXYtbWVudS5jb21wb25lbnQudHMiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IkFBQUEsT0FBTyxFQUNOLHVCQUF1QixFQUN2QixTQUFTLEVBQ1QsS0FBSyxHQUdMLE1BQU0sZUFBZSxDQUFDO0FBTXZCOztHQUVHO0FBT0gsTUFBTSxPQUFPLGdCQUFnQjtJQU43QjtRQU9DOztXQUVHO1FBRUgsVUFBSyxHQUFjLEVBQUUsQ0FBQztRQUV0Qjs7V0FFRztRQUVILFVBQUssR0FBaUIsTUFBTSxDQUFDO1FBRTdCOztXQUVHO1FBRUgsb0JBQWUsR0FBRyxJQUFJLENBQUM7UUFFdkI7O1dBRUc7UUFFSCxhQUFRLEdBQUcsSUFBSSxDQUFDO1FBRWhCOztXQUVHO1FBRUgsWUFBTyxHQUFHLEtBQUssQ0FBQztRQUNoQjs7V0FFRztRQUVILHVCQUFrQixHQUFHLEtBQUssQ0FBQztRQUUzQjs7V0FFRztRQUVILGdCQUFXLEdBQUcsRUFBRSxDQUFDO1FBRWpCOztXQUVHO1FBQ0gsYUFBUSxHQUFrQixJQUFJLENBQUM7UUFDL0I7O1dBRUc7UUFDSCxjQUFTLEdBQUcsS0FBSyxDQUFDO1FBRWxCOztXQUVHO1FBQ0gsWUFBTyxHQUFHLElBQUksQ0FBQztJQTBDaEIsQ0FBQztJQXhDQTs7T0FFRztJQUNILFdBQVcsQ0FBQyxPQUFzQjtRQUNqQyxJQUFJLE9BQU8sQ0FBQyxLQUFLLEVBQUU7WUFDbEIsSUFBSSxDQUFDLFFBQVEsR0FBRyxJQUFJLENBQUM7U0FDckI7UUFFRCxJQUFJLE9BQU8sQ0FBQyxXQUFXLEVBQUU7WUFDeEIsSUFBSSxDQUFDLFNBQVMsR0FBRyxDQUFDLENBQUMsT0FBTyxDQUFDLFdBQVcsQ0FBQyxZQUFZLENBQUMsSUFBSSxFQUFFLENBQUM7U0FDM0Q7SUFDRixDQUFDO0lBRUQ7O09BRUc7SUFDSCxJQUFJO1FBQ0gsSUFBSSxDQUFDLE9BQU8sR0FBRyxJQUFJLENBQUM7SUFDckIsQ0FBQztJQUVEOztPQUVHO0lBQ0gsSUFBSTtRQUNILElBQUksQ0FBQyxPQUFPLEdBQUcsS0FBSyxDQUFDO0lBQ3RCLENBQUM7SUFFRDs7T0FFRztJQUNILE1BQU07UUFDTCxJQUFJLENBQUMsT0FBTyxHQUFHLENBQUMsSUFBSSxDQUFDLE9BQU8sQ0FBQztJQUM5QixDQUFDO0lBRUQ7O09BRUc7SUFDSCxXQUFXLENBQUMsS0FBb0I7UUFDL0IsSUFBSSxDQUFDLFFBQVEsR0FBRyxLQUFLLENBQUM7SUFDdkIsQ0FBQzs7O1lBckdELFNBQVMsU0FBQztnQkFDVixRQUFRLEVBQUUsZ0JBQWdCO2dCQUMxQiwyM0JBQXdDO2dCQUV4QyxlQUFlLEVBQUUsdUJBQXVCLENBQUMsTUFBTTs7YUFDL0M7OztvQkFLQyxLQUFLO29CQU1MLEtBQUs7OEJBTUwsS0FBSzt1QkFNTCxLQUFLO3NCQU1MLEtBQUs7aUNBS0wsS0FBSzswQkFNTCxLQUFLIiwic291cmNlc0NvbnRlbnQiOlsiaW1wb3J0IHtcblx0Q2hhbmdlRGV0ZWN0aW9uU3RyYXRlZ3ksXG5cdENvbXBvbmVudCxcblx0SW5wdXQsXG5cdE9uQ2hhbmdlcyxcblx0U2ltcGxlQ2hhbmdlcyxcbn0gZnJvbSAnQGFuZ3VsYXIvY29yZSc7XG5cbmltcG9ydCB7IE5hdkl0ZW0gfSBmcm9tICcuLi8uLi9tb2RlbHMnO1xuXG50eXBlIE5hdk1lbnVUaGVtZSA9ICdkYXJrJyB8ICdsaWdodCc7XG5cbi8qKlxuICogTWFpbiBuYXZpZ2F0aW9uIGNvbXBvbmVudFxuICovXG5AQ29tcG9uZW50KHtcblx0c2VsZWN0b3I6ICd1aS1uZy1uYXYtbWVudScsXG5cdHRlbXBsYXRlVXJsOiAnLi9uYXYtbWVudS5jb21wb25lbnQuaHRtbCcsXG5cdHN0eWxlVXJsczogWycuL25hdi1tZW51LmNvbXBvbmVudC5zY3NzJ10sXG5cdGNoYW5nZURldGVjdGlvbjogQ2hhbmdlRGV0ZWN0aW9uU3RyYXRlZ3kuT25QdXNoLFxufSlcbmV4cG9ydCBjbGFzcyBOYXZNZW51Q29tcG9uZW50IGltcGxlbWVudHMgT25DaGFuZ2VzIHtcblx0LyoqXG5cdCAqIE5hdiBNdW51IEl0ZW1zXG5cdCAqL1xuXHRASW5wdXQoKVxuXHRpdGVtczogTmF2SXRlbVtdID0gW107XG5cblx0LyoqXG5cdCAqIE5hdiBNZW51IFRoZW1lXG5cdCAqL1xuXHRASW5wdXQoKVxuXHR0aGVtZTogTmF2TWVudVRoZW1lID0gJ2RhcmsnO1xuXG5cdC8qKlxuXHQgKiBEZWZpbmVzIHdoZXRoZXIgY3VzdG9tIG9yIGRlZmF1bHQgc2Nyb2xsYmFyIHNob3VsZCBiZSB1c2VkXG5cdCAqL1xuXHRASW5wdXQoKVxuXHRjdXN0b21TY3JvbGxiYXIgPSB0cnVlO1xuXG5cdC8qKlxuXHQgKiBOYXYgTWVudSBNaW5pIE1vZGUgRW5hYmxlZCAoaWYgc2hvd24gb25seSlcblx0ICovXG5cdEBJbnB1dCgpXG5cdG1pbmlNb2RlID0gdHJ1ZTtcblxuXHQvKipcblx0ICogRGVmaW5lcyBtZW51IHBvc2l0aW9uIC0gZml4ZWQgb3IgZGVmYXVsdFxuXHQgKi9cblx0QElucHV0KClcblx0aXNGaXhlZCA9IGZhbHNlO1xuXHQvKipcblx0ICogRGVmaW5lIGlzIGNvbGxhcHNlIChtaW5pIG1vZGUpIHRyaWdnZXJlZCBvblxuXHQgKi9cblx0QElucHV0KClcblx0aXNNaW5pT25Nb3VzZUxlYXZlID0gZmFsc2U7XG5cblx0LyoqXG5cdCAqIFNlYXJjaCB0ZXh0IHRvIGZpbHRlciBuYXYgaXRlbSBieSBuYW1lXG5cdCAqL1xuXHRASW5wdXQoKVxuXHRmaWx0ZXJWYWx1ZSA9ICcnO1xuXG5cdC8qKlxuXHQgKiBPcGVuZWQgbWVudSBpdGVtIGluZGV4O1xuXHQgKi9cblx0b3BlbmVkSWQ6IG51bWJlciB8IG51bGwgPSBudWxsO1xuXHQvKipcblx0ICogRGVmaW5lcyBpZiBhbGwgbWVudSBpdGVtcyBhcmUgZXhwYW5kZWQgYXQgYSB0aW1lIChpZiBmaWx0ZXIgaXMgYXBwbGllZClcblx0ICovXG5cdGV4cGFuZEFsbCA9IGZhbHNlO1xuXG5cdC8qKlxuXHQgKiBEZWZpbmVzIGlmIG5hdiBtZW51IHNob3VsZCBiZSBzaG93blxuXHQgKi9cblx0aXNTaG93biA9IHRydWU7XG5cblx0LyoqXG5cdCAqIFJlc2V0cyBvcGVuZWRJZCBpZiBpdGVtcyBjaGFuZ2Vcblx0ICovXG5cdG5nT25DaGFuZ2VzKGNoYW5nZXM6IFNpbXBsZUNoYW5nZXMpIHtcblx0XHRpZiAoY2hhbmdlcy5pdGVtcykge1xuXHRcdFx0dGhpcy5vcGVuZWRJZCA9IG51bGw7XG5cdFx0fVxuXG5cdFx0aWYgKGNoYW5nZXMuZmlsdGVyVmFsdWUpIHtcblx0XHRcdHRoaXMuZXhwYW5kQWxsID0gISFjaGFuZ2VzLmZpbHRlclZhbHVlLmN1cnJlbnRWYWx1ZS50cmltKCk7XG5cdFx0fVxuXHR9XG5cblx0LyoqXG5cdCAqIFNob3dzIG5hdiBtZW51XG5cdCAqL1xuXHRzaG93KCk6IHZvaWQge1xuXHRcdHRoaXMuaXNTaG93biA9IHRydWU7XG5cdH1cblxuXHQvKipcblx0ICogSGlkZXMgbmF2IG1lbnVcblx0ICovXG5cdGhpZGUoKTogdm9pZCB7XG5cdFx0dGhpcy5pc1Nob3duID0gZmFsc2U7XG5cdH1cblxuXHQvKipcblx0ICogVG9nZ2xlcyBuYXYgbWVudSBhcHBlYXJhbmNlXG5cdCAqL1xuXHR0b2dnbGUoKTogdm9pZCB7XG5cdFx0dGhpcy5pc1Nob3duID0gIXRoaXMuaXNTaG93bjtcblx0fVxuXG5cdC8qKlxuXHQgKiBTZXRzIGluZGV4IG9mIG9wZW5lZCBtZW51IGl0ZW1cblx0ICovXG5cdHNldE9wZW5lZElkKGluZGV4OiBudW1iZXIgfCBudWxsKTogdm9pZCB7XG5cdFx0dGhpcy5vcGVuZWRJZCA9IGluZGV4O1xuXHR9XG59XG4iXX0=