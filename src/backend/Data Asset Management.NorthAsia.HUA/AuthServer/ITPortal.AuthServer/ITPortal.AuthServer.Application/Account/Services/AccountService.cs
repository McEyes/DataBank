using ITPortal.AuthServer.Application.Account.Services.Dtos;
using ITPortal.Core.Services;

using Microsoft.AspNetCore.Authentication;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.AuthServer.Core.AccountService
{
    public abstract class AccountService : IAccountService
    {
        public abstract Task<ITPortal.Core.Services.IResult> Login(LoginInput input);
        public abstract Task<ITPortal.Core.Services.IResult> Reset(ResetInput input);
        public virtual ITPortal.Core.Services.IResult Logout()
        {
            return new Result<bool>() { Data = true };
        }

        public abstract Task<ITPortal.Core.Services.IResult> InitReset(ResetInput input);

        public abstract Task InitResetPwd();

    }
}
