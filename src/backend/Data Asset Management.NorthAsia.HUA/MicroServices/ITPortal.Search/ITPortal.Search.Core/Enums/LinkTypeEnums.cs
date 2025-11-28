using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Search.Core.Enums
{
    public enum LinkTypeEnums
    {

        /// <summary>
        /// 链接
        /// </summary>
        LinkUrl = 0,

        /// <summary>
        /// HTML内容
        /// </summary>
        Content = 1,

        /// <summary>
        /// 单元列
        /// </summary>
        CellItems = 2,

        /// <summary>
        /// 表单提交
        /// </summary>
        Form = 3,

        /// <summary>
        /// 回调
        /// </summary>
        Callback = 4,

        /// <summary>
        /// 确认模态窗口
        /// </summary>
        ConfirmModal = 5,

    }
}
