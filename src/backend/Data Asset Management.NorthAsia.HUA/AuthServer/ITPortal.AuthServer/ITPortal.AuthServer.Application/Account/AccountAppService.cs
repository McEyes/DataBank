using ITPortal.AuthServer.Application.Account.Dtos;
using ITPortal.AuthServer.Application.Account.Services.Dtos;
using ITPortal.AuthServer.Core.AccountService;
using ITPortal.Core.Extensions;
using ITPortal.Core.Services;
using ITPortal.Extension.System;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using System.Security.Principal;

namespace ITPortal.AuthServer.Application
{
    /// <summary>
    /// 账号管理
    /// </summary>
    public class AccountAppService : IDynamicApiController
    {
        private IAccountService _service;
        private readonly IUserService _userService;
        private readonly ILogger<AccountAppService> _logger;
        public AccountAppService(IUserService userService, ILogger<AccountAppService> _logger)
        {
            _userService = userService;
            _service = new DbAccountService(userService);//_passwordHasher
        }

        /// <summary>
        /// 检查配置文件
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet("/configuration/{key}")]
        public string GetMsg(string key)
        {
            var data = App.Configuration.GetSection(key);
            return data.Value;
        }


        [HttpGet("GetAdUserInfo")]
        [Authorize] // 要求身份验证
        [AppAuthorize] // 要求身份验证
        public async Task<ITPortal.Core.Services.IResult> GetAdUserInfo()
        {
            var result = new Result<LoginDto>();
            // 方法1: 从 HttpContext 直接获取 Windows 身份信息
            var windowsIdentity = App.User.Identity as WindowsIdentity;

            if (windowsIdentity == null)
            {
                result.Success = false;
                result.Msg = "未找到 Windows 身份信息";
                return await Task.FromResult(result);
            }

            // 获取不同格式的用户名
            string fullUserName = windowsIdentity.Name;
            if (string.IsNullOrEmpty(fullUserName) || !fullUserName.Contains('\\'))
            {
                _logger.LogWarning($"无效的用户名格式: {fullUserName}");
                result.Success = false;
                result.Msg = "无法解析用户信息";
                return await Task.FromResult(result);
                //throw new UnauthorizedAccessException("无法解析用户信息");
            }
            string[] nameParts = fullUserName.Split('\\',2);
            string domain = nameParts[0];//.Length > 1 ? nameParts[0] : string.Empty;
            string username = nameParts[1];//.Length > 1 ? nameParts[1] : fullUserName;
            var userinfo = await _userService.GetUserRoleInfo(username);
            if (userinfo == null)
            {
                userinfo = new UserEntity()
                {
                    Id = username,
                    UserName = username,
                    Name = username,
                    Status = 1,
                    Surname = username,
                    EnglishName = username,
                    Roles = (await _userService.GetUserRoles(username)).Select(f => f.Name).ToList(),
                };
            }
            result.Data = userinfo.Adapt<LoginDto>();// _mapper.Map<UserEntity, LoginDto>(userInfo);
            result.Data.Token = _userService.GenerateJwtToken(userinfo);
            result.Data.UserToken = ITPortal.Core.DESEncryption.Encrypt(userinfo.Id.ToString());
            return result;
        }


        /// <summary>
        /// 获取当前登陆token信息
        /// </summary>
        /// <returns></returns>
        [AppAuthorize]
        [Authorize]
        [HttpGet("UserInfo")]
        public async Task<ITPortal.Core.Services.IResult> GetTokenUserInfo()
        {
            if (App.User.Identity is WindowsIdentity) return await GetAdUserInfo();
            var result = new Result<UserInfo>();
            result.Data = new UserInfo(App.HttpContext.User);
            return await Task.FromResult(result);
        }



        /// <summary>
        /// AD登陆
        /// </summary>
        /// <returns></returns>
        public async Task<ITPortal.Core.Services.IResult> LoginAd(LoginInput input)
        {
            _service = new LDAPAccountService( _userService);
            return await _service.Login(input);
        }

        /// <summary>
        /// 登陆
        /// </summary>
        /// <returns></returns>
        public async Task<ITPortal.Core.Services.IResult> Login(LoginInput input)
        {
            return await _service.Login(input);
        }


        /// <summary>
        /// 重置密码
        /// </summary>
        /// <returns></returns>
        [AppAuthorize]
        [Authorize]
        public async Task<ITPortal.Core.Services.IResult> Reset(ResetInput input)
        {
            return await _service.Reset(input);
        }


        /// <summary>
        /// 注销
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ITPortal.Core.Services.IResult Logout()
        {
            return _service.Logout();
        }
    }
}
