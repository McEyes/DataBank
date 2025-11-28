

using DataTopicStore.Core.Enums;

namespace DataTopicStore.Application.Topics.Dtos
{
    public class TopicListItemDto
    {
        public long id { get; set; }

        public string name { get; set; }

        public string cover { get; set; }

        public string version { get; set; }
        public ApprovalStatus status { get; set; }
        public string remark { get; set; }

        public string description { get; set; }

        public string author_id { get; set; }
        public string author { get; set; }
        public string owner_id { get; set; }
        public string owner { get; set; }
        public string owner_email { get; set; }

        public string department { get; set; }

        public decimal? ratings { get; set; }
        public DateTimeOffset created_time { get; set; }
        public string email { get; set; }
    }
}
