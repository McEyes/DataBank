using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTopicStore.Core.Helpers;
using DataTopicStore.Core.Models;
using Microsoft.AspNetCore.Http;

public static class HttpContextExtensions
{
    public static UserInfo? GetCurrUserInfo(this HttpContext context)
    {
        if (context != null)
        {
            if (!context.Items.ContainsKey(ConstantHelper.UserInfoKey))
                context.Items.Add(ConstantHelper.UserInfoKey, new UserInfo(context.User));
            return context.Items[ConstantHelper.UserInfoKey] as UserInfo;
        }
        else return null;
    }
}
