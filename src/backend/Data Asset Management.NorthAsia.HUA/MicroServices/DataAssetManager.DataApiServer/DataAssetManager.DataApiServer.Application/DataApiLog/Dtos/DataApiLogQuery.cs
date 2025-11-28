using ITPortal.Core.Services;
using DataAssetManager.DataApiServer.Core;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Xml;
using StackExchange.Profiling.Internal;

namespace DataAssetManager.DataApiServer.Application.DataApi.Dtos
{
    public class DataApiLogQuery : PageEntity<string>
    {

        public string ApiId { get; set; }

        public string ApiName { get; set; }

        public string CallerId { get; set; }

        public string CallerIp { get; set; }

        public string CallerUrl { get; set; }

        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }

        public int? CallerSize { get; set; }

        public int? Time { get; set; }

        public string Msg { get; set; }

        public int? Status { get; set; }
        public string Owner { get;  set; }
        public string OwnerName { get;  set; }
        public string OwnerDepart { get;  set; }
        public string CallerName { get;  set; }
    }

}