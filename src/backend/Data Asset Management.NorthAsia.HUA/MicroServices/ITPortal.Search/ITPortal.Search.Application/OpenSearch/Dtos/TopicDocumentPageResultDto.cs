using ITPortal.Core.Services;
using ITPortal.Search.Application.SearchTopic.Dtos;
using ITPortal.Search.Application.TopicDocument.Dtos;

using Microsoft.AspNetCore.Mvc.RazorPages;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Search.Application.OpenSearch.Dtos
{
    public class TopicDocumentPageResultDto:PageResult<TopicDocumentDto>
    {
        public List<SearchTopicDto> Topics { get; set; } = new();
    }
}
