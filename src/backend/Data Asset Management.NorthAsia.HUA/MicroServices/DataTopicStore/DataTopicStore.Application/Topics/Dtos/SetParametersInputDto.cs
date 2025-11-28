using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTopicStore.Core.Models;

namespace DataTopicStore.Application.Topics.Dtos
{
    public class SetParametersInputDto
    {
        public long TopicId { get; set; }
        public ParametersInputSettingModel InputSetting { get; set; }
    }
}
