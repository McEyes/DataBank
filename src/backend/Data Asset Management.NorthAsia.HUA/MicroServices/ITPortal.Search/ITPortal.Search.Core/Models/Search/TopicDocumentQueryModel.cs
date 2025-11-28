using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Search.Core.Models.Search
{
    public class TopicDocumentQueryModel
    {
        public string Id { get; set; }

        public List<string> Topics { get; set; }

        public string Keyword { get; set; }

        public List<string> Classify { get; set; }

        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 10;

    }
}
