import { Component, EventEmitter, Input, Output, } from '@angular/core';
export class NavMenuItemComponent {
    constructor() {
        /**
         * Sets top-level menu item id
         */
        this.setOpenId = new EventEmitter();
        /**
         * Marks open menu items
         */
        this.isOpen = false;
    }
    /**
     * Updates menu item open state based on its id / owing of active item
     */
    ngOnChanges(changes) {
        var _a;
        if (changes.item) {
            this.isOpen = ((_a = changes.item.currentValue.children) === null || _a === void 0 ? void 0 : _a.length) ? this.hasActiveChild(changes.item.currentValue)
                : false;
        }
        else if (changes.openedId && this.id) {
            this.isOpen = this.id === changes.openedId.currentValue;
        }
    }
    /**
     * Toggles nav menu item state - open/close
     */
    toggle() {
        this.isOpen = !this.isOpen;
        if (this.id) {
            this.setOpenId.emit(this.isOpen ? this.id : null);
        }
    }
    /**
     * Checks if among menu items children is an active one
     */
    hasActiveChild(item) {
        var _a;
        if ((_a = item.children) === null || _a === void 0 ? void 0 : _a.length) {
            return item.children.some((child) => this.hasActiveChild(child));
        }
        else {
            return !!item.isActive;
        }
    }
}
NavMenuItemComponent.decorators = [
    { type: Component, args: [{
                selector: 'ui-ng-nav-menu-item',
                template: "<ng-container *ngIf=\"!item.isHeading; else heading\">\n\t<ng-container *ngIf=\"item.children?.length; else finalItem\">\n\t\t<li class=\"nav-main-item\" [class.open]=\"isOpen || expandAll\">\n\t\t\t<button\n\t\t\t\tclass=\"btn btn-link nav-main-link nav-main-link-submenu\"\n\t\t\t\t(click)=\"toggle()\"\n\t\t\t>\n\t\t\t\t<ng-container *ngTemplateOutlet=\"content\"></ng-container>\n\t\t\t</button>\n\t\t\t<ul class=\"nav-main-submenu\" [hidden]=\"!isOpen && !expandAll\">\n\t\t\t\t<ng-container\n\t\t\t\t\t*ngFor=\"\n\t\t\t\t\t\tlet child of item.children\n\t\t\t\t\t\t\t| uiNgNavMenuFilter: filterValue\n\t\t\t\t\t\"\n\t\t\t\t>\n\t\t\t\t\t<ui-ng-nav-menu-item\n\t\t\t\t\t\t[item]=\"child\"\n\t\t\t\t\t\t[filterValue]=\"filterValue\"\n\t\t\t\t\t\t[expandAll]=\"expandAll\"\n\t\t\t\t\t></ui-ng-nav-menu-item>\n\t\t\t\t</ng-container>\n\t\t\t</ul>\n\t\t</li>\n\t</ng-container>\n\n\t<ng-template #finalItem>\n\t\t<li class=\"nav-main-item\">\n\t\t\t<a\n\t\t\t\tclass=\"btn btn-link nav-main-link\"\n\t\t\t\t[class.active]=\"item.isActive\"\n\t\t\t\t[href]=\"item.href\"\n\t\t\t>\n\t\t\t\t<ng-container *ngTemplateOutlet=\"content\"></ng-container>\n\t\t\t</a>\n\t\t</li>\n\t</ng-template>\n\n\t<ng-template #content>\n\t\t<i\n\t\t\t*ngIf=\"item.icon\"\n\t\t\tclass=\"nav-main-link-icon\"\n\t\t\t[ngClass]=\"item.icon\"\n\t\t></i>\n\t\t<span class=\"nav-main-link-name text-left\">{{ item.name }}</span>\n\t</ng-template>\n</ng-container>\n\n<ng-template #heading>\n\t<li class=\"nav-main-heading\">{{ item.name }}</li>\n</ng-template>\n",
                styles: ["@charset \"UTF-8\";.nav-main-heading{padding:1.375rem 1.25rem .375rem;font-weight:600;font-size:.75rem;letter-spacing:.0625rem;text-transform:uppercase}.nav-main-item{display:flex;flex-direction:column}.nav-main-link{position:relative;display:flex;align-items:center;min-height:2.5rem;padding:.625rem 1.25rem;font-size:.875rem;line-height:1rem;text-decoration:none;border-radius:0;box-shadow:none;cursor:pointer}.nav-main-link .nav-main-link-icon{display:inline-block;flex:0 0 auto;width:1rem;margin-right:.625rem;text-align:center;transition:inherit}.nav-main-link .nav-main-link-name{display:inline-block;flex:1 1 auto;max-width:100%}.nav-main-link.nav-main-link-submenu{padding-right:2rem}.nav-main-link.nav-main-link-submenu:before{position:absolute;top:50%;right:.625rem;display:block;width:1rem;height:1rem;margin-top:-.5rem;font-weight:900;font-size:.75rem;font-family:Font Awesome\\ 5 Free,Font Awesome\\ 5 Pro,serif;line-height:1rem;text-align:center;opacity:.4;transition:opacity .25s ease-out,transform .25s ease-out;content:\"\uF104\"}.nav-main-submenu{height:0;padding-left:2.875rem;overflow:hidden;list-style:none}.nav-main-submenu .nav-main-item{transform:translateX(-.75rem);opacity:0;transition:opacity .25s ease-out,transform .25s ease-out}.nav-main-submenu .nav-main-heading{padding-top:1.25rem;padding-bottom:.25rem;padding-left:0}.nav-main-submenu .nav-main-link{min-height:2.125rem;margin:0;padding-top:.5rem;padding-bottom:.5rem;padding-left:0;font-size:.8125rem}.nav-main-submenu .nav-main-link .active,.nav-main-submenu .nav-main-link:focus,.nav-main-submenu .nav-main-link:hover{background-color:transparent}.nav-main-submenu .nav-main-submenu{padding-left:.75rem}.nav-main-submenu .nav-main-item.open .nav-main-link{background-color:transparent}.nav-main-item.open>.nav-main-link-submenu:before{transform:rotate(-90deg)}.nav-main-item.open>.nav-main-submenu{height:auto}.nav-main-item.open>.nav-main-submenu .nav-main-item{transform:translateX(0);opacity:1}"]
            },] }
];
NavMenuItemComponent.propDecorators = {
    item: [{ type: Input }],
    id: [{ type: Input }],
    openedId: [{ type: Input }],
    filterValue: [{ type: Input }],
    expandAll: [{ type: Input }],
    setOpenId: [{ type: Output }]
};
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoibmF2LW1lbnUtaXRlbS5jb21wb25lbnQuanMiLCJzb3VyY2VSb290IjoiIiwic291cmNlcyI6WyIuLi8uLi8uLi8uLi8uLi8uLi8uLi8uLi9zcmMvYXBwL2xpYi9uYXYtbWVudS9jb21wb25lbnRzL25hdi1tZW51LWl0ZW0vbmF2LW1lbnUtaXRlbS5jb21wb25lbnQudHMiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IkFBQUEsT0FBTyxFQUNOLFNBQVMsRUFDVCxZQUFZLEVBQ1osS0FBSyxFQUVMLE1BQU0sR0FFTixNQUFNLGVBQWUsQ0FBQztBQVN2QixNQUFNLE9BQU8sb0JBQW9CO0lBTGpDO1FBb0NDOztXQUVHO1FBRUgsY0FBUyxHQUFnQyxJQUFJLFlBQVksRUFBaUIsQ0FBQztRQUUzRTs7V0FFRztRQUNILFdBQU0sR0FBRyxLQUFLLENBQUM7SUFvQ2hCLENBQUM7SUFsQ0E7O09BRUc7SUFDSCxXQUFXLENBQUMsT0FBc0I7O1FBQ2pDLElBQUksT0FBTyxDQUFDLElBQUksRUFBRTtZQUNqQixJQUFJLENBQUMsTUFBTSxHQUFHLE9BQUEsT0FBTyxDQUFDLElBQUksQ0FBQyxZQUFZLENBQUMsUUFBUSwwQ0FBRSxNQUFNLEVBQ3ZELENBQUMsQ0FBQyxJQUFJLENBQUMsY0FBYyxDQUFDLE9BQU8sQ0FBQyxJQUFJLENBQUMsWUFBWSxDQUFDO2dCQUNoRCxDQUFDLENBQUMsS0FBSyxDQUFDO1NBQ1Q7YUFBTSxJQUFJLE9BQU8sQ0FBQyxRQUFRLElBQUksSUFBSSxDQUFDLEVBQUUsRUFBRTtZQUN2QyxJQUFJLENBQUMsTUFBTSxHQUFHLElBQUksQ0FBQyxFQUFFLEtBQUssT0FBTyxDQUFDLFFBQVEsQ0FBQyxZQUFZLENBQUM7U0FDeEQ7SUFDRixDQUFDO0lBRUQ7O09BRUc7SUFDSCxNQUFNO1FBQ0wsSUFBSSxDQUFDLE1BQU0sR0FBRyxDQUFDLElBQUksQ0FBQyxNQUFNLENBQUM7UUFFM0IsSUFBSSxJQUFJLENBQUMsRUFBRSxFQUFFO1lBQ1osSUFBSSxDQUFDLFNBQVMsQ0FBQyxJQUFJLENBQUMsSUFBSSxDQUFDLE1BQU0sQ0FBQyxDQUFDLENBQUMsSUFBSSxDQUFDLEVBQUUsQ0FBQyxDQUFDLENBQUMsSUFBSSxDQUFDLENBQUM7U0FDbEQ7SUFDRixDQUFDO0lBRUQ7O09BRUc7SUFDSyxjQUFjLENBQUMsSUFBYTs7UUFDbkMsVUFBSSxJQUFJLENBQUMsUUFBUSwwQ0FBRSxNQUFNLEVBQUU7WUFDMUIsT0FBTyxJQUFJLENBQUMsUUFBUSxDQUFDLElBQUksQ0FBQyxDQUFDLEtBQUssRUFBRSxFQUFFLENBQUMsSUFBSSxDQUFDLGNBQWMsQ0FBQyxLQUFLLENBQUMsQ0FBQyxDQUFDO1NBQ2pFO2FBQU07WUFDTixPQUFPLENBQUMsQ0FBQyxJQUFJLENBQUMsUUFBUSxDQUFDO1NBQ3ZCO0lBQ0YsQ0FBQzs7O1lBaEZELFNBQVMsU0FBQztnQkFDVixRQUFRLEVBQUUscUJBQXFCO2dCQUMvQiw0Z0RBQTZDOzthQUU3Qzs7O21CQUtDLEtBQUs7aUJBTUwsS0FBSzt1QkFNTCxLQUFLOzBCQU1MLEtBQUs7d0JBTUwsS0FBSzt3QkFNTCxNQUFNIiwic291cmNlc0NvbnRlbnQiOlsiaW1wb3J0IHtcblx0Q29tcG9uZW50LFxuXHRFdmVudEVtaXR0ZXIsXG5cdElucHV0LFxuXHRPbkNoYW5nZXMsXG5cdE91dHB1dCxcblx0U2ltcGxlQ2hhbmdlcyxcbn0gZnJvbSAnQGFuZ3VsYXIvY29yZSc7XG5cbmltcG9ydCB7IE5hdkl0ZW0gfSBmcm9tICcuLi8uLi9tb2RlbHMnO1xuXG5AQ29tcG9uZW50KHtcblx0c2VsZWN0b3I6ICd1aS1uZy1uYXYtbWVudS1pdGVtJyxcblx0dGVtcGxhdGVVcmw6ICcuL25hdi1tZW51LWl0ZW0uY29tcG9uZW50Lmh0bWwnLFxuXHRzdHlsZVVybHM6IFsnLi9uYXYtbWVudS1pdGVtLmNvbXBvbmVudC5zY3NzJ10sXG59KVxuZXhwb3J0IGNsYXNzIE5hdk1lbnVJdGVtQ29tcG9uZW50IGltcGxlbWVudHMgT25DaGFuZ2VzIHtcblx0LyoqXG5cdCAqIFNpbmdsZSBtYWluIG1hbnkgaXRlbSB3aXRoIG9yIHdpdGhvdXQgY2hpbGRyZW5cblx0ICovXG5cdEBJbnB1dCgpXG5cdGl0ZW0hOiBOYXZJdGVtO1xuXG5cdC8qKlxuXHQgKiBUb3AtbGV2ZWwgbWVudSBpdGVtIGlkXG5cdCAqL1xuXHRASW5wdXQoKVxuXHRpZCE6IG51bWJlcjtcblxuXHQvKipcblx0ICogT3BlbmVkIHRvcC1sZXZlbCBtZW51IGl0ZW0gaWRcblx0ICovXG5cdEBJbnB1dCgpXG5cdG9wZW5lZElkITogbnVtYmVyIHwgbnVsbDtcblxuXHQvKipcblx0ICogU2VhcmNoIHRleHQgdG8gZmlsdGVyIG5hdiBpdGVtIGJ5IG5hbWVcblx0ICovXG5cdEBJbnB1dCgpXG5cdGZpbHRlclZhbHVlITogc3RyaW5nO1xuXG5cdC8qKlxuXHQgKiBEZWZpbmVzIGlmIGFsbCBtZW51IGl0ZW1zIGFyZSBleHBhbmRlZCBhdCBhIHRpbWUgKGlmIGZpbHRlciBpcyBhcHBsaWVkKVxuXHQgKi9cblx0QElucHV0KClcblx0ZXhwYW5kQWxsITogYm9vbGVhbjtcblxuXHQvKipcblx0ICogU2V0cyB0b3AtbGV2ZWwgbWVudSBpdGVtIGlkXG5cdCAqL1xuXHRAT3V0cHV0KClcblx0c2V0T3BlbklkOiBFdmVudEVtaXR0ZXI8bnVtYmVyIHwgbnVsbD4gPSBuZXcgRXZlbnRFbWl0dGVyPG51bWJlciB8IG51bGw+KCk7XG5cblx0LyoqXG5cdCAqIE1hcmtzIG9wZW4gbWVudSBpdGVtc1xuXHQgKi9cblx0aXNPcGVuID0gZmFsc2U7XG5cblx0LyoqXG5cdCAqIFVwZGF0ZXMgbWVudSBpdGVtIG9wZW4gc3RhdGUgYmFzZWQgb24gaXRzIGlkIC8gb3dpbmcgb2YgYWN0aXZlIGl0ZW1cblx0ICovXG5cdG5nT25DaGFuZ2VzKGNoYW5nZXM6IFNpbXBsZUNoYW5nZXMpIHtcblx0XHRpZiAoY2hhbmdlcy5pdGVtKSB7XG5cdFx0XHR0aGlzLmlzT3BlbiA9IGNoYW5nZXMuaXRlbS5jdXJyZW50VmFsdWUuY2hpbGRyZW4/Lmxlbmd0aFxuXHRcdFx0XHQ/IHRoaXMuaGFzQWN0aXZlQ2hpbGQoY2hhbmdlcy5pdGVtLmN1cnJlbnRWYWx1ZSlcblx0XHRcdFx0OiBmYWxzZTtcblx0XHR9IGVsc2UgaWYgKGNoYW5nZXMub3BlbmVkSWQgJiYgdGhpcy5pZCkge1xuXHRcdFx0dGhpcy5pc09wZW4gPSB0aGlzLmlkID09PSBjaGFuZ2VzLm9wZW5lZElkLmN1cnJlbnRWYWx1ZTtcblx0XHR9XG5cdH1cblxuXHQvKipcblx0ICogVG9nZ2xlcyBuYXYgbWVudSBpdGVtIHN0YXRlIC0gb3Blbi9jbG9zZVxuXHQgKi9cblx0dG9nZ2xlKCk6IHZvaWQge1xuXHRcdHRoaXMuaXNPcGVuID0gIXRoaXMuaXNPcGVuO1xuXG5cdFx0aWYgKHRoaXMuaWQpIHtcblx0XHRcdHRoaXMuc2V0T3BlbklkLmVtaXQodGhpcy5pc09wZW4gPyB0aGlzLmlkIDogbnVsbCk7XG5cdFx0fVxuXHR9XG5cblx0LyoqXG5cdCAqIENoZWNrcyBpZiBhbW9uZyBtZW51IGl0ZW1zIGNoaWxkcmVuIGlzIGFuIGFjdGl2ZSBvbmVcblx0ICovXG5cdHByaXZhdGUgaGFzQWN0aXZlQ2hpbGQoaXRlbTogTmF2SXRlbSk6IGJvb2xlYW4ge1xuXHRcdGlmIChpdGVtLmNoaWxkcmVuPy5sZW5ndGgpIHtcblx0XHRcdHJldHVybiBpdGVtLmNoaWxkcmVuLnNvbWUoKGNoaWxkKSA9PiB0aGlzLmhhc0FjdGl2ZUNoaWxkKGNoaWxkKSk7XG5cdFx0fSBlbHNlIHtcblx0XHRcdHJldHVybiAhIWl0ZW0uaXNBY3RpdmU7XG5cdFx0fVxuXHR9XG59XG4iXX0=