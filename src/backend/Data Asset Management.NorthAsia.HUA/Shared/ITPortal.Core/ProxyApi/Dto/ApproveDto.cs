using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Core.ProxyApi.Dto
{

    public class ApproveDto
    {
        public int Sort { get; set; }
        public List<ApproveUser> UserList { get; set; }
    }

    
}
