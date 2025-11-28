using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTopicStore.Core.Enums;

namespace DataTopicStore.Application.Topics.Dtos
{
    public class TopicDraftItemDto
    {
        public long id { get; set; }
        public string name { get; set; }
        public string cover { get; set; }
        public string version { get; set; }
        public string description { get; set; }
        public string content { get; set; }
        public string json_formula { get; set; }
        public string author_id { get; set; }
        public string author { get; set; }
        public DateTimeOffset created_time { get; set; }
        public DateTimeOffset? updated_time { get; set; }
        public string updated_by { get; set; }
        public string json_parameters_input { get; set; }
        public string json_parameters_output { get; set; }
        public string sql_scripts { get; set; }
        public Guid? category_id { get; set; }
        public ProgressStatus progress { get; set; }
        public bool is_verification_passed { get; set; }
        public bool is_released { get; set; }
        public string verification_failure_reason { get; set; }
       
    }
}
