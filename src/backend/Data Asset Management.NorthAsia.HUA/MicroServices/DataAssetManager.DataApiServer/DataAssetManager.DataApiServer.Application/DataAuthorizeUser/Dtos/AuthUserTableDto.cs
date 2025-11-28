using ITPortal.Core.Services;

using Microsoft.Extensions.FileSystemGlobbing.Internal;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Npgsql.TypeHandlers.DateTimeHandlers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAssetManager.DataApiServer.Application.DataApi.Dtos
{
    /// <summary>
    /// 数据库表信息表
    /// </summary>
    [Serializable]
    public class AuthUserTableDto
    {
        /// <summary>
        /// 表操作规则配置
        /// </summary>
        public List<AuthTable>? Node { get; set; }

        /// <summary>
        /// 表名
        /// </summary>
        public List<AuthUser>? UserList { get; set; }

        public string Type { get; set; }
    }

    public class AuthTable
    {
       public string Key { get; set; }
        public string PId { get; set; }
        public string Type { get; set; }
//:"1764856621550272513"
//pId:"1764841504850837505"
//type: "catagroy"
    }

    public class AuthUser
    {
        public string Id { get; set; }
        public string Username { get; set; }
    }
}
