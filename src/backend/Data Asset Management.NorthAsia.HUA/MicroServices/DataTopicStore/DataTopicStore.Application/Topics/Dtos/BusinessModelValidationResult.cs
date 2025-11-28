using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTopicStore.Application.Topics.Dtos
{
    public class BusinessModelValidationResultDto
    {
        public long topic_id { get; set; }
        public BusinessModelValidationResult Result { get; set; }
    }
    public class BusinessModelValidationResult
    {
        public BusinessModelValidationResult() { }
        public object Data { get; set; }
    }
}
