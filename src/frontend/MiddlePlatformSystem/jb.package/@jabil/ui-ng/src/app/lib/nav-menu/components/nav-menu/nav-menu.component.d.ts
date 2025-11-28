import { OnChanges, SimpleChanges } from '@angular/core';
import { NavItem } from '../../models';
import * as ɵngcc0 from '@angular/core';
declare type NavMenuTheme = 'dark' | 'light';
/**
 * Main navigation component
 */
export declare class NavMenuComponent implements OnChanges {
    /**
     * Nav Munu Items
     */
    items: NavItem[];
    /**
     * Nav Menu Theme
     */
    theme: NavMenuTheme;
    /**
     * Defines whether custom or default scrollbar should be used
     */
    customScrollbar: boolean;
    /**
     * Nav Menu Mini Mode Enabled (if shown only)
     */
    miniMode: boolean;
    /**
     * Defines menu position - fixed or default
     */
    isFixed: boolean;
    /**
     * Define is collapse (mini mode) triggered on
     */
    isMiniOnMouseLeave: boolean;
    /**
     * Search text to filter nav item by name
     */
    filterValue: string;
    /**
     * Opened menu item index;
     */
    openedId: number | null;
    /**
     * Defines if all menu items are expanded at a time (if filter is applied)
     */
    expandAll: boolean;
    /**
     * Defines if nav menu should be shown
     */
    isShown: boolean;
    /**
     * Resets openedId if items change
     */
    ngOnChanges(changes: SimpleChanges): void;
    /**
     * Shows nav menu
     */
    show(): void;
    /**
     * Hides nav menu
     */
    hide(): void;
    /**
     * Toggles nav menu appearance
     */
    toggle(): void;
    /**
     * Sets index of opened menu item
     */
    setOpenedId(index: number | null): void;
    static ɵfac: ɵngcc0.ɵɵFactoryDeclaration<NavMenuComponent, never>;
    static ɵcmp: ɵngcc0.ɵɵComponentDeclaration<NavMenuComponent, "ui-ng-nav-menu", never, { "items": "items"; "theme": "theme"; "customScrollbar": "customScrollbar"; "miniMode": "miniMode"; "isFixed": "isFixed"; "isMiniOnMouseLeave": "isMiniOnMouseLeave"; "filterValue": "filterValue"; }, {}, never, ["*"]>;
}
export {};

//# sourceMappingURL=nav-menu.component.d.ts.map