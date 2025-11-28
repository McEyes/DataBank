import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FileUploadComponent } from './file-upload.component';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzProgressModule } from 'ng-zorro-antd/progress';
import * as i0 from "@angular/core";
export class FileUploadModule {
}
FileUploadModule.ɵfac = function FileUploadModule_Factory(t) { return new (t || FileUploadModule)(); };
FileUploadModule.ɵmod = /*@__PURE__*/ i0.ɵɵdefineNgModule({ type: FileUploadModule });
FileUploadModule.ɵinj = /*@__PURE__*/ i0.ɵɵdefineInjector({ imports: [CommonModule,
        NzIconModule,
        NzButtonModule,
        NzProgressModule] });
(function () { (typeof ngDevMode === "undefined" || ngDevMode) && i0.ɵsetClassMetadata(FileUploadModule, [{
        type: NgModule,
        args: [{
                declarations: [FileUploadComponent],
                imports: [
                    CommonModule,
                    NzIconModule,
                    NzButtonModule,
                    NzProgressModule
                ],
                exports: [
                    FileUploadComponent
                ]
            }]
    }], null, null); })();
(function () { (typeof ngJitMode === "undefined" || ngJitMode) && i0.ɵɵsetNgModuleScope(FileUploadModule, { declarations: [FileUploadComponent], imports: [CommonModule,
        NzIconModule,
        NzButtonModule,
        NzProgressModule], exports: [FileUploadComponent] }); })();
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiZmlsZS11cGxvYWQubW9kdWxlLmpzIiwic291cmNlUm9vdCI6IiIsInNvdXJjZXMiOlsiLi4vLi4vLi4vLi4vLi4vLi4vc3JjL2FwcC9jb21wb25lbnRzL2ZpbGUtdXBsb2FkL2ZpbGUtdXBsb2FkLm1vZHVsZS50cyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFBQSxPQUFPLEVBQUUsUUFBUSxFQUFFLE1BQU0sZUFBZSxDQUFDO0FBQ3pDLE9BQU8sRUFBRSxZQUFZLEVBQUUsTUFBTSxpQkFBaUIsQ0FBQztBQUMvQyxPQUFPLEVBQUUsbUJBQW1CLEVBQUUsTUFBTSx5QkFBeUIsQ0FBQTtBQUU3RCxPQUFPLEVBQUUsWUFBWSxFQUFFLE1BQU0sb0JBQW9CLENBQUM7QUFDbEQsT0FBTyxFQUFFLGNBQWMsRUFBRSxNQUFNLHNCQUFzQixDQUFDO0FBQ3RELE9BQU8sRUFBRSxnQkFBZ0IsRUFBRSxNQUFNLHdCQUF3QixDQUFDOztBQWMxRCxNQUFNLE9BQU8sZ0JBQWdCOztnRkFBaEIsZ0JBQWdCO2tFQUFoQixnQkFBZ0I7c0VBVHpCLFlBQVk7UUFDWixZQUFZO1FBQ1osY0FBYztRQUNkLGdCQUFnQjt1RkFNUCxnQkFBZ0I7Y0FaNUIsUUFBUTtlQUFDO2dCQUNSLFlBQVksRUFBRSxDQUFDLG1CQUFtQixDQUFDO2dCQUNuQyxPQUFPLEVBQUU7b0JBQ1AsWUFBWTtvQkFDWixZQUFZO29CQUNaLGNBQWM7b0JBQ2QsZ0JBQWdCO2lCQUNqQjtnQkFDRCxPQUFPLEVBQUU7b0JBQ1AsbUJBQW1CO2lCQUNwQjthQUNGOzt3RkFDWSxnQkFBZ0IsbUJBWFosbUJBQW1CLGFBRWhDLFlBQVk7UUFDWixZQUFZO1FBQ1osY0FBYztRQUNkLGdCQUFnQixhQUdoQixtQkFBbUIiLCJzb3VyY2VzQ29udGVudCI6WyJpbXBvcnQgeyBOZ01vZHVsZSB9IGZyb20gJ0Bhbmd1bGFyL2NvcmUnO1xyXG5pbXBvcnQgeyBDb21tb25Nb2R1bGUgfSBmcm9tICdAYW5ndWxhci9jb21tb24nO1xyXG5pbXBvcnQgeyBGaWxlVXBsb2FkQ29tcG9uZW50IH0gZnJvbSAnLi9maWxlLXVwbG9hZC5jb21wb25lbnQnXHJcblxyXG5pbXBvcnQgeyBOekljb25Nb2R1bGUgfSBmcm9tICduZy16b3Jyby1hbnRkL2ljb24nO1xyXG5pbXBvcnQgeyBOekJ1dHRvbk1vZHVsZSB9IGZyb20gJ25nLXpvcnJvLWFudGQvYnV0dG9uJztcclxuaW1wb3J0IHsgTnpQcm9ncmVzc01vZHVsZSB9IGZyb20gJ25nLXpvcnJvLWFudGQvcHJvZ3Jlc3MnO1xyXG5cclxuQE5nTW9kdWxlKHtcclxuICBkZWNsYXJhdGlvbnM6IFtGaWxlVXBsb2FkQ29tcG9uZW50XSxcclxuICBpbXBvcnRzOiBbXHJcbiAgICBDb21tb25Nb2R1bGUsXHJcbiAgICBOekljb25Nb2R1bGUsXHJcbiAgICBOekJ1dHRvbk1vZHVsZSxcclxuICAgIE56UHJvZ3Jlc3NNb2R1bGVcclxuICBdLFxyXG4gIGV4cG9ydHM6IFtcclxuICAgIEZpbGVVcGxvYWRDb21wb25lbnRcclxuICBdXHJcbn0pXHJcbmV4cG9ydCBjbGFzcyBGaWxlVXBsb2FkTW9kdWxlIHsgfVxyXG4iXX0=