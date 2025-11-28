import { EventEmitter, OnChanges, SimpleChanges } from '@angular/core';
import { NavItem } from '../../models';
import * as ɵngcc0 from '@angular/core';
export declare class NavMenuItemComponent implements OnChanges {
    /**
     * Single main many item with or without children
     */
    item: NavItem;
    /**
     * Top-level menu item id
     */
    id: number;
    /**
     * Opened top-level menu item id
     */
    openedId: number | null;
    /**
     * Search text to filter nav item by name
     */
    filterValue: string;
    /**
     * Defines if all menu items are expanded at a time (if filter is applied)
     */
    expandAll: boolean;
    /**
     * Sets top-level menu item id
     */
    setOpenId: EventEmitter<number | null>;
    /**
     * Marks open menu items
     */
    isOpen: boolean;
    /**
     * Updates menu item open state based on its id / owing of active item
     */
    ngOnChanges(changes: SimpleChanges): void;
    /**
     * Toggles nav menu item state - open/close
     */
    toggle(): void;
    /**
     * Checks if among menu items children is an active one
     */
    private hasActiveChild;
    static ɵfac: ɵngcc0.ɵɵFactoryDeclaration<NavMenuItemComponent, never>;
    static ɵcmp: ɵngcc0.ɵɵComponentDeclaration<NavMenuItemComponent, "ui-ng-nav-menu-item", never, { "item": "item"; "id": "id"; "openedId": "openedId"; "filterValue": "filterValue"; "expandAll": "expandAll"; }, { "setOpenId": "setOpenId"; }, never, never>;
}

//# sourceMappingURL=nav-menu-item.component.d.ts.map