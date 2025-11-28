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
    /// 流程回写状�?
    /// </summary>
    public class DataGrantAuthBackInput
    {
        /// <summary>
        /// 流程表单id
        /// </summary>
        public string ApplyFormId { get; set; }
        /// <summary>
        /// 1通过,2拒绝
        /// </summary>
        public int Result { get; set; }
        public string Remark { get; set; }
        public string Token { get; set; }
    }
}
