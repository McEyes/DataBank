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
    /// 数据类型验证_VARCHAR类型规则
    /// </summary>
    public class DataTypeVarcharTypeVerificationRule : VerificationBaseRule
    {
        private readonly ILogger<DataTypeVarcharTypeVerificationRule> _logger;
        protected override RuleValidateType ValidateType { get => RuleValidateType.DataTypeVarcharType; }

        public DataTypeVarcharTypeVerificationRule(RuleDataContext context) : base(context)
        {
            _logger = App.GetService<ILogger<DataTypeVarcharTypeVerificationRule>>();
        }

        public override async Task<double> EvaluateAsync(CancellationToken token)
        {
            var score = 1.0;
            var tableName = DbSetting.TableName;
            var sqlBuilder = new StringBuilder();
            string[] keywords = ["varchar"];
            var columnList = RuleSetting.Columns;
            var errorMessage = new StringBuilder();
            CreateColumns(keywords, out columnList);

            ulong invalidCount = 0L;
            ulong allCount = 0L;

            if (columnList is List<MetadataColumnModel> { Count: > 0 })
            {
                OpenConnection();
                var size = 5;
                var pageCount = columnList.Count % size == 0 ? columnList.Count / size : columnList.Count / size + 1;

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
                        foreach (var item in cols)
                        {
                            var dataRow = await gridReader.ReadAsync<QueryResultModel>();
                            if (dataRow != null && dataRow.Count() > 0)
                            {
                                invalidCount = invalidCount + dataRow.FirstOrDefault().Count;
                            }

                            token.ThrowIfCancellationRequested();
                        }
                    }

                    Thread.Sleep(5);
                    token.ThrowIfCancellationRequested();
                }

                OpenConnection();

                var sql = BuildSqlTotal(tableName);
                var totalResult = await DbConnection.QuerySingleAsync<QueryResultModel>(sql);
                if (totalResult != null && totalResult.Count > 0)
                {
                    var totalCount = totalResult.Count;
                    allCount = (ulong)totalCount * (ulong)columnList.Count;

                }

                var validCount = allCount - invalidCount;
                if (allCount > 0)
                {
                    score = validCount * 1.0 / allCount;
                }

                // return reports
                AddReport(RuleSetting.RuleNo, score, string.Join(",", columnList.Select(t => t.column_name)), keywords, validCount, allCount);

                return score;
            }
            else
            {
                AddReport(RuleSetting.RuleNo, score, "No matching columns found", keywords, 0, 0);
            }

            return await Task.FromResult(score);
        }

        protected override void CreateColumns(string[] keywords, out List<MetadataColumnModel> columnList)
        {
            columnList = new List<MetadataColumnModel>();
            foreach (var kw in keywords)
            {
                var clist = RuleSetting.Columns.Where(t => t.data_type.StartsWith(kw, StringComparison.OrdinalIgnoreCase)).ToList();
                if (clist != null && clist.Count > 0)
                {
                    columnList.AddRange(clist);
                }
            }
        }

        private string BuildSqlTotal(string tableName)
        {
            string sql = string.Empty;
            switch (DbSetting.DbType)
            {
                case DatabaseType.SqlServer:
                    sql = $"select count(*) as [Count] from [{tableName.Replace("[", "").Replace("]", "")}];";
                    break;
                case DatabaseType.Mysql:
                    sql = $"select count(*) as `Count` from `{tableName.Replace("`", "")}`;";
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
            List<string> whereStr = new List<string>();
            foreach (var item in columnList)
            {
                switch (DbSetting.DbType)
                {
                    case DatabaseType.SqlServer:
                        sql.Add($" select count(*) as [Count] from [{tableName.Replace("[", "").Replace("]", "")}] where len([{item.column_name}]) > 255 ");
                        break;
                    case DatabaseType.Mysql:
                        sql.Add($" select count(*) as `Count` from `{tableName.Replace("`", "")}` where length(`{item.column_name.Replace("`", "")}`) > 255 ");
                        break;
                    case DatabaseType.Postgresql:
                        sql.Add($" select count(*) as \"Count\" from \"{tableName}\" where length(\"{item.column_name}\") > 255 ");
                        break;
                    default:
                        break;
                }
            }

            sqlBuilder.AppendLine(string.Join(";", sql) + $";");

            _logger.LogInformation($"---------------{this.GetType().Name},sqlBuilder:{sqlBuilder.ToString()}");
        }
    }
}
