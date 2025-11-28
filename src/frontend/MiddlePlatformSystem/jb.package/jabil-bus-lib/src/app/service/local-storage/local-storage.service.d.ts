export declare class LocalStorage {
    localStorage: any;
    constructor();
    set(key: string, value: string): void;
    get(key: string): string;
    setObject(key: string, value: any): void;
    getObject(key: string): any;
    remove(key: string): any;
}
