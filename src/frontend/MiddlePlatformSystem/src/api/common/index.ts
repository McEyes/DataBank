import { Injectable } from '@angular/core';
import { HttpService } from 'jabil-bus-lib';
import { NzUploadFile } from 'ng-zorro-antd/upload';
import { environment } from 'src/environments/environment';
import internal from 'stream';
@Injectable()
export default class CommonHttpService {
  constructor(private http: HttpService) { }


  clearCookie() {
    localStorage.removeItem('loginuser');
    localStorage.removeItem('userList');
  }

  // getUserInfo(data?: any) {
  //   return this.http.request({
  //     url: environment.JabilBusServer + '/gateway/basic/accounts/userinfo',
  //     method: 'post',
  //     data,
  //   });
  // }

  async getCurrUser() {
    let userData = { id: '', name: "", department: '', email: '', phone: '', ntid: '' }
    try {
      userData = JSON.parse(localStorage.getItem('loginuser') || '')
    } catch (e) {
      var result = await this.getUserInfo();
      userData = result.data;
      localStorage.setItem('loginuser', JSON.stringify(userData));
      //  .then((res: any) => {
      //   userData = res.data;
      //   localStorage.setItem('loginuser', JSON.stringify(userData));
      // });
    }
    return userData;
  }
  getUserInfo(data?: any) {
    return this.http.request({
      url: environment.LoginBusServer + '/api/account/userinfo',
      method: 'get',
      data,
    });
  }
  // 中英文登录人名
  // useId
  userInfoExtend(data?: any) {
    return this.http.request({
      url: environment.LoginBusServer + '/api/Employee/info?WorkNTID=' + data.ntid,
      method: 'get',
    });
  }

  // 获取所有员工信息
  public getUserList() {
    const header: any = {}
    return this.http.request({
      url: environment.LoginBusServer + `/api/Employee/userinfo/all`,
      method: 'get',
    }, header);
  }

  public getLoginRoleMenuGrant(data?: any) {
    return this.http.request({
      url: environment.LoginBusServer + '/api/user/ownermenus',
      method: 'get',
      data,
    });
  }

  // public getMenuTree(data?: any) {
  //   return this.http.request({
  //     url: environment.JabilBusServer + '/gateway/basic/roleMenu/GetMenuTree',
  //     method: 'get',
  //     data,
  //   });
  // }

  // public getUserType(id?: any) {
  //   return this.http.request({
  //     url: environment.JabilBusServer + '/gateway/basic/accounts/userInfoExtend?useId=' + id,
  //     method: 'post',
  //   });
  // }

  public getUserInfoExt(data?: any) {
    return this.http.request({
      url: environment.LoginBusServer + '/api/Employee/info',
      method: 'get',
      data,
    });
  }

  public getApplication(data?: any) {
    return this.http.request({
      url: environment.BasicServer + '/api/clients/selfapplist',
      // url: environment.BasicServer + '/api/clients/list',
      method: 'get',
      data,
    });
  }

  public getUserInfoByNTID(ntid: string) {
    return this.http.request({
      url: environment.LoginBusServer + '/api/Employee/info/' + ntid,
      method: 'get',
    });
  }


  // 获取员工部门主管
  public GetEmployeeDeptManager(ntid: string) {
    return this.http.request({
      url: environment.LoginBusServer + '/api/Employee/EmployeeDeptManager/' + ntid,
      method: 'get',
    });
  }

  // 根据部门名称获取部门主管
  public GetDeptManager(dept: string) {
    return this.http.request({
      url: environment.LoginBusServer + '/api/Employee/DeptManager/' + dept,
      method: 'get',
    });
  }

  // 根据员工NTID获取Function  Manager
  public GetFunctionManager(ntid: string) {
    return this.http.request({
      url: environment.LoginBusServer + '/api/Employee/EmployeeFM/' + ntid,
      method: 'get',
    });
  }

  public getDepts(data?: any) {
    return this.http.request({
      url: environment.BasicServer + '/api/tables/depts',
      method: 'get',
      data,
    });
  }

  public getDepartments() {
    const header: any = {}
    return this.http.request({
      url: environment.LoginBusServer + `/api/Department/All`,
      method: 'get',
      header
    }, header);
  }


  /// 下载流程附件
  public downloadFlowAttachment(item: any) {
    let nameList = item.fileName.split('.');
    let name = nameList[0];
    this.getBlob(this.fileHost + "/api/file/download/" + item.fileUrl + "?category=DataSourceFlow").then(blob => {
      this.saveAs(blob, name);
    });
  }
  // 导出excel
  public exportExcel(url: any, fileName: string, data: any, httpMethod: string = "POST", xtoken?: any) {
    this.getBlob(url, data, httpMethod, xtoken).then(blob => {
      this.saveAs(blob, fileName);
    });
  }
  // 下载文件
  public download(url: any, fileName: string, data?: any, httpMethod?: string) {
    this.getBlob(url, data, httpMethod).then(blob => {
      this.saveAs(blob, fileName);
    });
  }

  fileHost: string = environment.FileServer;
  private getBlob(url: string, data?: any, httpMethod?: string, xtoken?: any) {
    const method = httpMethod || 'GET';
    data = data || {};
    return new Promise(resolve => {
      const xhr = new XMLHttpRequest();
      xhr.open(method, url, true);
      //, "application/x-www-form-urlencoded")
      // xhr.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
      xhr.setRequestHeader("Content-Type", "application/json");
      xhr.setRequestHeader('Accept', 'application/xlsx,application/vnd.openxmlformats-officedocument.spreadsheetml.sheet');
      xhr.setRequestHeader('Authorization', 'Bearer ' + localStorage.getItem('jwt'));
      if (typeof (xtoken) == 'string')
        xhr.setRequestHeader("x-token", xtoken);
      else if (typeof (xtoken) == 'object') {
        for (const key in xtoken) {
          if (Object.prototype.hasOwnProperty.call(xtoken, key)) {
            const element = xtoken[key];
            xhr.setRequestHeader(key, element);
          }
        }
      }
      xhr.responseType = 'blob';
      xhr.onload = () => {
        if (xhr.status === 200) {
          resolve(xhr.response);
        }
      };
      xhr.send(JSON.stringify(data));
    });
  }
  private saveAs(blob: any, filename: string) {
    const link = document.createElement('a');
    link.href = window.URL.createObjectURL(blob);
    if (filename.endsWith('.xlsx') || filename.indexOf('.') > 0) link.download = filename
    else link.download = filename + '.xlsx';
    link.click();
  }
  handleDownloadFile = async (file: NzUploadFile): Promise<void> => {
    if (file?.originFileObj) {
      window.open(URL.createObjectURL(file?.originFileObj), '_blank');
    } else {
      this.download({
        fileUrl: file['fileUrl'],
        fileName: file['fileName'],
      }, file['fileName']);
    }
  };

  /**
 * 从数组中删除指定元素
 * @param {Array} arr - 要操作的数组
 * @param {*} item - 要删除的元素或比较基准值
 * @param {Function} [matcher] - 可选的匹配函数，接收(a, b)两个参数，返回布尔值表示是否匹配
 * @returns {Array} 删除元素后的新数组（原数组不会被修改）
 */
  remove(arr: Array<any>, item: any, matcher?: (a: any, b: any) => boolean): Array<any> {
    // 如果提供了匹配函数，则使用它进行判断
    if (typeof matcher === 'function') {
      return arr.filter(element => !matcher(element, item));
    }
    // 默认使用严格相等匹配
    return arr.filter(element => element !== item);
  }
  /**
 * 将文件大小（字节数）转换为合适的单位（B、KB、MB、GB等）
 * @param {number} fileSize - 文件大小，单位为字节（B）
 * @param {number} [decimalPlaces=2] - 保留的小数位数，默认2位
 * @returns {string} 格式化后的文件大小字符串，如 "1.23 MB"
 */
  formatFileSize(fileSize: number, decimalPlaces = 2) {
    // 验证输入是否为有效数字
    if (typeof fileSize !== 'number' || isNaN(fileSize) || fileSize < 0) {
      return '0 B';
    }

    // 定义单位数组，从B开始
    const units = ['B', 'KB', 'MB', 'GB', 'TB'];

    // 计算应该使用的单位索引
    let unitIndex = 0;
    while (fileSize >= 1024 && unitIndex < units.length - 1) {
      fileSize /= 1024;
      unitIndex++;
    }

    // 格式化数字并拼接单位
    return `${fileSize.toFixed(decimalPlaces)} ${units[unitIndex]}`;
  }
  //获取错误详细内容
  getFailMsg(res: any) {
    if (!res.success) {
      if (res.data && typeof (res.data) == "object") {
        var msg = res.msg + " \n";
        for (var key in res.data) {
          msg += key + ":" + res.data[key] + "\n";
        }
        return msg;
      }
      return res.msg;
    }
  }
  getUrlParams(url: string) {
    let obj: any = {};
    if (url.indexOf('?') < 0) return obj;
    let urlStr = url.split('?')[1];
    let paramsArr = urlStr.split('&');
    for (let i = 0, len = paramsArr.length; i < len; i++) {
      let arr = paramsArr[i].split('=');
      obj[arr[0].toLowerCase()] = arr[1];
    }
    return obj;
  }
  /**
   * 对数组进行分页处理
   * @param {Array} array - 要分页的原始数组
   * @param {number} pageNum - 页码（从1开始）
   * @param {number} pageSize - 每页显示的条数
   * @returns {Array} 分页后的数组
   */
  pageArray(array: any[], pageNum: number, pageSize: number) {

    // 验证参数
    if (!Array.isArray(array)) {
      throw new Error('第一个参数必须是数组');
    }
    if (!Number.isInteger(pageNum) || pageNum < 1) {
      throw new Error('页码必须是大于等于1的整数');
    }
    if (!Number.isInteger(pageSize) || pageSize < 1) {
      throw new Error('每页条数必须是大于等于1的整数');
    }

    // 计算起始索引和结束索引
    const startIndex = (pageNum - 1) * pageSize;
    const endIndex = startIndex + pageSize;

    // 返回分页后的子数组
    return array.slice(startIndex, endIndex);
  }


  public CallSqlQueryApi(url: string, sql?: string, pageIndex: number = 1, pageSize: number = 5000) {
    const header: any = { 'x-token': '77373d7e-ec37-4c61-9071-3c2d58966133' }
    let data = {
      "sqlText": sql,
      "pageIndex": pageIndex,
      "pageSize": pageSize,
      "total": 1
    }
    return this.http.request({
      url: environment.BasicServer + url,//
      // url: environment.BasicServer + '/api/clients/list',
      method: 'POST',
      data,
    }, header);
  }
  public DashbordCallSqlQueryApi(url: string, sql?: string, pageIndex: number = 1, pageSize: number = 5000) {
    const header: any = { 'x-token': '77373d7e-ec37-4c61-9071-3c2d58966133' }
    let data = {
      "sqlText": sql,
      "pageNum": pageIndex,
      "pageSize": pageSize,
      "total": 0
    }
    return this.http.request({
      url: environment.BasicServer + url,//
      // url: environment.BasicServer + '/api/clients/list',
      method: 'POST',
      data,
    }, header);
  }

}
