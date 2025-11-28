using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTopicStore.Application.Common.Dtos;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DataTopicStore.Application.Topics.Dtos
{
    public class SearchTopicDraftDto : PagingBase
    {
        public Guid? CategoryId { get; set; }
        public string TopicName { get; set; }
    }
}
