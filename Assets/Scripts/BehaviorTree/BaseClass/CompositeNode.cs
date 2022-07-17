using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace GameFrame.Behavior.Tree
{
    /// <summary>
    /// ��Ͻڵ�
    /// </summary>
    [Node("#ffeaa7", NodeAttribute.PortType.Single, NodeAttribute.PortType.Multi)]
    [NodeCategory("Composite")]
    public abstract class CompositeNode : ProcessNode
    {
        /// <summary>
        /// �ýڵ��Ϸ�ʽ
        /// </summary>
        public AbortType AbortType => _abortType;
        [SerializeField, EnumToggleButtons]
        private AbortType _abortType = AbortType.None;
        /// <summary>
        /// �ӽڵ�
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
                Debug.LogError($"�����Ƴ�{Name}��{node.Name}�ӽڵ㣬����{Name}����{node.Name}�ӽڵ�");
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
    /// ��Ϸ�ʽ
    /// </summary>
    [System.Serializable]
    [System.Flags]
    public enum AbortType
    {
        /// <summary>
        /// �����
        /// </summary>
        None = 0,
        /// <summary>
        /// ��ϵ����ȼ��ڵ�
        /// </summary>
        LowerPriority = 1 << 0,
        /// <summary>
        /// �������
        /// </summary>
        Self = 1 << 1,
        Both = LowerPriority | Self,
    }
}