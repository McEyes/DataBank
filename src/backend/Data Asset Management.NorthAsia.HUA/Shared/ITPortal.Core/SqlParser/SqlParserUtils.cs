using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ITPortal.Core.SqlParse
{
    public class SqlParserUtils
    {
        private static readonly Dictionary<DatabaseType, string> PaginationPatterns = new Dictionary<DatabaseType, string>
        {
            { DatabaseType.MySql, @"LIMIT\s+(\d+)\s*(?:,\s*(\d+))?" },
            { DatabaseType.SqlServer, @"OFFSET\s+(\d+)\s+ROWS\s+FETCH\s+NEXT\s+(\d+)\s+ROWS\s+ONLY" },
            { DatabaseType.Oracle, @"ROWNUM\s*<=\s*(\d+)" },
            { DatabaseType.PostgreSql, @"LIMIT\s+(\d+)\s*(?:,\s*(\d+))?" },
            { DatabaseType.Sqlite, @"LIMIT\s+(\d+)\s*(?:,\s*(\d+))?" }
        };

        private static readonly Dictionary<DatabaseType, string> SqlTypePatterns = new Dictionary<DatabaseType, string>
        {
            { DatabaseType.MySql, @"^(SELECT|INSERT|UPDATE|DELETE)" },
            { DatabaseType.SqlServer, @"^(SELECT|INSERT|UPDATE|DELETE)" },
            { DatabaseType.Oracle, @"^(SELECT|INSERT|UPDATE|DELETE)" },
            { DatabaseType.PostgreSql, @"^(SELECT|INSERT|UPDATE|DELETE)" },
            { DatabaseType.Sqlite, @"^(SELECT|INSERT|UPDATE|DELETE)" }
        };

        public static List<string> ExtractTableNames(string sql, DatabaseType databaseType)
        {
            var pattern = @"(?i)(?:FROM|JOIN)\s+([\w\.]+)";
            var matches = Regex.Matches(sql, pattern);
            var tableNames = new List<string>();

            foreach (Match match in matches)
            {
                tableNames.Add(match.Groups[1].Value);
            }

            return tableNames;
        }

        public static PaginationInfo ExtractPaginationInfo(string sql, DatabaseType databaseType)
        {
            var pattern = PaginationPatterns[databaseType];
            var match = Regex.Match(sql, pattern);

            if (match.Success)
            {
                return new PaginationInfo
                {
                    Offset = match.Groups[1].Value,
                    Limit = match.Groups.Count > 2 ? match.Groups[2].Value : null
                };
            }

            return null;
        }

        public static string RemovePagination(string sql, DatabaseType databaseType)
        {
            var pattern = PaginationPatterns[databaseType];
            return Regex.Replace(sql, pattern, string.Empty);
        }

        public static SqlType GetSqlType(string sql, DatabaseType databaseType)
        {
            var pattern = SqlTypePatterns[databaseType];
            var match = Regex.Match(sql, pattern);

            if (match.Success)
            {
                return (SqlType)Enum.Parse(typeof(SqlType), match.Groups[1].Value.ToUpper());
            }

            return SqlType.Unknown;
        }
    }

    public enum DatabaseType
    {
        MySql,
        SqlServer,
        Oracle,
        PostgreSql,
        Sqlite
    }

    public enum SqlType
    {
        Select,
        Insert,
        Update,
        Delete,
        Unknown
    }

    public class PaginationInfo
    {
        public string Offset { get; set; }
        public string Limit { get; set; }
    }
}