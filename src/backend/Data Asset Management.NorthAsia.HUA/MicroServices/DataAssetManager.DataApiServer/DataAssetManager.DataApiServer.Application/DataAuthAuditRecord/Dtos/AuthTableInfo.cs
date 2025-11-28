using ITPortal.Core.Services;

using Microsoft.Extensions.FileSystemGlobbing.Internal;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Npgsql.TypeHandlers.DateTimeHandlers;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAssetManager.DataApiServer.Application.DataApi.Dtos
{
    /// <summary>
    /// 数据库表信息表
    /// [{"ctlName":"测试表别名","tableId":"1777542944465113090","ctlId":"1777595862713745410"}]
    /// </summary>
    [Serializable]
    public class AuthTableInfo
    {

        /// <summary>
        /// 审核结果
        /// </summary>
        public string ctlId;
        /// <summary>
        /// 申请id入参
        /// </summary>
        public string ctlName;

        /// <summary>
        /// 备注
        /// </summary>
        public string tableId;
    }
}
