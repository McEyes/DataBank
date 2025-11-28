using System.Text.RegularExpressions;

namespace ITPortal.Core.Extensions.Db
{
    public static class SqlExtension
    {
        private static readonly string REGEX = "(?i)\\bFROM\\s+(?:`?([\\w]+)`?\\.)?`?([\\w]+)`?|\\bJOIN\\s+(?:`?([\\w]+)`?\\.)?`?([\\w]+)`?";
        private static readonly Regex PATTERN = new Regex(REGEX);

        public static HashSet<string> GetTableNames(string sql)
        {
            if (!string.IsNullOrWhiteSpace(sql))
            {
                var matcher = PATTERN.Matches(sql);
                var tableNamesSet = new HashSet<string>();

                foreach (Match match in matcher)
                {
                    // 优先捕获 FROM 后面的表名，若为空，则捕获 JOIN 后面的表名
                    var tableName = match.Groups[2].Success ? match.Groups[2].Value : match.Groups[4].Value;
                    if (!string.IsNullOrEmpty(tableName))
                    {
                        tableNamesSet.Add(tableName);
                    }
                }
                return tableNamesSet;
            }
            return new HashSet<string>();
        }

        public static string ExtractLimit(string sql)
        {
            var regex = "(?i)LIMIT\\s+(\\d+)(?:\\s*,\\s*(\\d+))?";
            var pattern = new Regex(regex);
            var matcher = pattern.Match(sql);

            if (matcher.Success)
            {
                var limitValue = matcher.Groups[1].Value;
                // If there's a second number (for offset, e.g., LIMIT 10, 100), capture it as well.
                if (matcher.Groups.Count > 1 && matcher.Groups[2].Success)
                {
                    var group = matcher.Groups[2].Value;
                    limitValue += "," + group;
                }
                return limitValue;
            }
            return string.Empty;  // Return an empty string if no LIMIT clause is found
        }
    }
}
