import { Injectable } from '@angular/core';
import { TranslateData } from 'src/app/core/translate/translate-data';

@Injectable()
export default class SystemService {
  public showMsg = function (type: any, msg: any, info?: string) {
    if (type === 'cancelDelete') {
      msg.add({
        severity: 'info',
        summary: TranslateData.delete,
        detail: TranslateData.cancel,
      });
    } else if (type === 'deleteSuccess') {
      msg.add({
        severity: 'success',
        summary: TranslateData.delete,
        detail: TranslateData.success,
      });
    } else if (type === 'fail') {
      msg.add({
        severity: 'error',
        summary: TranslateData.fail,
        detail: info,
      });
    }
  }
}
