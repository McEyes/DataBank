

using ITPortal.Core.Extensions;
using ITPortal.Extension.System;


using System.Data;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Text;
using System.Text.RegularExpressions;

namespace ITPortal.Core.SqlParse
{
    // PostgreSQL
    public abstract class SqlParser : ISqlParser
    {
        public DbType SugerDbType { get; protected set; }

        public SqlParser(DbType sugerDbType)
        {
            SugerDbType = sugerDbType;
        }


        /// <summary>
        /// 冒号占位符
        /// </summary>
        protected const string ParamSymbols = ":";

        /**
         * 空格
         */
        protected const string SPACE = " ";
        /**
         * 问号占位符
         */
        protected const string MARK = "?";
        /**
         * where关键字 
         */
        protected const string WHERE_SQL = "WHERE";
        /**
         * AND连接符 
         */
        protected const string WHERE_AND = "AND";
        /**
         * where 1=1条件
         */
        protected readonly string WHERE_INIT = WHERE_SQL + " 1 = 1";
        /**
         * 左括号
         */
        protected const string LEFT_BRACKET = "(";
        /**
         * 右括号
         */
        protected const string RIGHT_BRACKET = ")";
        /**
         * 百分号%
         */
        protected const string PERCENT_SIGN = "%";
        /**
         * 单引号 '
         */
        protected const string SINGLE_QUOTE = "'";
        /**
         * 条件代码块标记开始
         */
        public const string MARK_KEY_START = "${";
        /**
         * 条件代码块标记结束
         */
        public const string MARK_KEY_END = "}";

        /// <summary>
        /// 
        /// </summary>
        public virtual string[] PagePattern => new string[] {
            @"(?i)\sLIMIT\s+(\d+)(?:\s*,\s*(\d+))?",//支持 MySQL,SQLite,PostgreSQL 的分页表达式： LIMIT m LIMIT n , LIMIT m, n
            @"(?i)\sLIMIT\s+(\d+)\s+OFFSET\s+(\d+)"//  PostgreSQL分页格式  LIMIT 1 OFFSET  23
        };

        /// <summary>
        /// 提取表名
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public virtual List<string> ExtractTableNames(string sql)
        {
            var tableNames = new List<string>();
            var pattern = @"(?i)\bFROM\s+([\w.]+)|JOIN\s+([\w.]+)";
            var matches = Regex.Matches(sql, pattern);
            foreach (Match match in matches)
            {
                if (match.Groups[1].Success)
                {
                    tableNames.Add(match.Groups[1].Value);
                }
                else if (match.Groups[2].Success)
                {
                    tableNames.Add(match.Groups[2].Value);
                }
            }
            return tableNames.Distinct().ToList();
        }

        /// <summary>
        /// 提取分页
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public virtual (bool hasPagination, string paginationInfo) ExtractPaginationInfo(string sql)
        {
            foreach (var match in PagePattern)
            {
                var result = ExtractLastPaginationInfo(sql, match);
                if (!result.hasPagination) continue;
                else return result;
            }
            return (false, "");
        }

        /// <summary>
        /// 获取最后一个分页信息
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        protected virtual (bool hasPagination, string paginationInfo) ExtractLastPaginationInfo(string sql, string pattern)
        {
            sql = sql.TrimEnd(',');
            var matches = Regex.Matches(sql, pattern);
            for (int i = matches.Count - 1; i >= 0; i--)
            {
                var match = matches[i];
                if (IsInOuterQuery(sql, match.Index))
                {
                    return (true, match.Value);
                }
            }
            return (false, string.Empty);
        }

        /// <summary>
        /// 是否 outer连接查询
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="matchIndex"></param>
        /// <returns></returns>
        protected virtual bool IsInOuterQuery(string sql, int matchIndex)
        {
            int openBracketCount = 0;
            for (int i = matchIndex - 1; i >= 0; i--)
            {
                if (sql[i] == ')')
                {
                    openBracketCount++;
                }
                else if (sql[i] == '(')
                {
                    if (openBracketCount > 0)
                    {
                        openBracketCount--;
                    }
                    else
                    {
                        return false; 
                    }
                }
            }
            return true; 
        }

        /// <summary>
        /// 移除sql分页信息
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public virtual string RemovePagination(string sql)
        {
            foreach (var pattern in PagePattern)
            {
                var result = Regex.Replace(sql, pattern, "");
                if (!result.IsNullOrWhiteSpace()) return result;
            }
            return string.Empty;
        }

        // MySQL, SQL Server, PostgreSQL, SQLite
        private static readonly Regex LimitRegex = new Regex(@"\sLIMIT\s+\d+(\s*,\s*\d+)?", RegexOptions.IgnoreCase);
        // Oracle
        private static readonly Regex RowNumRegex = new Regex(@"\sROWNUM\s*<=\s*\d+", RegexOptions.IgnoreCase);
        // SQL Server
        private static readonly Regex OffsetFetchRegex = new Regex(@"\sOFFSET\s+\d+\s+ROWS\s+FETCH\s+NEXT\s+\d+\s+ROWS\s+ONLY", RegexOptions.IgnoreCase);

        /// <summary>
        /// 是否含有分页信息
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="paginatedSql"></param>
        /// <returns></returns>
        public bool HasPagination(string sql, out string paginatedSql)
        {
            paginatedSql = sql;
            if (LimitRegex.IsMatch(sql) || RowNumRegex.IsMatch(sql) || OffsetFetchRegex.IsMatch(sql))
            {
                paginatedSql = LimitRegex.Replace(sql, "");
                paginatedSql = RowNumRegex.Replace(paginatedSql, "");
                paginatedSql = OffsetFetchRegex.Replace(paginatedSql, "");
                return true;
            }
            return false;
        }

        /// <summary>
        /// 判断sql类型：增删改查
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public virtual string GetSqlType(string sql)
        {
            var pattern = @"(?i)^\s*(SELECT|INSERT|UPDATE|DELETE)\b";
            var match = Regex.Match(sql, pattern);
            if (match.Success)
            {
                return match.Groups[1].Value.ToUpper();
            }
            return "UNKNOWN";
        }

        /// <summary>
        /// 提取查询字段
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public virtual List<string> ExtractFields(string sql)
        {
            List<string> fields = new List<string>();
            Match match = Regex.Match(sql, @"SELECT\s+(.*?)\s+FROM", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                string fieldList = match.Groups[1].Value.Trim();
                string[] fieldArray = fieldList.Split(',');
                foreach (string field in fieldArray)
                {
                    fields.Add(field.Trim());
                }
            }
            return fields;
        }

        /// <summary>
        /// 提取查询字段
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public virtual string ReplaceFields(string sql, string selectFrom)
        {
            // 定义正则表达式，忽略大小写
            Regex pattern = new Regex(@"SELECT\s+(.*?)\s+FROM", RegexOptions.IgnoreCase);
            return pattern.Replace(sql, selectFrom);
        }

        /// <summary>
        /// 提取查询字段
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public virtual string ReplaceFields(string sql, List<string> columns)
        {
            // 定义正则表达式，忽略大小写
            Regex pattern = new Regex(@"SELECT\s+(.*?)\s+FROM", RegexOptions.IgnoreCase);
            StringBuilder sqlBuilder = new StringBuilder("SELECT ");
            if (columns != null && columns.Count() > 0)
            {
                sqlBuilder.Append(string.Join(",", columns));
            }
            else
            {
                sqlBuilder.Append("*");
            }
            sqlBuilder.Append($" FROM ");
            return ReplaceFields(sql, sqlBuilder.ToString());
        }

        /// <summary>
        /// 提取查询条件
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public virtual List<string> ExtractConditions(string sql)
        {
            List<string> conditions = new List<string>();
            MatchCollection matches = Regex.Matches(sql, @"\$\{([^}]+)\}");
            foreach (Match match in matches)
            {
                conditions.Add(match.Value);
            }
            return conditions;
        }
        /// <summary>
        /// sql参数条件替换
        /// 参数格式格式  and fieldname1=:{fieldname1} and fieldname2=:{fieldname2}
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual string ReplaceConditions(string sql, Dictionary<string, object> parameters)
        {
            string pattern = @" ?\$\{([^}]+)\}";
            var sqlResult = Regex.Replace(sql, pattern, match =>
            {
                string condition = match.Groups[1].Value;
                MatchCollection paramMatches = Regex.Matches(condition, @":([^)]\w+[^)]*)");
                foreach (Match paramMatch in paramMatches)
                {
                    string paramName = paramMatch.Groups[1].Value;
                    string paramPlaceholder = $":{paramName}";
                    if (parameters.ContainsKey(paramName) && parameters[paramName] != null && parameters[paramName] != "null")
                    {
                        condition = " " + condition.Replace(paramPlaceholder, $"'{parameters[paramName]}'").Trim();
                    }
                    else
                    {
                        condition = "";
                    }
                }
                if (paramMatches.Count == 0)
                {
                    condition = " " + condition;
                }
                return condition;
            });
            if (sqlResult.Trim().EndsWith("WHERE 1 = 1")) sqlResult = sqlResult.Replace("WHERE 1 = 1", newValue: "");
            return sqlResult;
        }


        /// <summary>
        /// 根据参数定义对象构建查询条件参数
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public virtual string BuildConditions<T>(List<T> fieldParam) where T : class, IReqParam, new()//, Func<T, string> expression) where T : class, IReqParam, new()
        {
            StringBuilder conditions = new StringBuilder();
            foreach (var item in fieldParam)
            {
                BuildCondition(item, conditions);
            }
            return conditions.ToString().Trim();
        }


        /// <summary>
        /// 根据参数定义对象构建查询条件参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldParam"></param>
        /// <param name="sqlBuild"></param>
        /// <returns></returns>
        public virtual StringBuilder BuildCondition<T>(T fieldParam, StringBuilder sqlBuild) where T : class, IReqParam, new()
        {
            if (sqlBuild == null) sqlBuild = new StringBuilder();
            var whereType = fieldParam.whereType.GetWereTypeInfo();

            sqlBuild.Append(SPACE)
                .Append(MARK_KEY_START)
                .Append(WHERE_AND).Append(SPACE)
                .Append(fieldParam.paramName).Append(SPACE)
                .Append(whereType.Key).Append(SPACE);
            switch (whereType.KeyName)
            {
                case "like":
                    sqlBuild.Append("CONCAT").Append(LEFT_BRACKET)
                      .Append(SINGLE_QUOTE).Append(PERCENT_SIGN).Append(SINGLE_QUOTE).Append(",")
                      .Append(ParamSymbols).Append(fieldParam.paramName).Append(",")
                      .Append(SINGLE_QUOTE).Append(PERCENT_SIGN).Append(SINGLE_QUOTE)
                      .Append(RIGHT_BRACKET);
                    break;
                case "likel":
                    sqlBuild.Append("CONCAT").Append(LEFT_BRACKET)
                      .Append(SINGLE_QUOTE).Append(PERCENT_SIGN).Append(SINGLE_QUOTE).Append(",")
                      .Append(ParamSymbols).Append(fieldParam.paramName)
                      .Append(RIGHT_BRACKET);
                    break;
                case "liker":
                    sqlBuild.Append("CONCAT").Append(LEFT_BRACKET)
                      .Append(ParamSymbols).Append(fieldParam.paramName).Append(",")
                      .Append(SINGLE_QUOTE).Append(PERCENT_SIGN).Append(SINGLE_QUOTE)
                      .Append(RIGHT_BRACKET);
                    break;
                case "in":
                    sqlBuild.Append(LEFT_BRACKET)
                        .Append(ParamSymbols).Append(fieldParam.paramName)
                        .Append(RIGHT_BRACKET);
                    break;
                case "between":
                    sqlBuild.Append(ParamSymbols).Append(fieldParam.paramName).Append("_start").Append(SPACE)
                        .Append(WHERE_AND).Append(SPACE)
                        .Append(ParamSymbols).Append(fieldParam.paramName).Append("_end");
                    break;
                case ">":
                case ">=":
                case "<":
                case "<=":
                case "!=":
                case "=":
                case "<>":
                case "eq":
                case "ne":
                default:
                    sqlBuild.Append(ParamSymbols).Append(fieldParam.paramName);
                    break;

            }
            sqlBuild.Append(MARK_KEY_END);
            return sqlBuild;
        }

        /// <summary>
        /// 根据参数定义和实际参数构建查询条件
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public virtual string BuildConditions<T>(List<T> fieldParam, Dictionary<string, object> parameters) where T : class, IReqParam, new()//, Func<T, string> expression) where T : class, IReqParam, new()
        {
            StringBuilder conditions = new StringBuilder();
            foreach (var item in fieldParam)
            {
                if (item.whereType.GetWereType() == WhereType.BETWEEN)
                {
                    if (parameters.TryGetValue(item.paramName + "_start", out object? data_start) && data_start != null && data_start != "null"
                        && parameters.TryGetValue(item.paramName + "_end", out object? data_end) && data_end != null && data_end != "null")
                    {
                        BuildCondition(item, conditions, data_start, data_end);
                    }
                }
                else if (parameters.TryGetValue(item.paramName, out object? data) && data != null && data != "null")
                {
                    BuildCondition(item, conditions, data);
                }
            }
            return conditions.ToString().Trim();
        }


        /// <summary>
        /// 根据参数定义和实际参数构建查询条件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldParam"></param>
        /// <param name="sqlBuild"></param>
        /// <param name="data"></param>
        /// <param name="data_end"></param>
        /// <returns></returns>
        public virtual StringBuilder BuildCondition<T>(T fieldParam, StringBuilder sqlBuild, object data, object data_end = null) where T : class, IReqParam, new()
        {
            if (sqlBuild == null) sqlBuild = new StringBuilder();
            var whereType = fieldParam.whereType.GetWereTypeInfo();

            sqlBuild.Append(SPACE)
                //.Append(MARK_KEY_START)
                .Append(WHERE_AND).Append(SPACE)
                .Append(fieldParam.paramName).Append(SPACE)
                .Append(whereType.Key).Append(SPACE);
            switch (whereType.KeyName)
            {
                case "like":
                    sqlBuild.Append("CONCAT").Append(LEFT_BRACKET)
                      .Append(SINGLE_QUOTE).Append(PERCENT_SIGN).Append(SINGLE_QUOTE).Append(",")
                      .Append(data).Append(",")
                      .Append(SINGLE_QUOTE).Append(PERCENT_SIGN).Append(SINGLE_QUOTE)
                      .Append(RIGHT_BRACKET);
                    break;
                case "likel":
                    sqlBuild.Append("CONCAT").Append(LEFT_BRACKET)
                      .Append(SINGLE_QUOTE).Append(PERCENT_SIGN).Append(SINGLE_QUOTE).Append(",")
                      .Append(data)
                      .Append(RIGHT_BRACKET);
                    break;
                case "liker":
                    sqlBuild.Append("CONCAT").Append(LEFT_BRACKET)
                      .Append(data).Append(",")
                      .Append(SINGLE_QUOTE).Append(PERCENT_SIGN).Append(SINGLE_QUOTE)
                      .Append(RIGHT_BRACKET);
                    break;
                case "in":
                    sqlBuild.Append(LEFT_BRACKET)
                        .Append(SINGLE_QUOTE).Append(data).Append(SINGLE_QUOTE)
                        .Append(RIGHT_BRACKET);
                    break;
                case "between":
                    sqlBuild.Append(SINGLE_QUOTE).Append(data).Append(SINGLE_QUOTE).Append(SPACE)
                        .Append(WHERE_AND).Append(SPACE)
                        .Append(SINGLE_QUOTE).Append(data_end).Append(SINGLE_QUOTE);
                    break;
                case ">":
                case ">=":
                case "<":
                case "<=":
                case "!=":
                case "=":
                case "<>":
                case "eq":
                case "ne":
                default:
                    sqlBuild.Append(SINGLE_QUOTE).Append(data).Append(SINGLE_QUOTE);
                    break;

            }
            //sqlBuild.Append(MARK_KEY_END);
            return sqlBuild;
        }

        //public virtual ISugarQueryable<object> BuildSelectFromTable(ISqlSugarClient db, string tableName, string[] columns = null)
        //{
        //    var query = db.Queryable<object>().AS(tableName, "");
        //    if (columns != null && columns.Count() > 0)
        //    {
        //        var selector = new List<SelectModel>();
        //        foreach (var item in columns)
        //        {
        //            selector.Add(new SelectModel() { AsName = "", FieldName = item });
        //        }
        //        query = query.Select(selector);
        //    }
        //    return query;
        //}

        //public virtual StringBuilder BuildSelectFromTableToSql(ISqlSugarClient db, string tableName, string[] columns = null)
        //{
        //    StringBuilder sqlBuilder = new StringBuilder("SELECT ");
        //    if (columns != null && columns.Count() > 0)
        //    {
        //        sqlBuilder.Append(string.Join(",", columns));
        //    }
        //    else
        //    {
        //        sqlBuilder.Append("*");
        //    }
        //    sqlBuilder.Append($" FROM {tableName} {WHERE_INIT} ");
        //    return sqlBuilder;
        //}

        /// <summary>
        /// 获取数据库中表的字段sql
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="tableName"></param>
        /// <param name="schemaName"></param>
        /// <returns></returns>
        public virtual StringBuilder GetTableColumnsSql(String dbName, String tableName, string schemaName = "public")
        {
            return new StringBuilder(@$"SELECT COLUMN_NAME AS ColName,
DATA_TYPE AS DataType,
CHARACTER_MAXIMUM_LENGTH AS DataLength,
NUMERIC_PRECISION AS DataPrecision,
NUMERIC_SCALE AS DataScale,
COLUMN_KEY = 'PRI' AS ColKey,
IS_NULLABLE = 'YES' AS Nullable,
ORDINAL_POSITION AS ColPosition,
COLUMN_DEFAULT AS DataDefault,
COLUMN_COMMENT AS ColComment
FROM 
INFORMATION_SCHEMA.COLUMNS
WHERE 
TABLE_SCHEMA = '{dbName}'
AND TABLE_NAME = '{tableName}';");
        }
        /// <summary>
        /// 获取数据库中表的sql
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="schemaName"></param>
        /// <returns></returns>
        public virtual StringBuilder GetTablesSql(String dbName, string schemaName = "public")
        {
            return new StringBuilder(@$"SELECT 
TABLE_NAME as TableName, 
TABLE_COMMENT as TableComment
FROM 
INFORMATION_SCHEMA.TABLES
WHERE 
TABLE_SCHEMA = '{dbName}';");
        }

        ///// <summary>
        ///// 根据表字段数据类型配置，动态创建临时对象
        ///// </summary>
        ///// <param name="dbName"></param>
        ///// <param name="schemaName"></param>
        ///// <returns></returns>
        //public virtual Type GetTablesClassType(String tableName, List<column> columns)
        //{
        //}
    }
}