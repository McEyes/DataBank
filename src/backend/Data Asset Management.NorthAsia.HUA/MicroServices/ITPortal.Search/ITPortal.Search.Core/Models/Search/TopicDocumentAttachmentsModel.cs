using Nest;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Search.Core.Models.Search
{
    public class TopicDocumentAttachmentsModel
    {

        [Keyword]
        public string FileName { get; set; }

        [Keyword]
        public string ObjectName { get; set; }

        [Keyword]
        public string FileUrl { get; set; }

    }
}
