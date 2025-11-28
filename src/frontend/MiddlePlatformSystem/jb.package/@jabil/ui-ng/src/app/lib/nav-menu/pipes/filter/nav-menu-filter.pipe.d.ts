import { PipeTransform } from '@angular/core';
import { NavItem } from '../../models';
import * as ɵngcc0 from '@angular/core';
export declare class NavMenuFilterPipe implements PipeTransform {
    /**
     * Transform function returns array of items matching search term
     * @param value - menu item to be filtered
     * @param term - search term
     */
    transform(value?: NavItem[], term?: string): NavItem[];
    private containsFilterValue;
    static ɵfac: ɵngcc0.ɵɵFactoryDeclaration<NavMenuFilterPipe, never>;
    static ɵpipe: ɵngcc0.ɵɵPipeDeclaration<NavMenuFilterPipe, "uiNgNavMenuFilter">;
}

//# sourceMappingURL=nav-menu-filter.pipe.d.ts.map