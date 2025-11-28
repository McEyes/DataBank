using ITPortal.Core;
using ITPortal.Core.Services;
using DataAssetManager.DataApiServer.Core;

using Microsoft.Extensions.FileSystemGlobbing.Internal;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Npgsql.TypeHandlers.DateTimeHandlers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace DataAssetManager.DataApiServer.Application.DataApi.Dtos
{
    /// <summary>
    /// 数据API信息表 实体DTO
    /// </summary>
    public class SqlDto
    {
        public string SourceId { get; set; }
        public string SqlKey { get; set; }
        public string SqlText { get; set; }

    }
}
