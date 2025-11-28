import * as i0 from "@angular/core";
export declare class ExcelExportService {
    infoList: Array<any>;
    private fileName;
    exp(data: Array<any>, exportColumns?: Array<any>, fileName?: string): void;
    downloadData(scoredata: Array<any>, worksheet: any, exportColumns?: any): void;
    saveAsExcelFile(buffer: any, fileName: string): void;
    static ɵfac: i0.ɵɵFactoryDeclaration<ExcelExportService, never>;
    static ɵprov: i0.ɵɵInjectableDeclaration<ExcelExportService>;
}
