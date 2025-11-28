using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataQualityAssessment.Application.Logs.Services;
using DataQualityAssessment.Core.Dappers;
using DataQualityAssessment.Core.Enums;
using DataQualityAssessment.Core.Extensions;
using DataQualityAssessment.Core.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace DataQualityAssessment.Application.RuleEngines.Rules
{
    public abstract class VerificationBaseRule : IRule
    {
        private readonly ILogger<VerificationBaseRule> _logger;
        protected List<AssessmentReportModel> _reportModels = new List<AssessmentReportModel>();
        protected RuleSettings RuleSetting;
        protected DbSettings DbSetting;
        protected IDbConnection DbConnection;
        protected IExceptionLogService _lgger;
        private static object _locker = new object();
        public VerificationBaseRule(RuleDataContext context)
        {
            DbSetting = context.DbSetting;
            RuleSetting = context.RuleSetting;
            DbConnection = context.DbConnection;
            _logger = App.GetRequiredService<ILogger<VerificationBaseRule>>();
            _lgger = App.GetRequiredService<IExceptionLogService>();
        }

        protected abstract RuleValidateType ValidateType { get; }

        protected void OpenConnection()
        {
            lock (_locker)
            {
                try
                {
                    _logger.LogInformation($"ConnectionState1>>>>>>>>>>>>>:{DbConnection.State}");
                    if (DbConnection.State == ConnectionState.Broken)
                    {
                        DbConnection.Close();
                        DbConnection.Open();
                    }
                    else if (DbConnection.State == ConnectionState.Closed)
                    {
                        DbConnection.Open();
                    }
                    _logger.LogInformation($"ConnectionState2>>>>>>>>>>>>>:{DbConnection.State}");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"{this.GetType().Name}.Mysql.Connection Exception = {ex.Message},host={DbSetting.Host},port={DbSetting.Port},db={DbSetting.DbName},dbType={DbSetting.DbType}");
                }
            }
        }
        protected abstract void BuildSql(string tableName, StringBuilder sqlBuilder, List<MetadataColumnModel> columnList);
        protected virtual void AddReport(string ruleNo, double score, string columnName, IEnumerable<string> keywords, ulong validCount, ulong totalCount, string errorMessage = null)
        {
            StringBuilder messageBuilder = new StringBuilder();
            messageBuilder.AppendLine($" Check type: {ValidateType}");
            messageBuilder.AppendLine($" Database information: host={DbSetting.Host}, database={DbSetting.DbName}, table={DbSetting.TableName}, column names={columnName} ");
            messageBuilder.AppendLine($" Matching column keywords: {string.Join(", ", keywords)} ");
            messageBuilder.AppendLine($" Valid number: {validCount} ");
            messageBuilder.AppendLine($" Total number: {totalCount} ");
            messageBuilder.AppendLine($" Other information: {errorMessage} ");

            _reportModels.Add(new AssessmentReportModel
            {
                RuleNo = ruleNo,
                RuleName = ValidateType.GetDescription(),
                Description = messageBuilder.ToString(),
                Weight = RuleSetting.Weight,
                Score = score,
                TableId = DbSetting.TableId,
                ValidateType = ValidateType,
            });
        }

        public virtual Task<double> EvaluateAsync(CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public virtual async Task<IEnumerable<AssessmentReportModel>> GetAssessmentReportsAsync()
        {
            return await Task.FromResult(_reportModels);
        }

        public virtual int GetWeight()
        {
            return RuleSetting.Weight;
        }

        public virtual Task<bool> IsValidAsync()
        {
            var weight = GetWeight();
            if (weight > 100 || weight <= 0)
            {
                throw new Exception("The weight value must be greater than 0 and less than or equal to 100");
            }
            return Task.FromResult(true);
        }

        protected virtual void CreateColumns(string[] keywords, out List<MetadataColumnModel> columnList)
        {
            columnList = new List<MetadataColumnModel>();
            if (RuleSetting.Columns == null) return;

            foreach (var kw in keywords)
            {
                var clist = RuleSetting.Columns.Where(t => t.column_comment != null && t.column_comment.Contains(kw, StringComparison.OrdinalIgnoreCase)).ToList();
                if (clist != null && clist.Count > 0)
                {
                    columnList.AddRange(clist);
                }
            }
        }
    }
}
