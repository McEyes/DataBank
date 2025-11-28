using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace DataTopicStore.Core.Entities
{
    [SugarTable("dts_topic_authorization_user")]
    public partial class TopicAuthorizationUserEntity
    {
        public TopicAuthorizationUserEntity() { }

        [SugarColumn(IsPrimaryKey = true)]
        public long topic_id { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>           
        [SugarColumn(IsPrimaryKey = true)]
        public string user_id { get; set; }

        public string user_display_name { get; set; }
        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>           
        public DateTimeOffset created_time { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public DateTimeOffset? updated_time { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string created_by { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string updated_by { get; set; }
    }
}
