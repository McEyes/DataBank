using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTopicStore.Core.Datalineage.Openlineage.Models.ColumnLineages;
using DataTopicStore.Core.Datalineage.Sqllineage.Models;

namespace DataTopicStore.Core.Datalineage.Sqllineage.Parsers
{
    public class ColumnLineageParser
    {
        private static string inputFields = "inputFields";
        private static string datatopicstore = "datatopicstore";

        public static Dictionary<string, object> Extract(string content)
        {
            if (string.IsNullOrWhiteSpace(content)) return null;
            var list = content.Split("\n", StringSplitOptions.RemoveEmptyEntries).Where(t => !string.IsNullOrWhiteSpace(t)).Select(t => t.Trim()).ToList();
            Dictionary<string, object> columnLineages = new Dictionary<string, object>();
            foreach (var item in list)
            {
                var fields = item.Split("<-");
                if (fields.Length == 2)
                {
                    var key = fields[0].Trim();
                    var value = fields[1].Trim();
                    var columnName = key.Substring(key.LastIndexOf(".") + 1);
                    if (columnLineages.ContainsKey(columnName))
                    {
                        var dict = (Dictionary<string, object>)columnLineages[columnName];
                        if (dict[inputFields] is List<InputField> fieldList)
                        {
                            fieldList.Add(new InputField
                            {
                                Namespace = datatopicstore,
                                name = value.Substring(0, value.LastIndexOf(".")),
                                field = value.Substring(value.LastIndexOf(".") + 1)
                            });
                        }
                    }
                    else
                    {
                        columnLineages.Add(columnName, new Dictionary<string, object>
                        {
                            {
                                inputFields,new List<InputField>
                                {
                                     new InputField
                                     {
                                        Namespace = datatopicstore,
                                        name = value.Substring(0, value.LastIndexOf(".")),
                                        field = value.Substring(value.LastIndexOf(".")+1)
                                     }
                                }
                            }
                        });
                    }
                }
            }

            return columnLineages;
        }
    }
}
