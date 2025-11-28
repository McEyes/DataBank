using ITPortal.Core.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.AuthServer.Application.Account.Dtos
{


    [SugarTable(TableName = "asset_user_role")]
    public class UserRolesEntity 
    {
        [SugarColumn(ColumnName = "user_id")]
        public  string UserId { get; set; }
        [SugarColumn(ColumnName = "role_id")]
        public string RoleId { get; set; }
        //[SugarColumn(ColumnName = "TenantId")]
        //public Guid? TenantId { get; set; }
    }
}
