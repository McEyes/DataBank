import * as i0 from "@angular/core";
export declare class LoginService {
    constructor();
    /**
     * request
     * @param params
     * @returns {Promise<{success: boolean,msg: string}>|Promise<R>}
     */
    initLogin(params: any): any;
    static logout(): void;
    static ɵfac: i0.ɵɵFactoryDeclaration<LoginService, never>;
    static ɵprov: i0.ɵɵInjectableDeclaration<LoginService>;
}
