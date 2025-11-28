using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
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
    /// 数据类型验证_枚举类型规则
    /// </summary>
    public class DataTypeEnumTypeVerificationRule : VerificationBaseRule
    {
        private readonly ILogger<DataTypeEnumTypeVerificationRule> _logger;
        protected override RuleValidateType ValidateType { get => RuleValidateType.DataTypeEnumType; }

        public DataTypeEnumTypeVerificationRule(RuleDataContext context) : base(context)
        {
            _logger = App.GetService<ILogger<DataTypeEnumTypeVerificationRule>>();
        }

        public override async Task<double> EvaluateAsync(CancellationToken token)
        {
           

            var score = 1.0;
            var tableName = DbSetting.TableName;
            var sqlBuilder = new StringBuilder();
            string[] keywords = ["类型", "状态", "：", "；"];
            var columnList = RuleSetting.Columns;
            var errorMessage = new StringBuilder();
            CreateColumns(keywords, out columnList);

            ulong validCount = 0L;
            ulong allCount = 0L;

            if (columnList is List<MetadataColumnModel> { Count: > 0 })
            {
                OpenConnection();

                allCount = (ulong)columnList.Count;

                // #1 判断字段类型
                #region 判断字段类型
                foreach (var column in columnList)
                {
                    if (column.data_type.Contains("int", StringComparison.OrdinalIgnoreCase))
                    {
                        validCount++;
                        continue;
                    }
                    else
                    {
                        errorMessage.AppendLine($"field {column.column_name} is {column.data_type}");
                    }

                    token.ThrowIfCancellationRequested();
                }

                score = validCount * 1.0 / allCount;
                // return reports
                AddReport(RuleSetting.RuleNo, score, string.Join(",", columnList.Select(t => t.column_name)), keywords, validCount, allCount, errorMessage.ToString());

                return score;

                #endregion
            }
            else
            {
                AddReport(RuleSetting.RuleNo, score, "No matching columns found", keywords, 0, 0);
            }


            return await Task.FromResult(score);
        }

        protected override void BuildSql(string tableName, StringBuilder sqlBuilder, List<MetadataColumnModel> columnList)
        {
            List<string> sql = new List<string>();
            List<string> whereStr = new List<string>();
            foreach (var item in columnList)
            {
                switch (DbSetting.DbType)
                {
                    case DatabaseType.SqlServer:
                        sql.Add($"[{item.column_name.Replace("[", "").Replace("]", "")}]");
                        break;
                    case DatabaseType.Mysql:
                        sql.Add($"`{item.column_name.Replace("`", "")}`");
                        break;
                    case DatabaseType.Postgresql:
                        sql.Add($"\"{item.column_name}\"");
                        break;
                    default:
                        break;
                }
            }

            switch (DbSetting.DbType)
            {
                case DatabaseType.SqlServer:
                    sqlBuilder.AppendLine(" SELECT top 1 " + string.Join(",", sql) + $" FROM [{tableName.Replace("[", "").Replace("]", "")}] ");
                    break;
                case DatabaseType.Mysql:
                    sqlBuilder.AppendLine(" SELECT " + string.Join(",", sql) + $" FROM `{tableName.Replace("`", "")}` limit 1 ");
                    break;
                case DatabaseType.Postgresql:
                    sqlBuilder.AppendLine(" SELECT " + string.Join(",", sql) + $" FROM \"{tableName}\" limit 1 ");
                    break;
                default:
                    break;
            }

            _logger.LogInformation($"---------------{this.GetType().Name},sqlBuilder:{sqlBuilder.ToString()}");
        }
    }
}
