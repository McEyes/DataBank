using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Search.Application.OpenSearch.Dtos
{
    public class GroupByTopicDocumentQueryDto
    {

        public string Keyword { get; set; }

        public List<string> Classify { get; set; }

        public int MaxDocumentCount { get; set; } = 3;

        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 10;

    }
}
