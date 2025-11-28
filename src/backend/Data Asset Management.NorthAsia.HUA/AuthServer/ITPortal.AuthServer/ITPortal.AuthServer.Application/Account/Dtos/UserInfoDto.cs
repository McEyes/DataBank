using ITPortal.Core.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.AuthServer.Application.Account.Dtos
{


    public class UserInfoDto
    {
        /// <summary>
        /// 后续改成ntid
        /// </summary>
        public  string Id { get; set; }

        public string UserId { get; set; }
        /// <summary>
        /// user name 大多数=NTID
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        public string EmployeeName { get; set; }
        public string ChineseName { get; set; }
        public string EnglishName { get; set; }
        /// <summary>
        /// NTID
        /// </summary>
        public string Surname { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Department { get; set; }
    }
}
