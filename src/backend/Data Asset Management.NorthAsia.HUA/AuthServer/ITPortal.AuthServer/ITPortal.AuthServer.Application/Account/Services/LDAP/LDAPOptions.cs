using ITPortal.Core.Services;

using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.AuthServer.Core.AccountService
{
    public class LDAPOptions
    {
        public string LDAPServer { get; set; } = "corp.JABIL.ORG";
        public int LDAPPort { get; set; } = 636;
        public string LDAPBaseDn { get; set; } = "DC=corp,DC=******,DC=ORG";
        public string Domain { get; set; } = "JABIL";
        /// <summary>
        /// 查询账号
        /// </summary>
        public string MachineAccountName { get; set; }
        /// <summary>
        /// 查询账号密码
        /// </summary>
        public string MachineAccountPassword { get; set; }
    }
}
