using Furion.JsonSerialization;

using ITPortal.AuthServer.Application.Account.Dtos;
using ITPortal.AuthServer.Application.Account.Services.Dtos;
using ITPortal.Core.Services;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

using System.DirectoryServices;
using System.DirectoryServices.Protocols;
using System.Net;
using System.Security.Principal;

namespace ITPortal.AuthServer.Core.AccountService
{
    public class LDAPAccountService : AccountService
    {
        private readonly LDAPOptions lDAPOptions;
        private readonly PasswordHasher<UserEntity> _passwordHasher;
        private readonly IUserService _userService;
        private readonly ILogger<LDAPAccountService> _logger;
        public LDAPAccountService( IUserService userService)
        {
            lDAPOptions = App.GetConfig<LDAPOptions>("LDAP");
            _passwordHasher = new PasswordHasher<UserEntity>();
            _userService = userService;
            _logger = App.GetService<ILogger<LDAPAccountService>>();
        }

        public override async Task<ITPortal.Core.Services.IResult> Login(LoginInput input)
        {
            var result = new Result<LoginDto>();
            try
            {
                var flag =await AuthenticateAsync(lDAPOptions.Domain, input.Name, input.Password);
#if DEBUG
                flag = true;
                //input.Name = "4154038";//3657741，3099002 "3324558";// "3303739";// "HuangR9";// "Lis Chen";// "wangxl";// "3815794";//"3329593";// 
#endif
                UserEntity userInfo = await _userService.GetUserRoleInfo(input.Name);
                if (userInfo != null && (flag || _passwordHasher.VerifyHashedPassword(userInfo, userInfo.PasswordHash, input.Password) != PasswordVerificationResult.Failed))
                {
                    result.Data = userInfo.Adapt<LoginDto>();// _mapper.Map<UserEntity, LoginDto>(userInfo);
                    result.Data.Token = _userService.GenerateJwtToken(userInfo);
                    result.Data.UserToken = ITPortal.Core.DESEncryption.Encrypt(userInfo.Id.ToString());
                }
                else
                {
                    result.Success = false;
                    result.Msg = "Wrong account or password！";
                }
            }catch(Exception ex)
            {
                _logger.LogError(ex.Message,ex);
                result.Success = false;
                result.Msg = "Wrong account or password！";
            }
            return await Task.FromResult(result);
        }

        ///// <summary>
        ///// 获取用户LDAP属性信息
        ///// </summary>
        //public async Task<UserEntity> GetUserInfoAsync(string domain, string username)
        //{
        //    var result = new UserEntity();
        //    var fullUsername = $"{domain}\\{username}";

        //    using var connection = new LdapConnection(new LdapDirectoryIdentifier(lDAPOptions.LDAPServer, lDAPOptions.LDAPPort));
        //    connection.SessionOptions.ProtocolVersion = 3;

        //    try
        //    {
        //        // 这里可以使用服务账号绑定，而不是用户账号
        //        // 生产环境建议使用专用的LDAP查询服务账号
        //        connection.Bind();

        //        var searchFilter = $"(&(objectClass=user)(sAMAccountName={username}))";
        //        var searchRequest = new System.DirectoryServices.Protocols.SearchRequest(
        //            lDAPOptions.LDAPBaseDn,
        //            searchFilter,
        //            System.DirectoryServices.Protocols.SearchScope.Subtree,
        //            "displayName", "mail", "department", "title"
        //        );

        //        var response = await Task.Run(() =>
        //            (SearchResponse)connection.SendRequest(searchRequest)
        //        );

        //        if (response.Entries.Count > 0)
        //        {
        //            var userEntry = response.Entries[0];

        //            // 提取常用属性
        //            result.Id = username;
        //            result.UserName = username;
        //            result.Name = GetLdapAttribute(userEntry, "displayName");
        //            result.Email = GetLdapAttribute(userEntry, "mail");
        //            result.Department = GetLdapAttribute(userEntry, "department");
        //            result.Surname = GetLdapAttribute(userEntry, "title");
        //            //result["Domain"] = domain;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // 记录日志
        //        Console.WriteLine($"LDAP查询失败: {ex.Message}");
        //    }

        //    return result;
        //}

        private string GetLdapAttribute(SearchResultEntry entry, string attributeName)
        {
            if (entry.Attributes.Contains(attributeName) && entry.Attributes[attributeName].Count > 0)
            {
                return entry.Attributes[attributeName][0].ToString();
            }
            return string.Empty;
        }

        public override async Task<ITPortal.Core.Services.IResult> Reset(ResetInput input)
        {
            await Task.CompletedTask;
            throw new NotSupportedException("Please change the AD account password directly！");
        }


        public override ITPortal.Core.Services.IResult Logout()
        {
            return new Result<bool>() { Data = true };
        }

        /// <summary>
        /// 验证域账号密码
        /// </summary>
        public async Task<bool> AuthenticateAsync(string domain, string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return false;

            var fullUsername = $"{domain}\\{username}";

            using var connection = new LdapConnection(new LdapDirectoryIdentifier(lDAPOptions.LDAPServer, lDAPOptions.LDAPPort));
            connection.SessionOptions.ProtocolVersion = 3;
            //connection.AuthType = AuthType.Negotiate;
            // 替换原有的AuthType.Negotiate
            //connection.AuthType = AuthType.Ntlm;  // 适用于Windows域环境
            // 或
            connection.AuthType = AuthType.Basic;  // 需确保LDAP服务器允许

            try
            {
                await Task.Run(() =>
                    connection.Bind(new NetworkCredential(fullUsername, password))
                );
                return true;
            }
            catch (LdapException ex)
            {
                // 记录详细错误信息
                Console.WriteLine($"LDAP认证失败: {ex.Message}, 错误代码: {ex.ErrorCode}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"认证过程发生错误: {ex.Message}");
                return false;
            }
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
       
       
        private bool AuthenticateUserAd(string adServer, string domain, string username, string password)
        {
            try
            {
                DirectoryEntry entry = new DirectoryEntry($"{adServer}", $"{domain}\\{username}", password);
                // 尝试绑定到 AD
                object nativeObject = entry.NativeObject;
                Console.WriteLine($"验证成功，信息: {JSON.Serialize(nativeObject)}");
                return true;
            }
            catch (DirectoryServicesCOMException ex)
            {
                Console.WriteLine($"验证失败，错误信息: {ex.Message}");
                _logger.LogError($"验证失败，错误信息: {ex.Message}", ex);
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"验证失败，错误信息: {ex.Message}");
                _logger.LogError($"验证失败，错误信息: {ex.Message}", ex);
                return false;
            }
        }

        public override async Task<ITPortal.Core.Services.IResult> InitReset(ResetInput input)
        {
            return await Task.FromResult(new Result());
        }

        public override async Task InitResetPwd()
        {
             await Task.FromResult(new Result());
        }



        // 已有的GetUserInfoAsync方法保持不变
        public async Task<Dictionary<string, string>> GetUserInfoAsync(string domain, string username)
        {
            // 保持现有实现
            var result = new Dictionary<string, string>();

            if (string.IsNullOrEmpty(username))
            {
                _logger.LogWarning("用户名不能为空");
                return result;
            }

            using var connection = new LdapConnection(new LdapDirectoryIdentifier(lDAPOptions.LDAPServer, lDAPOptions.LDAPPort));
            connection.SessionOptions.ProtocolVersion = 3;

            try
            {
                // 使用服务账号绑定 - 生产环境应从安全配置中获取这些信息
                var serviceAccount = Environment.GetEnvironmentVariable("LDAP_SERVICE_ACCOUNT");
                var servicePassword = Environment.GetEnvironmentVariable("LDAP_SERVICE_PASSWORD");

                if (string.IsNullOrEmpty(serviceAccount) || string.IsNullOrEmpty(servicePassword))
                {
                    _logger.LogWarning("未配置LDAP服务账号，使用匿名绑定");
                    connection.Bind();
                }
                else
                {
                    connection.Bind(new NetworkCredential(serviceAccount, servicePassword));
                }

                var searchFilter = $"(&(objectClass=user)(sAMAccountName={username}))";
                _logger.LogInformation($"执行LDAP查询: BaseDN={lDAPOptions.LDAPBaseDn}, Filter={searchFilter}");

                var searchRequest = new SearchRequest(
                    lDAPOptions.LDAPBaseDn,
                    searchFilter,
                    System.DirectoryServices.Protocols.SearchScope.Subtree,
                    "displayName", "mail", "department", "title", "telephoneNumber", "physicalDeliveryOfficeName"
                );

                var response = await Task.Run(() =>
                    (SearchResponse)connection.SendRequest(searchRequest)
                );

                if (response.Entries.Count > 0)
                {
                    var userEntry = response.Entries[0];

                    result["Username"] = username;
                    result["DisplayName"] = GetLdapAttribute(userEntry, "displayName");
                    result["Email"] = GetLdapAttribute(userEntry, "mail");
                    result["Department"] = GetLdapAttribute(userEntry, "department");
                    result["Title"] = GetLdapAttribute(userEntry, "title");
                    result["Phone"] = GetLdapAttribute(userEntry, "telephoneNumber");
                    result["Office"] = GetLdapAttribute(userEntry, "physicalDeliveryOfficeName");
                    result["Domain"] = domain;
                    result["DistinguishedName"] = userEntry.DistinguishedName;
                }
                else
                {
                    _logger.LogWarning($"未找到用户 {username} 的LDAP记录");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"LDAP查询失败: {ex.Message}");
            }

            return result;
        }

        // 新增方法：获取当前访问者的AD信息
        public async Task<Dictionary<string, string>> GetAdUserInfoAsync(HttpContext context)
        {
            // 从Windows身份验证获取用户信息
            var windowsIdentity = context.User.Identity as WindowsIdentity;
            if (windowsIdentity == null)
            {
                _logger.LogWarning("无法获取Windows身份信息 - 可能未启用Windows身份验证");
                throw new UnauthorizedAccessException("需要Windows身份验证");
            }

            // 解析域名和用户名
            var fullName = windowsIdentity.Name;
            if (string.IsNullOrEmpty(fullName) || !fullName.Contains('\\'))
            {
                _logger.LogWarning($"无效的用户名格式: {fullName}");
                throw new UnauthorizedAccessException("无法解析用户信息");
            }

            var parts = fullName.Split('\\', 2);
            var domain = parts[0];
            var username = parts[1];

            _logger.LogInformation($"获取用户 {domain}\\{username} 的AD信息");

            // 调用现有方法获取详细信息
            var userInfo = await GetUserInfoAsync(domain, username);

            // 添加额外的客户端信息
            userInfo["ClientIp"] = GetClientIpAddress(context);
            userInfo["Authenticated"] = "true";

            return userInfo;
        }
        // 验证客户端IP是否属于域内子网
        public bool IsDomainComputer(string clientIp, string[] allowedSubnets)
        {
            if (string.IsNullOrEmpty(clientIp) || allowedSubnets == null || allowedSubnets.Length == 0)
                return false;

            try
            {
                var ipAddress = IPAddress.Parse(clientIp);
                foreach (var subnet in allowedSubnets)
                {
                    if (IsIpInSubnet(ipAddress, subnet))
                        return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"验证客户端IP {clientIp} 失败");
                return false;
            }
        }

        // 辅助方法：获取客户端IP地址
        private string GetClientIpAddress(HttpContext context)
        {
            if (context.Request.Headers.ContainsKey("X-Forwarded-For"))
                return context.Request.Headers["X-Forwarded-For"].FirstOrDefault()?.Split(',')[0].Trim() ?? context.Connection.RemoteIpAddress?.ToString();

            return context.Connection.RemoteIpAddress?.ToString() ?? "未知";
        }

        // 辅助方法：检查IP是否在子网内
        private bool IsIpInSubnet(IPAddress ip, string subnet)
        {
            var parts = subnet.Split('/');
            var subnetIp = IPAddress.Parse(parts[0]);
            var prefixLength = int.Parse(parts[1]);

            var ipBytes = ip.GetAddressBytes();
            var subnetBytes = subnetIp.GetAddressBytes();

            // 只处理IPv4地址
            if (ipBytes.Length != 4 || subnetBytes.Length != 4)
                return false;

            var mask = prefixLength == 0 ? new byte[4] :
                new byte[] {
                (byte)(prefixLength >= 8 ? 255 : (255 << (8 - prefixLength)) & 255),
                (byte)(prefixLength >= 16 ? 255 : (prefixLength >= 8 ? (255 << (16 - prefixLength)) & 255 : 0)),
                (byte)(prefixLength >= 24 ? 255 : (prefixLength >= 16 ? (255 << (24 - prefixLength)) & 255 : 0)),
                (byte)(prefixLength >= 32 ? 255 : (prefixLength >= 24 ? (255 << (32 - prefixLength)) & 255 : 0))
                };

            for (int i = 0; i < 4; i++)
            {
                if ((ipBytes[i] & mask[i]) != (subnetBytes[i] & mask[i]))
                    return false;
            }

            return true;
        }

    }
}
