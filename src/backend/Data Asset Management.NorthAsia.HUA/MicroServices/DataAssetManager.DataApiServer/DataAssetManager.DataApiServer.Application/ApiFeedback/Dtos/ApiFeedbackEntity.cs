using ITPortal.Core.Services;
using DataAssetManager.DataApiServer.Core;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Xml;
using ITPortal.Core.LightElasticSearch;

namespace DataAssetManager.DataApiServer.Application.DataApi.Dtos
{

    [ElasticIndexName("ApiFeedback", "DataAsset")]
    [SugarTable(TableName = "asset_api_feedback")]
    public class ApiFeedbackEntity : AuditEntity<Guid>
    {
        [SugarColumn(IsPrimaryKey = true, ColumnName = "id")]
        public override Guid Id { get; set; }

        [SugarColumn(ColumnName = "title")]
        public string Title { get; set; }

        [SugarColumn(ColumnName = "description")]
        public string Description { get; set; }

        [SugarColumn(ColumnName = "files")]
        public string Files { get; set; }

        [SugarColumn(ColumnName = "user_id")]
        public string UserId { get; set; }

        [SugarColumn(ColumnName = "user_name")]
        public string UserName { get; set; }

        [SugarColumn(ColumnName = "user_email")]
        public string UserEmail { get; set; }

        [SugarColumn(ColumnName = "user_dept")]
        public string UserDept { get; set; }

        [SugarColumn(ColumnName = "status")]
        public string Status { get; set; }

        [SugarColumn(ColumnName = "object_id")]
        public string ObjectId { get; set; }

        [SugarColumn(ColumnName = "object_type")]
        public string ObjectType { get; set; }

        [SugarColumn(ColumnName = "object_name")]
        public string ObjectName { get; set; }

        [SugarColumn(ColumnName = "owner_id")]
        public string OwnerId { get; set; }

        [SugarColumn(ColumnName = "owner_name")]
        public string OwnerName { get; set; }

        [SugarColumn(ColumnName = "owner_email")]
        public string OwnerEmail { get; set; }

    }
}