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
    public class SysDictDto:PageEntity<string>
    {


        public int? Status { get; set; }

        public string Remark { get; set; }

        public string DictId { get; set; }

        public string DictCode { get; set; }

        public string DictName { get; set; }
        public int? DictStatus { get; set; }

        public string DictRemark { get; set; }

        public string ItemText { get; set; }
        public string ItemTextEn { get; set; }

        public string ItemValue { get; set; }

        public string ItemData { get; set; }

        public int? ItemSort { get; set; }

    }
}
