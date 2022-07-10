using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrame.Behavior.Tree
{
    /// <summary>
    /// 组合节点
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
                Debug.LogWarning($"尝试移除{Name}的{node.Name}子节点，但是{Name}已无{node.Name}子节点");
                return;
            }
            Childrens.Remove(node);
        }
#endif
        /// <summary>
        /// 该节点打断方式
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
    /// 打断方式
    /// </summary>
    [System.Serializable]
    public enum AbortType
    {
        /// <summary>
        /// 不打断
        /// </summary>
        None,
        /// <summary>
        /// 打断低优先级节点
        /// </summary>
        LowerPriority,
        /// <summary>
        /// 打断自身
        /// </summary>
        Self,
        /// <summary>
        /// 打断自身和低优先级节点
        /// </summary>
        Both
    }
}