using ITPortal.Core.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.AuthServer.Application.Account.Dtos
{


    [SugarTable(TableName = "asset_privilege")]
    public class PrivilegeEntity : AuditEntity<string>
    {
        [SugarColumn(IsPrimaryKey = true,ColumnName ="id")]
        public override string Id { get; set; }

        [SugarColumn(ColumnName = "parent_id")]
        public string ParentId { get; set; }
        [SugarColumn(ColumnName = "privilege_name")]
        public string Name { get; set; }

        [SugarColumn(ColumnName = "privilege_path")]
        public string PathKey { get; set; }


        [SugarColumn(ColumnName = "privilege_component")]
        public string Component { get; set; }

        /// <summary>
        /// 跳转路径
        /// </summary>
        [SugarColumn(ColumnName = "privilege_redirect")]
        public string RedirectUrl { get; set; }



        [SugarColumn(ColumnName = "privilege_perms")]
        public string RedirectPerms { get; set; }


        [SugarColumn(ColumnName = "privilege_icon")]
        public string Icon { get; set; }
        [SugarColumn(ColumnName = "privilege_type")]
        public int? Type { get; set; }

        [SugarColumn(ColumnName = "privilege_code")]
        public string code { get; set; }
        [SugarColumn(ColumnName = "privilege_hidden")]
        public int? IsHidden { get; set; }

        [SugarColumn(ColumnName = "privilege_sort")]
        public int? Sort { get; set; }

        [SugarColumn(ColumnName = "status")]
        public int? Status { get; set; }



        [SugarColumn(ColumnName = "remark")]
        public string Remark { get; set; }
    }
}
