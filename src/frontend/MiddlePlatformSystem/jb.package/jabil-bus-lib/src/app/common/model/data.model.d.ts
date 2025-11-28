export declare class DataModule {
}
export declare class AppsInfo {
    applications: Application[];
}
export declare class Application {
    id: number;
    application: string;
    languages: Language[];
    namespaces: Namespace[];
}
export declare class Language {
    id: number;
    language: string;
    code: string;
}
export declare class Namespace {
    id: number;
    namespace: string;
}
