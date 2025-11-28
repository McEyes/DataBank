using ITPortal.Core.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.AuthServer.Application.Account.Dtos
{


    public class RoleDto : PageEntity<string>
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsDefault { get; set; }
        public bool IsStatic { get; set; }
        public bool IsPublic { get; set; }
    }
}
