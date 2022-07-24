using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrame.Behavior.Tree
{
    /// <summary>
    /// 修饰节点
    /// </summary>
    [Node(NodeAttribute.PortType.Single, NodeAttribute.PortType.Single)]
    [NodeCategory("Decorator")]
    [NodeColor("#00b894")]
    public abstract class DecoratorNode : ProcessNode
    {
        /// <summary>
        /// 子节点
        /// </summary>
        [HideInTreeInspector]
        public ProcessNode Child;

        protected override void OnAbort()
        {
            if (Child.Status == NodeStatus.Running) 
            {
                Child.Abort();
            }
        }

        public override ProcessNode[] GetChildren()
        {
            if (Child != null)
            {
                return new ProcessNode[] { Child };
            }
            else return new ProcessNode[] { };
        }

        public override void AddChild(ProcessNode node)
        {
            if (Child != null)
            {
                Debug.LogError("尝试向装饰节点非法添加复数个节点");
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

        public override Node Clone()
        {
            var n = base.Clone() as DecoratorNode;
            n.Child = null;
            return n;
        }
    }
}
