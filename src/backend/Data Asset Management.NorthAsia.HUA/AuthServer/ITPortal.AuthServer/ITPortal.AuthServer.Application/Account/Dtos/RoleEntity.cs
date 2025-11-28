using ITPortal.Core.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.AuthServer.Application.Account.Dtos
{


    [SugarTable(TableName = "asset_role")]
    public class RoleEntity : AuditEntity<string>
    {
        [SugarColumn(IsPrimaryKey = true,ColumnName ="id")]
        public override string Id { get; set; }
        [SugarColumn(ColumnName = "role_code")]
        public string Code { get; set; }
        [SugarColumn(ColumnName = "role_name")]
        public string Name { get; set; }
        [SugarColumn(ColumnName = "status")]
        public int? Status { get; set; }
        [SugarColumn(ColumnName = "is_default")]
        public bool IsDefault { get; set; }
        [SugarColumn(ColumnName = "is_static")]
        public bool IsStatic { get; set; }
        [SugarColumn(ColumnName = "is_public")]
        public bool IsPublic { get; set; }
    }
}
