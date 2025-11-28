using ITPortal.Core.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Search.Application.SearchTopic.Dtos
{
    public class SearchTopicDto:PageEntity<Guid>
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Topic
        /// </summary>
        public string Topic { get; set; }

        /// <summary>
        /// 是否为公开主题
        /// </summary>
        public bool? IsPublic { get; set; }

        /// <summary>
        /// 启用
        /// </summary>
        public bool? IsEnable { get; set; }

    }
}
