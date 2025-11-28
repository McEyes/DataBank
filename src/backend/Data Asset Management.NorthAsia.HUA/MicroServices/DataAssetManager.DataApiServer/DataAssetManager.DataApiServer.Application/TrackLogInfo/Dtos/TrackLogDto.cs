using ITPortal.Core.Services;
using DataAssetManager.DataApiServer.Core;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Xml;
using StackExchange.Profiling.Internal;
using ITPortal.Core.LightElasticSearch;

namespace DataAssetManager.DataApiServer.Application.DataApi.Dtos
{
    public class TrackLogDto : PageEntity<Guid>
    {

        [SugarColumn(ColumnName = "api_action")]
        public string ApiAction { get; set; }

        [SugarColumn(ColumnName = "method")]
        public string Method { get; set; }

        [SugarColumn(ColumnName = "path")]
        public string Path { get; set; }

        [SugarColumn(ColumnName = "user_id")]
        public string UserId { get; set; }

        [SugarColumn(ColumnName = "client_ip")]
        public string ClientIp { get; set; }

        [SugarColumn(ColumnName = "request_parameters")]
        public string RequestParameters { get; set; }

        [SugarColumn(ColumnName = "caller_date")]
        public DateTimeOffset CallerDate { get; set; } = DateTimeOffset.Now;

        [SugarColumn(ColumnName = "timestamp")]
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.Now;

        [SugarColumn(ColumnName = "component_trackings", IsJson = true)]
        public List<ComponentTrackingRecord> ComponentTrackings { get; set; }

        [SugarColumn(ColumnName = "times")]
        public long ElapsedMilliseconds { get; set; }

        [SugarColumn(ColumnName = "msg")]
        public string Msg { get; set; }

        [SugarColumn(ColumnName = "status_code")]
        public int? StatusCode { get; set; }
    }

}