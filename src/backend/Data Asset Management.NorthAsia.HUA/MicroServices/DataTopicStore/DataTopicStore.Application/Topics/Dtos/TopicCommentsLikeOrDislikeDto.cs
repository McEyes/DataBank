using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTopicStore.Core.Enums;

namespace DataTopicStore.Application.Topics.Dtos
{
    public class TopicCommentsLikeOrDislikeDto
    {
        public Guid comments_id { get; set; }
        public CancelLikesType like_type { get; set; }
    }
}
