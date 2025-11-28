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
    /// 数据类型验证_日期类型规则
    /// </summary>
    public class DataTypeDateTypeVerificationRule : VerificationBaseRule
    {
        private readonly ILogger<DataTypeDateTypeVerificationRule> _logger;
        protected override RuleValidateType ValidateType { get => RuleValidateType.DataTypeDateType; }

        public DataTypeDateTypeVerificationRule(RuleDataContext context) : base(context)
        {
            _logger = App.GetService<ILogger<DataTypeDateTypeVerificationRule>>();
        }

        public override async Task<double> EvaluateAsync(CancellationToken token)
        {
            var score = 1.0;
            var tableName = DbSetting.TableName;
            var sqlBuilder = new StringBuilder();
            string[] keywords = ["日期", "时间", "time", "date", "timestamp"];
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
                    if (token.IsCancellationRequested)break;
                    if (column.data_type.Contains("date", StringComparison.OrdinalIgnoreCase) || column.data_type.Contains("time", StringComparison.OrdinalIgnoreCase))
                    {
                        validCount++;
                        continue;
                    }
                    else
                    {
                        errorMessage.AppendLine($"field {column.column_name} is {column.data_type}");
                    }
                }

                double score1 = validCount * 1.0 / allCount;
                if (validCount == allCount)
                {
                    score = score1;
                    AddReport(RuleSetting.RuleNo, score, "all table", keywords, validCount, allCount, errorMessage.ToString());
                    return score;
                }

                #endregion

                // #2 判断数据
                #region  判断数据

                BuildSql(tableName, sqlBuilder, columnList);
                var queryEffective = sqlBuilder.ToString();
                validCount = 0;
                allCount = 0;
                using (var gridReader = await DbConnection.QueryMultipleAsync(queryEffective))
                {
                    var dataRow = await gridReader.ReadAsync();
                    var nullCountEntity = dataRow.FirstOrDefault();
                    if (nullCountEntity != null)
                    {
                        allCount = (ulong)columnList.Count;
                        IDictionary<string, object> dapperRowProperties = nullCountEntity as IDictionary<string, object>;
                        foreach (var item in columnList)
                        {
                            object dateValue = "";
                            if (dapperRowProperties.TryGetValue($"{item.column_name}", out dateValue))
                            {
                                if (DateTime.TryParse(dateValue?.ToString(), out DateTime _))
                                    validCount++;
                                else
                                    errorMessage.AppendLine($"field {item.column_name} is {item.data_type},value:{dateValue}");
                            }

                            token.ThrowIfCancellationRequested();
                        }
                    }
                }

                if (allCount > 0)
                {
                    var score2 = validCount * 1.0 / allCount;
                    score = (score1 + score2) / 2;
                }

                // return reports
                AddReport(RuleSetting.RuleNo, score, string.Join(",", columnList.Select(t => t.column_name)), keywords, validCount, allCount, errorMessage.ToString());

                #endregion
            }
            else
            {
                AddReport(RuleSetting.RuleNo, score, "No matching columns found", keywords, 0, 0);
            }

            return score;
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
                    sqlBuilder.AppendLine(" SELECT top 1 " + string.Join(",", sql) + $" FROM [{tableName.Replace("[", "").Replace("]", "")}]");
                    break;
                case DatabaseType.Mysql:
                    sqlBuilder.AppendLine(" SELECT " + string.Join(",", sql) + $" FROM `{tableName.Replace("`", "")}`  limit 1");
                    break;
                case DatabaseType.Postgresql:
                    sqlBuilder.AppendLine(" SELECT " + string.Join(",", sql) + $" FROM \"{tableName}\"  limit 1");
                    break;
                default:
                    break;
            }

            _logger.LogInformation($"---------------{this.GetType().Name},sqlBuilder:{sqlBuilder.ToString()}");
        }
    }
}
