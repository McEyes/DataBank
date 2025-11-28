using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Search.Application.TopicDocument.Dtos
{
    public class TopicDocumentPayloadDto
    {
        public List<TopicDocumentButtonCreateDto> Buttons { get; set; } = new();

    }
}
