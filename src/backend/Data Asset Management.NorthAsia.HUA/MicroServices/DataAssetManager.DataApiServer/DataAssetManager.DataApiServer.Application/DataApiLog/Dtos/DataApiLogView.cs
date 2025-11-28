using ITPortal.Core.Services;
using DataAssetManager.DataApiServer.Core;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Xml;
using ITPortal.Core.LightElasticSearch;
using Org.BouncyCastle.Utilities.Collections;

namespace DataAssetManager.DataApiServer.Application.DataApi.Dtos
{
    [SugarTable(TableName = "asset_data_api_visited_log")]
    public class DataApiLogView : Entity<string>
    {
        [MiniExcelLibs.Attributes.ExcelColumn(Ignore=true)]
        [SugarColumn(IsPrimaryKey = true, ColumnName = "id")]
        public override string Id { get; set; }

        //[SugarColumn(ColumnName = "api_id")]
        //public string ApiId { get; set; }

        [MiniExcelLibs.Attributes.ExcelColumn(Name = "Interface Name", Index = 0)]
        [SugarColumn(ColumnName = "api_name")]
        public string ApiName { get; set; }

        ///// <summary>
        ///// 表所属所属部门,从拥有者所属部门获取
        ///// </summary>
        //[SugarColumn(ColumnName = "table_id", Length = 50)]

        //public string TableId { get; set; }

        /// <summary>
        /// 表所属所属部门,从拥有者所属部门获取
        /// </summary>
        [MiniExcelLibs.Attributes.ExcelColumn(Name = "Table Name", Index = 1)]
        [SugarColumn(ColumnName = "table_name", Length = 50)]
        public string TableName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [MiniExcelLibs.Attributes.ExcelColumn(Name = "Table Comment", Index = 2)]
        [SugarColumn(ColumnName = "table_comment", Length = 50)]
        public string TableComment { get; set; }

        /// <summary>
        /// 表所属所属部门,从拥有者所属部门获取
        /// </summary>
        [MiniExcelLibs.Attributes.ExcelColumn(Ignore =true)]
        [SugarColumn(ColumnName = "owner_id")]
        public string Owner { get; set; }

        /// <summary>
        /// 表所属所属部门,从拥有者所属部门获取
        /// </summary>
        [MiniExcelLibs.Attributes.ExcelColumn(Name = "Owner name", Index = 3)]
        [SugarColumn(ColumnName = "owner_name", Length = 100)]
        public string OwnerName { get; set; }
        /// <summary>
        /// 表所属所属部门,从拥有者所属部门获取
        /// </summary>
        [MiniExcelLibs.Attributes.ExcelColumn(Name = "Owner dept", Index = 4)]
        [SugarColumn(ColumnName = "owner_depart", Length = 100)]
        public string OwnerDepart { get; set; }

        [MiniExcelLibs.Attributes.ExcelColumn(Ignore = true)]
        [SugarColumn(ColumnName = "caller_id")]
        public string CallerId { get; set; }

        [MiniExcelLibs.Attributes.ExcelColumn(Name = "Caller", Index = 5)]
        [SugarColumn(ColumnName = "caller_name")]
        public string CallerName { get; set; }

        [MiniExcelLibs.Attributes.ExcelColumn(Name = "Caller Ip", Index = 6)]
        [SugarColumn(ColumnName = "caller_ip")]
        public string CallerIp { get; set; }

        [MiniExcelLibs.Attributes.ExcelColumn(Ignore = true)]
        [SugarColumn(ColumnName = "caller_url")]
        public string CallerUrl { get; set; }

        [MiniExcelLibs.Attributes.ExcelColumn(Ignore = true)]
        [SugarColumn(ColumnName = "caller_params")]
        public string CallerParams { get; set; }

        [MiniExcelLibs.Attributes.ExcelColumn(Name = "Caller Date", Index = 7)]
        [SugarColumn(ColumnName = "caller_date")]
        public DateTimeOffset CallerDate { get; set; }

        [MiniExcelLibs.Attributes.ExcelColumn(Name = "Call Data Valume", Index = 8)]
        [SugarColumn(ColumnName = "caller_size")]
        public long CallerSize { get; set; }

        [MiniExcelLibs.Attributes.ExcelColumn(Name = "Elapsed time", Index = 9)]
        [SugarColumn(ColumnName = "time")]
        public int Time { get; set; }

        [MiniExcelLibs.Attributes.ExcelColumn(Ignore = true)]
        [SugarColumn(ColumnName = "status")]
        public int? Status { get; set; }

        [MiniExcelLibs.Attributes.ExcelColumn(Name = "Caller Status", Index = 10)]
        [SugarColumn(ColumnName = "statusName")]
        public string? StatusName { get; set; }

        [MiniExcelLibs.Attributes.ExcelColumn(Name = "Error Msg", Index = 11)]
        [SugarColumn(ColumnName = "msg")]
        public string Msg { get; set; }

    }

}