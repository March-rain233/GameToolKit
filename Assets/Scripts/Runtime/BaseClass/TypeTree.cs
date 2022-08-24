using System.Collections;
using System.Collections.Generic;
using System;

namespace GameFrame
{
    public class TypeTree
    {
        class TypeNode
        {
            public Type Type;
            public List<TypeNode> Children;
            public TypeNode Parent;
        }

        TypeNode _root;

        Dictionary<Type, TypeNode> _typeNodes = new Dictionary<Type, TypeNode>();

        public Type BaseType => _root.Type;

        public TypeTree(Type baseType)
        {
            _root = new TypeNode() 
            { 
                Type = baseType,
                Children = new List<TypeNode>()
            };
            _typeNodes.Add(baseType, _root);
        }

        /// <summary>
        /// 获取传入类型在缓存中的子代类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public HashSet<Type> GetDescendant(Type type)
        {
            //查找类型对应的节点
            TypeNode p = GetTypeNode(type);

            //输出子节点
            HashSet<Type> res = new HashSet<Type>();
            Stack<TypeNode> stack = new Stack<TypeNode>();
            stack.Push(p);
            while(stack.TryPop(out p))
            {
                foreach(TypeNode child in p.Children)
                {
                    stack.Push(child);
                }
                res.Add(p.Type);
            }

            return res;
        }

        /// <summary>
        /// 获取传入类型在缓存中的祖先类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public HashSet<Type> GetAncestor(Type type)
        {
            var p = GetTypeNode(type);
            HashSet<Type> res = new HashSet<Type>();
            while(p != null)
            {
                res.Add(p.Type);
                p = p.Parent;
            }
            return res;
        }

        TypeNode GetTypeNode(Type type)
        {
            return _typeNodes[type];
        }

        public bool TryAddType(Type type)
        {
            if (!_typeNodes.ContainsKey(type))
            {
                TypeNode p = _root;
                while (p.Type != type)
                {
                    if (p.Children.Count == 0)
                    {
                        p.Children.Add(new TypeNode() 
                        { 
                            Children = new List<TypeNode>(),
                            Type = type,
                            Parent = p
                        });
                        return true;
                    }
                    foreach (TypeNode child in p.Children)
                    {
                        if (type.IsSubclassOf(child.Type))
                        {
                            p = child;
                        }
                        else if (child.Type.IsSubclassOf(type))
                        {
                            int i = p.Children.FindIndex(x => x == child);
                            p.Children.RemoveAt(i);
                            p.Children.Insert(i, new TypeNode()
                            {
                                Children = new List<TypeNode> { child },
                                Type = type,
                                Parent = p
                            });
                            child.Parent = p.Children[i];
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
