using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameToolKit
{
    /// <summary>
    /// 表达式词元
    /// </summary>
    public struct ExpressionWord
    {
        public delegate void FuncCreatorHandler(ExpressionTreeNode node, out Type outType, out Action<ExpressionTreeNode> func);
        /// <summary>
        /// 文本内容
        /// </summary>
        public string Context;
        /// <summary>
        /// 是否是操作符
        /// </summary>
        public bool IsOperation;
        /// <summary>
        /// 参数个数
        /// </summary>
        public int ParameterNum;
        /// <summary>
        /// 操作符实际函数生成器
        /// </summary>
        public FuncCreatorHandler FuncCreator;
    }
}
