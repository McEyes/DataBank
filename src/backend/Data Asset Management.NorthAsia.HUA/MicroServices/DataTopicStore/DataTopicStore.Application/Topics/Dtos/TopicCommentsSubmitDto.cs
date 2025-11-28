using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTopicStore.Application.Topics.Dtos
{
    public class TopicCommentsSubmitDto
    {
        public long topic_id { get; set; }
        public string content { get; set; }
    }
}
