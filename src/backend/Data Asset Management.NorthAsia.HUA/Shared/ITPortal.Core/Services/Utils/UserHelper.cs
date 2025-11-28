using ITPortal.Core;
using ITPortal.Core.Services;
using ITPortal.Extension.System;

using Microsoft.AspNetCore.Http;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Core
{
    public static class UserHelper
    {
        public static UserInfo? GetCurrUserInfo(this HttpContext context)
        {
            if (context != null && (context.Items.ContainsKey(DataAssetManagerConst.HttpContext_UserInfo) || context.User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier).IsNotNullOrWhiteSpace()))
            {
                if (!context.Items.ContainsKey(DataAssetManagerConst.HttpContext_UserInfo))
                    context.Items.Add(DataAssetManagerConst.HttpContext_UserInfo, new UserInfo(context.User));
                //else context.Items[DataAssetManagerConst.HttpContext_UserInfo] = new UserInfo(context.User);
                var userInfo = context.Items[DataAssetManagerConst.HttpContext_UserInfo] as UserInfo;
                if (userInfo.Id.IsNullOrWhiteSpace())
                {
                    context.Items[DataAssetManagerConst.HttpContext_UserInfo] = userInfo = new UserInfo(context.User);
                }
                return userInfo;
            }
            else return new UserInfo();
        }
    }
}
