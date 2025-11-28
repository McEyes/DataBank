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
    /// 审批结果
    /// </summary>
    [Serializable]
    public class AuthResultDto
    {
        /// <summary>
        /// 流程id
        /// </summary>
        [Description("流程id")]
        public string ApplyFormId;

        /// <summary>
        /// 审批说明，批注
        /// </summary>
        [Description("备注")]
        public string Remark;

        /// <summary>
        /// 审批结果：通过，拒绝
        /// </summary>
        [Description("审核结果")]
        public string Result;
    }
}
