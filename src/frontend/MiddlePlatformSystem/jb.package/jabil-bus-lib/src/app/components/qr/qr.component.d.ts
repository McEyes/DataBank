import { OnInit } from '@angular/core';
import * as i0 from "@angular/core";
export declare class QRComponent implements OnInit {
    field: any;
    width: string;
    height: string;
    loading: boolean;
    QRUrl: string;
    constructor();
    ngOnInit(): Promise<void>;
    getQR(): void;
    printQR(): void;
    static ɵfac: i0.ɵɵFactoryDeclaration<QRComponent, never>;
    static ɵcmp: i0.ɵɵComponentDeclaration<QRComponent, "jabil-qr", never, { "field": "field"; "width": "width"; "height": "height"; }, {}, never, never, false>;
}
