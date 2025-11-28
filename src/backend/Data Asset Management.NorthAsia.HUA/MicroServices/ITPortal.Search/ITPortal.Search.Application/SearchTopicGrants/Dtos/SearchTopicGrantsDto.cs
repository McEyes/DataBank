using ITPortal.Core.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Search.Application.SearchTopicGrants.Dtos
{
    public class SearchTopicGrantsDto:PageEntity<Guid>
    {
        public string RoleId { get; set; }

        public dynamic[] TopicIds { get; set; } = new dynamic[0];
    }
}
