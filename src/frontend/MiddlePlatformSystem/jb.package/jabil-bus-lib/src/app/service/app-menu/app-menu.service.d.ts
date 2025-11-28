import * as i0 from "@angular/core";
export declare class AppMenuService {
    private defaultCheckedKeys;
    private translateObj;
    constructor();
    getMenuData(menuTreeKeys: any, menuTree: any): Promise<any>;
    sortMenu(data: Array<any>): void;
    buildTableTreeData(data: any, pName?: string, greatPName?: string): void;
    getWebApp(str: string): string;
    isExternalNetwork(): void;
    getMenuInfo(menuTreeKeys: any, menuTree: any, key: string): null;
    buildMenuData(data: any, menuTreeKeys: Array<any>): void;
    static ɵfac: i0.ɵɵFactoryDeclaration<AppMenuService, never>;
    static ɵprov: i0.ɵɵInjectableDeclaration<AppMenuService>;
}
