using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using GameFrame.Interface;
using System.Text;
using Cinemachine;

namespace GameFrame.Behavior.Tree
{
    using NodeStatus = Node.NodeStatus;
    /// <summary>
    /// 行为树
    /// </summary>
    [CreateAssetMenu(fileName = "行为树控制器", menuName = "角色/行为树控制器")]
    public class BehaviorTree : ScriptableObject, ITree, ICreateTree, INodeContainer
    {
        #region 编辑器变量
#if UNITY_EDITOR
        public Type NodeParentType => typeof(Node);

        public Type RootType => typeof(RootNode);

        public INode RootNode => _rootNode;

        /// <summary>
        /// 节点列表
        /// </summary>
        [SerializeField]
        public List<Node> Nodes = new List<Node>();

#endif
        #endregion

        /// <summary>
        /// 根节点
        /// </summary>
        [SerializeField]
        private RootNode _rootNode;

        /// <summary>
        /// 当前树的运行对象
        /// </summary>
        public BehaviorTreeRunner Runner { get; private set; }

        /// <summary>
        /// 当前树的黑板
        /// </summary>
        public BlackBoard BlackBoard { get; set; }

        /// <summary>
        /// 当前树使用的模板黑板
        /// </summary>
        public BlackBoard ModelBlackBoard { get=>_modelBlackBoard;}
        private BlackBoard _modelBlackBoard;
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="runner"></param>
        /// <returns></returns>
        public NodeStatus Tick()
        {
            if (_rootNode.Status == NodeStatus.Running ||_rootNode.Status == NodeStatus.None)
            {
                _rootNode.Tick();
            }
            return _rootNode.Status;
        }
        
        public BehaviorTree Create(BehaviorTreeRunner runner)
        {
            var tree = Instantiate(this);

            //将参与运行的节点进行复制
            tree._rootNode = _rootNode.Clone() as RootNode;
            Stack<Node> stack = new Stack<Node>(Nodes.Count);
            stack.Push(tree._rootNode);
            tree.Nodes.Clear();
            while (stack.Count > 0)
            {
                var node = stack.Pop();
                Array.ForEach(node.GetChildren(), child => stack.Push(child as Node));
                tree.Nodes.Add(node);
            }

            tree._rootNode.InitBinding(tree);
            tree.BlackBoard = ModelBlackBoard.CreateRuntimeBlackBoard(runner);
            return tree;
        }


        #region 编辑器方法
#if UNITY_EDITOR
        public void AddNode(INode node)
        {
            Nodes.Add(node as Node);
        }

        public void RemoveNode(INode node)
        {
            RemoveNode(node as Node);
        }

        public void CorrectnessChecking()
        {
            if (_rootNode == null) _rootNode = CreateNode(typeof(RootNode)) as RootNode;
            if (ModelBlackBoard == null) _modelBlackBoard = new BlackBoard(this);
        }
        /// <summary>
        /// 移除节点
        /// </summary>
        /// <param name="node"></param>
        public void RemoveNode(Node node)
        {
            Nodes.Remove(node);

            //移除与父节点的连接
            Node parent = null;
            for(int i = 0; i < Nodes.Count; ++i)
            {
                if(Nodes[i] == node) { continue; }
                parent = FindParent(node, Nodes[i]);
                if(parent != null) { break; }
            }
            DisconnectNode(parent, node);

            if (AssetDatabase.Contains(this))
            {
                AssetDatabase.RemoveObjectFromAsset(node);
                AssetDatabase.SaveAssets();
            }
        }

        /// <summary>
        /// 寻找指定节点的父节点
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public Node FindParent(Node target, Node start)
        {
            if (target == start) { return start; }
            foreach(var child in start.GetChildren())
            {
                if(child as Node == target)
                {
                    return start;
                }
                else
                {
                    var p = FindParent(target, child as Node);
                    if (p != null)
                    {
                        return p;
                    }
                }
            }
            return null;
        }

        public INode[] GetNodes()
        {
            return Nodes.ToArray();
        }

        public INode CreateNode(Type type)
        {
            var node = CreateInstance(type) as Node;

            int count = Nodes.FindAll(node => node.Name == type.Name).Count;
            string newName = type.Name;
            if (count > 0)
            {
                newName = newName + $"({count})";
            }
            node.name = newName;
            node.Guid = GUID.Generate().ToString();
            Nodes.Add(node);


            //把新建立的点保存至本地
            if (AssetDatabase.Contains(this))
            {
                AssetDatabase.AddObjectToAsset(node, this);
                AssetDatabase.SaveAssets();
            }
            return node;
        }

        public void ConnectNode(INode parent, INode child)
        {
            (parent as Node).AddChild(child as Node);
        }

        public void DisconnectNode(INode parent, INode child)
        {
            (parent as Node).RemoveChild(child as Node);
        }

        public Dictionary<Type, string> GetNodeTypeTree()
        {
            //获取节点的所有子类
            var types = TypeCache.GetTypesDerivedFrom<Node>();
            var node = typeof(Node);
            var root = typeof(RootNode);
            var res = new Dictionary<Type, string>();
            foreach (var type in types)
            {
                //排除抽象类和根节点
                if (type.IsAbstract || type.Equals(root)) continue;

                //创建类的继承路径
                Type parent = type;
                StringBuilder sb = new StringBuilder();
                while (!parent.Equals(node))
                {
                    //将父类插入路径前端
                    sb.Insert(0, '/' + parent.Name);
                    parent = parent.BaseType;
                }
                sb.Remove(0, 1);
                res.Add(type, sb.ToString());
            }
            return res;
        }
#endif
        #endregion
    }
}
