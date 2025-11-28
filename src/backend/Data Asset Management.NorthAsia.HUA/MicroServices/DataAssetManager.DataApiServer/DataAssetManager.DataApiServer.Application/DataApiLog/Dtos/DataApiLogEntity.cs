using ITPortal.Core.Services;
using DataAssetManager.DataApiServer.Core;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Xml;
using ITPortal.Core.LightElasticSearch;

namespace DataAssetManager.DataApiServer.Application.DataApi.Dtos
{
    [ElasticIndexName("DataApiLog", "DataAsset")]
    [SugarTable(TableName = "asset_data_api_log")]
    public class DataApiLogEntity : Entity<string>
    {
        [SugarColumn(IsPrimaryKey = true, ColumnName = "id")]
        public override string Id { get; set; }

        [SugarColumn(ColumnName = "api_id")]
        public string ApiId { get; set; }

        [SugarColumn(ColumnName = "api_name")]
        public string ApiName { get; set; }

        [SugarColumn(ColumnName = "caller_id")]
        public string CallerId { get; set; }

        [SugarColumn(ColumnName = "caller_ip")]
        public string CallerIp { get; set; }

        [SugarColumn(ColumnName = "caller_url")]
        public string CallerUrl { get; set; }

        [SugarColumn(ColumnName = "caller_params")]
        public string CallerParams { get; set; }

        public CallerParam _CallerParam;
        [SugarColumn(IsIgnore = true)]
        public CallerParam CallerParam
        {
            get
            {
                if (_CallerParam == null)
                    _CallerParam = ParseQueryInfo(CallerParams);
                 return _CallerParam;
            }
        }
        private IDictionary<string, object> _CallerParamDict;
        [SugarColumn(IsIgnore = true)]
        public IDictionary<string, object> CallerParamDict
        {
            get
            {
                if (_CallerParamDict == null)
                    _CallerParamDict = ParseQuery(CallerParams);
                return _CallerParamDict;
            }
        }

        [SugarColumn(ColumnName = "caller_date")]
        public DateTimeOffset CallerDate { get; set; }

        [SugarColumn(ColumnName = "caller_size")]
        public long CallerSize { get; set; }

        [SugarColumn(ColumnName = "time")]
        public int Time { get; set; }

        [SugarColumn(ColumnName = "msg")]
        public string Msg { get; set; }

        [SugarColumn(ColumnName = "status")]
        public int? Status { get; set; }
        /// <summary>
        /// 表所属所属部门,从拥有者所属部门获取
        /// </summary>
        [SugarColumn(ColumnName = "table_id", Length = 50)]

        public string TableId { get; set; }
        /// <summary>
        /// 表所属所属部门,从拥有者所属部门获取
        /// </summary>
        [SugarColumn(ColumnName = "owner_depart", Length = 100)]

        public string OwnerDepart { get; set; }


        public CallerParam ParseQueryInfo(string input)
        {
            input = input?.Trim('{', '}')??"";

            CallerParam queryInfo = new CallerParam();

            string[] parts = input.Split(',');
            foreach (string part in parts)
            {
                string trimmedPart = part.Trim();
                if (trimmedPart.StartsWith("pageSize="))
                {
                    queryInfo.pageSize = int.Parse(trimmedPart.Substring("pageSize=".Length));
                }
                else if (trimmedPart.StartsWith("pageNum="))
                {
                    queryInfo.pageNum = int.Parse(trimmedPart.Substring("pageNum=".Length));
                }
            }
            if (queryInfo.pageNum > 1) queryInfo.total = (queryInfo.pageNum + 1) * queryInfo.pageSize;
   
            int sqlTextStartIndex = input.IndexOf("sqlText=");
            if (sqlTextStartIndex != -1)
            {
                sqlTextStartIndex += "sqlText=".Length;

                int nextFieldIndex = int.MaxValue;
                int pageSizeIndex = input.IndexOf("pageSize=", sqlTextStartIndex);
                int pageNumIndex = input.IndexOf("pageNum=", sqlTextStartIndex);

                if (pageSizeIndex != -1)
                {
                    nextFieldIndex = Math.Min(nextFieldIndex, pageSizeIndex);
                }
                if (pageNumIndex != -1)
                {
                    nextFieldIndex = Math.Min(nextFieldIndex, pageNumIndex);
                }

                if (nextFieldIndex != int.MaxValue)
                {
                    queryInfo.sqlText = input.Substring(sqlTextStartIndex, nextFieldIndex - sqlTextStartIndex).Trim().TrimEnd(',');
                }
                else
                {
                    queryInfo.sqlText = input.Substring(sqlTextStartIndex).TrimEnd(',');
                }
            }

            return queryInfo;
        }


        public IDictionary<string, object> ParseQuery(string input)
        {
            return ParseKeyValuePairs(input);
        }

        public Dictionary<string, object> ParseKeyValuePairs(string input)
        {
            var result = new Dictionary<string, object>();
            int index = 0; 
            input = input?.Trim('{', '}').Trim()??"";
            while (index < input.Length)
            {
                int equalIndex = input.IndexOf('=', index);
                if (equalIndex == -1)
                {
                    break;
                }
                string key = input.Substring(index, equalIndex - index).Trim();

                int nextCommaIndex = FindNextCommaIndex(input, equalIndex + 1);
                string valueStr = input.Substring(equalIndex + 1, nextCommaIndex - equalIndex - 1).Trim();

                result[key] = valueStr;
                //if (int.TryParse(valueStr, out int intValue))
                //{
                //    result[key] = intValue;
                //}
                //else
                //{
                //    result[key] = valueStr;
                //}

                index = nextCommaIndex + 1;
            }

            return result;
        }

        public int FindNextCommaIndex(string input, int startIndex)
        {
            int depth = 0;
            for (int i = startIndex; i < input.Length; i++)
            {
                if (input[i] == ',')
                {
                    if (depth == 0)
                    {
                        return i;
                    }
                }
            }
            return input.Length;
        }
    }

    public class CallerParam
    {
        public int pageSize { get; set; }
        public int pageNum { get; set; }
        public string sqlText { get; set; }
        public int? total { get; set; }
    }
}