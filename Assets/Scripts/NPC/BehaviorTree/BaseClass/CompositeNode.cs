using GameFrame.Interface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrame.Behavior.Tree
{
    /// <summary>
    /// ��Ͻڵ�
    /// </summary>
    public abstract class CompositeNode : Node
    {
        /// <summary>
        /// �ýڵ��Ϸ�ʽ
        /// </summary>
        public AbortType AbortType => _abortType;
        [SerializeField]
        private AbortType _abortType = AbortType.None;

        public List<Node> Childrens = new List<Node>();

#if UNITY_EDITOR
        public override sealed INode.PortType Input => INode.PortType.Single;
        public override sealed INode.PortType Output => INode.PortType.Multi;
        public override void AddChild(Node node)
        {
            Childrens.Add(node);
        }
        public override void RemoveChild(Node node)
        {
            if (!Childrens.Contains(node))
            {
                Debug.Log($"�����Ƴ�{Name}��{node.Name}�ӽڵ㣬����{name}����{node.Name}�ӽڵ�");
                return;
            }
            Childrens.Remove(node);
        }
#endif

        protected override void OnAbort()
        {
            Childrens.Find(node => node.Status == NodeStatus.Running).Abort();
        }

        public override INode[] GetChildren()
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

        public override Node Clone()
        {
            var node = Instantiate(this);
#if UNITY_EDITOR
            node.Guid = UnityEditor.GUID.Generate().ToString();
#endif
            node.Childrens = new List<Node>(Childrens);
            for (int i = Childrens.Count - 1; i >= 0; --i)
            {
                if (!Childrens[i])
                {
                    Childrens.RemoveAt(i);
                    node.Childrens.RemoveAt(i);
                    continue;
                }
                node.Childrens[i] = Childrens[i].Clone();
            }
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