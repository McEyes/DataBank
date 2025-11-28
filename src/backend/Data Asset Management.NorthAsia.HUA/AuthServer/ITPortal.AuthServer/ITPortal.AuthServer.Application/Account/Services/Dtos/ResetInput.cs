using StackExchange.Profiling.Internal;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.AuthServer.Application.Account.Services.Dtos
{

    /// <summary>
    /// 登录
    /// </summary>
    public class ResetInput
    {
        /// <summary>
        /// 用户名或者邮箱
        /// </summary>
        [Required(ErrorMessage = "Email can not be null")]
        public string Name { get; set; }


        /// <summary>
        /// 密码
        /// </summary>
        [Required(ErrorMessage = "Password can not be null")]
        public string Password { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [Required(ErrorMessage = "New Password can not be null")]
        public string NewPassword { get; set; }

    }
}
