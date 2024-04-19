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
    /// ��Ϊ��
    /// </summary>
    [CreateAssetMenu(fileName = "BTree", menuName = "Behavior/Behavior Tree")]
    public class BehaviorTree : DataFlowGraph<BehaviorTree, Node>
    {
        /// <summary>
        /// ���ڵ�
        /// </summary>
        [SerializeField, HideDuplicateReferenceBox, HideReferenceObjectPicker]
        private RootNode _rootNode;
        public RootNode RootNode => _rootNode;

        /// <summary>
        /// ��ǰ�������ж���
        /// </summary>
        public BehaviorTreeRunner Runner { get; private set; }

        /// <summary>
        /// ��Ϊ������״̬
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
        /// ����
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
        /// ����������
        /// </summary>
        /// <param name="runner"></param>
        /// <returns></returns>
        internal BehaviorTree CreateRunningTree(BehaviorTreeRunner runner)
        {
            var tree = Clone();
            tree.Runner = runner;

            //���ƺڰ�
            foreach(var(id, value) in runner.Variables) 
                tree.Blackboard[id, Domain.Local].Value = value.Value;

            //��ʼ���ڵ�
            tree.Init();
            return tree;
        }

        public override BehaviorTree Clone()
        {
            var tree = base.Clone();
            tree._rootNode = tree.Nodes.First(node => node is RootNode) as RootNode;
            //������Ϊ��
            foreach(var node in tree.Nodes.OfType<ProcessNode>())
                foreach(var child in (FindNode(node.Id) as ProcessNode).GetChildren())
                    node.AddChild(tree.FindNode(child.Id) as ProcessNode);
            foreach(var node in tree.Nodes)
                node.SetTree(tree);
            return tree;
        }

        /// <summary>
        /// ������Ϊ��
        /// </summary>
        internal void Enable()
        {
            if (IsEnable) return;
            IsEnable = true;
            foreach (var node in Nodes)
                node.OnEnable();
        }

        /// <summary>
        /// ������Ϊ��
        /// </summary>
        internal void Disable()
        {
            if (!IsEnable) return;
            IsEnable = false;
            foreach (var node in Nodes)
                node.OnDiable();
        }

        /// <summary>
        /// �Ƴ��ڵ�
        /// </summary>
        /// <param name="node"></param>
        public override void RemoveNode(Node node)
        {
            //�Ƴ��븸�ڵ������
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
