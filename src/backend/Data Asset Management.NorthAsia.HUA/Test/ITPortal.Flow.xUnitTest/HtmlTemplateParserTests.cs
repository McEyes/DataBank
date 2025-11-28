using Furion;

using Google.Type;

using ITPortal.Core.Html;
using ITPortal.Extension.System;
using ITPortal.Flow.Application;

using Newtonsoft.Json.Linq;

using System.Linq.Expressions;
using System.Text.RegularExpressions;

using Xunit.Abstractions;

namespace ITPortal.Flow.xUnitTest
{
    public class HtmlTemplateParserTests
    {
        private readonly ITestOutputHelper Output;
        /// <summary>
        /// 
        /// </summary>
        public HtmlTemplateParserTests(ITestOutputHelper tempOutput)
        {
            Output = tempOutput;
            // 从提供的JSON字符串初始化测试数据
            string jsonData = @"{
""BasicUrl"":""test"",
                ""id"": ""677da2af-d75e-45c7-99e2-46920b152b67"",
                ""flowId"": ""677da2af-d75e-45c7-99e2-46920b152b67"",
                ""userId"": ""3317330"",
                ""userName"": ""Ada Gou(3317330)"",
                ""smeId"": ""2306994"",
                ""smeName"": ""Susan Zheng6994(2306994)"",
                ""smeDept"": ""Finance"",
                ""reason"": ""Test"",
                ""tableName"": ""Profit Center Master Data:profit_center_master_data"",
                ""tableComment"": ""Profit Center Master Data，来自SAP，每月1号凌晨3点更新"",
                ""columnList"": [
                    {
                        ""id"": ""1826539976897310723"",
                        ""colName"": ""entered_on"",
                        ""colComment"": ""创建日期"",
                        ""dataType"": ""date"",
                        ""colKey"": ""0""
                    },
                    {
                        ""id"": ""1826539976905699332"",
                        ""colName"": ""hierarchy_area"",
                        ""colComment"": ""层级组别"",
                        ""dataType"": ""varchar"",
                        ""dataLength"": ""20""
                    },
                    {
                        ""id"": ""1826539976888922113"",
                        ""colName"": ""profit_center"",
                        ""colComment"": ""利润中心编号"",
                        ""dataType"": ""varchar"",
                        ""colKey"": ""1"",
                        ""nullable"": ""0""
                    }
                ]
            }";

            _testData = JObject.Parse(jsonData);
        }
        private readonly HtmlTemplateParser _parser = new HtmlTemplateParser();
        private readonly JObject _testData;

        [Fact]
        public void Parse_ProfitCenterData2()
        {
            // 测试模板 - 包含多种解析场景
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

            // 执行解析
            string result = _parser.Parse(template, _testData);

            Output.WriteLine(result);
        }

        [Fact]
        public void Parse_ProfitCenterData_length()
        {
            // 测试模板 - 包含多种解析场景
            string template = @"
                    <h3>包含字段 (共 ${columnList.Count} 个):</h3>
                    <p>处理状态: ${status ?? '未处理'}</p>
                    <p>显示顺序: 第 ${(columnList.Count > 0 ? 1 : 0)} 组数据</p>
                </div>";

            // 执行解析
            string result = _parser.Parse(template, _testData);

            Output.WriteLine(result);
            // 验证列表循环
            Assert.Contains("<h3>包含字段 (共 3 个):</h3>", result);
            Assert.Contains("<p>处理状态: 未处理</p>", result); // 验证null值处理
            Assert.Contains("<p>显示顺序: 第 1 组数据</p>", result); // 验证算术表达式
        }

        [Fact]
        public void Parse_ProfitCenterData_ShouldRenderCorrectly()
        {
            // 测试模板 - 包含多种解析场景
            string template = @"
                <div class='request-info'>
                    <h2>${tableName}</h2>
                    <p>申请人: ${userName} (ID: ${userId})</p>
                    <p>审批人: ${smeName} (部门: ${smeDept})</p>
                    <p>数据来源: ${tableComment}</p>
                    <p>申请原因: ${reason}</p>
                    <p>请求ID: ${id}</p>
                    
                    <h3>包含字段 (共 ${columnList.Count} 个):</h3>
                    <table>
                        <tr>
                            <th>字段名</th>
                            <th>描述</th>
                            <th>类型</th>
                            <th>是否主键</th>
                        </tr>
                        $FOR{columnList}$
                        <tr>
                            <td>${colName}</td>
                            <td>${colComment}</td>
                            <td>${dataType}${dataLength ? '(' + dataLength + ')' : ''}</td>
                            <td>${colKey == '1' ? '是' : '否'}</td>
                        </tr>
                        $EFOR$
                    </table>
                    
                    <p>处理状态: ${status ?? '未处理'}</p>
                    <p>显示顺序: 第 ${(columnList.Count > 0 ? 1 : 0)} 组数据</p>
                </div>";

            // 执行解析
            string result = _parser.Parse(template, _testData);

            Output.WriteLine(result);
            // 验证基本信息
            Assert.Contains("<h2>Profit Center Master Data:profit_center_master_data</h2>", result);
            Assert.Contains("<p>申请人: Ada Gou(3317330) (ID: 3317330)</p>", result);
            Assert.Contains("<p>审批人: Susan Zheng6994(2306994) (部门: Finance)</p>", result);
            Assert.Contains("<p>数据来源: Profit Center Master Data，来自SAP，每月1号凌晨3点更新</p>", result);
            Assert.Contains("<p>请求ID: 677da2af-d75e-45c7-99e2-46920b152b67</p>", result);

            // 验证列表循环
            Assert.Contains("<h3>包含字段 (共 3 个):</h3>", result);
            Assert.Contains("<td>entered_on</td>", result);
            Assert.Contains("<td>创建日期</td>", result);
            Assert.Contains("<td>date</td>", result);

            // 验证条件表达式
            Assert.Contains("<td>是</td>", result); // profit_center是主键
            Assert.Contains("<td>否</td>", result); // 其他字段不是主键
            Assert.Contains("<td>varchar(20)</td>", result); // 验证数据长度显示
            Assert.Contains("<p>处理状态: 未处理</p>", result); // 验证null值处理
            Assert.Contains("<p>显示顺序: 第 1 组数据</p>", result); // 验证算术表达式
        }

        // 基础变量替换测试
        [Fact]
        public void Parse_WithSimpleVariables_ReturnsReplacedValues()
        {
            // 准备数据
            var data = JObject.Parse(@"{
                ""name"": ""张三"",
                ""age"": 30,
                ""isStudent"": false,
                ""score"": 85.5
            }");

            // 模板
            string template = @"
                <div>${name}</div>
                <div>${age}</div>
                <div>${isStudent}</div>
                <div>${score}</div>
            ";

            // 执行
            string result = _parser.Parse(template, data);

            // 验证
            Assert.Contains("<div>张三</div>", result);
            Assert.Contains("<div>30</div>", result);
            Assert.Contains("<div>false</div>", result);
            Assert.Contains("<div>85.5</div>", result);
        }

        // 多级属性访问测试
        [Fact]
        public void Parse_WithNestedProperties_ReturnsReplacedValues()
        {
            // 准备数据
            var data = JObject.Parse(@"{
                ""user"": {
                    ""name"": ""李四"",
                    ""address"": {
                        ""city"": ""北京"",
                        ""street"": ""长安街""
                    }
                }
            }");

            // 模板
            string template = @"
                <div>${user.name}</div>
                <div>${user.address.city}</div>
                <div>${user.address.street}</div>
            ";

            // 执行
            string result = _parser.Parse(template, data);

            // 验证
            Assert.Contains("<div>李四</div>", result);
            Assert.Contains("<div>北京</div>", result);
            Assert.Contains("<div>长安街</div>", result);
        }

        // 字符串数组循环测试
        [Fact]
        public void Parse_WithStringArrayLoop_ReturnsRepeatedContent()
        {
            // 准备数据
            var data = JObject.Parse(@"{
                ""tags"": [""C#"", ""ASP.NET"", ""HTML""]
            }");

            // 模板
            string template = @"
                <ul>
                    $FOR{tags}$
                    <li>${item}</li>
                    $EFOR$
                </ul>
            ";

            // 执行
            string result = _parser.Parse(template, data);
            Output.WriteLine(result);

            // 验证
            Assert.Contains("<li>C#</li>", result);
            Assert.Contains("<li>ASP.NET</li>", result);
            Assert.Contains("<li>HTML</li>", result);
            Assert.DoesNotContain("${.}", result);
        }

        // 对象数组循环测试
        [Fact]
        public void Parse_WithObjectArrayLoop_ReturnsRepeatedContent()
        {
            // 准备数据
            var data = JObject.Parse(@"{
                ""products"": [
                    {""id"": 1, ""name"": ""笔记本""},
                    {""id"": 2, ""name"": ""手机""}
                ]
            }");

            // 模板
            string template = @"
                <div>
                    $FOR{products}$
                    <p>${id}: ${name}</p>
                    $EFOR$
                </div>
            ";

            // 执行
            string result = _parser.Parse(template, data);

            // 验证
            Assert.Contains("<p>1: 笔记本</p>", result);
            Assert.Contains("<p>2: 手机</p>", result);
        }

        // 嵌套循环测试
        [Fact]
        public void Parse_WithNestedLoops_ReturnsCorrectlyNestedContent()
        {
            // 准备数据
            var data = JObject.Parse(@"{
                ""categories"": [
                    {
                        ""name"": ""电子"",
                        ""items"": [""手机"", ""电脑""]
                    },
                    {
                        ""name"": ""服装"",
                        ""items"": [""衬衫"", ""裤子""]
                    }
                ]
            }");

            // 模板
            string template = @"
                $FOR{categories}$
                <h3>${name}</h3>
                <ul>
                    $FOR{items}$
                    <li>${item}</li>
                    $EFOR$
                </ul>
                $EFOR$
            ";

            // 执行
            string result = _parser.Parse(template, data);
            Output.WriteLine(result);

            // 验证
            Assert.Contains("<h3>电子</h3>", result);
            Assert.Contains("<li>手机</li>", result);
            Assert.Contains("<li>电脑</li>", result);
            Assert.Contains("<h3>服装</h3>", result);
            Assert.Contains("<li>衬衫</li>", result);
            Assert.Contains("<li>裤子</li>", result);
        }

        // 算术表达式测试
        [Fact]
        public void Parse_WithArithmeticExpressions_ReturnsCalculatedValues()
        {
            // 准备数据
            var data = JObject.Parse(@"{
                ""a"": 10,
                ""b"": 5,
                ""c"": 3
            }");

            // 模板
            string template = @"
                <div>${a + b}</div>
                <div>${a - b}</div>
                <div>${a * b}</div>
                <div>${a / b}</div>
                <div>${a % c}</div>
                <div>${a + b * c}</div>
                <div>${(a + b) * c}</div>
            ";

            // 执行
            string result = _parser.Parse(template, data);

            // 验证
            Assert.Contains("<div>15</div>", result);
            Assert.Contains("<div>5</div>", result);
            Assert.Contains("<div>50</div>", result);
            Assert.Contains("<div>2</div>", result);
            Assert.Contains("<div>1</div>", result);
            Assert.Contains("<div>25</div>", result);
            Assert.Contains("<div>45</div>", result);
        }

        // 字符串拼接测试
        [Fact]
        public void Parse_WithStringConcatenation_ReturnsCombinedStrings()
        {
            // 准备数据
            var data = JObject.Parse(@"{
                ""prefix"": ""Hello"",
                ""name"": ""World"",
                ""number"": 123
            }");

            // 模板
            string template = @"
                <div>${prefix + ' ' + name}</div>
                <div>${'Number: ' + number}</div>
                <div>${name + ' has ' + number + ' messages'}</div>
            ";

            // 执行
            string result = _parser.Parse(template, data);

            // 验证
            Assert.Contains("<div>Hello World</div>", result);
            Assert.Contains("<div>Number: 123</div>", result);
            Assert.Contains("<div>World has 123 messages</div>", result);
        }

        // 三元表达式测试
        [Fact]
        public void Parse_WithTernaryExpressions_ReturnsCorrectBranch()
        {
            // 准备数据
            var data = JObject.Parse(@"{
                ""score"": 85,
                ""isMember"": true,
                ""count"": 0
            }");

            // 模板
            string template = @"
                <div>${score >= 60 ? '及格' : '不及格'}</div>
                <div>${isMember ? '会员' : '游客'}</div>
                <div>${count > 0 ? '有数据' : '无数据'}</div>
            ";

            // 执行
            string result = _parser.Parse(template, data);

            // 验证
            Assert.Contains("<div>及格</div>", result);
            Assert.Contains("<div>会员</div>", result);
            Assert.Contains("<div>无数据</div>", result);
        }

        // 混合表达式测试
        [Fact]
        public void Parse_WithMixedExpressions_ReturnsCorrectResults()
        {
            // 准备数据
            var data = JObject.Parse(@"{
                ""index"": 2,
                ""total"": 5,
                ""name"": ""测试""
            }");

            // 模板
            string template3 = @"
                <div>${index > 0 ? '上一页' : '首页'}</div>
            ";

            string template = @"
                <div>${'第' + (index + 1) + '页'}</div>
                <div>${index > 0 ? '上一页' : '首页'}</div>
                <div>${'共' + total + '页，当前' + (index + 1) + '页'}</div>
                <div>${name + '的得分: ' + (index * 10 + 50)}</div>
            ";

            // 执行
            string result = _parser.Parse(template, data);
            Output.WriteLine(result);

            // 验证
            Assert.Contains("<div>第3页</div>", result);
            Assert.Contains("<div>上一页</div>", result);
            Assert.Contains("<div>共5页，当前3页</div>", result);
            Assert.Contains("<div>测试的得分: 70</div>", result);
        }

        // 循环中的索引和表达式测试
        [Fact]
        public void Parse_WithLoopIndexExpressions_ReturnsCorrectResults()
        {
            // 准备数据
            var data = JObject.Parse(@"{
                ""items"": [""A"", ""B"", ""C""]
            }");

            // 模板
            string template = @"
                <ul>
                    $FOR{items}$
                    <li>${index + 1}. ${item}</li>
                    $EFOR$
                </ul>
            ";

            // 执行
            string result = _parser.Parse(template, data);

            Output.WriteLine(result);
            // 验证
            Assert.Contains("<li>1. A</li>", result);
            Assert.Contains("<li>2. B</li>", result);
            Assert.Contains("<li>3. C</li>", result);
        }

        // 空数据处理测试
        [Fact]
        public void Parse_WithNullValues_ReturnsEmptyStrings()
        {
            // 准备数据
            var data = JObject.Parse(@"{
                ""nullValue"": null,
                ""undefinedValue"": {},
                ""emptyArray"": []
            }");

            // 模板
            string template = @"
                <div>${nullValue}</div>
                <div>${undefinedValue.property}</div>
                $FOR{emptyArray}$
                <div>这行不应该显示</div>
                $EFOR$
            ";

            // 执行
            string result = _parser.Parse(template, data);
            Output.WriteLine(result);

            // 验证
            Assert.Contains("<div></div>", result); // nullValue替换为空
            Assert.Contains("<div></div>", result); // 未定义属性替换为空
            Assert.DoesNotContain("这行不应该显示", result); // 空数组不循环
        }


        // 条件真假判断
        #region 日期比较测试
        [Fact]
        public void EvaluateBoolean_DateComparisons_ReturnsCorrectResult()
        {
            var data = JObject.Parse(@"{
                ""date1"": ""2023-10-01"",
                ""date2"": ""2023-09-30"",
                ""date3"": ""2024-01-01"",
                ""date4"": ""2024-12-31"",
                ""date5"": ""2023-05-15"",
                ""date6"": ""2023/05/15""
            }");

            // 测试用例：表达式 → 预期结果
            var testCases = new Dictionary<string, bool>
            {
                { "date1 > date2", true },
                { "date3 < date4", true },
                { "date5 == date6", true },
                { "date1 >= date2", true },
                { "date1 != date3", true },
                { "\"2023-12-31\" <= \"2024-01-01\"", true },
                { "date2 > date1", false }
            };

            foreach (var (expr, expected) in testCases)
            {
                bool result = _parser.EvaluateBoolean(expr, data);
                Output.WriteLine($"【 {expr} 】result: {result}, real:{expected}");
                Assert.Equal(expected, result);
            }
        }
        #endregion

        #region 字符串判断测试
        [Fact]
        public void EvaluateBoolean_StringOperations_ReturnsCorrectResult()
        {
            var data = JObject.Parse(@"{
                ""str1"": ""hello world"",
                ""str2"": ""test123"",
                ""str3"": ""file.txt"",
                ""str4"": ""abc"",
                ""str5"": ""def"",
                ""str6"": ""Hello""
            }");

            // 测试用例：表达式 → 预期结果
            var testCases = new Dictionary<string, bool>
            {
                { "str1 contain \"world\"", true },
                { "str1 contain \"World\"", false },
                { "str2 startWith \"test\"", true },
                { "str2 startWith \"Test\"", false },
                { "str3 endWith \".txt\"", true },
                { "str3 endWith \".csv\"", false },
                { "str4 == str4", true },
                { "str4 != str5", true },
                { "str6 equal \"hello\"", false },
                { "\"Hello World\" contain str6", true }
            };

            foreach (var (expr, expected) in testCases)
            {
                bool result = _parser.EvaluateBoolean(expr, data);
                Output.WriteLine($"【 {expr} 】result: {result}, real:{expected}");
                Assert.Equal(expected, result);
            }
        }
        #endregion

        #region 数值与布尔值测试
        [Fact]
        public void EvaluateBoolean_NumberAndBoolean_ReturnsCorrectResult()
        {
            var data = JObject.Parse(@"{
                ""num1"": 10,
                ""num2"": 5,
                ""num3"": 10.5,
                ""flag1"": true,
                ""flag2"": false
            }");

            // 测试用例：表达式 → 预期结果
            var testCases = new Dictionary<string, bool>
            {
                { "num1 > num2", true },
                { "num3 <= num1", false },
                { "num1 == 10", true },
                { "num2 != 5", false },
                { "flag1 == true", true },
                { "flag2 != false", false },
                { "num1 + num2 > 14", true } // 支持简单算术表达式
            };

            foreach (var (expr, expected) in testCases)
            {
                bool result = _parser.EvaluateBoolean(expr, data);
                Output.WriteLine($"{expr} result: {result}, real:{expected}");
                Assert.Equal(expected, result);
            }
        }
        #endregion

        #region 混合逻辑测试
        [Fact]
        public void EvaluateBoolean_ComplexExpressions_ReturnsCorrectResult()
        {
            var data = JObject.Parse(@"{
                ""createdAt"": ""2023-01-15"",
                ""expiredAt"": ""2024-01-15"",
                ""fileName"": ""report_2023-01.pdf"",
                ""downloadCount"": 15
            }");

            // 测试用例：表达式 → 预期结果
            var testCases = new Dictionary<string, bool>
            {
                { "createdAt < expiredAt && fileName contain \"2023\"", true },
                { "fileName endWith \".pdf\" || downloadCount > 100", true },
                { "createdAt > expiredAt ? true : false", false },
                { "\"2023-01-01\" <= createdAt && createdAt <= \"2023-12-31\"", true }
            };

            foreach (var (expr, expected) in testCases)
            {
                bool result = _parser.EvaluateBoolean(expr, data);
                Output.WriteLine($"【 {expr} 】result: {result}, real:{expected}");
                Assert.Equal(expected, result);
            }
        }
        #endregion




        [Fact]
        public void EvaluateBoolean_ComplexExpressions_ReturnsCorrectResult2()
        {
            var data = JObject.Parse(@"{
                ""a"": 10,
                ""b"": 5,
                ""c"": 3,
                ""d"": 20,
                ""strPrefix"": ""user_"",
                ""strSuffix"": ""_active"",
                ""username"": ""john_doe"",
                ""status"": ""active"",
                ""createdAt"": ""2023-06-15"",
                ""expiresAt"": ""2024-06-15"",
                ""isVip"": true,
                ""loginCount"": 15
            }");

            // 测试用例：表达式 → 预期结果
            var testCases = new Dictionary<string, bool>
            {
                #region 1. 逻辑运算符(&&、||)与比较组合
                { "(a > b && c < d) || d == 0", true },
                //{ "a + b > 10 && c * d < 50", false },//不支持
                { "isVip || loginCount > 100", true },
                { @"createdAt > ""2023-01-01"" && expiresAt < ""2023-12-31""", false },
                //{ "!(a == b) && (c != d)", true },//不支持
                #endregion

                #region 2. 三元表达式与运算结合
                { "(a > 5 ? b : c) == 5", true },
                //{ "(isVip ? 100 : 50) > 60", true },//不支持
                { @"(loginCount > 10 ? ""active"" : ""inactive"") == status", true },
                { "(a + b > 20 ? d - c : c) <= 10", true },
                //{ "(2 * a + b > 20 ? d - c : c) <= 10", false }, //不支持
                #endregion

                #region 3. 算术运算与比较组合
                //{ "a + b * c > 30", false },//不支持
                { "(a + b) * c <= 50", true },
                //{ "d / a == 2 && b % c == 2", true },//不支持
                { "a * 2 - b / 5 >= 19", true },
                { "loginCount + 5 < 20", true },
                #endregion

                #region 4. 字符串拼接与判断组合
                //{ @"(strPrefix + username) == ""user_john_doe""", true },//不支持
                //{ @"(username + strSuffix) contain ""doe_active""", true },//不支持
                //{ @"(""user_"" + username) startWith ""user_john""", true },//不支持
                //{ @"(strPrefix + ""admin"") endWith ""_admin""", true },//不支持
                //{ @"(strPrefix + username + strSuffix) equal ""user_john_doe_active""", true },//不支持
                #endregion

                #region 5. 混合超级复杂表达式
                //不支持
                //{
                //    @"(a > b && (c + d) < 30) || " +
                //    @"(isVip && (strPrefix + username) contain ""john"") && " +
                //    "loginCount > 10",
                //    true
                //},
                //{
                //    @"(createdAt > ""2023-06-01"" ? (a * 2) : (b * 3)) > 15 && " +
                //    @"!(expiresAt < ""2024-01-01"") && " +
                //    @"(""status_"" + status) endWith ""_active""",
                //    true
                //},
                //{
                //    @"(a + b > 15 ? true : false) ? " +
                //    @"(strPrefix + ""admin"") == ""user_admin"" : " +
                //    "(loginCount * 2 < 30)",
                //    false
                //}
                #endregion
            };

            foreach (var (expression, expected) in testCases)
            {
                bool result = _parser.EvaluateBoolean(expression, data);
                Output.WriteLine($"【 {expression} 】result: {result}, real:{expected}");
                Assert.Equal(expected, result);
            }
        }

        [Fact]
        public void ExpressionEvaluate_ComplexExpressions_ReturnsCorrectResult3()
        {
            var data = JObject.Parse(@"{
                ""a"": 10,
                ""b"": 5,
                ""c"": 3,
                ""d"": 20,
                ""strPrefix"": ""user_"",
                ""strSuffix"": ""_active"",
                ""username"": ""john_doe"",
                ""status"": ""active"",
                ""createdAt"": ""2023-06-15"",
                ""expiresAt"": ""2024-06-15"",
                ""isVip"": true,
                ""loginCount"": 15
            }");

            // 测试用例：表达式 → 预期结果
            var testCases = new Dictionary<string, bool>
            { 
                #region 1. 逻辑运算符(&&、||)与比较组合
                { "(a > b && c < d) || d == 0", true },
                { "a + b > 10 && c * d < 50", false },
                { "isVip || loginCount > 100", true },
                { @"createdAt > ""2023-01-01"" && expiresAt < ""2023-12-31""", false },
                { "!(a == b) && (c != d)", true },
                #endregion

                #region 2. 三元表达式与运算结合
                { "(a > 5 ? b : c) == 5", true },
                { "(isVip ? 100 : 50) > 60", true },
                { @"(loginCount > 10 ? ""active"" : ""inactive"") == status", true },
                { "(a + b > 20 ? d - c : c) <= 11", true },
                { "(2 * a + b > 20 ? d - c : c) <= 11", false },
                #endregion

                #region 3. 算术运算与比较组合
                { "a + b * c > 30", false },
                { "(a + b) * c <= 50", true },
                { "d / a == 2 && b % c == 2", true },
                { "a * 2 - b / 5 >= 19", true },
                { "loginCount + 5 < 20", false },
                #endregion

                #region 4. 字符串拼接与判断组合
                { @"(strPrefix + username) == ""user_john_doe""", true },
                { @"(username + strSuffix) contain ""doe_active""", true },
                { @"(""user_"" + username) startWith ""user_john""", true },
                { @"(strPrefix + ""admin"") endWith ""_admin""", true },
                { @"(strPrefix + username + strSuffix) equal ""user_john_doe_active""", true },
                #endregion

                #region 5. 混合超级复杂表达式
                {
                    @"(a > b && (c + d) < 30) || " +
                    @"(isVip && (strPrefix + username) contain ""john"") && " +
                    "loginCount > 10",
                    true
                },
                {
                    @"(createdAt > ""2023-06-01"" ? (a * 2) : (b * 3)) > 15 && " +
                    @"!(expiresAt < ""2024-01-01"") && " +
                    @"(""status_"" + status) endWith ""_active""",
                    true
                },
                {
                    @"(a + b > 15 ? true : false) ? " +
                    @"(strPrefix + ""admin"") == ""user_admin"" : " +
                    "(loginCount * 2 < 30)",
                    false
                }
                #endregion
            };
            //var _parser2 = new ExpressionEvaluate();
            //foreach (var (expression, expected) in testCases)
            //{
            //    bool result = _parser2.EvaluateBoolean(expression, data);
            //    Output.WriteLine($"【 {expression} 】result: {result}, real:{expected}");
            //    Assert.Equal(expected, result);
            //}
        }


        /// <summary>
        /// 多层属性值获取
        /// </summary>
        [Fact]
        public void ExpressionEvaluate_ComplexExpressions_ReturnsCorrectResult4()
        {
            var data = JObject.Parse(@"{
                ""a"": 10,
                ""b"": 5,
                ""c"": 3,
                ""d"": 20,
                ""strPrefix"": ""user_"",
                ""strSuffix"": ""_active"",
                ""username"": ""john_doe"",
                ""status"": ""active"",
                ""createdAt"": ""2023-06-15"",
                ""expiresAt"": ""2024-06-15"",
                ""isVip"": true,
                ""loginCount"": 15,
                ""user"":{
                    ""name"":""user name liyanghui"",
                    ""age"":21,
                }
            }");

            // 测试用例：表达式 → 预期结果
            var testCases = new Dictionary<string, bool>
            { 
                #region 1. 嵌套取值
                //{ "a+b+2c = user.age", false},//不支持数值和字母混合写
                { @"user.name contain ""liyanghui"" ", true},
                { "d > user.age", false},
                { "loginCount > user.age", false},
                { "a+b+c = user.age", true},
                #endregion
                
            };
            //var _parser2 = new ExpressionEvaluate();
            //foreach (var (expression, expected) in testCases)
            //{
            //    bool result = _parser2.EvaluateBoolean(expression, data);
            //    Output.WriteLine($"【 {expression} 】result: {result}, real:{expected}");
            //    Assert.Equal(expected, result);
            //}
            //var val = _parser2.EvaluateValue("a+b+c", data);
            //Output.WriteLine($"【 a+b+c 】result: {18}, real:{val}");
            //Assert.Equal(21, val); 
            //_parser2.EvaluateValue("a+b+c", data);
            //Output.WriteLine($"【 a+b+c 】result: {18}, real:{val}");
            //Assert.Equal(18, val);
        }
    }
}