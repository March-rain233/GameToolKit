using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.Serialization;
using System.Linq;
using Sirenix.OdinInspector;

namespace GameToolKit.Behavior.Tree
{
    using NodeStatus = ProcessNode.NodeStatus;
    /// <summary>
    /// 行为树
    /// </summary>
    [CreateAssetMenu(fileName = "BTree", menuName = "Behavior/Behavior Tree")]
    public class BehaviorTree : DataFlowGraph<BehaviorTree, Node>
    {
        /// <summary>
        /// 根节点
        /// </summary>
        [SerializeField, HideDuplicateReferenceBox, HideReferenceObjectPicker]
        private RootNode _rootNode;
        public RootNode RootNode => _rootNode;

        /// <summary>
        /// 当前树的运行对象
        /// </summary>
        public BehaviorTreeRunner Runner { get; private set; }

        /// <summary>
        /// 行为树运行状态
        /// </summary>
        public bool IsEnable { get; private set; } = false;

        private void Reset()
        {
            _rootNode = CreateNode(typeof(RootNode)) as RootNode;
            Blackboard = new GraphBlackboard(true);
        }

        private void OnEnable()
        {
            Blackboard.Init();
        }

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

        /// <summary>
        /// 建造运行树
        /// </summary>
        /// <param name="runner"></param>
        /// <returns></returns>
        internal BehaviorTree CreateRunningTree(BehaviorTreeRunner runner)
        {
            var tree = Clone();
            tree.Runner = runner;

            //复制黑板
            foreach(var(id, value) in runner.Variables) 
                tree.Blackboard[id, Domain.Local].Value = value.Value;

            //初始化节点
            tree.Init();
            return tree;
        }

        public override BehaviorTree Clone()
        {
            var tree = base.Clone();
            tree._rootNode = tree.Nodes.First(node => node is RootNode) as RootNode;
            //连接行为边
            foreach(var node in tree.Nodes.OfType<ProcessNode>())
                foreach(var child in (FindNode(node.Id) as ProcessNode).GetChildren())
                    node.AddChild(tree.FindNode(child.Id) as ProcessNode);
            foreach(var node in tree.Nodes)
                node.SetTree(tree);
            return tree;
        }

        /// <summary>
        /// 启用行为树
        /// </summary>
        internal void Enable()
        {
            if (IsEnable) return;
            IsEnable = true;
            foreach (var node in Nodes)
                node.OnEnable();
        }

        /// <summary>
        /// 禁用行为树
        /// </summary>
        internal void Disable()
        {
            if (!IsEnable) return;
            IsEnable = false;
            foreach (var node in Nodes)
                node.OnDiable();
        }

        /// <summary>
        /// 移除节点
        /// </summary>
        /// <param name="node"></param>
        public override void RemoveNode(Node node)
        {
            //移除与父节点的连接
            var n = node as ProcessNode;
            if (n != null)
            {
                foreach (var item in Nodes)
                {
                    var p = item as ProcessNode;
                    if (p != null)
                    {
                        var children = p.GetChildren();
                        if (children.Contains(node)) p.RemoveChild(n);
                    }
                }
            }
            base.RemoveNode(node);
        }

        public override Node CreateNode(Type type)
        {
            var node = base.CreateNode(type);
            typeof(Node).GetProperty("BehaviorTree").SetValue(node, this);
            return node;
        }
    }
}
