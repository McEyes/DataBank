using ITPortal.Core.Services;
using DataAssetManager.DataApiServer.Core;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Xml;
using StackExchange.Profiling.Internal;

namespace DataAssetManager.DataApiServer.Application.DataApi.Dtos
{
    public class DataApiLogDto : PageEntity<string>
    {

        public string ApiId { get; set; }

        public string ApiName { get; set; }

        public string CallerId { get; set; }

        public string CallerIp { get; set; }

        public string CallerUrl { get; set; }

        public string CallerParams { get; set; }

        public CallerParam _CallerParam;
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
        public IDictionary<string, object> CallerParamDict
        {
            get
            {
                if (_CallerParamDict == null)
                    _CallerParamDict = ParseQuery(CallerParams);
                return _CallerParamDict;
            }
        }

        public DateTimeOffset CallerDate { get; set; }

        public int CallerSize { get; set; }

        public int Time { get; set; }

        public string Msg { get; set; }

        public int? Status { get; set; }


        public CallerParam ParseQueryInfo(string input)
        {
            CallerParam queryInfo = new CallerParam();
            if (string.IsNullOrWhiteSpace(input)) return queryInfo;
            input = input.Trim('{', '}');


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
            if (input.IsNullOrWhiteSpace()) return result;
            int index = 0; 
            input = input.Trim('{', '}').Trim();
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

}