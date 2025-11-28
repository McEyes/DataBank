import { HttpClient } from '@angular/common/http';
import * as i0 from "@angular/core";
export declare class BigFileUploadService {
    private httpClient;
    loading: boolean;
    progressShow: boolean;
    isCancel: boolean;
    fileProgress: number;
    totalPieces: number;
    firmwareFileList: Array<any>;
    bytesPerPiece: any;
    baseUrl: string;
    constructor(httpClient: HttpClient);
    inputFileChange(e: any, successFn: Function, failFn: Function, progressFn?: Function): void;
    loopSend(item: any, start: number, index: number, successFn: Function, failFn: Function, progressFn?: Function): void;
    cancelUpload(): void;
    static ɵfac: i0.ɵɵFactoryDeclaration<BigFileUploadService, never>;
    static ɵprov: i0.ɵɵInjectableDeclaration<BigFileUploadService>;
}
