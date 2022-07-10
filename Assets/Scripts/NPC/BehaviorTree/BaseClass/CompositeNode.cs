using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrame.Behavior.Tree
{
    /// <summary>
    /// ��Ͻڵ�
    /// </summary>
    [Node("#ffeaa7", NodeAttribute.PortType.Single, NodeAttribute.PortType.Multi)]
    [NodeCategory("Composite")]
    public abstract class CompositeNode : Node
    {
#if UNITY_EDITOR
        public override void AddChild(Node node)
        {
            Childrens.Add(node);
        }
        public override void RemoveChild(Node node)
        {
            if (!Childrens.Contains(node))
            {
                Debug.LogWarning($"�����Ƴ�{Name}��{node.Name}�ӽڵ㣬����{Name}����{node.Name}�ӽڵ�");
                return;
            }
            Childrens.Remove(node);
        }
#endif
        /// <summary>
        /// �ýڵ��Ϸ�ʽ
        /// </summary>
        public AbortType AbortType => _abortType;
        [SerializeField]
        private AbortType _abortType = AbortType.None;

        public List<Node> Childrens = new List<Node>();

        protected override void OnAbort()
        {
            Childrens.Find(node => node.Status == NodeStatus.Running).Abort();
        }

        public override Node[] GetChildren()
        {
            return Childrens.ToArray();
        }

        protected void AbortAllRunningNode(List<Node> except)
        {
            Childrens.ForEach(child =>
            {
                if (child.Status == NodeStatus.Running && !except.Contains(child))
                {
                    child.Abort();
                }
            });
        }

        public override BaseNode Clone(BehaviorTree tree)
        {
            var node = base.Clone(tree) as CompositeNode;
            node.Childrens = new List<Node>(Childrens);
            node.Childrens.Clear();
            Childrens.ForEach((child) =>
            {
                var n = tree.Nodes.Find(node => node.Guid == child.Guid);
                if(n == null)
                {
                    n = child.Clone(tree);
                }
                node.Childrens.Add(n as Node);
            });
            return node;
        }
    }
    /// <summary>
    /// ��Ϸ�ʽ
    /// </summary>
    [System.Serializable]
    public enum AbortType
    {
        /// <summary>
        /// �����
        /// </summary>
        None,
        /// <summary>
        /// ��ϵ����ȼ��ڵ�
        /// </summary>
        LowerPriority,
        /// <summary>
        /// �������
        /// </summary>
        Self,
        /// <summary>
        /// �������͵����ȼ��ڵ�
        /// </summary>
        Both
    }
}