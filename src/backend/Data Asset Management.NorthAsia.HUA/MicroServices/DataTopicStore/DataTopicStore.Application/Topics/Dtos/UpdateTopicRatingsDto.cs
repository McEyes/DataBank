

namespace DataTopicStore.Application.Topics.Dtos
{
    public class UpdateTopicRatingsDto
    {
        public long id { get; set; }
        public decimal ratings { get; set; }
        public int ratings_count { get; set; }
    }
}
