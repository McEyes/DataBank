using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTopicStore.Core.Models;
using Dm.util;

namespace DataTopicStore.Application.Helpers
{
    public class SqlParamHandler
    {
        public static string ReplaceParam(string sql, List<ParametersInputItemModel> parametersInputs, Dictionary<string, object> paramValues)
        {
            foreach (var item in parametersInputs)
            {
                if (paramValues.ContainsKey(item.Name))
                {
                    switch (item.DataType)
                    {
                        //"float", "double", "int32", "int64", "string"
                        case "array":
                            {
                                sql = sql.Replace($"@{item.Name}", paramValues[item.Name].ToString());
                            }
                            break;
                        default:
                            break;
                    }
                }
                
            }

            return sql;
        }
    }
}
