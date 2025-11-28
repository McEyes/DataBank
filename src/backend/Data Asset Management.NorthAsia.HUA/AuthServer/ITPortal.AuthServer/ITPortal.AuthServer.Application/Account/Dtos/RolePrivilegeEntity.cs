using ITPortal.Core.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.AuthServer.Application.Account.Dtos
{


    [SugarTable(TableName = "asset_role_privilege")]
    public class RolePrivilegeEntity:Entity<string>
    {
        [SugarColumn(IsPrimaryKey =true, ColumnName = "id")]
        public override string Id { get => base.Id; set => base.Id = value; }


        [SugarColumn(ColumnName = "role_id")]
        public string RoleId { get; set; }


        [SugarColumn(ColumnName = "privilege_id")]
        public string PrivilegeId { get; set; }
    }
}
