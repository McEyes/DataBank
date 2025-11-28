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
    /// 数据可解释性_数据字典完整性规则
    /// </summary>
    public class DataInterpretabilityVerificationRule : VerificationBaseRule
    {
        private readonly ILogger<DataInterpretabilityVerificationRule> _logger;
        protected override RuleValidateType ValidateType { get => RuleValidateType.DataInterpretability; }
        public DataInterpretabilityVerificationRule(RuleDataContext context) : base(context)
        {
            _logger = App.GetService<ILogger<DataInterpretabilityVerificationRule>>();
        }

        public override async Task<double> EvaluateAsync(CancellationToken token)
        {
            OpenConnection();

            var score = 1.0;
            var tableName = DbSetting.TableName;
            var sqlBuilder = new StringBuilder();
            var keywords = new string[] { };
            var columnList = RuleSetting.Columns;
            var errorMessage = new StringBuilder();

            ulong validCount = 0L;
            ulong allCount = 0L;

            BuildSql_schema(tableName, sqlBuilder);
            var queryEffective = sqlBuilder.ToString();
            if (string.IsNullOrWhiteSpace(queryEffective))
            {
                validCount = 1;
                allCount = 1;
            }
            else
            {
                var invalidCount = 0;
                using (var gridReader = await DbConnection.QueryMultipleAsync(queryEffective))
                {
                    var dataRow_table = await gridReader.ReadAsync<InformationSchemaTableModel>();
                    var table_comment_entity = dataRow_table.FirstOrDefault();

                    if (table_comment_entity != null && string.IsNullOrWhiteSpace(table_comment_entity.TABLE_COMMENT))
                        invalidCount++;
                    allCount++;

                    var dataRow_column = await gridReader.ReadAsync<InformationSchemaColumnModel>();
                    if (dataRow_column != null)
                    {
                        invalidCount += dataRow_column.Count(t => string.IsNullOrWhiteSpace(t.COLUMN_COMMENT));
                        allCount += (ulong)dataRow_column.Count();

                        foreach (var item in dataRow_column)
                        {
                            if ((item.IS_NULLABLE == "NO" && string.IsNullOrWhiteSpace(item.COLUMN_DEFAULT)) || (item.COLUMN_TYPE.Contains("varchar") && item.CHARACTER_MAXIMUM_LENGTH > 255))
                            {
                                invalidCount++;
                            }

                            allCount++;
                        }
                    }
                }

                validCount = allCount - (ulong)invalidCount;
            }

            score = validCount * 1.0 / allCount;

            // return reports
            AddReport(RuleSetting.RuleNo, score, "all table", keywords, validCount, allCount, errorMessage.ToString());

            return await Task.FromResult(score);
        }

        private void BuildSql_schema(string tableName, StringBuilder sqlBuilder)
        {
            switch (DbSetting.DbType)
            {
                case DatabaseType.SqlServer:
                    break;
                case DatabaseType.Mysql:
                    sqlBuilder.AppendLine(@"SELECT TABLE_COMMENT from information_schema.`tables` where LOWER(`TABLE_NAME`)  = LOWER('" + tableName + @"');");
                    sqlBuilder.AppendLine(@"SELECT COLUMN_COMMENT,IS_NULLABLE ,COLUMN_TYPE,COLUMN_DEFAULT,CHARACTER_MAXIMUM_LENGTH ,DATA_TYPE from information_schema.`columns` where LOWER(`TABLE_NAME`)  = LOWER('" + tableName + @"');");
                    break;
                case DatabaseType.Postgresql:
                    break;
                default:
                    break;
            }
        }

        protected override void BuildSql(string tableName, StringBuilder sqlBuilder, List<MetadataColumnModel> columnList)
        {

        }
    }
}
