using Newtonsoft.Json.Linq;

using StackExchange.Redis;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ITPortal.Core.Html
{

    public class HtmlTemplateParser
    {
        // 正则表达式匹配${}表达式
        private static readonly Regex ExpressionRegex = new Regex(@"\$\{(.+?)\}", RegexOptions.Compiled);

        // 改进的正则表达式，使用平衡组匹配支持嵌套的$FOR/$EFOR
        private static readonly Regex ForLoopRegex = new Regex(
            @"\$FOR\{(.+?)\}\$
            (
                (?>
                    \$FOR\{.*?\}\$ (?<Level>) |
                    \$EFOR\$ (?<-Level>) |
                    (?!\$FOR\{.*?\}\$|\$EFOR\$).
                )*
                (?(Level)(?!))
            )
            \$EFOR\$",
            RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);

        // 匹配算术运算符和字符串连接符
        private static readonly string[] Operators = { "+", "-", "*", "/", "%", "+" };
        private static readonly Dictionary<string, int> OperatorPrecedence = new Dictionary<string, int>
        {
            { "+", 1 }, { "-", 1 },
            { "*", 2 }, { "/", 2 }, { "%", 2 }
        };

        public string Parse(string template, JObject data)
        {
            string processedTemplate = ProcessForLoops(template, data);
            processedTemplate = ProcessExpressions(processedTemplate, data);
            return processedTemplate;
        }

        private string ProcessForLoops(string template, JObject data)
        {
            var matches = ForLoopRegex.Matches(template);
            for (int i = matches.Count - 1; i >= 0; i--)
            {
                Match match = matches[i];
                string loopVariable = match.Groups[1].Value.Trim();
                string loopContent = match.Groups[2].Value;

                var loopData = GetValueFromData(data, loopVariable);

                if (loopData is JArray array)
                {
                    string replacedContent = ProcessLoopContent(loopContent, array);
                    template = template.Replace(match.Value, replacedContent);
                }
                else
                {
                    template = template.Replace(match.Value, "");
                }
            }
            return template;
        }

        private string ProcessLoopContent(string loopContent, JArray array)
        {
            var result = new System.Text.StringBuilder();
            var i = 0;
            foreach (var item in array)
            {
                string itemContent = loopContent;
                JObject itemData;

                if (item is JObject itemObject)
                {
                    itemData = itemObject;
                }
                else
                {
                    itemData = new JObject();
                    itemData["index"] = i;
                    itemData["item"] = item;
                }

                itemContent = ProcessForLoops(itemContent, itemData);
                itemContent = ProcessExpressions(itemContent, itemData);

                result.Append(itemContent);
                i++;
            }
            return result.ToString();
        }

        private string ProcessExpressions(string template, JObject data)
        {
            return ExpressionRegex.Replace(template, match =>
            {
                string expression = match.Groups[1].Value.Trim();
                return EvaluateExpression(expression, data);
            });
        }

        private string EvaluateExpression(string expression, JObject data)
        {
            // 处理${.}情况
            //if (expression == ".")
            //{
            //    var value = GetValueFromData(data, ".");
            //    return value != null ? ProcessPrimitiveValue(value) : "";
            //}

            // ??表达式处理
            if (expression.Contains("??"))
            {
                return EvaluateTernaryExpression2(expression, data);
            }

            // 处理三元表达式
            if (expression.Contains("?"))
            {
                return EvaluateTernaryExpression(expression, data);
            }

            // 检查是否包含运算符（支持算术运算和字符串拼接）
            if (Operators.Any(op => expression.Contains(op) && !IsInsideQuotes(expression, op)))
            {
                return EvaluateArithmeticOrStringExpression(expression, data);
            }

            // 处理字符串字面量
            if ((expression.StartsWith("'") && expression.EndsWith("'")) ||
                (expression.StartsWith("\"") && expression.EndsWith("\"")))
            {
                return expression.Substring(1, expression.Length - 2);
            }

            // 处理原始值
            if (IsPrimitiveValue(expression))
            {
                return expression;
            }

            // 处理变量
            var val = GetValueFromData(data, expression);
            return val != null ? ProcessPrimitiveValue(val) : "";
        }

        // 新增：评估算术和字符串表达式
        private string EvaluateArithmeticOrStringExpression(string expression, JObject data)
        {
            try
            {
                // 分词处理
                var tokens = TokenizeExpression(expression);
                // 转换为后缀表达式
                var postfix = ShuntingYardAlgorithm(tokens);
                // 计算结果
                var result = EvaluatePostfix(postfix, data);
                return result?.ToString() ?? "";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"表达式计算错误: {expression}, 错误: {ex.Message}");
                return "";
            }
        }

        // 新增：表达式分词
        private List<string> TokenizeExpression(string expression)
        {
            var tokens = new List<string>();
            int i = 0;
            while (i < expression.Length)
            {
                if (char.IsWhiteSpace(expression[i]))
                {
                    i++;
                    continue;
                }

                // 处理运算符
                if (Operators.Contains(expression[i].ToString()))
                {
                    tokens.Add(expression[i].ToString());
                    i++;
                }
                // 处理括号
                else if (expression[i] == '(' || expression[i] == ')')
                {
                    tokens.Add(expression[i].ToString());
                    i++;
                }
                // 处理字符串
                else if (expression[i] == '"' || expression[i] == '\'')
                {
                    char quote = expression[i];
                    i++;
                    int start = i;
                    while (i < expression.Length && expression[i] != quote)
                    {
                        i++;
                    }
                    tokens.Add(quote + expression.Substring(start, i - start) + quote);
                    i++;
                }
                // 处理数字和变量
                else
                {
                    int start = i;
                    while (i < expression.Length &&
                           !char.IsWhiteSpace(expression[i]) &&
                           !Operators.Contains(expression[i].ToString()) &&
                           expression[i] != '(' && expression[i] != ')')
                    {
                        i++;
                    }
                    tokens.Add(expression.Substring(start, i - start));
                }
            }
            return tokens;
        }

        // 新增：调度场算法转换为后缀表达式
        private List<string> ShuntingYardAlgorithm(List<string> tokens)
        {
            var output = new List<string>();
            var stack = new Stack<string>();

            foreach (var token in tokens)
            {
                if (IsOperator(token))
                {
                    while (stack.Count > 0 && stack.Peek() != "(" &&
                           OperatorPrecedence[stack.Peek()] >= OperatorPrecedence[token])
                    {
                        output.Add(stack.Pop());
                    }
                    stack.Push(token);
                }
                else if (token == "(")
                {
                    stack.Push(token);
                }
                else if (token == ")")
                {
                    while (stack.Count > 0 && stack.Peek() != "(")
                    {
                        output.Add(stack.Pop());
                    }
                    stack.Pop(); // 弹出左括号
                }
                else
                {
                    output.Add(token);
                }
            }

            while (stack.Count > 0)
            {
                output.Add(stack.Pop());
            }

            return output;
        }

        // 新增：计算后缀表达式
        private object EvaluatePostfix(List<string> postfix, JObject data)
        {
            var stack = new Stack<object>();

            foreach (var token in postfix)
            {
                if (IsOperator(token))
                {
                    if (stack.Count < 2)
                        throw new InvalidOperationException("表达式格式错误");

                    var b = stack.Pop();
                    var a = stack.Pop();

                    stack.Push(ApplyOperator(a, b, token, data));
                }
                else
                {
                    stack.Push(ResolveValue(token, data));
                }
            }

            return stack.Count == 1 ? stack.Pop() : null;
        }

        // 新增：应用运算符
        private object ApplyOperator(object a, object b, string op, JObject data)
        {
            // 处理字符串拼接
            if (op == "+" && (a is string || b is string))
            {
                return $"{a}{b}";
            }

            // 处理算术运算
            if (a is IConvertible && b is IConvertible)
            {
                double aNum = Convert.ToDouble(a);
                double bNum = Convert.ToDouble(b);

                switch (op)
                {
                    case "+": return aNum + bNum;
                    case "-": return aNum - bNum;
                    case "*": return aNum * bNum;
                    case "/": return bNum == 0 ? 0 : aNum / bNum;
                    case "%": return aNum % bNum;
                    default: throw new NotSupportedException($"不支持的运算符: {op}");
                }
            }

            throw new InvalidOperationException($"无法对 {a} 和 {b} 应用运算符 {op}");
        }

        // 新增：解析值
        private object ResolveValue(string token, JObject data)
        {
            // 字符串字面量
            if ((token.StartsWith("'") && token.EndsWith("'")) ||
                (token.StartsWith("\"") && token.EndsWith("\"")))
            {
                return token.Substring(1, token.Length - 2);
            }

            // 数字
            if (double.TryParse(token, out double num))
            {
                return num;
            }

            // 布尔值
            if (bool.TryParse(token, out bool b))
            {
                return b;
            }

            // 变量
            var jToken = GetValueFromData(data, token);
            return jToken != null ? ConvertJTokenToObject(jToken) : null;
        }

        // 新增：转换JToken为对象
        private object ConvertJTokenToObject(JToken token)
        {
            switch (token.Type)
            {
                case JTokenType.String: return token.Value<string>();
                case JTokenType.Integer: return token.Value<long>();
                case JTokenType.Float: return token.Value<double>();
                case JTokenType.Boolean: return token.Value<bool>();
                default: return token.ToString();
            }
        }

        // 新增：检查是否为运算符
        private bool IsOperator(string token)
        {
            return Operators.Contains(token);
        }

        // 新增：检查运算符是否在引号内
        private bool IsInsideQuotes(string expression, string op)
        {
            bool inQuotes = false;
            char quoteChar = '\0';

            for (int i = 0; i < expression.Length; i++)
            {
                if (expression[i] == '"' || expression[i] == '\'')
                {
                    if (!inQuotes)
                    {
                        inQuotes = true;
                        quoteChar = expression[i];
                    }
                    else if (expression[i] == quoteChar)
                    {
                        inQuotes = false;
                    }
                }
                else if (inQuotes && i <= expression.Length - op.Length &&
                         expression.Substring(i, op.Length) == op)
                {
                    return true;
                }
            }
            return false;
        }


        private string EvaluateTernaryExpression2(string expression, JObject data)
        {
            int questionMarkIndex = expression.IndexOf('?');


            string conditionPart = expression.Substring(0, questionMarkIndex).Trim();
            string falsePart = expression.Substring(questionMarkIndex + 2).Trim();

            var conditionResult = GetValueFromData(data, conditionPart);

            return conditionResult == null ? EvaluateExpression(falsePart, data) : conditionResult + "";
        }

        private string EvaluateTernaryExpression(string expression, JObject data)
        {
            int questionMarkIndex = expression.IndexOf('?');
            int colonIndex = expression.IndexOf(':', questionMarkIndex + 1);

            if (questionMarkIndex == -1 || colonIndex == -1)
            {
                return "";
            }

            string conditionPart = expression.Substring(0, questionMarkIndex).Trim();
            string truePart = expression.Substring(questionMarkIndex + 1, colonIndex - questionMarkIndex - 1).Trim();
            string falsePart = expression.Substring(colonIndex + 1).Trim();

            bool conditionResult = EvaluateCondition(conditionPart, data);

            return conditionResult
                ? EvaluateExpression(truePart, data)
                : EvaluateExpression(falsePart, data);
        }

        private bool EvaluateCondition(string condition, JObject data)
        {
            string[] operators = { "==", "!=", ">", "<", ">=", "<=" };
            foreach (var op in operators)
            {
                int opIndex = condition.IndexOf(op);
                if (opIndex > 0)
                {
                    string leftPart = condition.Substring(0, opIndex).Trim();
                    string rightPart = condition.Substring(opIndex + op.Length).Trim();

                    var leftValue = ParseValue(leftPart, data);
                    var rightValue = ParseValue(rightPart, data);

                    return CompareValues(leftValue, rightValue, op);
                }
            }

            var value = GetValueFromData(data, condition) + "";
            if (bool.TryParse(value, out bool boolValue))
            {
                return boolValue;
            }
            else
                return !string.IsNullOrWhiteSpace(value);
            //return Convert.ToBoolean(value);
        }

        private object ParseValue(string valueStr, JObject data)
        {
            if (valueStr.StartsWith("'") && valueStr.EndsWith("'"))
            {
                return valueStr.Substring(1, valueStr.Length - 2);
            }

            if (int.TryParse(valueStr, out int intValue))
            {
                return intValue;
            }

            if (double.TryParse(valueStr, out double doubleValue))
            {
                return doubleValue;
            }

            if (bool.TryParse(valueStr, out bool boolValue))
            {
                return boolValue;
            }

            return GetValueFromData(data, valueStr);
        }
        private bool CompareValues(object left, object right, string op)
        {
            if (left == null || right == null)
            {
                return op == "==" ? (left == right) : (left != right);
            }

            // 转换JToken为实际值
            if (left is JValue leftVal) left = leftVal.Value;
            if (right is JValue rightVal) right = rightVal.Value;

            // 统一数值类型为double进行比较
            if (IsNumeric(left) && IsNumeric(right))
            {
                double leftNum = Convert.ToDouble(left);
                double rightNum = Convert.ToDouble(right);

                switch (op)
                {
                    case "==": return leftNum == rightNum;
                    case "!=": return leftNum != rightNum;
                    case ">": return leftNum > rightNum;
                    case "<": return leftNum < rightNum;
                    case ">=": return leftNum >= rightNum;
                    case "<=": return leftNum <= rightNum;
                    default: return false;
                }
            }

            // 字符串比较
            if (left is string leftStr && right is string rightStr)
            {
                int comparison = string.Compare(leftStr, rightStr, StringComparison.Ordinal);
                switch (op)
                {
                    case "==": return comparison == 0;
                    case "!=": return comparison != 0;
                    case ">": return comparison > 0;
                    case "<": return comparison < 0;
                    case ">=": return comparison >= 0;
                    case "<=": return comparison <= 0;
                    default: return false;
                }
            }

            // 布尔值比较
            if (left is bool leftBool && right is bool rightBool)
            {
                switch (op)
                {
                    case "==": return leftBool == rightBool;
                    case "!=": return leftBool != rightBool;
                    default: return false; // 布尔值不支持>、<等比较
                }
            }

            // 其他类型尝试使用IComparable
            if (left is IComparable leftComp && right is IComparable rightComp)
            {
                try
                {
                    // 尝试将值转换为相同类型
                    Type commonType = GetCommonType(left.GetType(), right.GetType());
                    if (commonType != null)
                    {
                        left = Convert.ChangeType(left, commonType);
                        right = Convert.ChangeType(right, commonType);
                        int comparison = leftComp.CompareTo(right);
                        switch (op)
                        {
                            case "==": return comparison == 0;
                            case "!=": return comparison != 0;
                            case ">": return comparison > 0;
                            case "<": return comparison < 0;
                            case ">=": return comparison >= 0;
                            case "<=": return comparison <= 0;
                            default: return false;
                        }
                    }
                }
                catch
                {
                    // 转换失败时返回false
                    return false;
                }
            }

            return false;
        }

        // 辅助方法：判断是否为数值类型
        private bool IsNumeric(object value)
        {
            return value is sbyte || value is byte || value is short || value is ushort ||
                   value is int || value is uint || value is long || value is ulong ||
                   value is float || value is double || value is decimal;
        }

        // 辅助方法：获取两个类型的公共类型
        private Type GetCommonType(Type type1, Type type2)
        {
            // 如果类型相同，直接返回
            if (type1 == type2) return type1;

            // 处理数值类型
            if (IsNumericType(type1) && IsNumericType(type2))
            {
                // 取范围更大的类型
                if (IsFloatingPointType(type1) || IsFloatingPointType(type2))
                    return typeof(double);

                if (type1 == typeof(ulong) || type2 == typeof(ulong))
                    return typeof(ulong);

                if (type1 == typeof(long) || type2 == typeof(long))
                    return typeof(long);

                return typeof(int);
            }

            return null;
        }

        // 辅助方法：判断是否为数值类型
        private bool IsNumericType(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return true;
                default:
                    return false;
            }
        }

        // 辅助方法：判断是否为浮点类型
        private bool IsFloatingPointType(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return true;
                default:
                    return false;
            }
        }
        //private bool CompareValues(object left, object right, string op)
        //{
        //    if (left == null || right == null)
        //    {
        //        return op == "==" ? (left == right) : (left != right);
        //    }

        //    if (left is JValue leftVal) left = leftVal.Value;
        //    if (right is JValue rightVal) right = rightVal.Value;

        //    if (left is IComparable leftComp && right is IComparable rightComp)
        //    {
        //        try
        //        {
        //            int comparison = leftComp.CompareTo(rightComp);
        //            switch (op)
        //            {
        //                case "==": return comparison == 0;
        //                case "!=": return comparison != 0;
        //                case ">": return comparison > 0;
        //                case "<": return comparison < 0;
        //                case ">=": return comparison >= 0;
        //                case "<=": return comparison <= 0;
        //                default: return false;
        //            }
        //        }
        //        catch(Exception ex)
        //        {
        //            return false;
        //        }
        //    }

        //    return false;
        //}

        private JToken GetValueFromData(JObject data, string path)
        {
            try
            {
                if (path.EndsWith(".Count"))
                {
                    var list = data.SelectToken(path.Replace(".Count", ""));
                    if (list is JArray)
                    {
                        return ((JArray)list).Count;
                    }
                }
                return data.SelectToken(path);
            }
            catch
            {
                return null;
            }
        }

        private bool IsPrimitiveValue(string value)
        {
            if (int.TryParse(value, out _) || double.TryParse(value, out _))
            {
                return true;
            }

            if (value.Equals("true", StringComparison.OrdinalIgnoreCase) ||
                value.Equals("false", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }

        private string ProcessPrimitiveValue(JToken value)
        {
            if (value == null) return "";

            if (value.Type == JTokenType.String)
            {
                return value.ToString();
            }
            if (value.Type == JTokenType.Boolean)
            {
                return value.ToObject<bool>().ToString().ToLower();
            }
            if (value.Type == JTokenType.Integer || value.Type == JTokenType.Float)
            {
                return value.ToString();
            }

            return value.ToString();
        }
    }
}
