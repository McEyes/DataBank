import { Observable } from 'rxjs';
import * as i0 from "@angular/core";
export declare class JBEventBusService {
    constructor();
    send(message: any): void;
    get(): Observable<any>;
    static ɵfac: i0.ɵɵFactoryDeclaration<JBEventBusService, never>;
    static ɵprov: i0.ɵɵInjectableDeclaration<JBEventBusService>;
}
