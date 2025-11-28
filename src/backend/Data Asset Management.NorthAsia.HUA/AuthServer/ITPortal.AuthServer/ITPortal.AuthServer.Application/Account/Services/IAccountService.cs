using ITPortal.AuthServer.Application.Account.Services.Dtos;

namespace ITPortal.AuthServer.Core.AccountService
{
    public interface IAccountService
    {
        Task<ITPortal.Core.Services.IResult> Login(LoginInput input);
        Task<ITPortal.Core.Services.IResult> Reset(ResetInput input);
        ITPortal.Core.Services.IResult Logout();
        Task<ITPortal.Core.Services.IResult> InitReset(ResetInput input);
        Task InitResetPwd();
    }
}
