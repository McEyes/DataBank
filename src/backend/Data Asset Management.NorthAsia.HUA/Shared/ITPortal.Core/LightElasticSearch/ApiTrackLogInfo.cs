using ITPortal.Core.Services;
using SqlSugar;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Core.LightElasticSearch
{
    [ElasticIndexName("ApiTrackLogInfo", "ITPortal")]
    [SugarTable(TableName = "asset_track_api_log")]
    public class ApiTrackLogInfo : Entity<Guid>
    {
        [SugarColumn(IsPrimaryKey = true, ColumnName = "id")]
        public override Guid Id { get; set; }


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

        [SugarColumn(ColumnName = "component_trackings", IsJson = true)]
        public List<ComponentTrackingRecord> ComponentTrackings { get; set; }=new List<ComponentTrackingRecord>();

        [SugarColumn(ColumnName = "times")]
        public long ElapsedMilliseconds { get; set; }

        [SugarColumn(ColumnName = "msg")]
        public string Msg { get; set; }

        [SugarColumn(ColumnName = "status_code")]
        public int? StatusCode { get; set; }

        [SugarColumn(ColumnName = "success")]
        public bool? Success { get; set; }

        //public DateTime RequestStartTime { get; set; } = DateTime.Now;
        //public DateTime RequestEndTime { get; set; } = DateTime.Now;
        //public long TotalExecutionTimeMs => (RequestEndTime - RequestStartTime).Milliseconds;

    }


    public class ComponentTrackingRecord
    {
        public string ComponentName { get; set; }
        public string Phase { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public long ElapsedMilliseconds { get; set; }
        public bool? Success { get; set; }
        public string ErrorMessage { get; set; }
    }
}
