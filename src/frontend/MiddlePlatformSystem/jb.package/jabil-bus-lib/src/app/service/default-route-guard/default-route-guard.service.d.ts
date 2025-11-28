import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import * as i0 from "@angular/core";
export declare class DefaultRouteGuardService implements CanActivate {
    private router;
    private document;
    constructor(router: Router, document: any);
    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Promise<boolean>;
    static ɵfac: i0.ɵɵFactoryDeclaration<DefaultRouteGuardService, never>;
    static ɵprov: i0.ɵɵInjectableDeclaration<DefaultRouteGuardService>;
}
