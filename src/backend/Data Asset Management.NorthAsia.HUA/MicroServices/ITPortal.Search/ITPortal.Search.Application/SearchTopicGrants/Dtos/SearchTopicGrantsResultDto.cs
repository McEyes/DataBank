using ITPortal.Search.Application.SearchTopic.Dtos;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Search.Application.SearchTopicGrants.Dtos
{
    public class SearchTopicGrantsResultDto : SearchTopicDto
    {

        /// <summary>
        /// 已授权
        /// </summary>
        public bool IsGrant { get; set; }

    }
}
