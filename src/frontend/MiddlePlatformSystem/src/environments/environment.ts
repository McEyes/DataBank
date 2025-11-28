// This file can be replaced during build by using the `fileReplacements` array.
// `ng build` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

export const environment = {
  production: false,
  // production server
  SearchServer: 'http://cnhuam0itds01/gateway/search',
  FileServer: 'http://cnhuam0itds01/gateway/resource',
  LoginBusServer: 'http://cnhuam0itds01/gateway/auth',
  ITPortalFlowServer: 'http://cnhuam0itds01/gateway/workflow',
  BasicServer: 'http://cnhuam0itds01/gateway/dataasset',
  DataQualityServer: 'http://cnhuam0webstg85:7701',
  DataTopicStoreServer: '/gateway/datatopicstore',
  DashbordServer: 'https://localhost:44342',

  // local development
  // ITPortalFlowServer: 'https://localhost:5001',
  // LoginBusServer: 'https://localhost:6001',
  // BasicServer: 'http://localhost:6010'


};

/*
 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/plugins/zone-error';  // Included with Angular CLI.
