using ITPortal.Core.Services;
using DataAssetManager.DataApiServer.Core;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Xml;
using StackExchange.Profiling.Internal;

namespace DataAssetManager.DataApiServer.Application.DataApi.Dtos
{
    /// <summary>
    /// 问题反馈问题
    /// </summary>
    public class FeedbackInputDto 
    {
        /// <summary>
        /// 问题反馈主题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 反馈内容
        /// </summary>
        [Required(ErrorMessage = "反馈问题描述不能为空", AllowEmptyStrings = false)]
        public string Description { get; set; }

        public string Files { get; set; }

        //public string UserId { get; set; }

        //public string UserName { get; set; }

        //public string UserEmail { get; set; }

        //public string UserDept { get; set; }

        //public string Status { get; set; }
        /// <summary>
        /// 反馈对象ObjectId，当前固定为tableid
        /// </summary>
        [Required(ErrorMessage = "反馈对象ObjectId不能为空", AllowEmptyStrings = false)]
        public string ObjectId { get; set; }

        //public string ObjectType { get; set; }
        /// <summary>
        /// 反馈对象ObjectName
        /// </summary>
        [Required(ErrorMessage = "反馈对象ObjectName不能为空", AllowEmptyStrings = false)]
        public string ObjectName { get; set; }

        //public string OwnerId { get; set; }

        //public string OwnerName { get; set; }

        //public string OwnerEmail { get; set; }



    }

}