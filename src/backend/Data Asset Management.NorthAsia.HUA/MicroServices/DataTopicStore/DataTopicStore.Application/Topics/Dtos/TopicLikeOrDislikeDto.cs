using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTopicStore.Core.Enums;

namespace DataTopicStore.Application.Topics.Dtos
{
    public class TopicLikeOrDislikeDto
    {
        public long topic_id { get; set; }
        public LikesType like_type { get; set; }
    }
}
