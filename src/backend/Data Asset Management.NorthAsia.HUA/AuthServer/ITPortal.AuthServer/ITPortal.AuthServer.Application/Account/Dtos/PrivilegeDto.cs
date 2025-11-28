using ITPortal.Core.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.AuthServer.Application.Account.Dtos
{


    public class PrivilegeDto
    {
        public string Id { get; set; }

        public string code { get; set; }
        public string Name { get; set; }

        public string ParentId { get; set; }


        public string Component { get; set; }

        /// <summary>
        /// 跳转路径
        /// </summary>
        public string RedirectUrl { get; set; }

        public string RedirectPerms { get; set; }


        public string Icon { get; set; }
        public int? Type { get; set; }

        public int? IsHidden { get; set; }
        public int? Sort { get; set; }

        public int? Status { get; set; }
    }
}
