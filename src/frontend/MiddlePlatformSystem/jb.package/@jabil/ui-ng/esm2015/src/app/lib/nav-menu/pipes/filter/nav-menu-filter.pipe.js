import { Pipe } from '@angular/core';
export class NavMenuFilterPipe {
    /**
     * Transform function returns array of items matching search term
     * @param value - menu item to be filtered
     * @param term - search term
     */
    transform(value = [], term = '') {
        const searchText = term.trim();
        if (!searchText) {
            return value;
        }
        return value.filter((item) => this.containsFilterValue(item, searchText));
    }
    containsFilterValue(menuItem, filterValue) {
        var _a;
        return !((_a = menuItem.children) === null || _a === void 0 ? void 0 : _a.length)
            ? menuItem.name.toLowerCase().includes(filterValue.toLowerCase())
            : menuItem.children.some((child) => this.containsFilterValue(child, filterValue));
    }
}
NavMenuFilterPipe.decorators = [
    { type: Pipe, args: [{
                name: 'uiNgNavMenuFilter',
            },] }
];
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoibmF2LW1lbnUtZmlsdGVyLnBpcGUuanMiLCJzb3VyY2VSb290IjoiIiwic291cmNlcyI6WyIuLi8uLi8uLi8uLi8uLi8uLi8uLi8uLi9zcmMvYXBwL2xpYi9uYXYtbWVudS9waXBlcy9maWx0ZXIvbmF2LW1lbnUtZmlsdGVyLnBpcGUudHMiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IkFBQUEsT0FBTyxFQUFFLElBQUksRUFBaUIsTUFBTSxlQUFlLENBQUM7QUFPcEQsTUFBTSxPQUFPLGlCQUFpQjtJQUM3Qjs7OztPQUlHO0lBQ0gsU0FBUyxDQUFDLFFBQW1CLEVBQUUsRUFBRSxPQUFlLEVBQUU7UUFDakQsTUFBTSxVQUFVLEdBQUcsSUFBSSxDQUFDLElBQUksRUFBRSxDQUFDO1FBRS9CLElBQUksQ0FBQyxVQUFVLEVBQUU7WUFDaEIsT0FBTyxLQUFLLENBQUM7U0FDYjtRQUVELE9BQU8sS0FBSyxDQUFDLE1BQU0sQ0FBQyxDQUFDLElBQUksRUFBRSxFQUFFLENBQzVCLElBQUksQ0FBQyxtQkFBbUIsQ0FBQyxJQUFJLEVBQUUsVUFBVSxDQUFDLENBQzFDLENBQUM7SUFDSCxDQUFDO0lBRU8sbUJBQW1CLENBQzFCLFFBQWlCLEVBQ2pCLFdBQW1COztRQUVuQixPQUFPLFFBQUMsUUFBUSxDQUFDLFFBQVEsMENBQUUsTUFBTSxDQUFBO1lBQ2hDLENBQUMsQ0FBQyxRQUFRLENBQUMsSUFBSSxDQUFDLFdBQVcsRUFBRSxDQUFDLFFBQVEsQ0FBQyxXQUFXLENBQUMsV0FBVyxFQUFFLENBQUM7WUFDakUsQ0FBQyxDQUFDLFFBQVEsQ0FBQyxRQUFRLENBQUMsSUFBSSxDQUFDLENBQUMsS0FBSyxFQUFFLEVBQUUsQ0FDakMsSUFBSSxDQUFDLG1CQUFtQixDQUFDLEtBQUssRUFBRSxXQUFXLENBQUMsQ0FDM0MsQ0FBQztJQUNOLENBQUM7OztZQTlCRCxJQUFJLFNBQUM7Z0JBQ0wsSUFBSSxFQUFFLG1CQUFtQjthQUN6QiIsInNvdXJjZXNDb250ZW50IjpbImltcG9ydCB7IFBpcGUsIFBpcGVUcmFuc2Zvcm0gfSBmcm9tICdAYW5ndWxhci9jb3JlJztcblxuaW1wb3J0IHsgTmF2SXRlbSB9IGZyb20gJy4uLy4uL21vZGVscyc7XG5cbkBQaXBlKHtcblx0bmFtZTogJ3VpTmdOYXZNZW51RmlsdGVyJyxcbn0pXG5leHBvcnQgY2xhc3MgTmF2TWVudUZpbHRlclBpcGUgaW1wbGVtZW50cyBQaXBlVHJhbnNmb3JtIHtcblx0LyoqXG5cdCAqIFRyYW5zZm9ybSBmdW5jdGlvbiByZXR1cm5zIGFycmF5IG9mIGl0ZW1zIG1hdGNoaW5nIHNlYXJjaCB0ZXJtXG5cdCAqIEBwYXJhbSB2YWx1ZSAtIG1lbnUgaXRlbSB0byBiZSBmaWx0ZXJlZFxuXHQgKiBAcGFyYW0gdGVybSAtIHNlYXJjaCB0ZXJtXG5cdCAqL1xuXHR0cmFuc2Zvcm0odmFsdWU6IE5hdkl0ZW1bXSA9IFtdLCB0ZXJtOiBzdHJpbmcgPSAnJykge1xuXHRcdGNvbnN0IHNlYXJjaFRleHQgPSB0ZXJtLnRyaW0oKTtcblxuXHRcdGlmICghc2VhcmNoVGV4dCkge1xuXHRcdFx0cmV0dXJuIHZhbHVlO1xuXHRcdH1cblxuXHRcdHJldHVybiB2YWx1ZS5maWx0ZXIoKGl0ZW0pID0+XG5cdFx0XHR0aGlzLmNvbnRhaW5zRmlsdGVyVmFsdWUoaXRlbSwgc2VhcmNoVGV4dCksXG5cdFx0KTtcblx0fVxuXG5cdHByaXZhdGUgY29udGFpbnNGaWx0ZXJWYWx1ZShcblx0XHRtZW51SXRlbTogTmF2SXRlbSxcblx0XHRmaWx0ZXJWYWx1ZTogc3RyaW5nLFxuXHQpOiBib29sZWFuIHtcblx0XHRyZXR1cm4gIW1lbnVJdGVtLmNoaWxkcmVuPy5sZW5ndGhcblx0XHRcdD8gbWVudUl0ZW0ubmFtZS50b0xvd2VyQ2FzZSgpLmluY2x1ZGVzKGZpbHRlclZhbHVlLnRvTG93ZXJDYXNlKCkpXG5cdFx0XHQ6IG1lbnVJdGVtLmNoaWxkcmVuLnNvbWUoKGNoaWxkKSA9PlxuXHRcdFx0XHRcdHRoaXMuY29udGFpbnNGaWx0ZXJWYWx1ZShjaGlsZCwgZmlsdGVyVmFsdWUpLFxuXHRcdFx0ICApO1xuXHR9XG59XG4iXX0=