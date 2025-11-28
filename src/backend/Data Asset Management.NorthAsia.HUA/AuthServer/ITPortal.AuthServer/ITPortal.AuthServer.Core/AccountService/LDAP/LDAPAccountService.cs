using Furion;

using ITPortal.Core.Services;

using Microsoft.IdentityModel.Tokens;

using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.AuthServer.Core.AccountService
{
    public class LDAPAccountService : AccountService
    {
        private readonly LDAPOptions lDAPOptions;
        public LDAPAccountService()
        {
            lDAPOptions = App.GetConfig<LDAPOptions>("LDAP");
        }

        public override IResult Login(string username, string password)
        {
            var result = new Result<bool>();
            result.Data = AuthenticateUser(lDAPOptions.LDAPServer, lDAPOptions.Domain, username, password);
            return result;
        }

        public override IResult Logout()
        {
            return new Result<bool>() { Data = true };
        }


        private bool AuthenticateUser(string ldapServer, string domain, string username, string password)
        {
            try
            {
                // 构建完整的用户名
                string fullUsername = $"{domain}\\{username}";

                // 创建 LDAP 连接
                using (LdapConnection connection = new LdapConnection(ldapServer))
                {
                    // 设置网络凭证
                    connection.Credential = new System.Net.NetworkCredential(fullUsername, password);

                    // 绑定到 LDAP 服务器
                    connection.Bind();

                    // 如果绑定成功，说明用户验证成功
                    return true;
                }
            }
            catch (LdapException ex)
            {
                // 处理 LDAP 异常，通常表示验证失败
                Console.WriteLine($"LDAP 验证失败: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                // 处理其他异常
                Console.WriteLine($"发生错误: {ex.Message}");
                return false;
            }
        }
        public string GenerateJwtToken(string username, string jwtSecret)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(jwtSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Email, username)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public bool ValidateToken(string token, string jwtSecret)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(jwtSecret);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
