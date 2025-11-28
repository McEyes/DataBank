using Furion;

using ITPortal.Extension.System;
using ITPortal.Flow.Application;

using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

using Xunit.Abstractions;

namespace ITPortal.Flow.xUnitTest
{
    public class UnitTest1
    {
        private readonly ITestOutputHelper Output;
        /// <summary>
        /// 
        /// </summary>
        public UnitTest1(ITestOutputHelper tempOutput)
        {
            Output = tempOutput;
        }

        [Fact]
        public void Test1()
        {

        }

        // 模拟 AddCurenDateParam 方法，返回固定测试数据
        private static IList<dynamic> AddCurenDateParam()
        {
            return new List<dynamic>
        {
            new { Name = "CurrentYear", DataValue = "2024" },
            new { Name = "CurrentMonth", DataValue = "06" }
        };
        }

        //// 由于 FillTemplate 中调用 AddCurenDateParam 是静态方法，且未注入，需用局部替代或修改方法
        //// 这里通过局部重写 FillTemplate 以注入 AddCurenDateParam 模拟实现
        //private static string FillTemplate(string tmpeStr, dynamic data, IList<dynamic> paramData = null)
        //{
        //    if (tmpeStr == null) return tmpeStr;
        //    if (!tmpeStr.Contains('{')) return tmpeStr;
        //    JObject jdata = null;
        //    if (data != null)
        //    {
        //        if (data is string) jdata = JObject.Parse(data);
        //        else if (data is JArray) return tmpeStr;
        //        else jdata = JObject.FromObject(data);
        //        foreach (var item in jdata.Properties())
        //        {
        //            if (!tmpeStr.Contains('{')) break;
        //            if (item.Value is JArray) continue;
        //            var strVal = jdata.Value<string>(item.Name) ?? "";
        //            tmpeStr = Regex.Replace(tmpeStr, $@"\$\{{{item.Name}\}}", strVal);
        //        }
        //    }
        //    if (!tmpeStr.Contains('{')) return tmpeStr;
        //    if (paramData != null)
        //    {
        //        foreach (var item in paramData)
        //        {
        //            if (!tmpeStr.Contains('{')) break;
        //            var strVal = item.DataValue + "";
        //            tmpeStr = Regex.Replace(tmpeStr, $@"\$\{{{item.Name}\}}", strVal);
        //        }
        //    }
        //    if (!tmpeStr.Contains('{')) return tmpeStr;
        //    var dateTimeCurrent = AddCurenDateParam();
        //    foreach (var item in dateTimeCurrent)
        //    {
        //        if (!tmpeStr.Contains('{')) break;
        //        string strVal = item.DataValue + "";
        //        tmpeStr = Regex.Replace(tmpeStr, $@"\$\{{{item.Name}\}}", strVal);
        //    }

        //    var ternaryPattern = new Regex(@"\$\{\s*(\w+)\s*\?\s*(.*?)\s*:\s*(.*?)\s*\}");
        //    tmpeStr = ternaryPattern.Replace(tmpeStr, match =>
        //    {
        //        var conditionVar = match.Groups[1].Value;
        //        var trueVal = match.Groups[2].Value;
        //        var falseVal = match.Groups[3].Value;

        //        string conditionValue = null;

        //        if (jdata != null && jdata.TryGetValue(conditionVar, out JToken token))
        //        {
        //            conditionValue = token.Type == JTokenType.Null ? null : token.ToString();
        //        }
        //        else if (paramData != null)
        //        {
        //            var param = paramData.FirstOrDefault(p => p.Name == conditionVar);
        //            if (param != null)
        //                conditionValue = param.DataValue;
        //        }

        //        bool condition = !string.IsNullOrEmpty(conditionValue);

        //        var result = condition ? trueVal : falseVal;
        //        return FillTemplate(result, data, paramData);
        //    });

        //    return tmpeStr;
        //}


        /// <summary>
        /// 替换模板字符串中的占位符，支持从动态对象和参数列表中提取数据进行替换。
        /// </summary>
        /// <param name="tmpeStr">待替换的模板字符串，格式中包含类似${Name}的占位符。</param>
        /// <param name="data">动态数据对象，用于替换模板中的占位符。</param>
        /// <param name="paramData">额外的参数列表，用于替换模板中的占位符。</param>
        /// <returns>替换完成后的字符串。</returns>
        public static string FillTemplate(string tmpeStr, dynamic data, IList<dynamic> paramData = null)
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
                return FillTemplate(result, data, paramData);
            });

            return tmpeStr;
        }

        [Fact]
        public void FillTemplate_NullInput_ReturnsNull()
        {
            string input = null;
            var result = FillTemplate(input, null);
            Assert.Null(result);
        }

        [Fact]
        public void FillTemplate_NoBraces_ReturnsOriginal()
        {
            string input = "Hello World";
            var result = FillTemplate(input, new { Name = "Test" });
            Assert.Equal(input, result);
        }

        [Fact]
        public void FillTemplate_SimpleReplacement_WithObject()
        {
            string template = "Hello ${Name}, your age is ${Age}.";
            var data = new { Name = "Alice", Age = 30 };
            var result = FillTemplate(template, data);
            Assert.Equal("Hello Alice, your age is 30.", result);
        }

        [Fact]
        public void FillTemplate_SimpleReplacement_WithJsonString()
        {
            string template = "Hello ${Name}, your city is ${City}.";
            string jsonData = @"{""Name"":""Bob"",""City"":""New York""}";
            var result = FillTemplate(template, jsonData);
            Assert.Equal("Hello Bob, your city is New York.", result);
        }

        [Fact]
        public void FillTemplate_SkipJArrayData_ReturnsOriginal()
        {
            string template = "Array test ${Name}";
            JArray arr = new JArray { 1, 2, 3 };
            var result = FillTemplate(template, arr);
            Assert.Equal(template, result);
        }

        [Fact]
        public void FillTemplate_SkipJArrayProperty()
        {
            string template = "User: ${Name}, Tags: ${Tags}";
            var data = new
            {
                Name = "Charlie",
                Tags = new JArray("tag1", "tag2")
            };
            var result = FillTemplate(template, data);
            // Tags is JArray, should not be replaced, Name replaced
            Assert.Equal("User: Charlie, Tags: ${Tags}", result);
        }

        [Fact]
        public void FillTemplate_ReplaceWithParamData()
        {
            string template = "Hello ${User}, your score is ${Score}.";
            var paramData = new List<dynamic>
        {
            new { Name = "User", DataValue = "David" },
            new { Name = "Score", DataValue = "95" }
        };
            var result = FillTemplate(template, null, paramData);
            Assert.Equal("Hello David, your score is 95.", result);
        }

        [Fact]
        public void FillTemplate_ReplaceDateTimeParams()
        {
            string template = "Year: ${CurrentYear}, Month: ${CurrentMonth}";
            var result = FillTemplate(template, null);
            Assert.Equal("Year: 2024, Month: 06", result);
        }

        [Fact]
        public void FillTemplate_TernaryExpression_TrueCondition_FromData()
        {
            string template = "Status: ${IsActive ? Active : Inactive}";
            var data = new { IsActive = "yes" };
            var result = FillTemplate(template, data);
            Assert.Equal("Status: Active", result);
        }

        [Fact]
        public void FillTemplate_TernaryExpression_FalseCondition_FromData()
        {
            string template = "Status: ${IsActive ? Active : Inactive}";
            var data = new { IsActive = "" };
            var result = FillTemplate(template, data);
            Assert.Equal("Status: Inactive", result);
        }

        [Fact]
        public void FillTemplate_TernaryExpression_TrueCondition_FromParamData()
        {
            string template = "Status: ${IsActive ? Active : Inactive}";
            var paramData = new List<dynamic>
        {
            new { Name = "IsActive", DataValue = "true" }
        };
            var result = FillTemplate(template, null, paramData);
            Assert.Equal("Status: Active", result);
        }

        [Fact]
        public void FillTemplate_TernaryExpression_FalseCondition_FromParamData()
        {
            string template = "Status: ${IsActive ? Active : Inactive}";
            var paramData = new List<dynamic>
        {
            new { Name = "IsActive", DataValue = "" }
        };
            var result = FillTemplate(template, null, paramData);
            Assert.Equal("Status: Inactive", result);
        }

        [Fact]
        public void FillTemplate_TernaryExpression_NestedVariableReplacement()
        {
            string template = "Result: ${Condition ? '${Value} passed' : 'failed'}";
            var data = new { Condition = "yes", Value = "Test" };
            var result = FillTemplate(template, data);
            Assert.Equal("Result: 'Test passed'", result);
        }

        [Fact]
        public void FillTemplate_MultipleReplacements_MixedSources()
        {
            string template = "Name: ${Name}, Score: ${Score}, Year: ${CurrentYear}, Passed: ${Passed ? Yes : No}";
            var data = new { Name = "Eva", Passed = "true" };
            var paramData = new List<dynamic>
        {
            new { Name = "Score", DataValue = "88" }
        };
            var result = FillTemplate(template, data, paramData);
            Assert.Equal("Name: Eva, Score: 88, Year: 2024, Passed: Yes", result);
        }

        [Fact]
        public void FillTemplate_VariableNotFound_RemainsUnchanged()
        {
            string template = "Hello ${UnknownVar}";
            var data = new { Name = "Frank" };
            var result = FillTemplate(template, data);
            Assert.Equal("Hello ${UnknownVar}", result);
        }


        [Fact]
        public void FillTemplate_VariableNotFound_RemainsUnchanged2()
        {
            string template = @"<style type=""text/css"">
	.approval_false { display: none !important; }
	.approval_true { display: inline !important; }
</style>

您有流程<span style=""color: blue;""><b>${TaskSubject}</b></span>
<span class=""${isPublicSecurityLevel?'approval_false':'approval_true'}"">需要审批</span>
<span class=""${isPublicSecurityLevel?'approval_true':'approval_false'}"">代办已自动审批完成，不需要审批，特此通知</span>
<br>
<br>申请人:<b>${ApplicantName}</b>
<br>来源系统:<b>黄埔数据资产目录</b>
<br>数据主题:<b>${tableComment}</b>
<br>内容:<b>${ApplicantName}</b>需要访问<span style=""color: blue;""><b>${tableComment}</b></span>主题数据,
<span class=""${isPublicSecurityLevel?'approval_true':'approval_false'}"">已自动审批完成，不需要审批，特此通知。
</span>
<span class=""${isPublicSecurityLevel?'approval_false':'approval_true'}"">需要您的审批。
<br><br>点击<a  target=""_blank"" href=""${BasicUrl}/#/home/workflow?flowTempName=${FlowTempName}&amp;id=${FlowId}&amp;type=email"" >这里</a>进入Web端审批。
</span>";
            var data = new { ApplicantName = "Frank", isPublicSecurityLevel=true };
            var result = FillTemplate(template, data);
            Output.WriteLine(result);

        }
        [Fact]
        public void FillTemplate_VariableNotFound_RemainsUnchanged3()
        {
            string template = @"<style type=""text/css"">
	.approval_false { display: none !important; }
	.approval_true { display: inline !important; }
</style>

您有流程<span style=""color: blue;""><b>${TaskSubject}</b></span>
<span class=""${isPublicSecurityLevel?'approval_false':'approval_true'}"">需要审批</span>
<span class=""${isPublicSecurityLevel?'approval_true':'approval_false'}"">代办已自动审批完成，不需要审批，特此通知</span>
<br>
<br>申请人:<b>${ApplicantName}</b>
<br>来源系统:<b>黄埔数据资产目录</b>
<br>数据主题:<b>${tableComment}</b>
<br>内容:<b>${ApplicantName}</b>需要访问<span style=""color: blue;""><b>${tableComment}</b></span>主题数据,
<span class=""${isPublicSecurityLevel?'approval_true':'approval_false'}"">已自动审批完成，不需要审批，特此通知。
</span>
<span class=""${isPublicSecurityLevel?'approval_false':'approval_true'}"">需要您的审批。
<br><br>点击<a  target=""_blank"" href=""${BasicUrl}/#/home/workflow?flowTempName=${FlowTempName}&amp;id=${FlowId}&amp;type=email"" >这里</a>进入Web端审批。
</span>";
            var data = new { ApplicantName = "Frank", isPublicSecurityLevel = false };
            var result = FillTemplate(template, data);
            Output.WriteLine(result);

        }
    }
}