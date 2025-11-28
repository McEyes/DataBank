using Dm;

using ITPortal.Core.Extensions;

using SqlSugar;

using StackExchange.Profiling.Internal;

using System.Data.Common;
using System.Data.SqlTypes;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace ITPortal.Core.SqlParse
{
    // PostgreSQL
    public abstract class SqlParser : ISqlParser
    {
        public SqlSugar.DbType SugerDbType { get; protected set; }

        public SqlParser(SqlSugar.DbType sugerDbType)
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
        /// 提取查询字段，支持包含各种函数和关键字的复杂字段
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns>字段列表</returns>
        public virtual List<string> ExtractFields(string sql)
        {
            List<string> fields = new List<string>();
            if (string.IsNullOrWhiteSpace(sql))
                return fields;

            // 清理SQL，移除注释（简单处理）
            string cleanedSql = CleanSql(sql);

            // 增强正则：支持更多关键字和复杂查询模式
            var match = Regex.Match(cleanedSql,
                @"SELECT\s+
                (DISTINCT|ALL|TOP\s+\d+(\s+PERCENT)?|SQL_CALC_FOUND_ROWS|SQL_SMALL_RESULT|SQL_BIG_RESULT|
                 SQL_BUFFER_RESULT|STRAIGHT_JOIN|HIGH_PRIORITY|SQL_CACHE|SQL_NO_CACHE|WITH\s+TIES|
                 USE\s+INDEX|IGNORE\s+INDEX|FORCE\s+INDEX)?\s*
                (.*?)\s+
                (FROM|INTO|CUBE|ROLLUP|GROUPING\s+SETS|PIVOT|UNPIVOT|OFFSET)",
                RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);

            if (match.Success)
            {
                string fieldList = match.Groups[3].Value.Trim();
                string[] fieldArray = SplitFields(fieldList);

                foreach (string field in fieldArray)
                {
                    if (!string.IsNullOrWhiteSpace(field))
                        fields.Add(field.Trim());
                }
            }

            return fields;
        }

        /// <summary>
        /// 清理SQL，移除注释和多余空格
        /// </summary>
        private string CleanSql(string sql)
        {
            // 移除单行注释
            sql = Regex.Replace(sql, @"--.*?$", "", RegexOptions.Multiline);
            // 移除多行注释
            sql = Regex.Replace(sql, @"/\*.*?\*/", "", RegexOptions.Singleline);
            // 合并多个空格为一个
            return Regex.Replace(sql, @"\s+", " ");
        }

        /// <summary>
        /// 智能分割字段列表，忽略括号内的逗号和关键字内部的逗号
        /// </summary>
        private string[] SplitFields(string fieldList)
        {
            List<string> fields = new List<string>();
            int startIndex = 0;
            int bracketDepth = 0; // 括号深度：0表示不在括号内
            bool inString = false; // 是否在字符串内部
            char stringDelimiter = '\0'; // 字符串分隔符（'或"）

            for (int i = 0; i < fieldList.Length; i++)
            {
                char c = fieldList[i];

                // 处理字符串
                if (c == '\'' || c == '"')
                {
                    if (inString && c == stringDelimiter)
                    {
                        inString = false;
                        stringDelimiter = '\0';
                    }
                    else if (!inString)
                    {
                        inString = true;
                        stringDelimiter = c;
                    }
                    continue;
                }

                if (inString)
                    continue;

                // 跟踪括号深度
                if (c == '(')
                {
                    bracketDepth++;
                }
                else if (c == ')')
                {
                    if (bracketDepth > 0)
                        bracketDepth--;
                }
                // 只有当不在括号内且遇到逗号时，才视为字段分隔符
                else if (c == ',' && bracketDepth == 0)
                {
                    string field = fieldList.Substring(startIndex, i - startIndex).Trim();
                    if (!string.IsNullOrEmpty(field))
                        fields.Add(field);
                    startIndex = i + 1; // 跳过逗号
                }
            }

            // 添加最后一个字段
            string lastField = fieldList.Substring(startIndex).Trim();
            if (!string.IsNullOrEmpty(lastField))
                fields.Add(lastField);

            return fields.ToArray();
        }


        ///// <summary>
        ///// 提取查询字段
        ///// </summary>
        ///// <param name="sql"></param>
        ///// <returns></returns>
        //public virtual List<string> ExtractFields(string sql)
        //{
        //    List<string> fields = new List<string>();
        //    Match match = Regex.Match(sql, @"SELECT\s+(.*?)\s+FROM", RegexOptions.IgnoreCase);
        //    if (match.Success)
        //    {
        //        string fieldList = match.Groups[1].Value.Trim();
        //        string[] fieldArray = fieldList.Split(',');
        //        foreach (string field in fieldArray)
        //        {
        //            fields.Add(field.Trim());
        //        }
        //    }
        //    return fields;
        //}


        ///// <summary>
        ///// 提取查询字段，支持包含各种函数的复杂字段
        ///// </summary>
        ///// <param name="sql">SQL语句</param>
        ///// <returns>字段列表</returns>
        //public virtual List<string> ExtractFields(string sql)
        //{
        //    List<string> fields = new List<string>();
        //    if (string.IsNullOrWhiteSpace(sql))
        //        return fields;

        //    // 改进正则：支持DISTINCT关键字，兼容多行SQL，捕获SELECT到FROM之间的内容
        //    var match = Regex.Match(sql, @"SELECT\s+(DISTINCT\s+)?(.*?)\s+FROM",
        //        RegexOptions.IgnoreCase | RegexOptions.Singleline);

        //    if (match.Success)
        //    {
        //        string fieldList = match.Groups[2].Value.Trim();
        //        string[] fieldArray = SplitFields(fieldList);

        //        foreach (string field in fieldArray)
        //        {
        //            if (!string.IsNullOrWhiteSpace(field))
        //                fields.Add(field.Trim());
        //        }
        //    }

        //    return fields;
        //}

        ///// <summary>
        ///// 智能分割字段列表，忽略括号内的逗号
        ///// </summary>
        ///// <param name="fieldList">字段列表字符串</param>
        ///// <returns>分割后的字段数组</returns>
        //private string[] SplitFields(string fieldList)
        //{
        //    List<string> fields = new List<string>();
        //    int startIndex = 0;
        //    int bracketDepth = 0; // 括号深度：0表示不在括号内

        //    for (int i = 0; i < fieldList.Length; i++)
        //    {
        //        char c = fieldList[i];

        //        // 跟踪括号深度
        //        if (c == '(')
        //        {
        //            bracketDepth++;
        //        }
        //        else if (c == ')')
        //        {
        //            if (bracketDepth > 0)
        //                bracketDepth--;
        //        }
        //        // 只有当不在括号内且遇到逗号时，才视为字段分隔符
        //        else if (c == ',' && bracketDepth == 0)
        //        {
        //            string field = fieldList.Substring(startIndex, i - startIndex).Trim();
        //            if (!string.IsNullOrEmpty(field))
        //                fields.Add(field);
        //            startIndex = i + 1; // 跳过逗号
        //        }
        //    }

        //    // 添加最后一个字段
        //    string lastField = fieldList.Substring(startIndex).Trim();
        //    if (!string.IsNullOrEmpty(lastField))
        //        fields.Add(lastField);

        //    return fields.ToArray();
        //}


        ///// <summary>
        ///// 提取查询字段
        ///// </summary>
        ///// <param name="sql"></param>
        ///// <returns></returns>
        //public virtual string ReplaceFields(string sql, string selectFrom)
        //{
        //    // 定义正则表达式，忽略大小写
        //    Regex pattern = new Regex(@"SELECT\s+(.*?)\s+FROM", RegexOptions.IgnoreCase);
        //    return pattern.Replace(sql, selectFrom);
        //}
        /// <summary>
        /// 替换SQL中最外层SELECT的字段，保留嵌套子查询不变
        /// </summary>
        /// <param name="sql">原始SQL语句</param>
        /// <param name="selectFrom">替换后的SELECT ... FROM部分</param>
        /// <returns>替换后的完整SQL</returns>
        public virtual string ReplaceFields(string sql, string selectFrom)
        {
            if (string.IsNullOrWhiteSpace(sql) || string.IsNullOrWhiteSpace(selectFrom))
                return sql;

            // 清理SQL，移除注释和多余空格，便于处理
            string cleanedSql = CleanSql(sql);

            // 找到最外层SELECT的位置
            int selectIndex = FindOuterSelectIndex(cleanedSql);
            if (selectIndex == -1)
                return sql; // 没有找到有效的SELECT子句

            // 找到与最外层SELECT匹配的FROM位置
            int fromIndex = FindMatchingFrom(cleanedSql, selectIndex);
            if (fromIndex == -1)
                return sql; // 没有找到匹配的FROM子句

            // 提取最外层SELECT ... FROM之外的部分
            string prefix = cleanedSql.Substring(0, selectIndex);
            string suffix = cleanedSql.Substring(fromIndex);

            // 组合替换后的SQL
            return $"{prefix} {selectFrom} {suffix}";
        }

        /// <summary>
        /// 查找最外层SELECT的起始索引（忽略子查询中的SELECT）
        /// </summary>
        private int FindOuterSelectIndex(string sql)
        {
            int index = 0;
            int bracketDepth = 0;
            bool inString = false;
            char stringDelimiter = '\0';

            while (index <= sql.Length - 6) // "SELECT"长度为6
            {
                // 处理字符串状态
                if (sql[index] == '\'' || sql[index] == '"')
                {
                    if (inString && sql[index] == stringDelimiter)
                    {
                        inString = false;
                        stringDelimiter = '\0';
                    }
                    else if (!inString)
                    {
                        inString = true;
                        stringDelimiter = sql[index];
                    }
                    index++;
                    continue;
                }

                if (inString)
                {
                    index++;
                    continue;
                }

                // 处理括号深度
                if (sql[index] == '(')
                {
                    bracketDepth++;
                    index++;
                    continue;
                }
                else if (sql[index] == ')')
                {
                    if (bracketDepth > 0)
                        bracketDepth--;
                    index++;
                    continue;
                }

                // 当不在括号内且找到SELECT关键字时，返回其位置
                if (bracketDepth == 0 &&
                    string.Compare(sql, index, "SELECT", 0, 6, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return index;
                }

                index++;
            }

            return -1;
        }

        /// <summary>
        /// 查找与最外层SELECT匹配的FROM位置
        /// </summary>
        private int FindMatchingFrom(string sql, int selectStartIndex)
        {
            int index = selectStartIndex + 6; // 从SELECT之后开始查找
            int bracketDepth = 0;
            bool inString = false;
            char stringDelimiter = '\0';

            while (index <= sql.Length - 4) // "FROM"长度为4
            {
                // 处理字符串状态
                if (sql[index] == '\'' || sql[index] == '"')
                {
                    if (inString && sql[index] == stringDelimiter)
                    {
                        inString = false;
                        stringDelimiter = '\0';
                    }
                    else if (!inString)
                    {
                        inString = true;
                        stringDelimiter = sql[index];
                    }
                    index++;
                    continue;
                }

                if (inString)
                {
                    index++;
                    continue;
                }

                // 处理括号深度
                if (sql[index] == '(')
                {
                    bracketDepth++;
                    index++;
                    continue;
                }
                else if (sql[index] == ')')
                {
                    if (bracketDepth > 0)
                        bracketDepth--;
                    index++;
                    continue;
                }

                // 当不在括号内且找到FROM关键字时，返回其位置
                if (bracketDepth == 0 &&
                    string.Compare(sql, index, "FROM", 0, 4, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return index;
                }

                index++;
            }

            return -1;
        }

        /// <summary>
        /// 提取查询字段
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public virtual string ReplaceFields(string sql, List<string> columns,string defaultField="*")
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
                sqlBuilder.Append(defaultField);
            }
            //sqlBuilder.Append($" FROM ");
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
                MatchCollection paramMatches = Regex.Matches(condition, @":([^)]*\w+[^)]*)");
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

        public virtual ISugarQueryable<object> BuildSelectFromTable(ISqlSugarClient db, string tableName, string[] columns = null)
        {
            var query =  db.SqlQueryable<object>($"select * from {tableName}");// db.Queryable<object>().AS(tableName, "");// 
            if (columns != null && columns.Count() > 0)
            {
                var selector = new List<SelectModel>();
                foreach (var item in columns)
                {
                    selector.Add(new SelectModel() { AsName = "", FieldName = item });
                }
                query = query.Select(selector);
            }
            return query;
        }

        public virtual StringBuilder BuildSelectFromTableToSql(ISqlSugarClient db, string tableName, string[] columns = null)
        {
            StringBuilder sqlBuilder = new StringBuilder("SELECT ");
            if (columns != null && columns.Count() > 0)
            {
                sqlBuilder.Append(string.Join(",", columns));
            }
            else
            {
                sqlBuilder.Append("*");
            }
            sqlBuilder.Append($" FROM {tableName} {WHERE_INIT} ");
            return sqlBuilder;
        }

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
case when is_nullable = 'YES' then 1 else 0 end AS Nullable,
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

        /// <summary>
        /// 从SQL语句中提取排序信息
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns>排序信息集合</returns>
        public virtual List<OrderByModel> ExtractOrderInfos(string sql)
        {
            if (string.IsNullOrEmpty(sql))
                return new List<OrderByModel>();
            try
            {  
                // 预处理SQL，移除注释和多余空格
                string processedSql = PreprocessSql(sql);

                // 提取ORDER BY子句
                string orderByClause = ExtractOrderByClause(processedSql);
                if (string.IsNullOrEmpty(orderByClause))
                    return new List<OrderByModel>();

                // 解析排序字段和方向
                return ParseOrderByClause(orderByClause);
            }catch(Exception ex)
            {

            }
            return new List<OrderByModel>();
        }

        /// <summary>
        /// 预处理SQL
        /// </summary>
        /// <param name="sql">原始SQL</param>
        /// <returns>处理后的SQL</returns>
        protected virtual string PreprocessSql(string sql)
        {
            if (string.IsNullOrEmpty(sql))
                return string.Empty;

            // 移除注释
            string processed = Regex.Replace(sql, @"/\*.*?\*/", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            processed = Regex.Replace(processed, @"--.*?$", "", RegexOptions.Multiline | RegexOptions.IgnoreCase);

            // 替换多个空格为单个空格
            processed = Regex.Replace(processed, @"\s+", " ");

            return processed.Trim();
        }

        ///// <summary>
        ///// 提取ORDER BY子句
        ///// </summary>
        ///// <param name="sql">处理后的SQL</param>
        ///// <returns>ORDER BY子句内容</returns>
        //protected virtual string ExtractOrderByClause(string sql)
        //{
        //    // 查找ORDER BY的位置
        //    int orderByIndex = sql.IndexOf("ORDER BY", StringComparison.OrdinalIgnoreCase);
        //    if (orderByIndex == -1)
        //        return null;

        //    // 查找后续可能的LIMIT/OFFSET/ROW_NUMBER等关键字，确定ORDER BY子句的结束位置
        //    string orderByPart = sql.Substring(orderByIndex + 9).Trim();

        //    // 常见的ORDER BY之后的关键字
        //    string[] endKeywords = { "LIMIT", "OFFSET", "FETCH", "FOR", "OPTION", "UNION", "EXCEPT", "INTERSECT", "ROW_NUMBER" };
        //    int minEndIndex = int.MaxValue;

        //    foreach (var keyword in endKeywords)
        //    {
        //        int index = orderByPart.IndexOf(keyword, StringComparison.OrdinalIgnoreCase);
        //        if (index != -1 && index < minEndIndex)
        //        {
        //            minEndIndex = index;
        //        }
        //    }

        //    if (minEndIndex != int.MaxValue)
        //    {
        //        orderByPart = orderByPart.Substring(0, minEndIndex).TrimEnd(',', ' ');
        //    }

        //    return orderByPart;
        //}

        /// <summary>
        /// 提取顶级ORDER BY子句，忽略函数内部的ORDER BY
        /// </summary>
        /// <param name="sql">处理后的SQL</param>
        /// <returns>顶级ORDER BY子句内容</returns>
        protected virtual string ExtractOrderByClause(string sql)
        {
            // 跟踪括号嵌套层级，0表示顶级
            int nestingLevel = 0;
            // 查找顶级的ORDER BY
            for (int i = 0; i <= sql.Length - 8; i++)
            {
                // 检查是否为ORDER BY关键字（不区分大小写）
                if (string.Compare(sql.Substring(i, 8), "ORDER BY", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    // 确保在顶级（不在任何括号内）
                    if (nestingLevel == 0)
                    {
                        // 提取ORDER BY后面的部分
                        string orderByPart = sql.Substring(i + 8).Trim();

                        // 常见的ORDER BY之后的关键字
                        string[] endKeywords = { "LIMIT", "OFFSET", "FETCH", "FOR", "OPTION", "UNION", "EXCEPT", "INTERSECT" };
                        int minEndIndex = int.MaxValue;

                        foreach (var keyword in endKeywords)
                        {
                            int index = FindKeywordInTopLevel(orderByPart, keyword);
                            if (index != -1 && index < minEndIndex)
                            {
                                minEndIndex = index;
                            }
                        }

                        if (minEndIndex != int.MaxValue)
                        {
                            orderByPart = orderByPart.Substring(0, minEndIndex).TrimEnd(',', ' ');
                        }

                        return orderByPart;
                    }
                }
                // 更新括号嵌套层级
                else if (sql[i] == '(')
                {
                    nestingLevel++;
                }
                else if (sql[i] == ')')
                {
                    if (nestingLevel > 0)
                        nestingLevel--;
                }
            }

            // 未找到顶级ORDER BY
            return null;
        }

        /// <summary>
        /// 在顶级（非括号内）查找关键字
        /// </summary>
        private int FindKeywordInTopLevel(string text, string keyword)
        {
            int nestingLevel = 0;
            for (int i = 0; i <= text.Length - keyword.Length; i++)
            {
                if (nestingLevel == 0)
                {
                    if (string.Compare(text.Substring(i, keyword.Length), keyword, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        // 确保是完整的关键字（前后不是字母数字）
                        bool isWholeWord = true;
                        if (i > 0 && char.IsLetterOrDigit(text[i - 1]))
                            isWholeWord = false;
                        if (i + keyword.Length < text.Length && char.IsLetterOrDigit(text[i + keyword.Length]))
                            isWholeWord = false;

                        if (isWholeWord)
                            return i;
                    }
                }

                // 更新括号嵌套层级
                if (text[i] == '(')
                {
                    nestingLevel++;
                }
                else if (text[i] == ')')
                {
                    if (nestingLevel > 0)
                        nestingLevel--;
                }
            }
            return -1;
        }


        /// <summary>
        /// 解析ORDER BY子句，提取排序字段和方向
        /// </summary>
        /// <param name="orderByClause">ORDER BY子句内容</param>
        /// <returns>排序信息集合</returns>
        protected virtual List<OrderByModel> ParseOrderByClause(string orderByClause)
        {
            List<OrderByModel> orderInfos = new List<OrderByModel>();

            if (string.IsNullOrEmpty(orderByClause))
                return orderInfos;

            // 分割多个排序字段
            string[] orderParts = Regex.Split(orderByClause, @",\s*(?![^()]*\))");

            foreach (string part in orderParts)
            {
                string trimmedPart = part.Trim();
                if (string.IsNullOrEmpty(trimmedPart))
                    continue;

                // 匹配排序方向
                Match match = Regex.Match(trimmedPart, @"(?<field>.*?)\s+(?<sort>ASC|DESC)\s*$", RegexOptions.IgnoreCase);

                if (match.Success)
                {
                    string field = CleanFieldName(match.Groups["field"].Value.Trim());
                    string sort = match.Groups["sort"].Value.Trim().ToUpper();
                    orderInfos.Add(new OrderByModel() { FieldName = field, OrderByType = (sort == "ASC") ? OrderByType.Asc : OrderByType.Desc });
                }
                else
                {
                    // 如果没有指定排序方向，默认为ASC
                    string field = CleanFieldName(trimmedPart);
                    orderInfos.Add(new OrderByModel() { FieldName = field, OrderByType = OrderByType.Asc });
                }
            }

            return orderInfos;
        }

        /// <summary>
        /// 清理字段名，移除可能的函数和别名
        /// </summary>
        /// <param name="fieldName">原始字段名</param>
        /// <returns>清理后的字段名</returns>
        protected virtual string CleanFieldName(string fieldName)
        {
            if (string.IsNullOrEmpty(fieldName))
                return fieldName;

            // 处理可能的别名
            string[] aliasParts = Regex.Split(fieldName, @"\s+AS\s+", RegexOptions.IgnoreCase);
            if (aliasParts.Length > 1)
            {
                fieldName = aliasParts[0].Trim();
            }

            // 处理可能的表别名
            string[] tableAliasParts = fieldName.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (tableAliasParts.Length > 1)
            {
                return tableAliasParts[tableAliasParts.Length - 1].Trim('"', '[', ']', '`');
            }

            // 处理可能的函数包裹，如MAX(field)  带函数直接返回
            //Match functionMatch = Regex.Match(fieldName, @"\w+\((?<field>.*?)\)", RegexOptions.IgnoreCase);
            //if (functionMatch.Success)
            //{
            //    return CleanFieldName(functionMatch.Groups["field"].Value);
            //}

            return fieldName.Trim('"', '[', ']', '`');
        }
    }
}