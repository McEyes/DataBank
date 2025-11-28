using DataTopicStore.Core.Models;

namespace DataTopicStore.Application.Helpers
{
    public static class QueryCollectionHelper
    {
        public static Dictionary<string, object> SetDefalutValue(List<ParametersInputItemModel> parametersInputs)
        {
            var queryParameters = new Dictionary<string, object>();
            foreach (var paramItem in parametersInputs)
            {
                //var ett = parametersInputs.FirstOrDefault(t => t.Name == item.Key);
                if (paramItem != null)
                {
                    //"string", "number", "integer", "boolean", "array", "object"
                    switch (paramItem.DataType)
                    {
                        case "string":
                            queryParameters.Add(paramItem.Name, paramItem.DefaultValue.ToString());
                            break;
                        case "number":
                            queryParameters.Add(paramItem.Name, Convert.ToDecimal(paramItem.DefaultValue));
                            break;
                        case "integer":
                            {
                                if (paramItem.Format == "int64")
                                    queryParameters.Add(paramItem.Name, Convert.ToInt64(paramItem.DefaultValue));
                                else
                                    queryParameters.Add(paramItem.Name, Convert.ToInt32(paramItem.DefaultValue));
                            }
                            break;
                        case "boolean":
                            queryParameters.Add(paramItem.Name, Convert.ToBoolean(paramItem.DefaultValue));
                            break;
                        case "array":
                            {
                                switch (paramItem.Format)
                                {
                                    case "int32":
                                    case "int64":
                                    case "float":
                                    case "double":
                                        queryParameters.Add(paramItem.Name, paramItem.DefaultValue);
                                        break;
                                    case "string":
                                        queryParameters.Add(paramItem.Name, "'" + string.Join("','", paramItem.DefaultValue.Split(",")) + "'");
                                        break;
                                    default:
                                        break;
                                }
                            }

                            break;
                        default:
                            throw new NotSupportedException();
                    }
                }
            }

            return queryParameters;
        }

        public static Dictionary<string, object> QueryToDictionary(IQueryCollection input, List<ParametersInputItemModel> parametersInputs)
        {
            var queryParameters = new Dictionary<string, object>();
            foreach (var item in input)
            {
                var ett = parametersInputs.FirstOrDefault(t => t.Name == item.Key);
                if (ett != null)
                {
                    //"string", "number", "integer", "boolean", "array", "object"
                    switch (ett.DataType)
                    {
                        case "string":
                            queryParameters.Add(item.Key, item.Value.ToString());
                            break;
                        case "number":
                            queryParameters.Add(item.Key, Convert.ToDecimal(item.Value.ToString()));
                            break;
                        case "integer":
                            {
                                if (ett.Format == "int64")
                                    queryParameters.Add(item.Key, Convert.ToInt64(item.Value.ToString()));
                                else
                                    queryParameters.Add(item.Key, Convert.ToInt32(item.Value.ToString()));
                            }
                            break;
                        case "boolean":
                            queryParameters.Add(item.Key, Convert.ToBoolean(item.Value.ToString()));
                            break;
                        case "array":
                            {
                                switch (ett.Format)
                                {
                                    case "int32":
                                    case "int64":
                                    case "float":
                                    case "double":
                                        queryParameters.Add(item.Key, item.Value.ToString());
                                        break;
                                    case "string":
                                        queryParameters.Add(item.Key, "'"+ string.Join("','", item.Value.ToArray()) + "'");
                                        break;
                                    default:
                                        break;
                                }
                            }
                            
                            break;
                        default:
                            throw new NotSupportedException();
                    }
                }
            }

            return queryParameters;
        }
    }
}
