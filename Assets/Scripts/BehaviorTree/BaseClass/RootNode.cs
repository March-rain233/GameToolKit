using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrame.Behavior.Tree
{
    /// <summary>
    /// 根节点
    /// </summary>
    [Node("#e84393", NodeAttribute.PortType.None, NodeAttribute.PortType.Single)]
    [NodeCategory("NULL")]
    [SerializeField]
    public class RootNode : ProcessNode
    {
        /// <summary>
        /// 子节点
        /// </summary>
        [HideInTreeInspector]
        public ProcessNode Child;

        public override ProcessNode[] GetChildren()
        {
            if (Child != null) return new ProcessNode[] { Child };
            else return new ProcessNode[] { };
        }

        protected override NodeStatus OnUpdate()
        {
            return Child.Tick();
        }

        public override Node Clone()
        {
            var n = base.Clone() as RootNode;
            n.Child = null;
            return n;
        }

        public override void AddChild(ProcessNode node)
        {
            if (Child != null)
            {
                Debug.LogError("尝试向根节点非法添加复数个节点");
                return;
            }
            Child = node;
        }
        public override void RemoveChild(ProcessNode node)
        {
            if (Child != node)
            {
                Debug.LogError($"尝试移除{Name}的{node.Name}子节点，但是{Name}已无{node.Name}子节点");
                return;
            }
            Child = null;
        }
    }
}
