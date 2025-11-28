using ITPortal.Core.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.AuthServer.Application.Account.Dtos
{


    [SugarTable(TableName = "asset_user")]
    public class UserEntity : AuditEntity<string>
    {
        /// <summary>
        /// 后续改成ntid
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, ColumnName = "id")]
        public override string Id { get; set; }
        /// <summary>
        /// user name 大多数=NTID
        /// </summary>
        [SugarColumn(ColumnName = "username")]
        public string UserName { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        [SugarColumn(ColumnName = "name")]
        public string Name { get; set; }
        public string EnglishName { get; set; }
        /// <summary>
        /// NTID
        /// </summary>
        [SugarColumn(ColumnName = "ntid")]
        public string Surname { get; set; }
        [SugarColumn(ColumnName = "email")]
        public string Email { get; set; }
        [SugarColumn(ColumnName = "PhoneNumber")]
        public string PhoneNumber { get; set; }
        [SugarColumn(ColumnName = "password")]
        public string PasswordHash { get; set; }
        /// <summary>
        /// 1启用
        /// </summary>
        [SugarColumn(ColumnName = "status")]
        public int? Status { get; set; } = 1;

        [SugarColumn(ColumnName = "SecurityStamp")]
        public string SecurityStamp { get; set; }

        [SugarColumn(IsIgnore = true)]
        public string Token { get; set; }

        [SugarColumn(IsIgnore = true)]
        public string UserToken { get; set; }

        [SugarColumn(IsIgnore = true)]
        public List<string> Roles { get; set; } = new List<string>();
        [SugarColumn(IsIgnore = true)]
        public string RoleId { get; set; }
        [SugarColumn(IsIgnore = true)]
        public string RoleName { get; set; }
        public string Department { get; set; }
    }
}
