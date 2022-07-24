using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace GameFrame.Behavior.Tree
{
    /// <summary>
    /// 组合节点
    /// </summary>
    [Node(NodeAttribute.PortType.Single, NodeAttribute.PortType.Multi)]
    [NodeCategory("Composite")]
    [NodeColor("#ffeaa7")]
    public abstract class CompositeNode : ProcessNode
    {
        /// <summary>
        /// 该节点打断方式
        /// </summary>
        public AbortType AbortType => _abortType;
        [SerializeField, EnumToggleButtons]
        private AbortType _abortType = AbortType.None;
        /// <summary>
        /// 子节点
        /// </summary>
        [HideInTreeInspector]
        public List<ProcessNode> Childrens = new List<ProcessNode>();
        public override void AddChild(ProcessNode node)
        {
            Childrens.Add(node);
        }
        public override void RemoveChild(ProcessNode node)
        {
            if (!Childrens.Contains(node))
            {
                Debug.LogError($"尝试移除{Name}的{node.Name}子节点，但是{Name}已无{node.Name}子节点");
                return;
            }
            Childrens.Remove(node);
        }
        protected override void OnAbort()
        {
            Childrens.Find(node => node.Status == NodeStatus.Running).Abort();
        }
        public override ProcessNode[] GetChildren()
        {
            return Childrens.ToArray();
        }
        protected void AbortAllRunningNode(List<ProcessNode> except)
        {
            Childrens.ForEach(child =>
            {
                if (child.Status == NodeStatus.Running && !except.Contains(child))
                {
                    child.Abort();
                }
            });
        }

        public override Node Clone()
        {
            var n = base.Clone() as CompositeNode;
            n.Childrens = new List<ProcessNode>();
            return n;
        }
    }
    /// <summary>
    /// 打断方式
    /// </summary>
    [System.Serializable]
    [System.Flags]
    public enum AbortType
    {
        /// <summary>
        /// 不打断
        /// </summary>
        None = 0,
        /// <summary>
        /// 打断低优先级节点
        /// </summary>
        LowerPriority = 1 << 0,
        /// <summary>
        /// 打断自身
        /// </summary>
        Self = 1 << 1,
        Both = LowerPriority | Self,
    }
}