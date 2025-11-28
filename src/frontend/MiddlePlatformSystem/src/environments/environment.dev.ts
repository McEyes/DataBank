// This file can be replaced during build by using the `fileReplacements` array.
// `ng build` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

export const environment = {
  production: true,
  LoginBusServer: 'https://cnhuam0itds02.corp.jabil.org/gateway/auth',
  SearchServer: 'https://cnhuam0itds02.corp.jabil.org/gateway/search',
  FileServer: 'https://cnhuam0itds02.corp.jabil.org/gateway/resource',
  ITPortalFlowServer: 'https://cnhuam0itds02.corp.jabil.org/gateway/workflow',
  BasicServer: 'https://cnhuam0itds02.corp.jabil.org/gateway/dataasset',
  DataQualityServer: 'https://hua-databank.corp.jabil.org/gateway/dataquality',
  DashbordServer: 'https://hua-databank.corp.jabil.org/gateway/dashbord',
  DataTopicStoreServer: '/gateway/datatopicstore'
};

/*
 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/plugins/zone-error';  // Included with Angular CLI.
