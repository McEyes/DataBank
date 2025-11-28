using FluentValidation.Validators;

using Newtonsoft.Json.Linq;

using StackExchange.Redis;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ITPortal.Core
{
    public class ExpressionTool
    {
        // 运算符优先级（新增字符串操作符）
        private static readonly Dictionary<string, int> OperatorPrecedence = new Dictionary<string, int>
        {
            { "?", 0 },   // 新增：三元表达式问号，最低优先级
            { ":", 0 },   // 新增：三元表达式冒号，最低优先级
            { "||", 1 },
            { "&&", 2 },
            { "==", 3 }, { "!=", 3 },
            { ">", 4 }, { "<", 4 }, { ">=", 4 }, { "<=", 4 },
            { "contain", 4 }, { "startWith", 4 }, { "endWith", 4 }, { "equal", 4 }, // 字符串操作符
            { "+", 5 }, { "-", 5 },
            { "*", 6 }, { "/", 6 }, { "%", 6 },
            { "!", 7 }
        };

        // 所有支持的运算符（包含字符串操作符）
        private static readonly HashSet<string> Operators = new HashSet<string>(OperatorPrecedence.Keys);

        // 三元表达式相关符号
        private static readonly HashSet<string> TernaryOperators = new HashSet<string> { "?", ":" };

        /// <summary>
        /// 支持各种复杂嵌套表达式
        /// 字符串用单引号，数值，bool可以直接使用，变量直接使用
        /// 支持大小，包含等判断，支持计算，支持三元符号计算
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="data">变量参数</param>
        /// <returns></returns>
        public static bool EvaluateBoolean(string expression, JObject data)
        {
            try
            {
                // 移除表达式中的空格（保留字符串内的空格）
                string processedExpr = RemoveSpacesExceptInStrings(expression);

                // 将中缀表达式转换为后缀表达式（逆波兰表示法）
                var postfix = InfixToPostfix(processedExpr);

                // 计算后缀表达式结果
                var result = EvaluatePostfix(postfix, data);

                // 转换为布尔值
                return Convert.ToBoolean(result);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 支持各种复杂嵌套表达式
        /// 字符串用单引号，数值，bool可以直接使用，变量直接使用
        /// 支持大小，包含等判断，支持计算，支持三元符号计算
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="data">变量参数</param>
        /// <returns></returns>
        public static object EvaluateValue(string expression, JObject data)
        {
            try
            {
                // 移除表达式中的空格（保留字符串内的空格）
                string processedExpr = RemoveSpacesExceptInStrings(expression);

                // 将中缀表达式转换为后缀表达式（逆波兰表示法）
                var postfix = InfixToPostfix(processedExpr);

                // 计算后缀表达式结果
                var result = EvaluatePostfix(postfix, data);

                // 转换为布尔值
                return result;
            }
            catch
            {
                return false;
            }
        }

        // 移除字符串外的空格
        // 修正：保留字符串外的空格（仅压缩连续空格），避免标识符与运算符合并
        private static string RemoveSpacesExceptInStrings(string expression)
        {
            var result = new StringBuilder();
            bool inString = false;
            char stringDelimiter = '\0';
            bool lastWasSpace = false; // 记录上一个字符是否为空格

            foreach (char c in expression)
            {
                if (c == '"' || c == '\'')
                {
                    // 处理字符串引号，重置空格标记
                    lastWasSpace = false;
                    if (inString && c == stringDelimiter)
                    {
                        inString = false;
                        stringDelimiter = '\0';
                    }
                    else if (!inString)
                    {
                        inString = true;
                        stringDelimiter = c;
                    }
                    result.Append(c);
                }
                else if (inString)
                {
                    // 字符串内的字符原样保留（包括空格）
                    lastWasSpace = false;
                    result.Append(c);
                }
                else if (char.IsWhiteSpace(c))
                {
                    // 字符串外的空格：仅保留一个（压缩连续空格）
                    if (!lastWasSpace)
                    {
                        result.Append(' ');
                        lastWasSpace = true;
                    }
                }
                else
                {
                    // 非空格、非字符串内的字符原样保留
                    lastWasSpace = false;
                    result.Append(c);
                }
            }

            return result.ToString().Trim(); // 移除首尾空格
        }

        // 修改中缀转后缀表达式方法中的三元运算符处理逻辑
        private static List<string> InfixToPostfix(string expression)
        {
            var output = new List<string>();
            var stack = new Stack<string>();
            var tokens = Tokenize(expression);

            foreach (var token in tokens)
            {
                if (token == "(")
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
                // 处理三元表达式的?
                else if (token == "?")
                {
                    // 弹出所有优先级高于?的运算符（因?优先级0，实际会弹出所有其他运算符）
                    while (stack.Count > 0 && stack.Peek() != "(")
                    {
                        output.Add(stack.Pop());
                    }
                    stack.Push(token);
                }
                // 处理三元表达式的:
                else if (token == ":")
                {
                    // 弹出所有优先级高于:的运算符，直到遇到?
                    while (stack.Count > 0 && stack.Peek() != "?")
                    {
                        output.Add(stack.Pop());
                    }
                    stack.Pop(); // 弹出?
                    stack.Push(token);
                }
                else if (Operators.Contains(token))
                {
                    // 处理普通运算符，根据优先级弹出栈中更高或相等优先级的运算符
                    while (stack.Count > 0 && stack.Peek() != "(" && stack.Peek() != "?" &&
                           OperatorPrecedence[stack.Peek()] >= OperatorPrecedence[token])
                    {
                        output.Add(stack.Pop());
                    }
                    stack.Push(token);
                }
                else
                {
                    output.Add(token); // 操作数
                }
            }

            while (stack.Count > 0)
            {
                var op = stack.Pop();
                if (op != "(") // 防止未匹配的括号
                {
                    output.Add(op);
                }
            }

            return output;
        }
        // 中缀表达式转后缀表达式（修复三元表达式和字符串操作符处理）
        private static List<string> InfixToPostfix2(string expression)
        {
            var output = new List<string>();
            var stack = new Stack<string>();
            var tokens = Tokenize(expression);

            foreach (var token in tokens)
            {
                if (token == "(")
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
                // 处理三元表达式的?
                else if (token == "?")
                {
                    while (stack.Count > 0 && stack.Peek() != "(")
                    {
                        output.Add(stack.Pop());
                    }
                    stack.Push(token);
                }
                // 处理三元表达式的:
                else if (token == ":")
                {
                    while (stack.Count > 0 && stack.Peek() != "?")
                    {
                        output.Add(stack.Pop());
                    }
                    stack.Pop(); // 弹出?
                    stack.Push(token);
                }
                else if (Operators.Contains(token) || TernaryOperators.Contains(token))
                {
                    while (stack.Count > 0 && stack.Peek() != "(" && stack.Peek() != "?" &&
                           OperatorPrecedence.ContainsKey(stack.Peek()) &&
                           OperatorPrecedence[stack.Peek()] >= OperatorPrecedence.GetValueOrDefault(token, 0))
                    {
                        output.Add(stack.Pop());
                    }
                    stack.Push(token);
                }
                else
                {
                    output.Add(token); // 操作数
                }
            }

            while (stack.Count > 0)
            {
                var op = stack.Pop();
                if (op != "(") // 防止未匹配的括号
                {
                    output.Add(op);
                }
            }

            return output;
        }

        // 表达式分词（修复多字符运算符识别）
        private static List<string> Tokenize(string expression)
        {
            var tokens = new List<string>();
            int i = 0;

            while (i < expression.Length)
            {
                // 处理字符串
                if (expression[i] == '"' || expression[i] == '\'')
                {
                    char delimiter = expression[i];
                    i++;
                    int start = i;
                    while (i < expression.Length && expression[i] != delimiter)
                    {
                        i++;
                    }
                    tokens.Add(delimiter + expression.Substring(start, i - start) + delimiter);
                    i++;
                }
                // 处理多字符运算符（优先匹配最长的）
                else if (i + 8 < expression.Length && Operators.Contains(expression.Substring(i, 8)))
                {
                    tokens.Add(expression.Substring(i, 8));
                    i += 8;
                }
                else if (i + 7 < expression.Length && Operators.Contains(expression.Substring(i, 7)))
                {
                    tokens.Add(expression.Substring(i, 7));
                    i += 7;
                }
                else if (i + 5 < expression.Length && Operators.Contains(expression.Substring(i, 5)))
                {
                    tokens.Add(expression.Substring(i, 5));
                    i += 5;
                }
                else if (i + 4 < expression.Length && Operators.Contains(expression.Substring(i, 4)))
                {
                    tokens.Add(expression.Substring(i, 4));
                    i += 4;
                }
                else if (i + 2 < expression.Length &&
                         (Operators.Contains(expression.Substring(i, 2)) ||
                          TernaryOperators.Contains(expression.Substring(i, 2))))
                {
                    tokens.Add(expression.Substring(i, 2));
                    i += 2;
                }
                // 处理单字符运算符
                else if (Operators.Contains(expression[i].ToString()))// ||TernaryOperators.Contains(expression[i].ToString())
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
                // 处理变量或数字
                else if (char.IsLetterOrDigit(expression[i]) || expression[i] == '.' || expression[i] == '_')
                {
                    int start = i;
                    while (i < expression.Length &&
                           (char.IsLetterOrDigit(expression[i]) || expression[i] == '.' || expression[i] == '_'))
                    {
                        i++;
                    }
                    tokens.Add(expression.Substring(start, i - start));
                }
                else
                {
                    i++; // 忽略未知字符
                }
            }

            return tokens;
        }

        // 计算后缀表达式（修复日期和类型转换）
        private static object EvaluatePostfix(List<string> postfix, JObject data)
        {
            var stack = new Stack<object>();

            foreach (var token in postfix)
            {
                if (Operators.Contains(token))// || TernaryOperators.Contains(token)
                {
                    // 处理三元表达式的:
                    if (token == ":")
                    {
                        object falseValue = stack.Pop();
                        object trueValue = stack.Pop();
                        object conditionResult = stack.Pop();
                        stack.Push(Convert.ToBoolean(conditionResult) ? trueValue : falseValue);
                    }
                    // 处理单目运算符
                    else if (token == "!")
                    {
                        object operand = stack.Pop();
                        stack.Push(!Convert.ToBoolean(operand));
                    }
                    // 处理双目运算符
                    else
                    {
                        object b = stack.Pop();
                        object a = stack.Pop();
                        stack.Push(ApplyOperator(a, b, token));
                    }
                }
                else
                {
                    // 处理操作数（优先解析日期）
                    stack.Push(ResolveValue(token, data));
                }
            }

            return stack.Count > 0 ? stack.Pop() : null;
        }

        // 解析值（优先处理日期）
        private static object ResolveValue(string token, JObject data)
        {
            // 字符串
            if (token.StartsWith("\"") && token.EndsWith("\""))
            {
                string str = token.Substring(1, token.Length - 2);
                // 尝试解析为日期
                if (DateTime.TryParse(str, out DateTime date))
                {
                    return date;
                }
                return str;
            }
            if (token.StartsWith("'") && token.EndsWith("'"))
            {
                string str = token.Substring(1, token.Length - 2);
                if (DateTime.TryParse(str, out DateTime date))
                {
                    return date;
                }
                return str;
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

            // 变量（优先解析为日期）
            var jToken = data.SelectToken(token);
            if (jToken != null)
            {
                if (jToken.Type == JTokenType.String &&
                    DateTime.TryParse(jToken.ToString(), out DateTime date))
                {
                    return date;
                }
                return jToken.ToObject<object>();
            }

            return token; // 未识别的值返回原始字符串
        }

        // 应用运算符（完善类型处理）
        private static object ApplyOperator(object a, object b, string op)
        {
            // 处理字符串操作符
            if (op == "contain" || op == "startWith" || op == "endWith" || op == "equal")
            {
                string aStr = a?.ToString() ?? "";
                string bStr = b?.ToString() ?? "";

                return op switch
                {
                    "contain" => aStr.Contains(bStr),
                    "startWith" => aStr.StartsWith(bStr),
                    "endWith" => aStr.EndsWith(bStr),
                    "equal" => string.Equals(aStr, bStr),
                    _ => false
                };
            }

            // 处理字符串拼接
            if (op == "+" && (a is string || b is string))
            {
                return $"{a}{b}";
            }

            // 日期比较
            if (a is DateTime aDate && b is DateTime bDate)
            {
                int cmp = DateTime.Compare(aDate, bDate);
                return CompareResult(cmp, op);
            }

            // 转换为数值进行算术运算和比较
            if (TryConvertToDouble(a, out double aNum) && TryConvertToDouble(b, out double bNum))
            {
                if (op == "+" || op == "-" || op == "*" || op == "/" || op == "%")
                {
                    return op switch
                    {
                        "+" => aNum + bNum,
                        "-" => aNum - bNum,
                        "*" => aNum * bNum,
                        "/" => bNum == 0 ? 0 : aNum / bNum,
                        "%" => aNum % bNum,
                        _ => 0
                    };
                }
                else
                {
                    return CompareResult(aNum.CompareTo(bNum), op);
                }
            }

            // 布尔运算
            if (a is bool aBool && b is bool bBool)
            {
                return op switch
                {
                    "&&" => aBool && bBool,
                    "||" => aBool || bBool,
                    "==" => aBool == bBool,
                    "!=" => aBool != bBool,
                    _ => false
                };
            }

            // 字符串比较（按字典序）
            if (a is string aStri && b is string bStri)
            {
                int cmp = string.Compare(aStri, bStri, StringComparison.Ordinal);
                return CompareResult(cmp, op);
            }

            return false;
        }

        // 比较结果处理
        private static bool CompareResult(int comparisonResult, string op)
        {
            return op switch
            {
                "==" => comparisonResult == 0,
                "!=" => comparisonResult != 0,
                ">" => comparisonResult > 0,
                "<" => comparisonResult < 0,
                ">=" => comparisonResult >= 0,
                "<=" => comparisonResult <= 0,
                _ => false
            };
        }

        // 尝试将对象转换为double
        private static bool TryConvertToDouble(object value, out double result)
        {
            if (value == null)
            {
                result = 0;
                return false;
            }

            if (value is double d)
            {
                result = d;
                return true;
            }

            if (value is int i)
            {
                result = i;
                return true;
            }

            if (value is long l)
            {
                result = l;
                return true;
            }

            if (value is float f)
            {
                result = f;
                return true;
            }

            if (value is decimal dec)
            {
                result = (double)dec;
                return true;
            }

            return double.TryParse(value.ToString(), out result);
        }
    }
}
