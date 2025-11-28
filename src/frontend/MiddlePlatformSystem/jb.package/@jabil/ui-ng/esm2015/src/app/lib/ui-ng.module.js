import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ImageOverlayComponent } from './image-overlay/components/image-overlay/image-overlay.component';
import { LayoutComponent } from './layout/components/layout.component';
import { NavMenuItemComponent } from './nav-menu/components/nav-menu-item/nav-menu-item.component';
import { NavMenuComponent } from './nav-menu/components/nav-menu/nav-menu.component';
import { NavMenuFilterPipe } from './nav-menu/pipes/filter/nav-menu-filter.pipe';
export * from './image-overlay/components/image-overlay/image-overlay.component';
export * from './layout/components/layout.component';
export * from './layout/models/layout-config';
export * from './nav-menu/components/nav-menu/nav-menu.component';
export * from './nav-menu/models/nav-item';
export class UiNgModule {
}
UiNgModule.decorators = [
    { type: NgModule, args: [{
                declarations: [
                    ImageOverlayComponent,
                    LayoutComponent,
                    NavMenuComponent,
                    NavMenuItemComponent,
                    NavMenuFilterPipe,
                ],
                imports: [CommonModule],
                exports: [ImageOverlayComponent, LayoutComponent, NavMenuComponent],
            },] }
];
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoidWktbmcubW9kdWxlLmpzIiwic291cmNlUm9vdCI6IiIsInNvdXJjZXMiOlsiLi4vLi4vLi4vLi4vLi4vc3JjL2FwcC9saWIvdWktbmcubW9kdWxlLnRzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiJBQUFBLE9BQU8sRUFBRSxZQUFZLEVBQUUsTUFBTSxpQkFBaUIsQ0FBQztBQUMvQyxPQUFPLEVBQUUsUUFBUSxFQUFFLE1BQU0sZUFBZSxDQUFDO0FBRXpDLE9BQU8sRUFBRSxxQkFBcUIsRUFBRSxNQUFNLGtFQUFrRSxDQUFDO0FBQ3pHLE9BQU8sRUFBRSxlQUFlLEVBQUUsTUFBTSxzQ0FBc0MsQ0FBQztBQUN2RSxPQUFPLEVBQUUsb0JBQW9CLEVBQUUsTUFBTSw2REFBNkQsQ0FBQztBQUNuRyxPQUFPLEVBQUUsZ0JBQWdCLEVBQUUsTUFBTSxtREFBbUQsQ0FBQztBQUNyRixPQUFPLEVBQUUsaUJBQWlCLEVBQUUsTUFBTSw4Q0FBOEMsQ0FBQztBQUVqRixjQUFjLGtFQUFrRSxDQUFDO0FBQ2pGLGNBQWMsc0NBQXNDLENBQUM7QUFDckQsY0FBYywrQkFBK0IsQ0FBQztBQUM5QyxjQUFjLG1EQUFtRCxDQUFDO0FBQ2xFLGNBQWMsNEJBQTRCLENBQUM7QUFhM0MsTUFBTSxPQUFPLFVBQVU7OztZQVh0QixRQUFRLFNBQUM7Z0JBQ1QsWUFBWSxFQUFFO29CQUNiLHFCQUFxQjtvQkFDckIsZUFBZTtvQkFDZixnQkFBZ0I7b0JBQ2hCLG9CQUFvQjtvQkFDcEIsaUJBQWlCO2lCQUNqQjtnQkFDRCxPQUFPLEVBQUUsQ0FBQyxZQUFZLENBQUM7Z0JBQ3ZCLE9BQU8sRUFBRSxDQUFDLHFCQUFxQixFQUFFLGVBQWUsRUFBRSxnQkFBZ0IsQ0FBQzthQUNuRSIsInNvdXJjZXNDb250ZW50IjpbImltcG9ydCB7IENvbW1vbk1vZHVsZSB9IGZyb20gJ0Bhbmd1bGFyL2NvbW1vbic7XG5pbXBvcnQgeyBOZ01vZHVsZSB9IGZyb20gJ0Bhbmd1bGFyL2NvcmUnO1xuXG5pbXBvcnQgeyBJbWFnZU92ZXJsYXlDb21wb25lbnQgfSBmcm9tICcuL2ltYWdlLW92ZXJsYXkvY29tcG9uZW50cy9pbWFnZS1vdmVybGF5L2ltYWdlLW92ZXJsYXkuY29tcG9uZW50JztcbmltcG9ydCB7IExheW91dENvbXBvbmVudCB9IGZyb20gJy4vbGF5b3V0L2NvbXBvbmVudHMvbGF5b3V0LmNvbXBvbmVudCc7XG5pbXBvcnQgeyBOYXZNZW51SXRlbUNvbXBvbmVudCB9IGZyb20gJy4vbmF2LW1lbnUvY29tcG9uZW50cy9uYXYtbWVudS1pdGVtL25hdi1tZW51LWl0ZW0uY29tcG9uZW50JztcbmltcG9ydCB7IE5hdk1lbnVDb21wb25lbnQgfSBmcm9tICcuL25hdi1tZW51L2NvbXBvbmVudHMvbmF2LW1lbnUvbmF2LW1lbnUuY29tcG9uZW50JztcbmltcG9ydCB7IE5hdk1lbnVGaWx0ZXJQaXBlIH0gZnJvbSAnLi9uYXYtbWVudS9waXBlcy9maWx0ZXIvbmF2LW1lbnUtZmlsdGVyLnBpcGUnO1xuXG5leHBvcnQgKiBmcm9tICcuL2ltYWdlLW92ZXJsYXkvY29tcG9uZW50cy9pbWFnZS1vdmVybGF5L2ltYWdlLW92ZXJsYXkuY29tcG9uZW50JztcbmV4cG9ydCAqIGZyb20gJy4vbGF5b3V0L2NvbXBvbmVudHMvbGF5b3V0LmNvbXBvbmVudCc7XG5leHBvcnQgKiBmcm9tICcuL2xheW91dC9tb2RlbHMvbGF5b3V0LWNvbmZpZyc7XG5leHBvcnQgKiBmcm9tICcuL25hdi1tZW51L2NvbXBvbmVudHMvbmF2LW1lbnUvbmF2LW1lbnUuY29tcG9uZW50JztcbmV4cG9ydCAqIGZyb20gJy4vbmF2LW1lbnUvbW9kZWxzL25hdi1pdGVtJztcblxuQE5nTW9kdWxlKHtcblx0ZGVjbGFyYXRpb25zOiBbXG5cdFx0SW1hZ2VPdmVybGF5Q29tcG9uZW50LFxuXHRcdExheW91dENvbXBvbmVudCxcblx0XHROYXZNZW51Q29tcG9uZW50LFxuXHRcdE5hdk1lbnVJdGVtQ29tcG9uZW50LFxuXHRcdE5hdk1lbnVGaWx0ZXJQaXBlLFxuXHRdLFxuXHRpbXBvcnRzOiBbQ29tbW9uTW9kdWxlXSxcblx0ZXhwb3J0czogW0ltYWdlT3ZlcmxheUNvbXBvbmVudCwgTGF5b3V0Q29tcG9uZW50LCBOYXZNZW51Q29tcG9uZW50XSxcbn0pXG5leHBvcnQgY2xhc3MgVWlOZ01vZHVsZSB7fVxuIl19