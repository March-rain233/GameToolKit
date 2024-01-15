using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace GameToolKit
{
    /// <summary>
    /// 后缀表达式转换器
    /// </summary>
    public class PostfixConverter
    {
        /// <summary>
        /// 操作符匹配委托
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="starIndex">检测起始索引</param>
        /// <param name="word">操作符</param>
        /// <param name="priority">操作符优先级</param>
        /// <returns>是否匹配成功</returns>
        public delegate bool OperationMatchHandler(string expression, int starIndex, out string word, out int priority, out int parameterNum, out ExpressionWord.FuncCreatorHandler funcCreator);
        /// <summary>
        /// 参数匹配委托
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="starIndex">检测起始索引</param>
        /// <param name="word">参数名</param>
        /// <returns>是否匹配成功</returns>
        public delegate bool ParameterMatchHandler(string expression, int starIndex, out string word);

        /// <summary>
        /// 操作符匹配函数
        /// </summary>
        public OperationMatchHandler OperationMatcher;
        /// <summary>
        /// 参数匹配函数
        /// </summary>
        public ParameterMatchHandler ParameterMatcher;

        public PostfixConverter(OperationMatchHandler operationMatcher, ParameterMatchHandler parameterMatcher)
        {
            OperationMatcher = operationMatcher;
            ParameterMatcher = parameterMatcher;
        }
        public IEnumerable<ExpressionWord> Convert(string expression)
        {

            var result = new List<ExpressionWord>();
            var opStack = new Stack<(string, int, int, ExpressionWord.FuncCreatorHandler)>();
            int i = 0;
            int len = expression.Length;
            while(i < len)
            {
                string value;
                if(ParameterMatcher(expression, i, out value))
                {
                    result.Add(new ExpressionWord() { Context = value, IsOperation = false, ParameterNum = 0});
                }
                else if(expression[i] == '(')
                {
                    value = "(";
                    opStack.Push(("(",0, default, default));
                }
                else if(expression[i] == ')')
                {
                    value=")";
                    while(opStack.TryPop(out var top) && top.Item1 != "(")
                    {
                        result.Add(new ExpressionWord() { Context = top.Item1, IsOperation = true, ParameterNum = top.Item3, FuncCreator = top.Item4 });
                    }
                }
                else if(OperationMatcher(expression, i, out value, out var priority, out var parameterNum, out var func))
                {
                    while(opStack.TryPeek(out var top) && top.Item2 <= priority)
                    {
                        result.Add(new ExpressionWord() { Context = top.Item1, IsOperation = true, ParameterNum = top.Item3, FuncCreator = top.Item4 });
                        opStack.Pop();
                    }
                    opStack.Push((value, priority, parameterNum, func));
                }
                else
                {
                    throw new ArgumentException("invalid expression");
                }
                i += value.Length;
            }
            while(opStack.TryPop(out var top)) result.Add(new ExpressionWord() { Context = top.Item1, IsOperation = true, ParameterNum = top.Item3 });
            return result;
        }
    }
}
