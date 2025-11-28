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
using static Dapper.SqlMapper;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace DataQualityAssessment.Application.RuleEngines.Rules
{
    /// <summary>
    /// 多余字段占比规则
    /// </summary>
    public class ExtraColumnsVerificationRule : VerificationBaseRule
    {
        private readonly ILogger<ExtraColumnsVerificationRule> _logger;
        protected override RuleValidateType ValidateType { get => RuleValidateType.ExtraColumns; }

        public ExtraColumnsVerificationRule(RuleDataContext context) : base(context)
        {
            _logger = App.GetService<ILogger<ExtraColumnsVerificationRule>>();
        }

        public override async Task<double> EvaluateAsync(CancellationToken token)
        {
            var score = 1.0;
            var tableName = DbSetting.TableName;
            var sqlBuilder = new StringBuilder();
            var keywords = new string[] { };
            var columnList = RuleSetting.Columns;
            ulong invalidCount = 0L;

            if (columnList is List<MetadataColumnModel> { Count: > 0 })
            {
                OpenConnection();
                // total
                var sqlTotal = BuildSqlTotal(tableName);
                var totalResult = await DbConnection.QuerySingleAsync<QueryResultModel>(sqlTotal);

                var size = 5;
                var pageCount = columnList.Count % size == 0 ? columnList.Count / size : columnList.Count / size + 1;

                OpenConnection();
                // 分页,优化性能
                for (int i = 0; i < pageCount; i++)
                {
                    var skip = i * size;
                    var cols = columnList.Skip(skip).Take(size).ToList();

                    sqlBuilder.Clear();
                    BuildSql(tableName, sqlBuilder, cols);

                    var queryEffective = sqlBuilder.ToString();
                    using (var gridReader = await DbConnection.QueryMultipleAsync(queryEffective))
                    {
                        var dataRow = await gridReader.ReadAsync();
                        var nullCountEntity = dataRow.FirstOrDefault();

                        if (nullCountEntity != null)
                        {
                            //allFeildCount = columnList.Count;
                            IDictionary<string, object> dapperRowProperties = nullCountEntity as IDictionary<string, object>;
                            foreach (var item in cols)
                            {
                                object nullCount = 0;
                                if (dapperRowProperties.TryGetValue($"{item.column_name}_null_count", out nullCount))
                                {
                                    if (totalResult.Count == Convert.ToUInt64(nullCount))
                                    {
                                        invalidCount += totalResult.Count;
                                    }
                                }

                                token.ThrowIfCancellationRequested();
                            }
                        }
                    }

                    Thread.Sleep(5);
                    token.ThrowIfCancellationRequested();
                }

                ulong totalCount = totalResult.Count * (ulong)columnList.Count;
                ulong validCount = totalCount - invalidCount;
                
                if (totalCount > 0)
                    score = validCount * 1.0 / totalCount;

                // return reports
                AddReport(RuleSetting.RuleNo, score, "all table", keywords, validCount, totalCount);

                return score;
            }
            else
            {
                AddReport(RuleSetting.RuleNo, score, "No matching columns found", keywords, 0, 0);
            }

            return score;
        }

        private string BuildSqlTotal(string tableName)
        {
            string sql = string.Empty;
            switch (DbSetting.DbType)
            {
                case DatabaseType.SqlServer:
                    sql = $"select count(*) as [Count] from [{tableName}];";
                    break;
                case DatabaseType.Mysql:
                    sql = $"select count(*) as `Count` from `{tableName}`;";
                    break;
                case DatabaseType.Postgresql:
                    sql = $"select count(*) as \"Count\" from \"{tableName}\";";
                    break;
                default:
                    throw new NotSupportedException("DbType Invalid");
            }

            _logger.LogInformation($"---------------sql:{sql}");

            return sql;
        }

        protected override void BuildSql(string tableName, StringBuilder sqlBuilder, List<MetadataColumnModel> columnList)
        {
            List<string> sql = new List<string>();
            foreach (var item in columnList)
            {
                switch (DbSetting.DbType)
                {
                    case DatabaseType.SqlServer:
                        sql.Add(@"SUM(CASE WHEN [" + item.column_name.Replace("[", "").Replace("]","") + @"] IS NULL THEN 1 ELSE 0 END) AS [" + item.column_name.Replace("[", "").Replace("]", "") + @"_null_count]");
                        break;
                    case DatabaseType.Mysql:
                        sql.Add(@"SUM(CASE WHEN `" + item.column_name.Replace("`","") + @"` IS NULL THEN 1 ELSE 0 END) AS `" + item.column_name.Replace("`","") + @"_null_count`");
                        break;
                    case DatabaseType.Postgresql:
                        sql.Add(@"SUM(CASE WHEN """ + item.column_name + @""" IS NULL THEN 1 ELSE 0 END) AS """ + item.column_name + @"_null_count""");
                        break;
                    default:
                        break;
                }
            }
            switch (DbSetting.DbType)
            {
                case DatabaseType.SqlServer:
                    sqlBuilder.AppendLine(" SELECT " + string.Join(",", sql) + $" FROM [{tableName}]; ");
                    break;
                case DatabaseType.Mysql:
                    sqlBuilder.AppendLine(" SELECT " + string.Join(",", sql) + $" FROM `{tableName}`; ");
                    break;
                case DatabaseType.Postgresql:
                    sqlBuilder.AppendLine(" SELECT " + string.Join(",", sql) + $" FROM \"{tableName}\"; ");
                    break;
                default:
                    break;
            }

            _logger.LogInformation($"---------------{this.GetType().Name},sqlBuilder:{sqlBuilder.ToString()}");
        }
    }
}
