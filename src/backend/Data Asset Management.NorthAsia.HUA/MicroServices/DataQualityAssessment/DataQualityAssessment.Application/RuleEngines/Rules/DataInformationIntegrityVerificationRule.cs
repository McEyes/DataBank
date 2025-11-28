using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DataQualityAssessment.Application.Models.PhoneNumber;
using DataQualityAssessment.Core.Dappers;
using DataQualityAssessment.Core.Enums;
using DataQualityAssessment.Core.Models;
using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace DataQualityAssessment.Application.RuleEngines.Rules
{
    /// <summary>
    /// 数据信息完整性
    /// </summary>
    public class DataInformationIntegrityVerificationRule : VerificationBaseRule
    {
        private readonly ILogger<DataInformationIntegrityVerificationRule> _logger;
        protected override RuleValidateType ValidateType { get => RuleValidateType.DataInformationIntegrity; }
        private List<MetadataAuthorizeOwnerModel> metadataAuthorizeOwnerModels;

        public DataInformationIntegrityVerificationRule(RuleDataContext context) : base(context)
        {
            if (context.Data is List<MetadataAuthorizeOwnerModel>)
            {
                metadataAuthorizeOwnerModels = context.Data as List<MetadataAuthorizeOwnerModel>;
            }

            _logger = App.GetService<ILogger<DataInformationIntegrityVerificationRule>>();
        }

        public override async Task<double> EvaluateAsync(CancellationToken token)
        {
            var score = 1.0;
            var table = RuleSetting.Table;
            var authorizeOwners = metadataAuthorizeOwnerModels;
            ulong invalidCount = 0L;
            ulong allCount = 0L;

            // Check metadata_table
            var columns = new string[] { table.table_comment, table.update_frequency, table.update_method, table.data_category };
            foreach (var column in columns)
            {
                if (string.IsNullOrWhiteSpace(column))
                {
                    invalidCount++;
                }

                allCount++;
            }

            // Check metadata_authorize_owner
            if (authorizeOwners != null && authorizeOwners.Count > 0)
            {
                var count = authorizeOwners.Count;
                var owner_name_null_qty = authorizeOwners.Count(t => string.IsNullOrWhiteSpace(t.owner_name));
                var owner_dept_null_qty = authorizeOwners.Count(t => string.IsNullOrWhiteSpace(t.owner_dept));

                invalidCount += (ulong)owner_name_null_qty;
                invalidCount += (ulong)owner_dept_null_qty;

                allCount += (ulong)count * 2;
            }

            var validCount = allCount - invalidCount;
            if (allCount > 0)
            {
                score = validCount * 1.0 / allCount;
            }

            AddReport(RuleSetting.RuleNo, score, "all table", "".Split(","), validCount, allCount);

            return await Task.FromResult(score);
        }

        protected override void CreateColumns(string[] keywords, out List<MetadataColumnModel> columnList)
        {
            columnList = new List<MetadataColumnModel>();
        }

        protected override void BuildSql(string tableName, StringBuilder sqlBuilder, List<MetadataColumnModel> columnList)
        {
            
        }
    }
}
