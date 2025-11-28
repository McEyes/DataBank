using ITPortal.Core.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.AuthServer.Core.AccountService
{
    public abstract class AccountService : IAccountService
    {

        public abstract IResult Login(string username, string password);

        public abstract IResult Logout();
    }
}
