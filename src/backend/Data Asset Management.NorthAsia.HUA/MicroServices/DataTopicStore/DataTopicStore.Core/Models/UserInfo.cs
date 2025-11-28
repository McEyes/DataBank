using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DataTopicStore.Core.Models
{
    public partial class UserInfo
    {
        /// <summary>
        /// userid guid
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 用户名，对应username字段
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 用户名，对应username字段
        /// </summary>
        public string EnglishName { get; set; }
        /// <summary>
        /// 显示名称：
        /// </summary>
        public string ChineseName { get; set; }
        /// <summary>
        /// 用户名，对应username字段
        /// </summary>
        public string Surname { get; set; }
        /// <summary>
        /// userid guid
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// Ntid
        /// </summary>
        public string NtId { get; set; }
        /// <summary>
        /// 显示名称：
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 显示名称：
        /// </summary>
        public string Department { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string SecurityStamp { get; set; }
        public List<string> Roles { get; set; }

        public UserInfo() { }

        public UserInfo(ClaimsPrincipal user)
        {
            if (user == null) return;
            Id = user.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier) ?? "";
            UserId = user.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier) ?? "";
            NtId = user.FindFirstValue("name") ?? "";
            UserName = user.FindFirstValue("name") ?? "";
            EnglishName = user.FindFirstValue("enname") ?? "";
            Surname = user.FindFirstValue(System.Security.Claims.ClaimTypes.Surname) ?? "";
            Name = user.FindFirstValue(ClaimTypes.GivenName) ?? ""; //姓名
            Email = user.FindFirstValue(System.Security.Claims.ClaimTypes.Email) ?? "";
            PhoneNumber = user.FindFirstValue(System.Security.Claims.ClaimTypes.MobilePhone) ?? "";
            SecurityStamp = user.FindFirstValue(System.Security.Claims.ClaimTypes.Hash) ?? "";
            Department = user.FindFirstValue("depart") ?? "";
            Roles = user.FindAll(System.Security.Claims.ClaimTypes.Role).Select(roles => roles.Value).ToList();
        }

        public bool IsDataAssetManager { get { return (Roles?.Contains("DataAssetCatalog") == true || Roles?.Contains("DATAASSETCATALOG") == true || Roles?.Contains("admin") == true || Roles?.Contains("ADMIN") == true); } }
    }
}
