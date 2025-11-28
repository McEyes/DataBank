using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Search.Application.TopicDocument.Dtos
{
    public class GroupByTopicDocumentDto
    {

        public string Topic { get; set; }

        public List<TopicDocumentDto> Documents { get; set; } = new();

    }
}
