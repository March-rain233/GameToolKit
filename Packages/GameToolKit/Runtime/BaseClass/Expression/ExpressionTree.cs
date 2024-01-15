using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameToolKit
{
    public class ExpressionTree
    {
        /// <summary>
        /// 表达式结果
        /// </summary>
        public object FinalValue
        {
            get
            {
                if (_head.Type.IsClass) return _head.Value.Object;
                if (_head.Value.Object is int) return _head.Value.Int;
                else if (_head.Value.Object is float) return _head.Value.Float;
                else if (_head.Value.Object is double) return _head.Value.Double;
                else if (_head.Value.Object is bool) return _head.Value.Bool;
                return null;
            }
        }

        /// <summary>
        /// 脏标记
        /// </summary>
        public bool IsDirty => _dirtyQueue.Count > 0;

        ExpressionTreeNode _head;
        /// <summary>
        /// 单词-索引映射
        /// </summary>
        Dictionary<string, List<ExpressionTreeNode>> _word2Index;
        /// <summary>
        /// 待更新队列
        /// </summary>
        SortedSet<ExpressionTreeNode> _dirtyQueue;

        /// <summary>
        /// 输入后缀表达式生成表达式树结构
        /// </summary>
        /// <param name="expression"></param>
        public ExpressionTree(IEnumerable<ExpressionWord> expression, Dictionary<string, (Type, object)> values)
        {
            _word2Index = new Dictionary<string, List<ExpressionTreeNode>>();
            _dirtyQueue = new SortedSet<ExpressionTreeNode>();
            var stack = new Stack<ExpressionTreeNode>();
            foreach(var word in expression)
            {
                if (word.IsOperation)
                {
                    var node = new ExpressionTreeNode()
                    {
                        Parent = null,
                        Children = new ExpressionTreeNode[word.ParameterNum],
                    };
                    _head = node;
                    for(int i = 0; i < word.ParameterNum; i++)
                    {
                        node.Children[i] = stack.Pop();
                        node.Children[i].Parent = node;
                    }
                    word.FuncCreator(node, out node.Type, out node.Function);
                }
                else
                {
                    var node = new ExpressionTreeNode()
                    {
                        Parent = null,
                        Children = new ExpressionTreeNode[0],
                        Type = values[word.Context].Item1
                    };
                    if(_word2Index.TryGetValue(word.Context, out var children))
                    {
                        children.Add(node);
                    }
                    else
                    {
                        _word2Index[word.Context] = new List<ExpressionTreeNode> { node };
                    }
                    stack.Push(node);
                }
            }
            SetLayer(_head, 1);
            foreach(var value in values)
            {
                SetValue(value.Key, value.Value.Item2);
            }
        }

        /// <summary>
        /// 设置层数
        /// </summary>
        /// <param name="node"></param>
        /// <param name="layer"></param>
        static void SetLayer(ExpressionTreeNode node, int layer)
        {
            node.Layer = layer;
            foreach(var child in node.Children)
            {
                SetLayer(child, layer + 1);
            }
        }

        /// <summary>
        /// 刷新
        /// </summary>
        /// <remarks>
        /// 向上更新表达式树上的脏节点
        /// </remarks>
        public void Refresh()
        {
            foreach(var node in _dirtyQueue)
            {
                if (node.Type.IsClass) continue;
                if(node.Value.Object is int) node.Value.Int = (int)node.Value.Object;
                else if(node.Value.Object is float) node.Value.Float = (float)node.Value.Object;
                else if(node.Value.Object is double) node.Value.Double = (double)node.Value.Object;
                else if(node.Value.Object is bool) node.Value.Bool = (bool)node.Value.Object;
            }
            while(_dirtyQueue.Count > 0)
            {
                var node =_dirtyQueue.First();
                _dirtyQueue.Remove(node);
                if(node.Function != null) node.Function(node);
                if(node.Parent != null) _dirtyQueue.Add(node.Parent);
            }
        }

        /// <summary>
        /// 设置节点值
        /// </summary>
        /// <param name="word"></param>
        /// <param name="value"></param>
        public void SetValue(string word, object value)
        {
            foreach(var node in _word2Index[word])
            {
                node.Value.Object = value;
                _dirtyQueue.Add(node);
            }
        }
    }
}
