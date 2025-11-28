using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Core.ProxyApi.Dto
{
    public class JabusUserInfo
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Ntid { get; set; }
        public string EmployeeName { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }

        public string Department { get; set; }
    }
}
