using ITPortal.Core.Services;
using DataAssetManager.DataApiServer.Application.DataApi.Dtos;

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

namespace DataAssetManager.DataApiServer.Application.DataTable.Dtos
{
    /// <summary>
    /// 3 Confidential, 2 Internal Use, 1 Public, 4 Restricted Confidential
    /// 3机密,2	内部使用级,1	公众级,4	受限机密
    /// </summary>
    public enum SecurityLevel
    {
        [Description("公众级")]
        Public=1,
        [Description("内部使用级")]
        InternalUse =2,
        [Description("机密")]
        Confidential =3,
        [Description("受限机密")]
        RestrictedConfidential =4

    }

}
