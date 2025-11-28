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
        public string LDAPServer { get; set; }
        public string Domain { get; set; }
    }
}
