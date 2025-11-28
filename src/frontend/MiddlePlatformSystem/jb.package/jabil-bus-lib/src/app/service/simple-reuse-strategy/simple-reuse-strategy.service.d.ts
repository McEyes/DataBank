import { ActivatedRouteSnapshot, DetachedRouteHandle, RouteReuseStrategy } from '@angular/router';
import * as i0 from "@angular/core";
export declare class SimpleReuseStrategy implements RouteReuseStrategy {
    private static routeCache;
    private static waitDelete;
    private static currentDelete;
    shouldReuseRoute(future: ActivatedRouteSnapshot, curr: ActivatedRouteSnapshot): boolean;
    shouldDetach(route: ActivatedRouteSnapshot): boolean;
    store(route: ActivatedRouteSnapshot, handle: DetachedRouteHandle): void;
    shouldAttach(route: ActivatedRouteSnapshot): boolean;
    retrieve(route: ActivatedRouteSnapshot): DetachedRouteHandle | any;
    private addRedirectsRecursively;
    private getFullRouteUrl;
    private getFullRouteUrlPaths;
    private getRouteUrlPaths;
    private getRouteData;
    /** delete the route snapshot*/
    static deleteRouteSnapshot(url: string): void;
    static ɵfac: i0.ɵɵFactoryDeclaration<SimpleReuseStrategy, never>;
    static ɵprov: i0.ɵɵInjectableDeclaration<SimpleReuseStrategy>;
}
