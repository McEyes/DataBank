
namespace DataTopicStore.Application.Topics.Dtos
{
    public class SetVerificationFailureDto
    {
        public long TopicId { get; set; }
        public string Reason { get; set; }
    }
}
