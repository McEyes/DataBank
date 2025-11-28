import { ElementRef, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import * as i0 from "@angular/core";
export declare class FileUploadComponent implements OnInit {
    private httpClient;
    fileImport: ElementRef | undefined;
    loading: boolean;
    progressShow: boolean;
    isCancel: boolean;
    fileProgress: number;
    totalPieces: number;
    firmwareFileList: Array<any>;
    examList: Array<any>;
    fileLast: any;
    fileObj: any;
    bytesPerPiece: any;
    uploadUrl: string;
    constructor(httpClient: HttpClient);
    ngOnInit(): Promise<void>;
    clickUpload(): void;
    inputFileChange(e: any): void;
    loopSend(item: any, start: number, index: number): void;
    cancelUpload(): void;
    static ɵfac: i0.ɵɵFactoryDeclaration<FileUploadComponent, never>;
    static ɵcmp: i0.ɵɵComponentDeclaration<FileUploadComponent, "jabil-file-upload", never, {}, {}, never, never, false>;
}
