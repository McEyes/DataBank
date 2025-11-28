using ITPortal.Core.Html;
using ITPortal.Extension.System;
using ITPortal.Flow.Application.EmailSendRecord.Dtos;
using ITPortal.Flow.Application.EmailTemplate.Dtos;

using Newtonsoft.Json.Linq;

using System.Text;
using System.Text.RegularExpressions;

namespace ITPortal.Flow.Application
{
    public class ApiEmailHelper
    {

        public const string FOR_Regex = "(\\$FOR\\$)([\\W\\w]*?)(\\$EFOR\\$)";

        public static bool IsForTemp(string tmplStr)
        {
            return (new System.Text.RegularExpressions.Regex(FOR_Regex, RegexOptions.IgnoreCase | RegexOptions.Multiline)).IsMatch(tmplStr);
        }

        public static string ReplaceForEarchTempl(string tmplStr, IList<dynamic> list, IList<dynamic> paramData = null)
        {
            /// $FOR$   $EFOR$
            if (list == null) return tmplStr;
            StringBuilder sb = new StringBuilder();
            var reg = new System.Text.RegularExpressions.Regex(FOR_Regex, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            if (reg.IsMatch(tmplStr))
            {
                var matchs = reg.Matches(tmplStr);
                foreach (Match natch in matchs)
                {
                    var result = new List<string>();
                    foreach (var item in list)
                    {
                        sb.AppendLine(FillTemplate2(natch.Groups[2].Value, item, paramData));
                        //result.Add(reg.Replace(tmplStr, new MatchEvaluator(mathReple)));
                    }
                    tmplStr = tmplStr.Replace("$FOR$" + natch.Groups[2].Value + "$EFOR$", sb.ToString());
                }
            }
            tmplStr = FillTemplate2(tmplStr, list.FirstOrDefault(), paramData);
            return tmplStr;
        }

        //public static string mathReple(Match match)
        //{
        //    return FillTemplate(match.Groups[2].Value, data);
        //}

        #region private function fill Template

        /// <summary>
        ///  "Sys_STG_EMailUrl"
        /// </summary>
        /// <param name="paramData"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string GetSysVerUrl(IList<dynamic> paramData, string code, string defualtValue = "")
        {
            return GetVerData(paramData, $"Sys_URL_{code}", defualtValue);
        }
        /// <summary>
        ///  "Sys_STG_EMailUrl"
        /// </summary>
        /// <param name="paramData"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string GetSysVerData(IList<dynamic> paramData, string code, string defualtValue = "")
        {
            return GetVerData(paramData, $"Sys_{code}", defualtValue);
        }

        /// <summary>
        ///  "Sys_STG_EMailUrl"
        /// </summary>
        /// <param name="paramData"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string GetVerData(IList<dynamic> paramData, string code, string defualtValue = "")
        {
            var url = defualtValue;
            var emialUrl = paramData.FirstOrDefault(f => f.Code.Equals(code, StringComparison.InvariantCultureIgnoreCase));
            if (emialUrl != null)
                url = emialUrl.DataValue;
            return url;
        }


        public static EmailTemplateDto FillEmailTemplate(EmailTemplateDto action, dynamic data, IList<dynamic> paramData = null)
        {
            if (data != null)
            {
                JObject jdata = JObject.FromObject(data);
                foreach (var item in jdata.Properties())
                {
                    var strVal = jdata.Value<string>(item.Name) ?? "";
                    action = FillEmailItem(action, $@"\$\{{{item.Name}\}}", strVal);
                }
            }
            if (paramData != null)
            {
                foreach (var item in paramData)
                {
                    var strVal = item.DataValue ?? "";
                    action = FillEmailItem(action, $@"\$\{{{item.Name}\}}", strVal);
                }
            }
            //替换默认参数：
            if (paramData != null)
            {
                foreach (var item in paramData)
                {
                    var strVal = item.DataValue ?? "";
                    action = FillEmailItem(action, $@"\$\{{{item.Name}\}}", strVal);
                }
            }

            if (!string.IsNullOrWhiteSpace(action.EmailCc))
            {
                action.EmailCc = Regex.Replace(action.EmailCc, $@"[,；、]", ",");
            }
            if (!string.IsNullOrWhiteSpace(action.EmailTo))
            {
                action.EmailTo = Regex.Replace(action.EmailTo, $@"[,；、]", ",");
            }
            return action;
        }

        public static EmailSendRecordEntity FillEmailTemplateByList(EmailSendRecordEntity action, dynamic data, Dictionary<string,object> paramData = null)
        {
            if (!string.IsNullOrWhiteSpace(action.EmailHtmlBody))
                action.EmailHtmlBody = FillTemplate(action.EmailHtmlBody, data, paramData);
            if (!string.IsNullOrWhiteSpace(action.EmailSubject))
                action.EmailSubject = FillTemplate(action.EmailSubject, data, paramData);
            if (!string.IsNullOrWhiteSpace(action.EmailCc))
            {
                action.EmailCc = FillTemplate(action.EmailCc, data, paramData);
                action.EmailCc = Regex.Replace(action.EmailCc, $@"[,；、]", ",");
            }
            if (!string.IsNullOrWhiteSpace(action.EmailTo))
            {
                action.EmailTo = FillTemplate(action.EmailTo, data, paramData);
                action.EmailTo = Regex.Replace(action.EmailTo, $@"[,；、]", ",");
            }
            return action;
        }


        public static EmailTemplateDto FillEmailItem(EmailTemplateDto action, string key, string value)
        {
            value = value ?? "";
            if (!string.IsNullOrWhiteSpace(action.EmailTemplate))
                action.EmailTemplate = Regex.Replace(action.EmailTemplate, key, value);
            if (!string.IsNullOrWhiteSpace(action.EmailSubject))
                action.EmailSubject = Regex.Replace(action.EmailSubject, key, value);
            if (!string.IsNullOrWhiteSpace(action.EmailCc))
            {
                action.EmailCc = Regex.Replace(action.EmailCc, key, value);
            }
            if (!string.IsNullOrWhiteSpace(action.EmailTo))
            {
                action.EmailTo = Regex.Replace(action.EmailTo, key, value);
            }
            return action;
        }

        /// <summary>
        /// 替换模板字符串中的占位符，支持从动态对象和参数列表中提取数据进行替换。
        /// </summary>
        /// <param name="tmpeStr">待替换的模板字符串，格式中包含类似${Name}的占位符。</param>
        /// <param name="data">动态数据对象，用于替换模板中的占位符。</param>
        /// <param name="paramData">额外的参数列表，用于替换模板中的占位符。</param>
        /// <returns>替换完成后的字符串。</returns>
        public static string FillTemplate2(string tmpeStr, dynamic data, IList<dynamic> paramData = null)
        {
            if (tmpeStr == null) return tmpeStr;
            if (!tmpeStr.Contains('{')) return tmpeStr;
            JObject jdata = null;
            if (data != null)
            {
                if (data is String) jdata = (data as String).To<JObject>();
                else if (data is JArray) return tmpeStr;
                else jdata = JObject.FromObject(data);
                foreach (var item in jdata.Properties())
                {
                    if (!tmpeStr.Contains('{')) break;
                    //Newtonsoft.Json.Linq.JArray to Newtonsoft.Json.Linq.JToken
                    if (item.Value is JArray) continue;
                    var strVal = jdata.Value<string>(item.Name) ?? "";
                    tmpeStr = Regex.Replace(tmpeStr, $@"\$\{{{item.Name}\}}", strVal);
                }
            }
            if (!tmpeStr.Contains('{')) return tmpeStr;
            if (paramData != null)
            {
                foreach (var item in paramData)
                {
                    if (!tmpeStr.Contains('{')) break;
                    var strVal = item.DataValue + "";
                    tmpeStr = Regex.Replace(tmpeStr, $@"\$\{{{item.Name}\}}", strVal);
                }
            }
            if (!tmpeStr.Contains('{')) return tmpeStr;
            var dateTimeCurrent = AddCurenDateParam();
            foreach (var item in dateTimeCurrent)
            {
                if (!tmpeStr.Contains('{')) break;
                string strVal = item.DataValue + "";
                tmpeStr = Regex.Replace(tmpeStr, $@"\$\{{{item.Name}\}}", strVal);
            }

            // 处理三元表达式 ${condition ? trueValue : falseValue}
            // 简单解析，condition为变量名，判断其值是否非空
            var ternaryPattern = new Regex(@"\$\{\s*(\w+)\s*\?\s*(.*?)\s*:\s*(.*?)\s*\}");
            tmpeStr = ternaryPattern.Replace(tmpeStr, match =>
            {
                var conditionVar = match.Groups[1].Value;
                var trueVal = match.Groups[2].Value;
                var falseVal = match.Groups[3].Value;

                string conditionValue = null;

                // 优先从data中取值
                if (jdata != null && jdata.TryGetValue(conditionVar, out JToken token))
                {
                    conditionValue = token.Type == JTokenType.Null ? null : token.ToString();
                }
                else if (paramData != null)
                {
                    var param = paramData.FirstOrDefault(p => p.Name == conditionVar);
                    if (param != null)
                        conditionValue = param.DataValue;
                }

                // 判断条件是否非空
                bool condition = !string.IsNullOrEmpty(conditionValue);

                // 返回对应值，且对返回值中可能的变量继续替换（递归调用FillTemplate）
                var result = condition ? trueVal : falseVal;
                // 递归替换，防止嵌套变量未替换
                return FillTemplate2(result, data, paramData);
            });

            return tmpeStr;
        }



        /// <summary>
        /// 替换模板字符串中的占位符，支持从动态对象和参数列表中提取数据进行替换。
        /// </summary>
        /// <param name="data">动态数据对象，用于替换模板中的占位符。</param>
        /// <param name="paramData">额外的参数列表，用于替换模板中的占位符。</param>
        /// <returns>替换完成后的字符串。</returns>
        public static JObject MargeData(string jsonData, Dictionary<string, object> paramData = null)
        {
            if (jsonData == null) return new JObject();
            var data = jsonData?.To<JObject>() ?? new JObject();
            if (paramData == null) return data;
            foreach (var item in paramData)
            {
                if (data.ContainsKey(item.Key)) data[item.Key] = item.Value + "";
                else data.Add(item.Key, item.Value + "");
            }
            return data;
        }

        /// <summary>
        /// 替换模板字符串中的占位符，支持从动态对象和参数列表中提取数据进行替换。
        /// </summary>
        /// <param name="data">动态数据对象，用于替换模板中的占位符。</param>
        /// <param name="paramData">额外的参数列表，用于替换模板中的占位符。</param>
        /// <returns>替换完成后的字符串。</returns>
        public static JObject MargeData(JObject data, Dictionary<string, object> paramData = null)
        {
            data= data ?? new JObject();
            if (paramData == null) return data;
            foreach (var item in paramData)
            {
                if (data.ContainsKey(item.Key)) data[item.Key] = item.Value + "";
                else data.Add(item.Key, item.Value + "");
            }
            return data;
        }

        /// <summary>
        /// 替换模板字符串中的占位符，支持从动态对象和参数列表中提取数据进行替换。
        /// </summary>
        /// <param name="tmpeStr">待替换的模板字符串，格式中包含类似${Name}的占位符。</param>
        /// <param name="jsonData">动态数据对象，用于替换模板中的占位符。</param>
        /// <param name="paramData">额外的参数列表，用于替换模板中的占位符。</param>
        /// <returns>替换完成后的字符串。</returns>
        public static string FillTemplateByJson(string tmpeStr, string jsonData, Dictionary<string, object> paramData = null)
        {
            if (!tmpeStr.Contains("$")) return tmpeStr;
            return new HtmlTemplateParser().Parse(tmpeStr, MargeData(jsonData, paramData = null));
        }


        /// <summary>
        /// 替换模板字符串中的占位符，支持从动态对象和参数列表中提取数据进行替换。
        /// </summary>
        /// <param name="tmpeStr">待替换的模板字符串，格式中包含类似${Name}的占位符。</param>
        /// <param name="data">动态数据对象，用于替换模板中的占位符。</param>
        /// <param name="paramData">额外的参数列表，用于替换模板中的占位符。</param>
        /// <returns>替换完成后的字符串。</returns>
        public static string FillTemplate(string tmpeStr, JObject data, Dictionary<string, object> paramData = null)
        {
            return new HtmlTemplateParser().Parse(tmpeStr, MargeData(data, paramData));
        }

        private static IList<dynamic> AddCurenDateParam(IList<dynamic> paramData = null)
        {
            if (paramData == null) paramData = new List<dynamic>();
            paramData.Add(new { Name = "CurrentDate", DataValue = DateTimeOffset.Now.ToString("yyyy-MM-dd") });
            paramData.Add(new { Name = "CurrentDateEn", DataValue = DateTimeOffset.Now.ToString() });
            paramData.Add(new { Name = "CurrentDateTime", DataValue = DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss") });
            paramData.Add(new { Name = "CurrentTime", DataValue = DateTimeOffset.Now.ToString("HH:mm:ss") });
            return paramData;
        }


        private static Dictionary<string, string> AddCurenDateParam(Dictionary<string, string> paramData)
        {
            if (paramData == null) paramData = new Dictionary<string, string>();
            paramData.Add("CurrentDate", DateTimeOffset.Now.ToString("yyyy-MM-dd"));
            paramData.Add("CurrentDateEn", DateTimeOffset.Now.ToString());
            paramData.Add("CurrentDateTime", DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            paramData.Add("CurrentTime", DateTimeOffset.Now.ToString("HH:mm:ss"));
            return paramData;
        }


        #endregion private function fill Template


    }
}