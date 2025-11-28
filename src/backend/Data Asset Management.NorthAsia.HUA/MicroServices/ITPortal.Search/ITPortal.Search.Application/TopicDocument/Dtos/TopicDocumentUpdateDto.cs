using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Search.Application.TopicDocument.Dtos
{
    public class TopicDocumentUpdateDto
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
        public List<string> Keyword { get; set; }

        /// <summary>
        /// 分类
        /// </summary>
        public List<string> Classify { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        public List<string> Tags { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 额外数据
        /// </summary>
        public TopicDocumentPayloadDto Payload { get; set; }


        /// <summary>
        /// 关联URL
        /// </summary>
        public string linkUrl { get; set; }

        /// <summary>
        /// 附件
        /// </summary>
        public List<TopicDocumentAttachmentDto> AttachmentList { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        public string Operator { get; set; }

        /// <summary>
        /// 开启数据主权
        /// </summary>
        public bool? EnableDataSovereignty { get; set; }

    }
}
