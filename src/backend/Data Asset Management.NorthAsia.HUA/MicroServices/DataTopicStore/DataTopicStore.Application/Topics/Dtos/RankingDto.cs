using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTopicStore.Core.Enums;

namespace DataTopicStore.Application.Topics.Dtos
{
    public class RankingDto
    {
        public int Count { get; set; }
        public TopicRankingType RankingType { get; set; }
    }
}
