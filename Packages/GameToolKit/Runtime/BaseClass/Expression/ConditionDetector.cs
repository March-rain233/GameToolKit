using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GameToolKit
{
    /// <summary>
    /// 条件检测器
    /// </summary>
    /// <remarks>
    /// 用于根据条件文本自动生成条件监测对象
    /// 监测对象仅为数据集上的全局变量
    /// </remarks>
    public class ConditionDetector : IDisposable
    {
        static readonly Regex _nameRegex = new Regex(@"\%[0-9|a-z]+");
        static readonly PostfixConverter _converter;

        /// <summary>
        /// 用于监听的委托列表
        /// </summary>
        /// <remarks>
        /// 暂存需要在析构时取消监听的委托<br></br>
        /// 类型:（变量uid，委托）
        /// </remarks>
        List<(BlackboardVariable, BlackboardVariable.VariableChangedHandler)> _listeners;
        ExpressionTree _expressionTree;
        IBlackboard _blackboard;
        private bool _disposedValue;

        /// <summary>
        /// 条件达成事件
        /// </summary>
        public event Action Complete;
        /// <summary>
        /// 当需要进行更新时发生
        /// </summary>
        public event Action<ConditionDetector> NeedRefresh;

        static ConditionDetector()
        {
            var opAttribute = new Dictionary<string, (int, int, ExpressionWord.FuncCreatorHandler)>() //Value:(priority, parameterNum, functionCreator)
            {
                {"(",  (1,  default, default)},
                {")",  (1,  default, default)},
                {"!",  (2,  1,       NotCreator)},
                {"*",  (3,  2,       MulCreator)},
                {"/",  (3,  2,       DivCreator)},
                {"%",  (3,  2,       ModCreator)},
                {"+",  (4,  2,       AddCreator)},
                {"-",  (4,  2,       SubCreator)},
                {">",  (6,  2,       GCreator)},
                {"<",  (6,  2,       LCreator)},
                {"==", (6,  2,       ECreator)},
                {">=", (6,  2,       GECreator)},
                {"<=", (6,  2,       LECreator)},
                {"!=", (6,  2,       NECreator)},
                {"&&", (11, 2,       AndCreator)},
                {"||", (12, 2,       OrCreator)},
            };
            var opRegex = new Regex(@"((<|>)=?)|((=|!)=)|(&&)|(\|\|)|(\(|\))|\+|\-|\*|\%|\/|!");
            var paRegex = new Regex(@"(""|\$)?[a-z|0-9]+""?|("""")");
            PostfixConverter.OperationMatchHandler opMatcher = (string expression, int starIndex, out string word, out int priority, out int parameterNum, out ExpressionWord.FuncCreatorHandler funcCreator) =>
            {
                if (opRegex.IsMatch(expression, starIndex))
                {
                    word = opRegex.Match(expression, starIndex).Value;
                    priority = opAttribute[word].Item1;
                    parameterNum = opAttribute[word].Item2;
                    funcCreator = opAttribute[word].Item3;
                    return true;
                }
                word = default;
                priority = default;
                parameterNum = default;
                funcCreator = default;
                return false;
            };
            PostfixConverter.ParameterMatchHandler paMatcher = (string expression, int starIndex, out string word) =>
            {
                if (paRegex.IsMatch(expression, starIndex))
                {
                    word = paRegex.Match(expression, starIndex).Value;
                    return true;
                }
                word = default;
                return false;
            };
            _converter = new PostfixConverter(opMatcher, paMatcher);
        }

        /// <summary>
        /// 生成条件检测器
        /// </summary>
        /// <param name="conditionContext">条件文本</param>
        /// <param name="blackboard">黑板</param>
        /// <exception cref="Exception"></exception>
        public ConditionDetector(string conditionContext, IBlackboard blackboard)
        {
            _listeners = new List<(BlackboardVariable, BlackboardVariable.VariableChangedHandler)>();
            _blackboard = blackboard;

            // 生成委托
            // eg：%level >= 10 &&% coin < 2 || $123546222 > 0
            // And优先级大于Or
            // $表示uid %表示名称

            //把表达式内名称重定位为uid
            string exp = _nameRegex.Replace(conditionContext, m => "$" + _blackboard.GUIDManager.Name2ID(m.Value[1..]));
            //构造后缀表达式
            var words = _converter.Convert(exp);
            //获取数据
            var valueStrings = from word in words where !word.IsOperation select word.Context;
            var values = new Dictionary<string, (Type, object)>();
            var blackboardIndexs = new HashSet<string>();
            foreach (var word in valueStrings)
            {
                if (word[0] == '$')
                {
                    var value = _blackboard[word[1..]];
                    blackboardIndexs.Add(word[1..]);
                    values[word] = (value.TypeOfValue, value.Value);
                }
                else
                {
                    if (word[0] == '"' && word[^1] == '"') values[word] = (typeof(string), word[1..^1]);
                    else if (int.TryParse(word, out var ivalue)) values[word] = (typeof(int), ivalue);
                    else if (float.TryParse(word, out var fvalue)) values[word] = (typeof(float), fvalue);
                    else if (double.TryParse(word, out var dvalue)) values[word] = (typeof(double), dvalue);
                    else if (bool.TryParse(word, out var bvalue)) values[word] = (typeof(bool), bvalue);
                    else throw new Exception();
                }
            };
            _expressionTree = new ExpressionTree(words, values);
            foreach (var index in blackboardIndexs)
            {
                var variable = _blackboard[index];
                BlackboardVariable.VariableChangedHandler handler = () =>
                {
                    _expressionTree.SetValue(index, variable.Value);
                    NeedRefresh?.Invoke(this);
                };
                variable.ValueChanged += handler;
                _listeners.Add((variable, handler));
            }
        }

        /// <summary>
        /// 刷新检测值
        /// </summary>
        public void Refresh()
        {
            _expressionTree.Refresh();
            if ((bool)_expressionTree.FinalValue) Complete?.Invoke();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                    foreach(var pair in _listeners) pair.Item1.ValueChanged -= pair.Item2;
                    _listeners = null;
                    _expressionTree = null;
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                _disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~ConditionDetector()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #region 生成函数
        static readonly Dictionary<Type, int> _typePriority = new Dictionary<Type, int>()
        {
            {typeof(int), 0},
            {typeof(float), 1},
            {typeof(double), 2},
        };
        /// <summary>
        /// 取参数中最高的数据类型作为函数数据类型
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        static Type GetHighestType(ExpressionTreeNode node)
        {
            int max = 0;
            foreach (var child in node.Children)
                max = Math.Max(max, _typePriority[child.Type]);
            return _typePriority.First(item=>item.Value == max).Key;
        }
        static void AddCreator(ExpressionTreeNode node, out Type outType, out Action<ExpressionTreeNode> func)
        {
            outType = GetHighestType(node);
            if (outType == typeof(int)) func = AddInt;
            else if (outType == typeof(float)) func = AddFloat;
            else if (outType == typeof(double)) func = AddDouble;
            else func = null;
        }
        static void SubCreator(ExpressionTreeNode node, out Type outType, out Action<ExpressionTreeNode> func)
        {
            outType = GetHighestType(node);
            if (outType == typeof(int)) func = SubInt;
            else if (outType == typeof(float)) func = SubFloat;
            else if (outType == typeof(double)) func = SubDouble;
            else func = null;
        }
        static void MulCreator(ExpressionTreeNode node, out Type outType, out Action<ExpressionTreeNode> func)
        {
            outType = GetHighestType(node);
            if (outType == typeof(int)) func = MulInt;
            else if (outType == typeof(float)) func = MulFloat;
            else if (outType == typeof(double)) func = MulDouble;
            else func = null;
        }
        static void DivCreator(ExpressionTreeNode node, out Type outType, out Action<ExpressionTreeNode> func)
        {
            outType = GetHighestType(node);
            if (outType == typeof(int)) func = DivInt;
            else if (outType == typeof(float)) func = DivFloat;
            else if (outType == typeof(double)) func = DivDouble;
            else func = null;
        }
        static void ModCreator(ExpressionTreeNode node, out Type outType, out Action<ExpressionTreeNode> func)
        {
            outType = GetHighestType(node);
            if (outType == typeof(int)) func = ModInt;
            else if (outType == typeof(float)) func = ModFloat;
            else if (outType == typeof(double)) func = ModDouble;
            else func = null;
        }
        static void GCreator(ExpressionTreeNode node, out Type outType, out Action<ExpressionTreeNode> func)
        {
            outType = GetHighestType(node);
            if (outType == typeof(int)) func = GInt;
            else if (outType == typeof(float)) func = GFloat;
            else if (outType == typeof(double)) func = GDouble;
            else func = null;
        }
        static void GECreator(ExpressionTreeNode node, out Type outType, out Action<ExpressionTreeNode> func)
        {
            outType = GetHighestType(node);
            if (outType == typeof(int)) func = GEInt;
            else if (outType == typeof(float)) func = GEFloat;
            else if (outType == typeof(double)) func = GEDouble;
            else func = null;
        }
        static void LCreator(ExpressionTreeNode node, out Type outType, out Action<ExpressionTreeNode> func)
        {
            outType = GetHighestType(node);
            if (outType == typeof(int)) func = LInt;
            else if (outType == typeof(float)) func = LFloat;
            else if (outType == typeof(double)) func = LDouble;
            else func = null;
        }
        static void LECreator(ExpressionTreeNode node, out Type outType, out Action<ExpressionTreeNode> func)
        {
            outType = GetHighestType(node);
            if (outType == typeof(int)) func = LEInt;
            else if (outType == typeof(float)) func = LEFloat;
            else if (outType == typeof(double)) func = LEDouble;
            else func = null;
        }
        static void ECreator(ExpressionTreeNode node, out Type outType, out Action<ExpressionTreeNode> func)
        {
            outType = GetHighestType(node);
            if (outType == typeof(int)) func = EInt;
            else if (outType == typeof(float)) func = EFloat;
            else if (outType == typeof(double)) func = EDouble;
            else func = null;
        }
        static void NECreator(ExpressionTreeNode node, out Type outType, out Action<ExpressionTreeNode> func)
        {
            outType = GetHighestType(node);
            if (outType == typeof(int)) func = NEInt;
            else if (outType == typeof(float)) func = NEFloat;
            else if (outType == typeof(double)) func = NEDouble;
            else func = null;
        }
        static void NotCreator(ExpressionTreeNode node, out Type outType, out Action<ExpressionTreeNode> func)
        {
            outType = typeof(bool);
            func = NotBool;
        }
        static void AndCreator(ExpressionTreeNode node, out Type outType, out Action<ExpressionTreeNode> func)
        {
            outType = typeof(bool);
            func = AndBool;
        }
        static void OrCreator(ExpressionTreeNode node, out Type outType, out Action<ExpressionTreeNode> func)
        {
            outType = typeof(bool);
            func = OrBool;
        }
        #endregion

        #region 函数实体
        static void AddInt(ExpressionTreeNode node) => 
            node.Value.Int = node.Children[0].Value.Int + node.Children[1].Value.Int;
        static void AddFloat(ExpressionTreeNode node) => 
            node.Value.Float = node.Children[0].Value.Float + node.Children[1].Value.Float;
        static void AddDouble(ExpressionTreeNode node) => 
            node.Value.Double = node.Children[0].Value.Double + node.Children[1].Value.Double;
        static void SubInt(ExpressionTreeNode node) =>
            node.Value.Int = node.Children[0].Value.Int - node.Children[1].Value.Int;
        static void SubFloat(ExpressionTreeNode node) =>
            node.Value.Float = node.Children[0].Value.Float - node.Children[1].Value.Float;
        static void SubDouble(ExpressionTreeNode node) =>
            node.Value.Double = node.Children[0].Value.Double - node.Children[1].Value.Double;
        static void MulInt(ExpressionTreeNode node) =>
            node.Value.Int = node.Children[0].Value.Int * node.Children[1].Value.Int;
        static void MulFloat(ExpressionTreeNode node) => 
            node.Value.Float = node.Children[0].Value.Float * node.Children[1].Value.Float;
        static void MulDouble(ExpressionTreeNode node) =>
            node.Value.Double = node.Children[0].Value.Double * node.Children[1].Value.Double;
        static void DivInt(ExpressionTreeNode node) =>
            node.Value.Int = node.Children[0].Value.Int / node.Children[1].Value.Int;
        static void DivFloat(ExpressionTreeNode node) =>
            node.Value.Float = node.Children[0].Value.Float / node.Children[1].Value.Float;
        static void DivDouble(ExpressionTreeNode node) =>
            node.Value.Double = node.Children[0].Value.Double / node.Children[1].Value.Double;
        static void ModInt(ExpressionTreeNode node) =>
            node.Value.Int = node.Children[0].Value.Int % node.Children[1].Value.Int;
        static void ModFloat(ExpressionTreeNode node) =>
            node.Value.Float = node.Children[0].Value.Float % node.Children[1].Value.Float;
        static void ModDouble(ExpressionTreeNode node) =>
            node.Value.Double = node.Children[0].Value.Double % node.Children[1].Value.Double;
        static void GInt(ExpressionTreeNode node) =>
            node.Value.Bool = node.Children[0].Value.Int > node.Children[1].Value.Int;
        static void GFloat(ExpressionTreeNode node) =>
            node.Value.Bool = node.Children[0].Value.Float > node.Children[1].Value.Float;
        static void GDouble(ExpressionTreeNode node) =>
            node.Value.Bool = node.Children[0].Value.Double > node.Children[1].Value.Double;
        static void GEInt(ExpressionTreeNode node) =>
            node.Value.Bool = node.Children[0].Value.Int >= node.Children[1].Value.Int;
        static void GEFloat(ExpressionTreeNode node) =>
            node.Value.Bool = node.Children[0].Value.Float >= node.Children[1].Value.Float;
        static void GEDouble(ExpressionTreeNode node) =>
            node.Value.Bool = node.Children[0].Value.Double >= node.Children[1].Value.Double;
        static void LInt(ExpressionTreeNode node) =>
            node.Value.Bool = node.Children[0].Value.Int < node.Children[1].Value.Int;
        static void LFloat(ExpressionTreeNode node) =>
            node.Value.Bool = node.Children[0].Value.Float < node.Children[1].Value.Float;
        static void LDouble(ExpressionTreeNode node) =>
            node.Value.Bool = node.Children[0].Value.Double < node.Children[1].Value.Double;
        static void LEInt(ExpressionTreeNode node) =>
            node.Value.Bool = node.Children[0].Value.Int <= node.Children[1].Value.Int;
        static void LEFloat(ExpressionTreeNode node) =>
            node.Value.Bool = node.Children[0].Value.Float <= node.Children[1].Value.Float;
        static void LEDouble(ExpressionTreeNode node) =>
            node.Value.Bool = node.Children[0].Value.Double <= node.Children[1].Value.Double;
        static void EInt(ExpressionTreeNode node) =>
            node.Value.Bool = node.Children[0].Value.Int == node.Children[1].Value.Int;
        static void EFloat(ExpressionTreeNode node) =>
            node.Value.Bool = node.Children[0].Value.Float == node.Children[1].Value.Float;
        static void EDouble(ExpressionTreeNode node) =>
            node.Value.Bool = node.Children[0].Value.Double == node.Children[1].Value.Double;
        static void NEInt(ExpressionTreeNode node) =>
            node.Value.Bool = node.Children[0].Value.Int != node.Children[1].Value.Int;
        static void NEFloat(ExpressionTreeNode node) =>
            node.Value.Bool = node.Children[0].Value.Float != node.Children[1].Value.Float;
        static void NEDouble(ExpressionTreeNode node) =>
            node.Value.Bool = node.Children[0].Value.Double != node.Children[1].Value.Double;
        static void NotBool(ExpressionTreeNode node) =>
            node.Value.Bool = !node.Children[0].Value.Bool;
        static void AndBool(ExpressionTreeNode node) =>
            node.Value.Bool = node.Children[0].Value.Bool && node.Children[1].Value.Bool;
        static void OrBool(ExpressionTreeNode node) =>
            node.Value.Bool = node.Children[0].Value.Bool || node.Children[1].Value.Bool;
        #endregion
    }
}