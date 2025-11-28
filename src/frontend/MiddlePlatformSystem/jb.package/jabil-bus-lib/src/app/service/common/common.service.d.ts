import * as i0 from "@angular/core";
export declare class CommonService {
    dateFormat: (fmt: any, date: any) => any;
    dateFormatNew: (type: any, now: any) => string;
    buildEChartLabel(params: string, n?: number, agrs?: Array<any>): string | undefined;
    genclass: (url: string) => string;
    isInvalid: (obj: any, list: Array<any>) => boolean;
    getNextDate: (date: any, day?: number, format?: string) => string | undefined;
    second2Hour: (value: number) => string;
    /**
     * Array transform to tree
     * @param {array} data
     * @param {string} id
     */
    arrayToTree: (data: any, id: string) => any[];
    /**
     * Tree transform to array
     * @param tree
     */
    treeToArray: (tree: any) => any[];
    printNewWindow: (imgSrc: string, option?: any) => void;
    getHourByMinute: (minute: number) => number;
    formInvalid: (obj: any, list: Array<any>) => boolean;
    getBase64: (file: File) => Promise<string | ArrayBuffer | null>;
    downloadFile: (urlStr: string, fileName: string) => void;
    optionsValid: (optionList: Array<any>, type: any, messageService: any, TranslateData: any) => boolean;
    translateData: (Obj: any, translate: any) => void;
    imgToBase64: (imgSrc: string) => void;
    setFullScreen: (exitFullScreen?: boolean) => void;
    addRouteEvent(router: any): void;
    logoff: () => void;
    getToken: () => string | null;
    saveLayout(layout: any): void;
    getGuid(): string;
    initWebNotice(signalR: any): void;
    reciveWebNotice(data: any): any;
    private notify;
    registerNotice(options: any): void;
    static ɵfac: i0.ɵɵFactoryDeclaration<CommonService, never>;
    static ɵprov: i0.ɵɵInjectableDeclaration<CommonService>;
}
