using ITPortal.Core.Exceptions;

using Microsoft.AspNetCore.Http;

using System.Collections.Generic;
using System.Linq;
namespace DataAssetManager.DataApiServer.Core
{
    public class RequestUtils
    {
        public static (bool flag,RequestParam param) Validate(HttpRequest request, List<RequestParam> reqParams)
        {
            foreach (var param in reqParams)
            {
                if (!param.Nullable)
                {
                    var paramValue = request.Query[param.ParamName].ToString();
                    if (string.IsNullOrEmpty(paramValue))
                        return (false, param); // 参数为必填但未提供
                }
            }
            return (true,null);
        }


        public static (bool flag, RequestParam param) Validate(IDictionary<string, object> paramsData, List<RequestParam> reqParams)
        {
            foreach (var param in reqParams)
            {
                if (!param.Nullable)
                {
                    if (paramsData.TryGetValue(param.ParamName, out object paramValue))
                    {
                        if (string.IsNullOrEmpty(paramValue?.ToString()))
                            return (false, param);
                    }
                    else
                        return (false, param);
                }
            }
            return (true, null);
        }


        public static (int pageIndex, int pageSize, int sqlOffset) GetPageInfo(Dictionary<string, object> paramsData, int? maxSizeLimit)
        {
            int pageNum = 1, pageSize = 0, sqlOffset = 0, total = 0;
            if (maxSizeLimit < 0) maxSizeLimit = 0;
            if (int.TryParse(paramsData["pageNum"]?.ToString(), out pageNum) && pageNum <= 0) pageNum = 1;
            int.TryParse(paramsData["pageSize"]?.ToString(), out pageSize);// 如果没有该参数，使用 maxSize
            if (pageSize > maxSizeLimit && maxSizeLimit > 0) pageSize = maxSizeLimit.Value;
            else if (pageSize <= 0) pageSize = 5000;
            if (pageNum > 1)
            {
                if (int.TryParse(paramsData["total"]?.ToString(), out total))
                {
                    sqlOffset = total - ((pageNum - 1) * pageSize);
                    //if (sqlOffset < 0) throw new DataQueryException("Out of range of current page");
                }
                else
                {
                    throw new DataQueryException($"Requires the parameter [total] to be passed in:{paramsData["total"]}");
                }
            }
            paramsData.Remove("pageNum");
            paramsData.Remove("pageSize");
            paramsData.Remove("total");
            return (pageNum, pageSize, sqlOffset);
        }

    }
}