using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Core.ProxyApi.Dto
{
    public class SearchTopicDto
    {
        // 业务ID(必填)：由调用者提供，比如订单ID，后续对文档的更改用以此ID作为主键
        public string id { get; set; }

        // 主题标识(必填)：由“搜索主题”定义的主题标识
        public string topic { get; set; }

        // 标题(必填)：用于检索结果标题
        public string title { get; set; }

        // 描述(必填)：用于检索结果描述
        public string description { get; set; }

        // 关键字(非必填)：作为候选词，为空则自动进行分词生成
        public List<string> keyword { get; set; } = new List<string>();

        // 分类(非必填)：用于分类查找
        public List<string> classify { get; set; } = new List<string>();

        // 标签(非必填)：用于标签推荐
        public List<string> tags { get; set; } = new List<string>();

        // 内容(必填)：全文检索的主要内容，不限制格式，推荐为html内容
        public string content { get; set; }

        // 负荷数据(非必填)，额外载荷数据
        public object payload { get; set; } = new { };

        // 跳转链接(非必填)：用于链接到其他页面，为空则展示content数据内容
        public string linkUrl { get; set; } = string.Empty;

        /// <summary>
        /// 跳转方式，为空时表示内部连接，open
        /// </summary>
        public string linkType { get; set; } = string.Empty;

        // 附件列表(非必填)
        public List<SearchTopicAttachment> attachmentList { get; set; } = new List<SearchTopicAttachment>();

        // 操作人(必填)：传入UserID，公开数据则为空
        public string Operator { get; set; } = string.Empty;

        // 数据主权(必填)：如果开启则仅Operator能查询到该数据
        public bool enableDataSovereignty { get; set; }

    }

    public class SearchTopicAttachment
    {
        public string fileName { get; set; }

        public string objectName { get; set; }

        public string fileUrl { get; set; }
    }
}
