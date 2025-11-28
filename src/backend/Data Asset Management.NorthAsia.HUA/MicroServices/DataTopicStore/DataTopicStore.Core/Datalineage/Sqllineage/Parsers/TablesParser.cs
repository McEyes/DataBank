using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTopicStore.Core.Datalineage.Sqllineage.Models;

namespace DataTopicStore.Core.Datalineage.Sqllineage.Parsers
{
    public class TablesParser
    {
        public static SqllineageTableResults Extract(string content)
        {
            if (string.IsNullOrWhiteSpace(content)) return null;
            SqllineageTableResults tableResults = new SqllineageTableResults
            {
                SourceTables = new List<string>(),
                TargetTables = new List<string>(),
                IntermediateTables = new List<string>()
            };

            var tagSource = "Source Tables:";
            var tagTarget = "Target Tables:";
            var tagIntermediate = "Intermediate Tables:";
            var list = content.Split("\n", StringSplitOptions.RemoveEmptyEntries).Where(t=>!string.IsNullOrWhiteSpace(t)).Select(t=>t.Trim()).ToList();
            var iSource = list.IndexOf(tagSource);
            var iTarget = list.IndexOf(tagTarget);
            var iIntermediate = list.IndexOf(tagIntermediate);

            if (iTarget > iSource)
                tableResults.SourceTables = list.Slice(iSource + 1, iTarget - iSource - 1);

            if (iIntermediate == -1)
            {
                tableResults.TargetTables = list.Slice(iTarget + 1, list.Count - iTarget - 1);
            }
            else
            {
                tableResults.TargetTables = list.Slice(iTarget + 1, iIntermediate - iTarget - 1);
                tableResults.IntermediateTables = list.Slice(iIntermediate + 1, list.Count - iIntermediate - 1);
            }

            return tableResults;
        }
    }
}
