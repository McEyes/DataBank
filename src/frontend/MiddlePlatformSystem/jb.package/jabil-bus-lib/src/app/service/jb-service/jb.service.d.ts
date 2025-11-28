import { HttpService } from '../api/api.service';
import * as i0 from "@angular/core";
export declare class JBService {
    private http;
    constructor(http: HttpService);
    setToken(url: string, data: any, fn?: Function): void;
    static ɵfac: i0.ɵɵFactoryDeclaration<JBService, never>;
    static ɵprov: i0.ɵɵInjectableDeclaration<JBService>;
}
