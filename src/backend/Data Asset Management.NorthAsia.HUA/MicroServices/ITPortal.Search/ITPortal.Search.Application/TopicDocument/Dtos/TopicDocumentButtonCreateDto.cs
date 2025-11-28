using ITPortal.Search.Core.Enums;

namespace ITPortal.Search.Application.TopicDocument.Dtos
{
    public class TopicDocumentButtonCreateDto
    {

        /// <summary>
        /// 文字
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 背景颜色
        /// </summary>
        public string Background { get; set; } = "#FFF";

        /// <summary>
        /// 文字颜色
        /// </summary>
        public string TextColor { get; set; } = "#333";

        /// <summary>
        /// 边框颜色
        /// </summary>
        public string BorderColor { get; set; } = "#F2F2F2";

        /// <summary>
        /// 链接类型
        /// </summary>
        public LinkTypeEnums LinkType { get; set; }

        /// <summary>
        /// 链接
        /// </summary>
        public string LinkUrl { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 单元列信息
        /// </summary>
        public IDictionary<string, object> CellItems { get; set; }


        /// <summary>
        /// 确认窗口参数
        /// </summary>
        public ConfirmModalParamsDto ConfirmModalParams { get; set; }

        /// <summary>
        /// 表单ID
        /// </summary>
        public Guid? FormId { get; set; }


        /// <summary>
        /// 回调链接
        /// </summary>
        public string CallbackUrl { get; set; }

        /// <summary>
        /// 回调方法
        /// </summary>
        public string CallbackMethod { get; set; }

        /// <summary>
        /// 默认参数
        /// </summary>
        public IDictionary<string, object> Parameters { get; set; }

        /// <summary>
        /// 拥有权限的用户
        /// </summary>
        public List<string> HasPermissionUserIds { get; set; } = new();

        /// <summary>
        /// 所在workcell用户有权限
        /// </summary>
        public List<string> HasPermissionWorkcells { get; set; } = new();

    }
}
