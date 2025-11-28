
namespace DataTopicStore.Application.Topics.Dtos
{
    public class TopicCommentListItemDto
    {
        public Guid id { get; set; }

        public long topic_id { get; set; }

        public string content { get; set; }

        public bool? is_liked { get; set; }

        public int liked_count { get; set; }
        public int disliked_count { get; set; }

        public DateTimeOffset created_time { get; set; }
        public string created_by { get; set; }
        public string reviewer { get; set; }
    }
}
