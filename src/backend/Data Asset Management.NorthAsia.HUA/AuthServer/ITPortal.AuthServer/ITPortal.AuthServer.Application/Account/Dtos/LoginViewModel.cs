using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.AuthServer.Application.Account.Dtos
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "请输入用户名")]
        public string Username { get; set; }

        [Required(ErrorMessage = "请输入密码")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
