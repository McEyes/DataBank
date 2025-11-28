import { OnInit, EventEmitter } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import * as i0 from "@angular/core";
export declare class VideoDialogComponent implements OnInit {
    private sanitizer;
    field: any;
    closeDialog: EventEmitter<string>;
    dialogDisplay: boolean;
    currentLanguage: any;
    srcUrl: any;
    constructor(sanitizer: DomSanitizer);
    ngOnInit(): Promise<void>;
    getSrcUrl(): import("@angular/platform-browser").SafeResourceUrl;
    closeEditDialog(): void;
    static ɵfac: i0.ɵɵFactoryDeclaration<VideoDialogComponent, never>;
    static ɵcmp: i0.ɵɵComponentDeclaration<VideoDialogComponent, "jabil-video-dialog", never, { "field": "field"; }, { "closeDialog": "closeDialog"; }, never, never, false>;
}
