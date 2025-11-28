using ITPortal.Core.Services;
using DataAssetManager.DataApiServer.Core;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITPortal.Core;

namespace DataAssetManager.DataApiServer.Application.Services
{
    public interface IApiServices
    {
        Task<PageResult<object>> Execute(HttpRequest request, RouteInfo apiInfo, Dictionary<String, Object> paramsData = null);
        Task<PageResult> ExecuteClass(HttpRequest request, RouteInfo apiInfo, Dictionary<String, Object> paramsData = null);
        /// <summary>
        /// "to_excel":"1"// 必填，且必须为字符串
        /// "excel_name"："下载文件名"// 为空时，默认api名称
        /// "column_info":"格式化配置：  列名1=列导出名称1,字符串格式;列名2=列导出名称2;列名3;" // ;分号分隔列，=号列名和显示名映射配置，逗号(,)后面时字符串格式化配置
        /// </summary>
        /// <param name="request"></param>
        /// <param name="apiInfo"></param>
        /// <param name="paramsData"></param>
        /// <returns></returns>
        Task<IActionResult> ExecuteToExcel(HttpRequest request, RouteInfo apiInfo, Dictionary<String, Object> paramsData = null);
        //PageResult<T> Query<T>(HttpRequest request, RouteInfo apiInfo, Dictionary<String, Object> paramList) where T : class, new();
        //T Insert<T>(HttpRequest request, RouteInfo apiInfo,T data);
        //T Modify<T>(HttpRequest request, RouteInfo apiInfo, T data);
        //T Delete<T>(HttpRequest request, RouteInfo apiInfo, T data);
    }
}
