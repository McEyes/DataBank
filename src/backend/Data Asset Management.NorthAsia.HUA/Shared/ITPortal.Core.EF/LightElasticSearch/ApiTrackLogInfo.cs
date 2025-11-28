using Furion.DatabaseAccessor;

using ITPortal.Core.Services;


using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Core.LightElasticSearch
{
    [ElasticIndexName("ApiTrackLogInfo", "ITPortal")]
    [Table( "asset_track_api_log")]
    public class ApiTrackLogInfo : EntityBase<Guid>
    {
        [Column("id")]
        public override Guid Id { get => base.Id; set => base.Id = value; }


        [Column( "api_action")]
        public string ApiAction { get; set; }

        [Column( "method")]
        public string Method { get; set; }

        [Column( "path")]
        public string Path { get; set; }

        [Column( "user_id")]
        public string UserId { get; set; }

        [Column( "client_ip")]
        public string ClientIp { get; set; }

        [Column( "request_parameters")]
        public string RequestParameters { get; set; }

        [Column( "caller_date")]
        public DateTimeOffset CallerDate { get; set; } = DateTimeOffset.Now;

        //[Column( "component_trackings")]
        //public List<ComponentTrackingRecord> ComponentTrackings { get; set; }=new List<ComponentTrackingRecord>();

        [Column( "times")]
        public long ElapsedMilliseconds { get; set; }

        [Column( "msg")]
        public string Msg { get; set; }

        [Column( "status_code")]
        public int? StatusCode { get; set; }

        [Column( "success")]
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
