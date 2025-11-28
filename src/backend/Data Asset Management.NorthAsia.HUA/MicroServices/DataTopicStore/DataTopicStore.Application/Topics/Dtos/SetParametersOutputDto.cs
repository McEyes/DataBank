
using DataTopicStore.Core.Models;

namespace DataTopicStore.Application.Topics.Dtos
{
    public class SetParametersOutputDto
    {
        public long TopicId { get; set; }
        public ParametersOutputSettingModel OutputSetting { get; set; }
    }
}
