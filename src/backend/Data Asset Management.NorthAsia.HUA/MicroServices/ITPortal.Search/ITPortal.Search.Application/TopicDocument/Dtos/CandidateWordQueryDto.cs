using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Search.Application.TopicDocument.Dtos
{
    public class CandidateWordQueryDto
    {

        public string Keyword { get; set; }

        public int MaxLenght { get; set; } = 10;

    }
}
