using Furion;
using Furion.DataEncryption;

using ITPortal.AuthServer.Application;
using ITPortal.AuthServer.Application.Account.Dtos;
using ITPortal.AuthServer.Core.AccountService;

using MapsterMapper;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json.Linq;

using SqlSugar;

using System.Security.Claims;

namespace ITPortal.AuthServer.Web.Entry.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountController(IUserService userService)
        {
            _accountService = new LDAPAccountService(userService);
        }

        public IActionResult Login()
        {
            string jwtToken = HttpContext.Request.Headers["access-token"];
            if (!string.IsNullOrWhiteSpace(jwtToken))
            {
                var result = JWTEncryption.Validate(jwtToken);
                if (result.IsValid)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountService.Login(new() { Name = model.Username, Password = model.Password });
                if (result.Success)
                {

                    // 验证成功，生成 JWT 令牌
                    var jwtToken = JWTEncryption.Encrypt(new Dictionary<string, object>()
                        {
                            { "UserId", model.Username },  // 存储Id
                            { "Account",model.Username }, // 存储用户名
                        });
                    // 将令牌存储在会话中
                    HttpContext.SetTokensOfResponseHeaders("token", jwtToken);
                    var principal = new ClaimsPrincipal(new ClaimsIdentity[]
                        {
                            new ClaimsIdentity(new Claim[]
                            {
                                new Claim(ClaimTypes.Name,"boo"),
                                new Claim("CustomClaim", "Good guy")
                            }, "Default")
                        });
                    await HttpContext.SignInAsync(principal);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "用户名或密码错误");
                }
            }
            return View(model);
        }
        public IActionResult Logout()
        {
            _accountService.Logout();

            return View();
        }
    }
}
