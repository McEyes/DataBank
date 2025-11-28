using Nest;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Search.Application.TopicDocument.Dtos
{
    public class TopicDocumentDto
    {

        /// <summary>
        /// ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 主题
        /// </summary>
        public string Topic { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 关键字
        /// </summary>
        public List<string> Keyword { get; set; } = new List<string>();

        /// <summary>
        /// 分类
        /// </summary>
        public List<string> Classify { get; set; } = new List<string>();

        /// <summary>
        /// 标签
        /// </summary>
        public List<string> Tags { get; set; } = new List<string>();

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }


        //public IDictionary<string, object> Payload { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// 按钮组
        /// </summary>

        public TopicDocumentPayloadDto Payload { get; set; } = new TopicDocumentPayloadDto();

        /// <summary>
        /// 附件
        /// </summary>
        public List<TopicDocumentAttachmentDto> AttachmentList { get; set; } = new();

        /// <summary>
        /// 创建人
        /// </summary>
        public string Creator { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>
        public string Updater { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTimeOffset CreateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTimeOffset UpdateTime { get; set; }

        /// <summary>
        /// 开启数据主权
        /// </summary>
        public bool EnableDataSovereignty { get; set; }

        /// <summary>
        /// 关联URL
        /// </summary>
        public string linkUrl { get; set; }

        /// <summary>
        /// 高亮字典
        /// </summary>
        public Dictionary<string, string> Highlight { get; set; } = new Dictionary<string, string>();

    }
}
