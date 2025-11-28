using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DataQualityAssessment.Application.Models.PhoneNumber;
using DataQualityAssessment.Core.Dappers;
using DataQualityAssessment.Core.Enums;
using DataQualityAssessment.Core.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace DataQualityAssessment.Application.RuleEngines.Rules
{
    /// <summary>
    /// 邮箱验证规则
    /// </summary>
    public class EmailVerificationRule : VerificationBaseRule
    {
        private readonly ILogger<EmailVerificationRule> _logger;
        protected override RuleValidateType ValidateType { get => RuleValidateType.Email; }

        public EmailVerificationRule(RuleDataContext context) : base(context)
        {
            _logger = App.GetService<ILogger<EmailVerificationRule>>();
        }

        public override async Task<double> EvaluateAsync(CancellationToken token)
        {
            OpenConnection();

            var score = 1.0;
            var tableName = DbSetting.TableName;
            var sqlBuilder = new StringBuilder();
            string[] keywords = ["email", "邮箱"];
            var columnList = new List<MetadataColumnModel>();
            CreateColumns(keywords, out columnList);

            if (columnList is List<MetadataColumnModel> { Count: > 0 })
            {
                BuildSql(tableName, sqlBuilder, columnList);

                var readList = new List<QueryResultModel>();
                var queryEffective = sqlBuilder.ToString();
                using (var gridReader = await DbConnection.QueryMultipleAsync(queryEffective))
                {
                    for (int i = 0; i < columnList.Count + 1; i++)
                    {
                        readList.Add((await gridReader.ReadAsync<QueryResultModel>()).FirstOrDefault());
                        token.ThrowIfCancellationRequested();
                    }
                }

                double validSum = 0;
                double totalSum = 0;
                var qty = readList.Count - 1;
                var columnNames = columnList.Select(t => t.column_name);
                for (int i = 0; i < qty; i++)
                {
                    // score
                    var item = readList[i];
                    var last = readList[qty];
                    ulong validCount = item.Count;
                    ulong totalCount = last.Count;

                    validSum += validCount;
                    totalSum += totalCount;

                    //score = validCount * 1.0 / totalCount;
                    //var columnName = columnNames.ElementAt(i);

                    token.ThrowIfCancellationRequested();
                }

                if (totalSum > 0)
                    score = validSum / totalSum;

                // return reports
                AddReport(RuleSetting.RuleNo, score, string.Join(",", columnNames), keywords, (ulong)validSum, (ulong)totalSum);

                return score;
            }
            else
            {
                // return reports
                AddReport(RuleSetting.RuleNo, score, "No matching columns found", keywords, 0, 0);
            }

            return score;
        }

        protected override void BuildSql(string tableName, StringBuilder sqlBuilder, List<MetadataColumnModel> columnList)
        {
            foreach (var item in columnList)
            {
                switch (DbSetting.DbType)
                {
                    case DatabaseType.SqlServer:
                        sqlBuilder.AppendLine(@"SELECT COUNT(0) as [Count]
                                    FROM [" + tableName + @"]
                                    WHERE [" + item.column_name.Replace("[", "").Replace("]","") + @"] like '_%@%_';");
                        break;
                    case DatabaseType.Mysql:
                        sqlBuilder.AppendLine(@"SELECT COUNT(0) as `Count`
                                    FROM `" + tableName + @"`
                                    WHERE `" + item.column_name.Replace("`","") + @"` REGEXP '^[A-Za-z0-9\u4e00-\u9fa5]+@[a-zA-Z0-9_-]+(\.[a-zA-Z0-9_-]+)+$';");
                        break;
                    case DatabaseType.Postgresql:
                        sqlBuilder.AppendLine(@"SELECT COUNT(0) as ""Count""
                                    FROM """ + tableName + @"""
                                    WHERE """ + item.column_name + @""" like '_%@%_';");
                        break;
                    default:
                        break;
                }

            }

            switch (DbSetting.DbType)
            {
                case DatabaseType.SqlServer:
                    sqlBuilder.AppendLine(@"SELECT COUNT(0) as ""Count"" FROM [" + tableName.Replace("[","").Replace("]","") + @"]; ");
                    break;
                case DatabaseType.Mysql:
                    sqlBuilder.AppendLine(@"SELECT COUNT(0) as `Count` FROM `" + tableName.Replace("`", "") + @"`; ");
                    break;
                case DatabaseType.Postgresql:
                    sqlBuilder.AppendLine(@"SELECT COUNT(0) as ""Count"" FROM """ + tableName + @"""; ");
                    break;
                default:
                    break;
            }

            _logger.LogInformation($"---------------{this.GetType().Name},sqlBuilder:{sqlBuilder.ToString()}");
        }
    }
}
